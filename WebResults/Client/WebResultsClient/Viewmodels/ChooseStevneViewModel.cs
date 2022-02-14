using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace WebResultsClient.Viewmodels
{
    public class ChooseStevneViewModel : ViewModelBase
    {
        private readonly IConfiguration m_configuration;

        public ChooseStevneViewModel(IConfiguration configuration)
        {
            m_configuration = configuration;

            m_IsSortByNameChecked = true;
            m_Competitions = new ObservableCollection<string>();
            var leonDir = configuration["LeonInstallDir"];
            if (!string.IsNullOrEmpty(leonDir))
            {
                m_selectedPath = leonDir;
                FillCompetitions(m_selectedPath);
               
            }

            var RemoteSubDir = configuration["RemoteDir"];
            m_RemoteDirs = new ObservableCollection<string>();
            if (!string.IsNullOrEmpty(RemoteSubDir))
            {
                var remoteDirsTemp = RemoteSubDir.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                m_RemoteDirs.Clear();
                foreach (var el in remoteDirsTemp)
                {
                    m_RemoteDirs.Add(el.Trim());
                }
                m_SelectedRemoteDir = m_RemoteDirs[0];
            }

            
            //m_SelectedCompetition=ConfigurationLoader.GetAppSettingsValue("LastStevne");
        }
        public delegate void StevneChange(string stevneNavn, string path);
        public event StevneChange OnStevneChange;

       

        public void OnHandleStevneChange()
        {
            m_configuration["LastStevne"] = m_SelectedCompetition;
            if (OnStevneChange == null)
            {
                return;
            }

            OnStevneChange(m_SelectedCompetition, m_selectedPath);
        }

        public delegate void RemoteDirChange(string remoteDir);
        public event RemoteDirChange OnRemoteDirChange;
        public void OnHandleRemoteDirChange()
        {
            if (OnRemoteDirChange == null)
            {
                return;
            }
            OnRemoteDirChange(m_SelectedRemoteDir);
        }
        private void FillCompetitions(string path)
        {
            m_Competitions.Clear();
            if (Directory.Exists(path))
            {
               // var listofDirs = Directory.GetDirectories(path);

                var di = new DirectoryInfo(path);
                List<string> directories;
                if (m_IsSortByNameChecked)
                {
                    directories = di.EnumerateDirectories()
                        .OrderBy(d => d.Name)
                        .Select(d => d.Name)
                        .ToList();
                }
                else
                {
                    directories = di.EnumerateDirectories()
                        .OrderBy(d => d.CreationTime)
                        .Select(d => d.Name)
                        .ToList();
                }
               
                foreach (var stevne in directories)
                {
                    var navn = Path.GetFileName(stevne);
                    m_Competitions.Add(navn);
                }

                var stevneConfig = m_configuration["LastStevne"];
                if (m_Competitions.Count > 0)
                {
                    if (!string.IsNullOrEmpty(stevneConfig))
                    {
                        if (m_Competitions.Contains(stevneConfig))
                        {
                            m_SelectedCompetition = m_Competitions[m_Competitions.IndexOf(stevneConfig)];
                            m_configuration["LastStevne"] = m_SelectedCompetition;
                        }
                    }
                    else
                    {
                        m_SelectedCompetition = m_Competitions[0];
                        m_configuration["LastStevne"] = m_SelectedCompetition;
                    } 
                   
                }
                
            }
        }

        private string m_selectedPath;

        private DelegateCommand m_openFileDialogCommand;

        public ICommand OpenFileDialogCommand
        {
            get
            {
                if (m_openFileDialogCommand == null)
                {
                    m_openFileDialogCommand = new DelegateCommand(this.OpenFileDialogExecute);
                }

                return m_openFileDialogCommand;
            }
        }

        private string m_SelectedCompetition;
        public string SelectedCompetition
        {
            get
            {
                return m_SelectedCompetition;
            }
            set
            {
                if(value!=null)
                { 
                 m_SelectedCompetition = value;
                 OnHandleStevneChange();
                }
                this.OnPropertyChanged("SelectedCompetition");
            }
        }
        private ObservableCollection<string> m_Competitions;

        public ObservableCollection<String> Competitions
        {
            get
            {
                return m_Competitions;
            }
            set
            {
                m_Competitions = value;
                this.OnPropertyChanged("Competitions");
            }
        }


        private ObservableCollection<string> m_RemoteDirs;
        public ObservableCollection<String> RemoteDirs
        {
            get
            {
                return m_RemoteDirs;
            }
            set
            {
                m_RemoteDirs = value;
                this.OnPropertyChanged("RemoteDirs");
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
                OnHandleRemoteDirChange();
                this.OnPropertyChanged("SelectedRemoteDir");
            }
        }

        
        private bool m_IsSortByNameChecked;
        public bool IsSortByNameChecked
        {
            get
            {
                return this.m_IsSortByNameChecked;
            }
            set
            {
                m_IsSortByNameChecked = value;
                OnPropertyChanged("IsSortByNameChecked");
                RefreshCompetitions();
                
            }
        }
        private bool m_IsSortByTimeChecked; 
        public bool IsSortByTimeChecked
        {
            get
            {
                return this.m_IsSortByTimeChecked;
            }
            set
            {
                m_IsSortByTimeChecked = value;
                OnPropertyChanged("IsSortByTimeChecked");
                RefreshCompetitions();

            }
        }


        public string SelectedPath
        {
            get
            {
                return this.m_selectedPath;
            }
            set
            {
                this.m_selectedPath = value;
                this.OnPropertyChanged("SelectedPath");
            }
        }

        public void OpenFileDialogExecute()
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(SelectedPath) && Directory.Exists(SelectedPath))
                {
                    dlg.SelectedPath = SelectedPath;
                }

                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    SelectedPath = dlg.SelectedPath;
                }
            }
        }

        private DelegateCommand m_RefreshCompetitionsCommand;

        public ICommand RefreshCompetitionsCommand
        {
            get
            {
                if (m_RefreshCompetitionsCommand == null)
                {
                    m_RefreshCompetitionsCommand = new DelegateCommand(this.RefreshCompetitions);
                }

                return m_RefreshCompetitionsCommand;
            }
        }

        private void RefreshCompetitions()
        {
            FillCompetitions(m_selectedPath);
            OnHandleStevneChange();
            this.OnPropertyChanged("Competitions");
            this.OnPropertyChanged("SelectedCompetition");
            OnPropertyChanged("IsSortByNameChecked");
            OnPropertyChanged("IsSortByTimeChecked");
        }
    }

}
