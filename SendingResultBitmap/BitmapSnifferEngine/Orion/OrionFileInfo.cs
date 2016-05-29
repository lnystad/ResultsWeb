using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapSnifferEngine.Orion
{
    using System.IO;

    using BitmapSnifferEngine.Common;
    using BitmapSnifferEngine.Logging;

    public class OrionFileInfo : IEquatable<OrionFileInfo>
    {
        #region Constructors and Destructors

        public OrionFileInfo()
        {
            this.LagInfo = new LagInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrionFileInfo"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        public OrionFileInfo(FileInfo info)
        {
            this.LagInfo = new LagInfo();
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

        public LagInfo LagInfo { get; set; }
        /// <summary>
        /// Gets or sets the lag.
        /// </summary>
        //public int Lag { get; set; }

        ///// <summary>
        ///// Gets or sets the skive.
        ///// </summary>
        //public int Skive { get; set; }

        public BaneType BaneType { get; set; }
        //public int Serie { get; set; }
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
                    this.LagInfo.LagNr = tall;
                }
                else
                {
                    this.LagInfo.LagNr = -1;
                    Log.Warning("Could not parse Lag {0}", this.FileInfo.Name);
                    return false;
                }

                tall = 0;
                if (int.TryParse(splitInf[2], out tall))
                {
                    this.LagInfo.SkiveNr = tall;
                }

                if (string.Compare(splitInf[3], "PNG", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (this.LagInfo.SkiveNr > 0 && this.LagInfo.LagNr > 0)
                    {
                        return true;
                    }
                }
                
                this.LagInfo.SkiveNr = -1;
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
