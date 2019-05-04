using FileUploaderService.Configuration;
using FileUploaderService.Diagnosis;
using FileUploaderService.Ftp;
using FileUploaderService.KME;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
namespace WebResultsClient.Viewmodels
{

    public class UpLoadStevneViewModel : ViewModelBase
    {
        public UpLoadStevneViewModel(string selectedcompetition, string selectedPath, string selectedRemoteDir)
        {
            m_FtpServer = ConfigurationLoader.GetAppSettingsValue("FtpServer");
            m_FtpUserName = ConfigurationLoader.GetAppSettingsValue("FtpUserName");
            m_FtpPassWord = ConfigurationLoader.GetAppSettingsValue("FtpPassWord");

            m_ProgrssbarVisibility = Visibility.Hidden;
            m_ProgrssbarBitMapVisibility = Visibility.Hidden;
            m_canExecute = false;
            m_canDeltaExecute = false;
            InitCommands();

            RapportXsltFilFileName = ConfigurationLoader.GetAppSettingsValue("RapportXsltFil");
            TopListSkyttereXsltFileName = ConfigurationLoader.GetAppSettingsValue("TopListSkyttereXsltFil");
            TopListXsltFileName = ConfigurationLoader.GetAppSettingsValue("TopListXsltFil");

            TopListLagSkyttereXsltFilFileName = ConfigurationLoader.GetAppSettingsValue("TopListLagSkyttereXsltFil");
            string debugXslt = ConfigurationLoader.GetAppSettingsValue("DebugXslt");
            if (!string.IsNullOrEmpty(debugXslt))
            {
                bool val;
                if (bool.TryParse(debugXslt, out val))
                {
                    DebugMerge = val;
                }
            }

            m_UploadBitmap = false;
            string UploadBitmapXslt = ConfigurationLoader.GetAppSettingsValue("UploadBitmap");
            if (!string.IsNullOrEmpty(UploadBitmapXslt))
            {
                bool val;
                if (bool.TryParse(UploadBitmapXslt, out val))
                {
                    m_UploadBitmap = val;
                }
            }

            m_UploadXml = true;
            string UploadXmlXslt = ConfigurationLoader.GetAppSettingsValue("UploadXml");
            if (!string.IsNullOrEmpty(UploadXmlXslt))
            {
                bool val;
                if (bool.TryParse(UploadXmlXslt, out val))
                {
                    m_UploadXml = val;
                }
            }

            m_SelectedRemoteDir = selectedRemoteDir;
            m_StevneDir = selectedPath;
            m_StevneNavn = selectedcompetition;

        }

        internal void HandleRemoteDirChange(string remoteDir)
        {
            SelectedRemoteDir = remoteDir;
        }

        public string RapportXsltFilFileName { get; set; }
        public string TopListSkyttereXsltFileName { get; set; }
        public string TopListXsltFileName { get; set; }
        public string TopListLagSkyttereXsltFilFileName { get; set; }
        public bool DebugMerge { get; set; }


        private bool m_UploadXml { get; set; }
        public bool UploadXml
        {
            get
            {
                return this.m_UploadXml;
            }
            set
            {
                this.m_UploadXml = value;

                this.OnPropertyChanged("UploadXml");
            }
        }

        private bool m_UploadBitmap { get; set; }
        public bool UploadBitmap
        {
            get
            {
                return this.m_UploadBitmap;
            }
            set
            {
                this.m_UploadBitmap = value;

                this.OnPropertyChanged("UploadBitmap");
            }
        }

        private LeonFileLoader m_fileLoader;

        public LeonFileLoader FileLoader
        {
            get
            {
                return m_fileLoader;
            }
            set { m_fileLoader = value; }
        }
        internal void HandleStevneChange(string stevneNavn, string path)
        {
            m_StevneDir = path;
            m_StevneNavn = stevneNavn;
            if (!string.IsNullOrEmpty(m_StevneNavn))
            {
                InitLeonFileLoader(DebugMerge, RapportXsltFilFileName, TopListSkyttereXsltFileName, TopListXsltFileName, TopListLagSkyttereXsltFilFileName);
                m_canExecute = true;
                m_canDeltaExecute = true;
                UploadStevneCommand.RaiseCanExecuteChanged();
                UploadStevneDeltaCommand.RaiseCanExecuteChanged();
            }
            else
            {
                m_fileLoader = null;
                m_canExecute = false;
                m_canDeltaExecute = false;
                UploadStevneCommand.RaiseCanExecuteChanged();
                UploadStevneDeltaCommand.RaiseCanExecuteChanged();
            }
        }

