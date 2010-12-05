using Microsoft.Win32; // Registry
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Vocola
{

    public enum BaseUsingSetOption
    {
        Vocola3,
        Vocola2,
        Custom,
    }

    public partial class Options : Form
    {
        private static Options TheWindow = null;
		private PersistWindowState WindowStatePersistor;

        public Options()
        {
            InitializeComponent();
			WindowStatePersistor = new PersistWindowState();
			WindowStatePersistor.Parent = this;
			WindowStatePersistor.RegistryPath = Vocola.RegistryKeyName + @"\OptionsPanel"; // in HKEY_CURRENT_USER
        }

        private void Options_Load(object sender, EventArgs e)
        {
            txtCommandFolderPath.Text   = Vocola.CommandFolder;
            txtExtensionFolderPath.Text = Vocola.ExtensionFolder;
            radUsingVocola3.Checked = (Vocola.BaseUsingSetCode == BaseUsingSetOption.Vocola3);
            radUsingVocola2.Checked = (Vocola.BaseUsingSetCode == BaseUsingSetOption.Vocola2);
            radUsingCustom .Checked = (Vocola.BaseUsingSetCode == BaseUsingSetOption.Custom);
            txtCustomUsingSet.Text = Vocola.CustomBaseUsingSet;
            txtCustomUsingSet.Enabled = (Vocola.BaseUsingSetCode == BaseUsingSetOption.Custom);
            chkCommandSequences.Checked = Vocola.CommandSequencesEnabled; 
            txtMaxSequencedCommands.Text = Vocola.MaxSequencedCommands.ToString();  
            txtMaxSequencedCommands.Enabled = chkCommandSequences.Checked;
            chkRequireControlNamePrefix.Checked = Vocola.RequireControlNamePrefix;
            chkDisableWsrDictationScratchpad.Checked = Vocola.DisableWsrDictationScratchpad;
            dgvBuiltinCommands.DataSource = BuiltinCommandGroup.Groups;
            chkDisableWsrDictationScratchpad.Visible = true;// !Utilities.RunningOnVista();
        }

        public static void ShowWindow()
        {
            if (TheWindow == null)
            {
                TheWindow = new Options();
                TheWindow.Show();
            }
            TheWindow.Activate();
        }

        private void btnBrowseCommandFolder_Click(object sender, EventArgs e)
        {
            string folderName = RunFolderBrowserDialog(txtCommandFolderPath.Text);
            if (folderName != null)
                txtCommandFolderPath.Text = folderName;
        }

        private void btnBrowseExtensionFolder_Click(object sender, EventArgs e)
        {
            string folderName = RunFolderBrowserDialog(txtExtensionFolderPath.Text);
            if (folderName != null)
                txtExtensionFolderPath.Text = folderName;
        }

        private string RunFolderBrowserDialog(string folderName)
        {
            DialogResult dialogResult = DialogResult.None;
            Thread newThread = new Thread(delegate()
            {
                // Vista-style dialog from http://www.codeproject.com/KB/vista/VistaControls.aspx
                using (Vista_Api.FolderBrowserDialog folderBrowserDialog = new Vista_Api.FolderBrowserDialog())
                {
                    folderBrowserDialog.SelectedPath = folderName;
                    dialogResult = folderBrowserDialog.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                        folderName = folderBrowserDialog.SelectedPath;
                }
            });
            newThread.SetApartmentState(ApartmentState.STA);
            newThread.Start();
            newThread.Join();
            if (dialogResult == DialogResult.OK)
                return folderName;
            else
                return null;
        }

        private void radUsing_CheckedChanged(object sender, EventArgs e)
        {
            txtCustomUsingSet.Enabled = (radUsingCustom.Checked);
        }

        private void chkCommandSequences_CheckedChanged(object sender, EventArgs e)
        {
            txtMaxSequencedCommands.Enabled = chkCommandSequences.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validate
            if (radUsingCustom.Checked && txtCustomUsingSet.Text != "")
            {
                List<string> undefinedReferences = new List<string>();
                foreach (string name in txtCustomUsingSet.Text.Replace(" ", "").Split(','))
                    if (!Extensions.ClassOrNamespaceExists(name))
                        undefinedReferences.Add(name);
                if (undefinedReferences.Count > 0)
                {
                    MessageBox.Show(String.Format("Base $using set reference(s) not found:\r\n{0}",
                                                  String.Join("\r\n", (string[])undefinedReferences.ToArray())));
                    return;
                }
            }
            if (!Directory.Exists(txtCommandFolderPath.Text))
            {
                MessageBox.Show(String.Format("Command folder not found:\r\n{0}", txtCommandFolderPath.Text));
                return;
            }
            if (!Directory.Exists(txtExtensionFolderPath.Text))
            {
                MessageBox.Show(String.Format("Extension folder not found:\r\n{0}", txtExtensionFolderPath.Text));
                return;
            }

            // If user changed command sequence parameters or enabled built-in commands,
            // commands will be updated automatically by the context change away from this dialog box

            // Invalidate commands if base using set changed
            BaseUsingSetOption newBaseUsingSetCode = 
                radUsingVocola3.Checked ? BaseUsingSetOption.Vocola3 :
                radUsingVocola2.Checked ? BaseUsingSetOption.Vocola2 :
                BaseUsingSetOption.Custom;
            if (newBaseUsingSetCode != Vocola.BaseUsingSetCode || txtCustomUsingSet.Text != Vocola.CustomBaseUsingSet)
            {
                Vocola.InitializeBaseUsingSet(newBaseUsingSetCode, txtCustomUsingSet.Text);
                LoadedFile.InvalidateAll();
            }

            if (chkDisableWsrDictationScratchpad.Checked != Vocola.DisableWsrDictationScratchpad)
            {
                if (chkDisableWsrDictationScratchpad.Checked)
                    MessageBox.Show("When you restart Windows Speech Recognition, Vocola dictation will be enabled and the WSR dictation scratchpad will be disabled.", "Please Restart WSR");
                else
                    MessageBox.Show("When you restart Windows Speech Recognition, Vocola dictation will be disabled and the WSR dictation scratchpad will be enabled.", "Please Restart WSR");
            }

            // Store values in member variables
            Vocola.CommandFolder   = txtCommandFolderPath.Text;
            Vocola.ExtensionFolder = txtExtensionFolderPath.Text;
            Vocola.BaseUsingSetCode = newBaseUsingSetCode;
            Vocola.CustomBaseUsingSet = txtCustomUsingSet.Text;
            Vocola.CommandSequencesEnabled = chkCommandSequences.Checked; 
            Vocola.MaxSequencedCommands = Int32.Parse(txtMaxSequencedCommands.Text);
            Vocola.RequireControlNamePrefix = chkRequireControlNamePrefix.Checked;
            Vocola.DisableWsrDictationScratchpad = chkDisableWsrDictationScratchpad.Checked;

            // Store values in registry
			RegistryKey key = Registry.CurrentUser.CreateSubKey(Vocola.RegistryKeyName);
			key.SetValue("CommandFolderPath"        , Vocola.CommandFolder);
			key.SetValue("ExtensionFolderPath"      , Vocola.ExtensionFolder);
			key.SetValue("BaseUsingSetCode"         , (int)Vocola.BaseUsingSetCode);
			key.SetValue("CustomBaseUsingSet"       , Vocola.CustomBaseUsingSet);
			key.SetValue("UseCommandSequences"      , Vocola.CommandSequencesEnabled ? 1 : 0);
			key.SetValue("MaxSequencedCommands"     , Vocola.MaxSequencedCommands);
			key.SetValue("RequireControlNamePrefix" , Vocola.RequireControlNamePrefix ? 1 : 0);

            RegistryKey msKey = Registry.CurrentUser.CreateSubKey(Vocola.RegistryKeyNameMicrosoft);
            msKey.SetValue("EnableDictationScratchpad", Vocola.DisableWsrDictationScratchpad ? 2 : 1);

            BuiltinCommandGroup.SaveToRegistry();

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            TheWindow = null;
        }

        private void lnkHelpBuiltinCommands_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://vocola.net/v3/BuiltinCommands.asp");
        }

    }
}
