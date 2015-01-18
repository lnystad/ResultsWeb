using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Orion
{
    using SendingResults.Diagnosis;

    public class Lag
    {
        public DateTime ArrangeTime { get; set; }
        public DateTime ArrangeDate { get; set; }
        public int? LagNr { get; set; }

        public List<Skive> Skiver { get; set; }
        public List<Skive> ThrashSkiver { get; set; }
        public Lag(DateTime arrangeTime, int? lagNr)
        {
            ArrangeDate = arrangeTime.Date;
            ArrangeTime = new DateTime(arrangeTime.Year,
                arrangeTime.Month,
                arrangeTime.Day,
                arrangeTime.Hour,
                arrangeTime.Minute,
                0);
            LagNr = lagNr;
            Skiver = new List<Skive>();
            ThrashSkiver = new List<Skive>();
        }
     
        internal void InsertSkive(OrionFileInfo inf)
        {
            var skive = new Skive(inf);
            var Skive = Skiver.FirstOrDefault(x => x.SkiveNr == inf.Skive);
            if (Skive != null)
            {
                Log.Warning("Skive already added {0} lag={1} Date={2}", inf.Skive, inf.Lag,inf.EventDateTime);
                ThrashSkiver.Add(skive);
            }
            else
            {
                Skiver.Add(skive); 
            }
     
        }

        internal void InsertSkive(Skive cpySkive)
        {
            if (Skiver == null)
            {
                Skiver = new List<Skive>();
            }

            Skiver.Add(cpySkive); 
        }
    }
}
