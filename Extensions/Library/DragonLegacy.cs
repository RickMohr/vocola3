using System;
using System.Drawing; // Point
using System.Threading;
using Vocola;

namespace Library
{

    /// <summary>Functions from the legacy Dragon Macro Language, for Vocola 2 compatibility.</summary>
    /// <remarks>This class contains re-implementations of many commonly-used functions from the Dragon Macro Language.
    /// The most important capabilities are well-supported, but some lesser-used functions and capabilities are not
    /// currently supported. Contributions welcome.
    /// <para>Vocola 2 users whose existing commands call these functions have two options for upgrading to Vocola 3:
    /// <list type="number"><item> Use the Vocola 3 Command File Upgrader to convert the calls to their Vocola 3 analogs, or</item>
    /// <item>Include <see cref="DragonLegacy"/> in the default using set via the Vocola Options panel.</item>
    /// </list></para></remarks>
    public class DragonLegacy : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // AppBringUp
        // AppSwapWith

        /// <summary>Activates a specified running application.</summary>
        /// <param name="applicationName">Name of a running application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <remarks>If the taskbar contains an instance of <paramref name="applicationName"/> (case insensitive), that instance
        /// becomes the active application. Otherwise,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// <para>Unlike the Dragon function, this implementation does not launch the application if it's not running.</para>
        /// <para>See the suggested Vocola 3 analog <see cref="Main.SwitchTo(string)">Main.SwitchTo</see> for an example.</para></remarks>
        [VocolaFunction]
        static public void AppBringUp(string applicationName)
        {
            Main.SwitchTo(applicationName);
        }

        /// <summary>Activates a specified running application.</summary>
        /// <param name="applicationName">Name of a running application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <remarks>If the taskbar contains an instance of <paramref name="applicationName"/> (case insensitive), that instance
        /// becomes the active application. Otherwise,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// <para>Unlike the Dragon function, this implementation does not launch the application if it's not running.</para>
        /// <para>See the suggested Vocola 3 analog <see cref="Main.SwitchTo(string)">Main.SwitchTo</see> for an example.</para></remarks>
        [VocolaFunction]
        static public void AppSwapWith(string applicationName)
        {
            Main.SwitchTo(applicationName);
        }

        // ---------------------------------------------------------------------
        // ButtonClick

        /// <summary>Clicks the left button associated with the pointing device.</summary>
        /// <remarks>In a Vocola command, the pseudo-keystroke <c>{LeftButton}</c> may be used as
        /// an alternative to calling this function, with the advantage that modifier keys such as
        /// <c>Shift</c> and <c>Control</c> may be added.
        /// <para>See the suggested Vocola 3 analog <see cref="Pointer.Click()">Pointer.Click</see> for an
        /// example.</para></remarks>
        /// <seealso cref="ButtonClick(int)">ButtonClick(button)</seealso>
        /// <seealso cref="ButtonClick(int, int)">ButtonClick(button, count)</seealso>
        [VocolaFunction]
        static public void ButtonClick()
        {
            Pointer.Click(1, 1);
        }

        /// <summary>Clicks a button associated with the pointing device.</summary>
        /// <param name="button">A number specifying which button(s) to
        /// click. Use 1 for the left button, 2 for the right button, 4 for the middle button,
        /// or add the numbers to click multiple buttons simultaneously.</param>
        /// <remarks>In a Vocola command, the pseudo-keystrokes <c>{LeftButton}</c>, <c>{RightButton}</c>,
        /// and <c>{MiddleButton}</c> may be used as alternatives to calling this function, with the advantage that
        /// modifier keys such as <c>Shift</c> and <c>Control</c> may be added.
        /// <para>See the suggested Vocola 3 analog <see cref="Pointer.Click(Int32)">Pointer.Click</see> for an example.</para></remarks>
        /// <seealso cref="ButtonClick()">ButtonClick()</seealso>
        /// <seealso cref="ButtonClick(int, int)">ButtonClick(button, count)</seealso>
        [VocolaFunction]
        static public void ButtonClick(int button)
        {
            Pointer.Click(button, 1);
        }

