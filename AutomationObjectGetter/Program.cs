using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;
using Microsoft.Win32; // RegistryKey

namespace Vocola
{

    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Listen for requests
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Vocola");
                int port = (int)key.GetValue("AutomationObjectGetterPort", 1649);
                TcpChannel channel = new TcpChannel(port);
                ChannelServices.RegisterChannel(channel, true);
                RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(AutomationObjectGetterServer),
                    "AutomationObjectGetterServer",
                    WellKnownObjectMode.Singleton);

                // Exit if Vocola process disappears
                int vocolaProcessId = Int32.Parse(args[0]);
                Process.GetProcessById(vocolaProcessId).WaitForExit();
            }
            catch {} // Exit quietly on failure
        }

    }

    public interface IAutomationObjectGetter
    {
        object GetAutomationObject(string progId);
    }

    public class AutomationObjectGetterServer : MarshalByRefObject, IAutomationObjectGetter
    {

        public object GetAutomationObject(string progId)
        {		
            return System.Runtime.InteropServices.Marshal.GetActiveObject(progId);
        }

        // The connection from a client to this server object times out after a while,
        // and the client gets exceptions when it tries to invoke methods.
        // I tried to address this issue with many variations of the following method, but nothing worked.
        // So clients need to handle the exception and re-connect.

        public override object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            if (lease.CurrentState == LeaseState.Initial)
            {
                lease.InitialLeaseTime   = TimeSpan.Zero;
                //lease.SponsorshipTimeout = TimeSpan.FromDays(10000);
                //lease.RenewOnCallTime    = TimeSpan.FromDays(10000);
            }
            return lease;
        }

    }

}
