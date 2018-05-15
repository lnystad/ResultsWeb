using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Input.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    using OrionLag.Input.Data;
    using OrionLag.Utils;

    public class LagOppsettViewModel : INotifyPropertyChanged
    {
        private List<Lag> lagOppsett;

        private StevneInfo m_stevneinfo;

        public LagOppsettViewModel(StevneInfo stevneinfo,List<Lag> lagOppsett,DateTime startTime)
        {
            m_stevneinfo = stevneinfo;
            lagOppsett = lagOppsett;
            m_inputRows = new ObservableCollection<Lag>(lagOppsett);
            m_skiver = new ObservableCollection<SkiverViewModel>();
            this.LagKilde = new ObservableCollection<Lag>();
            LagStart = startTime.ToString("HH:mm");
            LagDuration = string.Empty;

            var LagStartString = ConfigurationManager.AppSettings["LagStart"];
            if (!string.IsNullOrEmpty(LagStartString))
            {
                LagStart = LagStartString;
            }

            var LagDurationString = ConfigurationManager.AppSettings["LagDuration"];
            if (!string.IsNullOrEmpty(LagStartString))
            {
                LagDuration = LagDurationString;
            }
            var OppropsTidString = ConfigurationManager.AppSettings["OppropsTid"];
            if (!string.IsNullOrEmpty(OppropsTidString))
            {
                int dur = 0;
                if (int.TryParse(OppropsTidString, out dur))
                {
                    OppropsTid = dur;
                }
            }

            var LagFellesOppropString = ConfigurationManager.AppSettings["LagFellesOpprop"];
            if (!string.IsNullOrEmpty(LagFellesOppropString))
            {
                int dur = 0;
                if (int.TryParse(LagFellesOppropString, out dur))
                {
                    LagFellesOpprop = dur;
                }
            }

            var SortertFilePath = ConfigurationManager.AppSettings["SortertFilePath"];
            if (!string.IsNullOrEmpty(SortertFilePath))
            {
                FilePath = SortertFilePath;
            }



            int totalantSkyttere = 0;
             var sortedlagOppsett= lagOppsett.OrderBy(o => o.LagNummer).ToList();
            foreach (var lag in sortedlagOppsett)
            {
                this.LagKilde.Add(lag);
                var sortedSkiver = lag.SkiverILaget.OrderBy(x => x.SkiveNummer);
                foreach (var skive in sortedSkiver)
                {
                    totalantSkyttere++;
                    m_skiver.Add(item: new SkiverViewModel(lag.LagNummer, skive));
                }
            }

            m_TotalAntallSkytter = totalantSkyttere.ToString();
        }

        private void SortGrid()
        {
            var input = m_inputRows.OrderBy(o => o.LagNummer);
            m_inputRows = new ObservableCollection<Lag>(input.ToList());

            var sortskiver = m_skiver.OrderBy(x => x.LagNummer).ThenBy(y => y.SkiveNummer);
            m_skiver = new ObservableCollection<SkiverViewModel>(sortskiver.ToList());
            NotifyPropertyChanged("Skiver");
            NotifyPropertyChanged("InputRows");
        }

        

        private string m_TotalAntallSkytter;
        public string TotalAntallSkytter
        {
            get { return m_TotalAntallSkytter; }
            
        }

        private string m_lagDuration;
        public string LagDuration
        {
            get { return m_lagDuration; }
            set
            {
                m_lagDuration = value;
                NotifyPropertyChanged("LagDuration");
            }
        }

        private string m_lagStart;
        public string LagStart
        {
            get { return m_lagStart; }
            set
            {
                m_lagStart = value;
                NotifyPropertyChanged("LagStart");
            }
        }

        
        private int m_OppropsTid;
        public int OppropsTid
        {
            get { return m_OppropsTid; }
            set
            {
                m_OppropsTid = value;
                NotifyPropertyChanged("OppropsTid");
            }
        }

        private int m_LagFellesOpprop;
        public int LagFellesOpprop
        {
            get { return m_LagFellesOpprop; }
            set
            {
                m_LagFellesOpprop = value;
                NotifyPropertyChanged("LagFellesOpprop");
            }
        }

        private bool m_GenerateTimeAfterLagIsChecked;
        public bool GenerateTimeAfterLagIsChecked
        {
            get { return m_GenerateTimeAfterLagIsChecked; }
            set
            {
                m_GenerateTimeAfterLagIsChecked = value;
                NotifyPropertyChanged("GenerateTimeAfterLagIsChecked");
            }
        }
        

        private int m_lagStartNumber;
        public int LagStartNumber
        {
            get { return m_lagStartNumber; }
            set
            {
                m_lagStartNumber = value;
                NotifyPropertyChanged("LagStartNumber");
            }
        }
        

        private string m_filePath;
        public string  FilePath
        {
            get { return m_filePath; }
            set
            {
                m_filePath = value;
                NotifyPropertyChanged("FilePath");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private Lag m_selectedLag2;
        public Lag SelectedLag2
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

        private ObservableCollection<Lag> m_lagKilde;
        public ObservableCollection<Lag> LagKilde
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

        private ObservableCollection<Lag> m_inputRows;

        public ObservableCollection<Lag> InputRows
        {
            get { return m_inputRows; }
            set
            {
                m_inputRows = value;
                NotifyPropertyChanged("InputRows");
            }
        }

        public DataGrid GridManager { get; set; }

        public void SetTimesButton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if(string.IsNullOrEmpty(m_lagDuration))
            {
                return;
            }
            if (string.IsNullOrEmpty(m_lagStart))
            {
                return;
            }


        //    OppropsTid
        //{
        //        get { return m_OppropsTid;
        //        }
        //        set
        //    {
        //            m_OppropsTid = value;
        //            NotifyPropertyChanged("OppropsTid");
        //        }
        //    }

        //private int m_LagFellesOpprop;
        //public int LagFellesOpprop


            int minutes = 0;
            if (!int.TryParse(m_lagDuration, out minutes))
            {
                return;
            }

            var splits = m_lagStart.Split(new char[] { ':' });
            if (splits.Length != 2)
            {
                return;
            }
            int hour = 0;
            int min = 0;
            if (!int.TryParse(splits[0], out hour))
            {
                return;
            }
            if (!int.TryParse(splits[1], out min))
            {
                return;
            }

            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, min, 0);
            TimeSpan span = new TimeSpan(0, minutes, 0);

            int oppropfortid = 20;
            
            if (this.OppropsTid > 0)
            {
                oppropfortid = this.OppropsTid;
            }
            if (oppropfortid < 0)
            {
                oppropfortid = oppropfortid;
            }
            
            int LagCount = 1;
            int LagCountmedFelles = 0;
            string currentKlasse = string.Empty;
            foreach (var lag in LagKilde)
            {
                string klasseILaget = string.Empty;
                if (LagCount == 1)
                {
                    
                    currentKlasse = FinnKlasse(lag);
                    klasseILaget = currentKlasse;
                }
                else
                {
                    klasseILaget = FinnKlasse(lag);
                }
               
                
                if (GenerateTimeAfterLagIsChecked && klasseILaget!= currentKlasse)
                {
                    currentKlasse = klasseILaget;
                    startTime = startTime.Add(span);
                    startTime = startTime.Add(span);
                    startTime = startTime.Add(span);
                    startTime = startTime.Add(span);
                    lag.LagTid = startTime;
                    TimeSpan oppropspan = new TimeSpan(0, -oppropfortid, 0);
                    lag.OppropsTid = startTime.Add(oppropspan);
                    LagCountmedFelles = 1;
                    startTime = startTime.Add(span);
                }
                else
                {
                    lag.LagTid = startTime;
                   
                    if (LagFellesOpprop > 0)
                    {
                        int fellesOpp = LagCountmedFelles % LagFellesOpprop;
                        if (fellesOpp == 0)
                        {
                            TimeSpan oppropspan = new TimeSpan(0, -oppropfortid, 0);
                            lag.OppropsTid = startTime.Add(oppropspan);
                            LagCountmedFelles = 0;
                        }
                        else
                        {
                            TimeSpan oppropspan = new TimeSpan(0, -oppropfortid - (minutes * fellesOpp), 0);
                            lag.OppropsTid = startTime.Add(oppropspan);
                        }
                        LagCountmedFelles ++;
                    }
                    else
                    {
                        TimeSpan oppropspan = new TimeSpan(0, -oppropfortid, 0);
                        lag.OppropsTid = startTime.Add(oppropspan);
                    }

                    startTime = startTime.Add(span);
                }

               

                LagCount++;
            }

            var test = LagKilde;
            LagKilde = null;
            NotifyPropertyChanged("LagKilde");
            LagKilde = test;
            NotifyPropertyChanged("LagKilde");
        }

        private string FinnKlasse(Lag lag)
        {

            foreach (var skive in lag.SkiverILaget)
            {
                if (skive.Skytter != null)
                {
                    return skive.Skytter.Klasse;
                }
            }
            return string.Empty;
        }

        public void SortButton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            SortGrid();
        }

        public void GenerateFilesButtonBase_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if(string.IsNullOrEmpty(m_filePath))
            {
                return;
            }

            if (!Directory.Exists(m_filePath))
            {
                MessageBox.Show(string.Format("Path Does not exsist{0}", m_filePath),"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }

            Collection<Lag> lagCollection = new Collection<Lag>();
            XDocument doc = new XDocument(new XDeclaration("1.0","UTF-8","yes"));
            var root = new XElement("paamelding");
            doc.Add(root);



            root.Add(new XAttribute("stevnenavn", m_stevneinfo.stevnenavn));

            root.Add(new XAttribute("stevnenummer", m_stevneinfo.stevnenummer));
            root.Add(new XAttribute("arrangor", m_stevneinfo.arrangor));

            root.Add(new XAttribute("stevnestart", m_stevneinfo.stevnestart));
            root.Add(new XAttribute("stevneslutt", m_stevneinfo.stevneslutt));

            var ovelse= new XElement("ovelse");
            ovelse.Add(new XAttribute("id", "FE"));
            ovelse.Add(new XAttribute("hold", "Fin felt"));
            ovelse.Add(new XAttribute("stang", "0"));
            ovelse.Add(new XAttribute("felthurtig", "0"));
            ovelse.Add(new XAttribute("minne", "0"));

            root.Add(ovelse);



            foreach (var inputlag in LagKilde)
            {
                var lag = new XElement("lag");
                ovelse.Add(lag);
                lag.Add(new XAttribute("navn", ""));
                lag.Add(new XAttribute("lagnr", inputlag.LagNummer));
                if (inputlag.LagTid.HasValue)
                {
                    lag.Add(new XAttribute("dato", inputlag.LagTid.Value.ToString("dd.MM.yyyy")));
                }
                if (inputlag.LagTid.HasValue)
                {
                    lag.Add(new XAttribute("kl-skytetid", inputlag.LagTid.Value.ToString("HH:mm")));
                }

                if (inputlag.OppropsTid.HasValue)
                {
                    lag.Add(new XAttribute("kl-opprop", inputlag.OppropsTid.Value.ToString("HH:mm")));
                }
            }

            foreach (var inputlag in LagKilde)
            {
                foreach (var skive in inputlag.SkiverILaget)
                {
                    var skytter = new XElement("paamelding-skytter");
                    ovelse.Add(skytter);
                    skytter.Add(new XAttribute("lag", inputlag.LagNummer));
                    skytter.Add(new XAttribute("skive", skive.SkiveNummer));
                    foreach (var data in skive.Skytter.InputXmlData)
                    {
                        if (data.Name == "lag" || data.Name == "skive")
                        {
                            
                        }
                        else
                        {
                            skytter.Add(new XAttribute(data.Name, data.Value));
                        }

                    }
                }
            }

            string filename = Path.Combine(m_filePath, "paamelding_test.xml");
            doc.Save(filename);
            //foreach (var skive in Skiver)
            //{
            //    var lagFunnet = lagCollection.FirstOrDefault(x => x.LagNummer == skive.LagNummer);

            //    var funnetSkive = lagFunnet.SkiverILaget.FirstOrDefault(x => x.SkiveNummer == skive.SkiveNummer);
            //    if (funnetSkive.Skytter != null)
            //    {
            //        funnetSkive.Skytter = new Skytter(funnetSkive.Skytter);
            //    }

            //    funnetSkive.Free = funnetSkive.Free;
            //}

            //XmlSerializer ser = new XmlSerializer(typeof(Lag));

            //foreach (var utskriftsLag in lagCollection)
            //{
            //    string filename = Path.Combine(m_filePath, string.Format("Hold_{0}_Lag_{1}.xml","1", utskriftsLag.LagNummer));

            //    using (XmlWriter Write = new XmlTextWriter(filename, Encoding.UTF8))
            //    {
            //        ser.Serialize(Write, utskriftsLag);
            //    }
            //}


        }
    }
}
