namespace opendeployer
{
    partial class Proceed
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Proceed));
            this.txtbtnYes = new MetroFramework.Controls.MetroTextBox.MetroTextButton();
            this.txtbtnNo = new MetroFramework.Controls.MetroTextBox.MetroTextButton();
            this.lblApplicationName = new MetroFramework.Controls.MetroLabel();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.lblProceedMessage = new MetroFramework.Controls.MetroLabel();
            this.rbInstallNow = new MetroFramework.Controls.MetroRadioButton();
            this.rbInstallLaterDate = new MetroFramework.Controls.MetroRadioButton();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // txtbtnYes
            // 
            this.txtbtnYes.Image = null;
            this.txtbtnYes.Location = new System.Drawing.Point(167, 261);
            this.txtbtnYes.Name = "txtbtnYes";
            this.txtbtnYes.Size = new System.Drawing.Size(109, 23);
            this.txtbtnYes.TabIndex = 0;
            this.txtbtnYes.Text = "Yes";
            this.txtbtnYes.UseSelectable = true;
            this.txtbtnYes.UseVisualStyleBackColor = true;
            this.txtbtnYes.Click += new System.EventHandler(this.txtbtnYes_Click);
            // 
            // txtbtnNo
            // 
            this.txtbtnNo.Image = null;
            this.txtbtnNo.Location = new System.Drawing.Point(304, 261);
            this.txtbtnNo.Name = "txtbtnNo";
            this.txtbtnNo.Size = new System.Drawing.Size(109, 23);
            this.txtbtnNo.TabIndex = 1;
            this.txtbtnNo.Text = "No";
            this.txtbtnNo.UseSelectable = true;
            this.txtbtnNo.UseVisualStyleBackColor = true;
            this.txtbtnNo.Click += new System.EventHandler(this.txtbtnNo_Click);
            // 
            // lblApplicationName
            // 
            this.lblApplicationName.AutoSize = true;
            this.lblApplicationName.Location = new System.Drawing.Point(165, 23);
            this.lblApplicationName.Name = "lblApplicationName";
            this.lblApplicationName.Size = new System.Drawing.Size(111, 19);
            this.lblApplicationName.TabIndex = 2;
            this.lblApplicationName.Text = "Application Install";
            // 
            // pbLogo
            // 
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(7, 23);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(150, 150);
            this.pbLogo.TabIndex = 5;
            this.pbLogo.TabStop = false;
            // 
            // lblProceedMessage
            // 
            this.lblProceedMessage.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblProceedMessage.Location = new System.Drawing.Point(165, 60);
            this.lblProceedMessage.Name = "lblProceedMessage";
            this.lblProceedMessage.Size = new System.Drawing.Size(238, 60);
            this.lblProceedMessage.TabIndex = 6;
            this.lblProceedMessage.WrapToLine = true;
            // 
            // rbInstallNow
            // 
            this.rbInstallNow.AutoSize = true;
            this.rbInstallNow.Location = new System.Drawing.Point(167, 148);
            this.rbInstallNow.Name = "rbInstallNow";
            this.rbInstallNow.Size = new System.Drawing.Size(80, 15);
            this.rbInstallNow.TabIndex = 7;
            this.rbInstallNow.Text = "Install now";
            this.rbInstallNow.UseSelectable = true;
            this.rbInstallNow.CheckedChanged += new System.EventHandler(this.rbInstallNow_CheckedChanged);
            this.rbInstallNow.Checked = true;
            // 
            // rbInstallLaterDate
            // 
            this.rbInstallLaterDate.AutoSize = true;
            this.rbInstallLaterDate.Location = new System.Drawing.Point(167, 179);
            this.rbInstallLaterDate.Name = "rbInstallLaterDate";
            this.rbInstallLaterDate.Size = new System.Drawing.Size(128, 15);
            this.rbInstallLaterDate.TabIndex = 8;
            this.rbInstallLaterDate.Text = "Install at a later date";
            this.rbInstallLaterDate.UseSelectable = true;
            this.rbInstallLaterDate.CheckedChanged += new System.EventHandler(this.rbInstallLaterDate_CheckedChanged);
            // 
            // dtPicker
            // 
            this.dtPicker.Location = new System.Drawing.Point(165, 215);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.ShowUpDown = true;
            this.dtPicker.Size = new System.Drawing.Size(200, 20);
            this.dtPicker.TabIndex = 10;
            this.dtPicker.Enabled = false;
            this.dtPicker.MinDate = System.DateTime.Now;
            this.dtPicker.MaxDate = System.DateTime.Now.AddMonths(1);
            this.dtPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPicker.CustomFormat = "MMMMdd yyyy hh:mm tt";
            // 
            // Proceed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 307);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.rbInstallLaterDate);
            this.Controls.Add(this.rbInstallNow);
            this.Controls.Add(this.lblProceedMessage);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.lblApplicationName);
            this.Controls.Add(this.txtbtnNo);
            this.Controls.Add(this.txtbtnYes);
            this.MaximizeBox = false;
            this.Name = "Proceed";
            this.Resizable = false;
            this.Load += new System.EventHandler(this.Messagebox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox.MetroTextButton txtbtnYes;
        private MetroFramework.Controls.MetroTextBox.MetroTextButton txtbtnNo;
        private MetroFramework.Controls.MetroLabel lblApplicationName;
        private System.Windows.Forms.PictureBox pbLogo;
        private MetroFramework.Controls.MetroLabel lblProceedMessage;
        private MetroFramework.Controls.MetroRadioButton rbInstallNow;
        private MetroFramework.Controls.MetroRadioButton rbInstallLaterDate;
        private System.Windows.Forms.DateTimePicker dtPicker;
    }
}