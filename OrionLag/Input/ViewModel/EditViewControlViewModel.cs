using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Windows;

namespace OrionLag.Input.ViewModel
{
    using OrionLag.Input.Data;
    using OrionLag.Utils;
    using System.IO;

    public class EditViewControlViewModel : INotifyPropertyChanged
    {
        public EditViewControlViewModel()
        {
            m_lagKilde = new ObservableCollection<Lag>();
            m_finfeltRows = new ObservableCollection<InputData>();
            m_grovfeltRows = new ObservableCollection<InputData>();
            m_bane200mFileRows = new ObservableCollection<InputData>();
            m_skiver = new ObservableCollection<SkiverViewModel>();
        }




        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<OrionLag.Utils.Lag> m_lagKilde;
        public ObservableCollection<OrionLag.Utils.Lag> LagKilde
        {
            get { return m_lagKilde; }
            set
            {
                m_lagKilde = value;


                NotifyPropertyChanged("LagKilde");
            }
        }

        private SkiverViewModel m_selectedSkive;
        public SkiverViewModel SelectedSkive
        {
            get { return m_selectedSkive; }
            set
            {
                m_selectedSkive = value;
                NotifyPropertyChanged("SelectedSkive");
            }
        }

        private OrionLag.Utils.Lag m_selectedLag2;
        public OrionLag.Utils.Lag SelectedLag2
        {
            get { return m_selectedLag2; }
            set
            {
                m_selectedLag2 = value;
                if (m_selectedLag2 != null)
                {
                    m_selectedSkive = Skiver.FirstOrDefault(x => x.LagNummer == m_selectedLag2.LagNummer);

                    NotifyPropertyChanged("SelectedSkive");
                }

                NotifyPropertyChanged("SelectedLag2");
            }
        }

        private ObservableCollection<InputData> m_finfeltRows;

        public ObservableCollection<InputData> FinfeltRows
        {
            get { return m_finfeltRows; }
            set
            {
                m_finfeltRows = value;
                NotifyPropertyChanged("FinfeltRows");
            }
        }

        internal void CheckTotalButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
            string errormsg = string.Empty;
            List<InputData> bane = new List<InputData>();
            List<InputData> Felt = new List<InputData>();
            if(m_finfeltRows.Count > 0)
                {
                Felt.AddRange(m_finfeltRows.ToList());
               }

            if (m_grovfeltRows.Count > 0)
            {
                Felt.AddRange(m_grovfeltRows.ToList());
            }

            if (m_bane100mFileRows.Count > 0)
            {
                bane.AddRange(m_bane100mFileRows.ToList());
            }

            if (m_bane200mFileRows.Count > 0)
            {
                bane.AddRange(m_bane200mFileRows.ToList());
            }

            if (bane.Count > 0 || Felt.Count > 0)
            {
                             
                foreach(var element in bane)
                {
                    var sk = Felt.FirstOrDefault(x => x.SkytterNr == element.SkytterNr);
                    if(sk == null)
                    {
                        errormsg = errormsg + string.Format("Skytterno {0} kl={1} kun bane\r", element.SkytterNr,element.Klasse);
                    }
                    else
                    {
                        Felt.Remove(sk);
                    }
                }

                if(Felt.Count>0)
                {
                    foreach(var fel in Felt)
                    {
                        errormsg = errormsg + string.Format("Skytterno {0}  kl={1} kun felt \r", fel.SkytterNr, fel.Klasse);
                    }
                }
            }

            ErrorMsg = errormsg;
        }