        private string m_SelectedRemoteDir;
        public string SelectedRemoteDir
        {
            get
            {
                return this.m_SelectedRemoteDir;
            }
            set
            {
                this.m_SelectedRemoteDir = value;

                this.OnPropertyChanged("SelectedRemoteDir");
            }
        }

        private string m_StevneDir;
        public string StevneDir
        {
            get
            {
                return this.m_StevneDir;
            }
            set
            {
                this.m_StevneDir = value;
                this.OnPropertyChanged("StevneDir");
            }
        }

        private string m_StevneNavn;
        public string StevneNavn
        {
            get
            {
                return this.m_StevneNavn;
            }
            set
            {
                this.m_StevneNavn = value;
                this.OnPropertyChanged("StevneNavn");
            }
        }

        private string m_FtpPassWord;
        public string FtpPassWord
        {
            get
            {
                return this.m_FtpPassWord;
            }
            set
            {
                this.m_FtpPassWord = value;
                this.OnPropertyChanged("FtpPassWord");
            }
        }

        private string m_FtpUserName;
        public string FtpUserName
        {
            get
            {
                return this.m_FtpUserName;
            }
            set
            {
                this.m_FtpUserName = value;
                this.OnPropertyChanged("FtpUserName");
            }
        }
        private string m_FtpServer;
        public string FtpServer
        {
            get
            {
                return this.m_FtpServer;
            }
            set
            {
                this.m_FtpServer = value;
                this.OnPropertyChanged("FtpServer");
            }
        }
        //private string m_SelectedRemotePath;
        //public string SelectedRemotePath
        //{
        //    get
        //    {
        //        return this.m_SelectedRemotePath;
        //    }
        //    set
        //    {
        //        this.m_SelectedRemotePath = value;
        //        this.OnPropertyChanged("SelectedRemotePath");
        //    }
        //}

        private string m_TextOutput;
        public string TextOutput
        {
            get
            {
                return this.m_TextOutput;
            }
            set
            {
                this.m_TextOutput = value;
                this.OnPropertyChanged("TextOutput");
            }
        }



        private DelegateCommand m_UploadStevneCommand;
        public DelegateCommand UploadStevneCommand { get; private set; }

        private DelegateCommand m_UploadStevneDeltaCommand;
        public DelegateCommand UploadStevneDeltaCommand { get; private set; }
        private void InitCommands()
        {
            UploadStevneCommand = new DelegateCommand(OkExecute, OkCanExecute);
            UploadStevneDeltaCommand = new DelegateCommand(OkDeltaExecute, OkDeltaCanExecute);
        }
        private bool m_finBitmap;
        private bool m_finXml;

        private bool m_canExecute;

        private bool OkCanExecute()
        {
            return m_canExecute;
        }

        private bool m_canDeltaExecute;

        private bool OkDeltaCanExecute()
        {
            return m_canDeltaExecute;
        }

        private void OkExecute()
        {
            UpLoadStevne(true);
        }
        private void OkDeltaExecute()
        {
            UpLoadStevne(false);
        }
        //public ICommand UploadStevneCommand
        //{
        //    get
        //    {
        //        if (m_UploadStevneCommand == null)
        //        {
        //            m_UploadStevneCommand = new DelegateCommand(this.UpLoadStevne,CanExecuteUpload);
        //        }

        //        return m_UploadStevneCommand;
        //    }
        //}
        //
        //private bool CanExecuteUpload()
        //{
        //    return m_canExecute;
        //}

