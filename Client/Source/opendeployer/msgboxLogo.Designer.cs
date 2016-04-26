namespace opendeployer
{
    partial class msgboxLogo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(msgboxLogo));
            this.txtbtnOk = new MetroFramework.Controls.MetroTextBox.MetroTextButton();
            this.lblMessage = new MetroFramework.Controls.MetroLabel();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // txtbtnOk
            // 
            this.txtbtnOk.Image = null;
            this.txtbtnOk.Location = new System.Drawing.Point(164, 143);
            this.txtbtnOk.Name = "txtbtnOk";
            this.txtbtnOk.Size = new System.Drawing.Size(109, 23);
            this.txtbtnOk.TabIndex = 1;
            this.txtbtnOk.Text = "Ok";
            this.txtbtnOk.UseSelectable = true;
            this.txtbtnOk.UseVisualStyleBackColor = true;
            this.txtbtnOk.Click += new System.EventHandler(this.txtbtnOk_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblMessage.Location = new System.Drawing.Point(164, 36);
            this.lblMessage.MaximumSize = new System.Drawing.Size(400, 125);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(253, 96);
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
            // msgboxLogo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 177);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtbtnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "msgboxLogo";
            this.Resizable = false;
            this.Load += new System.EventHandler(this.msgboxLogo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox.MetroTextButton txtbtnOk;
        private MetroFramework.Controls.MetroLabel lblMessage;
        private System.Windows.Forms.PictureBox pbLogo;
    }
}