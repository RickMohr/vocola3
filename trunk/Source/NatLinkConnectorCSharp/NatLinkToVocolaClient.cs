using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace Vocola
{

	public interface INatLinkToVocola
	{
		void RunActions(string commandId, string variableWords, NatLinkCallbackHandler callbackHandler);
		void LogMessage(int level, string message);
	}

	public class NatLinkToVocolaClient
	{
		static private INatLinkToVocola ToVocola;

		static public void InitializeConnection()
		{
			var prov = new BinaryServerFormatterSinkProvider() { TypeFilterLevel = TypeFilterLevel.Full };
			var channel = new IpcServerChannel("NatLinkToVocolaClientChannel", "NatLinkToVocolaClientChannel", prov);
			ChannelServices.RegisterChannel(channel, false);
			string url = "ipc://NatLinkToVocolaServerChannel/NatLinkToVocolaListener";
			ToVocola = (INatLinkToVocola)Activator.GetObject(typeof(INatLinkToVocola), url);
		}

		static public void RunActions(string commandId, string variableWords)
		{
			var callbackHandler = new NatLinkCallbackHandler();
			Action runActions = () => ToVocola.RunActions(commandId, variableWords, callbackHandler);
			runActions.BeginInvoke(null, null);
			callbackHandler.HandleCallbacks();
		}

		static public void LogMessage(int level, string message)
		{
			ToVocola.LogMessage(level, message);
		}

	}

	public class NatLinkCallbackHandler : MarshalByRefObject
	{
		private EventWaitHandle CallbackRequestWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
		private EventWaitHandle CallbackDoneWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
		private Action CallbackThunk;

		public void HandleCallbacks()
		{
			while (true)
			{
				CallbackRequestWaitHandle.WaitOne();
				if (CallbackThunk == null)
					return;
				CallbackThunk.Invoke();
				CallbackDoneWaitHandle.Set();
			}
		}

		public void EmulateRecognize(string words)
		{
			//using (var sw = new StreamWriter(@"C:\Temp\rick.txt"))
			//{
			//    sw.WriteLine("HearCommand: " + words);
			//}
			CallbackThunk = () => NatLinkEmulateRecognize(words);
			CallbackRequestWaitHandle.Set();
			CallbackDoneWaitHandle.WaitOne();
		}

		public void ActionsDone()
		{
			CallbackThunk = null;
			CallbackRequestWaitHandle.Set();
		}

		[DllImport("NatLinkConnectorC.dll", CharSet=CharSet.Unicode)]
		private static extern void NatLinkEmulateRecognize(string words);

	}

}
