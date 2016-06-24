using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.ListeSort
{
    using System.Web.Hosting;
    using System.Windows.Forms;
    using System.Xml;

    using FileUploaderService.Diagnosis;
    using FileUploaderService.KME;

    public class ParseXmlOpp
    {

        public StartListBane ParseOvelse(List<fileOpprop> inputdata)
        {

            StartListBane bane = new StartListBane();
            foreach (var fileinfo in inputdata)
            {
                StartingListLag lag = ParseXmlFile(fileinfo);
                if (lag != null)
                {
                    bane.StevneLag.Add(lag);
                }
            }
            if (bane.StevneLag.Count > 0)
            {
                return bane;
            }

            return null;
        }

        public StartingListLag ParseXmlFile(fileOpprop fileinfo)
        {
            if (fileinfo == null)
            {
                return null;
            }

            if (fileinfo.Info == null)
            {
                return null;
            }

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileinfo.Info.OpenText());
                var ovelsenavn = doc.SelectSingleNode("/report/resulttotsum/@name");
                if (ovelsenavn == null)
                {
                    ovelsenavn = doc.SelectSingleNode("/report/column/series/@name");
                    if (ovelsenavn == null)
                    {
                        return null;
                    }
                }

                BaneType bane= BaneType.Undefined;
                ProgramType programtype = ProgramType.Innledende;
                switch (ovelsenavn.InnerText.ToUpper())
                {
                    case "30-SKUDD":
                        bane = BaneType.FinFelt | BaneType.GrovFelt;
                        break;
                    case "FINALE":
                        programtype = ProgramType.Finale;
                        break;
                    case "MINNE":
                        bane=BaneType.Minne;
                        programtype = ProgramType.Minne;
                        break;
                    case "35 SKUDD":
                        bane = BaneType.Tohundremeter | BaneType.Hundremeter| BaneType.Femtenmeter;
                        break;
                    case "25 SKUDD":
                        bane = BaneType.Tohundremeter | BaneType.Hundremeter | BaneType.Femtenmeter;
                        break;
                    default:
                        Log.Error("Unkown ovelse med Lagoppdeling {0} {1}", ovelsenavn.InnerText, fileinfo.Info.FullName);
                        return null;
                       
                }

                StartingListLag lag = new StartingListLag();
                lag.BaneType = bane;
                lag.ProgramType = programtype;
                var LagNavn = doc.SelectSingleNode("/report/header/@name");
                if (LagNavn == null)
                {
                    Log.Error("Unkown LagNavn {0}", fileinfo.Info.FullName);
                    return null;
                }

                if (!LagNavn.InnerText.ToUpper().Trim().StartsWith("LAG"))
                {
                    Log.Error("Unkown InnerText LagNavn {0} {1}", LagNavn.InnerText,fileinfo.Info.FullName);
                    return null;
                }
                var lagnostr = LagNavn.InnerText.Substring(3).Trim();
                int lagno = -1;

                if (!int.TryParse(lagnostr, out lagno))
                {
                    Log.Error("Unkown InnerText Lago {0} {1}", lagnostr, fileinfo.Info.FullName);
                    return null;
                }
                lag.LagNr = lagno;
                if (lag.LagNr <= 0)
                {
                    Log.Error("LagNr mindre enn null {0} ", lag.LagNr, fileinfo.Info.FullName);
                    return null;
                }
                
                var skyttereNode = doc.SelectNodes("/report/data/result");
                foreach (XmlNode skytter in skyttereNode)
                {
                    try
                    {

                   
                    var skivestr=skytter.SelectSingleNode("@num");
                    var namestr = skytter.SelectSingleNode("@name");
                    var clubstr = skytter.SelectSingleNode("@club");
                    var classstr = skytter.SelectSingleNode("@class");
                    var totsumstr = skytter.SelectSingleNode("@totsum");
                    int skivenr = -1;
                    if (!int.TryParse(skivestr.InnerText, out skivenr))
                    {
                        Log.Error("Unkown InnerText Lago {0}", lagnostr);
                        continue;
                    }

                        var skive = new StartingListSkive() { LagSkiveNr = skivenr,SkiveNr = skivenr };
                        if (classstr != null && !string.IsNullOrEmpty(classstr.InnerText))
                        {
                            skive.Klasse = classstr.InnerText.Trim();
                            string totsum = string.Empty;
                            if (totsumstr != null && !string.IsNullOrEmpty(totsumstr.InnerText))
                            {
                                totsum = totsumstr.InnerText.Trim();
                            }
                            lag.BaneType = ParseBaneTypeFromClass(skive.Klasse, bane, totsum, programtype);
                        }

                        if (clubstr != null && !string.IsNullOrEmpty(clubstr.InnerText))
                        {
                            skive.SkytterLag = clubstr.InnerText.Trim();
                        }

                        if (namestr != null && !string.IsNullOrEmpty(namestr.InnerText))
                        {
                            skive.SkytterNavn = namestr.InnerText.Trim();
                        }

                        skive.XmlFileName = fileinfo.Info.Name;
                        lag.Skiver.Add(skive);
                    }
                    catch (Exception e )
                    {
                        Log.Error(e,"Klarete ikke parse skytter {0} ",  fileinfo.Info.FullName);
                     
                    }
                }
                if (lag.Skiver.Count <= 0)
                {
                    Log.Error("Ingen skiver funnet {0}", lagnostr);
                    return null;
                }
                return lag;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error parsing file {0}", fileinfo.Info.FullName);
                throw;
            }
            
        }

        private BaneType ParseBaneTypeFromClass(string klasse, BaneType inputBane,string totSum, ProgramType programType)
        {
            switch (klasse)
            {
                case "V73":
                case "V65":
                case "ER":
                case "R":
                case "J":
                    if ((inputBane & BaneType.Minne) == BaneType.Minne)
                    {
                        return BaneType.MinneFin;
                    }

                    if ((inputBane & BaneType.FinFelt) == BaneType.FinFelt)
                    {
                        return BaneType.FinFelt;
                    }

                    if ((inputBane & BaneType.Hundremeter) == BaneType.Hundremeter)
                    {
                        return BaneType.Hundremeter;
                    }
                    if (programType == ProgramType.Finale && totSum.Contains("/"))
                    {
                        return BaneType.FinFelt;
                    }
                    break;
             
       
                case "AG3":
                case "HK416":
                case "V55":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                    if ((inputBane & BaneType.Minne) == BaneType.Minne)
                    {
                        return BaneType.MinneGrov;
                    }

                    if ((inputBane & BaneType.GrovFelt) == BaneType.GrovFelt)
                    {
                        return BaneType.GrovFelt;
                    }

                    if ((inputBane & BaneType.Tohundremeter) == BaneType.Tohundremeter)
                    {
                        return BaneType.Tohundremeter;
                    }
                    if (programType == ProgramType.Finale && totSum.Contains("/"))
                    {
                        return BaneType.GrovFelt;
                    }
                    break;
             
            }

            return inputBane;
        }

        public List<StartListBane> GetOppropKat(List<StartListBane> lister)
        {
            List < StartListBane > baner = new List<StartListBane>();
            foreach (var bane in lister)
            {
                if (bane.StevneLag.Count > 0)
                {
                    var banerFunnet = baner.FindAll(x => x.BaneType == bane.StevneLag[0].BaneType);
                    if (banerFunnet.Count==0)
                    {
                        bane.BaneType = bane.StevneLag[0].BaneType;
                        bane.ProgramType= bane.StevneLag[0].ProgramType;
                        baner.Add(bane);
                        
                    }
                    else
                    {
                        bool found = false;
                        foreach (var funnetbane in banerFunnet)
                        {
                            if (funnetbane.ProgramType == bane.StevneLag[0].ProgramType)
                            {
                                found = true;
                                string fil1 = string.Empty;
                                if (funnetbane.StevneLag[0].Skiver.Count > 0)
                                {
                                    fil1 = funnetbane.StevneLag[0].Skiver[0].XmlFileName;
                                }
                                string fil2 = string.Empty;
                                if (bane.StevneLag[0].Skiver.Count > 0)
                                {
                                    fil2 = bane.StevneLag[0].Skiver[0].XmlFileName;
                                }
                                Log.Info(
                                    "Possible double list {0} {1} fil1={2} fil1={3}",
                                    bane.StevneLag[0].BaneType,
                                    bane.StevneLag[0].ProgramType,
                                    fil1,
                                    fil2);
                            }
                        }

                        if (!found)
                        {
                            bane.BaneType = bane.StevneLag[0].BaneType;
                            bane.ProgramType = bane.StevneLag[0].ProgramType;
                            baner.Add(bane);
                        }
                        
                    }
                }
            }

            return baner;
        }
    }
}
