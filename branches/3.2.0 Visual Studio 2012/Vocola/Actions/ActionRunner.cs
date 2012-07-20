using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Vocola
{

    public class ActionsQueue
    {
        private List<CommandActions> TheQueue = new List<CommandActions>();

        public class CommandActions
        {
            public ArrayList Actions;
            public Dictionary<string, BoundValue> Environment = new Dictionary<string, BoundValue>();
        }

        public void AddActions(ArrayList actions, List<ArrayList> variableTermActions)
        {
            CommandActions ca = new CommandActions();
            ca.Actions = actions;
            // Make a binding environment for resolving references like $2
            // For example, bind "2" to the actions specified for the second variable term 
            for (int i = 0; i < variableTermActions.Count; i++)
                ca.Environment[(i+1).ToString()] = new BoundValue(variableTermActions[i]);
            TheQueue.Add(ca);
        }

        public List<CommandActions> GetQueue()
        {
            return TheQueue;
        }
    }

    public class EmulateRecognizeDepth
    {
        static private Dictionary<int, int> DepthByThread = new Dictionary<int, int>();
        static public void Initialize () {        DepthByThread       [Thread.CurrentThread.ManagedThreadId] = 0; }
        static public int  Get        () { return DepthByThread       [Thread.CurrentThread.ManagedThreadId]    ; }
        static public int  Increment  () { return DepthByThread       [Thread.CurrentThread.ManagedThreadId]++  ; }
        static public int  Decrement  () { return DepthByThread       [Thread.CurrentThread.ManagedThreadId]--  ; }
        static public bool Initialized() { return DepthByThread.ContainsKey(Thread.CurrentThread.ManagedThreadId)    ; }
        static public void Remove     () {        DepthByThread.Remove(Thread.CurrentThread.ManagedThreadId)    ; }
    }

    public class ActionRunner
    {

        static public void Launch(ActionsQueue actionsQueue)
        {
            // Launch thread to run actions from a sequence of commands
            Thread thread = new Thread(new ParameterizedThreadStart(LaunchActions));
            thread.SetApartmentState(ApartmentState.STA);  // Otherwise Clipboard.GetDataObject() fails 
            thread.Name = "Action Runner Thread";
            thread.Start(actionsQueue);
        }

        static private void LaunchActions(object actionsQueue)
        {
            try
            {
                EmulateRecognizeDepth.Initialize();
                RunActions((ActionsQueue)actionsQueue);
                EmulateRecognizeDepth.Remove();
            }
            catch (Exception ex)
            {
                // Problems encountered while handling a spoken phrase are detected
                // lower down and thrown to be logged here.
                // In addition we catch anything unexpected here so we don't crash.
                Trace.LogExecutionException(ex);
            }
        }

        // ---------------------------------------------------------------------
        // Entry points that don't launch a thread

        static public void RunActions(ActionsQueue actionsQueue)
        {
            foreach (ActionsQueue.CommandActions ca in actionsQueue.GetQueue())
            {
                Atoms atoms = SimplifyActions(ca.Actions, ca.Environment);
                atoms.Run();
            }
        }

        static public void RunActions(ArrayList actions)
        {
            Atoms atoms = SimplifyActions(actions, null);
            atoms.Run();
        }

        static public string EvaluateActions(ArrayList actions)
        {
            return EvaluateActions(actions, null);
        }

        static private string EvaluateActions(ArrayList actions, Dictionary<string, BoundValue> environment)
        {
            Atoms atoms = SimplifyActions(actions, environment);
            return atoms.Evaluate();
        }

        // ---------------------------------------------------------------------
        // Simplify actions by converting them to atoms (primitive actions):
        //     - Resolve references using environment
        //     - Unroll calls to user functions
        //     - Evaluate If() predicates and choose branch
        //     - Execute calls to Eval()

        static public Atoms SimplifyActions(ArrayList actions, Dictionary<string, BoundValue> environment)
        {
            Atoms atoms = new Atoms();
            foreach (object action in actions)
                SimplifyAction(action, environment, atoms);
            return atoms;
        }

        static private void SimplifyAction(object action, Dictionary<string, BoundValue> environment, Atoms atoms)
        {
            if (action is KeysAction)
            {
                atoms.Add((action as KeysAction).Keys);
            }
            else if (action is ReferenceAction)
            {
                string name = (action as ReferenceAction).Name;
                if (environment == null || !environment.ContainsKey(name))
                    throw new InternalException("Reference ${0} not found in recognized semantics", name);
                atoms.Add(environment[name].Atoms);
            }
            else if (action is CallAction)
            {
                SimplifyCallAction(action as CallAction, environment, atoms);
            }
            else if (action is EvalAction)
            {
                EvalAction evalAction = action as EvalAction;
                List<string> variableActionStrings = new List<string>();
                ArrayList a = new ArrayList(); a.Add(null);
                foreach (object va in evalAction.VariableActions)
                {
                    a[0] = va;
                    variableActionStrings.Add(EvaluateActions(a, environment));
                }
                string result = evalAction.Evaluate(variableActionStrings);
                Trace.WriteLine(LogLevel.Low, "    Eval() result: '{0}'", result);
                atoms.Add(result);
            }
            else
                throw new InternalException("Unexpected action: " + action.ToString());
        }

        static private void SimplifyCallAction(CallAction call, Dictionary<string, BoundValue> environment, Atoms atoms)
        {
            if (call.FunctionName == "If")
            {
                // Call is to "If" special form
                string predicate = EvaluateActions(call.Arguments[0], environment);
                int branch = (predicate.ToLower() == "true" ? 1 : 2);
                atoms.Add(SimplifyActions(call.Arguments[branch], environment));
            }
            else if (call.FunctionName == "Repeat")
            {
                // Call is to "Repeat" special form
                string countString = EvaluateActions(call.Arguments[0], environment);
                int count;
                if (!Int32.TryParse(countString, out count))
                    throw new ActionException(call, "Invalid first argument to Repeat(): '{0}'", countString);
                Atoms body = SimplifyActions(call.Arguments[1], environment);
                atoms.Add(new RepeatAtom(count, body));
            }
            else if (call.UserFunction != null)
            {
                // Call is to user function -- simplify
                Dictionary<string, BoundValue> bindings = new Dictionary<string, BoundValue>();
                Function function = call.UserFunction;
                for (int i = 0; i < function.Formals.Count; i++)
                    bindings[ (string)function.Formals[i] ] = new BoundValue(call.Arguments[i], environment);
                atoms.Add(SimplifyActions(function.Actions, bindings));
            }
            else if (call.NativeMethod != null)
            {
                // Call is to native function
                // Simplify arguments
                List<Atoms> argumentAtoms = new List<Atoms>();
                foreach (ArrayList argumentActions in call.Arguments)
                    argumentAtoms.Add(SimplifyActions(argumentActions, environment));

                atoms.Add(new Thunk(call, argumentAtoms));
            }
            else
            {
                throw new ActionException(call, "Call to undefined function: {0}", call.FunctionName);
            }
        }

    }

    // ---------------------------------------------------------------------
    // An action sequence packaged to be the value of an environment binding

    public class BoundValue
    {
        private ArrayList Actions;
        Dictionary<string, BoundValue> Environment = null;
        private Atoms atoms = null;

        public BoundValue(ArrayList actions)
        {
            Actions = actions;
        }

        public BoundValue(ArrayList actions, Dictionary<string, BoundValue> environment)
        {
            Actions = actions;
            Environment = environment;
        }

        public Atoms Atoms
        {
            get
            {
                if (atoms == null)
                    atoms = ActionRunner.SimplifyActions(Actions, Environment);
                return atoms;
            }
        }
    }

    // ---------------------------------------------------------------------
    // Atoms are primitive actions -- strings, native function calls (see Thunk class), and Repeat()

    public class Atom {}

    class RepeatAtom : Atom
    {
        public int Count;
        public Atoms Atoms;

        public RepeatAtom(int count, Atoms atoms)
        {
            Count = count;
            Atoms = atoms;
        }
    }


    public class Atoms
    {
        public ArrayList AtomList = new ArrayList();

        public void Add(string keys)
        {
            // Concatenate to final atom if possible
            int finalIndex = AtomList.Count - 1;
            if (finalIndex >= 0 && AtomList[finalIndex] is string)
                AtomList[finalIndex] = (AtomList[finalIndex] as string) + keys;
            else
                AtomList.Add(keys);
        }

        public void Add(Atom atom)
        {
            AtomList.Add(atom);
        }

        public void Add(Atoms atoms)
        {
            foreach (object atom in atoms.AtomList)
            {
                if (atom is string)
                    this.Add(atom as string);
                else if (atom is Atom)
                    this.Add(atom as Atom);
                else
                    throw new InternalException("Unexpected atom");
            }
        }

        public bool IsSingleString()
        {
            return (AtomList.Count == 1 && AtomList[0] is string);
        }

        public string Evaluate()
        {
            if (IsSingleString())
                return AtomList[0] as string;
            StringBuilder sb = new StringBuilder();
            foreach (object atom in AtomList)
            {
                if (atom is string)
                    sb.Append(atom as string);
                else if (atom is Thunk)
                {
                    var thunk = atom as Thunk;
                    //if (thunk.ReturnsVoid)
                    //    throw new ActionException(null, "attempt to call Unimacro or make a Dragon call in a functional context!");
                    FlushKeystrokesIfNecessary(thunk);   // for e.g. {Ctrl+c} String.ToCamelCaseWord( Clipboard.GetText() );
                    sb.Append(thunk.Execute());
                }
                else if (atom is RepeatAtom)
                {
                    RepeatAtom repeatAtom = (atom as RepeatAtom);
                    for (int i = 0; i < repeatAtom.Count; i++)
                        sb.Append(repeatAtom.Atoms.Evaluate());
                }
            }
            return sb.ToString();
        }

        private static StringBuilder KeystrokeBuffer;

        public void Run()
        {
            KeystrokeBuffer = new StringBuilder();
            ReallyRun();
            if (KeystrokeBuffer.Length > 0)
                Keystrokes.SendKeys(KeystrokeBuffer.ToString());
        }

        private void ReallyRun()
        {
            foreach (object atom in AtomList)
            {
                if (atom is string)
                {
                    Dictation.Clear();  // Do this first so "Fix Space" will work after correction
                    KeystrokeBuffer.Append(atom as string);
                }
                else if (atom is Thunk)
                {
                    var thunk = atom as Thunk;
                    FlushKeystrokesIfNecessary(thunk);
                    string result = thunk.Execute();
                    if (result != null && result != "")
                        KeystrokeBuffer.Append(result);
                }
                else if (atom is RepeatAtom)
                {
                    RepeatAtom repeatAtom = (atom as RepeatAtom);
                    for (int i = 0; i < repeatAtom.Count; i++)
                        repeatAtom.Atoms.ReallyRun();
                }
            }
        }

        private void FlushKeystrokesIfNecessary(Thunk thunk)
        {
            if (thunk.ShouldFlushKeystrokesBeforeCall && KeystrokeBuffer != null && KeystrokeBuffer.Length > 0)
            {
                Keystrokes.SendKeys(KeystrokeBuffer.ToString());
                KeystrokeBuffer.Clear();
            }
        }

    }
}
