using System;
using System.Drawing;
using System.IO;

namespace opendeployer
{
    public partial class msgboxLogo : MetroFramework.Forms.MetroForm
    {
        public string _message;
        public string _opendeployerLocalPath = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"\Opendeployer\");

        public msgboxLogo()
        {
            InitializeComponent();
        }
        private void txtbtnOk_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }
        private void msgboxLogo_Load(object sender, EventArgs e)
        {
            lblMessage.Text = _message;
        }
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
