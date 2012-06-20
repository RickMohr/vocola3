using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vocola
{
    // All Vocola extension classes must inherit from VocolaExtension.
    // This allows them to interact with Vocola via the inherited
    // static members VocolaApi and VocolaDictation.

    public class VocolaExtension
    {
        static public IVocolaApi       VocolaApi = null;
        static public IVocolaDictation VocolaDictation = null;
    }

    [AttributeUsage(AttributeTargets.Method)]
	public class VocolaFunction : Attribute {}

    [AttributeUsage(AttributeTargets.Method)]
	public class ClearDictationStack : Attribute
    {
        public bool ShouldClear;
        public ClearDictationStack(bool shouldClear)
        {
            ShouldClear = shouldClear;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
	public class CallEagerly : Attribute
    {
        public bool ShouldCallEagerly;
        public CallEagerly(bool shouldCallEagerly)
        {
            ShouldCallEagerly = shouldCallEagerly;
        }
    }

    public interface IVocolaApi
    {
        // Properties
        string CommandFolder { get; }
        string CommandBuiltinsFolder { get; }
        string CommandSamplesFolder { get; }
        string ExtensionFolder { get; }
        VocolaErrorInfo FirstErrorInLastFile { get; }

        // Methods
        void AddTermAlternate(string term, string alternate);
        void DisplayMessage(string message, bool isWarning);
        void EmulateRecognize(string words);
        void ExitVocola();
        object GetAutomationObject(string progId);
        string GetVariable(string name);
        void InsertText(string text);
        void LogMessage(LogLevel level, string message);
        void SendKeys(string keys);
        void SendSystemKeys(string keys);
        void SetVariable(string name, string value);
        void ShowFunctionLibraryDocumentation();
        void ShowLogWindow();
        void ShowOptionsDialog();
        void ShowVocolaMenu();
    }

    public interface IVocolaDictation
    {
        // Properties
        string TextJustDictated { get; }

        // Methods
        void Correct();
        void Enable(bool enable);
        void EnableForWindow(bool enable, string appName, string windowTitleFragment);
        string PopAlternates();
        void Replace(string text);
        string ReplaceInActiveText(string oldText, string newText);
        void ShowDictationShortcutsDialog();
    }

    public class VocolaErrorInfo
    {
        public string Filename;
        public int LineNumber;
        public int ColumnNumber;
        public string Message;
    }

    public enum LogLevel
    {
        Error = 0,
        High,
        Medium,
        Low
    }

    public class VocolaExtensionException : Exception
    {
        public LogLevel LogLevel = LogLevel.Error;

        public VocolaExtensionException(string message, params object[] arguments)
            : base(String.Format(message, arguments)) {}

        public VocolaExtensionException(LogLevel level, string message, params object[] arguments)
            : base(String.Format(message, arguments))
        {
            LogLevel = level;
        }

    }

}
