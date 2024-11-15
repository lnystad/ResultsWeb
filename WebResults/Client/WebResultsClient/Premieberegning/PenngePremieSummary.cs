using FileUploaderService.Diagnosis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebResultsClient.Definisjoner;

namespace WebResultsClient.Premieberegning
{
    public class PengePremieSummary
    {

        public Summary CalculateSummary(Stevneoppgjor oppgjor, List<string> klasser, Dictionary<string, DFSOvelse> PremieOvelse)
        {
            if (oppgjor == null)
                return new Summary();
            Summary summary = new Summary();
            foreach (var klasse in klasser)
            {
                SummaryClass summaryClass = new SummaryClass();
                summary.Classes.Add(summaryClass);
                summaryClass.ClassName = klasse;
            foreach (var item in oppgjor.Resultater)
                {
                    if (item.Ovelse.Klasse == klasse)
                    {
                        summaryClass.TotalParticipants += 1;
                        foreach (var res in item.Ovelse.Serier)
                        {
                            if(!string.IsNullOrEmpty(res.Premie))
                                {
                                    int prem = Int32.Parse(res.Premie);

                                if(prem>0)
                                {
                                    if(PremieOvelse.ContainsKey(res.Id))
                                    {
                                        var mappedName = PremieOvelse[res.Id];
                                        Premie newpremie = null;
                                        foreach (var ItemPremie in summaryClass.Premies)
                                        {
                                            if(ItemPremie.MappedName == mappedName)
                                            {
                                                newpremie= ItemPremie;
                                                ItemPremie.Value += prem;
                                            }
                                        }
                                        if(newpremie==null)
                                        {
                                            newpremie = new Premie
                                            {
                                                Name = res.Id,
                                                Value = prem,
                                                MappedName = mappedName
                                            };
                                            summaryClass.Premies.Add(newpremie);
                                        }
                                    }
                                    else
                                    {
                                        Log.Error("Ukjent øvelse med premie {0} {1} {2} {3}", res.Id, prem, item.Etternavn, item.Fornavn);
                                    }

                                    
                                };
                                    
                                }
                        }
                    }
                }
            }
            return summary;
        }
    }
}
