using SpeechLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

namespace Vocola
{

    public class Dictation
    {

        public class DictatedText
        {
            public string Text;
            public int TrailingSpaceCount;
            public SapiAlternates Alternates;

            public DictatedText(string text, SapiAlternates alternates, int trailingSpaceCount)
            {
                Text = text;
                Alternates = alternates;
                TrailingSpaceCount = trailingSpaceCount;
            }
        }

        static private SpSharedRecoContext RecognizerContext;
        static private RecognizerSapi RecognizerSapi;
        static private ISpeechRecoGrammar DictationGrammar;
        static private bool Enabled = true;
        static private Stack<DictatedText> DictationStack = new Stack<DictatedText>();

        static public uint MaxAlternates; 

        static public void Initialize(SpSharedRecoContext recognizerContext, RecognizerSapi recognizerSapi, int dictationGrammarId)
        {
            RecognizerContext = recognizerContext;
            RecognizerSapi = recognizerSapi;
            DictationGrammar = recognizerContext.CreateGrammar(dictationGrammarId);
            DictationGrammar.DictationLoad("", SpeechLoadOption.SLOStatic);
            DictationGrammar.DictationSetState(SpeechRuleState.SGDSActive);
            LaunchActiveTextUpdateThread();

            // Disable dictation in Vocola log window
            EnableForWindow(false, "Vocola", "Log");
        }

        static public void SetWeight(float weight)
        {
            ((ISpRecoGrammar2)DictationGrammar).SetDictationWeight(weight);
        }

        static public void Enable(bool enable)
        {
            Enabled = enable;
        }

        static public string TextJustDictated
        {
            get
            {
                lock (DictationStack)
                {
                    if (DictationStack.Count == 0)
                        return null;
                    else
                        return DictationStack.Peek().Text;
                }
            }
        }

        static public void HandleDictatedPhrase(string text, ISpeechRecoResult result)
        {
            if (!Enabled)
                Trace.WriteLine(LogLevel.High, "No keystrokes sent because dictation is disabled");
            else if (IsDisabledForCurrentWindow())
                Trace.WriteLine(LogLevel.High, "No keystrokes sent because dictation is disabled for current window");
            else
                lock (DictationStack)
                {
                    bool capitalizeAlternates;
                    if (TextJustDictated == null)
                        capitalizeAlternates = Char.IsUpper(text, 0);
                    else
                    {
                        string lastCharDictated = TextJustDictated.Substring(TextJustDictated.Length - 1);
                        capitalizeAlternates = (lastCharDictated == "." || lastCharDictated == "\n");

                        if (AllowsLeadingSpace(text, result))
                        {
                            int spaceCount = DictationStack.Peek().TrailingSpaceCount;
                            if (spaceCount == 1)
                                text = " " + text;
                            else if (spaceCount == 2)
                                text = "  " + text;
                        }
                    }
                    Keystrokes.SendTextUsingSeparateThread(text);
                    SapiAlternates alternates = 
                        (result == null) ? new SapiAlternates(text)
                                         : new SapiAlternates(result, capitalizeAlternates);
                    DictationStack.Push(new DictatedText(text, alternates, GetTrailingSpaceCount(text, result)));
                    ActiveTextUpdaterWaitHandle.Set();
                }
        }

        static public string CharsNoSpaceBefore = @".,!?-_/\;:)]}>";
        static public string CharsNoSpaceAfter  = @"-_/\([{<";

        static private bool AllowsLeadingSpace(string text, ISpeechRecoResult result)
        {
            if (result == null)
                return !CharsNoSpaceBefore.Contains(text.Substring(0,1));                
            else
            {
                byte attributes = (byte)result.PhraseInfo.GetDisplayAttributes(0, -1, true);
                byte consumeLeadingSpaces = (byte)SpeechDisplayAttributes.SDA_Consume_Leading_Spaces;
                return (attributes & consumeLeadingSpaces) == 0;
            }
        }

        static private int GetTrailingSpaceCount(string text, ISpeechRecoResult result)
        {
            if (result == null)
            {
                string lastChar = text.Substring(text.Length - 1);
                return CharsNoSpaceAfter.Contains(lastChar) ? 0 : 1;
            }
            else
            {
                byte attributes = (byte)result.PhraseInfo.GetDisplayAttributes(0, -1, true);
                byte one = (byte)SpeechDisplayAttributes.SDA_One_Trailing_Space;
                byte two = (byte)SpeechDisplayAttributes.SDA_Two_Trailing_Spaces;
                return (attributes & one) == one ? 1 : (attributes & two) == two ? 2 : 0;
            }
        }

        static public void CorrectDictation()
        {
            if (TextJustDictated != null)
                if (DictationStack.Count == 0)
                    Trace.WriteLine(LogLevel.High, "Dictation.Correct() called but no alternates are available.");
                else
                    Vocola.TrayIcon.BeginInvoke((MethodInvoker) delegate()
                    {
                        new Correction(DictationStack.Peek().Alternates);
                    });
        }

