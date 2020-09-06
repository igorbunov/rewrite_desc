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
    public partial class Donate : Form
    {
        public Donate()
        {
            InitializeComponent();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.SelectAll();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
            textBox2.SelectAll();
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            textBox3.Focus();
            textBox3.SelectAll();
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            textBox4.Focus();
            textBox4.SelectAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.SelectAll();
            textBox1.Copy();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
            textBox2.SelectAll();
            textBox2.Copy();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Focus();
            textBox3.SelectAll();
            textBox3.Copy();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Focus();
            textBox4.SelectAll();
            textBox4.Copy();
        }
    }
}
