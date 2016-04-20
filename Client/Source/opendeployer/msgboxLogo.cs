using System;
using System.Drawing;
using System.IO;

namespace opendeployer
{
    public partial class msgboxLogo : MetroFramework.Forms.MetroForm
    {
        public string _message;

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
            bool logoFileExists = File.Exists("logo.jpg");

            if (logoFileExists)
            {
                Image logo = Image.FromFile("logo.jpg", false);
                pbLogo.Image = logo;
            }
        }
    }
}
