using Microsoft.Extensions.Configuration;
using System;

namespace WebResultsClient.Viewmodels
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            m_chooseStevneViewModel = new ChooseStevneViewModel(configuration);
            m_UpLoadStevneViewModel = new UpLoadStevneViewModel(configuration, m_chooseStevneViewModel.SelectedCompetition,
                                                                m_chooseStevneViewModel.SelectedPath,
                                                                m_chooseStevneViewModel.SelectedRemoteDir);
           
            m_LogViewModel = new LogViewModel(configuration);
            m_BitMapSelectionViewModel = new BitMapSelectionViewModel(configuration, m_chooseStevneViewModel.SelectedCompetition,
                                                                      m_chooseStevneViewModel.SelectedPath);
            m_StevneoppgjorSelectionViewModel = new StevneoppgjorSelectionViewModel();
            m_chooseStevneViewModel.OnStevneChange += m_UpLoadStevneViewModel.HandleStevneChange;
            m_chooseStevneViewModel.OnStevneChange += m_BitMapSelectionViewModel.HandleStevneChange;
            m_chooseStevneViewModel.OnStevneChange += m_StevneoppgjorSelectionViewModel.HandleStevneChange;
            m_chooseStevneViewModel.OnRemoteDirChange += m_UpLoadStevneViewModel.HandleRemoteDirChange;
            m_UpLoadStevneViewModel.SelectedRemoteDir = m_chooseStevneViewModel.SelectedRemoteDir;
            m_chooseStevneViewModel.OnHandleStevneChange();
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

        private StevneoppgjorSelectionViewModel m_StevneoppgjorSelectionViewModel;
        public StevneoppgjorSelectionViewModel StevneoppgjorSelectionViewModel
        {
            get
            {
                return this.m_StevneoppgjorSelectionViewModel;
            }
            set
            {
                this.m_StevneoppgjorSelectionViewModel = value;
                this.OnPropertyChanged(nameof(StevneoppgjorSelectionViewModel));
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
    }
}
