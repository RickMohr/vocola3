using ManagedWinapi.Windows;
using ManagedWinapi.Accessibility;
using System;
using System.Collections.Generic;
using Vocola;
using System.Diagnostics;
using System.IO;

namespace Library
{

    /// <summary>Functions for accessing the Windows taskbar.</summary>
    public class TaskBar : VocolaExtension
    {

        // ---------------------------------------------------------------------
        // LaunchButtonNumber

        /// <summary>Launches the application associated with a specified "Quick Launch" button.</summary>
        /// <param name="buttonNumber">Number of the desired "Quick Launch" button.</param>
        /// <example><code title="Launch an application by number">
        /// Launch 1..20 = TaskBar.LaunchButtonNumber($1);</code>
        /// Saying "Launch 3" starts the third item in the taskbar's "Quick Launch" area.
        /// </example>
        [VocolaFunction]
        static public void LaunchButtonNumber(int buttonNumber)
        {
            if (!RunningOnVista())
                throw new VocolaExtensionException("This function only works in Windows Vista");

            SystemAccessibleObject[] buttons = GetQuickLaunchButtons();

            if (buttonNumber > buttons.Length)
                throw new VocolaExtensionException("There are not {0} quick launch items", buttonNumber);
            else if (buttonNumber <= 0)
                throw new VocolaExtensionException("'{0}' is not a valid button number", buttonNumber);

            SystemAccessibleObject theButton = buttons[buttonNumber - 1];
            theButton.DoDefaultAction();
        }

        static private SystemAccessibleObject[] GetQuickLaunchButtons()
        {
            // Find top level system tray window
            SystemWindow trayWindow = new SystemWindow(Win.FindWindowByClassName("Shell_traywnd"));
            // Find descendant window containing quick launch buttons
            SystemWindow buttonsWindow = null;
            foreach (SystemWindow win in trayWindow.AllDescendantWindows)
                if (win.Title == "Quick Launch")
                {
                    buttonsWindow = win;
                    break;
                }
            // Find quick launch buttons
            SystemAccessibleObject buttonsContainer = SystemAccessibleObject.FromWindow(buttonsWindow, AccessibleObjectID.OBJID_WINDOW);
            SystemAccessibleObject[] buttons = null;
            foreach (SystemAccessibleObject obj in buttonsContainer.Children)
                if (obj.Name == "Quick Launch")
                {
                    buttons = obj.Children;
                    break;
                }
            return buttons;
        }

        // ---------------------------------------------------------------------
        // SwitchToButtonNumber

        /// <summary>Activates the window associated with a specified taskbar button.</summary>
        /// <param name="buttonNumber">Number of the desired taskbar button. Positive numbers count forward from the first
        /// taskbar button, while negative numbers count backward from the last taskbar button.</param>
        /// <example><code title="Switch to a window by taskbar number">
        /// Use 1..9 = TaskBar.SwitchToButtonNumber($1);</code>
        /// If Microsoft Word is the third item in the taskbar, saying "Use 3" brings it into the foreground.
        /// <code title="Close a window by taskbar number">
        /// Close 1..9 Right = TaskBar.SwitchToButtonNumber(-$1) {Alt+F4};</code>
        /// If Notepad is the last item in the taskbar, saying "Close 1 Right" brings it into the foreground
        /// by calling <c>SwitchToButtonNumber(-1)</c> and then closes it by sending the keystroke <c>{Alt+F4}</c>.
        /// </example>
        [VocolaFunction]
        static public void SwitchToButtonNumber(int buttonNumber)
        {
            if (!RunningOnVista())
                throw new VocolaExtensionException("This function only works in Windows Vista");

            List<SystemAccessibleObject> visibleButtons = GetRunningApplicationButtons();
            SystemAccessibleObject theButton;

            if (buttonNumber > visibleButtons.Count)
                throw new VocolaExtensionException("There are not {0} items in the taskbar", buttonNumber);
            else if (-buttonNumber > visibleButtons.Count)
                throw new VocolaExtensionException("There are not {0} items in the taskbar", -buttonNumber);
            else if (buttonNumber > 0)
                theButton = visibleButtons[buttonNumber - 1];
            else if (buttonNumber < 0)
                theButton = visibleButtons[visibleButtons.Count + buttonNumber];
            else
                throw new VocolaExtensionException("'0' is not a valid button number");

            SystemWindow[] windows = GetWindowsMatchingButtonTitle(theButton.Name);
            ShowWindowForButton(theButton, windows, 5);
        }

