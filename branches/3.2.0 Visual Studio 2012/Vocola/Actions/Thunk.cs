using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices; // COMException
using System.Text;
using System.Threading; // Sleep

namespace Vocola
{

    class Thunk : Atom
    {
        private CallAction Call;
        private MethodInfo Method;
        private List<Atoms> ArgumentAtoms;

        public Thunk(CallAction call, List<Atoms> argumentAtoms)
        {
            Call = call;
            Method = call.NativeMethod;
            ArgumentAtoms = argumentAtoms;
        }

        public bool ReturnsVoid { get { return (Method.ReturnType == typeof(void)); }}

        // ---------------------------------------------------------------------

        public string Execute()
        {
            object[] actuals = GetActuals();

            if (Trace.LevelThreshold == LogLevel.Low)
                TraceCall(Method, actuals);

            string result = InvokeMethod(actuals);
            if (!ReturnsVoid)
                Trace.WriteLine(LogLevel.Low, "      Return value: '{0}'", result);

            // Clear dictation stack if appropriate
            // Methods returning void - clear dictation stack by default
            // Methods returning a value - preserve dictation stack by default
            // Methods can override default by declaring [ClearDictationStack] attribute
            bool shouldClear = ReturnsVoid;
            object[] attributes = Method.GetCustomAttributes(typeof(ClearDictationStack), false);
            if (attributes.Length > 0)
                shouldClear = (attributes[0] as ClearDictationStack).ShouldClear;
            if (shouldClear)
                Dictation.Clear();

            return result;
        }

        private string InvokeMethod(object[] actuals)
        {
            try
            {
                object result = ReallyInvokeMethod(actuals);
                if (result == null)
                    return "";
                else
                    return result.ToString();
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    TargetInvocationException tiex = ex as TargetInvocationException;
                    ex = (tiex.InnerException == null ? tiex : tiex.InnerException);
                }
                if (ex is ExceptionWrapper)
                    throw ex;
                else
                    throw new ExceptionWrapper(ex, Call);
            }
        }

        private object ReallyInvokeMethod(object[] actuals)
        {
            // Guard against "COM busy" exceptions by retrying
            int nTries = 3;
            while (true)
            {
                try
                {
                    return Method.Invoke(null, actuals);
                }
                catch (TargetInvocationException tiex)
                {
                    Exception ex = tiex.InnerException;
                    if (ex is COMException
                        && (uint)((ex as COMException).ErrorCode) == 0x8001010a // RPC_E_SERVERCALL_RETRYLATER
                        && nTries-- > 0)
                    {
                        Trace.WriteLine(LogLevel.Low, "COM server busy, retrying...");
                        Thread.Sleep(100);
                    }
                    else
                        throw tiex;
                }
            }
        }

        private void TraceCall(MethodInfo method, object[] actuals)
        {
            string call;
            string functionName = method.DeclaringType.FullName + "." + method.Name;
            if (actuals.Length == 0)
                call = String.Format("{0}()", functionName);
            else
            {
                string[] arguments = new string[actuals.Length];
                for (int i = 0; i < actuals.Length; i++)
                    if (actuals[i] is string)
                        arguments[i] = String.Format("'{0}'", (string)actuals[i]);
                    else if (actuals[i] is string[])
                        arguments[i] = String.Join(", ", (string[])actuals[i]);
                    else
                        arguments[i] = actuals[i].ToString();
                string args = String.Join(", ", arguments);
                call = String.Format("{0}({1})", functionName, args);
            }
            Trace.WriteLine(LogLevel.Low, "    Call: {0}", call);
        }

        // ---------------------------------------------------------------------

        private object[] GetActuals()
        {
            List<string> argumentStrings = GetArgumentStrings(ArgumentAtoms);
            ParameterInfo[] formals = Method.GetParameters();
            int nFormals = formals.Length;
            int nActuals = ArgumentAtoms.Count;
            object[] actuals = new object[nFormals];
            if (nFormals > 0)
            {
                ParameterInfo finalFormal = formals[nFormals - 1];
                if (finalFormal.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
                {
                    // Final parameter has "params" keyword
                    ConvertArguments(formals, argumentStrings, actuals, nFormals - 1);
                    int iFirstParamActual = nFormals - 1;
                    int nParamActuals = nActuals - nFormals + 1;
                    actuals[iFirstParamActual] = ConvertParams(finalFormal, argumentStrings, iFirstParamActual, nParamActuals);
                }
                else
                    ConvertArguments(formals, argumentStrings, actuals, nActuals);
            }
            return actuals;
        }

        private List<string> GetArgumentStrings(List<Atoms> argumentAtoms)
        {
            List<string> argumentStrings = new List<string>();
            foreach (Atoms a in argumentAtoms)
                argumentStrings.Add(a.Evaluate());
            return argumentStrings;
        }

        private void ConvertArguments(ParameterInfo[] formals, List<string> argumentStrings, object[] actuals, int n)
        {
            for (int i = 0; i < n; i++)
                actuals[i] = ConvertArgument(formals[i].ParameterType, argumentStrings[i]);
        }

        private object ConvertParams(ParameterInfo paramsFormal, List<string> argumentStrings, int firstParamActual, int nParamActuals)
        {
            if (paramsFormal.ParameterType == typeof(string[]))
            {
                string[] theParams = new string[nParamActuals];
                for (int i = 0; i < nParamActuals; i++)
                    theParams[i] = (string) ConvertArgument(typeof(string), argumentStrings[firstParamActual + i]);
                return theParams;
            }
            else if (paramsFormal.ParameterType == typeof(int[]))
            {
                int[] theParams = new int[nParamActuals];
                for (int i = 0; i < nParamActuals; i++)
                    theParams[i] = (int) ConvertArgument(typeof(int), argumentStrings[firstParamActual + i]);
                return theParams;
            }
            else if (paramsFormal.ParameterType == typeof(double[]))
            {
                double[] theParams = new double[nParamActuals];
                for (int i = 0; i < nParamActuals; i++)
                    theParams[i] = (double) ConvertArgument(typeof(double), argumentStrings[firstParamActual + i]);
                return theParams;
            }
            else if (paramsFormal.ParameterType == typeof(bool[]))
            {
                bool[] theParams = new bool[nParamActuals];
                for (int i = 0; i < nParamActuals; i++)
                    theParams[i] = (bool) ConvertArgument(typeof(bool), argumentStrings[firstParamActual + i]);
                return theParams;
            }
            return null;
        }

        private object ConvertArgument(Type parameterType, string argumentString)
        {
            if (parameterType == typeof(string))
            {
                return argumentString;
            }
            else if (parameterType == typeof(int))
            {
                int value;
                if (Int32.TryParse(argumentString, out value))
                    return value;
                else
                    throw new ActionException(Call, "Function '{0}' expected integer argument but received '{1}'",
                                              Call.FunctionName, argumentString);
            }
            else if (parameterType == typeof(double))
            {
                double value;
                if (Double.TryParse(argumentString, out value))
                    return value;
                else
                    throw new ActionException(Call, "Function '{0}' expected double argument but received '{1}'",
                                              Call.FunctionName, argumentString);
            }
            else if (parameterType == typeof(bool))
            {
                bool value;
                if (Boolean.TryParse(argumentString, out value))
                    return value;
                else
                    throw new ActionException(Call, "Function '{0}' expected bool argument but received '{1}'",
                                              Call.FunctionName, argumentString);
            }
            else
                throw new ActionException(Call, "Function '{0}' has unsupported argument type: '{1}'", parameterType);
        }

    }

}
