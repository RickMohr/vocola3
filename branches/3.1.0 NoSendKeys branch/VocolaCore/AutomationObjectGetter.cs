using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;

namespace Vocola
{

    // Helper class for connecting to a COM server, such as an application's automation interface.
    // 
    // COM has a lovely "feature" whereby a client and server with different integrity levels aren't allowed to
    // communicate. So because Vocola runs with elevated integrity it isn't allowed to connect with most COM servers.
    // 
    // The workaround (suggested by Rob Chambers) is to create a separate process to connect to the COM server and
    // marshall everything through that.

    class AutomationObjectGetter
    {
        static private object TheLock = new Object();
        static private Process ServerProcess;
        static private IAutomationObjectGetter TheGetter = null;

        static public object GetAutomationObject(string progId)
        {
            object automationObject = null;
            try
            {
                lock (TheLock)
                {
                    if (TheGetter == null)
                    {
                        // Start server process
                        ServerProcess = new Process();
                        ServerProcess.StartInfo.FileName = "VocolaAutomationObjectGetter.exe";
                        ServerProcess.StartInfo.Arguments = Process.GetCurrentProcess().Id.ToString();
                        ServerProcess.StartInfo.CreateNoWindow = true;
                        ServerProcess.Start();
                        // Connect to server
                        TcpChannel channel = new TcpChannel();
                        ChannelServices.RegisterChannel(channel, true);
                        string url = String.Format("tcp://127.0.0.1:{0}/AutomationObjectGetterServer", Vocola.AutomationObjectGetterPort);
                        TheGetter = (IAutomationObjectGetter) Activator.GetObject(typeof(IAutomationObjectGetter), url);
                    }
                }
                automationObject = TheGetter.GetAutomationObject(progId);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.Error, "Exception getting automation object '{0}':\n{1}", progId, ex.Message);
            }
            return automationObject;
        }

        static public void Cleanup()
        {
            try
            {
                if (ServerProcess != null)
                    ServerProcess.Kill();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.Error, "Exception stopping VocolaAutomationObjectGetter process:\n{0}", ex.Message);
            }
        }            

    }

}