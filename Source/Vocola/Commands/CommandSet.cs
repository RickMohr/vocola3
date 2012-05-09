using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using PerCederberg.Grammatica.Parser;
using System.Text.RegularExpressions; // Node

namespace Vocola
{

    public partial class CommandSet
    {
        public  Dictionary<string, Variable> Variables; // maps variable name to Variable instance
        private Dictionary<string, Function> Functions; // maps function name to Function instance (formals, actions)
        public  List<Command> Commands { get; private set; }
        public  List<List<CommandSet>> ConditionalCommandSets; // created by $if...$elseif...$else...$end
        public  ArrayList WindowTitlePatterns { get; private set; }
        private  string AppName;
        public  CommandSet ParentCommandSet { get; private set; } // if I was created by $if
        private LoadedFile LoadedFile;
		private string UniqueIdRoot;
        public  bool HasLanguageException { get; private set; }
		public int MaxSequencedCommands { get; private set; }
		public  int SequenceRuleNumber; // used by RecognizerNatLink

        public CommandSet(LoadedFile loadedFile, string appName)
        {
            Variables = new Dictionary<string, Variable>(); 
            Functions = new Dictionary<string, Function>(); 
            Commands = new List<Command>(); 
            ConditionalCommandSets = new List<List<CommandSet>>();
            WindowTitlePatterns = new ArrayList();
			AppName = appName;
            ParentCommandSet = null;
            HasLanguageException = false;
			MaxSequencedCommands = 1;// Vocola.MaxSequencedCommands;
            LoadedFile = loadedFile;
            if (loadedFile == null)
                UniqueIdRoot = "__internal_";
            else
            {
                UniqueIdRoot = Path.GetFileNameWithoutExtension(LoadedFile.Filename) + "_";
				UniqueIdRoot = UniqueIdRoot.Replace(".", "_").Replace("@", "_").Replace("-", "_");
				//Regex.Replace(UniqueIdRoot, "[^a-zA-Z0-9_]", "_");
            }
        }

        List<string> ReservedVariableNames = new List<string>(new string[]
            {
                "_anything",
                "_itemInWindow",
                "_startableName",
                "_textInDocument",
                "_vocolaDictation",
                "_windowTitle",
            });

        public void AddReservedVariables()
        {
            foreach (string name in ReservedVariableNames)
                Variables.Add(name, new Variable(name));
        }

		public bool IsTopLevel { get { return (ParentCommandSet == null); } }

		public bool IsGlobal { get { return (AppName == null); } }

        // ---------------------------------------------------------------------
        // Binding

        public void Bind()
        {
            // Note that errors (e.g. call to undefined function) are detected lower down
            // and neither abort binding nor invalidate the command.

            // Resolve variable and function references in commands
            BindCommands(Commands);

            // Resolve variable and function references in variable definitions
            foreach (Variable variable in Variables.Values)
            {
                BindCommands(variable.Menu.Alternatives);
                variable.Rule = null; // Clear cached rule
            }

            // Resolve function references in function definitions
            foreach (Function function in Functions.Values)
                BindActions(function.Actions);

            // Bind conditional commands sets
            foreach (List<CommandSet> ifGroup in ConditionalCommandSets)
                foreach (CommandSet cs in ifGroup)
                    cs.Bind();
        }

        private void BindCommands(List<Command> commands)
        {
            foreach (Command command in commands)
            {
                // Resolve variable references
                foreach (LanguageObject term in command.Terms)
                    if (term is VariableTerm)
                    {
                        VariableTerm vt = (term as VariableTerm);
                        if (BindVariable(vt))
                        {
                            if (vt.Variable.IsReserved)
                                command.HasWildcardTerm = true;
                        }
                        else
                        {
                            Trace.LogException(term, "Reference to undefined variable: {0}", vt);
                            HasLanguageException = true;
                        }
                    }
                    else if (term is MenuTerm)
                        BindCommands(((MenuTerm)term).Alternatives);

                // Resolve function references
                BindActions(command.Actions);

                // Clear cached rule
                command.Rule = null;
            }
        }

