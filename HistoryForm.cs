using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rewrite
{
    public partial class HistoryForm : Form
    {
        public List<History> history = new List<History>();

        public HistoryForm()
        {
            InitializeComponent();
        }


        internal void LoadHistory(List<History> h)
        {
            history = h;

            foreach (History item in history)
            {
                historyList.Items.Add(item.Title);                
            }
        }

        public string GetHistoryTextByTitle(string title)
        {
            foreach (History item in history)
            {
                if (item.Title == title)
                {
                    return item.Text;
                }
            }

            return null;
        }

        private void historyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            historyRichTextBox.Text = GetHistoryTextByTitle(historyList.SelectedItem.ToString());
        }
    }
}
