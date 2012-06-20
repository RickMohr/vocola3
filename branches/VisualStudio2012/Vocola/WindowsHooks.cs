// From http://niels.penneman.org/wordpress/2007/07/05/windows-event-hooks-net

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms; // MessageBox


namespace Vocola
{
    public sealed class WindowsHooks
    {
        private WinEventDelegate dEvent;
        private List<IntPtr> HookPointers = new List<IntPtr>();

        public WindowsHooks()
        {
            dEvent = this.WinEvent;
            SetHook(EVENT_SYSTEM_FOREGROUND);
            SetHook(EVENT_OBJECT_NAMECHANGE);
        }

        private void SetHook(uint eventCode)
        {
            IntPtr pHook = SetWinEventHook(eventCode, eventCode , IntPtr.Zero, dEvent, 0, 0, WINEVENT_OUTOFCONTEXT);
            if (IntPtr.Zero.Equals(pHook))
                throw new Win32Exception();
            HookPointers.Add(pHook);
        }

        private void WinEvent(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject,
                              int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                switch (eventType)
                {
                case EVENT_SYSTEM_FOREGROUND:
                case EVENT_OBJECT_NAMECHANGE:
                    Vocola.UpdateGrammarsIfContextChanged();
                    break;
                }
            }
            catch (Exception ex)
            {
                string message = String.Format("{0}\r\n({1})\r\n{2}",
                                               ex.Message, ex.GetType(), ex.StackTrace);
                MessageBox.Show(message, "Vocola Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Stop()
        {
            for (int i = 0; i < HookPointers.Count; i++)
            {
                if (HookPointers[i] != IntPtr.Zero)
                    UnhookWinEvent(HookPointers[i]);
                HookPointers[i] = IntPtr.Zero;
            }
            dEvent = null;
        }

        ~WindowsHooks()
        {
            Stop();
        }

        #region Windows API

        private const uint EVENT_SYSTEM_FOREGROUND  = 0x0003;
        private const uint EVENT_OBJECT_NAMECHANGE  = 0x800C;
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;

        private delegate void WinEventDelegate(
            IntPtr hWinEventHook,
            uint eventType,
            IntPtr hWnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(
            uint eventMin,
            uint eventMax,
            IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc,
            uint idProcess,
            uint idThread,
            uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(
            IntPtr hWinEventHook
            );

        #endregion
    }
}
