using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PerCederberg.Grammatica.Parser;

namespace Vocola
{
    public partial class CommandSet
    {
        static int UniqueId = 0;
        static List<SourceFile> SourceFileStack = new List<SourceFile>();

        private class SourceFile
        {
            public string Filename;
            public List<string> UsingSet;

            public SourceFile(string filename, List<string> usingSet)
            {
                Filename = filename;
                UsingSet = usingSet;
            }
        }

        private class ReferenceChecker
        {
            private bool ForFunctionBody;
            private int VariableTermCount;
            private ArrayList FormalNames;

            public ReferenceChecker(int variableTermCount)
            {
                VariableTermCount = variableTermCount;
                ForFunctionBody = false;
            }

            public ReferenceChecker(ArrayList formalNames)
            {
                FormalNames = formalNames;
                ForFunctionBody = true;
            }

            public void CheckReference(string text, Node actionNode)
            {
                if (ForFunctionBody)
                {
                    if (!FormalNames.Contains(text))
                        throw new LanguageException(actionNode, "Reference to unknown formal '${0}'", text);
                }
                else
                {
                    int i;
                    if (!Int32.TryParse(text, out i))
                        throw new LanguageException(actionNode, "Reference is not a number: '${0}'", text);
                    if (i < 1 || i > VariableTermCount)
                        throw new LanguageException(actionNode, "Reference ${0} is out of range", i);
                }
            }
        }

        private class LanguageException : Exception
        {
            public Node Node;

            public LanguageException(Node node, string message, params object[] arguments)
                : base(String.Format(message, arguments))
            {
                Node = node;
            }
        }

