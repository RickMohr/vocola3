using System;
using System.Collections.Generic;
using System.Text;
using Vocola;

namespace Library
{

    /// <summary>Functions related to Vocola dictation.</summary>
    /// <remarks>Vocola supports dictation because for many applications Windows Speech Recognition (WSR)
    /// allows dictation only via the correction panel and does not support correction or modification of dictated text.
    /// <para>This <see cref="Dictation"/> class allows correction and modification of the phrase most recently recognized by
    /// Vocola dictation, through functions such as <see cref="Get"/>, <see cref="Correct"/>,
    /// and <see cref="Replace"/>. Vocola maintains a stack of recently-dictated phrases, so earlier phrases can be accessed
    /// if the just-dictated phrase is removed (see <see cref="Replace"/>).</para>
    /// <para>The functions in this class cannot access phrases recognized by WSR dictation.</para>
    /// <para>Other than the stack of recently-dictated phrases, Vocola has no knowledge of a document's text or insertion point.
    /// Vocola modifies the just-dictated phrase by first sending enough "backspace" keystrokes to remove it,
    /// so modification will not work correctly if the insertion point moves from the end of the just-dictated phrase.
    /// To be conservative, Vocola clears the dictation stack after a command action unless that action is known to not affect
    /// the insertion point.</para>
    /// </remarks>
    public class Dictation : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // Correct

        /// <summary>Raises the Vocola correction panel to correct the just-dictated phrase.</summary>
        /// <remarks>If alternate text is chosen from the correction panel, this function replaces the
        /// just-dictated phrase by first sending enough "backspace" keystrokes to remove it,
        /// and then sending the alternate text keystrokes.
        /// It will not work correctly if the insertion point has moved from the end of the just-dictated phrase.
        /// <para>If the Vocola dictation stack is empty, this function does nothing.</para></remarks>
        /// <example><code title="Correct dictation">
        /// Fix That = If( Dictation.CanGet(),
        ///                Dictation.Correct(),
        ///                HearCommand("Correct That") );</code>
        /// This command allows correcting both Vocola dictation and WSR dictation. If <see cref="CanGet"/> reports that
        /// Vocola dictation is available, <see cref="Correct"/> raises the Vocola correction panel. Otherwise,
        /// attempt to raise the WSR correction panel using <see cref="Main.HearCommand"/>.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void Correct()
        {
            if (VocolaDictation.TextJustDictated == null)
                throw new VocolaExtensionException(LogLevel.High, "Dictation.Correct() called but there is nothing to correct.");
            else
                VocolaDictation.Correct();
        }

        // ---------------------------------------------------------------------
        // Disable
        // Enable

        /// <summary>Disables Vocola dictation.</summary>
        /// <example><code title="Disable Vocola dictation.">
        /// Text Off = Dictation.Disable();</code>
        /// Saying "Text Off" disables Vocola dictation.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void Disable()
        {
            VocolaDictation.Enable(false);
        }

        /// <summary>Enables Vocola dictation.</summary>
        /// <remarks>Since dictation is enabled by default, calling this function only makes sense if dictation
        /// was previously disabled by a call to <see cref="Disable"/>.</remarks>
        /// <example><code title="Enable Vocola dictation.">
        /// Text On = Dictation.Enable();</code>
        /// Saying "Text On" enables Vocola dictation.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void Enable()
        {
            VocolaDictation.Enable(true);
        }

        // ---------------------------------------------------------------------
        // DisableForApplication
        // DisableForWindow
        // EnableForApplication
        // EnableForWindow

        /// <summary>Disables Vocola dictation for a specific application.</summary>
        /// <param name="applicationName">Name of an application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <remarks>Voice commands are sometimes misinterpreted as dictation, which can cause trouble in applications
        /// which interpret single-character keystrokes as actions. For such applications you may want to disable dictation
        /// using this function or <see cref="DisableForWindow"/>. 
        /// <para>Use <see cref="EnableForApplication"/> if you want to re-enable dictation for the specified application.</para>
        /// <para>Note that calling the function <see cref="Enable"/> has no effect on applications where
        /// dictation has been disabled with <see cref="DisableForApplication"/>.</para>
        /// <para>See <see cref="DisableForWindow"/> for an example.</para></remarks>
        [VocolaFunction]
        static public void DisableForApplication(string applicationName)
        {
            VocolaDictation.EnableForWindow(false, applicationName, "");
        }

