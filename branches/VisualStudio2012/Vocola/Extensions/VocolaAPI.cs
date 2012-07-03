using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms; // Application

namespace Vocola
{

    public class VocolaApi : IVocolaApi
    {
        // Properties

        public string CommandFolder
        {
            get { return Options.CommandFolder; }
        }

        public string CommandBuiltinsFolder
        {
            get { return Vocola.CommandBuiltinsFolder; }
        }

        public string CommandSamplesFolder
        {
            get { return Vocola.CommandSamplesFolder; }
        }

        public string ExtensionFolder
        {
            get { return Options.ExtensionFolder; }
        }

        public VocolaErrorInfo FirstErrorInLastFile
        {
            get { return Trace.FirstErrorInLastFile; }
        }

        // Methods

        public void AddTermAlternate(string term, string alternate)
        {
            Vocola.TheRecognizer.AddTermAlternate(term, alternate);
        }

        public void DisplayMessage(string message, bool isWarning)
        {
            Vocola.TheRecognizer.DisplayMessage(message, isWarning);
        }

        public void EmulateRecognize(string words)
        {
            Vocola.TheRecognizer.EmulateRecognize(words);
        }

        public void ExitVocola()
        {
            Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
            {
                Vocola.TrayIcon.Exit();
            });
        }

        public object GetAutomationObject(string progId)
        {
            return AutomationObjectGetter.GetAutomationObject(progId);
        }

        public string GetVariable(string name)
        {
            return ActionVariable.Get(name);
        }

        public void LogMessage(LogLevel level, string message)
        {
            Trace.WriteLine(level, message);
        }

        public void SendKeys(string keys)
        {
            Keystrokes.SendKeys(keys);
        }

        public void SetVariable(string name, string value)
        {
            ActionVariable.Set(name, value);
        }

        public void SendSystemKeys(string keys)
        {
            Keystrokes.SendSystemKeys(keys);
        }

        public void InsertText(string text)
        {
            Keystrokes.SendText(text);
        }

        public void ShowFunctionLibraryDocumentation()
        {
            Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
            {
                Vocola.TrayIcon.ShowFunctionLibraryDocumentation();
            });
        }

        public void ShowLogWindow()
        {
            Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
            {
                Vocola.TrayIcon.ShowLogWindow();
            });
        }

        public void ShowOptionsDialog()
        {
            Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
            {
                Vocola.TrayIcon.ShowOptionsDialog();
            });
        }

        public void ShowVocolaMenu()
        {
            Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
            {
                Vocola.TrayIcon.ShowVocolaMenu();
            });
        }

    }

    public class VocolaDictation : IVocolaDictation
    {
        // Properties

        public string TextJustDictated
        {
            get { return Dictation.TextJustDictated; }
        }

        // Methods

        public void Correct()
        {
            Dictation.CorrectDictation();
        }

        public void Enable(bool enable)
        {
            Dictation.Enable(enable);
        }

        public void EnableForWindow(bool enable, string appName, string windowTitleFragment)
        {
            Dictation.EnableForWindow(enable, appName, windowTitleFragment);
        }

        public void Replace(string text)
        {
            Dictation.Replace(text);
        }

        public string ReplaceInActiveText(string oldText, string newText)
        {
            return Dictation.ReplaceInActiveText(oldText, newText);
        }

        public string PopAlternates()
        {
            return Dictation.PopAlternates();
        }

        public void ShowDictationShortcutsDialog()
        {
            Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
            {
                Vocola.TrayIcon.ShowDictationShortcutsDialog();
            });
        }

    }

/*
    public class VocolaWsr : IVocolaWsr
    {

        public void ShowFeedback()
        {
            Vocola.TrayIcon.ShowDictationShortcutsDialog();
        }

    }
*/

}
