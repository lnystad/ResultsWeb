using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Orion
{
    public class Skive
    {
        public DateTime? ArrangeTime { get; set; }
        public DateTime? ArrangeDate { get; set; }
        public OrionFileInfo Info { get; set; }
        public int? SkiveNr { get; set; }

        public Skive(OrionFileInfo info)
        {
            ArrangeDate = info.FileInfo.CreationTime;
            ArrangeTime = new DateTime(info.FileInfo.CreationTime.Year,
                info.FileInfo.CreationTime.Month,
                info.FileInfo.CreationTime.Day,
                info.FileInfo.CreationTime.Hour,
                info.FileInfo.CreationTime.Minute,
                0);
            SkiveNr = info.Skive;
            Info = info;
        }

        public Skive(Skive cpy)
        {
            ArrangeDate = cpy.ArrangeDate;
            ArrangeTime = cpy.ArrangeTime;
            SkiveNr = cpy.SkiveNr;
            Info = cpy.Info;
        }
        
    }
}
