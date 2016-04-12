using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using System.Drawing;

namespace opendeployer
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {
        private string _applicationName;
        private string _applicationInstallerLocation;
        private string _applicationLocalLocation;
        private string _applicationVersion;
        private string _applicationGuid;
        private string _companyName;

        public Main()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {                   
            try
            {
                checkXMLFile();
                assignArguementVariables();
                getLogo();
                getHelpText();
                setDetailLabels();
                checkOpenDeployerRegistry();
                checkApplicationRegistryEntry();
                checkApplicationNotInstalled();
                checkApplicationNotInstalled64();
                askUser();
            }
            catch (Exception ex)
            {
                writeEventLog(ex.Message);
                Environment.Exit(1);
            }
        }
        private void Main_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            try
            {
                checkApplicationLocalDir();
                getInstaller();
                extractInstaller();
                runInstaller();
                //sendEmailUser();
                deleteInstaller();
                writeEventLog(_applicationName + " installed successfully", EventLogEntryType.Information);
                notifyUserComplete();                
            }
            catch (Exception ex)
            {
                writeEventLog(ex.Message);
                Environment.Exit(1);
            }
        }
        private void checkXMLFile()
        {
            bool fileExists = File.Exists("Software.xml");
            if (!fileExists)
            {
                throw new Exception("Software XML file does not exist");
            }
        }
        private void assignArguementVariables()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Software.xml");

            XmlNode node = doc.DocumentElement.SelectSingleNode("/Software/applicationName");
            XmlNode node2 = doc.DocumentElement.SelectSingleNode("/Software/applicationInstallLocation");
            XmlNode node3 = doc.DocumentElement.SelectSingleNode("/Software/applicationLocalLocation");
            XmlNode node4 = doc.DocumentElement.SelectSingleNode("/Software/applicationVersion");
            XmlNode node5 = doc.DocumentElement.SelectSingleNode("/Software/applicationGUID");

            _applicationName = node.InnerText;
            _applicationInstallerLocation = node2.InnerText;
            _applicationLocalLocation = node3.InnerText;
            _applicationVersion = node4.InnerText;
            _applicationGuid = node5.InnerText;
                       
            doc.Load("config.xml");

            node = doc.DocumentElement.SelectSingleNode("/Config/installerName");

            _companyName = node.InnerText;
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
        private void getHelpText()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Config.xml");

            XmlNode node = doc.DocumentElement.SelectSingleNode("/Config/helpText");

            lblHelpText.Text = node.InnerText;
        }
        private void setDetailLabels()
        {
            lblApplicationName.Text = "Application package: " + _applicationName;
            lblApplicationVersion.Text = "Version: " + _applicationVersion;
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
        private void writeEventLog(string errorMessage, EventLogEntryType type)
        {
            string sSource = _companyName;
            string sLog = "Application";
            string sEvent = errorMessage;

            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
                EventLog.WriteEntry(sSource, sEvent, type);
            }

            EventLog.WriteEntry(sSource, sEvent, type);
        }
        private void checkOpenDeployerRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer");
            if (key == null)
            {
                RegistryKey software = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true);
                software.CreateSubKey("Opendeployer");
            }
        }
        private void checkApplicationRegistryEntry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer", true);
            var guidValue = key.GetValue(_applicationGuid);
            
            if (guidValue == null)
            {
                key.SetValue(_applicationGuid, "999", RegistryValueKind.DWord);
            }
            else if ((int)guidValue == 1)
            {
                writeEventLog("Application is already installed", EventLogEntryType.Information);
                Environment.Exit(0);
            }
            else if ((int)guidValue == 0)
            {
                writeEventLog("Application expereienced a failure when installing this package previously, please test on the machine manually and then remove the registry key containing the GUID for this package", EventLogEntryType.Error);
                Environment.Exit(1);
            }

        }
        private void checkApplicationNotInstalled()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        string displayName = (string)subkey.GetValue("DisplayName") ?? "NULL";
                        object version = subkey.GetValue("Version");

                        if (displayName.ToLower() == _applicationName.ToLower() && (version ?? "notinstalled").ToString() == Convert.ToString(_applicationVersion))
                        {
                            writeEventLog("Application already installed", EventLogEntryType.Information);
                            Environment.Exit(0);
                        }                     
                    }
                }
            }     
        }
        private void checkApplicationNotInstalled64()
        {
            string registry_key64 = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key64))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        string displayName = (string)subkey.GetValue("DisplayName") ?? "NULL";
                        object version = subkey.GetValue("Version");

                        if (displayName.ToLower() == _applicationName.ToLower() && (version ?? "notinstalled").ToString() == Convert.ToString(_applicationVersion))
                        {
                            writeEventLog("Application already installed", EventLogEntryType.Information);
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }
        private void checkApplicationLocalDir()
        {
            lblStatus.Text = "Status: Creating application directory";
            pbMain.Visible = true;
            Application.DoEvents();

            if (Directory.Exists(_applicationLocalLocation) == true)
            {
                Directory.Delete(_applicationLocalLocation, true);
                Directory.CreateDirectory(_applicationLocalLocation);
            }
            else
            {
                Directory.CreateDirectory(_applicationLocalLocation);
            }
        }        
        private void getInstaller()
        {
            lblStatus.Text = "Status: Downloading installer";
            pbMain.Value = 20;
            Application.DoEvents();

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(_applicationInstallerLocation, String.Concat(_applicationLocalLocation, @"\", _applicationName, ".zip"));
            }
        }
        private void extractInstaller()
        {
            lblStatus.Text = "Status: Extracting installer";
            pbMain.Value = 40;
            Application.DoEvents();

            System.IO.Compression.ZipFile.ExtractToDirectory(String.Concat(_applicationLocalLocation, @"\", _applicationName, ".zip"), String.Concat(_applicationLocalLocation, @"\", _applicationName));
        }
        private void askUser()
        {
            Proceed msgBox = new Proceed();
            msgBox._applicationName = _applicationName;
            msgBox._companyName = _companyName;

            formflash.FlashWindowEx(msgBox);            
            msgBox.ShowDialog();

            if (msgBox._messageBox == false)
            {
                Environment.Exit(1);
            }
                
        }
        private void runInstaller()
        {
            lblStatus.Text = "Status: Running installer";
            pbMain.Value = 80;
            Application.DoEvents();

            XmlDocument doc = new XmlDocument();
            doc.Load("Software.xml");

            XmlNode nodes = doc.DocumentElement.SelectSingleNode("/Software/CommandLines");

            foreach (XmlNode node in nodes.ChildNodes)
            {                
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = @node.FirstChild.InnerText;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = @node.LastChild.InnerText;


                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
        }
        private void deleteInstaller()
        {
            lblStatus.Text = "Status: Deleting installer";

            Directory.Delete(_applicationLocalLocation, true);
        }
        private void notifyUserComplete()
        {
            if (checkApplicationInstalled() != false && checkApplicationInstalled64() != false)
            {
                lblStatus.Text = "Status: Complete";
                pbMain.Value = 100;
                updateGUIDRegistryKey(true);
            }
            else
            {
                lblStatus.Text = "Status: Failed";
                pbMain.Value = 0;
                updateGUIDRegistryKey(false);
            }         

            Complete msgBox = new Complete();
            msgBox._applicationName = _applicationName;
            msgBox._companyName = _companyName;

            formflash.FlashWindowEx(msgBox);
            msgBox.ShowDialog();

            Environment.Exit(1);
        }
        private void updateGUIDRegistryKey(bool installed)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer", true);
            key.SetValue(_applicationGuid, Convert.ToInt32(installed), RegistryValueKind.DWord);
        }
        private bool checkApplicationInstalled()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        string displayName = (string)subkey.GetValue("DisplayName") ?? "NULL";
                        object version = subkey.GetValue("Version");

                        if (displayName.ToLower() == _applicationName.ToLower() && (version ?? "notinstalled").ToString() == Convert.ToString(_applicationVersion))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private bool checkApplicationInstalled64()
        {
            string registry_key = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        string displayName = (string)subkey.GetValue("DisplayName") ?? "NULL";
                        object version = subkey.GetValue("Version");

                        if (displayName.ToLower() == _applicationName.ToLower() && (version ?? "notinstalled").ToString() == Convert.ToString(_applicationVersion))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