        private bool BindVariable(VariableTerm variableTerm)
        {
            Variable variable;
            // Look in my variable definitions
            if (Variables.TryGetValue(variableTerm.Name, out variable))
            {
                variableTerm.Variable = variable;
                return true;
            }
            // Look (recursively) in ancestor command sets
            if (ParentCommandSet != null && ParentCommandSet.BindVariable(variableTerm))
                return true;
            return false;
        }

        private void BindActions(ArrayList actions)
        {
            if (actions != null)
                foreach (object action in actions)
                    if (action is CallAction)
                    {
                        CallAction call = action as CallAction;
                        BindFunctionCall(call);
                        foreach (ArrayList argumentActions in call.Arguments)
                            BindActions(argumentActions);
                    }
                    else if (action is EvalAction)
                        BindActions((action as EvalAction).ExpressionActions);
        }

        private void BindFunctionCall(CallAction callAction)
        {
            string functionName = callAction.FunctionName;
            int nFunctionNameParts = functionName.Split('.').Length;
            int nActuals = callAction.Arguments.Count;
            string arityMessage = null;
            
            if (nFunctionNameParts == 1) // No classname specified
            {
                // Look for true built-in
                int nFormalsForSpecialForm = GetNFormalsForSpecialForm(functionName);
                if (nFormalsForSpecialForm == nActuals)
                    return;
                else if (nFormalsForSpecialForm >= 0)
                    arityMessage = String.Format("Special form '{0}'", functionName);

                // Look for user-defined function
                Function userFunction = GetUserFunction(functionName);
                if (userFunction != null)
                {
                    if (nActuals == userFunction.Formals.Count)
                    {
                        callAction.UserFunction = userFunction;
                        return;
                    }
                    else if (arityMessage == null)
                        arityMessage = String.Format("User function '{0}'", functionName);
                }

                // Look for extension function
                if (BindToExtensionFunctionUsing(callAction, functionName, nActuals, ref arityMessage))
                    return;
            }
            else if (nFunctionNameParts == 2) // No namespace specified
            {
                if (BindToExtensionFunctionUsing(callAction, functionName, nActuals, ref arityMessage))
                    return;
            }
            else // Namespace and classname both specified
            {
                if (BindToExtensionFunction(callAction, functionName, nActuals, ref arityMessage))
                    return;
            }

            // Function not found
            HasLanguageException = true;
            if (arityMessage != null)
                Trace.LogException(callAction, "{0} does not take {1} argument{2}",
                                   arityMessage, nActuals, Utilities.GetPluralSuffix(nActuals));
            else
                Trace.LogException(callAction, "Reference to undefined function: {0}", callAction.FunctionName);
        }

        private int GetNFormalsForSpecialForm(string functionName)
        {
            switch (functionName)
            {
                case "Eval"                       : return 1;
                case "If"                         : return 3;
                case "Repeat"                     : return 2;
                default                           : return -99; // No such built-in
            }
        }

        private Function GetUserFunction(string functionName)
        {
            Function function;
            // Look in my function definitions
            if (Functions.TryGetValue(functionName, out function))
                return function;
            // Look (recursively) in ancestor command sets
            if (ParentCommandSet != null)
                return ParentCommandSet.GetUserFunction(functionName);
            return null;
        }

        private bool BindToExtensionFunctionUsing(CallAction callAction, string functionName, int nActuals, ref string arityMessage)
        {
            foreach (string prefix in callAction.UsingSet)
                if (BindToExtensionFunction(callAction, prefix + "." + functionName, nActuals, ref arityMessage))
                    return true;
            foreach (string prefix in Vocola.BaseUsingSet)
                if (BindToExtensionFunction(callAction, prefix + "." + functionName, nActuals, ref arityMessage))
                    return true;
            return false;
        }

