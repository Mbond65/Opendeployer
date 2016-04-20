using System;
using System.IO;
using System.Net;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using System.Configuration;
using Microsoft.Win32;
using System.Drawing;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Management;
using Microsoft.Win32.TaskScheduler;
using Ionic.Zip;

namespace opendeployer
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {
        private string _applicationName;
        private string _applicationInstallerLocation;
        private string _applicationLocalLocation;
        private string _applicationVersion;
        private string _applicationGuid;
        private string _sqlusername;
        private string _sqlpassword;
        private string _sqlserver;
        private string _companyName;
        private bool _isScheduledInstall;
        private bool _scheduledInstall;
        private string _scheduledInstallDate;
        private string _scheduledInstallTime;
        private string _scheduledInstallGUID;
        private string _computerID;
        private long _extractBytesTransferred;
        private long _extractBytesTotal;
        private int _commandLinesTotal;
        private int _commandLinesRan;
        private string _opendeployerLocalPath = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"\Opendeployer\");

        public Main()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                checkScheduledInstall();
                checkXMLFile();
                assignArguementVariables();
                getLogo();
                getHelpText();
                setDetailLabels();               
                checkOpenDeployerRegistry();
                checkComputerIDRegistry();
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
                if (_scheduledInstall == false)
                {
                    installNow();
                }
                else if (_scheduledInstall == true)
                {
                    installScheduledDate();
                }
                else if (_isScheduledInstall == true)
                {
                    doScheduledInstall();
                }                           
            }
            catch (Exception ex)
            {
                writeEventLog(ex.Message);
                updateGUIDRegistryKey(0);
                writeSQLDB(ex.Message);
                Environment.Exit(1);
            }

        }
        private void installNow()
        {
            checkApplicationLocalDir(_applicationLocalLocation, true);
            getInstaller(_applicationLocalLocation);
            extractInstaller();
            runInstaller();
            //sendEmailUser();
            deleteInstaller();
            writeEventLog(_applicationName + " installed successfully", EventLogEntryType.Information);
            notifyUserComplete();
        }
        private void installScheduledDate()
        {
            checkApplicationLocalDir(_opendeployerLocalPath, false);
            checkApplicationLocalDir(_applicationLocalLocation, true);
            getInstaller(_applicationLocalLocation);
            copyOpenDeployerFiles();
            createTask();
            updateGUIDRegistryKey(3);
            writeEventLog(_applicationName + " is scheduled for install", EventLogEntryType.Information);
            writeSQLDB("Awaiting install");
            notifyScheduledTaskComplete();

            Environment.Exit(0);
        }
        private void doScheduledInstall()
        {
            extractInstaller();
            runInstaller();
            //sendEmailUser();
            deleteInstaller();
            writeEventLog(_applicationName + " installed successfully", EventLogEntryType.Information);
            notifyUserComplete();
        }
        private void notifyScheduledTaskComplete()
        {
            
        }
        private void copyOpenDeployerFiles()
        {
            File.Copy("Opendeployer.exe", _opendeployerLocalPath + @"\opendeployer.exe", true);
            File.Copy("software.xml", _opendeployerLocalPath + @"\" + _applicationGuid + ".xml", true);
            File.Copy("config.xml", _opendeployerLocalPath + @"\config.xml", true);

            if (File.Exists("logo.jpg"))
            {
                File.Copy("logo.jpg", _opendeployerLocalPath + @"\logo.jpg", true);
            }
        }
        private void createTask()
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "Scheduled install of " + _applicationName;
                td.RegistrationInfo.Author = "Opendeployer";
                td.RegistrationInfo.Date = DateTime.Now;

                td.Settings.StartWhenAvailable = true;
                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.DisallowStartIfOnBatteries = false;

                td.Principal.RunLevel = TaskRunLevel.Highest;

                td.Triggers.Add(new TimeTrigger { Enabled = true,
                                                  StartBoundary = Convert.ToDateTime(_scheduledInstallDate).Add(TimeSpan.Parse(_scheduledInstallTime)) });

                td.Actions.Add(new ExecAction(_opendeployerLocalPath + "opendeployer.exe", "-install " + _applicationGuid, null));

                ts.RootFolder.RegisterTaskDefinition("Opendeployer - " + _applicationGuid, td);

            }
        }
        private void checkXMLFile()
        {
            bool fileExists = File.Exists("Software.xml");
            if (!fileExists && _isScheduledInstall == false)
            {
                throw new Exception("Software XML file does not exist");
            }
        }
        private void assignArguementVariables()
        {
            XmlDocument doc = new XmlDocument();

            if (_isScheduledInstall == false)
            {
                doc.Load("Software.xml");
            }
            else
            {
                doc.Load(_opendeployerLocalPath + @"\" + _scheduledInstallGUID + ".xml");
            }

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

            if (_isScheduledInstall == false)
            {
                doc.Load("config.xml");
            }
            else
            {
                doc.Load(_opendeployerLocalPath + @"\config.xml");
            }
            
            node = doc.DocumentElement.SelectSingleNode("/Config/installerName");
            node2 = doc.DocumentElement.SelectSingleNode("/Config/SQL/username");
            node3 = doc.DocumentElement.SelectSingleNode("/Config/SQL/password");
            node4 = doc.DocumentElement.SelectSingleNode("/Config/SQL/server");

            _companyName = node.InnerText;
            _sqlusername = node2.InnerText;
            _sqlpassword = node3.InnerText;
            _sqlserver = node4.InnerText;
        }
        private void getLogo()
        {
            bool logoFileExists;

            if (_isScheduledInstall == false)
            {
                logoFileExists = File.Exists("logo.jpg");
            }
            else
            {
                logoFileExists = File.Exists(_opendeployerLocalPath + @"\" + "logo.jpg");
            }

            if (logoFileExists)
            {
                Image logo = Image.FromFile("logo.jpg", false);                
                pbLogo.Image = logo;
            }
        }
        private void getHelpText()
        {
            XmlDocument doc = new XmlDocument();
            if (_isScheduledInstall == false)
            {
                doc.Load("config.xml");
            }
            else
            {
                doc.Load(_opendeployerLocalPath + @"\config.xml");
            }

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
            string sSource = _companyName ?? "Opendeployer";
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
            string sSource = _companyName ?? "Opendeployer";
            string sLog = "Application";
            string sEvent = errorMessage;

            if (!EventLog.SourceExists(sSource))
            {
                EventLog.CreateEventSource(sSource, sLog);
                EventLog.WriteEntry(sSource, sEvent, type);
            }

            EventLog.WriteEntry(sSource, sEvent, type);
        }
        private void checkComputerIDRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer", true);
            var guidValue = key.GetValue("ComputerID");

            if (guidValue == null)
            {
                string guid = Guid.NewGuid().ToString();
                key.SetValue("ComputerID", guid, RegistryValueKind.String);

                _computerID = guid;
            }
            else
            {
                _computerID = guidValue.ToString();
            }
        }
        private void checkScheduledInstall()
        {
            string[] Args = Environment.GetCommandLineArgs();
            foreach (string Arg in Args)
            {
                if (Arg == "-install")
                {
                    _isScheduledInstall = true;                   
                    _scheduledInstallGUID = Args[2];
                }
            }
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
                writeEventLog(_applicationName + " is already installed", EventLogEntryType.Information);
                Environment.Exit(0);
            }
            else if ((int)guidValue == 0)
            {
                writeEventLog(_applicationName + " expereienced a failure when installing previously, please test on the machine manually and then remove the registry key containing the GUID for this package", EventLogEntryType.Error);
                Environment.Exit(1);
            }
            else if ((int)guidValue == 3 && _isScheduledInstall == false)
            {
                writeEventLog(_applicationName + " is scheduled for install", EventLogEntryType.Information);
                Environment.Exit(1);
            }
            else if ((int)guidValue == 4)
            {
                writeEventLog(_applicationName + " failed to install when scheduled", EventLogEntryType.Error);
                updateSQLDB("Failed to install at the scheduled time");
                updateGUIDRegistryKey(0);
                Environment.Exit(1);
            }
            else if ((int)guidValue == 5)
            {
                writeEventLog(_applicationName + " was installed when scheduled", EventLogEntryType.Information);
                updateSQLDB("Installed successfully at the scheduled time");
                updateGUIDRegistryKey(1);
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
        private void checkApplicationLocalDir(string path, bool deleteDir)
        {
            lblStatus.Text = "Status: Creating application directory";
            pbMain.Visible = true;
            Application.DoEvents();

            if (Directory.Exists(path) == true && deleteDir == true)
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }        
        private void getInstaller(string path)
        {            
            TaskbarProgress.SetValue(this.Handle, 20, 100);

            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler((sender, e) => downloadProgressBar(sender, e, pbMain, lblStatus));
                client.DownloadFileAsync(new Uri(_applicationInstallerLocation), String.Concat(path, @"\", _applicationName, ".zip"));

                while (client.IsBusy) { Application.DoEvents(); }
            }
        }
        private void extractInstaller()
        {
            TaskbarProgress.SetValue(this.Handle, 40, 100);

            bwWorkerExtractFile.RunWorkerAsync();

            while (bwWorkerExtractFile.IsBusy)
            {
                Application.DoEvents();
            }
        }
        private void askUser()
        {
            if (_isScheduledInstall == false)
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
                else if (msgBox._scheduledInstall == true)
                {
                    _scheduledInstall = msgBox._scheduledInstall;
                    _scheduledInstallDate = msgBox._scheduledInstallDate;
                    _scheduledInstallTime = msgBox._scheduledInstallTime;
                }
            }
        }
        private void runInstaller()
        {            
            TaskbarProgress.SetValue(this.Handle, 80, 100);

            bwWorkerRunInstall.RunWorkerAsync();

            while (bwWorkerRunInstall.IsBusy)
            {
                Application.DoEvents();
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
                TaskbarProgress.SetValue(this.Handle, 100, 100);

                if (_isScheduledInstall == false)
                {
                    updateGUIDRegistryKey(1);
                }
                else
                {
                    updateGUIDRegistryKey(5);
                }

                writeSQLDB("Install Successfully");
            }
            else
            {
                lblStatus.Text = "Status: Failed";
                pbMain.Value = 0;
                TaskbarProgress.SetValue(this.Handle, 100, 100);

                if (_isScheduledInstall == false)
                {
                    updateGUIDRegistryKey(0);
                }
                else
                {
                    updateGUIDRegistryKey(4);
                }

                writeSQLDB("Failed to install");             
            }         

            Complete msgBox = new Complete();
            msgBox._applicationName = _applicationName;
            msgBox._companyName = _companyName;

            formflash.FlashWindowEx(msgBox);
            msgBox.ShowDialog();

            Environment.Exit(1);
        }
        private void updateGUIDRegistryKey(int installCode)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer", true);
            key.SetValue(_applicationGuid, installCode , RegistryValueKind.DWord);
        }
        private int getInstallCode()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer");
            object installcode = (object)key.GetValue(_applicationGuid) ?? "999";

            return Convert.ToInt32(installcode);
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
        private static string getLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
        private static string getOSFriendlyName()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }
        private static void downloadProgressBar(object sender, DownloadProgressChangedEventArgs e, ProgressBar pbMain, Label lblStatus)
        {           
            lblStatus.Text = "Status: Downloading (" + (e.BytesReceived / 1000) + "/KB of " + (e.TotalBytesToReceive / 1000) + "/KB)";
            pbMain.Value = e.ProgressPercentage;
        }
        private void writeSQLDB(string msg)
        {
            ConnectionStringSettings conSettings = new ConnectionStringSettings("opendeployer", "Server=" + _sqlserver + ";Database=opendeployer;User Id=" + _sqlusername + ";Password=" + _sqlpassword + "");

            Guid applicationGuid = new Guid(_applicationGuid);
            Guid computerID = new Guid(_computerID);

            SqlConnection sqlConn = new SqlConnection(conSettings.ConnectionString);
            SqlCommand sqlComm = new SqlCommand();
            sqlComm = sqlConn.CreateCommand();
            sqlComm.CommandText = @"INSERT INTO deployments (guid, name, version, installcode, message, date, time, hostname, hostos, hostip, scheduledinstalldate, scheduledinstalltime, computerID) VALUES (@guid, @name, @version, @installcode, @message, @date, @time, @hostname, @hostos, @hostip, @scheduledinstalldate, @scheduledinstalltime, @computerID)";
            sqlComm.Parameters.Add("guid", SqlDbType.UniqueIdentifier).Value = applicationGuid;
            sqlComm.Parameters.Add("name", SqlDbType.NVarChar).Value = _applicationName;
            sqlComm.Parameters.Add("version", SqlDbType.Float).Value = _applicationVersion;
            sqlComm.Parameters.Add("installcode", SqlDbType.Int).Value = getInstallCode();
            sqlComm.Parameters.Add("message", SqlDbType.NVarChar).Value = msg;
            sqlComm.Parameters.Add("date", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
            sqlComm.Parameters.Add("time", SqlDbType.Time).Value = DateTime.Now.ToShortTimeString();
            sqlComm.Parameters.Add("hostname", SqlDbType.NVarChar).Value = Dns.GetHostName();
            sqlComm.Parameters.Add("hostos", SqlDbType.NVarChar).Value = getOSFriendlyName();
            sqlComm.Parameters.Add("hostip", SqlDbType.NVarChar).Value = getLocalIPAddress();
            sqlComm.Parameters.Add("scheduledinstalldate", SqlDbType.Date).Value = _scheduledInstallDate ?? DateTime.MinValue.ToShortDateString();
            sqlComm.Parameters.Add("scheduledinstalltime", SqlDbType.Time).Value = _scheduledInstallTime ?? DateTime.MinValue.ToShortTimeString();
            sqlComm.Parameters.Add("computerID", SqlDbType.UniqueIdentifier).Value = computerID;

            try
            {
                sqlConn.Open();
                sqlComm.ExecuteNonQuery();
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                writeEventLog(ex.Message, EventLogEntryType.Error);
            }
        }
        private void updateSQLDB(string msg)
        {
            ConnectionStringSettings conSettings = new ConnectionStringSettings("opendeployer", "Server=" + _sqlserver + ";Database=opendeployer;User Id=" + _sqlusername + ";Password=" + _sqlpassword + "");

            Guid applicationGuid = new Guid(_applicationGuid);
            Guid computerID = new Guid(_computerID);

            SqlConnection sqlConn = new SqlConnection(conSettings.ConnectionString);
            SqlCommand sqlComm = new SqlCommand();
            sqlComm = sqlConn.CreateCommand();
            sqlComm.CommandText = @"UPDATE deployments Set installcode=@installcode, message=@message WHERE guid=@guid AND computerID=@computerID";
            sqlComm.Parameters.Add("guid", SqlDbType.UniqueIdentifier).Value = applicationGuid;
            sqlComm.Parameters.Add("computerID", SqlDbType.UniqueIdentifier).Value = computerID;
            sqlComm.Parameters.Add("installcode", SqlDbType.Int).Value = getInstallCode();
            sqlComm.Parameters.Add("message", SqlDbType.NVarChar).Value = msg;         
            sqlComm.Parameters.Add("computerID", SqlDbType.UniqueIdentifier).Value = computerID;

            try
            {
                sqlConn.Open();
                sqlComm.ExecuteNonQuery();
                sqlConn.Close();
            }
            catch (Exception ex)
            {
                writeEventLog(ex.Message, EventLogEntryType.Error);
                Environment.Exit(1);
            }
        }
        private void bwWorkerExtractFile_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            using (ZipFile zip = ZipFile.Read(String.Concat(_applicationLocalLocation, @"\", _applicationName, ".zip")))
            {
                zip.ExtractProgress += extractFileExtractProgress;
                zip.ExtractAll(String.Concat(_applicationLocalLocation, @"\", _applicationName), ExtractExistingFileAction.OverwriteSilently);
            }

        }
        private void extractFileExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            _extractBytesTotal = e.TotalBytesToTransfer;
            _extractBytesTransferred = e.BytesTransferred;

            if (e.TotalBytesToTransfer != 0)
            {
                bwWorkerExtractFile.ReportProgress(Convert.ToInt32(((int)e.BytesTransferred * 100.0 / (int)e.TotalBytesToTransfer)));
            }
            else
            {
                bwWorkerExtractFile.ReportProgress(0);
            }
        }
        private void bwWorkerExtractFile_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            lblStatus.Text = "Status: Extracting (" + _extractBytesTransferred + " of " + _extractBytesTotal + ")";
            pbMain.Value = e.ProgressPercentage;
        }
        private void bwWorkerRunInstall_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            XmlDocument doc = new XmlDocument();

            if (_isScheduledInstall == false)
            {
                doc.Load("Software.xml");
            }
            else
            {
                doc.Load(_opendeployerLocalPath + @"\" + _scheduledInstallGUID + ".xml");
            }

            XmlNode nodes = doc.DocumentElement.SelectSingleNode("/Software/CommandLines");

            _commandLinesTotal = nodes.ChildNodes.Count;
            _commandLinesRan = 0;

            bwWorkerRunInstall.ReportProgress(0);

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
                _commandLinesRan += 1;
                bwWorkerRunInstall.ReportProgress(Convert.ToInt32(_commandLinesRan * 100.0 / _commandLinesTotal));
            }
        }
        private void bwWorkerRunInstall_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            lblStatus.Text = "Status: Running installer (" + _commandLinesRan + " of " + _commandLinesTotal + ")";
            pbMain.Value = e.ProgressPercentage;
        }
    }
}
