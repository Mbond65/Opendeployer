﻿using System;
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
        public bool _installedSuccessfully { get; set; }
        public string _opendeployerLocalPath = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"\Opendeployer\");

        public Complete()
        {
            InitializeComponent();
        }

        private void txtbtnOk_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
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

        /// <summary>
        /// Gets custom complete message from config file if exists
        /// </summary>
        private void getCustomMessage()
        {
            XmlDocument doc = new XmlDocument();

            if (File.Exists("config.xml"))
            {
                doc.Load("Config.xml");
            }
            else if (File.Exists(_opendeployerLocalPath + @"\" + "config.xml"))
            {
                doc.Load(_opendeployerLocalPath + @"\" + "config.xml");
            }

            XmlNode node;

            if (_installedSuccessfully == true)
            {
                node = doc.DocumentElement.SelectSingleNode("/Config/completeMessageSuccess");
            }
            else
            {
                node = doc.DocumentElement.SelectSingleNode("/Config/completeMessageFailed");
            }

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

        /// <summary>
        /// Write to event log
        /// </summary>
        /// <param name="errorMessage"></param>
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