        /// <summary>Clicks a button associated with the pointing device a specified number of times.</summary>
        /// <param name="button">A number specifying which button(s) to
        /// click. Use 1 for the left button, 2 for the right button, 4 for the middle button,
        /// or add the numbers to click multiple buttons simultaneously.</param>
        /// <param name="count">A number specifying how many times to click the button(s).</param>
        /// <remarks>In a Vocola command, the pseudo-keystrokes <c>{LeftButton}</c>, <c>{RightButton}</c>,
        /// and <c>{MiddleButton}</c> may be used as alternatives to calling this function, with the advantage that
        /// modifier keys such as <c>Shift</c> and <c>Control</c> may be added.
        /// <para>See the suggested Vocola 3 analog <see cref="Pointer.Click(Int32,Int32)">Pointer.Click</see>
        /// for an example.</para></remarks>
        /// <seealso cref="ButtonClick()">ButtonClick()</seealso>
        /// <seealso cref="ButtonClick(int)">ButtonClick(button)</seealso>
        [VocolaFunction]
        static public void ButtonClick(int button, int count)
        {
            Pointer.Click(button, count);
        }

        // ---------------------------------------------------------------------
        // ControlPick
        // MenuPick

        /// <summary>Invokes a specified on-screen control (such as a button).</summary>
        /// <param name="label">Label of control to invoke.</param>
        /// <remarks>There is no suggested Vocola 3 analog function, but <see cref="Main.HearCommand">Main.HearCommand</see>
        /// may be used as in this example:
        /// <code title="Cancel politely">
        /// Please Cancel = HearCommand("Click Cancel");</code>
        /// </remarks>
        [VocolaFunction]
        static public void ControlPick(string label)
        {
            VocolaApi.EmulateRecognize("Click " + label);
        }

        /// <summary>Invokes a specified menu item.</summary>
        /// <param name="label">Label of menu item to invoke.</param>
        /// <remarks>There is no suggested Vocola 3 analog function, but <see cref="Main.HearCommand">Main.HearCommand</see>
        /// may be used as in this example:
        /// <code title='Open "File" menu'>
        /// Menu File = HearCommand("Click File");</code>
        /// </remarks>
        [VocolaFunction]
        static public void MenuPick(string label)
        {
            VocolaApi.EmulateRecognize("Click " + label);
        }

        // ---------------------------------------------------------------------
        // GoToSleep
        // WakeUp

        /// <summary>Puts WSR in the "Sleeping" state.</summary>
        /// <remarks>There is no suggested Vocola 3 analog function, but <see cref="Main.HearCommand">Main.HearCommand</see>
        /// may be used as in this example:
        /// <code title="Go to sleep">
        /// Go To Sleep = HearCommand("Stop Listening");</code>
        /// </remarks>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void GoToSleep()
        {
            VocolaApi.EmulateRecognize("Stop Listening");
        }

        /// <summary>Puts WSR in the "Listening" state.</summary>
        /// <remarks>There is no suggested Vocola 3 analog function, but <see cref="Main.HearCommand">Main.HearCommand</see>
        /// may be used as in this example:
        /// <code title="Wake up">
        /// Wake Up = HearCommand("Start Listening");</code>
        /// </remarks>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void WakeUp()
        {
            VocolaApi.EmulateRecognize("Start Listening");
        }

        // ---------------------------------------------------------------------
        // HeardWord

        /// <summary>Executes a WSR command or Vocola command as if words were spoken.</summary>
        /// <param name="words">Words of command to be executed. May be a single argument
        /// with words separated by spaces, or multiple arguments each containing one or more words.</param>
        /// <remarks>Executes a Windows Speech Recognition (WSR) command or Vocola command as if you spoke
        /// the words specified by <paramref name="words"/>.
        /// <para>Because of Vista security issues, this function only works if the Vocola
        /// certificate is installed in the trusted root store.</para>
        /// <para>See the suggested Vocola 3 analog <see cref="Main.HearCommand">Main.HearCommand</see> for examples.</para></remarks>
        [VocolaFunction]
        [ClearDictationStack(false)]  // Let whatever gets called handle the dictation stack.
        static public void HeardWord(params string[] words)
        {
            Main.HearCommand(words);
        }

