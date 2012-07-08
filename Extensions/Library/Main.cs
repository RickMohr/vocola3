using System;
using System.Diagnostics; // Process
using System.Threading;
using Vocola;

namespace Library
{

    /// <summary>Functions commonly used in voice commands.</summary>
    /// <remarks>This class contains many of the functions most commonly used in voice commands. Because
    /// <see cref="Library.Main">Library.Main</see> appears by default in Vocola's base $using set, most command
    /// writers omit the <c>Main.</c> prefix when calling these functions.
    /// <para>Note that Vocola's special forms <b>If</b>, <b>Repeat</b>, and <b>Eval</b>, are not documented in this
    /// class or elsewhere in the function library. These forms do use function call syntax but they are not true
    /// functions. See the online Vocola documentation for descriptions. </para> </remarks>

    public class Main : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // HearCommand

        /// <summary>Executes a WSR command or Vocola command as if words were spoken.</summary>
        /// <param name="words">Words of command to be executed. May be a single argument
        /// with words separated by spaces, or multiple arguments each containing one or more words.</param>
        /// <remarks>Executes a Windows Speech Recognition (WSR) command or Vocola command as if you spoke
        /// the words specified by <paramref name="words"/>.
        /// <para>If a Vocola command is recognized, this function waits for the command to be fully executed before
        /// returning. If on the other hand a WSR command is recognized, this function may return before the command has
        /// been fully executed; calls to <see cref="Wait"/> may be necessary in such cases.</para>
        /// <para>Because of Vista security precautions, this function only works if Vocola
        /// is installed in the <c>Program Files</c> folder.</para></remarks>
        /// <example><code title="Speech dictionary shortcut">
        /// Edit Words = HearCommand("Open Speech Dictionary");</code>
        /// This command allows saying "Edit Words" to invoke the WSR command "Open Speech Dictionary".
        /// <code title="Override single-word WSR commands">
        /// &lt;wsrWord> := ( delete | escape | start | home | end
        ///              | scratch | undo | correct | copy | paste
        ///              );
        /// &lt;wsrWord> = HearCommand("Insert $1");
        /// </code>
        /// WSR defines many single-word commands, which can be mistakenly invoked when dictating a single word.
        /// This command redefines some common WSR single-word commands to insert the word as dictation
        /// by invoking the WSR "Insert" command.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]  // Let whatever gets called handle the dictation stack.
        static public void HearCommand(params string[] words)
        {
            VocolaApi.EmulateRecognize(System.String.Join(" ", words));
        }

        // ---------------------------------------------------------------------
        // InsertText

        /// <summary>Inserts text by sending keystrokes to the current application.</summary>
        /// <param name="text">Text to insert.</param>
        /// <remarks>Most commands send keystrokes to an application by including the keystroke text
        /// directly. But you can use this method instead if the text includes special keystroke syntax
        /// (such as <c>{Ctrl+c}</c>) which you want inserted literally rather than interpreted as a keystroke.</remarks>
        /// <example><code title="Insert keystroke syntax while editing a Vocola command">
        /// $if .vcl | .vch;
        ///   Insert Control &lt;key> = InsertText( {Ctrl+$1} );
        /// $end</code>
        /// This command could be used when creating or editing a Vocola command. Saying "Insert Control Charlie"
        /// would insert the literal text "{Ctrl+c}".
        /// </example>
        [VocolaFunction]
        static public void InsertText(string text)
        {
            VocolaApi.InsertText(text);
        }

        // ---------------------------------------------------------------------
        // RunProgram

        /// <summary>Executes a program.</summary>
        /// <param name="program">Name of the program to execute.</param>
        /// <remarks>Executes a program, as if <paramref name="program"/> were typed at a command prompt. Returns after
        /// launching the program.
        /// <para>If <paramref name="program"/> does not specify a full file path, the program
        /// must be in a folder listed in the PATH environment variable.</para></remarks>
        /// <example><code title="Launch Firefox">
        /// Start Browser = RunProgram("C:\Program Files\Mozilla Firefox\firefox.exe");</code>
        /// Launches Mozilla Firefox.
        /// <code title="Open a document">
        /// Show Home Page = RunProgram(C:/WebSite/index.html);</code>
        /// Specifying a file for the <paramref name="program"/> argument opens the file using its default application,
        /// here presumably a web browser.
        /// </example>
        /// <seealso cref="RunProgram(string, string)">RunProgram(program, arguments)</seealso>
        /// <seealso cref="RunProgram(string, string, bool)">RunProgram(program, arguments, waitForExit)</seealso>
        /// <seealso cref="RunProgram(string, string, bool, string)">RunProgram(program, arguments, waitForExit, workingDirectory)</seealso>
        [VocolaFunction]
        static public void RunProgram(string program)
        {
            RunProgram(program, null, false, null);
        }

