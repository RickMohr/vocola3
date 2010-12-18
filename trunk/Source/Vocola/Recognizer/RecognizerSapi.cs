using Microsoft.Win32; // Registry
using SpeechLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Xml;
using System.Windows.Forms;
using System.Globalization;

namespace Vocola
{

    public class RecognizerSapi : Recognizer
    {
        private string GrammarsFolder;
        static private  int DictationGrammarId = 1;
        static private int CommandGrammarId   = 2;

        private SpSharedRecoContext TheRecognizer;
        public ISpeechRecoGrammar CommandGrammar;
        private ActiveCommands TheActiveCommands;
        private ISpeechRecoResult CurrentResult;

        static private int NGlobalCommandDummies;
        static private bool StoreGrammarFiles;
        static private int PriorityOfAppCommands;
        static private int PriorityOfAppWildcardCommands;
        static private int PriorityOfGlobalCommands;
        static private int PriorityOfGlobalWildcardCommands;
        static private int PriorityOfSequencedCommands;
        static private int PriorityOfInternalCommands;
        static private float DictationWeightForCommandSequences;

        public override void Initialize()
        {
            GrammarsFolder = Path.Combine(Vocola.AppDataFolder, "Grammars");
            ReadRegistry();
            TheRecognizer = new SpeechLib.SpSharedRecoContext();
            TheRecognizer.SoundStart += new _ISpeechRecoContextEvents_SoundStartEventHandler(SpeechDetected);
            TheRecognizer.Recognition += new _ISpeechRecoContextEvents_RecognitionEventHandler(SpeechRecognized);
            TheRecognizer.FalseRecognition += new _ISpeechRecoContextEvents_FalseRecognitionEventHandler(SpeechRecognitionRejected);
            TheRecognizer.RecognitionForOtherContext += new _ISpeechRecoContextEvents_RecognitionForOtherContextEventHandler(RecognitionForOtherContext);

            Dictation.Initialize(TheRecognizer, this, DictationGrammarId);
            LaunchWsrMonitorThread();
        }

