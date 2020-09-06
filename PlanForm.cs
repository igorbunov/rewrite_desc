using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rewrite
{
    public partial class PlanForm : Form
    {
        public PlanForm()
        {
            InitializeComponent();
        }
        public string GetPlan()
        {
            return planText.Text;
        }

        internal void SetPlan(string plan)
        {
            planText.Text = plan;
            planText.Focus();
            planText.SelectionStart = planText.Text.Length;
            planText.SelectionLength = planText.Text.Length;
        }
    }
}
