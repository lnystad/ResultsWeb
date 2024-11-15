using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using WebResultsClient.Commands;
using Microsoft.VisualBasic.ApplicationServices;
using WebResultsClient.Premieberegning;

namespace WebResultsClient.Viewmodels
{
    

    public class StevneoppgjorSelectionViewModel : ViewModelBase
    {
        public class PremieBeskrivelse
        {
            public string XMLTag { get; set; }
            public string Ovelse { get; set; }
        }

        private readonly PremieBeskrivelse _settings;


        private IConfiguration m_configuration;

        public StevneoppgjorSelectionViewModel(IConfiguration configuration)
        {
            m_configuration = configuration;

            // Klasse "NU", "Åpen" har ikke premieavgift???
            var seniorKlasser = configuration.GetSection("SeniorKlasser");
            SeniorKlasser = new ObservableCollection<string>();
            foreach (var klasse in seniorKlasser.GetChildren())
            {
                SeniorKlasser.Add(klasse.Value);
            }

            var ungdomsKlasser = configuration.GetSection("UngdomsKlasser");
            UngdomsKlasser = new ObservableCollection<string>();
            foreach (var klasse in ungdomsKlasser.GetChildren())
            {
                UngdomsKlasser.Add(klasse.Value);
            }

            SeniorPremieavgift = configuration["LastPremieavgiftSenior"];
            UngdomPremieavgift = configuration["LastPremieavgiftUngdom"]; ;
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
       
        private string m_seniorSummary;
        public string SeniorSummary
        {
            get
            {
                return m_seniorSummary;
            }
            set
            {
                m_seniorSummary = value;
                OnPropertyChanged(nameof(SeniorSummary));
            }
        }
        private string m_juniorSummary;
        public string JuniorSummary
        {
            get
            {
                return m_juniorSummary;
            }
            set
            {
                m_juniorSummary = value;
                OnPropertyChanged(nameof(JuniorSummary));
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
                    m_configuration["LastPremieavgiftSenior"] = SeniorPremieavgift;

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
                    m_configuration["LastPremieavgiftUngdom"] = UngdomPremieavgift;

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

