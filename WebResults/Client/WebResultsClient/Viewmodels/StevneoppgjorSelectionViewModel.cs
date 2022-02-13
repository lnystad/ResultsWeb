using FileUploaderService;
using FileUploaderService.Configuration;
using FileUploaderService.Utils;
using Microsoft.Practices.Prism.Commands;
using WebResultsClient.Viewmodels.BitMap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Input;
using WebResultsClient.Commands;

namespace WebResultsClient.Viewmodels
{

    public class StevneoppgjorSelectionViewModel : ViewModelBase
    {
        public StevneoppgjorSelectionViewModel()
        {
            SeniorKlasser = new ObservableCollection<string> { "1", "2", "3", "4", "5", "V55", "V65", "V73", "KIK" };
            UngdomsKlasser = new ObservableCollection<string> { "R", "ER", "J", "EJ" };

            SeniorPremieavgift = "70";
            UngdomPremieavgift = "50";
            GenererStevneoppgjorCommand = new GenererStevneoppgjorCommand(this);
        }

        public ICommand GenererStevneoppgjorCommand
        {
            get; set;
        }

        private ObservableCollection<string> m_seniorKlasser = new ObservableCollection<string>();
        public ObservableCollection<string> SeniorKlasser { 
            get
            {
                return m_seniorKlasser;
            }
            set
            {
                m_seniorKlasser = value;
                OnPropertyChanged(nameof(SeniorKlasser));
            }
        }

        private int m_seniorPremieavgift;
        
        public string SeniorPremieavgift
        {
            get { return m_seniorPremieavgift.ToString(); }
            set
            {
                if(int.TryParse(value, out int resultat))
                {
                    m_seniorPremieavgift = resultat;
                    OnPropertyChanged(nameof(SeniorPremieavgift));
                }
            }
        }

        private int m_ungdomPremieavgift;

        public string UngdomPremieavgift
        {
            get { return m_ungdomPremieavgift.ToString(); }
            set
            {
                if (int.TryParse(value, out int resultat))
                {
                    m_ungdomPremieavgift = resultat;
                    OnPropertyChanged(nameof(UngdomPremieavgift));
                }
            }
        }

        private ObservableCollection<string> m_ungdomsKlasser = new ObservableCollection<string>();
        public ObservableCollection<string> UngdomsKlasser
        {
            get
            {
                return m_ungdomsKlasser;
            }
            set
            {
                m_ungdomsKlasser = value;
                OnPropertyChanged(nameof(UngdomsKlasser));
            }
        }

        internal void HandleStevneChange(string stevneNavn, string path)
        {
            m_StevneDir = path;
            m_StevneNavn = stevneNavn;
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
    }
}