        public void ParseFile(Node fileNode, string sourceFilename)
        {
            List<string> usingSet = new List<string>();
            for (int i = 0; i < fileNode.GetChildCount(); i++)
            {
                Node node = fileNode.GetChildAt(i);
                switch (node.GetName())
                {
                case "using":
                    try
                    {
                        string usingName = ParseAndSimplifyLocalActions(node.GetChildAt(1));
                        if (Extensions.ClassOrNamespaceExists(usingName))
                            usingSet.Add(usingName);
                        else
                        {
                            Trace.LogException(new FileLineColumn(sourceFilename, node),
                                "Class or namespace '{0}' referenced by $using not found", usingName);
                            HasLanguageException = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.LogExecutionException(ex, new FileLineColumn(sourceFilename, node));
                    }
                    break;
                case "statements":
                    SourceFileStack.Add(new SourceFile(sourceFilename, usingSet));
                    ParseStatements(node);
                    SourceFileStack.RemoveAt(SourceFileStack.Count - 1);
                    break;
                }
            }
        }

        private void ParseStatements(Node statementsNode)
        {
            for (int i = 0; i < statementsNode.GetChildCount(); i++)
            {
                Node statementNode = statementsNode.GetChildAt(i);
                try
                {
                    switch (statementNode.GetName())
                    {
                    case "include"   : ParseInclude           (statementNode); break;
                    case "context"   : ParseIfElseEnd         (statementNode); break;
                    case "definition": ParseVariableDefinition(statementNode); break;
                    case "function"  : ParseFunctionDefinition(statementNode); break;
                    case "command"   : ParseTopCommand        (statementNode); break;
                    }
                }
                catch (LanguageException le)
                {
                    // Language exceptions (e.g. function redefined, reference out of range) are detected
                    // lower down and thrown to be logged here. They abort processing of the statement but not the file.
                    // Note that parser exceptions were handled earlier, and did abort processing of the file.
                    Trace.LogException(new FileLineColumn(GetSourceFilename(), le.Node), le.Message);
                    HasLanguageException = true;
                }
                catch (Exception ex)
                {
                    // Only ParseInclude() can cause an execution exception
                    Trace.LogExecutionException(ex, new FileLineColumn(GetSourceFilename(), statementNode));  
                }
            }
        }

        private string GetSourceFilename()
        {
            return SourceFileStack[SourceFileStack.Count - 1].Filename;
        }

        private List<string> GetUsingSet()
        {
            return SourceFileStack[SourceFileStack.Count - 1].UsingSet;
        }

        // include
        //   INCLUDE_T: "$include"
        //   includeName
        //     word
        //       NAME: "utilities_"
        //     REFERENCE: "$COMPUTERNAME"
        //     word
        //       CHARS: "."
        //     word
        //       NAME: "vch"
        //   STOP: ";"

        private void ParseInclude(Node includeNode)
        {
            Node filenameNode = includeNode.GetChildAt(1);
            string filename = ParseAndSimplifyLocalActions(filenameNode);
            // Prevent recursive includes
            foreach (SourceFile includedFile in SourceFileStack)
                if (filename == includedFile.Filename)
                    throw new LanguageException(filenameNode, "Ignoring recursive include of file '{0}'", filename);

            string pathname = Path.Combine(Vocola.CommandFolder, filename);
            if (!File.Exists(pathname)) 
                throw new LanguageException(filenameNode, "Include file not found: '{0}'", filename);

            LoadedFile.IncludedFiles.Add(pathname);
            MyVocolaAnalyzer analyzer = new MyVocolaAnalyzer();
            VocolaParser parser = LoadedFile.CreateParserForFile(pathname, analyzer);
            if (parser == null)
                throw new LanguageException(filenameNode, "Could not open include file: '{0}'", filename);
            Node parseTree = null;
            Trace.WriteLine(LogLevel.Low, "{0}Including {1}", new String(' ', 3 * SourceFileStack.Count), filename);
            try
            {
                parseTree = parser.Parse();
            }
            catch (Exception e)
            {
                LoadedFile.HandleParseException(filename, analyzer, e);
                return;
            }
            ParseFile(parseTree, filename);
        }

        private string ParseAndSimplifyLocalActions(Node actionsNode)
        {
            ArrayList actions = ParseActions(actionsNode, new ReferenceChecker(0));
            BindActions(actions);
            return ActionRunner.EvaluateActions(actions);
        }

        // context
        //   IF_T: "$if"
        //   ifbody
        //     words
        //       word
        //         NAME: "Open"
        //     OR: "|"
        //     words
        //       word
        //         NAME: "Save"
        //     STOP: ";"
        //     statements
        //       ...
        //   ELSEIF_T: "$elseif"
        //   ifbody
        //     ...
        //   ELSE_T: "$else"
        //   statements
        //     ...
        //   ENDIF_T: "$end"

        private void ParseIfElseEnd(Node contextNode)
        {
            List<CommandSet> conditionalCommandSet = new List<CommandSet>();
            for (int i = 0; i < contextNode.GetChildCount() - 1; i += 2)
            {
                CommandSet cs = new CommandSet(LoadedFile);
                cs.ParentCommandSet = this;
                cs.AppName = AppName;
                switch (contextNode.GetChildAt(i).GetName())
                {
                    case "IF_T":
                    case "ELSEIF_T":
                        cs.ParseIfBody(contextNode.GetChildAt(i + 1));
                        break;
                    case "ELSE_T":
                        cs.ParseStatements(contextNode.GetChildAt(i + 1));
                        break;
                }
                conditionalCommandSet.Add(cs);
            } 
            ConditionalCommandSets.Add(conditionalCommandSet);
        }            

        private void ParseIfBody(Node ifbodyNode)
        {
            int statementsIndex = ifbodyNode.GetChildCount() - 1;
            for (int i = 0; i < statementsIndex; i += 2)
            {
                object action = ParseAction(ifbodyNode.GetChildAt(i), new ReferenceChecker(0));
                WindowTitlePatterns.Add(((KeysAction)action).Keys);
            }
            ParseStatements(ifbodyNode.GetChildAt(statementsIndex));
        }

        private void ParseVariableDefinition(Node definitionNode)
        {
            Node variableNode = definitionNode.GetChildAt(0);
            VariableTerm vt = (VariableTerm)ParseTerm(variableNode);
            MenuTerm menu = ParseMenuBody(definitionNode.GetChildAt(2));
            Variable variable = new Variable(vt.Name, menu);
            variable.UniqueId = vt.Name + "_" + UniqueId++;
            if (Variables.ContainsKey(vt.Name))
                throw new LanguageException(variableNode, "Variable <{0}> already defined", vt.Name);
            Variables.Add(vt.Name, variable);
        }

        // function
        //   prototype
        //     NAMEPAREN: "drag("
        //     NAME: "dx"
        //     COMMA: ","
        //     NAME: "dy"
        //     PAREN_R: ")"
        //   ASSIGN: ":="
        //   actions
        //     ...

        private void ParseFunctionDefinition(Node functionNode)
        {
            Node prototypeNode = functionNode.GetChildAt(0);
            string functionName = GetText(prototypeNode.GetChildAt(0));
            functionName = functionName.Substring(0, functionName.Length - 1);  // strip trailing '('
            ArrayList formals = new ArrayList();
            for (int i = 1; i < prototypeNode.GetChildCount() - 1; i += 2)
                formals.Add(GetText(prototypeNode.GetChildAt(i)));
            ArrayList actions = ParseActions(functionNode.GetChildAt(2), new ReferenceChecker(formals));
            if (Functions.ContainsKey(functionName))
                throw new LanguageException(prototypeNode, "Function '{0}' already defined", functionName);
            Functions.Add(functionName, new Function(functionName, formals, actions));
        }

        private void ParseTopCommand(Node commandNode)
        {
            int variableTermCount;
            Command command = ParseCommand(commandNode, out variableTermCount);
            command.UniqueId = UniqueIdRoot + UniqueId++;
            command.IsGlobal = (AppName == null);
            command.IsInternal = (LoadedFile == null);
            Commands.Add(command);
        }

        // ---------------------------------------------------------------------
        // Terms

        private ArrayList ParseTerms(Node parentNode, out int nVariableTerms)
        {
            ArrayList terms = new ArrayList();
            bool hasOnlyOptionalWords = true;
            nVariableTerms = 0;
            for (int i = 0; i < parentNode.GetChildCount(); i++)
            {
                object term = ParseTerm(parentNode.GetChildAt(i));
                if (term is VariableTerm || term is RangeTerm || term is MenuTerm)
                    nVariableTerms++;
                terms.Add(term);
                hasOnlyOptionalWords = hasOnlyOptionalWords && term is WordTerm && (term as WordTerm).IsOptional;
            }
            if (hasOnlyOptionalWords)
                throw new LanguageException(parentNode, "At least one term must not be optional");
            return terms;
        }

        private object ParseTerm(Node termNode)
        {
            string text = GetText(termNode);
            switch (termNode.GetName())
            {
            case "NAME":
            case "CHARS":
                return new WordTerm(text);
            case "QUOTED_CHARS":
                text = text.Substring(1, text.Length - 2);  // strip quotes
                return new WordTerm(text);
            case "term":
            case "word":
                return ParseTerm(termNode.GetChildAt(0));
            case "words":
                // Combine all words into a single term
                WordTerm term = (WordTerm)ParseTerm(termNode.GetChildAt(0));
                for (int i = 1; i < termNode.GetChildCount(); i++)
                {
                    WordTerm wt = (WordTerm)ParseTerm(termNode.GetChildAt(i));
                    term.Text += " " + wt.Text;
                }
                return term;
            case "optionalWords":
                term = (WordTerm)ParseTerm(termNode.GetChildAt(1));
                term.IsOptional = true;
                return term;
            case "VARIABLE":
                text = GetText(termNode);
                text = text.Substring(1, text.Length - 2);  // strip angle brackets
                return new VariableTerm(text, GetSourceFilename(), termNode);
            case "RANGE":
                int iDot = text.IndexOf("..");
                int r1 = Int32.Parse(text.Substring(0, iDot));
                int r2 = Int32.Parse(text.Substring(iDot + 2));
                return new RangeTerm(r1, r2);
            case "menu":
                return ParseMenuBody(termNode.GetChildAt(1));
            default:
                throw new InternalException("Unexpected Term '{0}'", termNode.GetName());
            }
        }

        // menuBody
        //   alternative
        //     terms
        //       ...
        //   OR: "|"
        //   alternative
        //     terms
        //       ...
        //     EQUALS: "="
        //     actions
        //       ...

        private MenuTerm ParseMenuBody(Node menuBodyNode)
        {
            List<Command> alternatives = new List<Command>();
            for (int i = 0; i < menuBodyNode.GetChildCount(); i += 2)
            {
                Node commandNode = menuBodyNode.GetChildAt(i);
                int variableTermCount;
                Command command = ParseCommand(commandNode, out variableTermCount);
                if (command.Terms.Count > 1 && variableTermCount > 0)
                    throw new LanguageException(commandNode,
                                                "A variable alternative may not contain additional terms: '{0}'", command);
                else if (command.Terms[0] is VariableTerm)
                {
                    string name = (command.Terms[0] as VariableTerm).Name;
                    if (ReservedVariableNames.Contains(name))
                        throw new LanguageException(commandNode, "A special variable may not be an alternative: '{0}'", name);
                }
                alternatives.Add(command);
            }
            return new MenuTerm(alternatives);
        }

        private Command ParseCommand(Node commandNode, out int variableTermCount)
        {
            ArrayList terms = ParseTerms(commandNode.GetChildAt(0), out variableTermCount);
            ArrayList actions = null;
            if (commandNode.GetChildCount() == 2)
                // Empty alternative, e.g. (red = )
                actions = new ArrayList();
            else if (commandNode.GetChildCount() >= 3)
                // Command with actions (alternative has 3 children, top command has 4)
                actions = ParseActions(commandNode.GetChildAt(2), new ReferenceChecker(variableTermCount));
            return new Command(terms, actions);
        }

        // ---------------------------------------------------------------------
        // Actions

        private ArrayList ParseActions(Node parentNode, ReferenceChecker rc)
        {
            ArrayList actions = new ArrayList();
            for (int i = 0; i < parentNode.GetChildCount(); i++)
            {
                Node actionNode = parentNode.GetChildAt(i);
                object action = ParseAction(actionNode, rc);
                if (action is KeysAction)
                    AddKeysAndReferenceActions(action as KeysAction, actions, actionNode, rc);
                else
                    actions.Add(action);
            }
            return actions;
        }

        static private Regex ReferenceRegex = new Regex(@"(.*?)\$(\d+|[a-zA-Z_]\w*)", RegexOptions.Compiled);

        private void AddKeysAndReferenceActions(KeysAction action, ArrayList actions, Node actionNode, ReferenceChecker rc)
        {
            // Find embedded references (e.g. "{Up_$1}") and convert any instances of "\$" to "$"
            // Strategy:
            //    replace \$ with 0xFFFF
            //    find e.g. $3 or $name
            //    replace 0xFFFF with $
            int remainderIndex = 0;
            string text = HideEscapes(action.Keys);
            for (Match match = ReferenceRegex.Match(text); match.Success; match = match.NextMatch()) 
            {
                string word      = match.Groups[1].Value;
                string reference = match.Groups[2].Value;
                if (word != "")
                    actions.Add(new KeysAction(ConvertHiddenEscapes(word)));
                rc.CheckReference(reference, actionNode);
                actions.Add(new ReferenceAction(reference, GetSourceFilename(), action.ParserNode));
                remainderIndex = match.Index + match.Length;
            }
            if (remainderIndex == 0)
            {
                // There were no embedded references
                action.Keys = action.Keys.Replace(@"\$", "$");
                actions.Add(action);
            }
            else if (remainderIndex < text.Length)
                actions.Add(new KeysAction(ConvertHiddenEscapes(text.Substring(remainderIndex))));
        }

        static private string SecretChar = Char.ConvertFromUtf32(0xFFFF);
        private string HideEscapes         (string s) { return s.Replace(@"\$", SecretChar); }
        private string ConvertHiddenEscapes(string s) { return s.Replace(SecretChar, @"$"); }

        private object ParseAction(Node actionNode, ReferenceChecker rc)
        {
            string text = GetText(actionNode);
            switch (actionNode.GetName())
            {
            case "NAME":
            case "CHARS":
                return new KeysAction(text);
            case "QUOTED_CHARS":
                bool delimitedBySingleQuotes = text.StartsWith("'");
                text = text.Substring(1, text.Length - 2);  // strip quotes
                // convert quote pairs to singles
                if (delimitedBySingleQuotes)
                    text = text.Replace("''", "'");
                else
                    text = text.Replace("\"\"", "\"");
                return new KeysAction(text);
            case "REFERENCE":
                text = text.Substring(1, text.Length - 1);  // strip leading $
                rc.CheckReference(text, actionNode);
                return new ReferenceAction(text, GetSourceFilename(), actionNode);
            case "action":
            case "word":
                return ParseAction(actionNode.GetChildAt(0), rc);
            case "words":
                // Combine words into one action
                KeysAction action = (KeysAction)ParseAction(actionNode.GetChildAt(0), rc);
                for (int i = 1; i < actionNode.GetChildCount(); i++)
                {
                    KeysAction a = (KeysAction)ParseAction(actionNode.GetChildAt(i), rc);
                    action.Keys += a.Keys;
                }
                return action;
            case "call":
                return ParseCall(actionNode, rc);
            default:
                throw new InternalException("Unexpected Action '{0}'", actionNode.GetName());
            }
        }

        // call
        //   DOTTEDNAMEPAREN: "DragonLegacy.SetMousePosition("
        //   actions
        //     action
        //       REFERENCE: "$3"
        //   COMMA: ","
        //   actions
        //     action
        //       REFERENCE: "$2"
        //   PAREN_R: ")"

        private object ParseCall(Node callNode, ReferenceChecker rc)
        {
            string functionName = GetText(callNode.GetChildAt(0));
            functionName = functionName.Substring(0, functionName.Length - 1);  // strip trailing '('
            List<ArrayList> arguments = new List<ArrayList>();
            for (int i = 1; i < callNode.GetChildCount() - 1; i += 2)
                arguments.Add(ParseActions(callNode.GetChildAt(i), rc));
            if (functionName == "Eval")
                return new EvalAction(arguments[0], GetSourceFilename(), callNode);
            else
                return new CallAction(functionName, arguments, GetSourceFilename(), GetUsingSet(), callNode);
        }

        private string GetText(Node node)
        {
            string text = null;
            if (node is Token)
                text = ((Token)node).GetImage();
            return text;
        }

    }
}