        static public void Replace(string text)
        {
            lock (DictationStack)
            {
                if (text == "")
                    DictationStack.Pop();
                else
                    DictationStack.Peek().Text = text;
                ActiveTextUpdaterWaitHandle.Set();
            }
        }

        static public string ReplaceInActiveText(string oldText, string newText)
        {
            lock (DictationStack)
            {
                // Find where old text matches active text, starting from the end
                string activeText = GetActiveText();
                int matchStart = activeText.LastIndexOf(oldText);
                if (matchStart == -1)
                    throw new ActionException(null, "Active dictation does not contain '{0}'", oldText);
                int matchEnd = matchStart + oldText.Length - 1;

                // Put dictation stack into an array so we can iterate forwards
                int nPieces = DictationStack.Count;
                DictatedText[] pieces = new DictatedText[nPieces];
                int i = nPieces;
                foreach (DictatedText dt in DictationStack)
                    pieces[--i] = dt;

                int pieceStart = 0;
                foreach (DictatedText dt in pieces)
                {
                    int pieceEnd = pieceStart + dt.Text.Length - 1;
                    if (pieceStart <= matchEnd && pieceEnd >= matchStart)
                    {
                        // This piece contains part of the matched text. Remove that part.
                        int removeStart = Math.Max(pieceStart, matchStart) - pieceStart;
                        int removeEnd   = Math.Min(pieceEnd  , matchEnd  ) - pieceStart;
                        dt.Text = dt.Text.Remove(removeStart, removeEnd - removeStart + 1);

                        // If this piece contains the beginning of the matched text, insert the new text
                        if (matchStart >= pieceStart)
                            dt.Text = dt.Text.Insert(removeStart, newText);
                    }
                    pieceStart = pieceEnd + 1;
                }
                ActiveTextUpdaterWaitHandle.Set();
                string tail = activeText.Substring(matchEnd + 1);
                return tail;
            }
        }

        static public void Clear()
        {
            //Trace.WriteLine(LogLevel.Low, "Clearing dictation stack");
            lock (DictationStack)
                if (DictationStack.Count > 0)
                {
                    DictationStack.Clear();
                    ActiveTextUpdaterWaitHandle.Set();
                }
        }

        static public string PopAlternates()
        {
            if (DictationStack.Count > 0)
                return DictationStack.Peek().Alternates.Pop();
            else
                return null;
        }

        // ---------------------------------------------------------------------
        // Disable dictation for particular windows

        private class WindowIdentifier
        {
            public string AppName;
            public string WindowTitleFragment;

            public WindowIdentifier(string appName, string windowTitleFragment)
            {
                AppName = appName.ToLower();
                WindowTitleFragment = windowTitleFragment;
            }
        }

        static private List<WindowIdentifier> DisabledWindows = new List<WindowIdentifier>();

        static public void EnableForWindow(bool enable, string appName, string windowTitleFragment)
        {
            string id = String.Format("application '{0}'", appName);
            if (windowTitleFragment != "")
                id += String.Format(" in windows containing '{0}'", windowTitleFragment);
            for (int i = 0; i < DisabledWindows.Count; i++)
                if (appName == DisabledWindows[i].AppName && windowTitleFragment == DisabledWindows[i].WindowTitleFragment)
                {
                    // Found in collection
                    if (enable)
                    {
                        DisabledWindows.RemoveAt(i);
                        Trace.WriteLine(LogLevel.High, "Enabling dictation for {0}", id);
                    }
                    else
                        Trace.WriteLine(LogLevel.High, "Dictation already disabled for {0}", id);
                    return;
                }
            // Not in collection
            if (enable)
                Trace.WriteLine(LogLevel.High, "Dictation already enabled for {0}", id);
            else
            {
                DisabledWindows.Add(new WindowIdentifier(appName, windowTitleFragment));
                Trace.WriteLine(LogLevel.High, "Disabling dictation for {0}", id);
            }
        }

        static private bool IsDisabledForCurrentWindow()
        {
            string appName = Win.GetForegroundAppName();
            string windowTitle = Win.GetForegroundWindowTitle();
            foreach (WindowIdentifier wi in DisabledWindows)
                if (appName == wi.AppName && windowTitle.Contains(wi.WindowTitleFragment))
                    return true;
            return false;
        }

        // ---------------------------------------------------------------------
        // Update command grammar with active text from dictation stack

        // Since we must pause the recognizer to perform updates and since we'll be called
        // sometimes from speech event handlers (e.g. when handling Vocola dictation),
        // create a separate thread to handle the updates.

        static private EventWaitHandle ActiveTextUpdaterWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        static private void LaunchActiveTextUpdateThread()
        {
            Thread thread = new Thread(HandleActiveTextUpdates);
            thread.Name = "Active Text Update Thread";
            thread.Start();
        }

        static private void HandleActiveTextUpdates()
        {
            while (true)
            {
                ActiveTextUpdaterWaitHandle.WaitOne();
                try
                {
                    RecognizerContext.Pause();
                    lock (DictationStack)
                        UpdateActiveText();
                }
                catch (Exception ex)
                {
                    Trace.LogUnexpectedException(ex);
                }
                finally
                {
                    RecognizerContext.Resume();
                }
            }
        }

