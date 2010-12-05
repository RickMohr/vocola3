using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Vcl2to3
{
    class Program
    {

        // Usage: vcl2to3 [-functions] inputFolder outputFolder

        // Converts each .vcl or .vch file in inputFolder and writes the result to
        // outputFolder.

        static bool ShouldConvertFunctionCalls;

        static void Main(string[] args)
        {
            bool argsOk = (args.Length == 2 || (args.Length == 3 && args[0] == "-functions"));
            if (!argsOk)
                Die("Usage: vcl2to3 [-functions] inputFolder outputFolder");
            ShouldConvertFunctionCalls = (args.Length == 3);
            string inputFolder  = args[args.Length - 2];
            string outputFolder = args[args.Length - 1];
            if (!Directory.Exists(inputFolder))
                Die("Could not find input folder '{0}'", inputFolder);
            if (!Directory.Exists(outputFolder))
            {
                try
                {
                    Directory.CreateDirectory(outputFolder);
                    Console.Out.WriteLine("Created output folder '{0}'", outputFolder);
                }
                catch (Exception e)
                {
                    Die("Could not create output folder '{0}':\n{1}", outputFolder, e.Message);
                }
            }
            try 
            {
                foreach (string pathname in Directory.GetFiles(inputFolder))
                    if (pathname.EndsWith(".vcl") || pathname.EndsWith(".vch"))
                        ConvertFile(pathname, outputFolder);
            }
            catch (Exception e) 
            {
                Die("{0}: {1}", e.GetType(), e.Message);
            }
        }

        static void ConvertFile(string inputPath, string outputFolder)
        {
            string filename = Path.GetFileName(inputPath);
            string outputPath = Path.Combine(outputFolder, (filename == "_vocola.vcl" ? "_global.vcl" : filename));
            Console.Out.WriteLine("Converting {0}", filename);
            using (StreamReader sr = new StreamReader(inputPath)) 
                using (StreamWriter sw = new StreamWriter(outputPath)) 
                {
                    bool foundAContextStatement = false;
                    string line;
                    List<string> blankLines = new List<string>();
                    while ((line = sr.ReadLine()) != null) 
                    {
                        bool isBlank;
                        line = ConvertLine(line, out isBlank);
                        if (line.StartsWith("$if"))
                        {
                            if (foundAContextStatement)
                                sw.WriteLine("$end");
                            foundAContextStatement = true;
                        }
                        if (isBlank)
                            blankLines.Add(line);
                        else
                        {
                            foreach (string s in blankLines)
                                sw.WriteLine(s);
                            sw.WriteLine(line);
                            blankLines.Clear();
                        }
                    }
                    if (foundAContextStatement)
                        sw.WriteLine("$end");
                    foreach (string s in blankLines)
                        sw.WriteLine(s);
                }
        }

        static Regex CommentRx      = new Regex(@"^(.*?)(#.*)$", RegexOptions.Compiled);
        static Regex BlankRx        = new Regex(@"^\w*$"       , RegexOptions.Compiled);
        static Regex IncludeRx      = new Regex(@"^include"    , RegexOptions.Compiled);
        static Regex EnvironmentRx  = new Regex(@"\$(\w*)"     , RegexOptions.Compiled);
        static Regex ContextRx      = new Regex(@"^(.*):(\s*)$", RegexOptions.Compiled);

        static string ConvertLine(string line, out bool isBlank)
        {
            // Get trailing comment
            string comment = "";
            Match match = CommentRx.Match(line);
            if (match.Success)
            {
                line    = match.Groups[1].Value;
                comment = match.Groups[2].Value;
            }
            isBlank = BlankRx.IsMatch(line);
 
            if (IncludeRx.IsMatch(line))
            {
                // Found include statement.
                // Replace e.g. "foo_$bar.vcl" with "foo_ EnvironmenVariables.Get(foo) .vcl"
                line = EnvironmentRx.Replace(line, delegate(Match m)
                    {
                        string variable = m.Groups[1].Value;
                        return String.Format(" EnvironmentVariables.Get({0}) ", variable);
                    });
                line = IncludeRx.Replace(line, "$include");
            }
            else if (ContextRx.IsMatch(line))
            {
                // Found context statement
                match = ContextRx.Match(line);
                line = match.Groups[1].Value;
                string[] patterns = line.Split('|');
                for (int i = 0; i < patterns.Length; i++)
                {
                    String leadingSpace  = (patterns[i].StartsWith(" ") ? " " : "");
                    string trailingSpace = (patterns[i].EndsWith(" ") ? " " : "");
                    string p = patterns[i].Trim();
                    if (NeedsQuotes(p) && !p.StartsWith("\"") && !p.StartsWith("'"))
                        patterns[i] = String.Format("{0}\"{1}\"{2}", leadingSpace, p, trailingSpace);
                } 
                line = String.Join("|", patterns); 
                line = String.Format("$if {0};", line);
            }
            else if (ShouldConvertFunctionCalls)
                line = ConvertFunctionCalls(line);
            return line + comment;
        }

        static string[] CharactersRequiringQuotes = new string[] {" ", "'", "=", ";", "[", "]", "(", ")", "<", ">", ","};

        static bool NeedsQuotes(string pattern)
        {
            foreach (string c in CharactersRequiringQuotes)
                if (pattern.Contains(c))
                    return true;
            return false;
        }

        static Regex ControlPickRx      = new Regex(@"ControlPick\((.*?)\)"              , RegexOptions.Compiled);
        static Regex MenuPickRx         = new Regex(@"MenuPick\((.*?)\)"                 , RegexOptions.Compiled);
        static Regex SendKeysRx         = new Regex(@"SendKeys\((.*?)\)"                 , RegexOptions.Compiled);
        static Regex ShellExecuteRx     = new Regex(@"ShellExecute\((.*?),(.*?),(.*?)\)" , RegexOptions.Compiled);
        static Regex WaitForWindowRx    = new Regex(@"WaitForWindow\((.*?),(.*?),(.*?)\)", RegexOptions.Compiled);

        static Regex SetMousePositionRx = new Regex(@"SetMousePosition\((\d),\s*(([^\(\)]|\(.*\))*)\)", RegexOptions.Compiled);

        static string ConvertFunctionCalls(string line)
        {
            line = ControlPickRx.Replace(line, delegate(Match m)
                {
                    return String.Format("HearCommand(\"Click {0}\")", m.Groups[1].Value);
                });
            line = MenuPickRx.Replace(line, delegate(Match m)
                {
                    return String.Format("HearCommand(\"Click {0}\")", m.Groups[1].Value);
                });
            line = SendKeysRx.Replace(line, delegate(Match m)
                {
                    return m.Groups[1].Value;
                });
            line = SetMousePositionRx.Replace(line, delegate(Match m)
                {
                    string arguments = m.Groups[2].Value;
                    switch (m.Groups[1].Value)
                    {
                    case "0": return BuildMoveCall("MoveTo"    , arguments, "ScreenInner");
                    case "1": return BuildMoveCall("MoveTo"    , arguments, null);
                    case "2": return BuildMoveCall("MoveBy"    , arguments, null);
                    case "3": return BuildMoveCall("MoveToEdge", arguments, "ScreenInner");
                    case "4": return BuildMoveCall("MoveToEdge", arguments, null);
                    case "5": return BuildMoveCall("MoveTo"    , arguments, "WindowInner");
                    case "6": return BuildMoveCall("MoveToEdge", arguments, "WindowInner");
                    default: return "";
                    }
                });
            line = ShellExecuteRx.Replace(line, delegate(Match m)
                {
                    return String.Format("RunProgram({0},{1})", m.Groups[1].Value, m.Groups[3].Value);
                });
            line = WaitForWindowRx.Replace(line, delegate(Match m)
                {
                    return String.Format("WaitForWindow({0},{1})", m.Groups[1].Value, m.Groups[3].Value);
                });
            line = line.Replace("AppBringUp("   , "SwitchTo(");
            line = line.Replace("AppSwapWith("  , "SwitchTo(");
            line = line.Replace("ButtonClick("  , "Pointer.Click(");
            line = line.Replace("DragToPoint("  , "Pointer.DragFromSavedPoint(");
            line = line.Replace("GoToSleep("    , "HearCommand(\"Stop Listening\"");
            line = line.Replace("HeardWord("    , "HearCommand(");
            line = line.Replace("RememberPoint(", "Pointer.SavePoint(");
            line = line.Replace("WakeUp("       , "HearCommand(\"Start Listening\"");
            return line;
        }            

        static string BuildMoveCall(string function, string arguments, string rectangle)
        {
            if (rectangle == null)
                return String.Format("{0}({1})", function, arguments);
            else
                return String.Format("{0}({1}, {2})", function, arguments, rectangle);
        }

        static void Die(string message, params object[] arguments)
        {
            Console.Out.WriteLine(String.Format(message, arguments));
            Environment.Exit(1);
        }
            
    }
}
