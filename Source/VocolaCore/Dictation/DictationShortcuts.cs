using SpeechLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Vocola
{
    public partial class DictationShortcuts : Form
    {
        private static DictationShortcuts TheWindow = null;

        private SortableBindingList<ShortcutPair> ShortcutPairs;
        private SpShortcut SPShortcut = new SpShortcut();
		private PersistWindowState WindowStatePersistor;

        public DictationShortcuts()
        {
            InitializeComponent();
            ShortcutPairs = new SortableBindingList<ShortcutPair>();
			WindowStatePersistor = new PersistWindowState();
			WindowStatePersistor.Parent = this;
			WindowStatePersistor.RegistryPath = Vocola.RegistryKeyName + @"\DictationShortcutsWindow"; // in HKEY_CURRENT_USER
        }

        private class ShortcutPair
        {
            private string spokenForm;
            private string writtenForm;
            
            public string SpokenForm  { get { return spokenForm;  } set { spokenForm  = value; } }
            public string WrittenForm { get { return writtenForm; } set { writtenForm = value; } }

            public ShortcutPair()
            {
                this.spokenForm = "";
                this.writtenForm = "";
            }

            public ShortcutPair(string spokenForm, string writtenForm)
            {
                this.spokenForm = spokenForm;
                this.writtenForm = writtenForm;
            }

            public bool IsBlank()
            {
                return (spokenForm == "" && writtenForm == "");
            }

            public bool IsTheSame(ShortcutPair other)
            {
                return (other.SpokenForm == spokenForm && other.WrittenForm == writtenForm);
            }
        }

        private void DictationShortcuts_Load(object sender, EventArgs e)
        {
            SPSHORTCUTPAIRLIST pairList = new SPSHORTCUTPAIRLIST();
            SPShortcut.GetShortcuts(1033, ref pairList);
            IntPtr pairPointer = pairList.pFirstShortcutPair;
            while (pairPointer != IntPtr.Zero)
            {
                SPSHORTCUTPAIR pair = (SPSHORTCUTPAIR)System.Runtime.InteropServices.Marshal.
                    PtrToStructure(pairPointer, typeof(SPSHORTCUTPAIR));
                ShortcutPairs.Add(new ShortcutPair(pair.pszSpoken, pair.pszDisplay));
                pairPointer = pair.pNextSHORTCUTPAIR;
            }
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = ShortcutPairs;
            bindingSource.Sort = "SpokenForm ASC";
            bindingSource.AllowNew = true;
            dataGridView1.DataSource = bindingSource;
        }

        public static void ShowWindow()
        {
            if (TheWindow == null)
            {
                TheWindow = new DictationShortcuts();
                TheWindow.Show();
            }
            TheWindow.Activate();
        }

        ShortcutPair CurrentPairOldValue;

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            ShortcutPair pair = ShortcutPairs[e.RowIndex];
            CurrentPairOldValue = new ShortcutPair(pair.SpokenForm, pair.WrittenForm);
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            ShortcutPair pair = ShortcutPairs[e.RowIndex];
            if (pair.IsTheSame(CurrentPairOldValue))
                return;
            try
            {
                SPShortcut.AddShortcut(pair.WrittenForm, 1033, pair.SpokenForm, SPSHORTCUTTYPE.SPSHT_OTHER);
                if (CurrentPairOldValue.SpokenForm != "")
                    RemoveShortcut(CurrentPairOldValue);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.High, "Failure adding shortcut: {0}", ex.Message);
                ShortcutPairs[e.RowIndex] = CurrentPairOldValue;
                e.Cancel = true;
            }

        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            ShortcutPair pair = ShortcutPairs[e.Row.Index];
            if (pair.IsBlank())
                return;
            bool success = RemoveShortcut(ShortcutPairs[e.Row.Index]);
            e.Cancel = !success;
        }

        private bool RemoveShortcut(ShortcutPair pair)
        {
            try
            {
                SPShortcut.RemoveShortcut(pair.WrittenForm, 1033, pair.SpokenForm, SPSHORTCUTTYPE.SPSHT_OTHER);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.High, "Failure removing shortcut: {0}", ex.Message);
                return false;
            }
        }

/*
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            ShortcutPair pair = ShortcutPairs[e.RowIndex];
            CurrentPairOldValue = new ShortcutPair(pair.SpokenForm, pair.WrittenForm);
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ShortcutPair currentPairNewValue = ShortcutPairs[e.RowIndex];
            RemoveShortcut(CurrentPairOldValue);
            AddShortcut(currentPairNewValue);
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            bool success = AddShortcut(ShortcutPairs[e.Row.Index - 1]);
            e.Cancel = !success;
        }

        private bool AddShortcut(ShortcutPair pair)
        {
            try
            {
                SPShortcut.AddShortcut(pair.WrittenForm, 1033, pair.SpokenForm, SPSHORTCUTTYPE.SPSHT_OTHER);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(LogLevel.Error, "Failure adding shortcut: {0}", ex.Message);
                return false;
            }
        }
*/

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            TheWindow = null;
        }

    }
}
