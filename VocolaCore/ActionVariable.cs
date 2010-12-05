using System;
using System.Collections.Generic;

namespace Vocola
{
    // This is just for the use of extensions. It's internal rather than part of the
    // library Variable class so that a variable can be set by an extension in one DLL
    // and read by an extension in another DLL.

    public class ActionVariable
    {
        static private Dictionary<string, string> Variables = new Dictionary<string, string>();

        static public string Get(string name)
        {
            if (Variables.ContainsKey(name))
                return Variables[name];
            else
                throw new ActionException(null, "Variable does not exist: '{0}'", name);
        }

        static public void Set(string name, string value)
        {
            Variables[name] = value;
        }

    }

}
