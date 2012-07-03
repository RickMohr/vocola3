using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics; // Process
using Vocola;

namespace Library
{

    /// <summary>Functions for accessing Vocola command files.</summary>
    /// <remarks>Vocola command files contain user-defined Vocola commands and are stored in the Vocola command folder.
    /// The default Vocola command folder for a user named <c>Bob</c> is <c>C:\Users\Bob\Documents\Vocola\Commands</c>,
    /// but this location may be changed via the Vocola Options panel.
    /// <para>Functions such as <see cref="OpenCurrent"/> which open command files rely on the user
    /// associating the extensions <c>.vcl</c> and <c>.vch</c> with a particular text editor.</para>
    /// </remarks>

    public class CommandFile : VocolaExtension
    {
 
        // ---------------------------------------------------------------------
        // GetBuiltinsPathname

        /// <summary>Adds as a prefix to the given file name the pathname of the Vocola "CommandBuiltins" installation
        /// folder.</summary>
        /// <param name="filename">File name to be prefixed.</param>
        /// <returns>Given file name with added prefix of Vocola "CommandBuiltins" installation folder pathname.</returns>
        /// <example><code title="Include dictation commands">
        /// $include CommandFile.GetBuiltinsPathname(keys.vch);
        /// Touch &lt;key&gt; = {LeftButton}{$1};</code>
        /// Vocola ships with a collection of useful key definitions, such as <c>&lt;key&gt;</c>, defined
        /// in the installation file <c>keys.vch</c>. This example shows how to include those definitions
        /// in a command file so they can be used. If the installation folder containing built-in commands is
        /// <c>C:\Program Files\Vocola\Commands\Builtins</c> this command is equivalent to <c>$include "C:\Program
        /// Files\Vocola\Commands\Builtins\keys.vch;"</c>. 
        /// </example>
        [VocolaFunction]
        static public string GetBuiltinsPathname(string filename)
        {
            return Path.Combine(VocolaApi.CommandBuiltinsFolder, filename);
        }

        // ---------------------------------------------------------------------
        // GetCommandFolder

        /// <summary>Gets the pathname to the folder containing user-defined Vocola commands.</summary>
        /// <returns>Full pathname to the folder containing user-defined Vocola commands.</returns>
        [VocolaFunction]
        static public string GetCommandFolder()
        {
            return VocolaApi.CommandFolder;
        }

        // ---------------------------------------------------------------------
        // GetErrorFilename
        // GetErrorLineNumber
        // GetErrorColumnNumber

        /// <summary>Gets the file name of the most recently loaded Vocola command file which contained an error.</summary>
        /// <returns>The file name of the most recently loaded Vocola command file which contained an error.</returns>
        /// <example><code title="Show Vocola code for recent error">
        /// Show Error = CommandFile.Open(CommandFile.GetErrorFilename())
        ///              {Ctrl+Home}
        ///              {Down_  Eval( CommandFile.GetErrorLineNumber()   - 1) }
        ///              {Right_ Eval( CommandFile.GetErrorColumnNumber() - 1) };</code>
        /// When a command file contains errors you can view the Vocola statement for the first error using this command.
        /// It uses <see cref="GetErrorFilename"/>, <see cref="GetErrorLineNumber"/>, and
        /// <see cref="GetErrorColumnNumber"/> to retrieve the file name, line number, and column number of
        /// the error. It opens the file, moves to the top,
        /// and constructs keystrokes to move down to the appropriate line and right to the appropriate column.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public string GetErrorFilename()
        {
            if (VocolaApi.FirstErrorInLastFile == null)
                throw new VocolaExtensionException("No errors have yet occurred");
            else
                return VocolaApi.FirstErrorInLastFile.Filename;
        }

