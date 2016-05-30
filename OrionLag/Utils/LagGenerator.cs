using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Utils
{
    using OrionLag.Input.Data;

    public class LagGenerator
    {
        public List<Lag> GenererSimpelLag(List<List<InputData>> alleSkytterePrKlasse, int startLag, int antallskiver, int? GenerateSpaceEach)
        {
            List<Lag> retValue = new List<Lag>();

            int CurrentLagNo = startLag;
            Lag CurrentLag = new Lag();
            CurrentLag.LagNummer= CurrentLagNo;
            CurrentLag.MaxSkiveNummer= antallskiver;
            
            int CurrentSkivenr = 1;
            int TotalCount = 1;

            foreach (var Lagvis in alleSkytterePrKlasse)
            {
                bool alredyadded = false;
                foreach (var skytterIn in Lagvis)
                {
                    var SKive = new Skiver(CurrentSkivenr);
                    SKive.Skytter = new Skytter()
                    {
                        Klasse = skytterIn.Klasse,
                        Name = skytterIn.Name,
                        Skytterlag = skytterIn.Skytterlag,
                        SkytterNr = skytterIn.SkytterNr,InputXmlData = skytterIn.InputXmlData

                    };
                    
                    CurrentLag.SkiverILaget.Add(SKive);
                    CurrentSkivenr ++;
                    TotalCount++;
                    if (GenerateSpaceEach.HasValue && GenerateSpaceEach.Value != 0)
                    {
                        if (TotalCount % GenerateSpaceEach.Value == 0)
                        {
                            CurrentSkivenr++;
                        }
                    }

                    if (CurrentSkivenr > antallskiver)
                    {
                        alredyadded = true;
                        retValue.Add(CurrentLag);
                        CurrentLagNo++;

                        CurrentLag = new Lag();
                        CurrentLag.LagNummer = CurrentLagNo;
                        CurrentLag.MaxSkiveNummer = antallskiver;
                        CurrentSkivenr = 1;
                    }
                    else
                    {
                        alredyadded = false;
                    }
                    
                }

                if (!alredyadded)
                {
                    retValue.Add(CurrentLag);
                    CurrentLagNo++;
                }

                CurrentLag = new Lag();
                CurrentLag.LagNummer = CurrentLagNo;
                CurrentLag.MaxSkiveNummer = antallskiver;
                CurrentSkivenr = 1;

            }

            return retValue;
        }


        public List<Lag> GenererLag(List<InputData> data, int antallSkiver, int antallskyttereilaget, int antallHold,  bool avbrekk)
        {
            List<Lag> retVal = new List<Lag>();
            if (data == null)
            {
                return retVal;
            }

            if (data.Count == 0)
            {
                return retVal;
            }
           
            int StartSkytter = 0;
            foreach (var skytterIn in data)
            {
                int hold = 0;
                int LagNr = StartSkytter / antallskyttereilaget + 1 ;
                int forsteSkive = StartSkytter % antallskyttereilaget+1;
                while (hold < antallHold)
                {
                    Lag lag = GetLagNr(retVal, LagNr, antallSkiver);
                    int SkiveNr = (hold*antallskyttereilaget)+forsteSkive;
                    var skive = lag.FinnLedigSkive(SkiveNr);
                    skive.Skytter = new Skytter()
                                        {
                                            Klasse = skytterIn.Klasse,
                                            Name = skytterIn.Name,
                                            Skytterlag = skytterIn.Skytterlag,
                                            SkytterNr = skytterIn.SkytterNr
                                        };
                    if (avbrekk)
                    {
                        LagNr = LagNr + 2;
                    }
                    else
                    {
                        LagNr = LagNr + 1;
                    }

                    hold++;
                }

                StartSkytter++;

            }


            return retVal;
        }

        private Lag GetLagNr(List<Lag> retVal, int lagNr, int maxSkiveriLaget)
        {
            var foudLag = retVal.FirstOrDefault(x => x.LagNummer == lagNr);
            if (foudLag == null)
            {
                foudLag = new Lag(lagNr, maxSkiveriLaget);
                retVal.Add(foudLag);
            }

            return foudLag;
        }

       
    }
}
