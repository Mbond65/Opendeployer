using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace opendeployer
{
    public partial class Proceed : MetroFramework.Forms.MetroForm
    {
        public bool _messageBox { get; set; }
        public string _applicationName { get; set; }
        public string _companyName { get; set; }
        public bool _scheduledInstall { get; set; }
        public string _scheduledInstallDate { get; set; }
        public string _scheduledInstallTime { get; set; }
        public string _opendeployerLocalPath = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"\Opendeployer\");

        public Proceed()
        {
            InitializeComponent();
        }

        private void Messagebox_Load(object sender, EventArgs e)
        {
            _messageBox = false;
            lblApplicationName.Text = _applicationName + " Install";

            try
            {
                getLogo();
                getCustomMessage();
            }
            catch (Exception ex)
            {
                writeEventLog(ex.Message);
                Environment.Exit(1);
            }
                       
        }
        private void writeEventLog(string errorMessage)
        {
            string sSource = _companyName;
            string sLog = "Application";
            string sEvent = errorMessage;

            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
                EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error);
            }

            EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error);
        }
        private void txtbtnNo_Click(object sender, EventArgs e)
        {
            _messageBox = false;
            ActiveForm.Close();
           
        }
        private void txtbtnYes_Click(object sender, EventArgs e)
        {
            if (rbInstallNow.Checked == true)
            {
                _messageBox = true;
                ActiveForm.Close();
            }
            else if (rbInstallLaterDate.Checked == true)
            {
                _messageBox = true;
                _scheduledInstall = true;
                _scheduledInstallDate = dtPicker.Value.ToShortDateString();
                _scheduledInstallTime = dtPicker.Value.ToShortTimeString();
                ActiveForm.Close();
            }
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
        private void getCustomMessage()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Config.xml");

            XmlNode node = doc.DocumentElement.SelectSingleNode("/Config/proceedMessage");

            lblProceedMessage.Text = node.InnerText;
        }
        private void rbInstallLaterDate_CheckedChanged(object sender, EventArgs e)
        {
            dtPicker.Enabled = true;
        }
        private void rbInstallNow_CheckedChanged(object sender, EventArgs e)
        {
            dtPicker.Enabled = false;
        }
    }
}
