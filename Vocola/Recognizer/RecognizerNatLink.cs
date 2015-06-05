using Microsoft.Win32; // Registry
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Vocola
{

    public class RecognizerNatLink : Recognizer
    {
        private string GrammarsFolder;
        private string NatLinkConnectorDllPath = Path.Combine(Application.StartupPath, "NatLinkConnectorC.dll");
		public DateTime LastGrammarUpdateTime { get; private set; }
		public DateTime LastGrammarCreationTime { get; private set; }

        public override void Initialize()
        {
            GrammarsFolder = Path.Combine(OptionsNatLink.NatLinkInstallFolder, @"NatLink\MacroSystem");
            if (Directory.Exists(GrammarsFolder))
            {
                CleanGrammarsFolder();
                NatLinkListener.Start();
                EmitVocolaMain();
            }
            else
                MessageBox.Show(String.Format("NatLink MacroSystem folder not found: '{0}'", GrammarsFolder));
        }

        private void CleanGrammarsFolder()
        {
            foreach (string pathname in Directory.GetFiles(GrammarsFolder, "*_vcl.py"))
                DeleteFile(pathname);
            foreach (string pathname in Directory.GetFiles(GrammarsFolder, "*_vcl.pyc"))
                DeleteFile(pathname);
            DeleteFileIfPresent(Path.Combine(GrammarsFolder, "_vocola_main.py"));
            DeleteFileIfPresent(Path.Combine(GrammarsFolder, "_vocola_main.pyc"));
        }

        private void DeleteFileIfPresent(string pathname)
        {
            if (File.Exists(pathname))
                DeleteFile(pathname);
        }

        private void DeleteFile(string pathname)
        {
            try   { File.Delete(pathname); }
            catch { Trace.WriteLine(LogLevel.Error, "Unable to delete grammar file '{0}'", pathname); }
        }

        // ---------------------------------------------------------------------
        // Entry points

        public override void CommandFileChanged(LoadedFile loadedFile)
        {
            try
            {
                string moduleName = Path.GetFileNameWithoutExtension(loadedFile.Filename);
                string grammarFilePath = Path.Combine(GrammarsFolder, moduleName + "_vcl.py");
                grammarFilePath = grammarFilePath.Replace('@', '_');
                moduleName = moduleName.ToLower();
                if (loadedFile.ShouldActivateCommands())
                {
                    if (!File.Exists(grammarFilePath))
                        LastGrammarCreationTime = DateTime.Now;
                    EmitGrammarFile(loadedFile.CommandSet, grammarFilePath, moduleName);
                }
                else
                    DeleteFileIfPresent(grammarFilePath);
                LastGrammarUpdateTime = DateTime.Now;
            }
            catch (Exception e)
            {
                Trace.LogUnexpectedException(e);
            }
        }

		public override void EmulateRecognize(string words)
		{
			bool success = NatLinkToVocolaServer.CurrentNatLinkCallbackHandler.EmulateRecognize(words);
			if (!success)
				throw new ActionException(null, "HearCommand() failed to recognize '{0}'", words);
		}
    
        public override void DisplayMessage(string message, bool isWarning)
        {
            Trace.WriteLine(isWarning ? LogLevel.Error : LogLevel.High, message);
        }

        public override void Exit()
        {
            CleanGrammarsFolder();
        }

        public override string Description { get { return "Dragon NaturallySpeaking (DNS), with NatLink"; } }
        public override bool CommandSequencesEnabled { get { return OptionsNatLink.CommandSequencesEnabled; } }
        public override int MaxSequencedCommands { get { return OptionsNatLink.MaxSequencedCommands; } }

        // ---------------------------------------------------------------------
        // Convert one Vocola command file to a NatLink grammar file

		private void EmitGrammarFile(CommandSet commandSet, string grammarFilePath, string moduleName)
        {
            try
            {
				using (TheOutputStream = new StreamWriter(grammarFilePath, false, System.Text.Encoding.GetEncoding(1252)))
                {
                    EmitFileHeader();
                    EmitGrammars(commandSet);
                    if (commandSet.ReferencesDictationVariable)
                        EmitDictationGrammar();
                    EmitSequenceRules(commandSet, 0);
                    EmitFileMiddle(commandSet, moduleName);
                    EmitActivations(commandSet, moduleName);
                    EmitTopCommandActions(commandSet);
                    EmitFileTrailer();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.Error, "Exception writing grammar file '{0}':\n {1}",
					grammarFilePath, ex.Message);
            }
        }

        // ---------------------------------------------------------------------
        // Emit grammars

        private void EmitDictationGrammar()
        {
            EmitLine(2, "<dgndictation> imported;");
        }

        private void EmitGrammars(CommandSet commandSet)
        {
            foreach (Variable variable in commandSet.Variables.Values)
                if (!variable.IsReserved)
                    EmitDefinitionGrammar(variable);

            foreach (Command command in commandSet.Commands)
                EmitCommandGrammar(command);

            foreach (List<CommandSet> ifGroup in commandSet.ConditionalCommandSets)
                foreach (CommandSet cs in ifGroup)
                    EmitGrammars(cs);
        }

        private void EmitDefinitionGrammar(Variable variable)
        {
            Emit(2, "<{0}> = ", variable.UniqueId);
            EmitMenuGrammar(variable.Menu);
            EmitLine(0, ";");
        }

        private void EmitCommandGrammar(Command command)
        {
            InlineATermIfNothingConcrete(command);
    
            int firstTermIndex, lastTermIndex;
            FindTermsForMainRule(command, out firstTermIndex, out lastTermIndex);
            ArrayList mainTerms = new ArrayList(command.Terms.GetRange(firstTermIndex, lastTermIndex - firstTermIndex + 1));
            string nameA = command.UniqueId + "a";
            string nameB = command.UniqueId + "b";

            if (firstTermIndex > 0)
                mainTerms.Insert(0, new VariableTerm(nameA, "", null));
            if (lastTermIndex < command.Terms.Count - 1)
                mainTerms.Add(new VariableTerm(nameB, "", null));

            EmitRule(command.UniqueId, mainTerms);

            if (firstTermIndex > 0)
                EmitRule(nameA, command.Terms.GetRange(0, firstTermIndex));
            if (lastTermIndex < command.Terms.Count - 1)
                EmitRule(nameB, command.Terms.GetRange(lastTermIndex + 1, command.Terms.Count - lastTermIndex - 1));
        }

        private void EmitRule(string name, ArrayList terms)
        {
            Emit(2, "<{0}> = ", name);
            EmitCommandTerms(terms);
            EmitLine(0, ";");
        }

        private void EmitCommandTerms(ArrayList terms)
        {
            foreach (LanguageObject term in terms)
            {
                if (term is WordTerm)
                {
                    WordTerm wt = (WordTerm)term;
                    if (IsOptionalTerm(wt))
                        Emit("[ ");
					//string[] words = wt.Text.Split(' ');
					//if (TermHasAlternates(words))
					//    EmitTermWithAlternates(words);
					//else
						Emit("{0} ", MakeQuotedString(wt.Text));
                    if (IsOptionalTerm(wt))
                        Emit("] ");
                }
                else if (term is VariableTerm)
                {
                    VariableTerm vt = (VariableTerm)term;
                    if (vt.Variable == null)
                        // Reference to helper variable, inserted by splitting a rule
                        Emit("<{0}> ", vt.Name);
                    else if (!vt.Variable.IsReserved)
                        // Reference to normal variable
                        Emit("<{0}> ", vt.Variable.UniqueId);
                    else if (vt.Name == "_anything")
                        // Reference to dictation variable
                        Emit("<dgndictation> ");
                    else
                        // Reference to unknown variable; disable command by including unpronounceable word
                        Emit("'sdfghjkl' ");
                }
                else if (term is RangeTerm)
                {
                    EmitRangeGrammar(term as RangeTerm);
                }
                else if (term is MenuTerm)
                {
                    MenuTerm mt = (MenuTerm)term;
                    Emit("(");
                    EmitMenuGrammar(mt);
                    Emit(") ");
                }
            }
        }

		private void EmitTermWithAlternates(string[] words)
		{
			foreach (string word in words)
			{
				if (TermHasAlternates(word))
				{
					var quotedAlternates = GetAlternates(word).ConvertAll(s => MakeQuotedString(s));
					quotedAlternates.Insert(0, MakeQuotedString(word));
					Emit("({0}) ", string.Join(" | ", quotedAlternates));
				}
				else
					Emit("{0} ", MakeQuotedString(word));
			}
		}

        private bool IsOptionalTerm(object term)
        {
            return (term is WordTerm && (term as WordTerm).IsOptional);
        }

        private void EmitMenuGrammar(MenuTerm menu)
        {
            FlattenMenu(menu);
            bool isFirst = true;
            foreach (Command command in menu.Alternatives)
            {
                if (isFirst)
                    isFirst = false;
                else
                    Emit("| ");
                EmitCommandTerms(command.Terms);
            }
        }

        private void EmitRangeGrammar(RangeTerm rt)
        {
            int i = rt.From;
            Emit("({0}", i);
            while (++i <= rt.To)
                Emit(" | {0}", i);
            Emit(") ");
        }

        // ---------------------------------------------------------------------
        // Emit rules allowing speaking a sequence of commands

        private int EmitSequenceRules(CommandSet commandSet, int ruleNumber)
        {
            commandSet.SequenceRuleNumber = ruleNumber++;
            if (commandSet.Commands.Count > 0)
                EmitSequenceRule(commandSet);
            foreach (List<CommandSet> ifGroup in commandSet.ConditionalCommandSets)
                foreach (CommandSet cs in ifGroup)
                     ruleNumber = EmitSequenceRules(cs, ruleNumber);
            return ruleNumber;
        }

        private void EmitSequenceRule(CommandSet commandSet)
        {
            ArrayList ruleNames = new ArrayList();
            foreach (Command command in commandSet.Commands)
                ruleNames.Add(command.UniqueId);
            string ruleNamesString = String.Join(">|<", (string[])ruleNames.ToArray(typeof(string)));

            string any = String.Format("<any_{0}>", commandSet.SequenceRuleNumber);
            if (commandSet.IsTopLevel || commandSet.ParentCommandSet.Commands.Count == 0)
                EmitLine(2, "{0} = <{1}>;", any, ruleNamesString);
            else
                EmitLine(2, "{0} = <{2}>|<any_{1}>;", any, commandSet.ParentCommandSet.SequenceRuleNumber, ruleNamesString);
            int nSeq = commandSet.MaxSequencedCommands;
            EmitLine(2, "<sequence_{0}> exported = {1};",
                commandSet.SequenceRuleNumber, GetRepeatGrammar(any, nSeq));
        }

        private string GetRepeatGrammar(string spec, int count)
        {
            // Create grammar for "spec" repeated "count" times, e.g. <any_3> [<any_3> [<any_3>]];
            if (count > 99)
                return spec + "+";
            string result = spec;
            while (count-- > 1)
                result = String.Format("{0} [{1}]", spec, result);
            return result;
        }

        private void EmitActivations(CommandSet commandSet, string moduleName)
        {
            EmitLine();
            EmitLine(1, "def gotBegin(self, moduleInfo):");

			if (!commandSet.IsGlobal)
            {
                // For application-specific grammars, emit code to skip further processing if the application isn't active
                string moduleNamePrefix = moduleName;
                Match match = Regex.Match(moduleName.Replace("@", "_"), "^(.+?)_.*");
                if (match.Success)
                    moduleNamePrefix = match.Groups[1].Value;
                EmitLine(2, "window = matchWindow(moduleInfo, '{0}', '')", moduleNamePrefix);
                EmitLine(2, "if not window:");
                EmitLine(2, "    # A different app is active -- no action needed");
                EmitLine(2, "    return");
                EmitLine();
            }
            
            EmitLine(2, "self.firstWord = 0");
            EmitLine(2, "newTitle = string.lower(moduleInfo[1])");
            EmitLine();
            
            if (!commandSet.IsGlobal)
            {
                // For application-specific grammars, commands are activated for a specific window.
                // If the app's window changes we need to re-activate commands for that window.
                EmitLine(2, "if moduleInfo[2] != self.currentModule[2]:");
                EmitLine(2, "    # A different window of this app is active -- deactivate all rules and re-activate for the new window");
                EmitLine(2, "    self.deactivateAll()");
                if (commandSet.ConditionalCommandSets.Count > 0)
                    EmitLine(2, "    self.activateTitleSpecificCommands(newTitle, window)");
                EmitPrimaryActivation(commandSet, moduleName);
                EmitLine();
            }

            if (commandSet.ConditionalCommandSets.Count > 0)
            {
                // If the window title changes, update title-specific command activations
                EmitLine(2, "{0} moduleInfo[1] != self.currentModule[1]:", commandSet.IsGlobal ? "if" : "elif");
                EmitLine(2, "    # Window title has changed -- adjust activation of title-specific rules");
                EmitLine(2, "    oldTitle = string.lower(self.currentModule[1])");
                EmitLine(2, "    self.activateTitleSpecificCommandsThatWereInactive(oldTitle, newTitle{0})", commandSet.IsGlobal ? "" : ", window");
                EmitLine(2, "    self.deactivateTitleSpecificCommandsThatWereActive(oldTitle, newTitle)");
                EmitLine();
            }

            EmitLine(2, "self.currentModule = moduleInfo");
            EmitLine();
            EmitLine();

            EmitContextActivationMethods(commandSet);
        }

        private void EmitPrimaryActivation(CommandSet commandSet, string moduleName)
        {
            if (commandSet.Commands.Count > 0)
            {
                //EmitLine(indent, "try: self.vocolaConnector.LogMessage(2, unicode('  Enabling commands from {0} ({1})'))",
                EmitLine(3, "try: self.vocolaConnector.LogMessage(2, unicode('  Enabling commands from {0} ({1}) for window ' + str(window)))",
                    moduleName, commandSet.Commands.Count);
                EmitLine(3, "except: return");
                EmitLine(3, "self.activate('sequence_0', window, noError=1)");
            }
        }

        private void EmitPrimaryActivationGlobal(CommandSet commandSet, string moduleName)
        {
            if (commandSet.Commands.Count > 0)
            {
                EmitLine(2, "try: self.vocolaConnector.LogMessage(2, unicode('  Enabling commands from {0} ({1})'))",
                    moduleName, commandSet.Commands.Count);
                EmitLine(2, "except: return");
                EmitLine(2, "self.activate('sequence_0', noError=1)");
            }
        }

        private void EmitContextActivationMethods(CommandSet commandSet)
        {
            if (commandSet.ConditionalCommandSets.Count > 0)
            {
                if (!commandSet.IsGlobal)
                {
                    EmitLine(1, "def activateTitleSpecificCommands(self, title, window):");
                    EmitContextActivations(commandSet, 2, EmitContextActivation);
                    EmitLine();
                }
                EmitLine(1, "def activateTitleSpecificCommandsThatWereInactive(self, oldTitle, newTitle{0}):", commandSet.IsGlobal ? "" : ", window");
                EmitContextActivations(commandSet, 2, EmitConditionalContextActivation);
                EmitLine();

                EmitLine(1, "def deactivateTitleSpecificCommandsThatWereActive(self, oldTitle, newTitle):");
                EmitContextActivations(commandSet, 2, EmitConditionalContextDeactivation);
                EmitLine();
            }
        }

        delegate void EmitContextActivationDelegate(CommandSet commandSet, string ifWord, CommandSet firstCommandSet, int level);

        private void EmitContextActivations(CommandSet commandSet, int level, EmitContextActivationDelegate emitActivation)
        {
            foreach (List<CommandSet> ifGroup in commandSet.ConditionalCommandSets)
            {
                bool first = true;
                CommandSet firstCommandSet = ifGroup[0];
                foreach (CommandSet cs in ifGroup)
                    if (cs.Commands.Count > 0)
                    {
                        string ifWord = first ? "if" : cs.WindowTitlePatterns.Count > 0 ? "elif" : "else";
                        first = false;
                        emitActivation(cs, ifWord, firstCommandSet, level);
                        EmitLine();
                        EmitContextActivations(cs, level + 1, emitActivation);
                    }
            }
        }

        private void EmitContextActivation(CommandSet commandSet, string ifWord, CommandSet ignore, int level)
        {
            string tests = GetPredicateForWindowTitlePatterns(commandSet, "title");
            EmitLine(level, "{0} {1}:", ifWord, tests);
            EmitLine(level, "    try: self.vocolaConnector.LogMessage(2, unicode('  Enabling {0} commands matching: {1}'))",
                commandSet.Commands.Count, commandSet.WindowTitlePatternsAsString);
            EmitLine(level, "    except: return");
            EmitLine(level, "    self.activate('sequence_{0}', window, noError=1)", commandSet.SequenceRuleNumber);
        }

        private void EmitConditionalContextActivation(CommandSet commandSet, string ifWord, CommandSet ignore, int level)
        {
            EmitLine(level, "{0} {1}:", ifWord, GetPredicateForWindowTitlePatterns(commandSet, "newTitle"));
            EmitLine(level, "    if not ({0}):", GetPredicateForWindowTitlePatterns(commandSet, "oldTitle"));
            EmitLine(level, "        try: self.vocolaConnector.LogMessage(2, unicode('  Enabling {0} commands matching: {1}'))",
                commandSet.Commands.Count, commandSet.WindowTitlePatternsAsString);
            EmitLine(level, "        except: return");
            EmitLine(level, "        self.activate('sequence_{0}'{1}, noError=1)", commandSet.SequenceRuleNumber, commandSet.IsGlobal ? "" : ", window");
        }

        private void EmitConditionalContextDeactivation(CommandSet commandSet, string ifWord, CommandSet firstCommandSet, int level)
        {
            EmitLine(level, "{0} {1}:", ifWord, GetPredicateForWindowTitlePatterns(commandSet, "oldTitle"));
            if (ifWord == "else")
            {
                EmitLine(level, "    if ({0}):", GetPredicateForWindowTitlePatterns(firstCommandSet, "newTitle"));
                EmitLine(level, "        try: self.vocolaConnector.LogMessage(2, unicode('  Disabling {0} commands not matching: {1}'))",
                    commandSet.Commands.Count, commandSet.WindowTitlePatternsAsString);
            }
            else
            {
                EmitLine(level, "    if not ({0}):", GetPredicateForWindowTitlePatterns(commandSet, "newTitle"));
                EmitLine(level, "        try: self.vocolaConnector.LogMessage(2, unicode('  Disabling {0} commands matching: {1}'))",
                    commandSet.Commands.Count, commandSet.WindowTitlePatternsAsString);
            }
            EmitLine(level, "        except: return");
            EmitLine(level, "        self.deactivate('sequence_{0}', noError=1)", commandSet.SequenceRuleNumber);
        }

        private string GetPredicateForWindowTitlePatterns(CommandSet commandSet, string titleVariableName)
        {
            ArrayList patterns = new ArrayList();
            foreach (string pattern in commandSet.WindowTitlePatterns)
                patterns.Add(String.Format("{1} in {0}", titleVariableName, MakeQuotedString(pattern.ToLower())));
            string tests = String.Join(" or ", (string[])patterns.ToArray(typeof(string)));
            return tests;
        }

        // ---------------------------------------------------------------------
        // Emit functions to handle individual command recognitions

        private void EmitTopCommandActions(CommandSet commandSet)
        {
            foreach (Command command in commandSet.Commands)
                EmitTopCommandActions(command);
            foreach (List<CommandSet> ifGroup in commandSet.ConditionalCommandSets)
                foreach (CommandSet cs in ifGroup)
                    EmitTopCommandActions(cs);
        }

        private void EmitTopCommandActions(Command command)
        {
            ArrayList terms = command.Terms;
            int nTerms = terms.Count;
            string functionName = "gotResults_" + command.UniqueId;

            EmitLine(1, "# {0}", command.TermsToString());
            EmitLine(1, "def {0}(self, words, fullResults):", functionName);
            EmitLine(2, "if self.firstWord == 0:");
            EmitLine(2, "    try: self.logSpokenCommand(fullResults)");
            EmitLine(2, "    except: return");
            EmitOptionalTermFixup(terms);
            EmitLine(2, "variableTerms = ''");

            int termNumber = 0;
            foreach (LanguageObject term in command.Terms)
            {
                if (IsAnythingTerm(term))
                {
                    EmitLine(2, "fullResults = self.combineDictationWords(fullResults)");
                    EmitLine(2, "i = {0} + self.firstWord", termNumber);
                    EmitLine(2, "if (len(fullResults) <= i) or (fullResults[i][1] != 'dgndictation'):");
                    EmitLine(3, "fullResults.insert(i, ['','dummy'])");
                }
                if (term is MenuTerm || term is RangeTerm || term is VariableTerm)
                    EmitLine(2, "variableTerms += fullResults[{0} + self.firstWord][0] + '\\n'", termNumber);
                termNumber++;
            }
			//EmitLine(2, "print 'calling RunActions()'");
			EmitLine(2, "self.vocolaConnector.RunActions(unicode('{0}'), unicode(variableTerms))", command.UniqueId);
			//EmitLine(2, "print 'returned from RunActions()'");
			EmitLine(2, "self.firstWord += {0}", nTerms);

            // If repeating a command with no <variable> terms (e.g. "Scratch That
            // Scratch That"), our gotResults function will be called only once, with
            // all recognized words. Recurse!
            if (NoTermReferencesAVariable(terms))
                EmitLine(2, "if len(words) > {0}: self.{1}(words[{0}:], fullResults)", nTerms, functionName);
            EmitLine();
        }

        private static bool IsAnythingTerm(object term)
        {
            return (term is VariableTerm && (term as VariableTerm).Name == "_anything");
        }

        private bool NoTermReferencesAVariable(ArrayList terms)
        {
            foreach (LanguageObject term in terms)
                if (term is VariableTerm)
                    return false;
            return true;
        }

        // Our indexing into the "fullResults" array assumes all optional terms were 
        // spoken.  So we emit code to insert a dummy entry for each optional word 
        // that was not spoken.  (The strategy used could fail in the uncommon case 
        // where an optional word is followed by an identical required word.)

        private void EmitOptionalTermFixup(ArrayList terms)
        {
            int termNumber = -1;
            foreach (LanguageObject term in terms)
            {
                termNumber++;
                if (IsOptionalTerm(term))
                {
                    EmitLine(2, "opt = {0} + self.firstWord", termNumber);
                    EmitLine(2, "if opt >= len(fullResults) or fullResults[opt][0] != '{0}':", ((WordTerm)term).Text);
                    EmitLine(3, "fullResults.insert(opt, 'dummy')");
                }
            }   
        }

        // ---------------------------------------------------------------------------
        // Utilities for transforming command terms into NatLink rules 
        //
        // For each Vocola command, we define a NatLink ruIf ifle and an associated
        // "gotResults" function. When the command is spoken, we want the gotResults
        // function to be called exactly once. But life is difficult -- NatLink calls a
        // gotResults function once for each contiguous sequence of spoken words
        // specifically present in the associated rule. There are two problems:
        //
        // 1) If a rule contains only references to other rules, it won't be called 
        //
        // We solve this by "inlining" variables (replacing a variable term with the
        // variable's definition) until the command is "concrete" (all branches contain
        // a non-optional word).
        //
        // 2) If a rule is "split" (e.g. "Kill <n> Words") it will be called twice
        //
        // We solve this by generating two rules, e.g.
        //    <1> exported = 'Kill' <n> <1a> ;
        //    <1a> = 'Words' ;

        static private Regex FirstTermRegex = new Regex("^(v*o+v[ov]*c)", RegexOptions.Compiled);
        static private Regex LastTermRegex  = new Regex("^([ov]*c[co]*)v+[co]+", RegexOptions.Compiled);

        private void FindTermsForMainRule(Command command, out int firstTermIndex, out int lastTermIndex)
        {
            // Create a "variability profile" summarizing whether each term is
            // concrete (c), variable (v), or optional (o). For example, the
            // profile of "[One] Word <direction>" would be "ocv". (Menus are
            // assumed concrete, and dictation variables are treated like
            // normal variables.)

            StringBuilder sb = new StringBuilder();
            foreach (LanguageObject term in command.Terms)
                sb.Append(
                    (term is VariableTerm) ? 'v'
                    : (IsOptionalTerm(term)) ? 'o'
                    : 'c');
            string profile = sb.ToString();

            // Identify terms to use for main rule.
            // We might not start with the first term. For example:
            //     [Move] <n> Left  -->  "Left" is the first term to use
            // We might not end with the last term. For example:
            //     Kill <n> Words   -->  "Kill" is the last term to use
            // And in this combined example, our terms would be "Left and Kill"
            //     [Move] <n> Left and Kill <n> Words

            Match match = FirstTermRegex.Match(profile); // ^(v*o+v[ov]*c)
            if (match.Success)
                firstTermIndex = match.Groups[1].Value.Length - 1;
            else
                firstTermIndex = 0;

            match = LastTermRegex.Match(profile); // ^([ov]*c[co]*)v+[co]+
            if (match.Success)
                lastTermIndex = match.Groups[1].Value.Length - 1;
            else
                lastTermIndex = profile.Length - 1;
        }

        private void InlineATermIfNothingConcrete(Command command)
        {
            while (!CommandHasAConcreteTerm(command))
                InlineATerm(command);
        }

        private bool CommandHasAConcreteTerm(Command command)
        {
            foreach (LanguageObject term in command.Terms)
                if (TermIsConcrete(term))
                    return true;
            return false;
        }

        private bool TermIsConcrete(LanguageObject term)
        {
            if (term is MenuTerm)
                return true;
            else if (term is VariableTerm)
                return false;
            else
                return !IsOptionalTerm(term);
        }

        private void InlineATerm(Command command)
        {
            ArrayList terms = command.Terms;

            // Find the array index of the first non-optional term
            int index = -1;
            do
                index++;
            while
                (index < terms.Count &&
                    (IsOptionalTerm(terms[index]) || IsAnythingTerm(terms[index])));

            object term = terms[index];
            if (term is VariableTerm)
                terms[index] = (term as VariableTerm).Variable.Menu;
            else if (term is MenuTerm)
                foreach (Command c in (term as MenuTerm).Alternatives)
                    InlineATerm(c);
            else
                throw new InternalException("Internal error inlining '{0}'", term);
        }

        // ---------------------------------------------------------------------------
        // Utilities used by "emit" methods

        StreamWriter TheOutputStream; // initialized above

        public void Emit(string text, params object[] arguments)
        {
			Emit(0, text, arguments);
        }

		public void Emit(int level, string text, params object[] arguments)
		{
			TheOutputStream.Write(new string(' ', 4 * level));
			TheOutputStream.Write(text, arguments);
		}

        public void EmitLine(int level, string text, params object[] arguments)
        {
            TheOutputStream.Write(new string(' ', 4 * level));
            TheOutputStream.WriteLine(text, arguments);
        }

        public void EmitLine()
        {
            TheOutputStream.WriteLine("");
        }

        private string MakeQuotedString(string text)
        {
            text = text.Replace("\\", "\\\\"); // escape backslashes
            if (text.Contains("'"))
                return String.Format("\"{0}\"", text);
            else
                return String.Format("'{0}'", text);
        }

        // ---------------------------------------------------------------------------
		// Emit _vocola_main.py (grammar file treated specially by NatLink)

		private void EmitVocolaMain()
		{
			string path = Path.Combine(GrammarsFolder, "_vocola_main.py");
			using (TheOutputStream = new StreamWriter(path, false, Encoding.GetEncoding(1252)))
			{
				Emit(@"
import natlink
from natlinkutils import *
import ctypes
from ctypes import CFUNCTYPE, c_wchar_p, c_int
import string

VocolaEnabled = True

class ThisGrammar(GrammarBase):

    def initialize(self):
        global vocolaConnector
");
				EmitLine(2, "vocolaConnector = ctypes.windll.LoadLibrary(r'{0}')", NatLinkConnectorDllPath);
				EmitLine(2, "connected = vocolaConnector.InitializeConnection(unicode(r'{0}'))", Path.GetDirectoryName(NatLinkConnectorDllPath));
				EmitLine(2, "if connected == 0:");
				EmitLine(2, "    print 'Vocola is enabled but not running'");
				EmitLine(2, "    return");
				EmitLine(2, "print 'Vocola {0} is running'", Vocola.Version);
				Emit(@"
        MYFUNCTYPE = CFUNCTYPE(c_int, c_wchar_p)
        self.emulateRecognizeFunc = MYFUNCTYPE(emulateRecognize)
        self.sendKeysFunc = MYFUNCTYPE(sendKeys)
        vocolaConnector.SetCallbacks(self.emulateRecognizeFunc, self.sendKeysFunc)

# When speech is heard, vocolaBeginCallback is called (from natlinkmain) before any others.
# Return values indicate whether Vocola has changed any .py files since the last call:
#     0 - No changes
#     1 - A .py file changed
#     2 - A .py file was created
#
# (Note natlinkmain guarantees we are not called with CallbackDepth > 1)

def vocolaBeginCallback(moduleInfo):
    result = 0
    try: result = vocolaConnector.HaveAnyGrammarFilesChanged()
    except: 
        print 'Vocola is not responding'
        result = 1   # Vocola deletes grammar files on exit
    #print 'vocola files changed: %i'% result
    return result

def emulateRecognize(words):
    try: natlink.recognitionMimic(words.encode('ascii','replace').split(' '))
    except: return -1
    return 0

def sendKeys(keys):
    try: natlink.playString(keys.encode('ascii','replace'))
    except: return -1
    return 0


thisGrammar = ThisGrammar()
thisGrammar.initialize()

def unload():
    global thisGrammar
    if thisGrammar: thisGrammar.unload()
    thisGrammar = None
");
			}
		}

		// ---------------------------------------------------------------------------
		// Emit pieces of Python grammar files

        private void EmitFileHeader()
        {
            EmitLine(0, "# NatLink macro definitions, for Dragon NaturallySpeaking"); 
            EmitLine(0, "# coding: Latin-1");
            EmitLine(0, "# Generated by Vocola {0}, {1}", Vocola.VersionString, DateTime.Now);
            Emit(@"
import natlink
from natlinkutils import *
import ctypes
import string

class ThisGrammar(GrammarBase):

    gramSpec = """"""
");
        }

        private void EmitFileMiddle(CommandSet commandSet, string moduleName)
        {
            EmitLine(0, "\"\"\""); // close the grammar spec
            EmitLine();
            EmitLine(1, "def initialize(self):");
			EmitLine(2, "self.vocolaConnector = ctypes.windll.LoadLibrary(r'{0}')", NatLinkConnectorDllPath);
			EmitLine(2, "connected = self.vocolaConnector.InitializeConnection(unicode(r'{0}'))", Path.GetDirectoryName(NatLinkConnectorDllPath));
			EmitLine(2, "if connected == 0: return");
            EmitLine();
            EmitLine(2, "print 'Loading Vocola commands for {0}'", moduleName);
            EmitLine(2, "self.load(self.gramSpec)");
            EmitLine(2, "self.currentModule = ('', '', 0)");
            if (commandSet.IsGlobal)
            {
                EmitLine();
                EmitLine(2, "# Activate global commands. (They will remain active until the grammar is loaded.)");
                EmitPrimaryActivationGlobal(commandSet, moduleName);
            }
            EmitLine();
        }

        private void EmitFileTrailer()
        {
            Emit(@"
    # Massage recognition results to make a single entry for each <dgndictation> result.

    def logSpokenCommand(self, fullResults):
        words = map(lambda t : t[0], fullResults)
        phrase = ' '.join(words)
        self.vocolaConnector.LogMessage(3, unicode('-----------------------------------------'))
        self.vocolaConnector.LogMessage(1, 'Command: ' + unicode(phrase))
        if words == 1: print self.myFunc # hack to keep myFunc from being deallocated

    def combineDictationWords(self, fullResults):
        i = 0
        inDictation = 0
        while i < len(fullResults):
            if fullResults[i][1] == 'dgndictation':
                # This word came from a 'recognize anything' rule.
                # Convert to written form if necessary, e.g. '@\at-sign' --> '@'
                word = fullResults[i][0]
                backslashPosition = string.find(word, '\\')
                if backslashPosition > 0:
                    word = word[:backslashPosition]
                if inDictation:
                    fullResults[i-1] = [fullResults[i-1][0] + ' ' + word, 'dgndictation']
                    del fullResults[i]
                else:
                    fullResults[i] = [word, 'dgndictation']
                    i = i + 1
                inDictation = 1
            else:
                i = i + 1
                inDictation = 0
        return fullResults

thisGrammar = ThisGrammar()
thisGrammar.initialize()

def unload():
    global thisGrammar
    if thisGrammar: thisGrammar.unload()
    thisGrammar = None
");
        }

        // ---------------------------------------------------------------------------
        // Return actions for each variable word in recognized utterance

        public static List<ArrayList> GetVariableTermActions(Command command, string variableWords)
        {
            int variableTermIndex = 0;
            string[] words = variableWords.Split('\n');
            List<ArrayList> variableTermActions = new List<ArrayList>();
            foreach (LanguageObject term in command.Terms)
            {
                if (term is MenuTerm || term is RangeTerm || term is VariableTerm)
                {
                    string word = words[variableTermIndex++];
                    string variableTermDisplay;
                    ArrayList actions;
                    if (IsAnythingTerm(term) || term is RangeTerm)
                    {
                        actions = new ArrayList();
                        actions.Add(new KeysAction(word));
                        variableTermDisplay = (term is RangeTerm) ? word : "'" + word + "'";
                    }
                    else
                    {
                        List<Command> alternatives;
                        if (term is MenuTerm)
                            alternatives = (term as MenuTerm).Alternatives;
                        else // term is VariableTerm
                            alternatives = (term as VariableTerm).Variable.Menu.Alternatives;
                        Command alternative = GetAlternative(alternatives, word);
                        actions = alternative.Actions;
                        variableTermDisplay = alternative.ToString();
                    }
                    Trace.WriteLine(LogLevel.Low, "    ${0}:  {1}", variableTermIndex, variableTermDisplay);
                    variableTermActions.Add(actions);
                }
            }
            return variableTermActions;
        }

        private static Command GetAlternative(List<Command> alternatives, string word)
        {
            foreach (Command command in alternatives)
                if (command.Terms.Count == 1 && command.Terms[0] is WordTerm && (command.Terms[0] as WordTerm).Text == word)
                    return command;
            throw new InternalException("Spoken alternative '{0}' not found in recognized command", word);
        }

    }
}
