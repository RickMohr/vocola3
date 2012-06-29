using Microsoft.Win32; // Registry
using PerCederberg.Grammatica.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Vocola
{

    public class Vocola
    {
        static public string Version = "3.2";
        //static public Recognizer TheRecognizer = new RecognizerSapi();
        static public Recognizer TheRecognizer = new RecognizerNatLink();
        static private WindowsHooks TheWindowsHooks;
        static public TrayIcon TrayIcon;
        static public CommandSet SpokenControlNameCommandSet;

        // Important folders, derivable from startup path
        static public string CommandSamplesFolder;
        static public string CommandBuiltinsFolder;
        static public string FunctionLibraryFolder;
        static public string AppDataFolder;

        // Global parameters (stored in registry)
        static public string RegistryKeyName = @"Software\Vocola";
        static public string RegistryKeyNameMicrosoft = @"Software\Microsoft\Speech\Preferences";
        static public string CommandFolder;
        static public string ExtensionFolder;
        static public BaseUsingSetOption BaseUsingSetCode;
        static public string CustomBaseUsingSet;
        static public List<string> BaseUsingSet;
        static public bool CommandSequencesEnabled;
        static public int MaxSequencedCommands;
        static public bool RequireControlNamePrefix;
        static public bool DisableWsrDictationScratchpad;
        static public int AutomationObjectGetterPort;

        [STAThread]
        static public void Main()
        {
            try
            {
                Thread.CurrentThread.Name = "Main UI Thread";
                ReadRegistry();
                LogWindow.Create();
                SetFolderNames();
                CreateDefaultUserFoldersIfNotYetChosen();
                VocolaExtension.VocolaApi = new VocolaApi();
                VocolaExtension.VocolaDictation = new VocolaDictation();
                Extensions.Load();
                InitializeBaseUsingSet(BaseUsingSetCode, CustomBaseUsingSet);
                Keystrokes.Initialize();
                BuiltinCommandGroup.Initialize();
                LoadInternalCommands();
                TrayIcon = new TrayIcon();
                TheRecognizer.Initialize();
                LaunchGrammarUpdateThread();
                WatchCommandFolder();
				TheWindowsHooks = new WindowsHooks();
                Application.Run();
                // See TrayIcon.cs for Exit()
            }
            catch (Exception ex)
            {
                string message = String.Format("{0}\r\n({1})\r\n{2}",
                                               ex.Message, ex.GetType(), ex.StackTrace);
                MessageBox.Show(message, "Vocola Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static public void Stop()
        {
			if (TheWindowsHooks != null)
				TheWindowsHooks.Stop();
            AutomationObjectGetter.Cleanup();
            LogWindow.Destroy();
            System.Environment.Exit(0);
        }

        static private void SetFolderNames()
        {
            string vocolaInstallFolder = Path.GetFullPath(Path.Combine(Application.StartupPath, @".."));
            bool runningDevelopmentVersion = Application.StartupPath.Contains(@"Users");
            if (runningDevelopmentVersion && TheRecognizer.RunDevelopmentVersionFromProgramFiles)
            {
                if (Win.Is64Bit())
                    vocolaInstallFolder = @"C:\Program Files (x86)\Vocola " + Version;
                else
                    vocolaInstallFolder = @"C:\Program Files\Vocola " + Version;
            }
            if (!runningDevelopmentVersion || TheRecognizer.RunDevelopmentVersionFromProgramFiles)
            {
                // Running from Program Files folder
                FunctionLibraryFolder = Path.Combine(vocolaInstallFolder, "FunctionLibrary");
                CommandBuiltinsFolder = Path.Combine(vocolaInstallFolder, @"Commands\Builtins");
                CommandSamplesFolder = Path.Combine(vocolaInstallFolder, @"Commands\Samples");
            }
            else
            {
                // Running from source tree
                FunctionLibraryFolder = Path.Combine(vocolaInstallFolder, @"..\..\..\Extensions\Library\bin\Debug");
                CommandBuiltinsFolder = Path.Combine(vocolaInstallFolder, @"..\..\..\..\Commands\Builtins");
                CommandSamplesFolder = Path.Combine(vocolaInstallFolder, @"..\..\..\..\Commands\Samples");
            }
            AppDataFolder = Application.LocalUserAppDataPath;
            AppDataFolder = AppDataFolder.Substring(0, AppDataFolder.IndexOf("Vocola") + 6);
        }

        static private void ReadRegistry()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKeyName);
            Trace.LevelThreshold = (LogLevel)(int)key.GetValue("LogLevel", (int)LogLevel.Error);//Low
            Trace.ShowTimings = ((int)key.GetValue("ShowTimingInLogMessages", 0)) > 0;
            Trace.ShouldLogToFile = ((int)key.GetValue("ShouldLogToFile", 0)) > 0;//1
            CommandFolder = (string)key.GetValue("CommandFolderPath", null);
            ExtensionFolder = (string)key.GetValue("ExtensionFolderPath", null);
            BaseUsingSetCode = (BaseUsingSetOption)(int)key.GetValue("BaseUsingSetCode", (int)BaseUsingSetOption.Vocola3);
            CustomBaseUsingSet = ((string)key.GetValue("CustomBaseUsingSet", "")).Replace(" ", "");
            CommandSequencesEnabled = ((int)key.GetValue("UseCommandSequences", 0)) > 0;
            MaxSequencedCommands = (int)key.GetValue("MaxSequencedCommands", 6);
            RequireControlNamePrefix = ((int)key.GetValue("RequireControlNamePrefix", 0)) > 0;
            AutomationObjectGetterPort = (int)key.GetValue("AutomationObjectGetterPort", 1649);

            RegistryKey msKey = Registry.CurrentUser.CreateSubKey(RegistryKeyNameMicrosoft);
            DisableWsrDictationScratchpad = ((int)msKey.GetValue("EnableDictationScratchpad", 0)) == 2;
        }

        static private void CreateDefaultUserFoldersIfNotYetChosen()
        {
            RegistryKey mainRegistryKey = Registry.CurrentUser.CreateSubKey(RegistryKeyName);
            string defaultVocolaUserFolder = Path.Combine(Win.GetCurrentUserDocumentsFolder(), "Vocola3");
            if (CommandFolder == null)
            {
                // Folder for user commands hasn't been chosen yet
                string defaultCommandsFolder = Path.Combine(defaultVocolaUserFolder, "Commands");
                if (!Directory.Exists(defaultCommandsFolder))
                {
                    // Default command folder (Users/<currentUser>/Vocola3/Commands) doesn't exist; try to create it
                    try
                    {
                        Directory.CreateDirectory(defaultCommandsFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Failed to create Vocola command folder '{0}':\r\n{1}",
                                                      defaultCommandsFolder, ex.Message));
                        return;
                    }
                }
                // Folder for user commands will be the default folder
                CommandFolder = defaultCommandsFolder;
                mainRegistryKey.SetValue("CommandFolderPath", defaultCommandsFolder);
            }
            if (ExtensionFolder == null)
            {
                // Folder for user extensions hasn't been chosen yet
                string defaultExtensionsFolder = Path.Combine(defaultVocolaUserFolder, "Extensions");
                if (!Directory.Exists(defaultExtensionsFolder))
                {
                    // Default extensions folder (Users/<currentUser>/Vocola3/Extensions) doesn't exist; try to create it
                    try
                    {
                        Directory.CreateDirectory(defaultExtensionsFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Failed to create Vocola extension folder '{0}':\r\n{1}",
                                                      defaultExtensionsFolder, ex.Message));
                        return;
                    }
                }
                // Folder for user extensions will be the default folder
                ExtensionFolder = defaultExtensionsFolder;
                mainRegistryKey.SetValue("ExtensionFolderPath", defaultExtensionsFolder);
            }
        }

        static public void InitializeBaseUsingSet(BaseUsingSetOption option, string customBaseUsingSet)
        {
            BaseUsingSet = new List<string>();
            string usingNames = (option == BaseUsingSetOption.Vocola3 ? "Library,Library.Main,Library.Pointer" :
                                 option == BaseUsingSetOption.Vocola2 ? "Library,Library.DragonLegacy" :
                                 customBaseUsingSet);
            if (usingNames != "")
                foreach (string name in usingNames.Split(','))
                    if (Extensions.ClassOrNamespaceExists(name))
                        BaseUsingSet.Add(name);
                    else
                        Trace.WriteLine(LogLevel.Error, "Base $using set reference '{0}' not found (see Options dialog)", name);
        }

        static private void LoadInternalCommands()
        {
            Trace.WriteLine(LogLevel.Low, "Loading internal commands");
            string commandFileText = "<_itemInWindow> = Library.Main.HearCommand('Insert $1');";
            VocolaParser parser = new VocolaParser(new StringReader(commandFileText));
            Node parseTree = parser.Parse();
            SpokenControlNameCommandSet = new CommandSet(null, null);
            SpokenControlNameCommandSet.AddReservedVariables();
            SpokenControlNameCommandSet.ParseFile(parseTree, "");
            SpokenControlNameCommandSet.Bind();
        }

        // ---------------------------------------------------------------------
        // Watch for changes in command files

        private static void WatchCommandFolder()
        {
            if (Directory.Exists(CommandFolder))
            {
                CreateWatcher("*.vcl");
                CreateWatcher("*.vch");
            }
        }

        private static void CreateWatcher(string filter)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = CommandFolder;
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = filter;
            watcher.Changed += new FileSystemEventHandler(OnCommandFileChanged);
            watcher.Deleted += new FileSystemEventHandler(OnCommandFileDeleted);
            watcher.Renamed += new RenamedEventHandler(OnCommandFileRenamed);
            watcher.EnableRaisingEvents = true;
        }

        static private void OnCommandFileChanged(object source, FileSystemEventArgs e)
        {
            Trace.WriteLine(LogLevel.Low, "Command file '{0}' changed", e.Name);
            GrammarUpdateWaitHandle.Set();
        }

        static private void OnCommandFileDeleted(object source, FileSystemEventArgs e)
        {
            Trace.WriteLine(LogLevel.Low, "Command file '{0}' deleted", e.Name);
            LoadedFile.LoadedFiles.Remove(Path.Combine(CommandFolder, e.Name));
            GrammarUpdateWaitHandle.Set();
        }

        static private void OnCommandFileRenamed(object source, RenamedEventArgs e)
        {
            Trace.WriteLine(LogLevel.Low, "Command file '{0}' renamed to '{1}'", e.OldName, e.Name);
            GrammarUpdateWaitHandle.Set();
        }

        // ---------------------------------------------------------------------
        // Receive and handle context change notifications

        static private EventWaitHandle GrammarUpdateWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        static private VocolaContext CurrentContext = new VocolaContext();
        static private VocolaContext GrammarContext = new VocolaContext();

        static public void UpdateGrammarsIfContextChanged()
        {
            // CurrentContext is the computer's context, updated only here.
            VocolaContext context = new VocolaContext();
            if (context.Differs(CurrentContext))
            {
                CurrentContext = context;
                if (context.ApplicationDiffers(CurrentContext))
                    Dictation.Clear();
                GrammarUpdateWaitHandle.Set();
            }
        }

        // Make a separate thread to serialize grammar updates from context changes and command file edits.
        // That's because context change notifications arrive asynchronously,
        // interrupting the main UI thread (possibly while it's handling the previous context change).

        static private void LaunchGrammarUpdateThread()
        {
            Thread thread = new Thread(HandleGrammarUpdates);
            thread.Name = "Grammar Update Thread";
            thread.Start();
        }

        static private void HandleGrammarUpdates()
        {
            while (true)
            {
                try
                {
                    // GrammarContext is the context used to build the current grammar, updated only here.
                    VocolaContext context = new VocolaContext();
                    bool contextChanged = context.Differs(GrammarContext);
                    bool commandsChanged = UpdateCommands(context);
                    if (contextChanged || commandsChanged)
                    {
                        //Trace.InitializeTimer();
                        //Trace.WriteSeparator();
                        //if (contextChanged) Trace.WriteLine(LogLevel.Low, "Context changed ({0})", context);
                        //if (commandsChanged) Trace.WriteLine(LogLevel.Low, "Commands changed");
                        TheRecognizer.ContextChanged(context);
                        GrammarContext = context;
                    }
                    GrammarUpdateWaitHandle.WaitOne();
                }
                catch (Exception ex)
                {
                    // Anything caught here is unexpected.
                    // Problems with commands should be completely handled lower down.
                    Trace.LogUnexpectedException(ex);
                }
            }
        }

        // ---------------------------------------------------------------------
        // Load any changed command files

        static private bool UpdateCommands(VocolaContext context)
        {
            bool changed = UpdateCommands(CommandFolder, context.AppName);
            changed = UpdateCommands(CommandBuiltinsFolder, context.AppName) || changed;
            return changed;
        }

        static private bool UpdateCommands(string commandFolder, string appName)
        {
            if (!Directory.Exists(commandFolder))
            {
                Trace.WriteLine(LogLevel.Error, "Command folder not found: '{0}'", commandFolder);
                return false;
            }

            // Look for new/changed files:
            //   global commands:            _*.vcl
            //   app-specific commands:      app.vcl, app_*.vcl
            //   machine-specific versions:  ...@machine.vcl
            bool changed = false;
            bool isBuiltinsFolder = (commandFolder == CommandBuiltinsFolder);
            foreach (string pathname in Directory.GetFiles(commandFolder))
            {
                if (pathname.EndsWith(".vcl"))
                {
                    string[] filenameParts = Path.GetFileNameWithoutExtension(pathname).ToLower().Split('@');
                    if (filenameParts.Length == 1
                        || (filenameParts.Length == 2 && filenameParts[1].ToLower() == Environment.MachineName.ToLower()))
                    {
                        string f = filenameParts[0];
                        bool isAppFile = (appName != null
                                          && (f == appName                        // app.vcl, app@machine.vcl
                                              || f.StartsWith(appName + "_")));   // app_*.vcl, app_*@machine.vcl
                        bool isGlobalFile = f.StartsWith("_");                    // _*.vcl, _*@machine.vcl
                        if (isAppFile || isGlobalFile)
                        {
                            LoadedFile loadedFile = LoadedFile.Get(pathname, true/*isTopLevel*/, isBuiltinsFolder);
                            changed = loadedFile.LoadCommandsIfNecessary(isAppFile ? appName : null) || changed;
                            if (changed)
                                TheRecognizer.CommandFileChanged(loadedFile);
                        }
                    }
                }
            }
            return changed;
        }

        static private string versionString = null;

        static public string VersionString
        {
            get
            {
                if (versionString == null)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    versionString = versionInfo.ProductVersion;
                }
                return versionString;
            }
        }

    }


    // ---------------------------------------------------------------------
    // Represents a loaded Vocola command file. CommandSet contains most of the action

    public class LoadedFile
    {
        public string Pathname;
        public string Filename;
        public bool IsBuiltinsFile;
        public DateTime TimeLoaded;
        public CommandSet CommandSet;
        public List<string> IncludedFiles;
        public List<VocolaErrorInfo> Errors;

        static public Dictionary<string, LoadedFile> LoadedFiles = new Dictionary<string, LoadedFile>();

        // Get LoadedFile instance for given filename.
        static public LoadedFile Get(string pathname, bool isTopLevel, bool isBuiltinsFile)
        {
            if (!LoadedFiles.ContainsKey(pathname))
            {
                // Unknown filename. Construct (but don't load yet).
                LoadedFile lf = new LoadedFile();
                lf.Pathname = pathname;
                lf.Filename = Path.GetFileName(pathname);
                lf.IsBuiltinsFile = isBuiltinsFile;
                lf.TimeLoaded = new DateTime(0); // will force load
                LoadedFiles[pathname] = lf;
            }
            return LoadedFiles[pathname];
        }

        public bool LoadCommandsIfNecessary(string appName)
        {
            if (FilesUpToDate())
                return false;
            Trace.WriteSeparator();
            if (IsBuiltinsFile)
                Trace.WriteLine(LogLevel.Low, "Loading built-in commands from {0}", Filename);
            else
                Trace.WriteLine(LogLevel.Medium, "Loading {0}", Filename);
            bool loaded = LoadCommands(appName);
            if (!loaded)
                Trace.WriteLine(LogLevel.Error, "{0}: No commands loaded.", Filename);
            // Update load time even if we didn't load anything. No sense trying again until a file is updated.
            TimeLoaded = DateTime.Now;
            return loaded;
        }

        private bool FilesUpToDate()
        {
            if (File.GetLastWriteTime(Pathname) >= TimeLoaded)
                return false;
            foreach (string pathname in IncludedFiles)
                if (File.GetLastWriteTime(pathname) >= TimeLoaded)
                    return false;
            return true;
        }

        private bool LoadCommands(string appName)
        {
            // Parse file and create command set
            Trace.InitializeForNewFileBeingLoaded();
            IncludedFiles = new List<string>();
            MyVocolaAnalyzer analyzer = new MyVocolaAnalyzer();
            Node parseTree = null;
            VocolaParser parser = CreateParserForFile(Pathname, analyzer);
            if (parser == null)
                return false;
            try
            {
                parseTree = parser.Parse();
            }
            catch (Exception e)
            {
                bool fileEmpty = HandleParseException(Filename, analyzer, e);
                if (fileEmpty)
                {
                    TimeLoaded = DateTime.Now;
                    CommandSet = null;  // clear any existing commands
                }
                return fileEmpty;
            }
            if (parseTree.GetChildCount() > 0)
            {
                CommandSet commandSet = new CommandSet(this, appName);
                try
                {
                    commandSet.AddReservedVariables();
                    commandSet.ParseFile(parseTree, Filename);
                    commandSet.Bind();
                }
                catch (Exception ex)
                {
                    // Anything caught here is unexpected.
                    // Parser exceptions were handled above, and aborted processing of the file.
                    // Language exceptions are handled lower down and not thrown.
                    Trace.LogUnexpectedException(ex);
                    return false;
                }
                if (commandSet.HasLanguageException)
                    // Leave existing commands enabled
                    return false;
                commandSet.RunLoadFunction();
                CommandSet = commandSet;
            }
            return true;
        }

        internal VocolaParser CreateParserForFile(string pathname, MyVocolaAnalyzer analyzer)
        {
            VocolaParser parser = null;
            int nTries = 3;
            while (parser == null)
            {
                try
                {
                    parser = new VocolaParser(new StreamReader(pathname), analyzer);
                }
                catch (IOException e)
                {
                    if (nTries-- > 0)
                        Thread.Sleep(100); // Wait for (e.g.) "File Save" operation to complete
                    else
                        Trace.WriteLine(LogLevel.Error, "Exception loading '{0}': {1}", Pathname, e.Message);
                }
            }
            return parser;
        }

        static private Regex ParseErrorRegex = new Regex(@"(.*), on line: (\d+) column: (\d+)", RegexOptions.Compiled);

        static internal bool HandleParseException(string filename, MyVocolaAnalyzer analyzer, Exception e)
        {
            if (e.Message.StartsWith("unexpected end of file"))
            {
                // Distinguish different "unexpected end of file" conditions
                if (analyzer.NothingParsed)
                {
                    // Empty file -- not an error
                    Trace.WriteLine(LogLevel.Medium, "{0}: File contains no Vocola statements", filename);
                    return true;
                }
                else if (analyzer.DanglingIf != null)
                {
                    Trace.LogException(new FileLineColumn(filename, analyzer.DanglingIf), "$if unmatched by $end");
                    return false;
                }
            }
            for (Match match = ParseErrorRegex.Match(e.Message); match.Success; match = match.NextMatch())
            {
                string message = match.Groups[1].Value;
                string line = match.Groups[2].Value;
                string column = match.Groups[3].Value;
                Trace.LogException(new FileLineColumn(filename, line, column), message);
            }
            return false;
        }

        static public void InvalidateAll()
        {
            Trace.WriteSeparator();
            Trace.WriteLine(LogLevel.Medium, "Marking all command files for reloading");
            foreach (LoadedFile loadedFile in LoadedFiles.Values)
                loadedFile.TimeLoaded = new DateTime(0);
        }

        static public void ClearCachedRules()
        {
            foreach (LoadedFile loadedFile in LoadedFiles.Values)
                if (loadedFile.CommandSet != null)
                    loadedFile.CommandSet.ClearCachedRules();
        }

        public bool ShouldActivateCommands()
        {
            bool isDisabledBuiltinsFile = (IsBuiltinsFile && BuiltinCommandGroup.IsDisabled(Filename));
            bool hasCommands = (CommandSet != null && CommandSet.HasCommands);
            return (hasCommands && !isDisabledBuiltinsFile);
        }

    }

    // ---------------------------------------------------------------------

    public class VocolaContext
    {
        public string AppName = null;
        public string WindowTitle = null;

        public VocolaContext()
        {
            AppName = Win.GetForegroundAppName();
            WindowTitle = Win.GetForegroundWindowTitle();
        }

        public bool Differs(VocolaContext other)
        {
            bool differs = (AppName != other.AppName || WindowTitle != other.WindowTitle);
            return differs;
        }

        public bool ApplicationDiffers(VocolaContext other)
        {
            bool differs = (AppName != other.AppName);
            return differs;
        }

        public override string ToString()
        {
            return String.Format("{0}, '{1}'", AppName, WindowTitle);
        }

    }

    public class InternalException : Exception
    {
        public InternalException(string message, params object[] arguments)
            : base(String.Format(message, arguments)) { }
    }

    public class ActionException : Exception
    {
        public LanguageObject LanguageObject;

        public ActionException(LanguageObject languageObject, string message, params object[] arguments)
            : base(String.Format(message, arguments))
        {
            LanguageObject = languageObject;
        }
    }

    public class ExceptionWrapper : Exception
    {
        public LanguageObject LanguageObject;
        public Exception WrappedException;

        public ExceptionWrapper(Exception wrappedException, LanguageObject languageObject)
            : base("Vocola exception wrapper")
        {
            LanguageObject = languageObject;
            WrappedException = wrappedException;
        }
    }

}