        // ---------------------------------------------------------------------
        // RememberPoint
        // DragToPoint

        /// <summary>Saves the current pointer position.</summary>
        /// <remarks>Saves the current pointer position for a future call to <see cref="DragToPoint"/>.
        /// <para>See the suggested Vocola 3 analog <see cref="Pointer.SavePoint">Pointer.SavePoint</see>.</para></remarks>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void RememberPoint()
        {
            Pointer.SavePoint();
        }

        /// <summary>Drags the pointer from the position stored by <see cref="RememberPoint"/> to the current position.</summary>
        /// <remarks>If <see cref="RememberPoint"/> was not called, 
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// <para>See the suggested Vocola 3 analog <see cref="Pointer.DragFromSavedPoint">Pointer.DragFromSavedPoint</see>.
        /// </para></remarks>
        [VocolaFunction]
        static public void DragToPoint()
        {
            Pointer.DragFromSavedPoint();
        }

        // ---------------------------------------------------------------------
        // SendKeys
        // SendSystemKeys

        /// <summary>Send keystrokes to the current application.</summary>
        /// <param name="keys">Keystroke(s) to send, using Vocola keystroke syntax.</param>
        /// <remarks>Calling this function in a command has the same effect as including the keystroke text
        /// directly.</remarks>
        [VocolaFunction]
        static public void SendKeys(string keys)
        {
            VocolaApi.SendKeys(keys);
        }

        /// <summary>Sends keystrokes to an application by simulating keyboard events.</summary>
        /// <param name="keys">Keystroke(s) to send, using Vocola keystroke syntax.</param>
        /// <remarks>Most commands send keystrokes to an application by including the keystroke text
        /// directly. But if the direct method fails (as happens occasionally)
        /// this function may succeed, by simulating each "key down" and "key up" event for the specified keystrokes.
        /// <para>This function is identical to the suggested Vocola 3 analog
        /// <see cref="Main.SendSystemKeys">Main.SendSystemKeys</see>.</para></remarks>
        [VocolaFunction]
        static public void SendSystemKeys(string keys)
        {
            VocolaApi.SendSystemKeys(keys);
        }

        // ---------------------------------------------------------------------
        // SetMousePosition

        /// <summary>Move pointer to a new position, relative to one of several interesting points.</summary>
        /// <param name="relativeTo">Indicates the meaning of the <paramref name="dx"/> and <paramref name="dy"/>
        /// coordinate arguments as follows:<br/>
        /// 0: Coordinates are relative to the top left of the screen's inner rectangle, as with
        /// <see cref="Pointer.MoveTo(int,int,string)">Pointer.MoveTo</see> with the <c>ScreenInner</c> argument.<br/>
        /// 1: Coordinates are relative to the top left of the current window, as with
        /// <see cref="Pointer.MoveTo(int,int)">Pointer.MoveTo</see>.<br/>
        /// 2: Coordinates are relative to the current pointer position, as with <see cref="Pointer.MoveBy">Pointer.MoveBy</see>.<br/>
        /// 5: Coordinates are relative to the top left of the current window's inner rectangle, as with
        /// <see cref="Pointer.MoveTo(int,int,string)">Pointer.MoveTo</see> with the <c>WindowInner</c> argument.
        /// </param>
        /// <param name="dx">Horizontal offset in pixels from point specified by <paramref name="relativeTo"/>.</param>
        /// <param name="dy">Vertical offset in pixels from point specified by <paramref name="relativeTo"/>.</param>
        /// <remarks>The suggested Vocola 3 analog functions are specified in the description of the
        /// <paramref name="relativeTo"/> argument.</remarks>
        /// <seealso cref="SetMousePosition(int, int)">SetMousePosition(relativeTo, edgeCode)</seealso>
        [VocolaFunction]
        static public void SetMousePosition(int relativeTo, int dx, int dy)
        {
            switch (relativeTo)
            {
            case 0: // move to (relative to desktop "work area")
                Pointer.MoveTo(dx, dy, "ScreenInner");
                break;
            case 1: // move to (relative to window)
                Pointer.MoveTo(dx, dy);
                break;
            case 2: // move by
                Pointer.MoveBy(dx, dy);
                break;
            case 5: // move to (relative to window client area)
                Pointer.MoveTo(dx, dy, "WindowInner");
                break;
            default:
                throw new VocolaExtensionException("Mode {0} is illegal for SetMousePosition(relativeTo, dx, dy)", relativeTo);
            }
        }