        /// <summary>Gets the line number in the file returned by <see cref="GetErrorFilename"/> of that file's first
        /// error.</summary>
        /// <returns>The line number in the file returned by <see cref="GetErrorFilename"/> of that file's first error.</returns>
        /// <remarks>See <see cref="GetErrorFilename"/> for an example.</remarks>
        [VocolaFunction]
        static public int GetErrorLineNumber()
        {
            if (VocolaApi.FirstErrorInLastFile == null)
                throw new VocolaExtensionException("No errors have yet occurred");
            else
                return VocolaApi.FirstErrorInLastFile.LineNumber;
        }

        /// <summary>Gets the column number in the file returned by <see cref="GetErrorFilename"/> of that file's first
        /// error.</summary>
        /// <returns>The column number in the file returned by <see cref="GetErrorFilename"/> of that file's first error.</returns>
        /// <remarks>See <see cref="GetErrorFilename"/> for an example.</remarks>
        [VocolaFunction]
        static public int GetErrorColumnNumber()
        {
            if (VocolaApi.FirstErrorInLastFile == null)
                throw new VocolaExtensionException("No errors have yet occurred");
            else
                return VocolaApi.FirstErrorInLastFile.ColumnNumber;
        }

        // ---------------------------------------------------------------------
        // Open

        /// <summary>Opens a Vocola command file.</summary>
        /// <param name="filename">Name of file to open, relative to Vocola command folder or absolute.</param>
        /// <remarks>The file will be opened using the program associated with the <c>.vcl</c> extension in Windows
        /// Explorer, unless a different program has been specified using <see cref="SetCommandLineForOpen"/>.</remarks>
        [VocolaFunction]
        static public void Open(string filename)
        {
            if (filename == null || filename == "")
                throw new VocolaExtensionException("Open() called with empty file name");
            filename = Path.Combine(VocolaApi.CommandFolder, filename);
            if (!File.Exists(filename))
                throw new VocolaExtensionException("Vocola command file '{0}' does not exist", filename);
            LaunchFile(filename);
        }

        // ---------------------------------------------------------------------
        // OpenCurrent

        /// <summary>Opens the Vocola command file for the currently-active application.</summary>
        /// <remarks>Vocola stores commands for a particular application in files whose name derives from
        /// the name of the application's executable file. For example, <c>winword.exe</c> is the executable file
        /// for Microsoft Word, and <c>winword.vcl</c> contains Vocola commands for Microsoft Word.
        /// <para>If the command file does not exist it will be created automatically.</para>
        /// <para>The file will be opened using the program associated with the <c>.vcl</c> extension in Windows
        /// Explorer, unless a different program has been specified using <see cref="SetCommandLineForOpen"/>.</para>
        /// </remarks>
        /// <example><code title="Edit voice commands">
        /// Edit [Voice] Commands = CommandFile.OpenCurrent();</code>
        /// If Microsoft Word is active, saying "Edit Voice Commands" opens the file <c>winword.vcl</c> from the
        /// Vocola command folder.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void OpenCurrent()
        {
            string app = Win.GetForegroundAppName();
            string filename = app + ".vcl";
            string comment = "Voice commands for " + app;
            OpenCommandFile(filename, comment);
        }

        static private void OpenCommandFile(string filename, string comment)
        {
            filename = Path.Combine(VocolaApi.CommandFolder, filename);
            if (!File.Exists(filename))
                using (StreamWriter sw = new StreamWriter(filename)) 
                    sw.Write("# " + comment + "\n\n");
            LaunchFile(filename);
        }

        // ---------------------------------------------------------------------
        // OpenCurrentForMachine

