// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Skive.cs" company="">
//   
// </copyright>
// <summary>
//   The skive.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.Orion
{
    using System;

    /// <summary>
    /// The skive.
    /// </summary>
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
            this.ArrangeDate = info.FileInfo.CreationTime;
            this.ArrangeTime = new DateTime(
                info.FileInfo.CreationTime.Year, 
                info.FileInfo.CreationTime.Month, 
                info.FileInfo.CreationTime.Day, 
                info.FileInfo.CreationTime.Hour, 
                info.FileInfo.CreationTime.Minute, 
                0);
            this.SkiveNr = info.Skive;
            this.Info = info;
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
            this.Info = cpy.Info;
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
        public OrionFileInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the skive nr.
        /// </summary>
        public int? SkiveNr { get; set; }

        #endregion
    }
}