        static private string[] EdgeCodes = new string[] {
            "", "Top", "Right", "Bottom", "Left", "Top Right", "Bottom Right", "Bottom Left", "Top Left"
        };

        /// <summary>Move the pointer to an edge of the specified rectangle.</summary>
        /// <param name="relativeTo">Indicates the meaning of the <paramref name="edgeCode"/> argument as follows:<br/>
        /// 3: <paramref name="edgeCode"/> is relative to the screen's inner rectangle, as with
        /// <see cref="Pointer.MoveToEdge(string,string)">Pointer.MoveToEdge</see>with the <c>ScreenInner</c> argument.<br/>
        /// 4: <paramref name="edgeCode"/> is relative to the current window, as with
        /// <see cref="Pointer.MoveToEdge(string)">Pointer.MoveToEdge</see>.<br/>
        /// 6: <paramref name="edgeCode"/> is relative to the current window's inner rectangle, as with
        /// <see cref="Pointer.MoveToEdge(string,string)">Pointer.MoveToEdge</see> with the <c>WindowInner</c> argument.
        /// </param>
        /// <param name="edgeCode">The edge to move to, as follows:<br/>
        /// 1: Top edge of specified rectangle<br/>
        /// 2: Right edge of specified rectangle<br/>
        /// 3: Bottom edge of specified rectangle<br/>
        /// 4: Left edge of specified rectangle<br/>
        /// 5: Top Right corner of specified rectangle<br/>
        /// 6: Bottom Right corner of specified rectangle<br/>
        /// 7: Bottom Left corner of specified rectangle<br/>
        /// 8: Top Left corner of specified rectangle</param>
        /// <remarks>The suggested Vocola 3 analog functions are specified in the description of the
        /// <paramref name="relativeTo"/> argument.</remarks>
        /// <seealso cref="SetMousePosition(int, int, int)">SetMousePosition(relativeTo, dx, dy)</seealso>
        [VocolaFunction]
        static public void SetMousePosition(int relativeTo, int edgeCode)
        {
            switch (relativeTo)
            {
            case 3: // move to edge of work area (desktop minus taskbar)
                Pointer.MoveToEdge(EdgeCodes[edgeCode], "ScreenInner");
                break;
            case 4: // move to edge of window
                Pointer.MoveToEdge(EdgeCodes[edgeCode]);
                break;
            case 6: // move to edge of window client area
                Pointer.MoveToEdge(EdgeCodes[edgeCode], "WindowInner");
                break;
            default:
                throw new VocolaExtensionException("Mode {0} is illegal for SetMousePosition(relativeTo, edgeCode)", relativeTo);
            }
        }

        // ---------------------------------------------------------------------
        // ShellExecute

        /// <summary>Executes a program, with optional command line arguments.</summary>
        /// <param name="command">Name of the program to execute, followed by any arguments.</param>
        /// <remarks>Executes a program, as if <paramref name="command"/> were typed at a command prompt.
        /// <para>If <paramref name="command"/> does not specify a full file path, the program
        /// must be in a folder listed in the PATH environment variable.</para>
        /// <para>See the suggested Vocola 3 analog <see cref="Main.RunProgram(string)">Main.RunProgram</see>
        /// for an example.</para></remarks>
        [VocolaFunction]
        static public void ShellExecute(string command)
        {
            string arguments;
            string program = ParseCommand(command, out arguments);
            Main.RunProgram(program, arguments);
        }

