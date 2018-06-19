using FileUploaderService.Configuration;
using FileUploaderService.Utils;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebResultsClient.Viewmodels
{

    public class LogViewModel : ViewModelBase
    {
        public LogViewModel()
        {
            m_LogFile = ConfigurationLoader.GetAppSettingsValue("LogFile");
            if (!File.Exists(m_LogFile))
            {
                var dir = Path.GetDirectoryName(m_LogFile);
                string appPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string filename = Path.Combine(appPath, m_LogFile);
                if (File.Exists(filename))
                {
                    m_LogFile = filename;
                }
            }

            InitCommands();
            m_canExecute = true;
        }
        private DelegateCommand m_RefreshCommand;
        public DelegateCommand RefreshCommand { get; private set; }
        private void InitCommands()
        {
            RefreshCommand = new DelegateCommand(OkExecute, OkCanExecute);

        }

        private string m_LogFile;

        private bool m_canExecute;

        private bool OkCanExecute()
        {
            return m_canExecute;
        }

        private void OkExecute()
        {
            RefreshFile();
        }
      

        private void RefreshFile()
        {
            if (!File.Exists(m_LogFile))
            {
                System.Windows.Forms.MessageBox.Show(string.Format("Fant ingen fil", m_LogFile), "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            m_canExecute = false;
            RefreshCommand.RaiseCanExecuteChanged();
            Read();
        }



        private async void Read()
        {
            LogText = await Task.Run(() => FileAccessHelper.ReadText(m_LogFile));
            m_canExecute = true;
            RefreshCommand.RaiseCanExecuteChanged();
        }

        
        private string m_LogText;
        public string LogText
        {
            get
            {
                return this.m_LogText;
            }
            set
            {
                this.m_LogText = value;
                this.OnPropertyChanged("LogText");
            }
        }
    }
}
