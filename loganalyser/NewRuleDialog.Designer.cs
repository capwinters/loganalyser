﻿namespace loganalyser
{
    partial class NewRuleDialog
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
            this.RuleEditorCancel = new System.Windows.Forms.Button();
            this.RuleEditorSave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // RuleEditorCancel
            // 
            this.RuleEditorCancel.Location = new System.Drawing.Point(222, 61);
            this.RuleEditorCancel.Name = "RuleEditorCancel";
            this.RuleEditorCancel.Size = new System.Drawing.Size(75, 23);
            this.RuleEditorCancel.TabIndex = 11;
            this.RuleEditorCancel.Text = "Cancel";
            this.RuleEditorCancel.UseVisualStyleBackColor = true;
            this.RuleEditorCancel.Click += new System.EventHandler(this.RuleEditorCancel_Click);
            // 
            // RuleEditorSave
            // 
            this.RuleEditorSave.Location = new System.Drawing.Point(141, 61);
            this.RuleEditorSave.Name = "RuleEditorSave";
            this.RuleEditorSave.Size = new System.Drawing.Size(75, 23);
            this.RuleEditorSave.TabIndex = 10;
            this.RuleEditorSave.Text = "Save";
            this.RuleEditorSave.UseVisualStyleBackColor = true;
            this.RuleEditorSave.Click += new System.EventHandler(this.RuleEditorSave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Rule";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Name";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(65, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(353, 20);
            this.textBox2.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(65, 9);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(353, 20);
            this.textBox1.TabIndex = 6;
            // 
            // NewRuleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 89);
            this.ControlBox = false;
            this.Controls.Add(this.RuleEditorCancel);
            this.Controls.Add(this.RuleEditorSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewRuleDialog";
            this.ShowIcon = false;
            this.Text = "New Rule";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RuleEditorCancel;
        private System.Windows.Forms.Button RuleEditorSave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
    }
}