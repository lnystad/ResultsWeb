﻿using System.Text.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using WebResultsClient.Definisjoner;
using WebResultsClient.Premieberegning;
using WebResultsClient.Viewmodels;

namespace WebResultsClient.Commands
{
    public class GenererStevneoppgjorCommand : ICommand
    {
        private StevneoppgjorSelectionViewModel m_stevneoppgjorViewModel;

        public GenererStevneoppgjorCommand(StevneoppgjorSelectionViewModel stevneoppgjorSelectionViewModel)
        {
            m_stevneoppgjorViewModel = stevneoppgjorSelectionViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string stevneoppgjorDir = Path.Combine(m_stevneoppgjorViewModel.StevneDir, m_stevneoppgjorViewModel.StevneNavn);
            stevneoppgjorDir = Path.Combine(stevneoppgjorDir, "Stevneoppgjør");

            var stevneoppgjorFile = Directory.EnumerateFiles(stevneoppgjorDir, "Stevneoppgjør-*.xml").ToList();
            if (stevneoppgjorFile.Count() > 1)
            {
                MessageBox.Show("Fant flere aktuelle filer for stevneoppgjør. Slett filer som ikke er aktuelle");
                Process.Start("explorer.exe", stevneoppgjorDir);
            }
            else
            {
                var filename = stevneoppgjorFile.FirstOrDefault();

                if (File.Exists(filename))
                {
                    var stevneoppgjor = DeserializeXml<Stevneoppgjor>(filename);

                    var pengepremier = new Pengepremier(stevneoppgjor);
                    pengepremier.BeregnPengepremier(int.Parse(m_stevneoppgjorViewModel.SeniorPremieavgift), m_stevneoppgjorViewModel.SeniorKlasser.ToList());
                    pengepremier.BeregnPengepremier(int.Parse(m_stevneoppgjorViewModel.UngdomPremieavgift), m_stevneoppgjorViewModel.UngdomsKlasser.ToList());

                    var nyttStevneoppgjorFilename = Path.Combine(stevneoppgjorDir, "LAST OPP DENNE TIL DFS " + m_stevneoppgjorViewModel.StevneNavn + ".xml"); ;
                    SerializeXml(stevneoppgjor, nyttStevneoppgjorFilename);
                    PengePremieSummary summarySenior = new PengePremieSummary();
                    var ConclusionSenior = summarySenior.CalculateSummary(stevneoppgjor, m_stevneoppgjorViewModel.SeniorKlasser.ToList(), Pengepremier.PremieOvelse);
                    ConclusionSenior.CompleteCalculation();
                    PengePremieSummary summaryJunior = new PengePremieSummary();
                    var ConclusionJunior=summaryJunior.CalculateSummary(stevneoppgjor, m_stevneoppgjorViewModel.UngdomsKlasser.ToList(), Pengepremier.PremieOvelse);
                    ConclusionJunior.CompleteCalculation();
                    JsonSerializerOptions context = new JsonSerializerOptions();
                    context.WriteIndented = true;
                    m_stevneoppgjorViewModel.JuniorSummary  = JsonSerializer.Serialize(ConclusionJunior,typeof(Summary),context);
                    m_stevneoppgjorViewModel.SeniorSummary = JsonSerializer.Serialize(ConclusionSenior, typeof(Summary), context);
                   
                    Process.Start("explorer.exe", stevneoppgjorDir);
                }
                else
                {
                    MessageBox.Show("Fant ikke fil for stevneoppgjør. Har du husket å generere stevneoppgjøret i Leon?");
                }
            }
        }

        public static T DeserializeXml<T>(string filename) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return serializer.Deserialize(reader) as T;
                }
            }
        }

        public static void SerializeXml<T>(T xml, string filename) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            var encoding = Encoding.GetEncoding("UTF-8");
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            using (StreamWriter writer = new StreamWriter(filename, false, encoding))
            {
                serializer.Serialize(writer, xml, xmlns);
            }
        }
    }
}
