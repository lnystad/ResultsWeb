using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    using System.Globalization;
    using System.IO;

    public class LagTimeInfo
    {

        public LagTimeInfo()
        {
            
        }

        public LagTimeInfo(int lagNr, DateTime? generateTime)
        {
            LagNr = lagNr;
            GenerateTime = generateTime;
        }

        public string StevneNavnInDirName { get; set; }

        public int LagNr { get; set; }

        public DateTime? GenerateTime { get; set; }

        public DirectoryInfo Directory { get; set; }

        internal static LagTimeInfo ParseTimeInfo(string name)
        {
            var parts = name.Split(new[] { ' ' });
            if (parts.Length > 1)
            {
                int nummer = -1;
                int.TryParse(parts[0].Substring(3), out nummer);
                string format = "yyyyMMdd-HHmm";
                DateTime dateTime;
                string timeInfo = parts[1].Substring(0, 13);
                string rest = parts[1].Substring(13);
                if (!string.IsNullOrEmpty(rest))
                {
                    rest=rest.Trim(new[] { '-' });
                }
                if (parts.Length > 2)
                {
                    int i = 2;
                    while(i<parts.Length) 
                    {
                        rest = rest + " " + parts[i];
                        i++;
                    }
                }
                if (!string.IsNullOrEmpty(rest))
                {
                    rest = rest.Replace(',', ' ');
                    rest = rest.Replace('/', ' ');
                    rest = rest.Replace('\\', ' ');
                }

                if (DateTime.TryParseExact(timeInfo, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    if (nummer > 0)
                    {
                        var ret=new LagTimeInfo(nummer,dateTime);
                        if (!string.IsNullOrEmpty(rest))
                        {
                            ret.StevneNavnInDirName = rest;
                        }
                        return ret;
                    }
                }

            }

            return null;
        }
    }
}
