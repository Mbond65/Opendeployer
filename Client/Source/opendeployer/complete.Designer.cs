namespace opendeployer
{
    partial class Complete
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Complete));
            this.txtbtnOk = new MetroFramework.Controls.MetroTextBox.MetroTextButton();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.lblCompleteMessage = new MetroFramework.Controls.MetroLabel();
            this.lblApplicationName = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // txtbtnOk
            // 
            this.txtbtnOk.Image = null;
            this.txtbtnOk.Location = new System.Drawing.Point(308, 131);
            this.txtbtnOk.Name = "txtbtnOk";
            this.txtbtnOk.Size = new System.Drawing.Size(109, 23);
            this.txtbtnOk.TabIndex = 1;
            this.txtbtnOk.Text = "Ok";
            this.txtbtnOk.UseSelectable = true;
            this.txtbtnOk.UseVisualStyleBackColor = true;
            this.txtbtnOk.Click += new System.EventHandler(this.txtbtnOk_Click);
            // 
            // pbLogo
            // 
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(23, 23);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(150, 150);
            this.pbLogo.TabIndex = 4;
            this.pbLogo.TabStop = false;
            // 
            // lblCompleteMessage
            // 
            this.lblCompleteMessage.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblCompleteMessage.Location = new System.Drawing.Point(179, 60);
            this.lblCompleteMessage.Name = "lblCompleteMessage";
            this.lblCompleteMessage.Size = new System.Drawing.Size(238, 60);
            this.lblCompleteMessage.TabIndex = 7;
            this.lblCompleteMessage.WrapToLine = true;
            // 
            // lblApplicationName
            // 
            this.lblApplicationName.AutoSize = true;
            this.lblApplicationName.Location = new System.Drawing.Point(179, 23);
            this.lblApplicationName.Name = "lblApplicationName";
            this.lblApplicationName.Size = new System.Drawing.Size(103, 19);
            this.lblApplicationName.TabIndex = 8;
            this.lblApplicationName.Text = "Install Complete";
            // 
            // Complete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 177);
            this.Controls.Add(this.lblApplicationName);
            this.Controls.Add(this.lblCompleteMessage);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.txtbtnOk);
            this.MaximizeBox = false;
            this.Name = "Complete";
            this.Resizable = false;
            this.Load += new System.EventHandler(this.Complete_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox.MetroTextButton txtbtnOk;
        private System.Windows.Forms.PictureBox pbLogo;
        private MetroFramework.Controls.MetroLabel lblCompleteMessage;
        private MetroFramework.Controls.MetroLabel lblApplicationName;
    }
}