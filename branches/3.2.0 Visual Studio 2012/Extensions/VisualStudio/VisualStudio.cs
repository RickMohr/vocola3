using System;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Vocola;

namespace Library
{

    /// <summary>Functions to control Microsoft Visual Studio via its automation interface.
    /// <para>Note that if more than one instance of Visual Studio is running this extension can only communicate with the
    /// first one launched, even if another one is in the foreground.</para>
    /// </summary>
    public class VisualStudio : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // GoToVisibleLine

        /// <summary>Moves caret to a visible line, specified by last two digits of line number.</summary>
        /// <param name="lineNumberSuffix">Last two digits of desired line number.</param>
        /// <example><code title="Paste clipboard contents at specified line">
        /// 0..99 Paste = VisualStudio.GoToVisibleLine($1) {Ctrl+v};</code>
        /// If Visual Studio is currently displaying lines 378-432 of the current document, saying "12 Paste" 
        /// pastes the current clipboard contents at line 412.
        /// </example>
        [VocolaFunction]
        static public void GoToVisibleLine(int lineNumberSuffix)
        {
            GoToVisibleLine(lineNumberSuffix, false);
        }

        /// <summary>Moves caret or extends selection to a visible line, specified by last two digits of line number.</summary>
        /// <param name="lineNumberSuffix">Last two digits of desired line number.</param>
        /// <param name="extend"><b>true</b> to extend the selection, or <b>false</b> to move the caret.</param>
        /// <remarks>See <see cref="GoToVisibleLine(int)">GoToVisibleLine</see> for an example.</remarks>
        [VocolaFunction]
        static public void GoToVisibleLine(int lineNumberSuffix, bool extend)
        {
            FocusOnCurrentDocument();
            TextWindow textWindow = (TextWindow)DTE2.ActiveWindow.Object;
            TextPane2 pane = (TextPane2)textWindow.ActivePane;
            int firstVisibleLine = pane.StartPoint.Line;
            int line = firstVisibleLine / 100 * 100 + lineNumberSuffix;
            if (line < firstVisibleLine)
                line += 100;
            textWindow.Selection.MoveToLineAndOffset(line, 1, extend);
        }

        // ---------------------------------------------------------------------
        // SetCurrentFileVariables

        /// <summary>Puts Visual Studio's current filename, line number, and column number into specified Vocola variables.</summary>
        /// <param name="filenameVariable">Name of Vocola variable to contain file name of active Visual Studio document.</param>
        /// <param name="lineNumberVariable">Name of Vocola variable to contain current line number of caret.</param>
        /// <param name="columnNumberVariable">Name of Vocola variable to contain current column number of caret.</param>
        [VocolaFunction]
        [CallEagerly(true)]
        static public void SetCurrentFileVariables(string filenameVariable, string lineNumberVariable, string columnNumberVariable)
        {
            FocusOnCurrentDocument();
            VocolaApi.SetVariable(filenameVariable, DTE2.ActiveDocument.FullName);
            TextSelection selection = GetSelection();
            VocolaApi.SetVariable(lineNumberVariable, selection.CurrentLine.ToString());
            VocolaApi.SetVariable(columnNumberVariable, selection.CurrentColumn.ToString());
        }

        // ---------------------------------------------------------------------
        // SaveCaret
        // RestoreCaret

        static private EditPoint SavedCaret = null;

        /// <summary>Save the current caret position.</summary>
        [VocolaFunction]
        static public void SaveCaret()
        {
            TextSelection selection = GetSelection();
            if (selection != null)
                SavedCaret = selection.TopPoint.CreateEditPoint();
            else
                SavedCaret = null;
        }

        /// <summary>Restore the caret to the last saved position.</summary>
        [VocolaFunction]
        static public void RestoreCaret()
        {
            if (SavedCaret != null)
            {
                TextSelection selection = GetSelection();
                if (selection != null)
                    selection.MoveToAbsoluteOffset(SavedCaret.AbsoluteCharOffset, false);
            }
        }

        // ---------------------------------------------------------------------
        // RunCommand

        /// <summary>Run a Visual Studio command.</summary>
        /// <param name="command">Name of command to run.</param>
        [VocolaFunction]
        static public void RunCommand(string command)
        {
            RunCommand(command, "");
        }

        /// <summary>Run a Visual Studio command, with arguments.</summary>
        /// <param name="command">Name of command to run.</param>
        /// <param name="args">Command arguments.</param>
        [VocolaFunction]
        static public void RunCommand(string command, string args)
        {
            DTE2.ExecuteCommand(command, args);
        }