        static private void ShowWindowForButton(SystemAccessibleObject theButton, SystemWindow[] windows, int timeout)
        {
            // Windows has a long-standing bug where clicking one of these buttons sometimes minimizes the window
            // instead of raising it. Workaround is to find a window whose title matches the button title.
            // But if there's more than one such window, just click the button.
            if (windows.Length == 1)
            {
                Win.ShowWindow(windows[0].HWnd);
                Main.WaitForWindow(windows[0].Title, timeout);
            }
            else
                theButton.DoDefaultAction();
        }

        // ---------------------------------------------------------------------
        // SwitchToApplication

        /// <summary>Activates the window associated with the nth taskbar instance of a specified application.</summary>
        /// <param name="applicationName">Name of a running application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <param name="instance">Which instance of specified application to activate, in taskbar order.</param>
        /// <remarks>Counts to instance number <paramref name="instance"/> of <paramref name="applicationName"/> (case insensitive)
        /// in the taskbar, and activates the window associated with that taskbar button. If too few instances are running,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// It also errors out if the desired instance does not become active within 5 seconds.
        /// </remarks>
        /// <example><code title="Switch to a particular taskbar item">
        /// &lt;app> := ( Emacs
        ///          | Notepad
        ///          | Visual Studio  = devenv
        ///          | Browser        = firefox
        ///          | Command Prompt = cmd
        ///          | Mailer         = thunderbird
        ///          );
        /// Use &lt;app> 1..9 = SwitchToApplication($1, $2);</code>
        /// Saying For example "Use Browser 2" makes the call <c>SwitchToApplication(firefox, 2)</c>, which activates
        /// the second instance of Firefox in the taskbar.</example>
        /// <seealso cref="Main.SwitchTo(string)">Main.SwitchTo</seealso>
        /// <seealso cref="SwitchToApplication(string, int, int)">SwitchToApplication(applicationName, instance, timeout)</seealso>
        [VocolaFunction]
        static public void SwitchToApplication(string applicationName, int instance)
        {
            TaskBar.SwitchToApplication(applicationName, instance, 5);
        }

