using System;
using System.Collections.Generic;
using System.Diagnostics; // Process
using System.IO;
using System.Runtime.InteropServices; // DllImport
using System.Text;
using System.Threading;
using System.Windows.Forms;  // Form

namespace Vocola
{

    public class Win
    {

        static public ushort GetCurrentLanguageID()
        {
            return GetUserDefaultLangID();
        }

        static public string GetCurrentUserDocumentsFolder()
        {
            // See http://pinvoke.net/default.aspx/shell32/SHGetKnownFolderPath.html
            IntPtr pPath;
            string folderPathname = "";
            Guid Documents = new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7");
            if (SHGetKnownFolderPath(Documents, 0, IntPtr.Zero, out pPath ) == 0)
            {
                folderPathname = System.Runtime.InteropServices.Marshal.PtrToStringUni(pPath);
                System.Runtime.InteropServices.Marshal.FreeCoTaskMem(pPath);
            }
            return folderPathname;
        }

        static public string GetForegroundAppName()
        {
            try
            {
                IntPtr hWnd = GetForegroundWindowHandle();
                if (hWnd == IntPtr.Zero)
                    return null;
                Process process = Process.GetProcessById(GetWindowProcessID(hWnd));
                int nChars = 1024;
                StringBuilder filename = new StringBuilder(nChars);
                //GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, nChars);
                QueryFullProcessImageName(process.Handle, false, filename, ref nChars);
                return Path.GetFileNameWithoutExtension(filename.ToString()).ToLower();
            }
            catch
            {
                return null;
            }
        }
            
/*
        static public string GetForegroundAppName()
        {
            try
            {
                IntPtr hWnd = GetForegroundWindowHandle();
                Process process = Process.GetProcessById(GetWindowProcessID(hWnd));
                try
                {
                    return Path.GetFileNameWithoutExtension(process.MainModule.FileName).ToLower();
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // This appears to be a Windows bug. Trying to access Process.MainModule for a Windows Explorer process
                    // gives the error "Only part of a ReadProcessMemory or WriteProcessMemory request was completed".
                    return "explorer";
                }
            }
            catch
            {
                return null;
            }
        }
*/
        
        static private Int32 GetWindowProcessID(IntPtr hWnd)
        {
            int pid = 1;
            GetWindowThreadProcessId((int)hWnd, out pid);
            return pid;
        }

        static public IntPtr GetForegroundWindowHandle()
        {
            return (IntPtr)GetForegroundWindow();
        }

        public static bool Is64Bit() 
        { 
            bool retVal; 
            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal); 
            return retVal; 
        }

        static public bool SetForegroundWindowHandle(IntPtr hWnd)
        {
            int tries = 5;
            while (tries-- > 0)
            {
                if (SetForegroundWindow(hWnd))
                    return true;
                Thread.Sleep(100);
            }
            return false;
        }

        static public string GetForegroundWindowTitle()
        {
            return GetWindowTitle(GetForegroundWindowHandle());
        }

