using System;
using System.IO;
using System.Diagnostics;
using PerCederberg.Grammatica.Parser;
using System.Threading;

namespace Vocola
{

    public class Trace
    {
        static public LogLevel LevelThreshold = LogLevel.High;
        static public bool ShowTimings = false;
        static public bool ShouldLogToFile = true;
        static private DateTime StartTime = DateTime.Now;
        static public VocolaErrorInfo FirstErrorInLastFile = null;

        static public void InitializeTimer()
        {
            StartTime = DateTime.Now;
        }

        // ---------------------------------------------------------------------

        static public void WriteLine(LogLevel level, string message, params object[] arguments)
        {
            WriteLine(level, String.Format(message, arguments));
        }

        static public void WriteLine(LogLevel level, string message)
        {
            if (level <= LevelThreshold)
            {
                if (ShowTimings)
                {
                    double elapsedSeconds = (DateTime.Now - StartTime).TotalSeconds;
                    message = String.Format("{0:f1}: {1}", elapsedSeconds, message);
                }
                Debug.WriteLine(message);
                if (level == LogLevel.Error)
                    LogWindow.ShowWindow(false);
                LogWindow.AppendLine(message, level == LogLevel.Error);
                if (ShouldLogToFile)
                    LogToFile(message);
            }
        }

        private static void LogToFile(string message)
        {
            int nTries = 3;
            while (true)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(Options.CommandFolder, @"..\Log.txt"), true /*append*/))
                        sw.WriteLine("{0}  {1}", DateTime.Now, message);
                    return;
                }
                catch (IOException)
                {
                    if (nTries-- <= 0)
                        return;
                    Thread.Sleep(100);
                }
            }
        }

        static public void WriteSeparator()
        {
            if (LevelThreshold == LogLevel.Medium || LevelThreshold == LogLevel.Low)
            {
                Debug.WriteLine("-----------------------------------------");
                LogWindow.AppendLine("-----------------------------------------", false);
            }
        }

        // ---------------------------------------------------------------------

        static public void LogExecutionException(Exception ex)
        {
            LogExecutionException(ex, (FileLineColumn)null);
        }

        static public void LogExecutionException(Exception ex, LanguageObject lo)
        {
            LogExecutionException(ex, new FileLineColumn(lo));
        }

        static public void LogExecutionException(Exception ex, FileLineColumn flc)
        {
            if (ex is ExceptionWrapper)
            {
                ExceptionWrapper exw = ex as ExceptionWrapper;
                ex  = exw.WrappedException;
                flc = new FileLineColumn(exw.LanguageObject);
            }
            if (ex is ActionException)
            {
                ActionException aex = ex as ActionException;
                if (aex.LanguageObject != null)
                    flc = new FileLineColumn(aex.LanguageObject);
                LogException(flc, ex.Message);
            }
            else if (ex is VocolaExtensionException)
            {
                LogException((ex as VocolaExtensionException).LogLevel, flc, ex.Message);
            }
            else if (ex is InternalException)
            {
                LogException(flc, "Internal Vocola Exception: {0}", ex.Message);
            }
            else
            {
                LogUnexpectedException(ex);
            }
        }

        static public void LogUnexpectedException(Exception ex)
        {
            WriteLine(LogLevel.Error, "Unexpected Exception: {0}", ex.Message);
            WriteLine(LogLevel.Error, "({0})", ex.GetType());
            WriteLine(LogLevel.Error, ex.StackTrace);
        }

        // ---------------------------------------------------------------------

        static public void LogException(LanguageObject lo, string message, params object[] arguments)
        {
            LogException(new FileLineColumn(lo), String.Format(message, arguments));
        }

        static public void LogException(FileLineColumn flc, string message, params object[] arguments)
        {
            LogException(flc, String.Format(message, arguments));
        }

        static public void LogException(FileLineColumn flc, string message)
        {
            WriteLine(LogLevel.Error, FormatExceptionMessage(flc, message));
            UpdateErrorInfo(flc, message);
        }

        static public void LogException(LogLevel level, FileLineColumn flc, string message)
        {
            WriteLine(level, FormatExceptionMessage(flc, message));
            UpdateErrorInfo(flc, message);
        }

        static private string FormatExceptionMessage(FileLineColumn flc, string message)
        {
            if (flc == null)
                return message;
            else
                return String.Format("{0} ({1},{2}): {3}", flc.Filename, flc.Line, flc.Column, message);
        }

        static private bool FileBeingLoadedHasError = false;

        static public void InitializeForNewFileBeingLoaded()
        {
            FileBeingLoadedHasError = false;
        }

        static private void UpdateErrorInfo(FileLineColumn flc, string message)
        {
            if (flc != null && !FileBeingLoadedHasError)
            {
                FileBeingLoadedHasError = true;
                FirstErrorInLastFile = new VocolaErrorInfo();
                FirstErrorInLastFile.Filename = flc.Filename;
                FirstErrorInLastFile.Message = message;
                try
                {
                    FirstErrorInLastFile.LineNumber   = Int32.Parse(flc.Line);
                    FirstErrorInLastFile.ColumnNumber = Int32.Parse(flc.Column);
                }
                catch (FormatException) {}
            }
        }

    }

    // ---------------------------------------------------------------------

    public class FileLineColumn
    {
        public string Filename;
        public string Line;
        public string Column;

        public FileLineColumn(string filename, string line, string column)
        {
            Filename = filename;
            Line = line;
            Column = column;
        }

        public FileLineColumn(string filename, Node node)
        {
            Filename = filename;
            Line   = node.GetStartLine().ToString();
            Column = node.GetStartColumn().ToString();
        }

        public FileLineColumn(LanguageObject lo)
        {
            Filename = lo.SourceFilename;
            Line   = lo.ParserNode.GetStartLine().ToString();
            Column = lo.ParserNode.GetStartColumn().ToString();
        }
        
    }

}
