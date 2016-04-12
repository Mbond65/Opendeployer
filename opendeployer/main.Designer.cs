namespace opendeployer
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.pbMain = new MetroFramework.Controls.MetroProgressBar();
            this.lblHelpText = new MetroFramework.Controls.MetroLabel();
            this.lblStatus = new MetroFramework.Controls.MetroLabel();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.lblApplicationName = new MetroFramework.Controls.MetroLabel();
            this.lblApplicationVersion = new MetroFramework.Controls.MetroLabel();
            this.pbLoading = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // pbMain
            // 
            this.pbMain.Location = new System.Drawing.Point(177, 140);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(390, 23);
            this.pbMain.TabIndex = 0;
            // 
            // lblHelpText
            // 
            this.lblHelpText.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblHelpText.Location = new System.Drawing.Point(177, 166);
            this.lblHelpText.Name = "lblHelpText";
            this.lblHelpText.Size = new System.Drawing.Size(390, 30);
            this.lblHelpText.TabIndex = 1;
            this.lblHelpText.WrapToLine = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.lblStatus.Location = new System.Drawing.Point(177, 102);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 0);
            this.lblStatus.TabIndex = 2;
            // 
            // pbLogo
            // 
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(11, 32);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(150, 150);
            this.pbLogo.TabIndex = 3;
            this.pbLogo.TabStop = false;
            // 
            // lblApplicationName
            // 
            this.lblApplicationName.AutoSize = true;
            this.lblApplicationName.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblApplicationName.Location = new System.Drawing.Point(177, 46);
            this.lblApplicationName.Name = "lblApplicationName";
            this.lblApplicationName.Size = new System.Drawing.Size(0, 0);
            this.lblApplicationName.TabIndex = 4;
            // 
            // lblApplicationVersion
            // 
            this.lblApplicationVersion.AutoSize = true;
            this.lblApplicationVersion.FontSize = MetroFramework.MetroLabelSize.Small;
            this.lblApplicationVersion.Location = new System.Drawing.Point(177, 69);
            this.lblApplicationVersion.Name = "lblApplicationVersion";
            this.lblApplicationVersion.Size = new System.Drawing.Size(0, 0);
            this.lblApplicationVersion.TabIndex = 5;
            // 
            // pbLoading
            // 
            this.pbLoading.Image = ((System.Drawing.Image)(resources.GetObject("pbLoading.Image")));
            this.pbLoading.InitialImage = ((System.Drawing.Image)(resources.GetObject("pbLoading.InitialImage")));
            this.pbLoading.Location = new System.Drawing.Point(533, 102);
            this.pbLoading.Name = "pbLoading";
            this.pbLoading.Size = new System.Drawing.Size(34, 32);
            this.pbLoading.TabIndex = 6;
            this.pbLoading.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 205);
            this.Controls.Add(this.pbLoading);
            this.Controls.Add(this.lblApplicationVersion);
            this.Controls.Add(this.lblApplicationName);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblHelpText);
            this.Controls.Add(this.pbMain);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Resizable = false;
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroProgressBar pbMain;
        private MetroFramework.Controls.MetroLabel lblHelpText;
        private MetroFramework.Controls.MetroLabel lblStatus;
        private System.Windows.Forms.PictureBox pbLogo;
        private MetroFramework.Controls.MetroLabel lblApplicationName;
        private MetroFramework.Controls.MetroLabel lblApplicationVersion;
        private System.Windows.Forms.PictureBox pbLoading;
    }
}

