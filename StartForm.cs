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
    public partial class StartForm : Form
    {
        public Timer t = new Timer();
        public StartForm()
        {
            InitializeComponent();
            t.Interval = 1000;
            t.Tick += new EventHandler(t_Tick);
        }

        void t_Tick(object sender, EventArgs e)
        {
            string text = labelLoading.Text;

            switch(text)
            {
                case "Загрузка словарей...":
                    labelLoading.Text = "Загрузка словарей";
                    break;
                case "Загрузка словарей":
                    labelLoading.Text = "Загрузка словарей.";
                    break;
                case "Загрузка словарей.":
                    labelLoading.Text = "Загрузка словарей..";
                    break;
                case "Загрузка словарей..":
                    labelLoading.Text = "Загрузка словарей...";
                    break;
            }
        }

        private void StartForm_Shown(object sender, EventArgs e)
        {
            t.Start();
        }

        private void StartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Stop();
        }
    }
}
