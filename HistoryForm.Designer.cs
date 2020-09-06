namespace Rewrite
{
    partial class HistoryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.historyOkBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.historyList = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.historyRichTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.historyOkBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(918, 46);
            this.panel1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(450, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // historyOkBtn
            // 
            this.historyOkBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.historyOkBtn.Location = new System.Drawing.Point(345, 12);
            this.historyOkBtn.Name = "historyOkBtn";
            this.historyOkBtn.Size = new System.Drawing.Size(75, 23);
            this.historyOkBtn.TabIndex = 0;
            this.historyOkBtn.Text = "Применить";
            this.historyOkBtn.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel2.Controls.Add(this.historyList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 46);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(171, 738);
            this.panel2.TabIndex = 1;
            // 
            // historyList
            // 
            this.historyList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyList.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.historyList.FormattingEnabled = true;
            this.historyList.ItemHeight = 19;
            this.historyList.Location = new System.Drawing.Point(0, 0);
            this.historyList.Margin = new System.Windows.Forms.Padding(0);
            this.historyList.Name = "historyList";
            this.historyList.Size = new System.Drawing.Size(171, 726);
            this.historyList.TabIndex = 0;
            this.historyList.SelectedIndexChanged += new System.EventHandler(this.historyList_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel3.Controls.Add(this.historyRichTextBox);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(171, 46);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(747, 738);
            this.panel3.TabIndex = 2;
            // 
            // historyRichTextBox
            // 
            this.historyRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyRichTextBox.Font = new System.Drawing.Font("Times New Roman", 12F);
            this.historyRichTextBox.Location = new System.Drawing.Point(0, 0);
            this.historyRichTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.historyRichTextBox.Name = "historyRichTextBox";
            this.historyRichTextBox.Size = new System.Drawing.Size(747, 738);
            this.historyRichTextBox.TabIndex = 0;
            this.historyRichTextBox.Text = "";
            // 
            // HistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 784);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistoryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "История изменений";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button historyOkBtn;
        private System.Windows.Forms.ListBox historyList;
        public System.Windows.Forms.RichTextBox historyRichTextBox;


    }
}