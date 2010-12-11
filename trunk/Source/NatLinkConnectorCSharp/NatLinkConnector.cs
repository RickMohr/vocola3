using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;

namespace Vocola
{

    public interface INatLinkToVocola
    {
        void RunActions(string commandId, string variableWords);
    }

    public class NatLinkToVocolaClient
    {
        static private INatLinkToVocola ToVocola;

        static public void InitializeConnection()
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            ToVocola = (INatLinkToVocola) Activator.GetObject(typeof(INatLinkToVocola), "tcp://127.0.0.1:9753/NatLinkToVocola");
        }

        static public void RunActions(string commandId, string variableWords)
        {
            ToVocola.RunActions(commandId, variableWords);
        }
        
    }
}
