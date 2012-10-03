using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vocola
{
    public partial class Correction : Form
    {
        static private SapiAlternates Alternates;
        private IntPtr DictationWindowHandle;
		private PersistWindowState WindowStatePersistor;

        public Correction(SapiAlternates alternates)
        {
            InitializeComponent();
            Alternates = alternates;
            DictationWindowHandle = Win.GetForegroundWindowHandle();
			WindowStatePersistor = new PersistWindowState();
			WindowStatePersistor.Parent = this;
			WindowStatePersistor.RegistryPath = Options.RegistryKeyName + @"\CorrectionWindow"; // in HKEY_CURRENT_USER

            txtPhrase.Text = Alternates.GetText(0);
            Show();
            Activate();
        }

        private class AlternateItem
        {
            private int index;

            public AlternateItem(int i)
            {
                index = i;
            }

            public string Value   { get { return Alternates.GetText(index); } }
            public string Display { get { return String.Format("{0}:  {1}", index, Value); } }
        }

        private void Correction_Load(object sender, EventArgs e)
        {
            lstChoices.ValueMember = "Value";
            lstChoices.DisplayMember = "Display";
            for (int i = 0; i < Alternates.Count; i++)
            {
                lstChoices.Items.Add(new AlternateItem(i));
                //Trace.WriteLine(LogLevel.Low, "Replacements for alternative {0}:", i);
                //Alternates.LogReplacements(i);
            }
        }

        private void lstChoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPhrase.Text = ((AlternateItem)lstChoices.SelectedItem).Value;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.Visible = false;
                string oldText = Alternates.GetText(0);
                string newText = txtPhrase.Text;
                if (newText != null && newText != oldText)
                {
                    Trace.WriteLine(LogLevel.Low, "Correcting '{0}' with '{1}'", oldText, newText);
                    Alternates.Commit(newText);
                    Trace.WriteLine(LogLevel.Low, "Switching to dictation window '{0}' ({1})",
                        Win.GetWindowTitle(DictationWindowHandle), DictationWindowHandle);

                    if (Win.SetForegroundWindowHandle(DictationWindowHandle))
                    {
                        string backspaces = "{BACKSPACE " + oldText.Length.ToString() + "}";
                        Vocola.KeySender.SendKeys(backspaces);
                        Dictation.Clear();
                        Dictation.HandleDictatedPhrase(newText, null);
                    }
                    else
                        Trace.WriteLine(LogLevel.High, "Could not switch to dictation window: '{0}' ({1})",
                            Win.GetWindowTitle(DictationWindowHandle), DictationWindowHandle);
                }
                Close();
            }
            catch (Exception ex)
            {
                Trace.LogUnexpectedException(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
