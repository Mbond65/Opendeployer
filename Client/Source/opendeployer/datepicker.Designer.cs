﻿namespace opendeployer
{
    partial class datepicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(datepicker));
            this.txtbtnSave = new MetroFramework.Controls.MetroTextBox.MetroTextButton();
            this.lblMessage = new MetroFramework.Controls.MetroLabel();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // txtbtnSave
            // 
            this.txtbtnSave.Image = null;
            this.txtbtnSave.Location = new System.Drawing.Point(164, 159);
            this.txtbtnSave.Name = "txtbtnSave";
            this.txtbtnSave.Size = new System.Drawing.Size(109, 23);
            this.txtbtnSave.TabIndex = 1;
            this.txtbtnSave.Text = "Save";
            this.txtbtnSave.UseSelectable = true;
            this.txtbtnSave.UseVisualStyleBackColor = true;
            this.txtbtnSave.Click += new System.EventHandler(this.txtbtnSave_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblMessage.Location = new System.Drawing.Point(164, 16);
            this.lblMessage.MaximumSize = new System.Drawing.Size(400, 125);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(253, 112);
            this.lblMessage.TabIndex = 7;
            this.lblMessage.Text = resources.GetString("lblMessage.Text");
            this.lblMessage.WrapToLine = true;
            // 
            // pbLogo
            // 
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(8, 16);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(150, 150);
            this.pbLogo.TabIndex = 8;
            this.pbLogo.TabStop = false;
            // 
            // dtPicker
            // 
            this.dtPicker.CustomFormat = "MMMMdd yyyy hh:mm tt";
            this.dtPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPicker.Location = new System.Drawing.Point(164, 131);
            this.dtPicker.MaxDate = System.DateTime.Now.AddMonths(1);
            this.dtPicker.MinDate = System.DateTime.Now;
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.ShowUpDown = true;
            this.dtPicker.Size = new System.Drawing.Size(200, 20);
            this.dtPicker.TabIndex = 11;
            this.dtPicker.Value = System.DateTime.Now;
            // 
            // datepicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 205);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtbtnSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "datepicker";
            this.Resizable = false;
            this.Load += new System.EventHandler(this.msgboxLogo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox.MetroTextButton txtbtnSave;
        private MetroFramework.Controls.MetroLabel lblMessage;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.DateTimePicker dtPicker;
    }
}