using System;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace opendeployer
{
    public partial class msgbox : MetroFramework.Forms.MetroForm
    {
        public string _message;

        public msgbox()
        {
            InitializeComponent();
        }

        private void txtbtnOk_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }

        private void msgbox_Load(object sender, EventArgs e)
        {
            lblMessage.Text = _message;
        }
    }
}
