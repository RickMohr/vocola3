using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Vocola
{

    public class Utilities
    {

        public static string GetPluralSuffix(int n)
        {
            return (n == 1 ? "" : "s");
        }

        public static bool RunningOnVista()
        {
            //Trace.WriteLine(LogLevel.Low, "Windows version: {0}.{1}",
            //    Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor);
            return (
                Environment.OSVersion.Version.Major == 6 &&
                Environment.OSVersion.Version.Minor == 0);
        }

    }
}


