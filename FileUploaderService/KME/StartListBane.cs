// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartListBane.cs" company="">
//   
// </copyright>
// <summary>
//   The start list bane.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.KME
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    using FileUploaderService.Diagnosis;

    /// <summary>
    /// The start list bane.
    /// </summary>
    public class StartListBane
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StartListBane"/> class.
        /// </summary>
        public StartListBane()
        {
            this.StevneLag = new List<StartingListLag>();
            this.BitmapsStoredInBane = new BitmapDirInfo();
            this.BaneRapporter = new List<RapportXmlClass>();
            this.ToppListeRapporter = new List<RapportXmlClass>();
            this.ToppListeLagRapporter = new List<RapportXmlClass>();
            this.ToppListeFilPrefix= new List<string>();
            this.ToppListeLagFilPrefix = new List<string>();
        }

        #endregion

        #region Public Properties

        public BitmapDirInfo BitmapsStoredInBane { get; set; }

        public string ReportDirStevneNavn { get; set; }

        public string StevneNavn { get; set; }

        [XmlIgnore]
        public List<RapportXmlClass> BaneRapporter { get; set; }

        [XmlIgnore]
        public List<RapportXmlClass> ToppListeRapporter { get; set; }

        [XmlIgnore]
        public List<string> ToppListeFilPrefix { get; set; }

        [XmlIgnore]
        public List<string> ToppListeLagFilPrefix { get; set; }

        [XmlIgnore]
        public List<RapportXmlClass> ToppListeLagRapporter { get; set; }
        /// <summary>
        /// Gets or sets the bane type.
        /// </summary>
        public BaneType BaneType { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the stevne lag.
        /// </summary>
        [XmlArray("AllLag")]
        [XmlArrayItem("Laget")]
        public List<StartingListLag> StevneLag { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The set start and end date.
        /// </summary>
        /// <param name="bane">
        /// The bane.
        /// </param>
        internal static void SetStartAndEndDate(StartListBane bane)
        {
            if (bane != null)
            {
                var list = bane.StevneLag.OrderBy(x => x.StartTime);
                var time = list.FirstOrDefault();
                if (time != null && time.StartTime.HasValue)
                {
                    bane.StartDate = time.StartTime.Value.Date;
                }

                var listend = bane.StevneLag.OrderByDescending(x => x.StartTime);
                var timeend = listend.FirstOrDefault();
                if (timeend != null && timeend.StartTime.HasValue)
                {
                    bane.EndDate = timeend.StartTime.Value.Date;
                }
            }
        }

        internal static BaneType FindBaneType(string lagText)
        {
            if (!string.IsNullOrEmpty(lagText))
            {
                if (lagText.Contains(Constants.Prefix100m))
                {
                    return BaneType.Hundremeter;
                }

                if (lagText.Contains(Constants.Prefix200m))
                {
                    return BaneType.Tohundremeter;
                }

                if (lagText.Contains(Constants.PrefixOnly200m))
                {
                    return BaneType.Tohundremeter;
                }

                if (lagText.Contains(Constants.PrefixOnly300m))
                {
                    return BaneType.Tohundremeter;
                }

                if (lagText.Contains(Constants.Prefix15m))
                {
                    return BaneType.Femtenmeter;
                }
                if (lagText.Contains(Constants.PrefixGrovFelt))
                {
                    return BaneType.GrovFelt;
                }
                if (lagText.Contains(Constants.PrefixFinFelt))
                {
                    return BaneType.FinFelt;
                }

                if (lagText.ToUpper() == "LAGVIS")
                {
                    return BaneType.Femtenmeter;
                }
            }

            return BaneType.Undefined;
        }


        internal void AddLag(StartingListLag nyttlag)
        {
            if (nyttlag == null)
            {
                return;
            }

            if (nyttlag.BaneType != BaneType)
            {
                return;
            }

            if (this.StevneLag != null)
            {
                this.StevneLag.Add(nyttlag);

            }
            else
            {
                Log.Error("Fant ikke Lag type {0} navn={1}", BaneType, nyttlag.StevneNavn);
            }
        }

        #endregion
    }
}