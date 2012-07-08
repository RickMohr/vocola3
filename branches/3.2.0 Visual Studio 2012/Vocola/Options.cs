using Microsoft.Win32; // Registry
using System;
using System.Collections.Generic;
using System.IO;

namespace Vocola
{

    public class Options
    {
        static public readonly string RegistryKeyName = @"Software\Vocola";
        
        static private RegistryKey Key = Registry.CurrentUser.CreateSubKey(RegistryKeyName);

        static public RecognizerType TheRecognizerType;
        static public string CommandFolder;
        static public string ExtensionFolder;
        static public BaseUsingSetOption BaseUsingSetCode;
        static public string CustomBaseUsingSet;
        static public int AutomationObjectGetterPort;

        static public bool IsVocolaDictationEnabled { get { return (TheRecognizerType == RecognizerType.Wsr && OptionsSapi.DisableWsrDictationScratchpad); } }

        static public void Load()
        {
            Trace.LevelThreshold = (LogLevel)(int)Key.GetValue("LogLevel", (int)LogLevel.Error);//Low
            Trace.ShowTimings = ((int)Key.GetValue("ShowTimingInLogMessages", 0)) > 0;
            Trace.ShouldLogToFile = ((int)Key.GetValue("ShouldLogToFile", 0)) > 0;//1
            CommandFolder = (string)Key.GetValue("CommandFolderPath", null);
            ExtensionFolder = (string)Key.GetValue("ExtensionFolderPath", null);
            BaseUsingSetCode = (BaseUsingSetOption)(int)Key.GetValue("BaseUsingSetCode", (int)BaseUsingSetOption.Vocola3);
            CustomBaseUsingSet = ((string)Key.GetValue("CustomBaseUsingSet", "")).Replace(" ", "");
            TheRecognizerType = (RecognizerType)(int)Key.GetValue("RecognizerType", 0);

            // Not settable via GUI, but present in case it needs to be tweaked in the field
            AutomationObjectGetterPort = (int)Key.GetValue("AutomationObjectGetterPort", 1649);

            OptionsSapi.Load();
            OptionsNatLink.Load();
        }

        static public void Save()
        {
            Key.SetValue("CommandFolderPath", CommandFolder);
            Key.SetValue("ExtensionFolderPath", ExtensionFolder);
            Key.SetValue("BaseUsingSetCode", (int)BaseUsingSetCode);
            Key.SetValue("CustomBaseUsingSet", CustomBaseUsingSet);
            Key.SetValue("RecognizerType", (int)TheRecognizerType);

            OptionsSapi.Save();
            OptionsNatLink.Save();
        }

        static public void SaveLogLevel(LogLevel level)
        {
            Key.SetValue("LogLevel", (int)level);
        }

    }

    public class OptionsSapi
    {
        static public readonly RegistryKey Key = Registry.CurrentUser.CreateSubKey(Path.Combine(Options.RegistryKeyName, "RecognizerSapi"));
        static private RegistryKey MsKey = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Speech\Preferences");

        static public bool DisableWsrDictationScratchpad;
        static public bool RequireControlNamePrefix;
        static public bool CommandSequencesEnabled;
        static public int MaxSequencedCommands;

        static public void Load()
        {
            DisableWsrDictationScratchpad = ((int)MsKey.GetValue("EnableDictationScratchpad", 0)) == 2;
            RequireControlNamePrefix = ((int)Key.GetValue("RequireControlNamePrefix", 0)) > 0;
            CommandSequencesEnabled = ((int)Key.GetValue("UseCommandSequences", 0)) > 0;
            MaxSequencedCommands = (int)Key.GetValue("MaxSequencedCommands", 6);
        }

        static public void Save()
        {
            MsKey.SetValue("EnableDictationScratchpad", DisableWsrDictationScratchpad ? 2 : 1);
            Key.SetValue("UseCommandSequences", CommandSequencesEnabled ? 1 : 0);
            Key.SetValue("MaxSequencedCommands", MaxSequencedCommands);
            Key.SetValue("RequireControlNamePrefix", RequireControlNamePrefix ? 1 : 0);
        }

    }

    public class OptionsNatLink
    {
        static public readonly RegistryKey Key = Registry.CurrentUser.CreateSubKey(Path.Combine(Options.RegistryKeyName, "RecognizerNatLink"));

        static public string NatLinkInstallFolder;
        static public bool CommandSequencesEnabled;
        static public int MaxSequencedCommands;

        static public void Load()
        {
            NatLinkInstallFolder = (string)Key.GetValue("NatLinkInstallFolder", @"C:\NatLink");
            CommandSequencesEnabled = ((int)Key.GetValue("UseCommandSequences", 0)) > 0;
            MaxSequencedCommands = (int)Key.GetValue("MaxSequencedCommands", 1);
        }

        static public void Save()
        {
            Key.SetValue("NatLinkInstallFolder", NatLinkInstallFolder);
            Key.SetValue("UseCommandSequences", CommandSequencesEnabled ? 1 : 0);
            Key.SetValue("MaxSequencedCommands", MaxSequencedCommands);
        }

    }

}
