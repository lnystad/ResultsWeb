using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;

    using OrionLag.Input.Data;
    using OrionLag.Input.ViewModel;
    using OrionLag.Input.Views;
    using OrionLag.Utils;

    public class UserInputControlViewModel : INotifyPropertyChanged

    {

        public UserInputControlViewModel()
        {
            int count=0;
            m_inputRows = new ObservableCollection<InputData>();
            //while (count < 10)
            //{
            //    InputData data = new InputData();
            //    data.SkytterNr = count;
            //    m_inputRows.Add(data);
            //    count++;
            //}

            var SkiverILaget = ConfigurationManager.AppSettings["SkiverILaget"];
            if (!string.IsNullOrEmpty(SkiverILaget))
            {
                int num = 0;
                if (int.TryParse(SkiverILaget, out num))
                {
                    this.m_SkiverILaget = num;
                }
            }
            var LagNummer = ConfigurationManager.AppSettings["LagNummer"];
            if (!string.IsNullOrEmpty(LagNummer))
            {
                int num = 0;
                if (int.TryParse(LagNummer, out num))
                {
                    this.m_lagNummer = num;
                }
            }

            var KlasseSort = ConfigurationManager.AppSettings["KlasseSort"];
            if (!string.IsNullOrEmpty(KlasseSort))
            {
                this.m_KlasseSort = KlasseSort;
            }
            m_SpaceAfterKlasse = true;
        }

        private ObservableCollection<InputData> m_inputRows;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private int m_lagNummer;
        public int LagNummer
        {
            get { return m_lagNummer; }
            set
            {
                m_lagNummer = value;
                NotifyPropertyChanged("LagNummer");
            }
        }

        private int m_SkiverILaget;
        public int SkiverILaget
        {
            get { return m_SkiverILaget; }
            set
            {
                m_SkiverILaget = value;
                NotifyPropertyChanged("SkiverILaget");
            }
        }

        private int m_GenerateSpaceEach;
        public int GenerateSpaceEach
        {
            get { return m_GenerateSpaceEach; }
            set
            {
                m_GenerateSpaceEach = value;
                NotifyPropertyChanged("GenerateSpaceEach");
            }
        }
        private string m_KlasseSort;
        public string KlasseSort
        {
            get { return m_KlasseSort; }
            set
            {
                m_KlasseSort = value;
                NotifyPropertyChanged("KlasseSort");
            }
        }

        private bool m_GenerateEmptyLagIsChecked;
        public bool GenerateEmptyLagIsChecked
        {
            get { return m_GenerateEmptyLagIsChecked; }
            set
            {
                m_GenerateEmptyLagIsChecked = value;
                NotifyPropertyChanged("GenerateEmptyLagIsChecked");
            }
        }
        private bool m_SpaceAfterKlasse;
        public bool SpaceAfterKlasse
        {
            get { return m_SpaceAfterKlasse; }
            set
            {
                m_SpaceAfterKlasse = value;
                NotifyPropertyChanged("SpaceAfterKlasse");
            }
        }

        private bool m_FinfeltLinks;
        public bool FinfeltLinks
        {
            get { return m_FinfeltLinks; }
            set
            {
                m_FinfeltLinks = value;
                NotifyPropertyChanged("FinfeltLinks");
            }
        }
        

        public ObservableCollection<InputData> InputRows
        {
            get { return m_inputRows; }
            set
            {
                m_inputRows = value;
                NotifyPropertyChanged("InputRows");
            }
        }

        public void OnReadInputbutton_OnClick(object sender, RoutedEventArgs routedEventArgs)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string FileName = Path.GetFileName(openFileDialog.FileName);
                string FilePath = Path.GetDirectoryName(openFileDialog.FileName);
                var input = InputFileParser.ParseFile(FilePath, FileName);
                InputRows = new ObservableCollection<InputData>(input);
            }
            return;
        }

        public void OnWriteInputbutton_OnClickOn(object sender, RoutedEventArgs routedEventArgs)
        {
            if (InputRows.Count == 0)
            {
                return;
            }
            LagGenerator Generator = new LagGenerator();

            List<KlasseSort> KlasseListe = new List<KlasseSort>();

            if (string.IsNullOrEmpty(KlasseSort))
            {
                var liste= InputRows.Select(o => o.Klasse).Distinct().ToList();
                string viewString = string.Empty;
                foreach (var kl in liste)
                {
                    KlasseSort el = new KlasseSort();
                    el.Klasse = kl;
                    el.SpaceInLag = 0;
                    viewString = viewString + "," + el.Klasse + "-" + el.SpaceInLag;
                }

                KlasseSort = viewString;
            }
            else
            {
                KlasseListe = GetklasseListe(KlasseSort);
               
            }

            List<InputData> AlleSkyttere = new List<InputData>();
            var ListerAvKlasser = SorterLagPaaKlasse(KlasseListe, InputRows);
            foreach (var KlasseVis in ListerAvKlasser)
            {
                InputDataComparer computer = new InputDataComparer();
                KlasseVis.Sort(computer);
                //InputDataComparerLinksFelt links = new InputDataComparerLinksFelt();
                //KlasseVis.Sort(links);
            }

            List<Lag> inputGenererteLag=null;
            if (FinfeltLinks)
            {
                inputGenererteLag = Generator.GenererSimpelLagFinfelt(ListerAvKlasser, KlasseListe, LagNummer, SkiverILaget);
            }
            else
            {
                inputGenererteLag = Generator.GenererSimpelLag(ListerAvKlasser, KlasseListe, LagNummer, SkiverILaget, GenerateSpaceEach, GenerateEmptyLagIsChecked, SpaceAfterKlasse);
            }
            

            LagOppsettViewModel viewmodel  = new LagOppsettViewModel(inputGenererteLag,new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            var view = new LagOppsettView(viewmodel);

            OpenWindow(view, "Data input");

        }

        private List<KlasseSort> GetklasseListe(string klasseSort)
        {
            List<KlasseSort> retVal = new List<KlasseSort>();
            var klasser = KlasseSort.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var element in klasser)
            {
                if (!string.IsNullOrEmpty(element))
                {
                    KlasseSort el = new KlasseSort();
                    if (element.Contains("-"))
                    {
                        var splits = element.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        el.Klasse = splits[0];
                        if (splits.Length > 0)
                        {
                            int num = 0;
                            if (int.TryParse(splits[1], out num))
                            {
                                el.SpaceInLag = num;
                            }
                        }
                    }
                    else
                    {
                        el.Klasse = element;
                        el.SpaceInLag = 0;
                    }

                    retVal.Add(el);
                }
            }

            return retVal;
        }

        private List<List<InputData>> SorterLagPaaKlasse(List<KlasseSort> klasseListe, ObservableCollection<InputData> inputRows)
        {

            List<List<InputData>> retVal = new List<List<InputData>>();


            foreach (var klasseel in klasseListe)
            {
                List<InputData> klasseliste= new List<InputData>();

                klasseliste = inputRows.Where(o => o.Klasse == klasseel.Klasse).ToList();

                retVal.Add(klasseliste);
            }

            return retVal;
        }

        private void OpenWindow(object windowContent, string title = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = string.Concat("[No Name (", windowContent.GetType().Name, ")]");
            }

            var window = new Window { Content = windowContent, Height = 600, Width = 900, Title = title };
            window.Show();
        }
    }
}
