using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Vocola
{

    public class Extensions
    {
        static public Dictionary<string, bool> NamespaceNames;
        static public Dictionary<string, bool> ClassNames;

        // Keep a dictionary of available extension methods.
        // Key: fully qualified method name (NamespaceName.ClassName.FunctionName)
        // Value: list of methods matching that name (there may be several with different argument counts)
        static private Dictionary<string, List<MethodInfo>> Methods;

        public static void Load()
        {
            NamespaceNames = new Dictionary<string, bool>();
            ClassNames = new Dictionary<string, bool>();
            Methods = new Dictionary<string, List<MethodInfo>>();
            LoadFromFolder(Vocola.FunctionLibraryFolder);
            LoadFromFolder(Options.ExtensionFolder);
        }

        private static void LoadFromFolder(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                Trace.WriteLine(LogLevel.Error, "Extension folder not found: '{0}'", folderName);
                return;
            }
            string[] dllNames = Directory.GetFiles(folderName, "*.dll", SearchOption.AllDirectories);
            Array.Sort(dllNames);
            foreach (string pathname in dllNames)
            {
                string filename = Path.GetFileName(pathname);
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFile(pathname);
                    Trace.WriteLine(LogLevel.Medium, "Loading extension file '{0}':", pathname);
                    foreach (Module module in assembly.GetLoadedModules())
                    {
                        Type[] types = module.GetTypes();
                        Array.Sort(types, new TypeNameComparer());
                        foreach (Type type in types)
                            if (type.BaseType == typeof(VocolaExtension))
                            {
                                string classname = type.Name;
                                int nMethods = 0;
                                string NamespaceAndClass = type.Namespace + "." + classname;
                                NamespaceNames[type.Namespace] = true;
                                ClassNames[NamespaceAndClass] = true;
                                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                                    if (method.GetCustomAttributes(typeof(VocolaFunction), false).Length > 0)
                                    {
                                        string methodFullName = NamespaceAndClass + "." + method.Name;
                                        if (!Methods.ContainsKey(methodFullName))
                                            Methods[methodFullName] = new List<MethodInfo>();
                                        Methods[methodFullName].Add(method);
                                        nMethods++;
                                    }
                                Trace.WriteLine(LogLevel.Medium, "  Class '{0}' has {1} function{2}",
                                    NamespaceAndClass, nMethods, Utilities.GetPluralSuffix(nMethods));
                            }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ReflectionTypeLoadException)
                        ex = (ex as ReflectionTypeLoadException).LoaderExceptions[0];
                    Trace.WriteLine(LogLevel.Error, "Error loading extension file '{0}':", pathname);
                    Trace.WriteLine(LogLevel.Error, ex.Message);
                    continue;
                }
            }
        }

        /*
        static private string GetVocolaClassName(Type type)
        {
            VocolaClassName[] attributes = (VocolaClassName[])type.GetCustomAttributes(typeof(VocolaClassName), false);
            if (attributes.Length == 0)
                return type.Name;
            else
                return attributes[0].Name;
        }
        */

        public class TypeNameComparer : IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                return String.Compare(((Type)x).Name, ((Type)y).Name);
            }
        }


        public static MethodInfo GetMethod(string methodFullName, int nFormals)
        {
            if (Methods.ContainsKey(methodFullName))
                foreach (MethodInfo method in Methods[methodFullName])
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    int nParameters = parameters.Length;
                    if (nFormals == nParameters
                        || (nParameters > 0 &&  // does final parameter have "params" keyword
                            parameters[nParameters - 1].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0))
                        {
                            return method;
                        }
                }
            return null;
        }

        public static bool ClassOrNamespaceExists(string name)
        {
            return ClassNames.ContainsKey(name) || NamespaceNames.ContainsKey(name);
        }

        public static bool MethodExists(string methodFullName)
        {
            return Methods.ContainsKey(methodFullName);
        }

    }
}