        static public string GetWindowTitle(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        static public void SendKeyEvent(byte vk, bool isKeyDownEvent)
        {
            keybd_event(vk, 0, isKeyDownEvent ? 0 : KEYEVENTF_KEYUP, 0);
        }

		static public void SendKeyEvent(byte vk, string keyText, bool isKeyDownEvent)
		{
			uint scanCode = MapVirtualKeyEx(vk, 0/*MAPVK_VK_TO_VSC*/, InputLanguage.InstalledInputLanguages[0].Handle);
			if (isKeyDownEvent)
				Trace.WriteLine(LogLevel.Low, "    Sending '{0}', virtual key {1}, scan code {2}", keyText, vk, scanCode);
			keybd_event(vk, (byte)scanCode, isKeyDownEvent ? 0 : KEYEVENTF_KEYUP, 0);
		}

		static public bool GetVirtualKey(char c, out byte virtualKey, out bool shift, out bool ctrl, out bool alt)
		{
			virtualKey = 0;
			shift = ctrl = alt = false;
			if (c == 10)
			{
				// For some reason VkKeyScanEx returns ctrl+chr(13); we don't want ctrl
				virtualKey = 13;
				return true;
			}
			short result = VkKeyScanEx(c, InputLanguage.InstalledInputLanguages[0].Handle); // YET: active language?
			if (result == -1)
				return false;
			else
			{
				virtualKey = (byte)(result & 0xFF);
				byte modifierKeys = (byte)(result >> 8);
				shift = ((modifierKeys & 1) > 0);
				ctrl = ((modifierKeys & 2) > 0);
				alt = ((modifierKeys & 4) > 0);
				return true;
			}
		}

        static public void SendButtonEvent(byte vk, bool isButtonDownEvent)
        {
            if (isButtonDownEvent)
            {
                switch (vk)
                {
                case VK_LBUTTON: mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0); break;
                case VK_RBUTTON: mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0); break;
                case VK_MBUTTON: mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0); break;
                }
            }
            else
            {
                switch (vk)
                {
                case VK_LBUTTON: mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0); break;
                case VK_RBUTTON: mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0); break;
                case VK_MBUTTON: mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0); break;
                }
            }
        }

        static public void MoveMouseWheel(bool down)
        {
            int amount = (down ? -120 : 120);  // http://pinvoke.net/default.aspx/user32/mouse_event.html
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)amount, 0);
        }

        /*
        static public void RegisterHotKey(Form form, string keyText)
        {
            Keystroke keystroke = Keystrokes.GetComplexKeystroke(keyText);
            byte vk = Keystrokes.GetVirtualKeyCode(keystroke.KeyName);
            int modifiers = 0;
            if (keystroke.Alternate) modifiers += MOD_ALT;
            if (keystroke.Control  ) modifiers += MOD_CONTROL;
            if (keystroke.Shift    ) modifiers += MOD_SHIFT;
            if (keystroke.Windows  ) modifiers += MOD_WIN;
            Win.RegisterHotKey(form.Handle, form.GetType().GetHashCode(), modifiers, (int)vk);            
        }
        */

        // ---------------------------------------------------------------------

        const byte VK_LBUTTON = 0x01;
        const byte VK_RBUTTON = 0x02;
        const byte VK_MBUTTON = 0x04;

        const int MOD_ALT = 0x1;
        const int MOD_CONTROL = 0x2;
        const int MOD_SHIFT = 0x4;
        const int MOD_WIN = 0x8;

        const uint KEYEVENTF_KEYUP       = 0x2;

        const uint MOUSEEVENTF_LEFTDOWN   = 0x0002;
        const uint MOUSEEVENTF_LEFTUP     = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN  = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP    = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP   = 0x0040;
        const uint MOUSEEVENTF_WHEEL      = 0x0800;

        [DllImport("user32.dll")]                                           static extern int    GetForegroundWindow();
        [DllImport("kernel32.dll")]                                         static extern ushort GetUserDefaultLangID();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]  static extern int    GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError=true)]                        static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);
        [DllImport("kernel32.dll")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool   IsWow64Process(IntPtr hProcess, out bool lpSystemInfo); 
        [DllImport("user32.dll")]                                           static extern void   keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        [DllImport("user32.dll")]                                           static extern void   mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [DllImport("kernel32.dll")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool   QueryFullProcessImageName(IntPtr ProcessHandle, [MarshalAs(UnmanagedType.Bool)] bool UseNativeName, StringBuilder ExeName, ref int Size);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)]   static extern bool   RegisterHotKey(IntPtr hWnd, int id,int fsModifiers,int vlc);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)]   static extern bool   SetForegroundWindow(IntPtr hWnd);
        [DllImport("shell32.dll")]                                          static extern int    SHGetKnownFolderPath( [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath );

		[DllImport("user32.dll")]
		static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

		[DllImport("user32.dll")]
		static extern short VkKeyScanEx(char ch, IntPtr dwhkl);
     
    }

}