        /// <summary>Disables Vocola dictation for a specific application window.</summary>
        /// <param name="applicationName">Name of an application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <param name="windowTitleFragment">All or part of the title of the desired window. Case insensitive.</param>
        /// <remarks>Voice commands are sometimes misinterpreted as dictation, which can cause trouble in applications
        /// which interpret single-character keystrokes as actions. For such applications you may want to disable dictation
        /// using this function or <see cref="DisableForApplication"/>.
        /// <para>Use <see cref="EnableForWindow"/> if you want to re-enable dictation for the specified window.</para>
        /// <para>Note that calling the function <see cref="Enable"/> has no effect on applications where
        /// dictation has been disabled with <see cref="DisableForWindow"/>.</para></remarks>
        /// <example><code title="Disable dictation for a window">
        /// onLoad() := Dictation.DisableForWindow(thunderbird, Thunderbird);</code>
        /// The Mozilla Thunderbird mailer's main window interprets single-character keystrokes as actions and doesn't
        /// require dictation, so it's safer to disable dictation for that window.
        /// <para>This code disables dictation for that window by specifying the "thunderbird" application, in windows
        /// containing the text "Thunderbird".</para>
        /// <para> Here <see cref="DisableForWindow"/> is called not by a voice command but by defining an
        /// <c>onLoad</c> function. Putting this code in <c>thunderbird.vcl</c> or <c>_global.vcl</c> will
        /// disable dictation in Thunderbird's main window without further action on your part.</para>
        /// </example>
        [VocolaFunction]
        static public void DisableForWindow(string applicationName, string windowTitleFragment)
        {
            VocolaDictation.EnableForWindow(false, applicationName, windowTitleFragment);
        }

        /// <summary>Enables Vocola dictation for a specific application.</summary>
        /// <param name="applicationName">Name of an application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <remarks>Since dictation is enabled by default, calling this function only makes sense if dictation
        /// was previously disabled by a call to <see cref="DisableForApplication"/> .
        /// <para>Calling this function has no effect if dictation was disabled using <see cref="Disable"/>.</para></remarks>
        [VocolaFunction]
        static public void EnableForApplication(string applicationName)
        {
            VocolaDictation.EnableForWindow(true, applicationName, "");
        }

        /// <summary>Enables Vocola dictation for a specific application window.</summary>
        /// <param name="applicationName">Name of an application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <param name="windowTitleFragment">All or part of the title of the desired window. Case insensitive.</param>
        /// <remarks>Since dictation is enabled by default, calling this function only makes sense if dictation
        /// was previously disabled by a call to <see cref="DisableForWindow"/> .
        /// <para>Calling this function has no effect if dictation was disabled using <see cref="Disable"/>.</para></remarks>
        [VocolaFunction]
        static public void EnableForWindow(string applicationName, string windowTitleFragment)
        {
            VocolaDictation.EnableForWindow(true, applicationName, windowTitleFragment);
        }

        // ---------------------------------------------------------------------
        // CanGet
        // Get

        /// <summary>Indicates whether a just-dictated phrase is available to <see cref="Get"/>.</summary>
        /// <returns><c>"True"</c> if a just-dictated phrase is available to <see cref="Get"/>; <c>"False"</c> otherwise.</returns>
        /// <remarks><para>See <see cref="Correct"/> for an example.</para></remarks>
        [VocolaFunction]
        static public bool CanGet()
        {
            return (VocolaDictation.TextJustDictated != null);
        }

        /// <summary>Returns the phrase most recently recognized by Vocola dictation, if available.</summary>
        /// <returns>The phrase most recently recognized by Vocola dictation, if available; nothing otherwise.</returns>
        /// <remarks><para>See <see cref="Correct"/> for an example.</para></remarks>
        [VocolaFunction]
        static public string Get()
        {
            string text = VocolaDictation.TextJustDictated;
            if (text == null)
                throw new VocolaExtensionException(LogLevel.High, "Dictation.Get() called but there is nothing to get.");
            else
                return text;                
        }

        // ---------------------------------------------------------------------
        // Replace

        /// <summary>Replaces the just-dictated phrase with specified text.</summary>
        /// <param name="newText">Text to replace the just-dictated phrase.</param>
        /// <remarks>This function replaces the just-dictated phrase by first sending enough "backspace" keystrokes to remove it,
        /// and then sending the keystrokes specified by <paramref name="newText"/>.
        /// It will not work correctly if the insertion point has moved from the end of the just-dictated phrase.
        /// <para>If <paramref name="newText"/> is empty, this function removes the just-dictated phrase
        /// from the dictation stack, leaving the previously-dictated phrase (if any) on top.</para>
        /// <para>If the Vocola dictation stack is empty, this function does nothing.</para></remarks>
        /// <example><code title="Capitalize the just-dictated phrase">
        /// Cap That = If( Dictation.CanGet(),
        ///                Dictation.Replace(String.Capitalize(Dictation.Get())),
        ///                HearCommand("Capitalize That") );</code>
        /// This command allows capitalizing both Vocola dictation and WSR dictation. If <see cref="CanGet"/> reports that
        /// Vocola dictation is available we capitalize the just-dictated Vocola phrase; otherwise, we
        /// attempt to capitalize the just-dictated WSR phrase using <see cref="Main.HearCommand"/>.
        /// <para>Three Vocola library functions are used to capitalize the just-dictated Vocola phrase: <see cref="Get"/>
        /// retrieves the just-dictated phrase, <see cref="String.Capitalize"/> capitalizes it, and <see cref="Replace"/>
        /// replaces it.</para>
        /// <para>This is one of Vocola's built-in commands.</para>
        /// <code title="Remove the just-dictated phrase">
        /// Scratch That = Dictation.Replace("");</code>
        /// Saying "Scratch That" removes the just-dictated phrase. Subsequent dictation-modification commands
        /// will affect the previously-dictated phrase.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void Replace(string newText)
        {
            string oldText = VocolaDictation.TextJustDictated;
            if (oldText == null)
                throw new VocolaExtensionException(LogLevel.High, "Dictation.Replace() called but there is nothing to replace.");
            string backspaces = "{BACKSPACE " + oldText.Length.ToString() + "}";
            VocolaApi.SendKeys(backspaces);
            VocolaApi.InsertText(newText);
            VocolaDictation.Replace(newText);
        }

