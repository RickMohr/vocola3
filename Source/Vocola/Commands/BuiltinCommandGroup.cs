using Microsoft.Win32; // Registry
using System;
using System.Collections.Generic;
using System.IO; // Path.Combine

namespace Vocola
{

    class BuiltinCommandGroup
    {

		public string Filename { get; private set; }
        private string description;
        private bool   enable;

        public BuiltinCommandGroup(string filename, string description)
        {
            this.Filename    = filename;
            this.description = description;
            enable = ((int)Key.GetValue("filename", 1)) > 0;
        }

        public string Description
        {
            get { return description; }
        }

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        // Static members to maintain list of groups

        public static List<BuiltinCommandGroup> Groups;
        private static RegistryKey Key;

        public static void Initialize()
        {
            Key = Registry.CurrentUser.CreateSubKey(Path.Combine(Vocola.RegistryKeyName, "BuiltinCommands"));
            Groups = new List<BuiltinCommandGroup>();
            Groups.Add(new BuiltinCommandGroup("_ui.vcl"         , "Vocola user interface - control it"));
            Groups.Add(new BuiltinCommandGroup("_commandFile.vcl", "Command files - open them"));
            Groups.Add(new BuiltinCommandGroup("_keys.vcl"       , "Writing commands - insert keystrokes with Vocola syntax"));
			if (Vocola.TheRecognizer.SupportsDictation)
			{
				Groups.Add(new BuiltinCommandGroup("_dictation.vcl", "Dictation - modify dictated phrase"));
				Groups.Add(new BuiltinCommandGroup("Vocola.vcl", "Vocola correction panel - choose alternatives"));
			}
		}

        public static bool IsDisabled(string filename)
        {
            foreach (BuiltinCommandGroup group in Groups)
                if (filename == group.Filename)
                    return !group.Enable;
			// Unexpected builtin command file name
			if (filename == "_dictation.vcl")
				return true;
			throw new InternalException("Unexpected builtin command file name");
        }

        public static void SaveToRegistry()
        {
            foreach (BuiltinCommandGroup group in Groups)
                Key.SetValue(group.Filename, group.Enable ? 1 : 0);
        }

    }
}