        /// <summary>Opens the Vocola machine-specific command file for the currently-active application.</summary>
        /// <remarks>Vocola stores commands for a particular computer in files whose names derive from
        /// the name of that computer. For example, <c>winword@venus.vcl</c> contains Vocola commands
        /// for Microsoft Word on the computer named "venus".
        /// <para>If the command file does not exist it will be created automatically.</para>
        /// <para>The file will be opened using the program associated with the <c>.vcl</c> extension in Windows
        /// Explorer, unless a different program has been specified using <see cref="SetCommandLineForOpen"/>.</para>
        /// </remarks>
        /// <example><code title="Edit voice commands for current computer">
        /// Edit Machine [Voice] Commands = CommandFile.OpenCurrentForMachine();</code>
        /// If Microsoft Word is active and the current computer is named "venus", saying "Edit Machine Commands"
        /// opens the file <c>winword@venus.vcl</c> from the Vocola command folder.
        /// </example>
        [VocolaFunction]
        static public void OpenCurrentForMachine()
        {
            string app = Win.GetForegroundAppName();
            string machine = GetMachineName();
            string filename = app + '@' + machine + ".vcl";
            string comment = "Voice commands for " + app + " on " + machine;
            OpenCommandFile(filename, comment);
        }

        static private string GetMachineName()
        {
            return Environment.MachineName;
        } 

        // ---------------------------------------------------------------------
        // OpenCurrentSamples

        /// <summary>Opens the installed file of sample Vocola commands for the currently-active application.</summary>
        /// <remarks>The Vocola installation includes  a folder called <c>CommandSamples</c>, containing sample command files
        /// for common applications. This function opens the sample command file for the currently-active application.
        /// <para>If the command file does not exist,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.</para>
        /// <para>The file will be opened using the program associated with the <c>.vcl</c> extension in Windows
        /// Explorer, unless a different program has been specified using <see cref="SetCommandLineForOpen"/>.</para>
        /// </remarks>  
        /// <example><code title="Edit sample commands">
        /// Edit Sample [Voice] Commands = CommandFile.OpenCurrentSamples();</code>
        /// If Microsoft Word is active, saying "Edit Sample Commands" opens the file <c>winword.vcl</c> from the
        ///  <c>CommandSamples</c> folder in the Vocola installation folder.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void OpenCurrentSamples()
        {
            string app = Win.GetForegroundAppName();
            string filename = app + ".vcl";
            OpenSamplesFile(filename);
        }

        static private void OpenSamplesFile(string filename)
        {
            string pathname = Path.Combine(VocolaApi.CommandSamplesFolder, filename);
            if (File.Exists(pathname))
                LaunchFile(pathname);
            else
                throw new VocolaExtensionException("There is no samples file '{0}'", filename);
        }

        // ---------------------------------------------------------------------
        // OpenGlobal

        /// <summary>Opens the Vocola file containing commands for all applications.</summary>
        /// <remarks>Opens the file <c>_global.vcl</c>, containing commands which apply to all applications.
        /// <para>If the command file does not exist it will be created automatically.</para>
        /// <para>The file will be opened using the program associated with the <c>.vcl</c> extension in Windows
        /// Explorer, unless a different program has been specified using <see cref="SetCommandLineForOpen"/>.</para>
        /// </remarks>
        /// <example><code title="Edit global commands">
        /// Edit Global [Voice] Commands = CommandFile.OpenGlobal();</code>
        /// Saying "Edit Global Commands" opens the file <c>_global.vcl</c> from the Vocola command folder.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void OpenGlobal()
        {
            string filename = "_global.vcl"  ;
            string comment = "Global voice commands";
            OpenCommandFile(filename, comment);
        }

        // ---------------------------------------------------------------------
        // OpenGlobalForMachine

