using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.ViewModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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
            while (count < 10)
            {
                InputData data = new InputData();
                data.SkytterNr = count;
                m_inputRows.Add(data);
                count++;
            }
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

            List<string> KlasseListe = new List<string>();

            if (string.IsNullOrEmpty(KlasseSort))
            {
                KlasseListe = InputRows.Select(o => o.Klasse).Distinct().ToList();
            }
            else
            {
                var klasser=KlasseSort.Split(new []{';', ',' },StringSplitOptions.None);
                foreach (var element in klasser)
                {
                    if (!string.IsNullOrEmpty(element))
                    {
                        KlasseListe.Add(element);
                    }
                }
            }

            List<InputData> AlleSkyttere = new List<InputData>();
            var ListerAvKlasser = SorterLagPaaKlasse(KlasseListe, InputRows);
            foreach (var KlasseVis in ListerAvKlasser)
            {
                InputDataComparer computer = new InputDataComparer();
                KlasseVis.Sort(computer);
            }

            //public List<Lag> GenererSimpelLag(List<List<InputData>> alleSkytterePrKlasse, int startLag, int antallskiver, int? GenerateSpaceEach)

            var list = Generator.GenererSimpelLag(ListerAvKlasser, LagNummer, SkiverILaget, GenerateSpaceEach);

            LagOppsettViewModel viewmodel  = new LagOppsettViewModel(list,6,new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
            var view = new LagOppsettView(viewmodel);

            OpenWindow(view, "Data input");

        }

        private List<List<InputData>> SorterLagPaaKlasse(List<string> klasseListe, ObservableCollection<InputData> inputRows)
        {

            List<List<InputData>> retVal = new List<List<InputData>>();


            foreach (var klasseNavn in klasseListe)
            {
                List<InputData> klasseliste= new List<InputData>();

                klasseliste = inputRows.Where(o => o.Klasse == klasseNavn).ToList();

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
