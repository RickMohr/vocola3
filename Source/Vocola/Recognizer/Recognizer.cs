using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Vocola
{

    public class Recognizer
    {
        // Subclass responsibility
        public virtual void Initialize() {}
        public virtual bool SupportsDictation { get { return false; } }
		public virtual bool RunDevelopmentVersionFromProgramFiles { get { return false; } }
		public virtual void CommandFileChanged(LoadedFile loadedFile) { }
        public virtual void ContextChanged(VocolaContext context) {}
        public virtual void EmulateRecognize(string words) {}
        public virtual void DisplayMessage(string message, bool isWarning) {}

        // ---------------------------------------------------------------------
        // Manage and access term alternates

        private Dictionary<string, List<string>> TermAlternates = new Dictionary<string, List<string>>();

        public void AddTermAlternate(string term, string alternate)
        {
            if (term.Contains(" "))
                throw new ActionException(null, "Multi-word term may not have alternates: '{0}'", term);
            List<string> alternates;
            term = term.ToLower();
            if (!TermAlternates.ContainsKey(term))
            {
                alternates = new List<string>();
                TermAlternates[term] = alternates;
            }
            else
            {
                alternates = TermAlternates[term];
                foreach (string a in alternates)
                    if (alternate == a)
                        return;
            }
            alternates.Add(alternate);
            LoadedFile.ClearCachedRules();
        }

        protected bool TermHasAlternates(string word)
        {
            return TermAlternates.ContainsKey(word.ToLower());
        }

        protected bool TermHasAlternates(string[] words)
        {
            foreach (string word in words)
                if (TermHasAlternates(word))
                    return true;
            return false;
        }

        protected List<string> GetAlternates(string term)
        {
            List<string> alternates = null;
            TermAlternates.TryGetValue(term.ToLower(), out alternates);
            return alternates;
        }

        // ---------------------------------------------------------------------
        // Build a flat list of (canonicalized) commands:
        //     - recursively extract commands from nested menus
        // Note that error checking happened during parsing, in ParseMenuBody()

        protected static void FlattenMenu(MenuTerm menu)
        {
            //Trace.WriteLine(LogLevel.Low, "Menu  pre-flatten: {0}", menu);
            List<Command> flattenedAlternatives = new List<Command>();
            FlattenMenu(menu, flattenedAlternatives, null);
            menu.Alternatives = flattenedAlternatives;
            //Trace.WriteLine(LogLevel.Low, "Menu post-flatten: {0}", menu);
        }

        private static void FlattenMenu(MenuTerm menu, List<Command> flattenedAlternatives, ArrayList actionsToDistribute)
        {
            foreach (Command command in menu.Alternatives)
            {
                object term = command.Terms[0];
                if (term is WordTerm)
                {
                    if (command.Actions == null)
                    {
                        // No actions specified, so use term as action
                        WordTerm wordTerm = (term as WordTerm);
                        command.Actions = new ArrayList();
                        command.Actions.Add(new KeysAction(wordTerm.Text));
                    }
                    if (actionsToDistribute != null)
                        ReplaceActions(command, actionsToDistribute);
                    flattenedAlternatives.Add(command);
                }
                else
                {
                    ArrayList newActionsToDistribute = actionsToDistribute;
                    if (command.Actions != null)
                        newActionsToDistribute = command.Actions;
                    if (term is MenuTerm)
                    {
                        FlattenMenu(term as MenuTerm, flattenedAlternatives, newActionsToDistribute);
                    }
                    else if (term is VariableTerm)
                    {
                        Variable variable = (term as VariableTerm).Variable;
                        if (variable != null)
                            FlattenMenu(variable.Menu, flattenedAlternatives, newActionsToDistribute);
                    }
                    else if (term is RangeTerm)
                    {
                        RangeTerm range = term as RangeTerm;
                        for (int i = range.From; i <= range.To; i++)
                        {
                            string s = (i >=0 && i <= 100 ? NumberWords[i] : i.ToString());
                            ArrayList terms   = new ArrayList(); terms.Add(new WordTerm(s));
                            ArrayList actions = new ArrayList(); actions.Add(new KeysAction(i.ToString()));
                            Command c = new Command(terms, actions);
                            if (newActionsToDistribute != null)
                                ReplaceActions(c, newActionsToDistribute);
                            flattenedAlternatives.Add(c);
                        }
                    }
                }
            }
        }

        private static void ReplaceActions(Command command, ArrayList replacementActions)
        {
            ArrayList newActions = new ArrayList();
            foreach (LanguageObject action in replacementActions)
            {
                if (action is ReferenceAction)
                {
                    string referenceName = ((ReferenceAction)action).Name;
                    if (referenceName == "1")
                        foreach (LanguageObject commandAction in command.Actions)
                            newActions.Add(commandAction);
                    else
                        Trace.LogException(action, "Reference ${0} not valid", referenceName);
                }
                else
                    newActions.Add(action);
            }
            command.Actions = newActions;
        } 

        // Using text instead of digits increases recognition speed
        private static string[] NumberWords = new string[] {
            "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
            "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen",
            "Twenty", "Twenty One", "Twenty Two", "Twenty Three", "Twenty Four", "Twenty Five", "Twenty Six", "Twenty Seven", "Twenty Eight", "Twenty Nine",
            "Thirty", "Thirty One", "Thirty Two", "Thirty Three", "Thirty Four", "Thirty Five", "Thirty Six", "Thirty Seven", "Thirty Eight", "Thirty Nine",
            "Forty", "Forty One", "Forty Two", "Forty Three", "Forty Four", "Forty Five", "Forty Six", "Forty Seven", "Forty Eight", "Forty Nine",
            "Fifty", "Fifty One", "Fifty Two", "Fifty Three", "Fifty Four", "Fifty Five", "Fifty Six", "Fifty Seven", "Fifty Eight", "Fifty Nine",
            "Sixty", "Sixty One", "Sixty Two", "Sixty Three", "Sixty Four", "Sixty Five", "Sixty Six", "Sixty Seven", "Sixty Eight", "Sixty Nine",
            "Seventy", "Seventy One", "Seventy Two", "Seventy Three", "Seventy Four", "Seventy Five", "Seventy Six", "Seventy Seven", "Seventy Eight", "Seventy Nine",
            "Eighty", "Eighty One", "Eighty Two", "Eighty Three", "Eighty Four", "Eighty Five", "Eighty Six", "Eighty Seven", "Eighty Eight", "Eighty Nine",
            "Ninety", "Ninety One", "Ninety Two", "Ninety Three", "Ninety Four", "Ninety Five", "Ninety Six", "Ninety Seven", "Ninety Eight", "Ninety Nine",
            "One Hundred"
        };

    }

}
