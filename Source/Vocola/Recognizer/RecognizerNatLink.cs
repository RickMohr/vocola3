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
        private string GrammarsFolder = @"C:\Programs\NatLink\NatLink\MacroSystem";
        private int NatLinkToVocolaPort = 9753;
        private string NatLinkConnectorDllPath = Path.Combine(Application.StartupPath, "NatLinkConnectorC.dll");

        public override void Initialize()
        {
            ReadRegistry();
            CleanGrammarsFolder();
            NatLinkListener.Start(NatLinkToVocolaPort);
        }

        private void ReadRegistry()
        {
			RegistryKey key = Registry.CurrentUser.CreateSubKey(Path.Combine(Vocola.RegistryKeyName, "NatLinkRecognizer"));
        }

        private void CleanGrammarsFolder()
        {
            foreach (string pathname in Directory.GetFiles(GrammarsFolder, "*_vcl.py"))
                DeleteFile(pathname);
            foreach (string pathname in Directory.GetFiles(GrammarsFolder, "*_vcl.pyc"))
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
                string grammarFilename = Path.Combine(GrammarsFolder, moduleName + "_vcl.py");
                grammarFilename = grammarFilename.Replace('@', '_');
                moduleName = moduleName.ToLower();
                if (loadedFile.ShouldActivateCommands())
                    EmitGrammarFile(loadedFile.CommandSet, grammarFilename, moduleName);
            }
            catch (Exception e)
            {
                Trace.LogUnexpectedException(e);
            }
        }
    
        public override void DisplayMessage(string message, bool isWarning)
        {
            Trace.WriteLine(isWarning ? LogLevel.Error : LogLevel.High, message);
        }

        // ---------------------------------------------------------------------
        // Convert one Vocola command file to a NatLink grammar file

        private void EmitGrammarFile(CommandSet commandSet, string grammarFilename, string moduleName)
        {
            try
            {
                using (TheOutputStream = new StreamWriter(grammarFilename, false, System.Text.Encoding.GetEncoding(1252)))
                {
                    EmitFileHeader();
                    EmitDictationGrammar();
                    EmitGrammars(commandSet);
                    EmitSequenceRules(commandSet, 0);
                    EmitFileMiddle(moduleName);
                    EmitActivations(commandSet, moduleName);
                    EmitTopCommandActions(commandSet);
                    EmitFileTrailer();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.Error, "Exception writing grammar file '{0}':\n {1}",
                    grammarFilename, ex.Message);
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
                        Emit(0, "[ ");
                    Emit(0, "{0} ", MakeQuotedString(wt.Text));
                    if (IsOptionalTerm(wt))
                        Emit(0, "] ");
                }
                else if (term is VariableTerm)
                {
                    VariableTerm vt = (VariableTerm)term;
                    if (vt.Variable == null)
                        // Reference to helper variable, inserted by splitting a rule
                        Emit(0, "<{0}> ", vt.Name);
                    else if (!vt.Variable.IsReserved)
                        // Reference to normal variable
                        Emit(0, "<{0}> ", vt.Variable.UniqueId);
                    else if (vt.Name == "_anything")
                        // Reference to dictation variable
                        Emit(0, "<dgndictation> ");
                    else
                        // Reference to unknown variable; disable command by including unpronounceable word
                        Emit(0, "'sdfghjkl' ");
                }
                else if (term is RangeTerm)
                {
                    EmitRangeGrammar(term as RangeTerm);
                }
                else if (term is MenuTerm)
                {
                    MenuTerm mt = (MenuTerm)term;
                    Emit(0, "(");
                    EmitMenuGrammar(mt);
                    Emit(0, ") ");
                }
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
                    Emit(0, "| ");
                EmitCommandTerms(command.Terms);
            }
        }

        private void EmitRangeGrammar(RangeTerm rt)
        {
            int i = rt.From;
            Emit(0, "({0}", i);
            while (++i <= rt.To)
                Emit(0, " | {0}", i);
            Emit(0, ") ");
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
                EmitLine(2, "{0} = <any_{1}>|<{2}>;", any, commandSet.ParentCommandSet.SequenceRuleNumber, ruleNamesString);
			int nSeq = 2;// (Vocola.CommandSequencesEnabled ? Vocola.MaxSequencedCommands : 0);
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
			bool moduleHasPrefix = false;
			string prefix = "";
            Match match = Regex.Match(moduleName, "^(.+?)_.*");
            if (match.Success)
            {
                prefix = match.Groups[1].Value;
                moduleHasPrefix = true;
            }
            EmitLine(0, "");
            EmitLine(1, "def gotBegin(self,moduleInfo):");
			if (commandSet.IsGlobal)
                EmitLine(2, "window = moduleInfo[2]");
            else
            {
                EmitLine(2, "# Return if wrong application");
                EmitLine(2, "window = matchWindow(moduleInfo,'{0}','')", moduleName);
                if (moduleHasPrefix)
                    EmitLine(2, "if not window: window = matchWindow(moduleInfo,'{0}','')", prefix);
                EmitLine(2, "if not window: return None");
            }
            EmitLine(2, "self.firstWord = 0");
            EmitLine(2, "# Return if same window and title as before");
            EmitLine(2, "if moduleInfo == self.currentModule: return None");
            EmitLine(2, "self.currentModule = moduleInfo");
            EmitLine(0, "");
            EmitLine(2, "self.deactivateAll()");
			if (commandSet.Commands.Count > 0)
			{
				EmitLine(2, "self.vocolaConnector.LogMessage(2, unicode('  Enabling commands from {0} ({1})'))",
					moduleName, commandSet.Commands.Count);
				if (commandSet.IsGlobal)
					EmitLine(2, "self.activate('sequence_0')");
				else
					EmitLine(2, "self.activate('sequence_0', window)");
			}
            EmitLine(2, "title = string.lower(moduleInfo[1])");

            // Emit code to activate the context's commands if one of the context
            // strings matches the current window
            EmitContextActivations(commandSet, 2);
            EmitLine(0, "");
        }

        private void EmitContextActivations(CommandSet commandSet, int level)
        {
            foreach (List<CommandSet> ifGroup in commandSet.ConditionalCommandSets)
            {
                bool first = true;
                foreach (CommandSet cs in ifGroup)
                    if (cs.Commands.Count > 0)
                    {
                        string ifWord = first ? "if" : cs.WindowTitlePatterns.Count > 0 ? "elif" : "else";
                        first = false;
                        EmitContextActivation(cs, ifWord, level);
                        EmitContextActivations(cs, level + 1);
                    }
            }
        }

        private void EmitContextActivation(CommandSet commandSet, string ifWord, int level)
        {
            ArrayList patterns = new ArrayList();
            foreach (string pattern in commandSet.WindowTitlePatterns)
                patterns.Add(String.Format("string.find(title,{0}) >= 0", MakeQuotedString(pattern)));
            string tests = String.Join(" or ", (string[])patterns.ToArray(typeof(string)));
            EmitLine(level, "{0} {1}:", ifWord, tests);
			if (commandSet.IsGlobal)
				EmitLine(level, "    try: self.activate('sequence_{0}')", commandSet.SequenceRuleNumber);
			else
				EmitLine(level, "    try: self.activate('sequence_{0}', window)", commandSet.SequenceRuleNumber);
            EmitLine(level, "    except BadWindow: pass");
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
			EmitLine(2, "if self.firstWord == 0: self.logSpokenCommand(fullResults)");
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
            EmitLine(2, "self.vocolaConnector.RunActions(unicode('{0}'), unicode(variableTerms))", command.UniqueId);
            EmitLine(2, "self.firstWord += {0}", nTerms);

            // If repeating a command with no <variable> terms (e.g. "Scratch That
            // Scratch That"), our gotResults function will be called only once, with
            // all recognized words. Recurse!
            if (NoTermReferencesAVariable(terms))
                EmitLine(2, "if len(words) > {0}: self.{1}(words[{0}:], fullResults)", nTerms, functionName);
            EmitLine(0, "");
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
        // For each Vocola command, we define a NatLink rule and an associated
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

        private string MakeQuotedString(string text)
        {
            text = text.Replace("\\", "\\\\"); // escape backslashes
            if (text.Contains("'"))
                return String.Format("\"{0}\"", text);
            else
                return String.Format("'{0}'", text);
        }

        // ---------------------------------------------------------------------------
        // Pieces of the output Python file

        private void EmitFileHeader()
        {
            EmitLine(0, "# NatLink macro definitions, for Dragon NaturallySpeaking"); 
            EmitLine(0, "# coding: Latin-1");
            EmitLine(0, "# Generated by Vocola {0}, {1}", Vocola.VersionString, DateTime.Now);
            Emit(0, @"
import natlink
from natlinkutils import *
import ctypes

class ThisGrammar(GrammarBase):

    gramSpec = """"""
");
        }

        private void EmitFileMiddle(string moduleName)
        {
            EmitLine(0, "");
            EmitLine(0, "\"\"\"");
            EmitLine(1, "def initialize(self):");
            EmitLine(2, "print 'Loading Vocola commands for {0}'", moduleName);
            EmitLine(2, "self.vocolaConnector = ctypes.windll.LoadLibrary(r'{0}')", NatLinkConnectorDllPath);
            EmitLine(2, "self.vocolaConnector.InitializeConnection({0}, unicode(r'{1}'))", NatLinkToVocolaPort, Path.GetDirectoryName(NatLinkConnectorDllPath));
            //EmitLine(2, "self.vocolaConnector.InitializeConnection()");
            Emit(0, @"
        self.load(self.gramSpec)
        self.currentModule = ("","",0)
");
        }

        private void EmitFileTrailer()
        {
            Emit(0, @"
    # Massage recognition results to make a single entry for each <dgndictation> result.

    def logSpokenCommand(self, fullResults):
		words = map(lambda t : t[0], fullResults)
		phrase = ' '.join(words)
		self.vocolaConnector.LogMessage(1, 'Command: ' + unicode(phrase))

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
