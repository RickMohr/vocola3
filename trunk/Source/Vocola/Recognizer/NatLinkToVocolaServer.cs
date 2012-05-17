using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;

namespace Vocola
{

    public class NatLinkListener
    {

        static public void Start()
        {
			var prov = new BinaryServerFormatterSinkProvider() { TypeFilterLevel = TypeFilterLevel.Full };
			var channel = new IpcServerChannel("NatLinkToVocolaServerChannel", "NatLinkToVocolaServerChannel", prov);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(NatLinkToVocolaServer), "NatLinkToVocolaListener", WellKnownObjectMode.Singleton);
        }

    }

    public class NatLinkToVocolaServer : MarshalByRefObject, INatLinkToVocola
    {
		private static Stack<NatLinkCallbackHandler> CallbackHandlers = new Stack<NatLinkCallbackHandler>();
		private DateTime LastGrammarQueryTime = new DateTime(0);

		public static NatLinkCallbackHandler CurrentNatLinkCallbackHandler
		{
			get { return CallbackHandlers.Peek(); }
		}

		public int HaveAnyGrammarFilesChanged()
		{
			int result = 0; // nothing changed
			if (((RecognizerNatLink)Vocola.TheRecognizer).LastGrammarCreationTime > LastGrammarQueryTime)
			{
				result = 2; // a previously-nonexistent grammar file was created
			}
			else if (((RecognizerNatLink)Vocola.TheRecognizer).LastGrammarUpdateTime > LastGrammarQueryTime)
			{
				result = 1; // an existing grammar file was changed
			}
			LastGrammarQueryTime = DateTime.Now;
			return result;
		}

		public void RunActions(string commandId, string variableWords, NatLinkCallbackHandler callbackHandler)
        {
			CallbackHandlers.Push(callbackHandler);
			try
			{
				Command command = CommandSet.GetCommand(commandId);
				if (command == null)
					throw new InternalException("Could not find command '{0}'", commandId);
				Trace.WriteLine(LogLevel.Medium, "  Executing {0}:  {1}", commandId, command);

				ActionsQueue actionsQueue = new ActionsQueue();
				List<ArrayList> variableTermActions = RecognizerNatLink.GetVariableTermActions(command, variableWords);
				actionsQueue.AddActions(command.Actions, variableTermActions);
				ActionRunner.RunActions(actionsQueue);
			}
			catch (Exception ex)
			{
				Trace.LogExecutionException(ex);
			}
			finally
			{
				CallbackHandlers.Pop();
				callbackHandler.ActionsDone();
			}
        }

		public void LogMessage(int level, string message)
		{
			Trace.WriteLine((LogLevel)level, message);
		}

    }

}