        /// <summary>Executes a program, with arguments.</summary>
        /// <param name="program">Name of the program to execute.</param>
        /// <param name="arguments">Program arguments, separated by spaces. If an argument contains a space
        /// character, that argument should be enclosed in double quotes.</param>
        /// <remarks>Executes a program, as if <paramref name="program"/> followed by <paramref name="arguments"/> was typed at a command prompt.
        /// Returns after launching the program.
        /// <para>If <paramref name="program"/> does not specify a full file path, the program
        /// must be in a folder listed in the PATH environment variable.</para></remarks>
        /// <example><code title="Launch a perl script">
        /// Upload Home Page = RunProgram(perl, C:/Scripts/uploadHomePage.pl);</code>
        /// Launches a perl script, passing the name of the script as an argument.
        /// <code title="Copy a file">
        /// Run Backup = RunProgram(xcopy, '/y C:/Users/Pat/important.txt "F:/My Backups"');</code>
        /// Runs xcopy, passing 3 arguments.
        /// </example>
        /// <seealso cref="RunProgram(string)">RunProgram(program)</seealso>
        /// <seealso cref="RunProgram(string, string, bool)">RunProgram(program, arguments, waitForExit)</seealso>
        /// <seealso cref="RunProgram(string, string, bool, string)">RunProgram(program, arguments, waitForExit, workingDirectory)</seealso>
        [VocolaFunction]
        static public void RunProgram(string program, string arguments)
        {
            RunProgram(program, arguments, false, null);
        }

        /// <summary>Executes a program, with arguments and termination condition.</summary>
        /// <param name="program">Name of the program to execute.</param>
        /// <param name="arguments">Program arguments, separated by spaces. If an argument contains a space
        /// character, that argument should be enclosed in double quotes.</param>
        /// <param name="waitForExit">If <c>False</c>, return after launching
        /// the program. If <c>True</c>, wait until the program has exited before returning.</param>
        /// <remarks>Executes a program, as if <paramref name="program"/> followed by <paramref name="arguments"/> was typed at a command prompt.
        /// <para>If <paramref name="program"/> does not specify a full file path, the program
        /// must be in a folder listed in the PATH environment variable.</para></remarks>
        /// <seealso cref="RunProgram(string)">RunProgram(program)</seealso>
        /// <seealso cref="RunProgram(string, string)">RunProgram(program, arguments)</seealso>
        /// <seealso cref="RunProgram(string, string, bool, string)">RunProgram(program, arguments, waitForExit, workingDirectory)</seealso>
        [VocolaFunction]
        static public void RunProgram(string program, string arguments, bool waitForExit)
        {
            RunProgram(program, arguments, waitForExit, null);
        }

