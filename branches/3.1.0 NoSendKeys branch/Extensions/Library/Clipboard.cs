using System;
using System.Threading;
using System.Windows.Forms; // Clipboard
using Vocola;

namespace Library
{

    /// <summary>Functions for accessing the Windows clipboard.</summary>
    public class Clipboard : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // ConvertToPlainText

        /// <summary>Converts the Windows clipboard content to plain text, if possible.</summary>
        /// <remarks>Data on the Windows clipboard may be in a format other than plain text, such as
        /// formatted text. This function converts the data to plain text, if possible.</remarks>
        /// <example><code title="Paste as plain text">
        /// Paste Clean = Clipboard.ConvertToPlainText() {Ctrl+v};</code>
        /// This command pastes the current clipboard contents as plain text. It is useful, for example,
        /// when copying text from a Web browser and pasting into Microsoft Word.
        /// </example>
        [VocolaFunction]
        static public void ConvertToPlainText()
        {
            if (HasData(DataFormats.Text))
                SetText(GetPlainText());
        }

        // ---------------------------------------------------------------------
        // GetText
        // SetText

        /// <summary>Returns the Windows clipboard content as plain text, if possible.</summary>
        /// <returns>A plain text version of the Windows clipboard content if available; nothing otherwise.</returns>
        /// <example><code title="Paste">
        /// Paste That = Clipboard.GetText();</code>
        /// For most programs you can implement a "Paste That" command using the keyboard shortcut <c>{Ctrl+v}</c>.
        /// The command in this example works in programs like Command Prompt which don't support that shortcut. Clipboard content
        /// is returned by the call to <c>GetText()</c> and sent as keystrokes to the current application.
        /// <code title='Convert text to a "title case" word'>
        /// Title That = {Ctrl+c} String.ToTitleCaseWord( Clipboard.GetText() );</code>
        /// This command converts selected text to a single "title case" word, by copying text to the clipboard with
        /// <c>{Ctrl+c}</c>, retrieving it with <see cref="GetText"/>, and converting it with <see
        /// cref="String.ToTitleCaseWord"/>.
        /// </example>
        [VocolaFunction]
        [CallEagerly(false)] // Support {Ctrl+c} Clipboard.GetText()
        static public string GetText()
        {
            if (HasData(DataFormats.Text))
                return GetPlainText();
            else
                return "";
        }

        /// <summary>Copies text to the Windows clipboard.</summary>
        /// <param name="text">Text to copy to the clipboard.</param>
        /// <example><code title="Get mouse coordinates">
        /// Get Pointer = Clipboard.SetText(Pointer.GetOffsetFromNearestCorner());</code>
        /// When creating voice commands which position the mouse pointer it's useful to retrieve the
        /// pointer coordinates for a specific point so you can put those coordinates in a command.
        /// The command in this example retrieves the current pointer coordinates as an
        /// offset from the nearest corner of the foreground window, and puts the result on the clipboard
        /// where it can be easily pasted into a command definition.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void SetText(string text)
        {
            System.Windows.Forms.Clipboard.SetDataObject(text, true);
        }

        static private bool HasData(string format)
        {
            IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
            if (data != null)
                return data.GetDataPresent(format);
            else
                return false;
        }

        static private string GetPlainText()
        {
            Thread.Sleep(100); // allow a previous "copy" to finish
            return System.Windows.Forms.Clipboard.GetDataObject().GetData(DataFormats.Text).ToString();
        }

    }

}