        static private void ReadRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Path.Combine(Vocola.RegistryKeyName, "SapiRecognizer"));
            Dictation.MaxAlternates = (uint)(int)key.GetValue("MaxCorrectionChoices", 20);
            NGlobalCommandDummies = (int)key.GetValue("NGlobalCommandDummies", 1000);
            StoreGrammarFiles = ((int)key.GetValue("StoreGrammarFiles", 0)) > 0;
            PriorityOfAppCommands = (int)key.GetValue("PriorityOfAppCommands", 126);
            PriorityOfAppWildcardCommands = (int)key.GetValue("PriorityOfAppWildcardCommands", 124);
            PriorityOfGlobalCommands = (int)key.GetValue("PriorityOfGlobalCommands", 122);
            PriorityOfGlobalWildcardCommands = (int)key.GetValue("PriorityOfGlobalWildcardCommands", 120);
            PriorityOfSequencedCommands = (int)key.GetValue("PriorityOfSequencedCommands", 118);
            PriorityOfInternalCommands = (int)key.GetValue("PriorityOfInternalCommands", 116);
            DictationWeightForCommandSequences = (float)key.GetValue("DictationWeightForCommandSequences", 1E-10F);
        }

        public override bool RunDevelopmentVersionFromProgramFiles { get { return true; } }

        // ---------------------------------------------------------------------
        // Exit gracefully if WSR dies

        private void LaunchWsrMonitorThread()
        {
            Thread thread = new Thread(MonitorWsr);
            thread.Name = "WSR Monitor Thread";
            thread.Start();
        }

        private void MonitorWsr()
        {
            while (true)
            {
                try
                {
                    TheRecognizer.SetAdaptationData("");
                    Thread.Sleep(500);
                }
                catch
                {
                    Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
                    {
                        Vocola.TrayIcon.Exit();
                    });
                }
            }
        }

        // ---------------------------------------------------------------------
        // Entry points

        public override void ContextChanged(VocolaContext context)
        {
            try
            {
                TheRecognizer.Pause();
            }
            catch (Exception e)
            {
                // Exception seems to happen when system comes up after being asleep
                Trace.WriteLine(LogLevel.High, "Exception pausing recognizer: {0}", e.Message);
            }
            try
            {
                ReallyUpdateGrammars(context);
                Trace.WriteLine(LogLevel.Low, "Done building grammar");
            }
            catch (Exception e)
            {
                Trace.LogUnexpectedException(e);
            }
            finally
            {
                TheRecognizer.Resume();
            }
        }

        public override void DisplayMessage(string message, bool isWarning)
        {
            if (CurrentResult != null)
                ((ISpeechRecoResult2)CurrentResult).SetTextFeedback(message, !isWarning);            
        }

        public override void EmulateRecognize(string words)
        {
            if (!EmulateRecognizeDepth.Initialized())
                throw new ActionException(null, "EmulateRecognize called in illegal context");
            if (EmulateRecognizeDepth.Get() > 5)
                throw new ActionException(null, "EmulateRecognize nested too deeply");

            // Package info for communication with SAPI event handler thread
            EmulateRecognizeFrame frame = new EmulateRecognizeFrame();
            frame.Words = words;
            frame.WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            EmulateRecognizeQueue.Enqueue(frame);

            // Emulate recognition
            EmulateRecognizeDepth.Increment();
            object dummy = SpeechDisplayAttributes.SDA_No_Trailing_Space; // ignored
            TheRecognizer.Recognizer.EmulateRecognition(words, ref dummy, 0);

            // Wait for recognition to finish, and see what happened.
            frame.WaitHandle.WaitOne();
            if (frame.ShouldAbort)
                throw new InternalException("Aborting EmulateRecognize");
            if (frame.ActionsQueue != null)
                ActionRunner.RunActions(frame.ActionsQueue);
            EmulateRecognizeDepth.Decrement();
        }

        // For communication between EmulateRecognize(words) and SAPI event handler thread
        private class EmulateRecognizeFrame
        {
            public string Words;
            public EventWaitHandle WaitHandle;
            public bool ShouldAbort = false;
            public ActionsQueue ActionsQueue;
        }

        // This is a queue instead of a single member for the unlikely case of executing 2 commands at once
        Queue<EmulateRecognizeFrame> EmulateRecognizeQueue = new Queue<EmulateRecognizeFrame>();

        private bool EmulatingRecognition()
        {
            return (EmulateRecognizeQueue.Count > 0);
        }

        // ---------------------------------------------------------------------
        // Build and reload grammars

        private void ReallyUpdateGrammars(VocolaContext context)
        {
            // Make sure GrammarsFolder exists
            if (!Directory.Exists(GrammarsFolder))
            {
                try
                {
                    Directory.CreateDirectory(GrammarsFolder);
                }
                catch (Exception e)
                {
                    Trace.WriteLine(LogLevel.Error, "Could not create grammars folder '{0}'", GrammarsFolder);
                    Trace.WriteLine(LogLevel.Error, e.Message);
                    return;
                }
            }
            // Create a grammar with commands appropriate for the current context
            TheActiveCommands = new ActiveCommands(context);
            if (TheActiveCommands.Commands.Count > 0)
            {
                bool enableCommandSequences = (Vocola.CommandSequencesEnabled && Vocola.MaxSequencedCommands > 1);
                Dictation.SetWeight(enableCommandSequences ? DictationWeightForCommandSequences : 0.9F); // If above .95 it trumps WSR dictation
                SapiGrammar grammar = BuildGrammar(enableCommandSequences);
                LoadGrammar(grammar, enableCommandSequences);
                Dictation.UpdateActiveText();
            }
        }

        private void LoadGrammar(SapiGrammar grammar, bool enableCommandSequences)
        {
            try
            {
                ISpeechRecoGrammar newGrammar = LoadGrammar(grammar, CommandGrammarId, Win.GetForegroundAppName() + ".xml", "");
                newGrammar.CmdSetRuleIdState(0, SpeechRuleState.SGDSActive);

                ISpRecoGrammar2 newGrammar2 = (ISpRecoGrammar2)newGrammar;
                newGrammar2.SetRulePriority("app"           , 0, PriorityOfAppCommands);
                newGrammar2.SetRulePriority("appWildcard"   , 0, PriorityOfAppWildcardCommands);
                newGrammar2.SetRulePriority("global"        , 0, PriorityOfGlobalCommands);
                newGrammar2.SetRulePriority("globalWildcard", 0, PriorityOfGlobalWildcardCommands);
                if (enableCommandSequences)
                    newGrammar2.SetRulePriority("sequence", 0, PriorityOfSequencedCommands);
                // Our (one) internal command gets lower priority.
                // Otherwise, saying "back 2" inserts "back to" if a thunderbird message
                // you're writing contains "back to".
                newGrammar2.SetRulePriority("internal", 0, PriorityOfInternalCommands);

                if (CommandGrammar != null)
                    CommandGrammar.Reset(0);
                CommandGrammar = newGrammar;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Trace.WriteLine(LogLevel.Error, "Exception creating SAPI grammar: {0}", SapiErrors.GetErrorMessage(ex.Message));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.Error, "Exception creating SAPI grammar: {0}", ex.Message);
            }
        }

        private ISpeechRecoGrammar LoadGrammar(SapiGrammar grammar, int grammarId, string grammarFilename, string uri)
        {
            // There's no way to load SAPI XML from memory, so write it out and load it from the file
            string grammarPathname = Path.Combine(GrammarsFolder, grammarFilename);
            using (StreamWriter sw = new StreamWriter(grammarPathname, false, System.Text.Encoding.GetEncoding(1252)))
            {
                sw.WriteLine("<?xml version='1.0' encoding='Windows-1252' ?>");
                string xml = grammar.GetXml();
                sw.Write(xml);
            }
            ISpeechRecoGrammar newGrammar = TheRecognizer.CreateGrammar(grammarId);
            ((ISpRecoGrammar2)newGrammar).LoadCmdFromFile2(grammarPathname, SPLOADOPTIONS.SPLO_STATIC, uri, "");
            if (!StoreGrammarFiles)
                File.Delete(grammarPathname);
            return newGrammar;
        }

        private SapiGrammar BuildGrammar(bool enableCommandSequences)
        {
            SapiGrammar grammar = new SapiGrammar();
            grammar.Add(new SapiDictationInCommandRule());

            // Add each variable's rule
            foreach (Variable variable in TheActiveCommands.Variables.Values)
            {
                if (variable.Rule == null)
                    variable.Rule = CreateRuleForVariable(variable);
                grammar.Add(variable.Rule as SapiRule);
            }

            // Add each command's rule (note they are not public)
            foreach (Command command in TheActiveCommands.Commands.Values)
            {
                if (command.Rule == null)
                    command.Rule = CreateRuleForCommand(command);
                SapiRule rule = command.Rule as SapiRule;
                grammar.Add(rule);
            }

            // Make a public rule for all global commands and one for all app commands.
            // This avoids three SAPI bugs with command sequences and large grammars:
            // 1) If each command has its own public rule, e.g. "Line Start Line End" is recognized as "Line Start".
            // 2) If each command has its own public rule, adjusting their priorities with SetRulePriority() takes forever.
            // 3) If only the sequence rule is public, e.g. "Cap That" won't override the MS Speech command.

            SapiChoices appChoices            = new SapiChoices();
            SapiChoices appWildcardChoices    = new SapiChoices();
            SapiChoices globalChoices         = new SapiChoices();
            SapiChoices globalWildcardChoices = new SapiChoices();
            SapiChoices internalChoices       = new SapiChoices();
            foreach (Command command in TheActiveCommands.Commands.Values)
            {
                SapiRuleRef ruleRef = new SapiRuleRef(command.Rule as SapiRule);
                if (command.IsInternal)          internalChoices.Add(ruleRef);
                else if (command.IsGlobal)
                    if (command.HasWildcardTerm) globalWildcardChoices.Add(ruleRef);
                    else                         globalChoices        .Add(ruleRef);
                else
                    if (command.HasWildcardTerm) appWildcardChoices.Add(ruleRef);
                    else                         appChoices        .Add(ruleRef);
            }

            // Simplify code by ensuring there's at least one command in each category
            SapiText dummyWord = new SapiText("zq");
            if (appChoices           .Count == 0) appChoices           .Add(dummyWord);
            if (appWildcardChoices   .Count == 0) appWildcardChoices   .Add(dummyWord);
            if (globalChoices        .Count == 0) globalChoices        .Add(dummyWord);
            if (globalWildcardChoices.Count == 0) globalWildcardChoices.Add(dummyWord);
            if (internalChoices      .Count == 0) internalChoices      .Add(dummyWord);

            SapiRule appRule            = CreateChoicesRule(grammar, "app"           , appChoices           , true);
            SapiRule appWildcardRule    = CreateChoicesRule(grammar, "appWildcard"   , appWildcardChoices   , true);
            SapiRule globalRule         = CreateChoicesRule(grammar, "global"        , globalChoices        , true);
            SapiRule globalWildcardRule = CreateChoicesRule(grammar, "globalWildcard", globalWildcardChoices, true);
            SapiRule internalRule       = CreateChoicesRule(grammar, "internal"      , internalChoices      , true);

            // Create rule for command sequences.
            // Note internal commands aren't sequenced because of this one:
            //     <_itemInWindow> = HearCommand('Insert $1');
            // If sequenced it can cause an infinite loop
            // if there's an "Insert" menu (as in Word, PowerPoint, Wordpad)

            if (enableCommandSequences)
                CreateSequenceRule(grammar, appRule, appWildcardRule, globalRule, globalWildcardRule);
            return grammar;
        }

        private void CreateSequenceRule(SapiGrammar grammar, SapiRule appRule   , SapiRule appWildcardRule,
                                                             SapiRule globalRule, SapiRule globalWildcardRule)
        {
            // I've found no declarative way to make MS Speech favor an application-specific command over a global
            // command when both are possible choices in a command sequence. But MS Speech does favor commands with
            // fewer alternatives (Mike Plumpe said so). So, the strange but effective hack is to "complexify" the
            // global commands rule by adding dummy alternatives. This will work as long as nobody has a very complex
            // local command overriding a very simple global command.

            SapiRule globalRuleComplexified         = CreateComplexifiedRule(grammar, globalRule        , "globalComplexified");
            SapiRule globalWildcardRuleComplexified = CreateComplexifiedRule(grammar, globalWildcardRule, "globalWildcardComplexified");

            SapiChoices commandChoices = new SapiChoices();
            commandChoices.Add(new SapiRuleRef(appRule));
            commandChoices.Add(new SapiRuleRef(appWildcardRule));
            commandChoices.Add(new SapiRuleRef(globalRuleComplexified));
            commandChoices.Add(new SapiRuleRef(globalWildcardRuleComplexified));
            SapiRule commandsRule = CreateChoicesRule(grammar, "commands", commandChoices, false);

            // Allow a sequence of commands to be spoken
            SapiChoices sequenceChoices = new SapiChoices();
            for (int n = 2; n <= Vocola.MaxSequencedCommands; n++)
            {
                SapiSequence sequence = new SapiSequence();
                for (int i = 1; i <=n; i++)
                    sequence.Add(new SapiRuleRef(commandsRule));
                sequenceChoices.Add(sequence);
            }
            CreateChoicesRule(grammar, "sequence", sequenceChoices, true);
        }

        private SapiRule CreateComplexifiedRule(SapiGrammar grammar, SapiRule rule, string ruleName)
        {
            SapiChoices complexifiedChoices = new SapiChoices();
            complexifiedChoices.Add(new SapiRuleRef(rule));
            SapiText dummyWord = new SapiText("zq");
            for (int i = 0; i <= NGlobalCommandDummies; i++)
                complexifiedChoices.Add(dummyWord);
            return CreateChoicesRule(grammar, ruleName, complexifiedChoices, false);
        }

        private SapiRule CreateChoicesRule(SapiGrammar grammar, string ruleName, SapiChoices choices, bool isPublic)
        {
            SapiRule rule = new SapiRule(ruleName, isPublic, choices);
            grammar.Add(rule);
            return rule;
        }

        // ---------------------------------------------------------------------
        // Create SRGS rules for Vocola commands and variables

        private SapiRule CreateRuleForVariable(Variable variable)
        {
            SapiChoices choices = GetMenuChoices(variable.Menu);
            SapiRule rule = new SapiRule(variable.UniqueId, false, choices);
            return rule;
        }

        private SapiRule CreateRuleForCommand(Command command)
        {
            string ruleId = command.UniqueId;
            List<SapiElement> elements = GetCommandElements(command);
            SapiRule rule = new SapiRule(ruleId, false, elements);
            return rule;
        }

        private SapiChoices GetMenuChoices(MenuTerm menu)
        {
            FlattenMenu(menu);
            SapiChoices choices = new SapiChoices();
            int i = 0;
            foreach (Command command in menu.Alternatives)
            {
                List<SapiElement> elements = GetCommandElements(command);
                if (elements.Count == 1 && elements[0] is SapiText && !(elements[0] as SapiText).IsOptional)
                {
                    SapiText textElement = elements[0] as SapiText;
                    textElement.Id = i++;
                    choices.Add(textElement);
                }
                else
                {
                    SapiSequence sequence = new SapiSequence(elements);
                    sequence.Id = i++;
                    choices.Add(sequence);
                }
            }
            return choices;
        }

        private List<SapiElement> GetCommandElements(Command command)
        {
            List<SapiElement> elements = new List<SapiElement>();
            foreach (object term in command.Terms)
                AddTermElements(term, elements);
            return elements;
        }

        private void AddTermElements(object term, List<SapiElement> elements)
        {
            if (term is WordTerm)
            {
                WordTerm wt = (WordTerm)term;
                string[] words = wt.Text.Split(' ');
                if (TermHasAlternates(words))
                    AddElementsForWordsWithAlternates(words, wt.IsOptional, elements);
                else
                    elements.Add(new SapiText(wt.Text, wt.IsOptional));
            }
            else if (term is VariableTerm)
            {
                VariableTerm vt = (VariableTerm)term;
                if (vt.Variable == null)
                    // Reference to unknown variable; disable command by including unpronounceable word
                    elements.Add(new SapiText("asdfghjkl"));
                else if (vt.Variable.IsReserved)
                    elements.Add(new SapiRuleRef(vt.Name));
                else 
                    elements.Add(new SapiRuleRef(vt.Variable.Rule as SapiRule));
            }
            else if (term is RangeTerm)
            {
                RangeTerm rt = (RangeTerm)term;
                SapiChoices choices = new SapiChoices();
                for (int j = rt.From; j <= rt.To; j++)
                    choices.Add(new SapiText(j.ToString(), j - rt.From));
                elements.Add(choices);
            }
            else if (term is MenuTerm)
            {
                elements.Add(GetMenuChoices(term as MenuTerm));
            }
        }

        private void AddElementsForWordsWithAlternates(string[] words, bool isOptional, List<SapiElement> elements)
        {
            List<SapiElement> wordElements = new List<SapiElement>();
            foreach (string word in words)
            {
                if (TermHasAlternates(word))
                {
                    SapiChoices choices = new SapiChoices();
                    choices.Add(new SapiText(word));
                    foreach (string termAlternate in GetAlternates(word))
                        choices.Add(new SapiText(termAlternate));
                    wordElements.Add(choices);
                }
                else
                    wordElements.Add(new SapiText(word));
            }
            if (isOptional)
            {
                SapiOptional optional = new SapiOptional();
                foreach (SapiElement e in wordElements)
                    optional.Add(e);
                elements.Add(optional);
            }
            else
                foreach (SapiElement e in wordElements)
                    elements.Add(e);
        }

        // ---------------------------------------------------------------------
        // Recognition

        private void SpeechDetected(int streamNumber, object streamPosition)
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Recognition events";
            if (!EmulatingRecognition())
            {
                Trace.InitializeTimer();
                Trace.WriteSeparator();
                Trace.WriteLine(LogLevel.Low, "Speech detected");
                //Trace.WriteLine(LogLevel.Low, "{0} active rules", TheRecognizer.Recognizer.Status.NumberOfActiveRules);
            }
        }

        private void SpeechRecognized(int streamNumber, 
                                      object streamPosition, 
                                      SpeechRecognitionType recognitionType,
                                      ISpeechRecoResult result)
        {
            CurrentResult = result;
            ISpeechPhraseInfo phraseInfo = result.PhraseInfo;
            //PrintResults(0, phraseInfo);
            if (!EmulatingRecognition())
            {
                try
                {
                    string text = phraseInfo.GetText(0, -1, true);
                    if ((decimal)phraseInfo.GrammarId == DictationGrammarId)
                    {
                        // Dictation
                        Trace.WriteLine(LogLevel.High, "Dictation: '{0}'", text);
                        Dictation.HandleDictatedPhrase(text, result);
                    }
                    else
                    {
                        // Command(s) -- run in a separate thread
                        Trace.WriteLine(LogLevel.High, "Command: '{0}'", text);
                        ActionRunner.Launch(GetRecognizedCommands(phraseInfo));
                    }
                }
                catch (Exception ex)
                {
                    Trace.LogExecutionException(ex);  // Unexpected
                }
            }
            else 
            {
                // EmulateRecognize
                if (phraseInfo.Rule.Name == null)
                    // For some reason we're called if a MS Speech command is unavailable in current context
                    HandleFailedEmulation(result);
                else
                {
                    Trace.WriteLine(LogLevel.Low, "EmulateRecognize");
                    EmulateRecognizeFrame frame = EmulateRecognizeQueue.Dequeue();
                    try
                    {
                        // Signal waiting EmulateRecognize() call to run commands
                        frame.ActionsQueue = GetRecognizedCommands(phraseInfo);
                        frame.WaitHandle.Set();
                    }
                    catch (Exception ex)
                    {
                        Trace.LogExecutionException(ex);  // Unexpected
                        frame.ShouldAbort = true;
                        frame.WaitHandle.Set();
                    }
                }
            }
        }

        private void HandleFailedEmulation(ISpeechRecoResult result)
        {
            try
            {
                EmulateRecognizeFrame frame = EmulateRecognizeQueue.Dequeue();
                if (frame.Words.StartsWith("Insert "))
                {
                    // MS Speech failed to handle "Insert" command (probably because it hasn't deigned
                    // to enable dictation for this window), so do it ourselves
                    string text = frame.Words.Substring(7);
                    Trace.WriteLine(LogLevel.High, "Inserting as dictation: '{0}'", text);
                    Dictation.HandleDictatedPhrase(text, null);
                }
                else
                {
                    Trace.WriteLine(LogLevel.High, "EmulateRecognize() failed to recognize '{0}'", frame.Words);
                    ((ISpeechRecoResult2)result).SetTextFeedback(String.Format("'{0}' not recognized", frame.Words), false);
                }
                frame.WaitHandle.Set();  // Signal EmulateRecognize() to continue
            }
            catch (Exception ex)
            {
                Trace.LogExecutionException(ex);  // Unexpected
            }
        }

        private void SpeechRecognitionRejected(int streamNumber, object streamPosition, ISpeechRecoResult result)
        {
            if (EmulatingRecognition())
                HandleFailedEmulation(result);
            else
                Trace.WriteLine(LogLevel.High, "Recognition failed");
        }

        private void RecognitionForOtherContext(int streamNumber, object streamPosition)
        {
            if (EmulatingRecognition())
            {
                Trace.WriteLine(LogLevel.Medium, "EmulateRecognize(): Non-Vocola recognition");
                EmulateRecognizeFrame frame = EmulateRecognizeQueue.Dequeue();
                frame.WaitHandle.Set();  // Signal EmulateRecognize() to continue
            }
            else
            {
                Trace.WriteLine(LogLevel.High, "Non-Vocola recognition");
                Dictation.Clear(); // Vocola dictation is probably now stale
            }
        }

        private void PrintResults(LogLevel level, ISpeechPhraseInfo phraseInfo)
        {
            Trace.WriteLine(level, "Rules:");
            PrintRule(level, phraseInfo.Rule, 1);
            Trace.WriteLine(level, "Properties:");
            PrintProperties(level, phraseInfo.Properties, 1);
        }

        private void PrintRule(LogLevel level, ISpeechPhraseRule rule, int indentLevel)
        {
            string indent = new String(' ', indentLevel * 3);
            Trace.WriteLine(level, "{0}{1} ({2}+{3})", indent, rule.Name, rule.FirstElement, rule.NumberOfElements);
            if (rule.Children != null)
                for (int i = 0; i < rule.Children.Count; i++)
                    PrintRule(level, rule.Children.Item(i), indentLevel + 1);
        }

        private void PrintProperties(LogLevel level, ISpeechPhraseProperties properties, int indentLevel)
        {
            string indent = new String(' ', indentLevel * 3);
            if (properties != null)
                for (int i = 0; i < properties.Count; i++)
                {
                    ISpeechPhraseProperty property = properties.Item(i);
                    Trace.WriteLine(level, "{0} Id '{1}' ({2}+{3})", indent,
                                    property.Id, property.FirstElement, property.NumberOfElements);
                    PrintProperties(level, property.Children, indentLevel + 1);
                }
        }

        // ---------------------------------------------------------------------
        // Which commands and alternatives were actually spoken?

        private int PropertyIndex;

        private ActionsQueue GetRecognizedCommands(ISpeechPhraseInfo phraseInfo)
        {
            PropertyIndex = 0;
            ActionsQueue actionsQueue = new ActionsQueue();
            GetRecognizedCommands(actionsQueue, phraseInfo.Rule, phraseInfo);
            return actionsQueue;
        }

        private void GetRecognizedCommands(ActionsQueue actionsQueue, ISpeechPhraseRule rule, ISpeechPhraseInfo phraseInfo)
        {
            Command command = TheActiveCommands.GetCommand(rule.Name);
            if (command == null)
            {
                // Drill down through "grouping" rules to find actual command(s)
                if (rule.Children == null)
                {
                    PrintResults(0, phraseInfo);
                    throw new InternalException("A branch of the rule result tree contained no command");
                }
                foreach (ISpeechPhraseRule childRule in rule.Children)
                    GetRecognizedCommands(actionsQueue, childRule, phraseInfo);
            }
            else
            {
                // Found a command, get its actions
                Trace.WriteLine(LogLevel.Medium, "  Executing {0}:  {1}", command.UniqueId, command);
                List<ArrayList> variableTermActions = GetVariableTermActions(command, rule, phraseInfo);
                actionsQueue.AddActions(command.Actions, variableTermActions);
            }
        }

        private List<ArrayList> GetVariableTermActions(Command command, ISpeechPhraseRule rule, ISpeechPhraseInfo phraseInfo)
        {
            List<ArrayList> variableTermActions = new List<ArrayList>();
            int variableTermIndex = 1;
            Queue<ISpeechPhraseRule> specialChildRules = GetSpecialChildRules(rule);
            foreach (object term in command.Terms)
            {
                if (term is MenuTerm || term is RangeTerm || term is VariableTerm)
                {
                    string variableTermDisplay;
                    ArrayList actions;
                    VariableTerm vt = term as VariableTerm;
                    if (vt != null && vt.Variable.IsReserved)
                    {
                        string text;
                        if (vt.Name == "_vocolaDictation")
                        {
                            int elementIndex = phraseInfo.Properties.Item(PropertyIndex++).FirstElement;
                            text = phraseInfo.Elements.Item(elementIndex).DisplayText;
                        }
                        else
                            text = GetSpokenWordsForSpecialVariable(specialChildRules, phraseInfo);
                        actions = new ArrayList();
                        actions.Add(new KeysAction(text));
                        variableTermDisplay = "'" + text + "'";
                    }
                    else if (term is RangeTerm)
                    {
                        int number = (term as RangeTerm).From + GetNextAlternativeIndex(phraseInfo);
                        actions = new ArrayList();
                        actions.Add(new KeysAction(number.ToString()));
                        variableTermDisplay = number.ToString();
                    }
                    else
                    {
                        List<Command> alternatives;
                        if (term is MenuTerm)
                            alternatives = (term as MenuTerm).Alternatives;
                        else // term is VariableTerm
                            alternatives = (term as VariableTerm).Variable.Menu.Alternatives;
                        int alternativeIndex = GetNextAlternativeIndex(phraseInfo);
                        actions = alternatives[alternativeIndex].Actions;
                        variableTermDisplay = alternatives[alternativeIndex].ToString();
                    }
                    Trace.WriteLine(LogLevel.Low, "    ${0}:  {1}", variableTermIndex++, variableTermDisplay);
                    variableTermActions.Add(actions);
                }
            }
            return variableTermActions;
        }

        private Queue<ISpeechPhraseRule> GetSpecialChildRules(ISpeechPhraseRule rule)
        {
            if (rule.Children == null)
                return null;
            Queue<ISpeechPhraseRule> specialChildRules = new Queue<ISpeechPhraseRule>();
            foreach (ISpeechPhraseRule childRule in rule.Children)
                if (   childRule.Name == "dictationInCommand"
                    || childRule.Name == "Dictation1"
                    || childRule.Name == "CTL_ITEM_TEXTBUFFER"
                    || childRule.Name == "DYNAMIC_ITEM_TEXTBUFFER"
                    || childRule.Name == "SWITCH_ITEM_TBUFFER")
                {
                    specialChildRules.Enqueue(childRule);
                }
            return specialChildRules;
        }

        private string GetSpokenWordsForSpecialVariable(Queue<ISpeechPhraseRule> specialChildRules, ISpeechPhraseInfo phraseInfo)
        {
            ISpeechPhraseRule childRule = specialChildRules.Dequeue();
            if (childRule.Name != "CTL_ITEM_TEXTBUFFER" && childRule.Name != "SWITCH_ITEM_TBUFFER")
                return phraseInfo.GetText(childRule.FirstElement, childRule.NumberOfElements, true);
            else
            {
                // Workaround for WSR bizareness.
                // For <_itemInWindow> and <_windowTitle> WSR enables any word subset of the text.
                // And strangely, the PhraseInfo's single element has:
                //    DisplayText = entire text
                //    LexicalForm = what was actually said
                // We want what was actually said, so if they're different
                // we return (a fixed-up version of) the lexical form.
                ISpeechPhraseElement element = phraseInfo.Elements.Item(childRule.FirstElement);
                string displayText = element.DisplayText;
                string lexicalForm = element.LexicalForm;
                if (displayText == lexicalForm)
                    return displayText;
                else
                {
                    Trace.WriteLine(LogLevel.Low, "    Lexical: '{0}' - Display: '{1}'", lexicalForm, displayText);
                    // Convert e.g. "Hi Bob ," to "Hi Bob,"
                    foreach (char c in Dictation.CharsNoSpaceBefore.ToCharArray())
                    {
                        string s = c.ToString();
                        lexicalForm = lexicalForm.Replace(" " + s, s);
                    }
                    foreach (char c in Dictation.CharsNoSpaceAfter.ToCharArray())
                    {
                        string s = c.ToString();
                        lexicalForm = lexicalForm.Replace(s + " ", s);
                    }
                    return lexicalForm;
                }
            }
        }

        private int GetNextAlternativeIndex(ISpeechPhraseInfo phraseInfo)
        {
            ISpeechPhraseProperties properties = phraseInfo.Properties;
            if (properties == null)
                throw new InternalException("Recognition result contains no properties");
            if (PropertyIndex > properties.Count)
                throw new InternalException("Recognition result contains too few properties");
            return properties.Item(PropertyIndex++).Id;
        }

    }

}