        /// <summary>Opens the Vocola machine-specific file containing commands for all applications.</summary>
        /// <remarks>Vocola stores commands for a particular computer in files whose names derive from
        /// the name of that computer. For example, <c>_global@venus.vcl</c> contains global commands
        /// for the computer named "venus".
        /// <para>If the command file does not exist it will be created automatically.</para>
        /// <para>The file will be opened using the program associated with the <c>.vcl</c> extension in Windows
        /// Explorer, unless a different program has been specified using <see cref="SetCommandLineForOpen"/>.</para>
        /// </remarks>
        /// <example><code title="Edit global commands for current computer">
        /// Edit Global Machine [Voice] Commands = CommandFile.OpenGlobalForMachine();</code>
        /// If the current computer is named "venus", saying "Edit Global Machine Commands"
        /// opens the file <c>_global@venus.vcl</c> from the Vocola command folder.
        /// </example>
        [VocolaFunction]
        static public void OpenGlobalForMachine()
        {
            string machine = GetMachineName();
            string filename = "_global@" + machine + ".vcl";
            string comment = "Global voice commands on " + machine;
            OpenCommandFile(filename, comment);
        }

        // ---------------------------------------------------------------------
        // OpenGlobalSamples

        /// <summary>Opens the installed file of sample Vocola commands which apply to all applications.</summary>
        /// <remarks>The Vocola installation includes  a folder called <c>CommandSamples</c>, containing sample command files.
        /// This function opens the sample command file <c>_global.vcl</c>, containing sample commands which apply to all
        /// applications.
        /// <para>If the command file does not exist,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.</para>
        /// <para>The file will be opened using the program associated with the <c>.vcl</c> extension in Windows
        /// Explorer, unless a different program has been specified using <see cref="SetCommandLineForOpen"/>.</para>
        /// </remarks>
        /// <example><code title="Edit global sample commands">
        /// Edit Global Sample [Voice] Commands = CommandFile.OpenGlobalSamples();</code>
        /// Saying "Edit Global Sample Commands" opens the file <c>_global.vcl</c> from the
        ///  <c>CommandSamples</c> folder in the Vocola installation folder.
        /// <para>This is one of Vocola's built-in commands.</para>
        /// </example>
        [VocolaFunction]
        static public void OpenGlobalSamples()
        {
            string filename = "_global.vcl"  ;
            OpenSamplesFile(filename);
        }

        // ---------------------------------------------------------------------
        // SetCommandLineForOpen

        static string FilenameForOpenCommand = null;
        static string ArgumentsForOpenCommand = null;

        /// <summary>Specifies the command line to be used for opening command files.</summary>
        /// <param name="executableFilename">Name of executable file to run when opening command files.</param>
        /// <param name="arguments">Arguments to command line used for opening command files.</param>
        /// <remarks>This function may be used to specify the program and command line arguments which
        /// functions like <see cref="Open"/> will use for opening command files.
        /// </remarks>
        /// <example><code title="Using Emacs to open command files">
        /// onLoad() := CommandFile.SetCommandLineForOpen(emacsclientw.exe, -n);</code>
        /// The program <c>emacsclientw.exe</c> may be used externally to open files in a running instance
        /// of the editor Emacs. The command line argument <c>-n</c> specifies that the calling process should exit once the file
        /// has been opened rather than waiting for a signal that editing is complete.
        /// <para> Here <see cref="SetCommandLineForOpen"/> is called not by a voice command but by defining an
        /// <c>onLoad</c> function. Putting this code in <c>_global.vcl</c> will cause <c>emacsclientw.exe -n</c> to be
        /// used for opening command files without further action on your part.</para>
        /// </example>
        [VocolaFunction]
        static public void SetCommandLineForOpen(string executableFilename, string arguments)
        {
            FilenameForOpenCommand  = executableFilename;
            ArgumentsForOpenCommand = arguments;
        }

        static private void LaunchFile(string filename)
        {
            Process process = new Process();
            if (FilenameForOpenCommand == null)
                process.StartInfo.FileName = filename;
            else
            {
                process.StartInfo.FileName = FilenameForOpenCommand;
                process.StartInfo.Arguments = System.String.Format("{0} \"{1}\"", ArgumentsForOpenCommand, filename);
            }
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw new VocolaExtensionException("Can't open command file: {0}\r\n(Command line: {1} {2}", ex.Message,
                    process.StartInfo.FileName, process.StartInfo.Arguments);
            }
        }
        
    }
}