        private bool BindToExtensionFunction(CallAction callAction, string functionName, int nActuals, ref string arityMessage)
        {
            MethodInfo nativeMethod = Extensions.GetMethod(functionName, nActuals);
            if (nativeMethod != null)
            {
                callAction.NativeMethod = nativeMethod;
                return true;
            }
            else if (Extensions.MethodExists(functionName) && arityMessage == null)
                arityMessage = String.Format("Extension function '{0}'", functionName);
            return false;
        }

        // ---------------------------------------------------------------------
        // Run onLoad() function if defined

        public void RunLoadFunction()
        {
            Function loadFunction = GetUserFunction("onLoad");
            if (loadFunction != null)
            {
                Trace.WriteLine(LogLevel.High, "Running onLoad() function");
                try
                {
                    ActionRunner.RunActions(loadFunction.Actions);
                }
                catch (Exception ex)
                {
                    Trace.LogExecutionException(ex);
                }
            }
        }

        // ---------------------------------------------------------------------
        // Clear cached rules

        public void ClearCachedRules()
        {
            ClearCachedRules(Commands);
            foreach (Variable variable in Variables.Values)
            {
                variable.Rule = null;
                ClearCachedRules(variable.Menu.Alternatives);
            }
            foreach (List<CommandSet> ifGroup in ConditionalCommandSets)
                foreach (CommandSet cs in ifGroup)
                    cs.ClearCachedRules();
        }

        private void ClearCachedRules(List<Command> commands)
        {
            foreach (Command command in commands)
            {
                command.Rule = null;
                foreach (object term in command.Terms)
                    if (term is MenuTerm)
                        ClearCachedRules(((MenuTerm)term).Alternatives);
            }
        }

        // ---------------------------------------------------------------------
        // Collect rules to activate for the current context

        // If my commands are appropriate for the given context, add
        // each to the given grammar. Recurse to imported and child
        // command sets.

        public void Activate(ActiveCommands activeCommands)
        {
            Activate(activeCommands, 1);
        }

        private bool Activate(ActiveCommands activeCommands, int nestingLevel)
        {
            VocolaContext context = activeCommands.VocolaContext;
            bool shouldActivate = ((AppName == null || AppName == context.AppName) && MatchesWindowTitle(context.WindowTitle));
            if (shouldActivate)
            {
                string indent = new String(' ', nestingLevel * 3);
                int nCommands = CountCommands(context.WindowTitle);
                if (nestingLevel == 1)
                {
                    if (LoadedFile != null)
                        if (LoadedFile.IsBuiltinsFile)
                            Trace.WriteLine(LogLevel.Low, "{0}{1} ({2}, built-in)", indent, LoadedFile.Filename, nCommands);
                        else
                            Trace.WriteLine(LogLevel.Medium, "{0}{1} ({2})", indent, LoadedFile.Filename, nCommands);
                }
                else
                {
                    string patterns = String.Format("'{0}'", String.Join(" | ", (string[])WindowTitlePatterns.ToArray(typeof(String))));
                    if (patterns == "''")
                        patterns = "$else";
                    Trace.WriteLine(LogLevel.Medium, "{0}Context: {1} ({2})", indent, patterns, nCommands);
                }

                // Add my variables
                foreach (Variable variable in Variables.Values)
                    if (!variable.IsReserved)
                        activeCommands.AddVariable(variable);

                // Add my commands
                foreach (Command command in Commands)
                    activeCommands.AddCommand(command);

                // Add commands from conditional command sets
                foreach (List<CommandSet> ifGroup in ConditionalCommandSets)
                    foreach (CommandSet cs in ifGroup)
                        if (cs.Activate(activeCommands, nestingLevel + 1))
                            break;
            }
            return shouldActivate;
        }

        private bool MatchesWindowTitle(string windowTitle)
        {
            if (WindowTitlePatterns.Count == 0)
                return true;
            foreach (string pattern in WindowTitlePatterns)
                if (windowTitle.IndexOf(pattern) > -1)
                    return true;
            return false;
        }