        static private string ParseCommand(string command, out string arguments)
        {
            string program;
            arguments = "";
            command = command.Trim();
            if (command.StartsWith("\""))
            {
                int closeQuoteIndex = command.IndexOf("\"", 1);
                if (closeQuoteIndex > -1)
                {
                    program = command.Substring(1, closeQuoteIndex - 1);
                    arguments = command.Substring(closeQuoteIndex + 1);
                }
                else
                    program = command;
            }
            else
            {
                int spaceIndex = command.IndexOf(" ");
                if (spaceIndex > -1)
                {
                    program = command.Substring(0, spaceIndex);
                    arguments = command.Substring(spaceIndex + 1);
                }
                else
                    program = command;
            }
            return program;            
        }

        /// <summary>Executes a program, using the specified working directory.</summary>
        /// <param name="command">Name of the program to execute, followed by any arguments.</param>
        /// <param name="ignore">Parameter ignored.</param>
        /// <param name="workingDirectory">Working directory pathname for program execution.</param>
        /// <remarks>Executes a program, as if <paramref name="command"/> were typed at a command prompt
        /// with the working directory set to <paramref name="workingDirectory"/>.
        /// <para>If <paramref name="command"/> does not specify a full file path, the program
        /// must be in a folder listed in the PATH environment variable.</para>
        /// <para>See the suggested Vocola 3 analog <see cref="Main.RunProgram(string)">Main.RunProgram</see>
        /// for an example.</para></remarks>
        [VocolaFunction]
        static public void ShellExecute(string command, int ignore, string workingDirectory)
        {
            string arguments;
            string program = ParseCommand(command, out arguments);
            Main.RunProgram(program, arguments, false, workingDirectory);
        }

        // ---------------------------------------------------------------------
        // Wait

        /// <summary>Pauses command execution for a specified number of milliseconds.</summary>
        /// <param name="milliseconds">Number of milliseconds to pause.</param>
        /// <remarks>When controlling applications by sending keystrokes, effects are not always instantaneous.
        /// Use this function to insert pauses in a command, allowing the active application to respond to previous actions.
        /// <para>Choosing the right number of milliseconds can be a challenging tradeoff between command speed and
        /// command reliability. When possible, use <see cref="Main.WaitForWindow(string)">Main.WaitForWindow</see> instead.</para>
        /// <para>See the (identical) Vocola 3 analog <see cref="Main.Wait">Main.Wait</see> for an example.</para></remarks>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        // ---------------------------------------------------------------------
        // WaitForWindow

        /// <summary>Pauses command execution until a specified window is the foreground window.</summary>
        /// <param name="windowTitleFragment">All or part of the title of the desired window.</param>
        /// <remarks>Switching between windows is not instantaneous. When a command controls more than one window, 
        /// use this function to pause until the window you want is the foreground window.
        /// <para>If within 5 seconds the foreground window title does not contain <paramref name="windowTitleFragment"/>,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.</para>
        /// <para>See the suggested Vocola 3 analog <see cref="Main.WaitForWindow(string)">Main.WaitForWindow</see>
        /// for an example.</para></remarks>
        /// <seealso cref="WaitForWindow(string, string, int)">WaitForWindow(windowTitleFragment, ignore, timeout)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void WaitForWindow(string windowTitleFragment)
        {
            Main.WaitForWindow(windowTitleFragment.Replace("*",""));
        }

        /// <summary>Pauses command execution until a specified window is the foreground window, with a specified timeout.</summary>
        /// <param name="windowTitleFragment">All or part of the title of the desired window. Case insensitive.</param>
        /// <param name="ignore">Parameter ignored.</param>
        /// <param name="timeout">Number of seconds to wait before aborting.</param>
        /// <remarks>Switching between windows is not instantaneous. When a command controls more than one window, 
        /// use this function to pause until the window you want is the foreground window.
        /// <para>If within <paramref name="timeout"/> seconds the foreground window title does not contain
        /// <paramref name="windowTitleFragment"/>,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.</para>
        /// <para>See the suggested Vocola 3 analog <see cref="Main.WaitForWindow(string,int)">Main.WaitForWindow</see>.
        /// </para></remarks>
        /// <seealso cref="WaitForWindow(string)">WaitForWindow(windowTitleFragment)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void WaitForWindow(string windowTitleFragment, string ignore, int timeout)
        {
            Main.WaitForWindow(windowTitleFragment.Replace("*",""), timeout/1000);
        }
        
    }

}
