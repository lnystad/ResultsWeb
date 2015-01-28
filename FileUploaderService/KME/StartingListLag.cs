using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    using System.IO;
    using System.Xml.Serialization;

    using SendingResults.Diagnosis;

    public class StartingListLag : IEquatable<StartingListLag>
    {
        public StartingListLag()
        {
            Skiver = new List<StartingListSkive>();
        }

        public StartingListLag(int lagnr)
        {
            Skiver = new List<StartingListSkive>();
            LagNr = lagnr;
        }

        public StartingListLag(StartingListLag cpy)
        {
            Skiver = new List<StartingListSkive>();
            LagNr = cpy.LagNr;
            StevneNavn = cpy.StevneNavn;
            StartTime = cpy.StartTime;
        }

        public string XmlOppropsListe { get; set; } 
      
        public int LagNr { get; set; }
        public string StevneNavn { get; set; }
        //public StevneType StevneType { get; set; }

        public BaneType BaneType { get; set; }

        public ProgramType ProgramType { get; set; }

        public DateTime? StartTime { get; set; }

        //public HtmlAgilityPack.HtmlDocument Gravlappliste { get; set; }

         [XmlArray("AllSkiver")]
         [XmlArrayItem("Skive")]
        public List<StartingListSkive> Skiver { get; set; }

        internal bool ParseLagInfo(string infoString)
        {
            var infoPart=infoString.Split(new[] { '-' });
            if (infoPart.Length > 2)
            {
                var lagInfo = infoPart[0].Trim();
                string dayName ;
                int day = -1;
                int month = -1;
                int hour = -1;
                int min = -1;
                int res = -1;
                var startDate = infoPart[1].Trim();
                var dateParts=startDate.Split(new[] { '/',' ' });
                if (dateParts.Length > 2)
                {
                    if (int.TryParse(dateParts[1], out res))
                    {
                        dayName = dateParts[0];
                    }

                    if (int.TryParse(dateParts[1], out res))
                    {
                        day = res;
                    }
                    res = -1;
                    if (int.TryParse(dateParts[2], out res))
                    {
                        month = res;
                    }
                }
                var startTime = infoPart[2].Trim();
                var startTimeParts = startTime.Split(new[] { ':' });
                if (startTimeParts.Length > 1)
                {
                    if (int.TryParse(startTimeParts[0], out res))
                    {
                        hour = res;
                    }

                    res = -1;
                    if (int.TryParse(startTimeParts[1], out res))
                    {
                        min = res;
                    }
                }

                if (day > 0 && month > 0 && hour > -1 && min > -1)
                {
                    if (month > DateTime.Now.Month)
                    {
                        StartTime = new DateTime(DateTime.Now.Year-1, month, day, hour, min, 0);
                    }
                    else
                    {
                        StartTime = new DateTime(DateTime.Now.Year, month, day, hour, min, 0);
                    }
                }
            }

            return true;
        }

        public bool Equals(StartingListLag other)
        {
            return other.LagNr == this.LagNr && other.ProgramType == ProgramType;
        }

        public void SortSkiver()
        {

            this.Skiver = this.Skiver.OrderBy(x => x.SkiveNr).ToList();
        }

        internal void SetNotCheckOnAllSkiver()
        {
            foreach (var skive in Skiver)
            {
                skive.Checked = false;
            }
        }

        internal int CountBitmapFiles()
        {
            int count = 0;
            foreach (var skive in Skiver)
            {
                if (skive.BackUpBitMapFile != null)
                {
                    count++;
                }
            }

            return count;
        }



        internal static DateTime? ParseStartDate(string stevneNavn, string datoString)
        {
            DateTime? oppropsTid = null;

            if (string.IsNullOrEmpty(datoString))
            {
                Log.Warning("Dato for stevne er tom {0}", stevneNavn);
                return null;
            }

            DateTime year = new DateTime(DateTime.Now.Year,1,1);
            if (!string.IsNullOrEmpty(stevneNavn))
            {
                int maxYear = 2010;
                var datePart = stevneNavn.Split(new[] { ' ' });
                foreach (var stringParts in datePart)
                {
                    int testYear = 0;
                    if (int.TryParse(stringParts, out testYear))
                    {
                        if (testYear > 2010 && testYear < 2050)
                        {
                            if (testYear > maxYear)
                            {
                                maxYear = testYear;
                            }
                        }
                    }
                }

                if (maxYear != 2010)
                {
                    year = new DateTime(maxYear, 1, 1);
                }
            }

            var infoPart = datoString.Split(new[] { ' ' });
            if (infoPart.Length > 1)
            {
               
                string dayName = infoPart[0].Trim();
                int day = -1;
                int month = -1;
                int hour = -1;
                int min = -1;
                int res = -1;
                var startDate = infoPart[1].Trim();
                var dateParts = startDate.Split(new[] { '/', ' ' });
                if (dateParts.Length > 1)
                {
                    if (int.TryParse(dateParts[0], out res))
                    {
                        day = res;
                    }
                    res = -1;
                    if (int.TryParse(dateParts[1], out res))
                    {
                        month = res;
                    }
                }

                if (day > 0 && month > 0 )
                {
                    //LANTODO
                    //if (month > DateTime.Now.Month)
                    //{
                    //    oppropsTid = new DateTime(year.Year - 1, month, day, 0, 0, 0);
                    //}
                    //else
                    //{
                        oppropsTid = new DateTime(year.Year, month, day, 0, 0, 0);
                    //}
                }
            }

            return oppropsTid;
        }

        internal static DateTime? ParseStartDateLagSkyting(string stevneNavn, string datoString)
        {
            DateTime? oppropsTid = null;

            if (string.IsNullOrEmpty(datoString))
            {
                Log.Warning("Dato for stevne er tom {0}", stevneNavn);
                return null;
            }

            DateTime year = new DateTime(DateTime.Now.Year, 1, 1);
            if (!string.IsNullOrEmpty(stevneNavn))
            {
                int maxYear = 2010;
                var datePart = stevneNavn.Split(new[] { ' ' });
                foreach (var stringParts in datePart)
                {
                    int testYear = 0;
                    if (int.TryParse(stringParts, out testYear))
                    {
                        if (testYear > 2010 && testYear < 2050)
                        {
                            if (testYear > maxYear)
                            {
                                maxYear = testYear;
                            }
                        }
                    }
                }

                if (maxYear != 2010)
                {
                    year = new DateTime(maxYear, 1, 1);
                }
            }

            var infoPart = datoString.Split(new[] { '-' });
            if (infoPart.Length >= 3)
            {

                string dayName = infoPart[1].Trim();
                int day = -1;
                int month = -1;
                int hour = -1;
                int min = -1;
                int res = -1;
                var startDate = infoPart[1].Trim();
                var dateParts = startDate.Split(new[] { '/', ' ' });
                if (dateParts.Length > 2)
                {
                    if (int.TryParse(dateParts[1], out res))
                    {
                        day = res;
                    }
                    res = -1;
                    if (int.TryParse(dateParts[2], out res))
                    {
                        month = res;
                    }
                }

                var startTimeParts = infoPart[2].Trim();
                var hourParts = startTimeParts.Split(new[] { ':' });
                if (hourParts.Length > 1)
                {
                    if (int.TryParse(hourParts[0], out res))
                    {
                        hour = res;
                    }
                    res = -1;
                    if (int.TryParse(hourParts[1], out res))
                    {
                        min = res;
                    }
                }

                if (day > 0 && month > 0)
                {
                    //LANTODO
                    //if (month > DateTime.Now.Month)
                    //{
                    //    oppropsTid = new DateTime(year.Year - 1, month, day, 0, 0, 0);
                    //}
                    //else
                    //{
                    oppropsTid = new DateTime(year.Year, month, day, hour, min, 0);
                    //}
                }
            }

            return oppropsTid;
        }


        internal static DateTime? ParseStartTime(DateTime? oppropsDato, string klokke)
        {
            if (string.IsNullOrEmpty(klokke))
            {
                return oppropsDato;
            }
            if (oppropsDato == null)
            {
                oppropsDato= new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
            }
            var infoPart = klokke.Split(new[] { ':' });
            if (infoPart.Length > 1)
            {
                int hour = -1;
                int min = -1;

                int res = -1;
                if (int.TryParse(infoPart[0], out res))
                {
                    hour = res;
                }

                res = -1;
                if (int.TryParse(infoPart[1], out res))
                {
                    min = res;
                }

                if (hour > -1 && min > -1)
                {
                    return new DateTime(oppropsDato.Value.Year,oppropsDato.Value.Month,oppropsDato.Value.Day, hour, min,0);
                }
            }

            return oppropsDato;
        }

        internal static int ParseLagNr(string lagstreng)
        {
            if (string.IsNullOrEmpty(lagstreng))
            {
                return -1;
            }
            if (lagstreng.StartsWith("Lag"))
            {
                var split = lagstreng.Split(new[] { ' ' });
                if(split.Length > 1)
                {
                    int res = -1;
                    if (int.TryParse(split[1].Trim(), out res))
                    {
                        return res;
                    }


                }
            }

             return -1;
            
        }

        internal static ProgramType ParseOvelse(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                string ovelse = text.Trim().ToUpper();
                switch (ovelse)
                {
                    case "25 SKUDD":
                        return ProgramType.Innledende;
                        break;
                    case "INNLEDENDE":
                        return ProgramType.Innledende;
                        break;
                    case "FINALE":
                        return ProgramType.Finale;
                        break;
                    //case "LAGSKYTING":
                    //    return ProgramType.Lagskyting;
                    //    break;
                    //case "SAMLAGSSKYTING":
                    //    return ProgramType.SamLagskyting;
                    //    break;
                    //case "NY GRUPPE":
                    //    return ProgramType.Lagskyting;
                    //    break;
                }
            }

            return ProgramType.UnKnown;
        }

       
    }
}
