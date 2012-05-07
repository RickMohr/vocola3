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

	// NatLink to Vocola calls. 
	// Each arrive via Python to C to C# hops, and execute in the NatSpeak process.
	// Each make inter-process calls (IPC) to the Vocola process.

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

	// Handling Vocola to NatLink callbacks (notably EmulateRecognize()) is tricky. 
	// NatSpeak has only one thread, and we must use it to execute callbacks. 
	// But Vocola initiates the callbacks via IPC, so they arrive on a different thread. 
	// Here we set up synchronization so a callback can make a thunk and signal the main thread
	// to execute it (synchronously).

	public class NatLinkCallbackHandler : MarshalByRefObject
	{
		private EventWaitHandle CallbackRequestWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
		private EventWaitHandle CallbackDoneWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
		private Action CallbackThunk;

		// Executed on NatSpeak thread. Wait for a callback request, execute it, 
		// and signal the requestor when done.

		public void HandleCallbacks()
		{
			while (true)
			{
				CallbackRequestWaitHandle.WaitOne();
				if (CallbackThunk == null)
					return; // actions done
				CallbackThunk.Invoke();
				CallbackDoneWaitHandle.Set();
			}
		}

		// A callback arrives on a separate thread. Create a thunk, signal main thread to handle it,
		// and wait for completion.

		public void EmulateRecognize(string words)
		{
			CallbackThunk = () => NatLinkEmulateRecognize(words);
			CallbackRequestWaitHandle.Set();
			CallbackDoneWaitHandle.WaitOne();
		}

		public void ActionsDone()
		{
			CallbackThunk = null;
			CallbackRequestWaitHandle.Set();
		}

		// The callbacks reach NatLink via these C calls, which then call Python

		[DllImport("NatLinkConnectorC.dll", CharSet=CharSet.Unicode)]
		private static extern void NatLinkEmulateRecognize(string words);

	}

}
