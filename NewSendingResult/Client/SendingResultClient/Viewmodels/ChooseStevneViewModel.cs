using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SendingResultClient.Viewmodels
{
    public class ChooseStevneViewModel : ViewModelBase
    {
       
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
        }
    
}
