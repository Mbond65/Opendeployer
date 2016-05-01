using System;
using System.Drawing;
using System.IO;

namespace opendeployer
{
    public partial class datepicker : MetroFramework.Forms.MetroForm
    {
        public string _message;
        public string _scheduledInstallDate { get; set; }
        public string _scheduledInstallTime { get; set; }
        public string _opendeployerLocalPath = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"\Opendeployer\");

        public datepicker()
        {
            InitializeComponent();
        }

        private void txtbtnSave_Click(object sender, EventArgs e)
        {
            _scheduledInstallDate = dtPicker.Value.ToShortDateString();
            _scheduledInstallTime = dtPicker.Value.ToShortTimeString();

            ActiveForm.Close();
        }

        private void msgboxLogo_Load(object sender, EventArgs e)
        {
            getLogo();
        }

        /// <summary>
        /// Gets logo if exists
        /// </summary>
        private void getLogo()
        {
            if (File.Exists("logo.jpg"))
            {
                pbLogo.Image = Image.FromFile("logo.jpg", false);
            }
            else if (File.Exists(_opendeployerLocalPath + @"\" + "logo.jpg"))
            {
                pbLogo.Image = Image.FromFile(_opendeployerLocalPath + @"\" + "logo.jpg", false);
            }
        }
    }
}
