using Microsoft.Win32; // Registry
using System;
using System.Collections.Generic;
using System.IO; // Path.Combine

namespace Vocola
{

    class BuiltinCommandGroup
    {
		public string Filename { get; private set; }
        public string Description { get; private set; }
        public bool Include { get; set; }
        public bool RequiresVocolaDictation { get; private set; }

        public BuiltinCommandGroup(string filename, bool requiresVocolaDictation, string description)
        {
            Filename = filename;
            RequiresVocolaDictation = requiresVocolaDictation;
            Description = description;
            Include = ((int)Key.GetValue(filename, 1)) > 0;
        }

        // Static members to maintain list of groups

        public static List<BuiltinCommandGroup> Groups;
        private static RegistryKey Key;

        public static void Initialize()
        {
            Key = Registry.CurrentUser.CreateSubKey(Path.Combine(Options.RegistryKeyName, "BuiltinCommands"));
            Groups = GetGroups(Options.IsVocolaDictationEnabled);
		}

        public static List<BuiltinCommandGroup> GetGroups(bool isVocolaDictationEnabled)
        {
            var groups = new List<BuiltinCommandGroup>();
            groups.Add(new BuiltinCommandGroup("_ui.vcl", false, "Vocola user interface - control it"));
            groups.Add(new BuiltinCommandGroup("_commandFile.vcl", false, "Command files - open them"));
            groups.Add(new BuiltinCommandGroup("_keys.vcl", false, "Writing commands - insert keystrokes with Vocola syntax"));
            if (isVocolaDictationEnabled)
            {
                groups.Add(new BuiltinCommandGroup("_dictation.vcl", true, "Dictation - modify dictated phrase"));
                groups.Add(new BuiltinCommandGroup("Vocola.vcl", true, "Vocola correction panel - choose alternatives"));
            }
            return groups;
        }

        public static bool IsExcluded(string filename)
        {
            foreach (BuiltinCommandGroup group in Groups)
                if (filename == group.Filename)
                    return !group.Include;
            return true;
        }

        public static void UpdateAndSave()
        {
            Initialize();
            foreach (BuiltinCommandGroup group in Groups)
                Key.SetValue(group.Filename, group.Include ? 1 : 0);
        }

    }
}