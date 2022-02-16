using System;
using System.Collections.Generic;
using System.Linq;
using WebResultsClient.Definisjoner;

namespace WebResultsClient.Premieberegning
{
    public class Pengepremier
    {
        private Dictionary<string, Dictionary<string, double>> SerierMedPremieForOvelse = new Dictionary<string, Dictionary<string, double>> 
        { 
            { 
                "IN", new Dictionary<string, double> 
                { 
                    { "IN_1To3", .6 }, // 15 skudd med 60% vekting av premien
                    { "IN_4", .4 }, // 10 skudd med 40% vekting av premien
                } 
            } ,
            {
                "BA", new Dictionary<string, double>
                {
                    { "BA_1To3", .6 },
                    { "BA_4", .4 },
                }
            },
            {
                "FE", new Dictionary<string, double>
                {
                    { "FE_1To5", 1.0 },
                }
            }
        };

        private Stevneoppgjor m_stevneOppgjor;

        public Pengepremier(Stevneoppgjor stevneOppgjor)
        {
            m_stevneOppgjor = stevneOppgjor;
        }

        public void BeregnPengepremier(int premieavgift, List<string> klasser)
        {
            var forsteResultat = m_stevneOppgjor.Resultater.FirstOrDefault();
            if(forsteResultat != null)
            {
                var ovelse = forsteResultat.Ovelse.Id;
                var groupedOppgjor = m_stevneOppgjor.Resultater.GroupBy(r => r.Ovelse.Klasse);

                foreach (var group in groupedOppgjor)
                {
                    if (klasser.Contains(group.Key))
                    {
                        var serierMedPremie = SerierMedPremieForOvelse[ovelse];

                        var skyttereMedTellendeResultat = group.Where(g => HarTellendeTotalSum(g, ovelse)).ToList();

                        var antallPremier = (int)Math.Ceiling(skyttereMedTellendeResultat.Count / 3.0);
                        var totaltPremieinnskudd = skyttereMedTellendeResultat.Count * premieavgift;

                        foreach(var serie in serierMedPremie)
                        {
                            var premieForSerie = totaltPremieinnskudd * serie.Value;
                            var premier = PengepremieFordeler.BeregnPremieSummer(antallPremier, premieForSerie);

                            var sortertResultatForSerie = skyttereMedTellendeResultat.OrderBy(r => SorterResultat(r, serie.Key)).ToList();
                            AssignPremier(sortertResultatForSerie, premier, serie.Key);
                        }

                        skyttereMedTellendeResultat.ForEach(r => AssignPremieToOvelse(r.Ovelse, serierMedPremie));
                    }
                }
            }
        }

        private void AssignPremieToOvelse(Ovelse ovelse, Dictionary<string, double> serierMedPremie)
        {
            int premieOvelse = 0;
            foreach(var serie in serierMedPremie.Keys)
            {
                premieOvelse += int.Parse(ovelse.Serier.Single(s => s.Id == serie).Premie);
            }
            ovelse.Premie = premieOvelse.ToString();
        }

        private void AssignPremier(List<Resultat> resultater, List<int> premier, string serie)
        {
            var currentPremie = 1;

            while (currentPremie < premier.Count + 1)
            {
                var resultaterRank = resultater.Where(r => currentPremie == int.Parse(r.Ovelse.Serier.Single(s => s.Id == serie).RankKlasse)).ToList();

                double premieSum = 0;
                for (int premie = currentPremie - 1; premie < currentPremie + resultaterRank.Count - 1; premie++)
                {
                    if (premie < premier.Count)
                    {
                        premieSum += premier[premie];
                    }
                }

                premieSum /= resultaterRank.Count;
                foreach (var resultat in resultaterRank)
                {
                    var premieSerie = resultat.Ovelse.Serier.Single(s => s.Id == serie);
                    premieSerie.Premie = ((int)Math.Round(premieSum)).ToString();
                }

                currentPremie += resultaterRank.Count;
            }
        }

        private bool HarTellendeTotalSum(Resultat resultat, string ovelse)
        {
            var totalSum = resultat.Ovelse.Serier.Single(s => s.Id == (ovelse + "_tot"));

            if (string.IsNullOrEmpty(totalSum.Sum))
            {
                return false;
            }

            return true;
        }

        private int SorterResultat(Resultat resultat, string serie)
        {
            var seriePlassering = resultat.Ovelse.Serier.Single(s => s.Id == serie).RankKlasse;

            return int.Parse(seriePlassering);
        }
    }
}