        public string FinfeltFile { get; set; }
        internal void FinFeltButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FinfeltFile))
            {
                return;
            }
            string path = Path.GetDirectoryName(FinfeltFile);
            string filename = Path.GetFileName(FinfeltFile);
            var input = InputXmlFileParser.ParseXmlFile(path, filename);

            m_finfeltRows = new ObservableCollection<InputData>();
            m_skiver.Clear();
            foreach (var element in input)
            {
                m_finfeltRows.Add(element);
                Skiver sk = new Utils.Skiver();
                sk.SkiveNummer = element.Skive;
                sk.Skytter = new Skytter();
                sk.Skytter.Klasse = element.Klasse;
                sk.Skytter.Name = element.Name;
                sk.Skytter.Skytterlag = element.Skytterlag;
                sk.Skytter.SkytterNr = element.SkytterNr;
                sk.Skytter.InputXmlData = element.InputXmlData;
                sk.Skytter.Links = element.Links;
                SkiverViewModel mod = new SkiverViewModel(element.Lagnr, sk);
                m_skiver.Add(mod);
            }

            NotifyPropertyChanged("Skiver");


        }
        private ObservableCollection<InputData> m_bane100mFileRows;

        public ObservableCollection<InputData> Bane100mFileRows
        {
            get { return m_bane100mFileRows; }
            set
            {
                m_bane100mFileRows = value;
                NotifyPropertyChanged("Bane100mFileRows");
            }
        }

        public string Bane100mFile { get; set; }
        internal void Bane100mButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Bane100mFile))
            {
                return;
            }
            string path = Path.GetDirectoryName(Bane100mFile);
            string filename = Path.GetFileName(Bane100mFile);
            var input = InputXmlFileParser.ParseXmlFile(path, filename);

            m_bane100mFileRows = new ObservableCollection<InputData>();
            m_skiver.Clear();
            foreach (var element in input)
            {
                m_bane100mFileRows.Add(element);
                Skiver sk = new Utils.Skiver();
                sk.SkiveNummer = element.Skive;
                sk.Skytter = new Skytter();
                sk.Skytter.Klasse = element.Klasse;
                sk.Skytter.Name = element.Name;
                sk.Skytter.Skytterlag = element.Skytterlag;
                sk.Skytter.SkytterNr = element.SkytterNr;
                sk.Skytter.InputXmlData = element.InputXmlData;
                sk.Skytter.Links = element.Links;
                SkiverViewModel mod = new SkiverViewModel(element.Lagnr, sk);
                m_skiver.Add(mod);
            }

            NotifyPropertyChanged("Skiver");


        }

        private ObservableCollection<InputData> m_grovfeltRows;

        public ObservableCollection<InputData> GrovfeltRows
        {
            get { return m_grovfeltRows; }
            set
            {
                m_grovfeltRows = value;
                NotifyPropertyChanged("GrovfeltRows");
            }
        }

        public string GrovfeltFile { get; set; }
        internal void GrovFeltButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(GrovfeltFile))
            {
                return;
            }
            string path = Path.GetDirectoryName(GrovfeltFile);
            string filename = Path.GetFileName(GrovfeltFile);
            var input = InputXmlFileParser.ParseXmlFile(path, filename);
            var inputLagData = InputXmlFileParser.ParseXmlFileLagData(path, filename);

            m_grovfeltRows = new ObservableCollection<InputData>();
            m_skiver.Clear();
            foreach (var element in input)
            {
                m_grovfeltRows.Add(element);
                Skiver sk = new Utils.Skiver();
                sk.SkiveNummer = element.Skive;
                sk.Skytter = new Skytter();
                sk.Skytter.Klasse = element.Klasse;
                sk.Skytter.Name = element.Name;
                sk.Skytter.Skytterlag = element.Skytterlag;
                sk.Skytter.SkytterNr = element.SkytterNr;
                sk.Skytter.InputXmlData = element.InputXmlData;
                sk.Skytter.Links = element.Links;
                SkiverViewModel mod = new SkiverViewModel(element.Lagnr, sk);
                m_skiver.Add(mod);
            }

            NotifyPropertyChanged("Skiver");


        }
        private ObservableCollection<InputData> m_bane200mFileRows;

        public ObservableCollection<InputData> Bane200mFileRows
        {
            get { return m_bane200mFileRows; }
            set
            {
                m_bane200mFileRows = value;
                NotifyPropertyChanged("Bane200mFileRows");
            }
        }

        public string Bane200mFile { get; set; }
        internal void Bane200mButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Bane200mFile))
            {
                return;
            }
            string path = Path.GetDirectoryName(Bane200mFile);
            string filename = Path.GetFileName(Bane200mFile);
            var input = InputXmlFileParser.ParseXmlFile(path, filename);

            m_bane200mFileRows = new ObservableCollection<InputData>();
            m_skiver.Clear();
            foreach (var element in input)
            {
                m_bane200mFileRows.Add(element);
                Skiver sk = new Utils.Skiver();
                sk.SkiveNummer = element.Skive;
                sk.Skytter = new Skytter();
                sk.Skytter.Klasse = element.Klasse;
                sk.Skytter.Name = element.Name;
                sk.Skytter.Skytterlag = element.Skytterlag;
                sk.Skytter.SkytterNr = element.SkytterNr;
                sk.Skytter.InputXmlData = element.InputXmlData;
                sk.Skytter.Links = element.Links;
                SkiverViewModel mod = new SkiverViewModel(element.Lagnr, sk);
                m_skiver.Add(mod);
            }

            NotifyPropertyChanged("Skiver");


        }


        private ObservableCollection<SkiverViewModel> m_skiver;
        public ObservableCollection<SkiverViewModel> Skiver
        {
            get { return m_skiver; }
            set
            {
                m_skiver = value;
                NotifyPropertyChanged("Skiver");
            }
        }


        private string m_ErrorMsg;
        public string ErrorMsg
        {
            get { return m_ErrorMsg; }
            set
            {
                m_ErrorMsg = value;
                NotifyPropertyChanged("ErrorMsg");
            }
        }

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }
}
