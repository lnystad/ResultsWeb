using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendingResultClient.Viewmodels
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            m_chooseStevneViewModel = new ChooseStevneViewModel();
            m_UpLoadStevneViewModel = new UpLoadStevneViewModel();
            m_LogViewModel = new LogViewModel();
            m_BitMapSelectionViewModel = new BitMapSelectionViewModel();
            m_UploadStevneMode = true;
            m_chooseStevneViewModel.OnStevneChange += m_UpLoadStevneViewModel.HandleStevneChange;
            m_chooseStevneViewModel.OnStevneChange += m_BitMapSelectionViewModel.HandleStevneChange;
        }
        private BitMapSelectionViewModel m_BitMapSelectionViewModel;

        public BitMapSelectionViewModel BitMapSelectionViewModel
        {
            get
            {
                return this.m_BitMapSelectionViewModel;
            }
            set
            {
                this.m_BitMapSelectionViewModel = value;
                this.OnPropertyChanged("BitMapSelectionViewModel");
            }
        }

        private LogViewModel m_LogViewModel;

        public LogViewModel LogViewModel
        {
            get
            {
                return this.m_LogViewModel;
            }
            set
            {
                this.m_LogViewModel = value;
                this.OnPropertyChanged("LogViewModel");
            }
        }

        private ChooseStevneViewModel m_chooseStevneViewModel;

       public ChooseStevneViewModel ChooseStevneViewModel
        {
            get
            {
                return this.m_chooseStevneViewModel;
            }
            set
            {
                this.m_chooseStevneViewModel = value;
                this.OnPropertyChanged("ChooseStevneViewModel");
            }
        }

        private UpLoadStevneViewModel m_UpLoadStevneViewModel;

        public UpLoadStevneViewModel UpLoadStevneViewModel
        {
            get
            {
                return this.m_UpLoadStevneViewModel;
            }
            set
            {
                this.m_UpLoadStevneViewModel = value;
                this.OnPropertyChanged("UpLoadStevneViewModel");
            }
        }
        private bool m_UploadStevneMode;

        public bool UploadStevneMode
        {
            get
            {
                return this.m_UploadStevneMode;
            }
            set
            {
                this.m_UploadStevneMode = value;
                this.OnPropertyChanged("UploadStevneMode");
            }
        }
    }
}
