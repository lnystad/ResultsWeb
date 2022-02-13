using System;
using System.Collections.Generic;
using System.Linq;
using WebResultsClient.Definisjoner;

namespace WebResultsClient.Premieberegning
{
    public class Pengepremier
    {
        private Stevneoppgjor m_stevneOppgjor;

        public Pengepremier(Stevneoppgjor stevneOppgjor)
        {
            m_stevneOppgjor = stevneOppgjor;
        }

        public void BeregnPengepremier(int premieavgift, List<string> klasser)
        {
            var groupedOppgjor = m_stevneOppgjor.Resultater.GroupBy(r => r.Ovelse.Klasse);

            foreach (var group in groupedOppgjor)
            {
                if(klasser.Contains(group.Key))
                {
                    var skyttereMedTellendeResultat = group.Where(HarTellendeTotalSum).ToList();

                    var antallPremier = (int)Math.Ceiling(skyttereMedTellendeResultat.Count / 3.0);
                    var totaltPremieinnskudd = skyttereMedTellendeResultat.Count * premieavgift;
                    var premieTotal15Skudd = totaltPremieinnskudd * .6;
                    var premieTotal10Skudd = totaltPremieinnskudd * .4;

                    // 15 skudd
                    var sorted15Skudd = skyttereMedTellendeResultat.OrderBy(Sorter15Skudd).ToList();
                    var premier15Skudd = PengepremieFordeler.BeregnPremieSummer(antallPremier, premieTotal15Skudd);
                    AssignPremier15Skudd(sorted15Skudd, premier15Skudd);

                    // 10 skudd
                    var sorted10Skudd = skyttereMedTellendeResultat.OrderBy(Sorter10Skudd).ToList();
                    var premier10Skudd = PengepremieFordeler.BeregnPremieSummer(antallPremier, premieTotal10Skudd);
                    AssignPremier10Skudd(sorted10Skudd, premier10Skudd);

                    skyttereMedTellendeResultat.ForEach(r => AssignPremieToOvelse(r.Ovelse));
                }
            }
        }

        private void AssignPremieToOvelse(Ovelse ovelse)
        {
            var premie15Skudd = int.Parse(ovelse.Serier.Single(s => s.Id == "IN_1To3").Premie);
            var premie10Skudd = int.Parse(ovelse.Serier.Single(s => s.Id == "IN_4").Premie);
            ovelse.Premie = (premie15Skudd + premie10Skudd).ToString();
        }

        private void AssignPremier15Skudd(List<Resultat> resultater, List<int> premier)
        {
            var currentPremie = 1;

            while (currentPremie < premier.Count + 1)
            {
                var resultaterRank = resultater.Where(r => currentPremie == int.Parse(r.Ovelse.Serier.Single(s => s.Id == "IN_1To3").RankKlasse)).ToList();

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
                    var serie15Skudd = resultat.Ovelse.Serier.Single(s => s.Id == "IN_1To3");
                    serie15Skudd.Premie = ((int)Math.Round(premieSum)).ToString();
                }

                currentPremie += resultaterRank.Count;
            }
        }

        private void AssignPremier10Skudd(List<Resultat> resultater, List<int> premier)
        {
            var currentPremie = 1;

            while (currentPremie < premier.Count + 1)
            {
                var resultaterRank = resultater.Where(r => currentPremie == int.Parse(r.Ovelse.Serier.Single(s => s.Id == "IN_4").RankKlasse)).ToList();

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
                    var serie10Skudd = resultat.Ovelse.Serier.Single(s => s.Id == "IN_4");
                    serie10Skudd.Premie = ((int)Math.Round(premieSum)).ToString();
                }

                currentPremie += resultaterRank.Count;
            }
        }

        private bool HarTellendeTotalSum(Resultat resultat)
        {
            var serie = resultat.Ovelse.Serier.Single(s => s.Id == "IN_tot");

            if (string.IsNullOrEmpty(serie.Sum) || serie.Sum == "0")
            {
                return false;
            }

            return true;
        }

        private int Sorter15Skudd(Resultat resultat)
        {
            var serie = resultat.Ovelse.Serier.Single(s => s.Id == "IN_1To3");

            return int.Parse(serie.RankKlasse);
        }

        private int Sorter10Skudd(Resultat resultat)
        {
            var serie = resultat.Ovelse.Serier.Single(s => s.Id == "IN_4");

            return int.Parse(serie.RankKlasse);
        }
    }
}
