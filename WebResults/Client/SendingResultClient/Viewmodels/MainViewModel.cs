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

    }
}
