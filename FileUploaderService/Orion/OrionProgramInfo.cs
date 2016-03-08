// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrionProgramInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The orion program info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUploaderService.Orion
{
    using System;
    using System.Diagnostics;

    using ProcessMemoryReaderLib;

    using SendingResults.Diagnosis;

    /// <summary>
    /// The orion program info.
    /// </summary>
    public class OrionProgramInfo
    {
        #region Public Methods and Operators

        /// <summary>
        /// The GetStevneNavn.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetStevneNavn()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("Orion");
                if (processes.Length > 0)
                {
                    using (var mem = new ProcessMemoryReader())
                    {
                        mem.ReadProcess = processes[0];
                        mem.OpenProcess();
                        int indeks = mem.ReadMultiLevelPointer(
                            processes[0].MainModule.BaseAddress.ToInt32() + 0x2ADAA0, 
                            4, 
                            new[] { 0x2c4, 0x1f4 });
                        Log.Info("stevneindeks {0}", indeks);
                        string forsteStevne = mem.ReadMultiLevelPointerString(
                            processes[0].MainModule.BaseAddress.ToInt32() + 0x2ADA58, 
                            4, 
                            new[] { 0x54, 0x50, (indeks - 1) * 4, 0x0 }, 
                            30);

                        return forsteStevne;
                    }
                }

                Log.Warning("Fant Ikke Prosess");
                return null;
            }
            catch (Exception e)
            {
                Log.Error(e, "GetStevneNavn");
                return null;
            }
        }

        #endregion
    }
}