        /// <summary>Executes a program, with arguments, termination condition, and working directory.</summary>
        /// <param name="program">Name of the program to execute.</param>
        /// <param name="arguments">Program arguments, separated by spaces. If an argument contains a space
        /// character, that argument should be enclosed in double quotes.</param>
        /// <param name="waitForExit">If <c>False</c>, return after launching
        /// the program. If <c>True</c>, wait until the program has exited before returning.</param>
        /// <param name="workingDirectory">Working directory pathname for program execution.</param>
        /// <remarks>Executes a program, as if <paramref name="program"/> followed by <paramref name="arguments"/> was typed at a command prompt.
        /// <para>If <paramref name="program"/> does not specify a full file path, the program
        /// must be in a folder listed in the PATH environment variable.</para></remarks>
        /// <seealso cref="RunProgram(string)">RunProgram(program)</seealso>
        /// <seealso cref="RunProgram(string, string)">RunProgram(program, arguments)</seealso>
        /// <seealso cref="RunProgram(string, string, bool)">RunProgram(program, arguments, waitForExit)</seealso>
        [VocolaFunction]
        static public void RunProgram(string program, string arguments, bool waitForExit, string workingDirectory)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = program;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.Start();
                if (waitForExit)
                    process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw new VocolaExtensionException("RunProgram failed: {0}", ex.Message);
            }
        }

        // ---------------------------------------------------------------------
        // SendSystemKeys

        /// <summary>Sends keystrokes to an application by simulating keyboard events.</summary>
        /// <param name="keys">Keystroke(s) to send, using Vocola keystroke syntax.</param>
        /// <remarks>Most commands send keystrokes to an application by including the keystroke text
        /// directly. But if the direct method fails (as happens occasionally)
        /// this function may succeed, by simulating each "key down" and "key up" event for the specified keystrokes.</remarks>
        /// <example><code title='Raise "Start" menu'>
        /// Start Menu = SendSystemKeys({Ctrl+Esc});</code>
        /// This command raises the "Start" menu by sending the keystroke {Ctrl+Esc}.
        /// </example>
        [VocolaFunction]
        static public void SendSystemKeys(string keys)
        {
            VocolaApi.SendSystemKeys(keys);
        }

        // ---------------------------------------------------------------------
        // ShowMessage
        // ShowWarning

        /// <summary>Displays a message in the WSR user interface panel.</summary>
        /// <param name="message">The message to display.</param>
        /// <example><code title="Display a message">
        /// Copy That = {Ctrl+c} ShowMessage("Text copied");</code>
        /// When you say "Copy That", this command copies the currently-selected text and displays "Text copied" in the
        /// WSR user interface panel.
        /// </example>
        [VocolaFunction]
        static public void ShowMessage(string message)
        {
            VocolaApi.DisplayMessage(message, false);
        }

        /// <summary>Displays a warning message in the WSR user interface panel.</summary>
        /// <param name="message">The warning message to display.</param>
        /// <example><code title="Disable a WSR command">
        /// Start = ShowWarning("Command disabled");</code>
        /// WSR defines many single-word commands, which can be invoked by mistake.
        /// This command redefines the WSR command "Start" (which opens the start menu)
        /// to do nothing except show the warning "Command disabled" in the WSR user interface panel.
        /// </example>
        [VocolaFunction]
        static public void ShowWarning(string message)
        {
            VocolaApi.DisplayMessage(message, true);
        }

        // ---------------------------------------------------------------------
        // SwitchTo

        /// <summary>Activates a specified running application.</summary>
        /// <param name="applicationName">Name of a running application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <remarks>If the taskbar contains an instance of <paramref name="applicationName"/> (case insensitive), that instance
        /// becomes the active application. Otherwise,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// It also errors out if the specified application does not become active within 5 seconds.
        /// </remarks>
        /// <example><code title="Start a Web search">
        /// Web Search = SwitchTo(firefox) {Alt+g};</code>
        /// This command makes Firefox the active application and puts focus in the Google search box.
        /// </example>
        /// <seealso cref="SwitchTo(string, int)">SwitchTo(applicationName, timeout)</seealso>
        /// <seealso cref="TaskBar.SwitchToApplication(string,int)">TaskBar.SwitchToApplication</seealso>
        [VocolaFunction]
        static public void SwitchTo(string applicationName)
        {
            TaskBar.SwitchToApplication(applicationName, 1, 5);
        }

        /// <summary>Activates a specified running application, with a specified timeout.</summary>
        /// <param name="applicationName">Name of a running application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <param name="timeout">Number of seconds to wait before aborting.</param>
        /// <remarks>If the taskbar contains an instance of <paramref name="applicationName"/> (case insensitive), that instance
        /// becomes the active application. Otherwise,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// It also errors out if the specified application does not become active within <paramref name="timeout"/> seconds.
        /// <para>See <see cref="SwitchTo(string)">SwitchTo</see> for an example.</para>
        /// </remarks>
        /// <seealso cref="SwitchTo(string)">SwitchTo(applicationName)</seealso>
        /// <seealso cref="TaskBar.SwitchToApplication(string,int,int)">TaskBar.SwitchToApplication</seealso>
        [VocolaFunction]
        static public void SwitchTo(string applicationName, int timeout)
        {
            TaskBar.SwitchToApplication(applicationName, 1, timeout);
        }

        // ---------------------------------------------------------------------
        // Wait

        /// <summary>Pauses command execution for a specified number of milliseconds.</summary>
        /// <param name="milliseconds">Number of milliseconds to pause.</param>
        /// <remarks>When controlling applications by sending keystrokes, effects are not always instantaneous.
        /// Use this function to insert pauses in a command, allowing the active application to respond to previous actions.
        /// <para>Choosing the right number of milliseconds can be a challenging tradeoff between command speed and
        /// command reliability. When possible, use <see cref="WaitForWindow(string)">WaitForWindow</see> instead.</para></remarks>
        /// <example><code title="Close a window by pointing at the taskbar">
        /// Close Here = {RightButton} Wait(500) c;</code>
        /// This command allows you to close a window by pointing at its taskbar button and saying "Close Here".
        /// It clicks the right mouse button to raise the window's context menu and presses the "c" key to choose the "Close"
        /// option. The call <c>Wait(500)</c> is needed to give the context menu enough time to appear.
        /// </example>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void Wait(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

