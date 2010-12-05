using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics; // Process
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Vocola
{
    public partial class TrayIcon : Form
    {
        NotifyIcon SystrayIcon;

        public TrayIcon()
        {
            InitializeComponent();

            Visible = false;

            SystrayIcon = new NotifyIcon(components);
            System.ComponentModel.ComponentResourceManager resources = 
                new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            SystrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            SystrayIcon.Text = "Vocola " + Vocola.Version;
            SystrayIcon.ContextMenu = CreateContextMenu();
            //SystrayIcon.DoubleClick += new System.EventHandler(Icon_DoubleClick);
            SystrayIcon.Visible = true;

            // Force creation of the window handle
            IntPtr handle = this.Handle;

            //Win.RegisterHotKey(this, "Ctrl+Shift+F11");
        }

        private ContextMenu CreateContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem;
            int menuItemIndex = 0;

            menuItem = new MenuItem("&Options...");
            menuItem.Click += new System.EventHandler(Options_Click);
            menuItem.Index = menuItemIndex++;
            contextMenu.MenuItems.Add(menuItem);

            menuItem = new MenuItem("&Log Window...");
            menuItem.Click += new System.EventHandler(LogWindow_Click);
            menuItem.Index = menuItemIndex++;
            contextMenu.MenuItems.Add(menuItem);

            menuItem = new MenuItem("&Dictation Shortcuts...");
            menuItem.Click += new System.EventHandler(DictationShortcuts_Click);
            menuItem.Index = menuItemIndex++;
            contextMenu.MenuItems.Add(menuItem);

            menuItem = new MenuItem("&Function Library Documentation...");
            menuItem.Click += new System.EventHandler(FunctionLibrary_Click);
            menuItem.Index = menuItemIndex++;
            contextMenu.MenuItems.Add(menuItem);

            menuItem = new MenuItem("&About Vocola...");
            menuItem.Click += new System.EventHandler(About_Click);
            menuItem.Index = menuItemIndex++;
            contextMenu.MenuItems.Add(menuItem);

            menuItem = new MenuItem("E&xit");
            menuItem.Click += new System.EventHandler(Exit_Click);
            menuItem.Index = menuItemIndex++;
            contextMenu.MenuItems.Add(menuItem);

            return contextMenu;
        }

        private const Int32 WM_HOTKEY       = 0x0312;
        private const Int32 WM_DEVICECHANGE = 0x0219;

        private static DateTime DeviceChangeTime = DateTime.Now;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
            case WM_HOTKEY:
                ShowVocolaMenu();
                break;
            }
            base.WndProc(ref m);
        }

        // ---------------------------------------------------------------------
        // Event Handlers

        private void LogWindow_Click(object Sender, EventArgs e)
        {
            ShowLogWindow();
        }

        private void Options_Click(object Sender, EventArgs e)
        {
            ShowOptionsDialog();
        }

        private void DictationShortcuts_Click(object Sender, EventArgs e)
        {
            ShowDictationShortcutsDialog();
        }

        private void FunctionLibrary_Click(object Sender, EventArgs e)
        {
            ShowFunctionLibraryDocumentation();
        }

        private void About_Click(object Sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void Exit_Click(object Sender, EventArgs e)
        {
            Exit();
        }

        // ---------------------------------------------------------------------
        // Menu options, also invokable via API

        public void ShowVocolaMenu()
        {
            // Show context menu (hack from http://www.csharphelp.com/board2/read.html?f=1&i=44906&t=44906&v=f)
            // (hack needed because ContextMenu.Show needs a Control and there's none around)
            Type type = SystrayIcon.GetType();
            type.InvokeMember("ShowContextMenu",
                              BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                              null,
                              SystrayIcon,
                              null);
        }

        public void ShowLogWindow()
        {
            LogWindow.ShowWindow(true);
        }

        public void ShowOptionsDialog()
        {
            Options.ShowWindow();
        }

        public void ShowDictationShortcutsDialog()
        {
            DictationShortcuts.ShowWindow();
        }

        public void ShowFunctionLibraryDocumentation()
        {
            LaunchFile(Path.Combine(Vocola.FunctionLibraryFolder, @"Documentation\VocolaFunctionLibrary.chm"));
        }

        private void LaunchFile(string filename)
        {
            Process process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = filename;
            process.Start();
        }

        public void Exit()
        {
            SystrayIcon.Visible = false;
            Vocola.Stop();
        }

    }
}
