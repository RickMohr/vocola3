using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Vocola
{

    public class NatLinkListener
    {

        static public void Start(int port)
        {
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NatLinkToVocolaServer), "NatLinkToVocola", WellKnownObjectMode.Singleton);
        }

    }

    public class NatLinkToVocolaServer : MarshalByRefObject, INatLinkToVocola
    {

        public void RunActions(string commandId, string variableWords)
        {
            try
            {
                Command command = CommandSet.GetCommand(commandId);
                if (command == null)
                    throw new InternalException("Could not find command '{0}'", commandId);
                Trace.WriteLine(LogLevel.Medium, "  Executing {0}:  {1}", commandId, command);

                ActionsQueue actionsQueue = new ActionsQueue();
                List<ArrayList> variableTermActions = RecognizerNatLink.GetVariableTermActions(command, variableWords);
                actionsQueue.AddActions(command.Actions, variableTermActions);
                ActionRunner.Launch(actionsQueue);
            }
            catch (Exception ex)
            {
                Trace.LogExecutionException(ex);
            }
        }

		public void LogMessage(int level, string message)
		{
			Trace.WriteLine((LogLevel)level, message);
		}

    }

}
