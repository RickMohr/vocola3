using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PerCederberg.Grammatica.Parser; // Node

namespace Vocola
{

    public class LanguageObject
    {
        public Node ParserNode;
        public string SourceFilename;

        protected string ArrayListToString(ArrayList a, string joiner)
        {
            if (a == null)
                return "";
            int n = a.Count;
            string[] strings = new string[n];
            for (int i = 0; i < n; i++)
                strings[i] = a[i].ToString();
            return String.Join(joiner, strings);
        }

        protected string ListToString(object[] a, string joiner)
        {
            int n = a.Length;
            string[] strings = new string[n];
            for (int i = 0; i < n; i++)
                strings[i] = a[i].ToString();
            return String.Join(joiner, strings);
        }

    }

    // ---------------------------------------------------------------------
    // Top level

    public class Command : LanguageObject
    {
        public ArrayList Terms;
        public ArrayList Actions;
        public bool IsGlobal;
        public bool IsInternal;
        public bool HasWildcardTerm;
        public string UniqueId;
        public object Rule;
        private string Text;

        public Command(ArrayList terms, ArrayList actions)
        {
            Terms = terms;
            Actions = actions;
            Text = GetCommandText();
        }

        private string GetCommandText()
        {
            string terms = ArrayListToString(Terms, " ");
            string actions = ArrayListToString(Actions, " ");
            return (terms == actions || Actions == null ? terms : terms + " = " + actions);
        }

        public override string ToString()
        {
            return Text;
        }

        public string TermsToString()
        {
            return ArrayListToString(Terms, " ");
        }
    }

    public class Function : LanguageObject
    {
        public string Name;
        public ArrayList Formals;
        public ArrayList Actions;

        public Function(string name, ArrayList formals, ArrayList actions)
        {
            Name = name;
            Formals = formals;
            Actions = actions;
        }

        public override string ToString()
        {
            return Name + "(" + ArrayListToString(Formals, ", ") + ") := " + ArrayListToString(Actions, " ") + ";";
        }
    }

    public class Variable : LanguageObject
    {
        public string Name;
        public MenuTerm Menu;
        public string UniqueId;
        public object Rule;
        public bool IsReserved;

        public Variable(string name)
        {
            Name = name;
            Menu = new MenuTerm(new List<Command>());
            IsReserved = true;
        }

        public Variable(string name, MenuTerm menu)
        {
            Name = name;
            Menu = menu;
            IsReserved = false;
        }

        public override string ToString()
        {
            return "<" + Name + "> := " + Menu.ToString() + ";";
        }
    }

    // ---------------------------------------------------------------------
    // Terms

    public class WordTerm : LanguageObject
    {
        public string Text;
        public bool IsOptional;

        public WordTerm(string text)
        {
            Text = text;
            IsOptional = false;
        }

        public override string ToString()
        {
            return IsOptional ? "[" + Text + "]" :  Text;
        }

    }

    public class VariableTerm : LanguageObject
    {
        public string Name;
        public Variable Variable;   // looked up at bind time

        public VariableTerm(string name, string sourceFilename, Node parserNode) {
            Name = name;
            Variable = null;
            SourceFilename = sourceFilename;
            ParserNode = parserNode;
        }

        public override string ToString()
        {
            return "<" + Name + ">";
        }
    }

    public class RangeTerm : LanguageObject
    {
        public int From;
        public int To;

        public RangeTerm(int from, int to) {
            From = from;
            To = to;
        }

        public override string ToString() {
            return From.ToString() + ".." + To.ToString();
        }
    }

    public class MenuTerm  : LanguageObject
    {
        public List<Command> Alternatives;

        public MenuTerm(List<Command> alternatives)
        {
            Alternatives = alternatives;
        }

        public override string ToString()
        {
            return "(" + ListToString(Alternatives.ToArray(), " | ") + ")";
        }
    }

    // ---------------------------------------------------------------------
    // Actions

    public class KeysAction : LanguageObject
    {
        public string Keys;

        public KeysAction(string keys)
        {
            Keys = keys;
        }

        public override string ToString()
        {
            return Keys;
        }
    }

    public class ReferenceAction : LanguageObject
    {
        public string Name;

        public ReferenceAction(string name, string sourceFilename, Node parserNode) {
            ParserNode = parserNode;
            Name = name;
            ParserNode = parserNode;
            SourceFilename = sourceFilename;
        }

        public override string ToString()
        {
            return "$" + Name;
        }
    }

    public class CallAction : LanguageObject
    {
        public string FunctionName;
        public List<string> UsingSet;
        public Function UserFunction; // resolved at bind time
        public MethodInfo NativeMethod; // resolved at bind time
        public List<ArrayList> Arguments; // list of lists of actions, to be passed in call

        public CallAction(string functionName, List<ArrayList> arguments, string sourceFilename, List<string> usingSet, Node parserNode) {
            FunctionName = functionName;
            Arguments = arguments;
            ParserNode = parserNode;
            SourceFilename = sourceFilename;
            UsingSet = usingSet;
        }

        public override string ToString()
        {
            ArrayList args = new ArrayList();
            foreach (ArrayList actions in Arguments)
                args.Add(ArrayListToString(actions, " "));
            return FunctionName + "(" + ArrayListToString(args, ", ") + ")";
        }
    }

}

