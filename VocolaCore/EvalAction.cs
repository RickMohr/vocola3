using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PerCederberg.Grammatica.Parser; // Node

namespace Vocola
{
    public class EvalAction : LanguageObject
    {
        public ArrayList ExpressionActions; // Expression to be evaluated
        private MethodInfo Method;           // Compiled JavaScript method to invoke

        public EvalAction(ArrayList expressionActions, string sourceFilename, Node parserNode)
        {
            ExpressionActions = expressionActions;
            ParserNode = parserNode;
            SourceFilename = sourceFilename;
            Method = null;
        }

        public override string ToString()
        {
            return "Eval(" + ArrayListToString(ExpressionActions, "") + ")";
        }

        private ArrayList variableActions; // non-keystroke actions

        public ArrayList VariableActions
        {
            get
            {
                if (variableActions == null)
                {
                    variableActions = new ArrayList();
                    foreach (object action in ExpressionActions)
                        if (!(action is KeysAction))
                            variableActions.Add(action);
                }
                return variableActions;
            }
        }

        public string Evaluate(List<string> argumentStrings)
        {
            if (Method == null)
            {
                // Create a JavaScript function to do the evaluation.
                string jsText = BuildJavaScriptFunction();

                // Compile to an assembly
                Assembly assembly = Compile(jsText);

                // Find the class and method
                try
                {
                    Type type = assembly.GetType("theClass");
                    Method = type.GetMethod("theMethod", BindingFlags.Public | BindingFlags.Static);
                }
                catch (Exception) {}
                if (Method == null)
                    throw new InternalException("Eval() could not find constructed JavaScript method");
            }

            // Convert arguments to integers if possible
            object[] arguments = new object[argumentStrings.Count];
            for (int i = 0; i < argumentStrings.Count; i++)
            {
                int intResult;
                if (Int32.TryParse(argumentStrings[i], out intResult))
                    arguments[i] = intResult;
                else
                    arguments[i] = argumentStrings[i];
            }

            // Evaluate the expression by invoking the JavaScript method
            try
            {
                return Method.Invoke(null, arguments).ToString();
            }
            catch (TargetInvocationException tiex)
            {
                Exception ex = (tiex.InnerException == null ? tiex : tiex.InnerException);
                throw new ActionException(this, "Error in Eval(): " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new ActionException(this, "Error in Eval(): " + ex.Message);
            }
        }

        // Example, for "Eval($1 + 1)":
        // 
        //     class theClass
        //     {
        //         static function theMethod(v1)
        //         {
        //             return v1 + 1;
        //         }
        //     }

        private string BuildJavaScriptFunction()
        {
            StringBuilder sb = new StringBuilder();

            // Argument list (e.g. "v1,v2,v3,v4")
            string formals = "";
            int nVariableActions = VariableActions.Count;
            if (nVariableActions > 0)
            {
                formals += "v1";
                for (int i = 2; i <= nVariableActions; i++)
                    formals += String.Format(",v{0}", i) ;
            }

            // Header
            sb.AppendLine  ("class theClass");
            sb.AppendLine  ("{");
            sb.AppendFormat("    static function theMethod({0})\n", formals);
            sb.AppendLine  ("    {");

            // The expression to be evaluated
            sb.Append("        return ");
            int v = 1;
            foreach (object action in ExpressionActions)
                if (action is KeysAction)
                    sb.Append((action as KeysAction).Keys);
                else
                    sb.AppendFormat("v{0}", v++);
            sb.Append(";");

            // Footer
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private Assembly Compile(string code)
        {
            CodeDomProvider provider = new Microsoft.JScript.JScriptCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.IncludeDebugInformation = false;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.CompilerOptions = "/fast-";

            CompilerResults results;
            try
            {
                // Compile. (Documentation says never returns null)
                results = provider.CompileAssemblyFromSource(parameters, code);
            }
            catch (Exception ex)
            {
                throw new ActionException(this, "Compile failed for Eval(): " + ex.Message);
            }
            if (results.NativeCompilerReturnValue > 0)
            {
                // Compile failed -- log the compiler output
                string output = "";
                foreach (string s in results.Output)
                    output += s + "\r\n";
                throw new ActionException(this, "Compile failed for Eval(): " + output);
            }
            return results.CompiledAssembly;
        }

    }
}
