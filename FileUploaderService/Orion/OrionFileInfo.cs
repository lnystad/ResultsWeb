using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Orion
{
    using System.IO;
  
    using SendingResults.Diagnosis;

    public class OrionFileInfo : IEquatable<OrionFileInfo>
    {

        public FileInfo FileInfo { get; set; }
        public int Lag { get; set; }
        public int Skive { get; set; }

        public DateTime EventDate { get; set; }
        public DateTime EventDateTime { get; set; }

        public OrionFileInfo(FileInfo info)
        {
            FileInfo = info;
            EventDate = info.CreationTime.Date;
            EventDateTime = new DateTime(info.CreationTime.Year,
                info.CreationTime.Month,
                info.CreationTime.Day,
                info.CreationTime.Hour,
                info.CreationTime.Minute,
                0);
        }

        public bool Equals(OrionFileInfo other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.FileInfo == null && this.FileInfo == null)
            {
                return true;
            }

            if (other.FileInfo != null && this.FileInfo == null)
            {
                return false;
            }

            if (other.FileInfo == null && this.FileInfo != null)
            {
                return false;
            }

            if (other.FileInfo != null && this.FileInfo != null)
            {
                if (other.FileInfo.Name == this.FileInfo.Name)
                {
                    return true;
                }
            }

            return false;
        }

        internal bool ParseTarget()
        {
            try
            {

            var splitInf = FileInfo.Name.Split(new[] { '-','.' });

            int tall = 0;
            if (int.TryParse(splitInf[1], out tall))
            {
                Lag = tall;
            }
            else
            {
                Lag = -1;
                Log.Warning("Could not parse Lag {0}", FileInfo.Name);
            }

            tall = 0;
            if (int.TryParse(splitInf[2], out tall))
            {
                Skive = tall;
            }

            if (string.Compare(splitInf[3], "PNG", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (Skive > 0 && Lag > 0)
                {
                    return true;
                }
            }

            Skive = -1;
            Log.Warning("Could not parse Skive {0}", FileInfo.Name);

           }
            catch (Exception e )
            {
                Log.Error(e, "Error Parsing {0}", FileInfo.Name);
            }

            return false;
        }

    }
}
