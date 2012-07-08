using Microsoft.Win32; // Registry
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vocola
{
    public partial class LogWindow : Form
    {
        private static LogWindow TheLogWindow = null;
        private static bool ReallyClosing = false;
		private PersistWindowState WindowStatePersistor;

        private LogWindow()
        {
            InitializeComponent();
            ddlLogLevel.SelectedIndex = (int)Trace.LevelThreshold;
        }

        public static void Create()
        {
            if (TheLogWindow == null)
                TheLogWindow = new LogWindow();
            TheLogWindow.Visible = false;
            // Force creation of the window handle. This must be done from the main i/o thread.
            // We do it on app startup because we can't otherwise count on finding the i/o thread.
            IntPtr handle = TheLogWindow.Handle;
            InitializeHtml();
        }

        public static void Destroy()
        {
            ReallyClosing = true;
            TheLogWindow.Close();
            TheLogWindow.TheLogBox.Dispose();
            TheLogWindow.Dispose();
        }

        public static void InitializeHtml()
        {
            TheLogWindow.TheLogBox.DocumentText =
                @"<html>
                    <head>
                      <style type='text/css'>
                        div {
                          font-family: Arial, Sans-Serif;
                          font-size: 8pt;
                        }
                      </style>
                    </head>
                    <body style='background: #f0f0f0; border: 1px solid black'></body>
                  </html>";
        }

        public static void ShowWindow(bool activate)
        {
            if (TheLogWindow != null)
            {
                TheLogWindow.WindowStatePersistor = new PersistWindowState();
                TheLogWindow.WindowStatePersistor.Parent = TheLogWindow;
                TheLogWindow.WindowStatePersistor.RegistryPath = Options.RegistryKeyName + @"\LogWindow"; // in HKEY_CURRENT_USER
                TheLogWindow.BeginInvoke((MethodInvoker) delegate()
                {
                    if (activate || !TheLogWindow.Visible)
                    {
                        TheLogWindow.Visible = true;
                        if (TheLogWindow.WindowState == FormWindowState.Minimized)
                            TheLogWindow.WindowState = FormWindowState.Normal;
                        TheLogWindow.Activate();
                    }
                });
            }
        }

        public static void AppendLine(string text, bool important)
        {
            // Append using TheLogWindow's thread, and don't wait for it to finish
            TheLogWindow.BeginInvoke((MethodInvoker) delegate()
            {
                try
                {
                    // Use HTML because RichTextBox caused intermittent hangs
                    HtmlDocument doc = TheLogWindow.TheLogBox.Document;
                    HtmlElement line = doc.CreateElement("div");
                    line.InnerText = text;
                    if (important)
                        line.Style = "color: red;";
                    doc.Body.AppendChild(line);
                    if (TheLogWindow.chkAutoScroll.Checked)
                        line.ScrollIntoView(true);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception writing to log window: " + e.Message);
                }
            });
        }

        private void ddlLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Trace.LevelThreshold = (LogLevel)ddlLogLevel.SelectedIndex;
            Options.SaveLogLevel(Trace.LevelThreshold);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            InitializeHtml();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ReallyClosing)
            {
                // Hide the window instead of closing it
                TheLogWindow.Visible = false;
                e.Cancel = true;
            }
        }

    }
}
