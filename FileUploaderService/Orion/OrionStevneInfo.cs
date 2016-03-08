// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrionStevneInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The orion stevne info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.Orion
{
    using System;

    /// <summary>
    /// The orion stevne info.
    /// </summary>
    public class OrionStevneInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the last checked time.
        /// </summary>
        public DateTime LastCheckedTime { get; set; }

        /// <summary>
        /// Gets or sets the navn.
        /// </summary>
        public string Navn { get; set; }

        #endregion
    }
}