        // ---------------------------------------------------------------------
        // ReplaceInActiveText

        /// <summary>Modifies specified text in recently-dictated phrases.</summary>
        /// <param name="oldText">Text that should be replaced.</param>
        /// <param name="newText">Replacement text.</param>
        /// <remarks>This function replaces the most recent occurrence of <paramref name="oldText"/> with <paramref
        /// name="newText"/> in active dictation. Active dictation contains all phrases dictated since the last command,
        /// possibly separated by pauses.</remarks>
        /// <example><code title="Capitalize a word in a recently-spoken phrase">
        /// Cap &lt;_vocolaDictation> = Dictation.ReplaceInActiveText($1, String.Capitalize($1));</code>
        /// If you said "The town public library <i>(pause)</i> is excellent", and then "Cap public library", you would
        /// see "The town Public Library is excellent".
        /// <para>The special variable <c>&lt;_vocolaDictation></c> matches
        /// any series of words in active dictation. Those words are then capitalized using <see cref="String.Capitalize"/>, and both the
        /// original and capitalized versions are passed to <see cref="ReplaceInActiveText"/>.</para>
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void ReplaceInActiveText(string oldText, string newText)
        {
            if (VocolaDictation.TextJustDictated == null)
                throw new VocolaExtensionException(LogLevel.High, "Dictation.ReplaceInActiveText() called but there is no active text.");
            string tail = VocolaDictation.ReplaceInActiveText(oldText, newText);
            int nBackspaces = oldText.Length + tail.Length;
            string backspaces = "{BACKSPACE " + nBackspaces.ToString() + "}";
            VocolaApi.SendKeys(backspaces);
            VocolaApi.InsertText(newText + tail);
        }

        // ---------------------------------------------------------------------
        // ReplaceWithAlternate

        /// <summary>Replace the just-dictated phrase with the best-guess alternate.</summary>
        /// <remarks>This function replaces the just-dictated phrase with the best-guess alternate by first sending
        /// enough "backspace" keystrokes to remove it, and then sending the alternate keystrokes.
        /// It will not work correctly if the insertion point has moved from the end of the just-dictated phrase.
        /// <para>Successive calls to this function retrieve successive alternates.</para>
        /// <para>If the Vocola dictation stack is empty, this function does nothing.</para></remarks>
        /// <example><code title='"Quick fix" correction'>
        /// Try Again = Dictation.ReplaceWithAlternate();</code>
        /// Sometimes you just know that the speech engine's best-guess alternate is the right one, and you want to
        /// try it without the overhead of raising the correction panel.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void ReplaceWithAlternate()
        {
            string oldText = VocolaDictation.TextJustDictated;
            string newText = VocolaDictation.PopAlternates();
            if (oldText == null)
                throw new VocolaExtensionException(
                    LogLevel.High, "Dictation.ReplaceWithAlternate() called but there is nothing to replace.");
            else if (newText == null)
                throw new VocolaExtensionException(
                    LogLevel.High, "Dictation.ReplaceWithAlternate() called but there are no alternates.");
            else
            {
                // Replicate initial space and capitalization
                if (oldText.StartsWith(" "))
                {
                    if (Char.IsUpper(oldText[1]))
                        newText = Char.ToUpper(newText[0]) + newText.Substring(1);
                    newText = " " + newText;
                }
                if (Char.IsUpper(oldText[0]))
                    newText = Char.ToUpper(newText[0]) + newText.Substring(1);
                // Replace it
                string backspaces = "{BACKSPACE " + oldText.Length.ToString() + "}";
                VocolaApi.SendKeys(backspaces + newText);
                VocolaDictation.Replace(newText);
            }
        }

        // ---------------------------------------------------------------------
        // ShowDictationShortcutsDialog

        /// <summary>Activates the Vocola "Dictation Shortcuts" dialog box.</summary>
        /// <remarks>A dictation shortcut allows you to say one word sequence within a dictated phrase
        /// and have a different word sequence inserted. This function raises Vocola's
        /// dialog box which allows managing your personal collection of dictation shortcuts.</remarks>
        /// <example><code title='Activate "Dictation Shortcuts" dialog'>
        /// Dictation Shortcuts = Dictation.ShowDictationShortcutsDialog();</code>
        /// Saying "Dictation Shortcuts" activates the dictation shortcuts dialog.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void ShowDictationShortcutsDialog()
        {
            VocolaDictation.ShowDictationShortcutsDialog();
        }
        
    }

}
