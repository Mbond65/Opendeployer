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
using System.Security.Principal;
using System.ServiceProcess;
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
        private string _deldir;
        private long _extractBytesTransferred;
        private long _extractBytesTotal;
        private int _commandLinesTotal;
        private int _commandLinesRan;
        private int _opendeployerVersion = 2;
        private string _opendeployerLocalPath = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"\Opendeployer\");

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                checkDotNetVersion();
                checkAdministrativePermissions();
                checkScheduledInstall();
                checkXMLFile();
                assignArguementVariables();
                getLogo();
                getHelpText();
                setDetailLabels();                
                checkOpenDeployerRegistry();
                checkMultiInstall();
                checkOpenDeployerVersionRegistry();
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

        /// <summary>
        /// Calls the methods neccessary to do an install.
        /// </summary>
        private void installNow()
        {
            checkApplicationLocalDir(_applicationLocalLocation, true);
            getInstaller(_applicationLocalLocation);
            extractInstaller();
            runInstaller();
            deleteInstaller();
            writeEventLog(_applicationName + " installed successfully", EventLogEntryType.Information);
            notifyUserComplete();
        }

        /// <summary>
        /// Calls the methods necessary to schedule an install.
        /// </summary>
        private void installScheduledDate()
        {
            checkTaskSchedulerRunning();
            checkApplicationLocalDir(_opendeployerLocalPath, false);
            checkApplicationLocalDir(_applicationLocalLocation, true);
            getInstaller(_applicationLocalLocation);
            getScheduledInstallDate();
            copyOpenDeployerFiles();
            createTask();
            updateGUIDRegistryKey(3);
            writeEventLog(_applicationName + " is scheduled for install", EventLogEntryType.Information);
            writeSQLDB("Awaiting install");
            notifyScheduledTaskComplete();

            Environment.Exit(0);
        }

        /// <summary>
        /// Calls the methods necessary to do a scheduled install.
        /// </summary>
        private void doScheduledInstall()
        {
            notifyUserInstallReady();
            extractInstaller();
            runInstaller();
            deleteInstaller();
            writeEventLog(_applicationName + " installed successfully", EventLogEntryType.Information);
            notifyUserComplete();
        }

        /// <summary>
        /// Notifies the user a scheduled install is about to take place.
        /// </summary>
        private void notifyUserInstallReady()
        {
            msgboxLogo box = new msgboxLogo();
            box._message = _applicationName + " will now install";

            formflash.FlashWindowEx(box);

            box.Show();
        }
       
        /// <summary>
        /// Notifies the user a scheduled install has completed.
        /// </summary>
        private void notifyScheduledTaskComplete()
        {
            msgboxLogo msgbox = new msgboxLogo();
            msgbox._message = "Download complete, the installer will run at the time and date you specified. If your computer isn't switched on at the time of install , the installer will run whenever possible.";

            formflash.FlashWindowEx(msgbox);

            msgbox.ShowDialog();
        }

        /// <summary>
        /// Copies binaries and config files to machine ready for a scheduled install.
        /// </summary>
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

        /// <summary>
        /// Creates a scheduled task.
        /// </summary>
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
                td.Settings.DeleteExpiredTaskAfter = new TimeSpan(1, 0, 0);

                td.Principal.RunLevel = TaskRunLevel.Highest;

                td.Triggers.Add(new TimeTrigger { Enabled = true,
                                                  StartBoundary = Convert.ToDateTime(_scheduledInstallDate).Add(TimeSpan.Parse(_scheduledInstallTime)) , EndBoundary = DateTime.Now.AddMonths(1) });

                td.Actions.Add(new ExecAction(_opendeployerLocalPath + "opendeployer.exe", "-install " + _applicationGuid, null));

                ts.RootFolder.RegisterTaskDefinition("Opendeployer - " + _applicationGuid, td);

            }
        }

        /// <summary>
        /// Checks Software.xml exists if isScheduledInstall is false.
        /// </summary>
        private void checkXMLFile()
        {
            bool fileExists = File.Exists("Software.xml");
            if (!fileExists && _isScheduledInstall == false)
            {
                throw new Exception("Software XML file does not exist");
            }
        }

        /// <summary>
        /// Get variables from XML config files.
        /// </summary>
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

        /// <summary>
        /// Gets logo file from source and assigns to image control if exists.
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
        /// Gets help text from config file and add it to a label control if exists.
        /// </summary>
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

        /// <summary>
        /// Set values for detail labels.
        /// </summary>
        private void setDetailLabels()
        {
            lblApplicationName.Text = "Application package: " + _applicationName;
            lblApplicationVersion.Text = "Version: " + _applicationVersion;
        }

        /// <summary>
        /// Writes to Application event log.
        /// </summary>
        /// <param name="errorMessage"></param>
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

        /// <summary>
        /// Writes to Application event log.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <param name="type"></param>
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

        /// <summary>
        /// Checks the machine the application is running on meets the .Net 4.5 requirement.
        /// </summary>
        /// <returns></returns>
        private void checkDotNetVersion()
        {
            if (checkDotNet45() == false)
            {
                throw new Exception("Opendeployer requires .Net 4.5 or higher, this computer does not meet the requirement.");
            }
        }     

        /// <summary>
        /// Checks the application hasn't already been install on another machine.
        /// </summary>
        private void checkMultiInstall()
        {
            if (_isScheduledInstall != true)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("config.xml");

                XmlNode node = doc.DocumentElement.SelectSingleNode("/Config/preventMultiInstall");

                if (node.InnerText == "true")
                {
                    try
                    {
                        if (checkInstallSQLDB() == true)
                        {
                            throw new Exception("Application has already been installed on another computer with the same user name");
                        }
                    }
                    catch (Exception ex)
                    {
                        writeEventLog(ex.Message, EventLogEntryType.Error);
                    }                   
                }
            }
        }

        /// <summary>
        /// Checks the account running the application is in the local administrator security group.
        /// </summary>
        private void checkAdministrativePermissions()
        {
            bool isUserAdmin = IsUserAdministrator();

            if (isUserAdmin == false)
            {
                writeEventLog("Opendeployer requires local admins permissions to function, please run as administrator", EventLogEntryType.Error);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Checks a GUID for the computer it's running on exists in the registry, if not it creates and stores one.
        /// </summary>
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

        /// <summary>
        /// Checks if scheduled install was called.
        /// </summary>
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

        /// <summary>
        /// Checks that Opendeployer registry key exists, if not then create one.
        /// </summary>
        private void checkOpenDeployerRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer");
            if (key == null)
            {
                RegistryKey software = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true);
                software.CreateSubKey("Opendeployer");
            }
        }

        /// <summary>
        /// Create opendeployer version registry value.
        /// </summary>
        private void checkOpenDeployerVersionRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer", true);

            var versionValue = key.GetValue("Client Version");
            key.SetValue("Client Version", _opendeployerVersion, RegistryValueKind.DWord);            
        }

        /// <summary>
        /// Checks to see the if the deployment has already run and if it has what installcode it returned.
        /// </summary>
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

        /// <summary>
        /// Iterates through the Wow6432node of the uninstall values to ensure the application isn't already installed.
        /// </summary>
        private void checkApplicationNotInstalled()
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

                        if ((displayName.ToLower() == _applicationName.ToLower()) && ((version ?? "notinstalled").ToString() == Convert.ToString(_applicationVersion)))
                        {
                            writeEventLog("Application already installed", EventLogEntryType.Information);
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Iterates through the registry uninstall values to ensure the application isn't already installed.
        /// </summary>
        private void checkApplicationNotInstalled64()
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

                        if ((displayName.ToLower() == _applicationName.ToLower()) && ((version ?? "notinstalled").ToString() == Convert.ToString(_applicationVersion)))
                        {
                            writeEventLog("Application already installed", EventLogEntryType.Information);
                            Environment.Exit(0);
                        }                     
                    }
                }
            }     
        }

        /// <summary>
        /// Create application local directory.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="deleteDir"></param>
        private void checkApplicationLocalDir(string path, bool deleteDir)
        {
            lblStatus.Text = "Status: Creating application directory";
            pbMain.Visible = true;
            Application.DoEvents();

            if (Directory.Exists(path) == true && deleteDir == true)
            {
                deleteDirectory(path);
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }        

        /// <summary>
        /// Downloads the installer.
        /// </summary>
        /// <param name="path"></param>
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

        /// <summary>
        /// Extracts the downloaded installer.
        /// </summary>
        private void extractInstaller()
        {
            TaskbarProgress.SetValue(this.Handle, 40, 100);

            bwWorkerExtractFile.RunWorkerAsync();

            while (bwWorkerExtractFile.IsBusy)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Asks the user whether they would like to proceed before the install takes place.
        /// </summary>
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
                }
            }
        }

        /// <summary>
        /// For each of the command lines found in the XML file, waits for processes to finish.
        /// </summary>
        private void runInstaller()
        {            
            TaskbarProgress.SetValue(this.Handle, 80, 100);

            bwWorkerRunInstall.RunWorkerAsync();

            while (bwWorkerRunInstall.IsBusy)
            {
                Application.DoEvents();
            }
            
        }

        /// <summary>
        /// Deletes the local copy of the installer.
        /// </summary>
        /// 
        private void deleteInstaller()
        {
            lblStatus.Text = "Status: Deleting installer";

            deleteDirectory(_applicationLocalLocation);
        }

        /// <summary>
        /// Notifies the user the process has completed, write to the event log and alter registry key depending on result.
        /// </summary>
        private void notifyUserComplete()
        {
            pbLoading.Visible = false;

            Complete msgBox = new Complete();
            msgBox._applicationName = _applicationName;
            msgBox._companyName = _companyName;

            if ((checkApplicationInstalled() == true) || (checkApplicationInstalled64() == true))
            {
                msgBox._installedSuccessfully = true;
                lblStatus.Text = "Status: Complete";
                pbMain.Value = 100;
                TaskbarProgress.SetValue(this.Handle, 100, 100);

                if (_isScheduledInstall == false)
                {
                    updateGUIDRegistryKey(1);
                }
                else if (_isScheduledInstall == true)
                {
                    updateGUIDRegistryKey(5);
                }

                if (_isScheduledInstall == false)
                {
                    writeSQLDB("Install Successfully");
                }         
                
            }
            else
            {
                msgBox._installedSuccessfully = false;
                lblStatus.Text = "Status: Failed";
                pbMain.Value = 0;
                TaskbarProgress.SetValue(this.Handle, 100, 100);

         
                if (_isScheduledInstall == false)
                {
                    updateGUIDRegistryKey(0);
                }
                else if (_isScheduledInstall == true)
                {
                    updateGUIDRegistryKey(4);
                }

                if (_isScheduledInstall == false)
                {
                    writeSQLDB("Failed to install");
                }
          
            }

            formflash.FlashWindowEx(msgBox);

            msgBox.ShowDialog();

            Environment.Exit(1);
        }

        /// <summary>
        /// Updates the deployment registry key.
        /// </summary>
        /// <param name="installCode"></param>
        private void updateGUIDRegistryKey(int installCode)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer", true);
            key.SetValue(_applicationGuid, installCode , RegistryValueKind.DWord);
        }

        /// <summary>
        /// Gets installer installcode from the Opendeployer key in the registry.
        /// </summary>
        /// <returns></returns>
        private int getInstallCode()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Opendeployer");
            object installcode = (object)key.GetValue(_applicationGuid) ?? "999";

            return Convert.ToInt32(installcode);
        }

        /// <summary>
        /// Verifies the application has been installed.
        /// </summary>
        /// <returns></returns>
        private bool checkApplicationInstalled()
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

        /// <summary>
        /// Verifies the application has been installed.
        /// </summary>
        /// <returns></returns>
        private bool checkApplicationInstalled64()
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

        /// <summary>
        /// Checks user running the application is a local administrator.
        /// </summary>
        /// <returns></returns>
        private bool IsUserAdministrator()
        {
            bool isAdmin;
            WindowsIdentity user = null;
            try
            {
                user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }
            return isAdmin;
        }

        /// <summary>
        /// Gets local IP address of the machine running the application.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Prompts user for time and date when they would like the install to happen.
        /// </summary>
        private void getScheduledInstallDate()
        {
            datepicker dp = new datepicker();
            formflash.FlashWindowEx(dp);

            dp.ShowDialog();
            _scheduledInstallDate = dp._scheduledInstallDate;
            _scheduledInstallTime = dp._scheduledInstallTime;

        }

        /// <summary>
        /// Gets hostname of the machine running the application.
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// Writes to deployment table.
        /// </summary>
        /// <param name="msg"></param>
        private void writeSQLDB(string msg)
        {
            ConnectionStringSettings conSettings = new ConnectionStringSettings("opendeployer", "Server=" + _sqlserver + ";Database=opendeployer;User Id=" + _sqlusername + ";Password=" + _sqlpassword + "");

            Guid applicationGuid = new Guid(_applicationGuid);
            Guid computerID = new Guid(_computerID);

            SqlConnection sqlConn = new SqlConnection(conSettings.ConnectionString);
            SqlCommand sqlComm = new SqlCommand();
            sqlComm = sqlConn.CreateCommand();
            sqlComm.CommandText = @"INSERT INTO deployments (guid, name, version, installcode, message, date, time, hostname, hostos, hostip, scheduledinstalldate, scheduledinstalltime, computerID, username) VALUES (@guid, @name, @version, @installcode, @message, @date, @time, @hostname, @hostos, @hostip, @scheduledinstalldate, @scheduledinstalltime, @computerID, @username)";
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
            sqlComm.Parameters.Add("username", SqlDbType.NVarChar).Value = Environment.UserName;

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

        /// <summary>
        /// Updates deployment table.
        /// </summary>
        /// <param name="msg"></param>
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

        /// <summary>
        /// Checks Task Scheduler service is running.
        /// </summary>
        private void checkTaskSchedulerRunning()
        {
            ServiceController sc = new ServiceController("Schedule");


            if (sc.Status != ServiceControllerStatus.Running)
            {
                throw new Exception("Task Scheduler is not running");
            }
        }

        /// <summary>
        /// Checks application not already installed for user.
        /// </summary>
        /// <returns></returns>
        private bool checkInstallSQLDB()
        {

            ConnectionStringSettings conSettings = new ConnectionStringSettings("opendeployer", "Server=" + _sqlserver + ";Database=opendeployer;User Id=" + _sqlusername + ";Password=" + _sqlpassword + "");

            using (SqlConnection sqlConn = new SqlConnection(conSettings.ConnectionString))
            {
                sqlConn.Open();

                using (SqlCommand sqlComm = new SqlCommand("SELECT * FROM  Deployments WHERE guid=@guid AND username=@username", sqlConn))
                {
                    Guid applicationGuid = new Guid(_applicationGuid);
                    sqlComm.Parameters.Add("guid", SqlDbType.UniqueIdentifier).Value = applicationGuid;
                    sqlComm.Parameters.Add("username", SqlDbType.NVarChar).Value = Environment.UserName;

                    using (SqlDataReader rdr = sqlComm.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            if (rdr.HasRows == true)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
              
            return false;
        }

        /// <summary>
        /// Delete files and folder in specified directory.
        /// </summary>
        /// <param name="target_dir"></param>
        private void deleteDirectory(string target_dir)
        {
            _deldir = target_dir;
            bwWorkerDeleteDirectory.RunWorkerAsync();
                
            while (bwWorkerDeleteDirectory.IsBusy)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Deletes directory recursively, used by backgroundworker.
        /// </summary>
        /// <param name="target_dir"></param>
        private void deleteDirectoryRecursive(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                deleteDirectoryRecursive(dir);
            }

            Directory.Delete(target_dir, false);
        }

        /// <summary>
        /// Checks the machine the application is running on meets the .Net 4.5 requirement.
        /// </summary>
        /// <returns></returns>
        private static bool checkDotNet45()
        {
            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                int releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                if ((releaseKey >= 378389))
                {
                    return true;
                }
            }
            
            return false;
        }

        private static void downloadProgressBar(object sender, DownloadProgressChangedEventArgs e, ProgressBar pbMain, Label lblStatus)
        {
            lblStatus.Text = "Status: Downloading (" + (e.BytesReceived / 1000) + "/KB of " + (e.TotalBytesToReceive / 1000) + "/KB)";
            pbMain.Value = e.ProgressPercentage;
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
                startInfo.Verb = "runas";

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

        private void bwWorkerRunInstall_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
        }

        private void bwWorkerExtractFile_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
        }

        private void bwWorkerDeleteDirectory_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string target_dir = _deldir;
            deleteDirectoryRecursive(target_dir);            
        }

        private void bwWorkerDeleteDirectory_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            lblStatus.Text = "Status: Deleting installer";
            pbMain.Value = e.ProgressPercentage;
        }

        private void bwWorkerDeleteDirectory_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
        }
    }
}
