using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;

namespace opendeployer
{
    public partial class Complete : MetroFramework.Forms.MetroForm
    {
        public string _applicationName { get; set; }
        public string _companyName { get; set; }

        public Complete()
        {
            InitializeComponent();
        }

        private void txtbtnOk_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
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
        private void getCustomMessage()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Config.xml");

            XmlNode node = doc.DocumentElement.SelectSingleNode("/Config/completeMessage");

            lblCompleteMessage.Text = node.InnerText;
        }
        private void Complete_Load(object sender, EventArgs e)
        {
            lblApplicationName.Text = _applicationName + " Install Complete";

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
    }
}
