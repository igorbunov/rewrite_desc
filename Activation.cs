using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rewrite
{
    public partial class Activation : Form
    {
        public Activation()
        {
            InitializeComponent();
        }

        internal void SetValues(string curProgramCode, int curProgramRunCount)
        {
            if (curProgramRunCount == -1)
            {
                curProgramRunCount = 0;
            }

            txtProgramCode.Text = curProgramCode;
            lblCount.Text = "У вас осталось " + curProgramRunCount.ToString() + " запусков";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtProgramCode.Focus();
            txtProgramCode.SelectAll();
            txtProgramCode.Copy();
        }

        private void txtProgramCode_Click(object sender, EventArgs e)
        {
            txtProgramCode.Focus();
            txtProgramCode.SelectAll();
        }
    }
}
