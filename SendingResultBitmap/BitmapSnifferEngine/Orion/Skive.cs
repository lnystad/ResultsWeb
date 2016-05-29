using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapSnifferEngine.Orion
{
    public class Skive
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Skive"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        public Skive(OrionFileInfo info)
        {
            this.SerieInfo = new List<OrionFileInfo>();
            this.ArrangeDate = info.FileInfo.CreationTime;
            this.ArrangeTime = new DateTime(
                info.FileInfo.CreationTime.Year,
                info.FileInfo.CreationTime.Month,
                info.FileInfo.CreationTime.Day,
                info.FileInfo.CreationTime.Hour,
                info.FileInfo.CreationTime.Minute,
                0);
            this.SkiveNr = info.LagInfo.SkiveNr;
            this.SerieInfo.Add(info);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skive"/> class.
        /// </summary>
        /// <param name="cpy">
        /// The cpy.
        /// </param>
        public Skive(Skive cpy)
        {
            this.ArrangeDate = cpy.ArrangeDate;
            this.ArrangeTime = cpy.ArrangeTime;
            this.SkiveNr = cpy.SkiveNr;
            this.SerieInfo = new List<OrionFileInfo>();
            foreach (var serie in cpy.SerieInfo)
            {
                this.SerieInfo.Add(serie);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the arrange date.
        /// </summary>
        public DateTime? ArrangeDate { get; set; }

        /// <summary>
        /// Gets or sets the arrange time.
        /// </summary>
        public DateTime? ArrangeTime { get; set; }

        /// <summary>
        /// Gets or sets the info.
        /// </summary>
        public List<OrionFileInfo> SerieInfo { get; set; }


        /// <summary>
        /// Gets or sets the skive nr.
        /// </summary>
        public int? SkiveNr { get; set; }

        #endregion
    }
}
