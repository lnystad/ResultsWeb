using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    using System.Xml.Serialization;

    using FileUploaderService.Utils;

    using SendingResults.Diagnosis;

    public class StartingListStevne
    {
        public StartingListStevne()
        {
            StevneLag15m = new List<StartingListLag>();
            StevneLag100m = new List<StartingListLag>();
            StevneLag200m = new List<StartingListLag>();
            StevneLag = new List<StartingListLag>();
            Rapporter = new List<RapportXmlClass>();
        }

        public StartingListStevne(StartingListStevne cpy)
        {
            StevneLag15m = new List<StartingListLag>();
            StevneLag100m = new List<StartingListLag>();
            StevneLag200m = new List<StartingListLag>();
            StevneLag = new List<StartingListLag>();

            StevneNavn = cpy.StevneNavn;
            StartDate15m = cpy.StartDate15m;
            EndDate15m = cpy.EndDate15m;
            StartDate100m = cpy.StartDate100m;
            EndDate100m = cpy.EndDate100m;
            StartDate200m = cpy.StartDate200m;
            EndDate200m = cpy.EndDate200m;
            Rapporter = new List<RapportXmlClass>();
        }

        public string StevneNavn { get; set; }
        public StevneType StevneType { get; set; }
        public DateTime? StartDate15m { get; set; }
        public DateTime? EndDate15m { get; set; }
        [XmlIgnore]
        public List<StartingListLag> StevneLag15m { get; set; }

        public DateTime? StartDate100m { get; set; }
        public DateTime? EndDate100m { get; set; }
         [XmlIgnore]
        public List<StartingListLag> StevneLag100m { get; set; }

        public DateTime? StartDate200m { get; set; }
        public DateTime? EndDate200m { get; set; }
        [XmlIgnore]
        public List<StartingListLag> StevneLag200m { get; set; }

        [XmlArray("AllLag")]
        [XmlArrayItem("Laget")]
        public List<StartingListLag> StevneLag { get; set; }

         [XmlIgnore]
        public List<RapportXmlClass> Rapporter { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [XmlIgnore]
        public List<StartingListStevne> DynamiskeStevner { get; set; }

        internal void SetStartAndEndDate()
        {
            if (StevneLag15m != null)
            {
                var list = StevneLag15m.OrderBy(x => x.StartTime);
                var time = list.FirstOrDefault();
                if (time != null && time.StartTime.HasValue)
                {
                    this.StartDate15m = time.StartTime.Value.Date;
                }

                var listend = StevneLag15m.OrderByDescending(x => x.StartTime);
                var timeend = listend.FirstOrDefault();
                if (timeend != null && timeend.StartTime.HasValue)
                {
                    this.EndDate15m = timeend.StartTime.Value.Date;
                }
            }
        }

        internal void SetStartAndEndDate(StartingListStevne stevne)
        {
            if (stevne != null)
            {
                var list = stevne.StevneLag.OrderBy(x => x.StartTime);
                var time = list.FirstOrDefault();
                if (time != null && time.StartTime.HasValue)
                {
                    stevne.StartDate = time.StartTime.Value.Date;
                }

                var listend = stevne.StevneLag.OrderByDescending(x => x.StartTime);
                var timeend = listend.FirstOrDefault();
                if (timeend != null && timeend.StartTime.HasValue)
                {
                    stevne.EndDate = timeend.StartTime.Value.Date;
                }
            }
        }

        internal int CountBitmapFiles(StevneType stevnetype)
        {
            int count = 0;
            switch (stevnetype)
            {
                case StevneType.Femtenmeter:
                    foreach (var lag in StevneLag15m)
                    {
                        count = count + lag.CountBitmapFiles();
                    }
                    break;
                case StevneType.Hundremeter:
                    foreach (var lag in StevneLag100m)
                    {
                        count = count + lag.CountBitmapFiles();
                    }
                    break;
                case StevneType.Tohundremeter:
                    foreach (var lag in StevneLag200m)
                    {
                        count = count + lag.CountBitmapFiles();
                    }
                    break;
            }


            return count;
        }

        internal void ParseStevneNavn(string navneText)
        {
            if (!string.IsNullOrEmpty(navneText))
            {
                this.StevneNavn = ParseHelper.RemoveDirLetters(navneText);

            }
        }

        internal StevneType LagType(string lagText)
        {
            if (!string.IsNullOrEmpty(lagText))
            {
                if (lagText.Contains(Constants.Prefix100m))
                {
                    return StevneType.Hundremeter;
                }

                if (lagText.Contains(Constants.Prefix200m))
                {
                    return StevneType.Tohundremeter;
                }

                return StevneType.Femtenmeter;
            }

            return StevneType.Undefined;
        }



        internal StartingListStevne FinnStevne(StevneType stevneType)
        {

            if (DynamiskeStevner == null)
            {
                DynamiskeStevner = new List<StartingListStevne>();
                return null;
            }

            StartingListStevne funnetStevne = null;
            foreach (var stevner in DynamiskeStevner)
            {
                if (stevner.StevneType == stevneType)
                {
                    funnetStevne = stevner;
                    break;
                }
            }

            return funnetStevne;
        }

        internal StartingListStevne AddNewStevne(string navn, StevneType stevneType)
        {
            if (this.DynamiskeStevner == null)
            {
                this.DynamiskeStevner = new List<StartingListStevne>();
            }

            StartingListStevne funnetStevne = null;
            foreach (var stevner in this.DynamiskeStevner)
            {
                if (stevner.StevneType == stevneType)
                {
                    funnetStevne = stevner;
                    break;
                }
            }

            if (funnetStevne == null)
            {
                var newStevne = new StartingListStevne();
                newStevne.StevneNavn = navn;
                newStevne.StevneType = stevneType;
                this.DynamiskeStevner.Add(newStevne);
                return newStevne;
            }

            return null;
        }

        internal void AddLag(StartingListLag nyttlag)
        {
            if (nyttlag == null)
            {
                return;
            }

            StartingListStevne funnetStevne = null;
            foreach (var stevner in this.DynamiskeStevner)
            {
                if (stevner.StevneType == nyttlag.StevneType)
                {
                    funnetStevne = stevner;
                    break;
                }
            }

            if (funnetStevne != null)
            {
                funnetStevne.StevneLag.Add(nyttlag);
               
            }
            else
            {
                Log.Error("Fant ikke stevne type {0} navn={1}", nyttlag.StevneType, nyttlag.StevneNavn);
            }
        }
    }
}
