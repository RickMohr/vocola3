using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using System.IO;
using System.Runtime.InteropServices;

namespace Vocola
{

	public interface INatLinkToVocola
	{
		void SetVocolaToNatlinkCallbackObject(NatLinkCallbacks natLinkCallbacks);
		void RunActions(string commandId, string variableWords);
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
			ToVocola.SetVocolaToNatlinkCallbackObject(new NatLinkCallbacks());
		}

		static public void RunActions(string commandId, string variableWords)
		{
			ToVocola.RunActions(commandId, variableWords);
		}

		static public void LogMessage(int level, string message)
		{
			ToVocola.LogMessage(level, message);
		}

	}

	public class NatLinkCallbacks : MarshalByRefObject
	{

		public void EmulateRecognize(string words)
		{
			using (var sw = new StreamWriter(@"C:\Temp\rick.txt"))
			{
				sw.WriteLine("HearCommand: " + words);
			}
			NatLinkEmulateRecognize(words);
		}

		[DllImport("NatLinkConnectorC.dll", CharSet=CharSet.Unicode)]
		private static extern void NatLinkEmulateRecognize(string words);

	}

}
