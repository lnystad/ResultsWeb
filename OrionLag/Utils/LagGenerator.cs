using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Utils
{
    using OrionLag.Input.Data;
    using OrionLag.Input.ViewModel;

    public class LagGenerator
    {
        public List<Lag> GenererSimpelLag(List<List<InputData>> alleSkytterePrKlasse, List<KlasseSort> configklasse, int startLag, int antallskiver, int? GenerateSpaceEach,bool GenerateEmptyLagIsChecked,bool spaceafterKlasse)
        {
            List<Lag> retValue = new List<Lag>();
            bool space = spaceafterKlasse;
            int CurrentLagNo = startLag;
            Lag CurrentLag = new Lag();
            CurrentLag.LagNummer= CurrentLagNo;
            CurrentLag.MaxSkiveNummer= antallskiver;

          

            int CurrentSkivenr = 1;
            int TotalCount = 1;
            bool alredyadded = false;
            int totalLag = 0;
            foreach (var Lagvis in alleSkytterePrKlasse)
            {
                var linkslist = new List<InputData>();
                
                    linkslist = Lagvis.FindAll(x => x.Links == true).ToList();
                    int count = Lagvis.RemoveAll(x => x.Links == true);
                    if (linkslist.Count != count)
                    {
                        //WTF
                    }
                

                totalLag++;
                int GenerateSpace = 0;
                if (Lagvis.Count > 0)
                {
                    string currentKlasse = Lagvis[0].Klasse;
                    var el=configklasse.FirstOrDefault(x => x.Klasse == currentKlasse);
                    if (el != null)
                    {
                        GenerateSpace = el.SpaceInLag;
                    }
                }
                bool firstLinksFinfelt = false;
                int skytteriklassen = 0;
                Skytter prevSkytter = null;
                foreach (var skytterIn in Lagvis)
                {
                    skytteriklassen++;
                   

                    var SKive = new Skiver(CurrentSkivenr);
                    if (CurrentSkivenr == CurrentLag.MaxSkiveNummer && linkslist.Count > 0 )
                    {
                        var linkSkytter = linkslist[0];
                        linkslist.RemoveAt(0);
                        SKive.Skytter = new Skytter()
                                            {
                                                Klasse = linkSkytter.Klasse,
                                                Name = linkSkytter.Name,
                                                Skytterlag = linkSkytter.Skytterlag,
                                                Links = linkSkytter.Links,
                                                SkytterNr = linkSkytter.SkytterNr,
                                                InputXmlData = linkSkytter.InputXmlData
                                            };
                        prevSkytter = SKive.Skytter;
                        CurrentLag.SkiverILaget.Add(SKive);
                        alredyadded = true;
                        retValue.Add(CurrentLag);
                        CurrentLagNo++;

                        CurrentLag = new Lag();
                        CurrentLag.LagNummer = CurrentLagNo;
                        CurrentLag.MaxSkiveNummer = antallskiver;
                        CurrentSkivenr = 1;
                        var SKiveNy = new Skiver(CurrentSkivenr);
                        SKiveNy.Skytter = new Skytter()
                        {
                            Klasse = skytterIn.Klasse,
                            Name = skytterIn.Name,
                            Skytterlag = skytterIn.Skytterlag,
                            Links = skytterIn.Links,
                            SkytterNr = skytterIn.SkytterNr,
                            InputXmlData = skytterIn.InputXmlData
                        };
                        prevSkytter = SKive.Skytter;
                        CurrentLag.SkiverILaget.Add(SKiveNy);

                    }
                    else
                    {
                        
                        SKive.Skytter = new Skytter()
                                            {
                                                Klasse = skytterIn.Klasse,
                                                Name = skytterIn.Name,
                                                Skytterlag = skytterIn.Skytterlag,
                                                Links = skytterIn.Links,
                                                SkytterNr = skytterIn.SkytterNr,
                                                InputXmlData = skytterIn.InputXmlData
                                            };
                        prevSkytter = SKive.Skytter;
                        CurrentLag.SkiverILaget.Add(SKive);
                    }

                    CurrentSkivenr ++;
                   

                    TotalCount++;
                    if (GenerateSpace != 0)
                    {
                        if (TotalCount % GenerateSpace == 0)
                        {
                            CurrentSkivenr++;
                        }
                    }

                    if (CurrentSkivenr > antallskiver)
                    {
                        alredyadded = true;
                        retValue.Add(CurrentLag);
                        bool lastlinks = false;
                        
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
                   
                    

                    if (space)
                    {
                        retValue.Add(CurrentLag);
                        CurrentLagNo++;
                        CurrentSkivenr = 1;
                        CurrentLag = new Lag();
                        CurrentLag.LagNummer = CurrentLagNo;
                        CurrentLag.MaxSkiveNummer = antallskiver;
                    }
                    else
                    {
                        
                    }

                    

                }
                else
                {
                    if (space)
                    {
                        CurrentSkivenr = 1;
                    }

                    CurrentLag = new Lag();
                    CurrentLag.LagNummer = CurrentLagNo;
                    CurrentLag.MaxSkiveNummer = antallskiver;
                }

               
                
               

            }

            return retValue;
        }

        public List<Lag> GenererSimpelLagFinfelt(List<List<InputData>> alleSkytterePrKlasse, List<KlasseSort> configklasse, int startLag, int antallskiver)
        {
            List<Lag> retValue = new List<Lag>();

            int CurrentLagNo = startLag;
            Lag CurrentLag = new Lag();
            CurrentLag.LagNummer = CurrentLagNo;
            CurrentLag.MaxSkiveNummer = antallskiver;



            int CurrentSkivenr = 1;
            int TotalCount = 1;
            bool alredyadded = false;
            int totalLag = 0;
            foreach (var Lagvis in alleSkytterePrKlasse)
            {
                var linkslist = new List<InputData>();
                // Denne gir oss linksskytterne først i hver klasse
                InputDataComparerLinksFelt links = new InputDataComparerLinksFelt();
                Lagvis.Sort(links);
                totalLag++;
                bool firstLinksFinfelt = false;
                int skytteriklassen = 0;
                //Tuple<int,Skytter> prevSkytter = null;
                foreach (var skytterIn in Lagvis)
                {
                    skytteriklassen++;
                    Skytter prevSKive1 = null;
                    if (!skytterIn.Links)
                    {
                        if (CurrentSkivenr == 1)
                        {
                            prevSKive1 = this.FindPrevSKytterSkive1(retValue, CurrentLagNo);
                            
                            if (prevSKive1!=null && prevSKive1.Links)
                            {
                                var nySKive = new Skiver(CurrentSkivenr);
                                nySKive.Skytter = new Skytter()
                                {
                                    Klasse = skytterIn.Klasse,
                                    Name = skytterIn.Name,
                                    Skytterlag = skytterIn.Skytterlag,
                                    Links = skytterIn.Links,
                                    SkytterNr = skytterIn.SkytterNr,
                                    InputXmlData = skytterIn.InputXmlData
                                };
                                CurrentLag.SkiverILaget.Add(nySKive);
                                retValue.Add(CurrentLag);
                                CurrentLagNo++;
                                CurrentLag = new Lag();
                                CurrentLag.LagNummer = CurrentLagNo;
                                CurrentLag.MaxSkiveNummer = antallskiver;
                                alredyadded = false;
                                CurrentSkivenr = 1;
                                continue;
                            }
                        }
                        else if (CurrentSkivenr == 2)
                        {
                            prevSKive1 = this.FindPrevSKytterSkive1(retValue, CurrentLagNo);
                            if (prevSKive1 != null && prevSKive1.Links)
                            {
                                CurrentSkivenr = 1;
                                retValue.Add(CurrentLag);
                                CurrentLagNo++;
                                CurrentLag = new Lag();
                                CurrentLag.LagNummer = CurrentLagNo;
                                CurrentLag.MaxSkiveNummer = antallskiver;
                                alredyadded = false;
                            }
                        }
                    }
                         
                    var SKive = new Skiver(CurrentSkivenr);
                    SKive.Skytter = new Skytter()
                    {
                        Klasse = skytterIn.Klasse,
                        Name = skytterIn.Name,
                        Skytterlag = skytterIn.Skytterlag,
                        Links = skytterIn.Links,
                        SkytterNr = skytterIn.SkytterNr,
                        InputXmlData = skytterIn.InputXmlData
                    };

                    if (CurrentLag.SkiverILaget.Count == CurrentLag.MaxSkiveNummer)
                    {
                        retValue.Add(CurrentLag);
                        CurrentLagNo++;
                        CurrentLag = new Lag();
                        CurrentLag.LagNummer = CurrentLagNo;
                        CurrentLag.MaxSkiveNummer = antallskiver;
                        alredyadded = false;
                        CurrentSkivenr = 1;
                        SKive.SkiveNummer = CurrentSkivenr;
                    }

                    CurrentLag.SkiverILaget.Add(SKive);
                    CurrentSkivenr++;
                    if (CurrentSkivenr > CurrentLag.MaxSkiveNummer)
                    {
                        retValue.Add(CurrentLag);
                        CurrentLagNo++;
                        CurrentLag = new Lag();
                        CurrentLag.LagNummer = CurrentLagNo;
                        CurrentLag.MaxSkiveNummer = antallskiver;
                        alredyadded = false;
                        CurrentSkivenr = 1;
                    }
                }
            }

            if (!alredyadded)
            {
                retValue.Add(CurrentLag);
            }

            return retValue;
        }

        private Skytter FindPrevSKytterSkive1(List<Lag> retValue, int currentLagNo)
        {
            var lag = retValue.FirstOrDefault(x => x.LagNummer == currentLagNo - 1);
            if (lag != null)
            {
                Skiver sk = lag.SkiverILaget.FirstOrDefault(x => x.SkiveNummer == 1);
                if (sk != null)
                {
                    return sk.Skytter;
                }
            }

            return null;
        }

        private int FindMaxSKiveNo(List<Skiver> skiverILaget)
        {
            int maxSKiveNo = 0;

            foreach (var skive in skiverILaget)
            {
                if (skive.SkiveNummer > maxSKiveNo)
                {
                    maxSKiveNo = skive.SkiveNummer;
                }
            }

            return maxSKiveNo;
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