        /// <summary>Activates the window associated with the nth taskbar instance of a specified application, with a specified timeout.</summary>
        /// <param name="applicationName">Name of a running application executable file, not including the <c>.exe</c> extension.
        /// Case insensitive.</param>
        /// <param name="instance">Which instance of specified application to activate, in taskbar order.</param>
        /// <param name="timeout">Number of seconds to wait before aborting.</param>
        /// <remarks>Counts to instance number <paramref name="instance"/> of <paramref name="applicationName"/> (case insensitive)
        /// in the taskbar, and activates the window associated with that taskbar button. If too few instances are running,
        /// this function aborts the calling command and displays an error message in the Vocola Log window.
        /// It also errors out if the desired instance does not become active within <paramref name="timeout"/> seconds.
        /// <para>See <see cref="SwitchToApplication(string,int)">SwitchToApplication</see> for an example.</para>
        /// </remarks>
        /// <seealso cref="SwitchToApplication(string, int)">SwitchToApplication(applicationName, instance)</seealso>
        [VocolaFunction]
        static public void SwitchToApplication(string applicationName, int instance, int timeout)
        {
            applicationName = applicationName.ToLower();
            if (applicationName.EndsWith(".exe"))
                applicationName = applicationName.Substring(0, applicationName.Length - 4);

            if (RunningOnVista())
            {
                // This digs through the taskbar windows. Works great in Vista, but everything changed with Windows 7.
                SwitchToApplicationVista(applicationName, instance, timeout);
                return;
            }

            // This gets the main window of the first process with the right name.
            // If a process has more than one window, too bad.
            // It fails for Explorer and NatSpeak.
            foreach (var process in Process.GetProcesses())
            {
                if (Path.GetFileNameWithoutExtension(process.ProcessName).ToLower() == applicationName)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        Win.ShowWindow(process.MainWindowHandle);
                        Main.WaitForWindow(process.MainWindowTitle, timeout);
                        return;
                    }
                }
            }
            /*
            // This enumerates desktop windows
            // If a process has more than one window it will find them, in Z-order.
            // But you need "taskbar order" to say "Use Explorer 2". I've found no way to do that.
            foreach (var hWnd in Win.GetDesktopWindows())
            {
                string processName = Win.GetAppName(hWnd);
                //var winTitle = Win.GetWindowTitle(hWnd);
                //var className = Win.GetWindowClassName(hWnd);
                //Trace.WriteLine(string.Format("app = '{0}', class = '{1}', title = '{2}'", processName, className, winTitle));
                if (processName != null && processName.ToLower() == applicationName)
                {
                    Win.ShowWindow(hWnd);
                    var title = Win.GetWindowTitle(hWnd);
                    Main.WaitForWindow(title);
                    return;
                }
            }
             */
            throw new VocolaExtensionException("Could not switch to instance {0} of application '{1}'", instance, applicationName);
        }

        static private void SwitchToApplicationVista(string applicationName, int instance, int timeout)
        {
            if (!RunningOnVista())
                throw new VocolaExtensionException("This function only works in Windows Vista");

            int i = 0;
            foreach (SystemAccessibleObject button in GetRunningApplicationButtons())
            {
                SystemWindow[] windows = GetWindowsMatchingButtonTitle(button.Name);
                if (windows.Length > 0)
                {
                    string processName = Win.GetAppName(windows[0].HWnd);
                    //string processName = windows[0].Process.ProcessName.ToLower();
                    VocolaApi.LogMessage(LogLevel.Low, string.Format("Checking '{0}'", processName));
                    if (processName == applicationName && ++i == instance)
                    {
                        ShowWindowForButton(button, windows, timeout);
                        return;
                    }
                }
            }
            throw new VocolaExtensionException("Could not switch to instance {0} of application '{1}'", instance, applicationName);
        }

        // How I figured out the taskbar window/object structure:
        // - Run WinternalExplorer utility from The "Managed Windows API" project (mwinapi.sourceforge.net)
        // - Use "Tools > Window Information" to find hwnd of taskbar
        // - Use "Find > Windows by Handle..." to find that hwnd in the tree view, then explore

        static private List <SystemAccessibleObject> GetRunningApplicationButtons()
        {
            // Find top level system tray window
            SystemWindow trayWindow = new SystemWindow(Win.FindWindowByClassName("Shell_traywnd"));
            // Find descendant window containing running application buttons
            SystemWindow buttonsWindow = null;
            foreach (SystemWindow win in trayWindow.AllDescendantWindows)
                if (win.Title == "Running Applications" && win.AllChildWindows.Length == 0)
                {
                    buttonsWindow = win;
                    break;
                }
            // Find running application buttons
            SystemAccessibleObject buttonsContainer = SystemAccessibleObject.FromWindow(buttonsWindow, AccessibleObjectID.OBJID_WINDOW);
            SystemAccessibleObject[] buttons = null;
            foreach (SystemAccessibleObject obj in buttonsContainer.Children)
                if (obj.Name == "Running Applications")
                {
                    buttons = obj.Children;
                    break;
                }
            // Find *visible* running application buttons
            List <SystemAccessibleObject> visibleButtons = new List <SystemAccessibleObject>();
            foreach (SystemAccessibleObject button in buttons)
                if (button.Visible)
                    visibleButtons.Add(button);
            return visibleButtons;
        }

        static private SystemWindow[] GetWindowsMatchingButtonTitle(string title)
        {
            // Button titles are truncated to 78 characters, so if there are more than 77 characters we only look for
            // window which start with those characters.
            // Button titles omit the first "&" char! So we remove it from window titles when matching.
            if (title.Length < 78)
                return SystemWindow.FilterToplevelWindows(
                    delegate(SystemWindow win)
                    {
                        return (RemoveFirstAmpersand(win.Title) == title);
                    });
            else
                return SystemWindow.FilterToplevelWindows(
                    delegate(SystemWindow win)
                    {
                        return (RemoveFirstAmpersand(win.Title).StartsWith(title));
                    });
        }

        static private string RemoveFirstAmpersand(string title)
        {
            int index = title.IndexOf("&");
            if (index >= 0)
                title = title.Substring(0, index) + title.Substring(index + 1);
            return title;
        }

        static private bool RunningOnVista()
        {
            return (
                Environment.OSVersion.Version.Major == 6 &&
                Environment.OSVersion.Version.Minor == 0);
        }

    }

}