        static public void UpdateActiveText()
        {
            string text = "";
            if (DictationStack.Count > 0)
            {
                text = GetActiveText();
                Trace.WriteLine(LogLevel.Low, "Enabling updated Vocola dictation grammar");
            }
            else
            {
                if (Utilities.RunningOnVista())
                    text = "zq"; // SetWordSequenceData() throws an exception if text is empty
                Trace.WriteLine(LogLevel.Low, "Disabling Vocola dictation grammar");
            }

            SpTextSelectionInformation textSelectInfo = new SpTextSelectionInformation();
            textSelectInfo.ActiveOffset = 0;               // Start of text
            textSelectInfo.ActiveLength = text.Length;     // Length of text
            textSelectInfo.SelectionOffset = 0;            // Start of selected text
            textSelectInfo.SelectionOffset = text.Length;  // Length of selected text

            if (RecognizerSapi.CommandGrammar != null)
                try
                {
                    RecognizerSapi.CommandGrammar.SetWordSequenceData(text, text.Length, textSelectInfo);
                }
                catch (Exception)
                {
                    Trace.WriteLine(LogLevel.Low, "Failed to update Vocola dictation grammar");
                }
        }

        static private string GetActiveText()
        {
            string text = "";
            foreach (DictatedText dt in DictationStack)
                text = dt.Text + text;
            return text;
        }

    }

    // ---------------------------------------------------------------------

    public class SapiAlternates
    {
        private ISpeechRecoResult RecoResult;
        private bool CapitalizeAlternates;
        private int NDisplayAlternatesOfRecognizedPhrase;
        private int PopIndex; 

        // 0: recognized phrase
        // 1: recognized phrase with initial case flipped and no "replacements"
        // 2... other "display alternates" of recognized phrase
        // ... primary "display alternate" of each acoustic alternate

        public SapiAlternates(ISpeechRecoResult recoResult, bool capitalizeAlternates)
        {
            RecoResult = recoResult;
            CapitalizeAlternates = capitalizeAlternates;
        }

        public SapiAlternates(string text)
        {
            alternates = new List<string>();
            alternates.Add(text);
            RecoResult = null;
            CapitalizeAlternates = false;
            NDisplayAlternatesOfRecognizedPhrase = 0;
        }

        // Alternates may be expensive to get so do it lazily
        private List<string> alternates = null;
        private List<string> Alternates
        {
            get
            {
                try
                {
                    if (alternates == null)
                    {
                        alternates = new List<string>();
                        string recognizedPhrase = RecoResult.PhraseInfo.GetText(0, -1, true);
                        alternates.Add(recognizedPhrase);
                        NDisplayAlternatesOfRecognizedPhrase
                            = SapiHelper.GetAlternateStrings(RecoResult, Dictation.MaxAlternates, CapitalizeAlternates, alternates);
                        Trace.WriteLine(LogLevel.Low, "There are {0} display alternates of the recognized phrase",
                                        NDisplayAlternatesOfRecognizedPhrase);
                        PopIndex = NDisplayAlternatesOfRecognizedPhrase + 1;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(LogLevel.Error, "Exception getting alternates: {0}", ex.Message);
                }
                return alternates;                
            }
        }

        public int Count { get { return Alternates.Count; } }

        public string GetText(int i)
        {
            return Alternates[i];
        }

        public string Pop()
        {
            int count = Count; // causes alternates to be retrieved and PopIndex to be initialized!
            //Trace.WriteLine(LogLevel.Low, "PopIndex: {0}, Count: {1}", PopIndex, count);
            if (PopIndex < Count)
                return GetText(PopIndex++);
            else
                return null;
        }

        public void Commit(string correctedText)
        {
            // If corrected text matches an alternate, commit the alternate
            for (int i = 0; i < Alternates.Count; i++)
                if (correctedText == Alternates[i])
                {
                    if (i <= NDisplayAlternatesOfRecognizedPhrase)
                    {
                        Trace.WriteLine(LogLevel.Low, "Correction successful, but no need to update speech model");
                        return;
                    }
                    else
                    {
                        int alternateIndex = i - NDisplayAlternatesOfRecognizedPhrase;
                        ISpeechPhraseAlternates alternates = RecoResult.Alternates(100, 0, -1);
                        alternates.Item(alternateIndex).Commit();
                        //SapiHelper.CommitText(RecoResult, 0, (uint)RecoResult.PhraseInfo.Elements.Count, correctedText, false);
                        Trace.WriteLine(LogLevel.Low, "Correction successful; speech model updated with chosen alternate");
                        return;
                    }
                }

            // Correction doesn't match any alternates
            SapiHelper.CommitText(RecoResult, 0, (uint)RecoResult.PhraseInfo.Elements.Count, correctedText, true);
            Trace.WriteLine(LogLevel.Low, "Correction successful; speech model updated with new text");
        }
    }

}
