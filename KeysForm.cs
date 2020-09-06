using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Rewrite
{
    public partial class KeysForm : Form
    {
        private int itemsCounter = 0;
        public List<Key> keys = new List<Key>();

        public KeysForm()
        {
            InitializeComponent();
        }

        internal List<Key> GetKeyStringsArray()
        {
            keys.Clear();

            for (int i = 0; i < this.itemsPanel.Controls.Count; i++)
            {
                Key k = new Key();

                for (int j = 0; j < this.itemsPanel.Controls[i].Controls.Count; j++)
                {
                    if (this.itemsPanel.Controls[i].Controls[j].Name.IndexOf("textBoxCnt") != -1)
                    {
                        string c = this.itemsPanel.Controls[i].Controls[j].Text.Trim();
                        c = (c == "") ? "0" : c;
                        k.count = int.Parse(c);
                    }
                    else if (this.itemsPanel.Controls[i].Controls[j].Name.IndexOf("textBoxText") != -1)
                    {
                        k.text = this.itemsPanel.Controls[i].Controls[j].Text.Trim();
                    }
                }

                keys.Add(k);
            }

            return keys;
        }

        internal void SetKeys(List<Key> keys)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                Control panel = AddPanel(false);

                for (int j = 0; j < panel.Controls.Count; j++)
                {
                    if (panel.Controls[j].Name.IndexOf("textBoxCnt") != -1)
                    {
                        panel.Controls[j].Text = keys[i].count.ToString();
                    }
                    else if (panel.Controls[j].Name.IndexOf("textBoxText") != -1)
                    {
                        panel.Controls[j].Text = keys[i].text;
                    }
                    else if (panel.Controls[j].Name.IndexOf("sublabel") != -1)
                    {
                        panel.Controls[j].Text = "Кол-во: " + keys[i].doneCount + " из ";
                    }
                } 
            }
        }

        private Control AddPanel(bool setDefautlValues)
        {
            // 
            // textBox2
            // 
            int heightStep = 40;

            TextBox textBoxCnt = new TextBox();
            textBoxCnt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBoxCnt.Location = new System.Drawing.Point(581, 3);
            textBoxCnt.Name = "textBoxCnt" + itemsCounter;
            textBoxCnt.Size = new System.Drawing.Size(61, 26);
            textBoxCnt.TabIndex = 24;
            if (setDefautlValues)
            {
                textBoxCnt.Text = "1";
            }
            // 
            // label1
            // 
            Label label = new Label();
            label.AutoSize = true;
            label.Location = new System.Drawing.Point(511, 10);
            label.Name = "sublabel" + itemsCounter;
            label.Size = new System.Drawing.Size(64, 13);
            label.TabIndex = 23;
            label.Text = "Кол-во: 0 из ";
            // 
            // clearSearchBtn
            // 
            Button removeKeyBtn = new Button();
            removeKeyBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            removeKeyBtn.Location = new System.Drawing.Point(651, 3);
            removeKeyBtn.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            removeKeyBtn.Name = "removeKeyBtn" + itemsCounter;
            removeKeyBtn.Padding = new System.Windows.Forms.Padding(2, 0, 0, 1);
            removeKeyBtn.Size = new System.Drawing.Size(26, 26);
            removeKeyBtn.TabIndex = 22;
            removeKeyBtn.Text = "X";
            removeKeyBtn.Click += new EventHandler(removeKeyBtn_Click);
            removeKeyBtn.UseVisualStyleBackColor = true;
            // 
            // textBox
            // 
            TextBox textBoxText = new TextBox();
            textBoxText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            textBoxText.Location = new System.Drawing.Point(3, 3);
            textBoxText.Name = "textBoxText" + itemsCounter;
            textBoxText.Size = new System.Drawing.Size(502, 26);
            textBoxText.TabIndex = 21;
            // 
            // panel1
            // 
            Panel panel = new Panel();
            panel.BackColor = System.Drawing.SystemColors.Control;
            panel.Location = new System.Drawing.Point(13, 3 + heightStep * itemsCounter);
            panel.Name = "subpanel" + itemsCounter;
            panel.Size = new System.Drawing.Size(714, 34);
            panel.TabIndex = 5;

            panel.Controls.Add(textBoxText);
            panel.Controls.Add(label);
            panel.Controls.Add(textBoxCnt);
            panel.Controls.Add(removeKeyBtn);

            this.itemsPanel.Controls.Add(panel);
            itemsCounter++;

            return panel;
        }

        private void addPhraseBtn_Click(object sender, EventArgs e)
        {
            AddPanel(true);
        }

        void removeKeyBtn_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int number = int.Parse(b.Name.Replace("removeKeyBtn", ""));
            Control[] cnt = this.itemsPanel.Controls.Find("subpanel" + number, true);
            int heightStep = 40;

            if (cnt.Length > 0)
            {
                this.itemsPanel.Controls.Remove(cnt[0]);
                this.itemsCounter--;
            }

            for (int i = 0; i < this.itemsPanel.Controls.Count; i++)
            {
                this.itemsPanel.Controls[i].Location = new System.Drawing.Point(13, 3 + heightStep * i);
            }
            
        }
    }
}
