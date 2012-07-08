using System;
using System.Collections.Generic;
using System.Diagnostics; // Process
using System.IO;
using System.Drawing; // Point
using System.Runtime.InteropServices; // DllImport
using System.Text;
using System.Threading;
using System.Windows.Forms;  // Form
using Vocola;

#pragma warning disable 1591  // Don't complain about missing XML comments

namespace Library
{

    public class Win
    {

        static public void ButtonClick(int buttons, int count)
        {
            while (count-- > 0)
            {
                if ((buttons & 1) > 0) mouse_event(MOUSEEVENTF_LEFTDOWN  , 0, 0, 0, 0);
                if ((buttons & 2) > 0) mouse_event(MOUSEEVENTF_RIGHTDOWN , 0, 0, 0, 0);
                if ((buttons & 4) > 0) mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, 0);
                if ((buttons & 4) > 0) mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, 0);
                if ((buttons & 2) > 0) mouse_event(MOUSEEVENTF_RIGHTUP , 0, 0, 0, 0);
                if ((buttons & 1) > 0) mouse_event(MOUSEEVENTF_LEFTUP  , 0, 0, 0, 0);
            }
            // This seems to be necessary when running from Program Files
            Thread.Sleep(50);
        }

        static public void DragToPoint(Point p1, Point p2)
        {
            Cursor.Position = p1;
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(500);
            Cursor.Position = p2;
            Thread.Sleep(500);
            mouse_event(MOUSEEVENTF_LEFTUP  , 0, 0, 0, 0);
        }

        static public IntPtr FindWindowByClassName(string className)
        {
            return FindWindow(className, "");
        }

        static public void FocusOnTaskBar()
        {
            IntPtr taskbarHwnd = Win.FindWindowByClassName("Shell_traywnd");
            SetForegroundWindow(taskbarHwnd);
        }

        static public string GetForegroundAppName()
        {
            return GetAppName((IntPtr)GetForegroundWindow());
        }

        static public string GetAppName(IntPtr hWnd)
        {
            try
            {
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

        static public Int32 GetWindowProcessID(IntPtr hWnd)
        {
            int pid = 1;
            GetWindowThreadProcessId((int)hWnd, out pid);
            return pid;
        }

        static public string GetForegroundWindowTitle()
        {
            IntPtr hWnd = (IntPtr)GetForegroundWindow();
            // Allocate correct string length first
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        static public Rectangle GetForegroundWindowClientRect()
        {
            IntPtr hWnd = (IntPtr)GetForegroundWindow();
            RECT clientBox = new RECT();
            GetClientRect(hWnd, ref clientBox);
            Point p1 = new Point(clientBox.left,  clientBox.top);
            Point p2 = new Point(clientBox.right, clientBox.bottom);
            ClientToScreen(hWnd, ref p1);
            ClientToScreen(hWnd, ref p2);
            return new Rectangle(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.Y);
        }

        static public Rectangle GetForegroundWindowRect()
        {
            return GetWindowRect((IntPtr)GetForegroundWindow());
        }

        static private Rectangle GetWindowRect(IntPtr hWnd)
        {
            RECT box = new RECT();
            GetWindowRect(hWnd, ref box);
            return CreateRectangleFromRect(box);
        }

        static private Rectangle CreateRectangleFromRect(RECT box)
        {
            return new Rectangle(box.left, box.top, box.right - box.left, box.bottom - box.top);
        }

        static public void MoveForegroundWindowToNextScreen()
        {
            // If window is maximized, restore it
            IntPtr hWnd = (IntPtr)GetForegroundWindow();
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT(); placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            bool maximized = (placement.showCmd == SW_MAXIMIZE);
            if (maximized)
                ShowWindow((uint)hWnd, SW_RESTORE);

            // Get screen containing window origin
            Rectangle windowBox = GetForegroundWindowRect();
            int screenNumber = GetScreenNumberContaining(windowBox.Location);

            // Get position difference of next screen
            Rectangle r1 = Screen.AllScreens[screenNumber].WorkingArea;
            screenNumber++;
            if (screenNumber >= Screen.AllScreens.Length)
                screenNumber = 0;
            Rectangle r2 = Screen.AllScreens[screenNumber].WorkingArea;
            Size delta = new Size(r2.X - r1.X, r2.Y - r1.Y);

            // Move the window
            SetForegroundWindowPosition(windowBox.Location + delta);

            // Re-maximize the window if necessary
            if (maximized)
                ShowWindow((uint)hWnd, SW_SHOWMAXIMIZED);
        }

        static private int GetScreenNumberContaining(Point p)
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            { 
                Screen screen = Screen.AllScreens[i];
                if (screen.Bounds.Contains(p))
                    return i;
            } 
            return 0;
        }

        static public void SetForegroundWindowPosition(Point p)
        {
            IntPtr hWnd = (IntPtr)GetForegroundWindow();
            SetWindowPos(hWnd, (IntPtr)0, p.X, p.Y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        }

        static public void SetForegroundWindowSize(int width, int height)
        {
            IntPtr hWnd = (IntPtr)GetForegroundWindow();
            SetWindowPos(hWnd, (IntPtr)0, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
        }

        static public void ShowWindow(IntPtr hWnd)
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT(); placement.length = Marshal.SizeOf(placement);
            GetWindowPlacement(hWnd, ref placement);
            if (placement.showCmd == SW_SHOWMINIMIZED)
            {
                // Window is minimized, so restore it (maximizing if appropriate)
                int showCmd = (placement.flags == WPF_RESTORETOMAXIMIZED) ? SW_SHOWMAXIMIZED : SW_SHOWNORMAL;
                ShowWindow((uint)hWnd, showCmd);
            }
            SetForegroundWindow(hWnd);
        }


        // ---------------------------------------------------------------------
        
        const byte VK_TAB  = 0x09;
        const byte VK_LWIN = 0x5B;
        
        const uint MOUSEEVENTF_LEFTDOWN   = 0x0002;
        const uint MOUSEEVENTF_LEFTUP     = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN  = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP    = 0x0010;
        const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        const uint MOUSEEVENTF_MIDDLEUP   = 0x0040;
        
        const uint SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const uint SWP_NOZORDER = 0x0004;

        const int SW_HIDE            = 0;
        const int SW_SHOWNORMAL      = 1;
        const int SW_NORMAL          = 1;
        const int SW_SHOWMINIMIZED   = 2;
        const int SW_SHOWMAXIMIZED   = 3;
        const int SW_MAXIMIZE        = 3;
        const int SW_SHOWNOACTIVATE  = 4;
        const int SW_SHOW            = 5;
        const int SW_MINIMIZE        = 6;
        const int SW_SHOWMINNOACTIVE = 7;
        const int SW_SHOWNA          = 8;
        const int SW_RESTORE         = 9;
        const int SW_SHOWDEFAULT     = 10;
        const int SW_FORCEMINIMIZE   = 11;
        const int SW_MAX             = 11;

        const int WPF_RESTORETOMAXIMIZED = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTAPI
        {
            public int x;
            public int y;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public POINTAPI ptMinPosition;
            public POINTAPI ptMaxPosition;
            public RECT rcNormalPosition;
        }

        [DllImport("user32.dll")] static extern bool   ClientToScreen(IntPtr hwnd, ref Point lpPoint);
        [DllImport("user32.dll")] static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")] static extern int    GetClientRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll")] static extern int    GetForegroundWindow();
        [DllImport("user32.dll")] static extern bool   GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")] static extern bool   GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll")] static extern int    GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")] static extern int    GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")] static extern uint   GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);
        [DllImport("user32.dll")] static extern bool   IsWindow(IntPtr hWnd);
        [DllImport("user32.dll")] static extern void   mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
        [DllImport("user32.dll")] static extern bool   SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")] static extern bool   SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);
        [DllImport("user32.dll")] static extern bool   SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] static extern uint   ShowWindow(uint hwnd, int showCommand);

        [DllImport("kernel32.dll")] [return: MarshalAs(UnmanagedType.Bool)] static extern bool  
            QueryFullProcessImageName(IntPtr ProcessHandle, [MarshalAs(UnmanagedType.Bool)] bool UseNativeName, StringBuilder ExeName, ref int Size);
        
    }

}