/*
        // ---------------------------------------------------------------------
        // WaitForDisambiguation

        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void WaitForDisambiguation()
        {
            WaitForDisambiguation(15);
        }

        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void WaitForDisambiguation(int timeout)
        {
            DateTime startTime = DateTime.Now;
            SpSharedRecoContext recognizer = new SpeechLib.SpSharedRecoContext();
            Wait(200); // Disambiguation mode appears to always start within 70 ms
            while ((DateTime.Now - startTime).TotalSeconds <= timeout)
            {
                if (recognizer.Recognizer.Status.NumberOfActiveRules > 20)
                {
                    Thread.Sleep(100); // Otherwise selection sometimes not yet made
                    return;
                }
                Thread.Sleep(10);
            }
            throw new VocolaExtensionException("Timeout waiting for disambiguation");
        }
*/

        // ---------------------------------------------------------------------
        // WaitForWindow

        /// <summary>Pauses command execution until a specified window is the foreground window.</summary>
        /// <param name="windowTitleFragment">All or part of the title of the desired window.</param>
        /// <remarks>Switching between windows is not instantaneous. When a command controls more than one window, 
        /// use this function to pause until the window you want is the foreground window.
        /// <para>If within 5 seconds the foreground window title does not contain <paramref name="windowTitleFragment"/>,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// </para></remarks>
        /// <example><code title="Change Notepad font size">
        /// Font Size 6..72 = {Alt+o}f WaitForWindow(font)
        ///                   {Alt+s} $1 {Enter};</code>
        /// This command changes Notepad's font size. <c>{Alt+o}</c> opens the "Format" menu and <c>f</c>
        /// chooses the "Font..." option. <c>WaitForWindow(font)</c> waits for the font dialog to appear because
        /// the font dialog's title contains the word "font". Without this call the remaining keystrokes would be sent too quickly
        /// and the command would not work correctly.
        /// </example>
        /// <seealso cref="WaitForWindow(string, int)">WaitForWindow(windowTitleFragment, timeout)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void WaitForWindow(string windowTitleFragment)
        {
            WaitForWindow(windowTitleFragment, 5);
        }

        /// <summary>Pauses command execution until a specified window is the foreground window, with a specified timeout.</summary>
        /// <param name="windowTitleFragment">All or part of the title of the desired window. Case insensitive.</param>
        /// <param name="timeout">Number of seconds to wait before aborting.</param>
        /// <remarks>Switching between windows is not instantaneous. When a command controls more than one window, 
        /// use this function to pause until the window you want is the foreground window.
        /// <para>If within <paramref name="timeout"/> seconds the foreground window title does not contain
        /// <paramref name="windowTitleFragment"/>,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.</para>
        /// <para>See <see cref="WaitForWindow(string)">WaitForWindow(windowTitleFragment)</see> for an example.</para></remarks>
        /// <seealso cref="WaitForWindow(string)">WaitForWindow(windowTitleFragment)</seealso>
        [VocolaFunction]
        [ClearDictationStack(false)]
        static public void WaitForWindow(string windowTitleFragment, int timeout)
        {
            string s = windowTitleFragment.ToLower();
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds <= timeout)
            {
                if (Win.GetForegroundWindowTitle().ToLower().Contains(s))
                {
                    Thread.Sleep(300); // Otherwise keystrokes sometimes missed
                    return;
                }
                Thread.Sleep(10);
            }
            throw new VocolaExtensionException("Timeout waiting for window '{0}'", windowTitleFragment);
        }
        
    }

}
