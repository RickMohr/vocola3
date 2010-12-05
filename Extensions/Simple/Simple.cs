using System;
using System.Collections.Generic;
using System.Text;
using Vocola;

namespace Library
{
    public class Simple : VocolaExtension
    {
        [VocolaFunction]
        static public void LogHelloWorld()
        {
            VocolaApi.LogMessage(LogLevel.Medium, "Hello, world!");
        }
    }
}
