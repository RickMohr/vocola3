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

    public partial class OptionsWindow : Form
    {
        private static OptionsWindow TheWindow = null;
		private PersistWindowState WindowStatePersistor;

        public OptionsWindow()
        {
            InitializeComponent();
            dgvBuiltinCommands.AutoGenerateColumns = false;
			WindowStatePersistor = new PersistWindowState();
			WindowStatePersistor.Parent = this;
            WindowStatePersistor.RegistryPath = Options.RegistryKeyName + @"\OptionsPanel"; // in HKEY_CURRENT_USER
        }

        private void Options_Load(object sender, EventArgs e)
        {
            txtCommandFolderPath.Text   = Options.CommandFolder;
            txtExtensionFolderPath.Text = Options.ExtensionFolder;

            radUsingVocola3.Checked = (Options.BaseUsingSetCode == BaseUsingSetOption.Vocola3);
            radUsingVocola2.Checked = (Options.BaseUsingSetCode == BaseUsingSetOption.Vocola2);
            radUsingCustom .Checked = (Options.BaseUsingSetCode == BaseUsingSetOption.Custom);
            txtCustomUsingSet.Text = Options.CustomBaseUsingSet;
            txtCustomUsingSet.Enabled = (Options.BaseUsingSetCode == BaseUsingSetOption.Custom);

            radRecognizerWsr.Checked = (Options.TheRecognizerType == RecognizerType.Wsr);
            radRecognizerDns.Checked = (Options.TheRecognizerType == RecognizerType.Dns);

            chkDisableWsrDictationScratchpad.Checked = OptionsSapi.DisableWsrDictationScratchpad;
            chkRequireControlNamePrefix.Checked = OptionsSapi.RequireControlNamePrefix;
            chkCommandSequencesWsr.Checked = OptionsSapi.CommandSequencesEnabled;
            numMaxSequencedCommandsWsr.Text = OptionsSapi.MaxSequencedCommands.ToString();  
            numMaxSequencedCommandsWsr.Enabled = chkCommandSequencesWsr.Checked;

            txtNatLinkFolderPath.Text = OptionsNatLink.NatLinkInstallFolder;
            chkCommandSequencesDns.Checked = OptionsNatLink.CommandSequencesEnabled;
            numMaxSequencedCommandsDns.Text = OptionsNatLink.MaxSequencedCommands.ToString();
            numMaxSequencedCommandsDns.Enabled = chkCommandSequencesDns.Checked;

            LoadBuiltinCommandsGrid();
        }

        private void LoadBuiltinCommandsGrid()
        {
            bool vocolaDictationEnabled = (radRecognizerWsr.Checked && chkDisableWsrDictationScratchpad.Checked);
            dgvBuiltinCommands.DataSource = BuiltinCommandGroup.GetGroups(vocolaDictationEnabled);
        }

        public static void ShowWindow()
        {
            if (TheWindow == null)
            {
                TheWindow = new OptionsWindow();
                TheWindow.Show();
            }
            TheWindow.Activate();
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            var textField = (
                sender == btnBrowseCommandFolder ? txtCommandFolderPath :
                sender == btnBrowseExtensionFolder ? txtExtensionFolderPath :
                sender == btnBrowseNatLinkFolder ? txtNatLinkFolderPath :
                null);
            string folderName = RunFolderBrowserDialog(textField.Text);
            if (folderName != null)
                textField.Text = folderName;
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

        private void radRecognizer_CheckedChanged(object sender, EventArgs e)
        {
            bool isWsr = (sender == radRecognizerWsr);
            pnlRecognizerWsr.Enabled = isWsr;
            pnlRecognizerDns.Enabled = !isWsr;
            LoadBuiltinCommandsGrid();
        }

        private void chkDisableWsrDictationScratchpad_CheckedChanged(object sender, EventArgs e)
        {
            LoadBuiltinCommandsGrid();
        }

        private void radUsing_CheckedChanged(object sender, EventArgs e)
        {
            txtCustomUsingSet.Enabled = (radUsingCustom.Checked);
        }

        private void chkCommandSequencesWsr_CheckedChanged(object sender, EventArgs e)
        {
            numMaxSequencedCommandsWsr.Enabled = chkCommandSequencesWsr.Checked;
        }

        private void chkCommandSequencesDns_CheckedChanged(object sender, EventArgs e)
        {
            numMaxSequencedCommandsDns.Enabled = chkCommandSequencesDns.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateData())
            {
                bool vocolaShouldRestart = ShouldVocolaRestart();
                if (vocolaShouldRestart)
                {
                    if (DialogResult.Cancel == MessageBox.Show("Vocola will now restart to change the speech recognizer.", "Vocola will restart", MessageBoxButtons.OKCancel))
                        return;
                }
                StoreData();
                if (vocolaShouldRestart)
                    Vocola.Restart();
                Close();
            }
        }

        private bool ValidateData()
        {
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
                    return false;
                }
            }
            if (!Directory.Exists(txtCommandFolderPath.Text))
            {
                MessageBox.Show(String.Format("Command folder not found:\r\n{0}", txtCommandFolderPath.Text));
                return false;
            }
            if (!Directory.Exists(txtExtensionFolderPath.Text))
            {
                MessageBox.Show(String.Format("Extension folder not found:\r\n{0}", txtExtensionFolderPath.Text));
                return false;
            }
            if (radRecognizerDns.Checked && !Directory.Exists(txtNatLinkFolderPath.Text))
            {
                MessageBox.Show(String.Format("NatLink installation folder not found:\r\n{0}", txtNatLinkFolderPath.Text));
                return false;
            }
            return true;
        }

        private bool ShouldVocolaRestart()
        {
            if (radRecognizerWsr.Checked)
            {
                if (chkDisableWsrDictationScratchpad.Checked != OptionsSapi.DisableWsrDictationScratchpad)
                {
                    if (chkDisableWsrDictationScratchpad.Checked)
                        MessageBox.Show("When you restart Windows Speech Recognition, Vocola dictation will be enabled and the WSR dictation scratchpad will be disabled.", "Please Restart WSR");
                    else
                        MessageBox.Show("When you restart Windows Speech Recognition, Vocola dictation will be disabled and the WSR dictation scratchpad will be enabled.", "Please Restart WSR");
                }
                return (Options.TheRecognizerType != RecognizerType.Wsr);
            }
            else // radRecognizerDns.Checked
                return (Options.TheRecognizerType != RecognizerType.Dns);
        }

        private void StoreData()
        {
            // If user changed command sequence parameters or enabled built-in commands,
            // commands will be updated automatically by the context change away from this dialog box

            // Invalidate commands if base using set changed
            BaseUsingSetOption newBaseUsingSetCode =
                radUsingVocola3.Checked ? BaseUsingSetOption.Vocola3 :
                radUsingVocola2.Checked ? BaseUsingSetOption.Vocola2 :
                BaseUsingSetOption.Custom;
            if (newBaseUsingSetCode != Options.BaseUsingSetCode || txtCustomUsingSet.Text != Options.CustomBaseUsingSet)
            {
                Vocola.InitializeBaseUsingSet(newBaseUsingSetCode, txtCustomUsingSet.Text);
                LoadedFile.InvalidateAll();
            }

            // Store values in member variables
            Options.CommandFolder = txtCommandFolderPath.Text;
            Options.ExtensionFolder = txtExtensionFolderPath.Text;
            Options.BaseUsingSetCode = newBaseUsingSetCode;
            Options.CustomBaseUsingSet = txtCustomUsingSet.Text;
            Options.TheRecognizerType = (radRecognizerWsr.Checked ? RecognizerType.Wsr : RecognizerType.Dns);

            OptionsSapi.DisableWsrDictationScratchpad = chkDisableWsrDictationScratchpad.Checked;
            OptionsSapi.RequireControlNamePrefix = chkRequireControlNamePrefix.Checked;
            OptionsSapi.CommandSequencesEnabled = chkCommandSequencesWsr.Checked;
            OptionsSapi.MaxSequencedCommands = (int)numMaxSequencedCommandsWsr.Value;

            OptionsNatLink.NatLinkInstallFolder = txtNatLinkFolderPath.Text;
            OptionsNatLink.CommandSequencesEnabled = chkCommandSequencesDns.Checked;
            OptionsNatLink.MaxSequencedCommands = (int)numMaxSequencedCommandsDns.Value;
            
            // Persist option values
            Options.Save();
            BuiltinCommandGroup.UpdateAndSave();
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
