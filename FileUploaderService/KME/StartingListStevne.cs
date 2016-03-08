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
            Rapporter = new List<RapportXmlClass>();
            ToppListInfoWithRef = new List<RapportXmlClass>();
            DynamiskeBaner = new List<StartListBane>();
            TopLister = new List<string>();
        }

        public StartingListStevne(StartingListStevne cpy)
        {
            StevneNavn = cpy.StevneNavn;
            Rapporter = new List<RapportXmlClass>();
            ToppListInfoWithRef = new List<RapportXmlClass>();
            DynamiskeBaner = new List<StartListBane>();
            TopLister = new List<string>();
        }

        public string StevneNavn { get; set; }
        public string ReportDirStevneNavn { get; set; }
    

        [XmlIgnore]
        public List<RapportXmlClass> Rapporter { get; set; }

        [XmlIgnore]
        public List<RapportXmlClass> ToppListInfoWithRef { get; set; }
        

        [XmlIgnore]
        public List<StartListBane> DynamiskeBaner { get; set; }
        
        [XmlIgnore]
        public List<string> TopLister { get; set; }
        
       

        internal void ParseStevneNavn(string navneText)
        {
            if (!string.IsNullOrEmpty(navneText))
            {
                this.StevneNavn = navneText;
                this.ReportDirStevneNavn = ParseHelper.RemoveDirLetters(navneText);
            }
        }

        //internal StevneType LagType(string lagText)
        //{
        //    if (!string.IsNullOrEmpty(lagText))
        //    {
        //        if (lagText.Contains(Constants.Prefix100m))
        //        {
        //            return StevneType.Hundremeter;
        //        }

        //        if (lagText.Contains(Constants.Prefix200m))
        //        {
        //            return StevneType.Tohundremeter;
        //        }

        //        return StevneType.Femtenmeter;
        //    }

        //    return StevneType.Undefined;
        //}



        internal StartListBane FinnBane(BaneType baneType)
        {

            if (DynamiskeBaner == null)
            {
                DynamiskeBaner = new List<StartListBane>();
                return null;
            }

            StartListBane funnetBane= null;
            foreach (var stevner in DynamiskeBaner)
            {
                if (stevner.BaneType == baneType)
                {
                    funnetBane = stevner;
                    break;
                }
            }

            return funnetBane;
        }

        internal StartListBane AddNewBane(string navn, string reportdirnavn, BaneType baneType)
        {
            if (this.DynamiskeBaner == null)
            {
                this.DynamiskeBaner = new List<StartListBane>();
            }

            StartListBane funnetStevne = null;
            foreach (var stevner in this.DynamiskeBaner)
            {
                if (stevner.BaneType == baneType)
                {
                    funnetStevne = stevner;
                    break;
                }
            }

            if (funnetStevne == null)
            {
                var newStevne = new StartListBane();
                newStevne.StevneNavn = navn;
                newStevne.BaneType = baneType;
                  this.DynamiskeBaner.Add(newStevne);
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

            StartListBane funnetBane = null;
            foreach (var bane in this.DynamiskeBaner)
            {
                if (bane.BaneType == nyttlag.BaneType)
                {
                    funnetBane = bane;
                    break;
                }
            }

            if (funnetBane != null)
            {
                funnetBane.StevneLag.Add(nyttlag);

            }
            else
            {
                Log.Error("Fant ikke stevne type {0} navn={1}", nyttlag.BaneType, nyttlag.StevneNavn);
            }
        }

        
    }
}