        private int CountCommands(string windowTitle)
        {
            if (MatchesWindowTitle(windowTitle))
            {
                int nCommands = Commands.Count;
                foreach (List<CommandSet> ifGroup in ConditionalCommandSets)
                    foreach (CommandSet cs in ifGroup)
                        nCommands += cs.CountCommands(windowTitle);
                return nCommands;
            }
            else
                return 0;
        }

        // ---------------------------------------------------------------------
        // Find a command (for NatLink recognizer). Inefficient, but may not matter.

        static public Command GetCommand(string commandId)
        {
            foreach (LoadedFile loadedFile in LoadedFile.LoadedFiles.Values)
                if (loadedFile.CommandSet != null)
                {
                    Command command = loadedFile.CommandSet.ReallyGetCommand(commandId);
                    if (command != null)
                        return command;
                }
            return null;
        }

        private Command ReallyGetCommand(string commandId)
        {
            foreach (Command command in Commands)
                if (command.UniqueId == commandId)
                    return command;
            // Search conditional command sets
            foreach (List<CommandSet> ifGroup in ConditionalCommandSets)
                foreach (CommandSet cs in ifGroup)
                    foreach (Command command in cs.Commands)
                        if (command.UniqueId == commandId)
                            return command;
            return null;
        }           

        // ---------------------------------------------------------------------
        // Dump for debugging

        public void WriteDebug()
        {
            Debug.WriteLine("");
            foreach (Variable variable in Variables.Values)
                Debug.WriteLine(variable);

            Debug.WriteLine("");
            foreach (Function function in Functions.Values)
                Debug.WriteLine(function);

            Debug.WriteLine("");
            foreach (Command command in Commands)
                Debug.WriteLine(command);

            foreach (List<CommandSet> ifGroup in ConditionalCommandSets)
                foreach (CommandSet cs in ifGroup)
                    cs.Bind();

            foreach (List<CommandSet> ifGroup in ConditionalCommandSets)
            {
                Debug.WriteLine("");
                bool first = false;
                foreach (CommandSet cs in ifGroup)
                {
                    string context = String.Join("|", (string[])cs.WindowTitlePatterns.ToArray(typeof(String)));
                    if (first)
                    {
                        Debug.WriteLine("$if " + context + ";");
                        first = false;
                    }
                    else if (context != "")
                        Debug.WriteLine("$elseif " + context + ";");
                    else
                        Debug.WriteLine("$else");
                    Debug.Indent();
                    cs.WriteDebug();
                    Debug.Unindent();
                    Debug.WriteLine("$end");
                }
            }
        }

    }

    // ---------------------------------------------------------------------
    // Collects active commands and variables
    
    public class ActiveCommands
    {
        public VocolaContext VocolaContext;
        public Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
        public Dictionary<string, Command > Commands  = new Dictionary<string, Command >();

        public ActiveCommands(VocolaContext context)
        {
            this.VocolaContext = context;
            Trace.WriteLine(LogLevel.Medium, "Enabling commands:");
            foreach (LoadedFile loadedFile in LoadedFile.LoadedFiles.Values)
            {
                if (loadedFile.ShouldActivateCommands())
                    loadedFile.CommandSet.Activate(this);
            }
            if (Vocola.RequireControlNamePrefix)
                Vocola.SpokenControlNameCommandSet.Activate(this);
        }

        public void AddVariable(Variable variable)
        {
            if (!Variables.ContainsKey(variable.UniqueId))
                Variables[variable.UniqueId] = variable;
        }

        public void AddCommand(Command command)
        {
            if (!Commands.ContainsKey(command.UniqueId))
                Commands[command.UniqueId] = command;
        }

        public Command GetCommand(string commandId)
        {
            if (Commands.ContainsKey(commandId))
                return Commands[commandId];
            else
                return null;
        }

    }

}


