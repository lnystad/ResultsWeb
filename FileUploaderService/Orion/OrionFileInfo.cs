// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrionFileInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The orion file info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.Orion
{
    using System;
    using System.IO;

    using SendingResults.Diagnosis;

    /// <summary>
    /// The orion file info.
    /// </summary>
    public class OrionFileInfo : IEquatable<OrionFileInfo>
    {
        #region Constructors and Destructors

        public OrionFileInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrionFileInfo"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        public OrionFileInfo(FileInfo info)
        {
            this.FileInfo = info;
            this.EventDate = info.CreationTime.Date;
            this.EventDateTime = new DateTime(
                info.CreationTime.Year, 
                info.CreationTime.Month, 
                info.CreationTime.Day, 
                info.CreationTime.Hour, 
                info.CreationTime.Minute, 
                0);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the event date.
        /// </summary>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets the event date time.
        /// </summary>
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// Gets or sets the file info.
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// Gets or sets the lag.
        /// </summary>
        public int Lag { get; set; }

        /// <summary>
        /// Gets or sets the skive.
        /// </summary>
        public int Skive { get; set; }


        public int Serie { get; set; }
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        #endregion

        #region Methods

        /// <summary>
        /// The parse target.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool ParseTarget()
        {
            try
            {
                var splitInf = this.FileInfo.Name.Split(new[] { '-', '.' });

                int tall = 0;
                if (int.TryParse(splitInf[1], out tall))
                {
                    this.Lag = tall;
                }
                else
                {
                    this.Lag = -1;
                    Log.Warning("Could not parse Lag {0}", this.FileInfo.Name);
                }

                tall = 0;
                if (int.TryParse(splitInf[2], out tall))
                {
                    this.Skive = tall;
                }

                if (string.Compare(splitInf[3], "PNG", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (this.Skive > 0 && this.Lag > 0)
                    {
                        return true;
                    }
                }

                this.Skive = -1;
                Log.Warning("Could not parse Skive {0}", this.FileInfo.Name);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error Parsing {0}", this.FileInfo.Name);
            }

            return false;
        }

        #endregion
    }
}