        private void UpLoadStevne(bool fullUpload)
        {

            if (string.IsNullOrEmpty(m_StevneDir))
            {
                System.Windows.Forms.MessageBox.Show("Stevne ikke valgt", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            m_canExecute = false;
            m_canDeltaExecute = false;
            UploadStevneCommand.RaiseCanExecuteChanged();
            UploadStevneDeltaCommand.RaiseCanExecuteChanged();
            m_finBitmap = false;
            m_finXml = false;

            TextOutput = "";

            FtpUtility test = new FtpUtility(true, m_FtpServer, "", "", m_FtpUserName, m_FtpPassWord);
            test.OnLogEvent += LogEvent;
            test.OnHandleFileFinishedEvent += HandleFileFinishedEvent;

            var dir = Path.Combine(m_StevneDir, m_StevneNavn);
            DirectoryInfo info = new DirectoryInfo(dir);
            LeonDirInfo dirInfo = new LeonDirInfo(info);
            var BitMaps = dirInfo.ListBitMapByRange(dir);
            if (BitMaps != null && UploadBitmap)
            {
                sendBitMapsAsync(fullUpload, BitMaps, test);
            }

            string webDir = Path.Combine(m_StevneDir, m_StevneNavn);
            webDir = Path.Combine(webDir, "Web");
            if (Directory.Exists(webDir))
            {
                var files = Directory.GetFiles(webDir);
                if (files.Length <= 0)
                {
                    TextOutput = "No File to send ";
                    return;
                }
                SetBitmapLinks();
                if (UploadXml)
                {
                    send(fullUpload, files, test);
                }
                else
                {
                    TextOutput = "UploadXml False No Xml Uploaded Just generated new version";
                    Log.Info("UploadXml False No Xml Uploaded generated new version");
                    FinishedLoading("XML");
                }
                //test.UploadFiles(true, this.m_SelectedRemoteSubDir, m_StevneNavn, files);
            }
            else
            {
                TextOutput = "No File to send ";
            }

        }

        private async void sendBitMapsAsync(bool full, List<BitmapDirInfo> bitMaps, FtpUtility util)
        {


            foreach (var bitmapofRane in bitMaps)
            {
                if (bitmapofRane.BitmapFiles.Count > 0)
                {
                    List<string> bitlist = new List<string>();
                    foreach (var fili in bitmapofRane.BitmapFiles)
                    {
                        bitlist.Add(fili.FullName);
                    }
                    string[] list = bitlist.ToArray();
                    PercentBitMap = 0;
                    ProgrssbarBitMapVisibility = Visibility.Visible;
                    var t = await Task.Run(() => util.UploadFiles(full, "BITMAP", this.m_SelectedRemoteDir, m_StevneNavn, list, bitmapofRane.BitmapSubDir));
                    ProgrssbarBitMapVisibility = Visibility.Hidden;
                    PercentBitMap = 0;
                }
            }

            FinishedLoading("BITMAP");
        }

        private void FinishedLoading(string v)
        {
            if (UploadBitmap)
            {
                if (string.Compare(v, "BITMAP") == 0)
                {
                    m_finBitmap = true;

                }
                else
                {
                    m_finXml = true;
                }
            }
            else
            {
                m_finBitmap = true;
                m_finXml = true;
            }

            if (m_finBitmap && m_finXml)
            {
                m_canExecute = true;
                m_canDeltaExecute = true;
                UploadStevneCommand.RaiseCanExecuteChanged();
                UploadStevneDeltaCommand.RaiseCanExecuteChanged();
            }
        }

        private async void send(bool full, string[] list, FtpUtility util)
        {
            ProgrssbarVisibility = Visibility.Visible;
            var t = await Task.Run(() => util.UploadFiles(full, "XML", this.m_SelectedRemoteDir, m_StevneNavn, list));
            ProgrssbarVisibility = Visibility.Hidden;
            Percent = 0;

            FinishedLoading("XML");
        }
        private async void SetBitmapLinksReferenceAsyc()
        {
            var t = await Task.Run(() => SetBitmapLinks());
        }

        private Visibility m_ProgrssbarVisibility;
        public Visibility ProgrssbarVisibility
        {
            get { return m_ProgrssbarVisibility; }
            set
            {
                this.m_ProgrssbarVisibility = value;
                this.OnPropertyChanged("ProgrssbarVisibility");
            }
        }

        private int percent = 0;
        public int Percent
        {
            get { return this.percent; }
            set
            {
                this.percent = value;
                this.OnPropertyChanged("Percent");
            }
        }

        private Visibility m_ProgrssbarBitMapVisibility;
        public Visibility ProgrssbarBitMapVisibility
        {
            get { return m_ProgrssbarBitMapVisibility; }
            set
            {
                this.m_ProgrssbarBitMapVisibility = value;
                this.OnPropertyChanged("ProgrssbarBitMapVisibility");
            }
        }

        private int percentBitMap = 0;
        public int PercentBitMap
        {
            get { return this.percentBitMap; }
            set
            {
                this.percentBitMap = value;
                this.OnPropertyChanged("PercentBitMap");
            }
        }

        private bool HandleFileFinishedEvent(string type, string message, int count, int total)
        {
            if (string.Compare(type, "BITMAP") == 0)
            {
                PercentBitMap = (int)Math.Round((double)(100 * count) / total);
            }
            else
            {
                Percent = (int)Math.Round((double)(100 * count) / total);
            }


            return true;
        }

        private DelegateCommand m_OpenRemoteDirCommand;

        public ICommand OpenRemoteDirCommand
        {
            get
            {
                if (m_OpenRemoteDirCommand == null)
                {
                    m_OpenRemoteDirCommand = new DelegateCommand(this.OpenChooseRemoteDir);
                }

                return m_OpenRemoteDirCommand;
            }
        }

        public void OpenChooseRemoteDir()
        {
            //m_FtpServer = Configur
            //m_FtpUserName = Config
            //m_FtpPassWord = Config
            //m_SelectedRemoteSubDir

            FtpUtility test = new FtpUtility(true, m_FtpServer, "", "", m_FtpUserName, m_FtpPassWord);
            test.OnLogEvent += LogEvent;
            var list = test.ListDirectories();


        }

        private bool LogEvent(string message)
        {
            m_TextOutput = m_TextOutput + "\r\n" + message;
            this.OnPropertyChanged("TextOutput");
            return true;
        }



        public void InitLeonFileLoader(bool debugXslt, string rapportXsltFile, string topListSkyttere, string TopListXslt, string topListLagSkyttere)
        {
            string RapportXsltFilFileName = rapportXsltFile;

            string TopListSkyttereXsltFileName = topListSkyttere;
            string TopListXsltFileName = TopListXslt;

            string TopListLagSkyttereXsltFilFileName = topListLagSkyttere;

            bool debugMerge = debugXslt;


            this.m_fileLoader = new LeonFileLoader(m_StevneDir);
            this.m_fileLoader.DebugMergedXml = debugMerge;
            if (!string.IsNullOrEmpty(RapportXsltFilFileName))
            {
                this.m_fileLoader.InitRapportTransform(RapportXsltFilFileName);
            }

            if (!string.IsNullOrEmpty(RapportXsltFilFileName) && !string.IsNullOrEmpty(TopListSkyttereXsltFileName))
            {
                this.m_fileLoader.InitToppListInfoTransform(TopListXsltFileName, TopListSkyttereXsltFileName);
            }

            if (!string.IsNullOrEmpty(TopListLagSkyttereXsltFilFileName))
            {
                this.m_fileLoader.InitToppListLagSkytingTransform(TopListLagSkyttereXsltFilFileName);
            }
        }

        public bool SetBitmapLinks()
        {
            var dir = Path.Combine(m_StevneDir, m_StevneNavn);
            DirectoryInfo info = new DirectoryInfo(dir);
            LeonDirInfo dirInfo = new LeonDirInfo(info);
            dirInfo.UpdateWebFiles(true);

            if (!m_UploadBitmap)
            {
                Log.Info("UploadBitmap false no bitmap reference generation");

            }
            else
            {
                Log.Info("UploadBitmap true Start setting Bitmap reference");
                dirInfo.CheckBitMap();
                LogEvent("Start setting Bitmap reference");
            }

            if (m_fileLoader.GenerateNewReports(dirInfo, true, true))
            {
                Log.Info("Updated Reports Detected name");
                LogEvent("pdated Reports Detected name");
            }
            if (m_UploadBitmap)
            {
                Log.Info("Finished setting Bitmap reference");
                LogEvent("Finished setting Bitmap reference");
            }
            return true;
        }
    }
}