        // ---------------------------------------------------------------------
        // MoveByParagraphs

        /// <summary>Move the caret forward or back by the specified number of paragraphs.</summary>
        /// <param name="count">Number of paragraphs to move. If positive, move forward; if negative, move backward.</param>
        /// <remarks>Paragraphs are delimited by blank lines.</remarks>
        [VocolaFunction]
        static public void MoveByParagraphs(int count)
        {
            MoveByParagraphs(count, false);
        }

        /// <summary>Move the caret forward or back by the specified number of paragraphs.</summary>
        /// <param name="count">Number of paragraphs to move. If positive, move forward; if negative, move backward.</param>
        /// <param name="extend"><b>true</b> to extend the selection, or <b>false</b> to move the caret.</param>
        /// <remarks>Paragraphs are delimited by blank lines.</remarks>
        [VocolaFunction]
        static public void MoveByParagraphs(int count, bool extend)
        {
            TextSelection selection = GetSelection();
            if (selection == null)
                return;
            int initialOffset = selection.TopPoint.CreateEditPoint().AbsoluteCharOffset;
            if (count > 0) // Move forward
            {
                selection.CharRight(false, 1);
                bool moveToEndOfDocument = false;
                while (count-- > 0)
					if (selection.FindText(@"^[ \t]*$", (int)vsFindOptions.vsFindOptionsRegularExpression))
					{
						if (DteVersion < 10)
							selection.CharRight(false, 2); // move to start of next line
					}
					else
					{
						moveToEndOfDocument = true;
						break;
					}
                if (moveToEndOfDocument)
                    selection.EndOfDocument(false);
                else if (DteVersion < 10)
                    selection.CharLeft(false, 1);
            }
            else if (count < 0) // Move backward
            {
                while (count++ < 0)
                    if (!selection.FindText(@"^[ \t]*$", (int)(vsFindOptions.vsFindOptionsRegularExpression |
                                                               vsFindOptions.vsFindOptionsBackwards)))
                    {
                        selection.StartOfDocument(false);
                        break;
                    }
				if (DteVersion < 10)
					selection.CharLeft(false, 1);
            }
            if (extend)
                selection.MoveToAbsoluteOffset(initialOffset, true);
        }

        // ---------------------------------------------------------------------
        // Emacs-style word navigation
        // MoveByWords
        // SelectWords

        /// <summary>Move the caret forward or back by the specified number of words.</summary>
        /// <param name="count">Number of words to move. If positive, move forward; if negative, move backward.</param>
        /// <remarks>A word is defined as a contiguous sequence of letters or digits. This definition (borrowed from
        /// Emacs) is more convenient for editing code than the Microsoft definition, where punctuation symbols count as
        /// words.</remarks>
        [VocolaFunction]
        static public void MoveByWords(int count)
        {
            TextSelection selection = GetSelection();
            if (selection != null)
            {
                int newOffset = GetOffsetOfMoveByWords(selection, count);
                selection.MoveToAbsoluteOffset(newOffset, false);
            }
            else
            {
                // Focus is not on a document, so send normal keystrokes
                if (count >= 0)
                    VocolaApi.SendKeys("{Ctrl+Right_" + count.ToString() + "}");
                else
                    VocolaApi.SendKeys("{Ctrl+Left_" + (-count).ToString() + "}");
            }
        }

        /// <summary>Extend the selection forward or back by the specified number of words.</summary>
        /// <param name="count">Number of words to extend the selection. If positive, move forward; if negative, move backward.</param>
        /// <remarks>A word is defined as a contiguous sequence of letters or digits. This definition (borrowed from
        /// Emacs) is more convenient for editing code than the Microsoft definition, where punctuation symbols count as
        /// words.</remarks>
        [VocolaFunction]
        static public void SelectWords(int count)
        {
            TextSelection selection = GetSelection();
            if (selection != null)
            {
                int newOffset = GetOffsetOfMoveByWords(selection, count);
                selection.MoveToAbsoluteOffset(newOffset, true);
            }
            else
            {
                // Focus is not on a document, so send normal keystrokes
                if (count >= 0)
                    VocolaApi.SendKeys("{Ctrl+Shift+Right_" + count.ToString() + "}");
                else
                    VocolaApi.SendKeys("{Ctrl+Shift+Left_" + (-count).ToString() + "}");
            }
        }

        static private int GetOffsetOfMoveByWords(TextSelection selection, int count)
        {
            EditPoint editPoint = selection.TopPoint.CreateEditPoint();

            if (count > 0) // Move forward
            {
                while (count-- > 0)
                {
                    // Make sure we start at a word character 
                    if (!HasWordCharAfter(editPoint))
                        while (!editPoint.AtEndOfDocument && !HasWordCharAfter(editPoint))
                            editPoint.CharRight(1);
                    // Move to first non-word character
                    while (!editPoint.AtEndOfDocument && HasWordCharAfter(editPoint))
                        editPoint.CharRight(1);
                }
            }
            else if (count < 0) // Move backward
            {
                while (count < 0)
                {
                    count++;
                    // Make sure we start at a word character 
                    if (!HasWordCharBefore(editPoint))
                        while (!editPoint.AtStartOfDocument && !HasWordCharBefore(editPoint))
                            editPoint.CharLeft(1);
                    // Move to first non-word character
                    while (!editPoint.AtStartOfDocument && HasWordCharBefore(editPoint))
                        editPoint.CharLeft(1);
                }
            }

            return editPoint.AbsoluteCharOffset;
        }

        static private bool HasWordCharAfter(EditPoint editPoint)
        {
            if (editPoint.AtEndOfDocument)
                return false;
            else
                return Char.IsLetterOrDigit(editPoint.GetText(1)[0]);
        } 

        static private bool HasWordCharBefore(EditPoint editPoint)
        {
            if (editPoint.AtStartOfDocument)
                return false;
            else
                return Char.IsLetterOrDigit(editPoint.GetText(-1)[0]);
        } 

        // ---------------------------------------------------------------------
        // Utilities

        // Open and maintain a connection to the Visual Studio automation object.
        // Because the connection times out after a while, always try the connection before using it.
        // Reestablish it if there's a RemotingException.

        static private DTE2 dte2 = null;
        static private DTE2 DTE2
        {
            get
            {
                if (dte2 != null)
                {
                    try
                    {
                        // We have a connection -- make sure it's still alive
                        VocolaApi.LogMessage(LogLevel.Low, "Testing connection to Visual Studio");
                        string dummy = dte2.Name;
                    }
                    catch (Exception ex) // (System.Runtime.Remoting.RemotingException)
                    {
                        // No connection -- reestablish below
                        dte2 = null;
                        VocolaApi.LogMessage(LogLevel.Low, String.Format("Test failed: '{0}' ({1})", ex.Message, ex.GetType().FullName));
                        VocolaApi.LogMessage(LogLevel.Low, "Reestablishing connection to Visual Studio");
                        //try
                        //{
                        //    // Haven't been able to eliminate "The pipe is being closed" exception, so just trap it
                        //    dte2 = (DTE2)VocolaApi.GetAutomationObject("VisualStudio.DTE");
                        //}
                        //catch { }
                    }
                }
				if (dte2 == null)
					dte2 = GetAutomationObject("VisualStudio.DTE");
                if (dte2 == null)
                    dte2 = GetAutomationObject("VisualStudio.DTE.10.0");
                if (dte2 == null)
                    dte2 = GetAutomationObject("VisualStudio.DTE.9.0");
                if (dte2 == null)
					dte2 = GetAutomationObject("VisualStudio.DTE.8.0");
				return dte2;
            }
        }

        static private DTE2 GetAutomationObject(string progId)
        {
            try
            {
                return (DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject(progId);
            }
            catch
            {
                return null;
            }
            //return (DTE2)VocolaApi.GetAutomationObject(progId);
        }

            
		static private int DteVersion
		{
get
			{
				float version = 0;
				float.TryParse(DTE2.Version, out version);
				return (int)version;
			}
		}

        static private void FocusOnCurrentDocument()
        {
            Document activeDocument;
            try
            {
                activeDocument = DTE2.ActiveDocument;
            }
            catch (Exception ex)
            {
                throw new VocolaExtensionException("Exception ({0}) getting Visual Studio active document: {1}", ex.GetType(), ex.Message);
            }
            if (activeDocument == null)
                throw new VocolaExtensionException("This instance of Visual Studio has no active document");
            try
            {
                activeDocument.Activate();
            }
            catch (Exception ex)
            {
                throw new VocolaExtensionException("Exception ({0}) activating Visual Studio document: {1}", ex.GetType(), ex.Message);
            }
        }

        static private TextSelection GetSelection()
        {
            DTE2 dte2 = DTE2;
            if (dte2.ActiveWindow == null || dte2.ActiveWindow.Document == null)
                return null;
            try
            {
                return (TextSelection)dte2.ActiveDocument.Selection;
            }
            catch (Exception ex)
            {
                throw new VocolaExtensionException("Exception ({0}) getting Visual Studio selection: {1}", ex.GetType(), ex.Message);
            }
        }

    }
}
