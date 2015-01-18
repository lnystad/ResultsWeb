using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Orion
{
    using System.Diagnostics;

    using ProcessMemoryReaderLib;

    using SendingResults.Diagnosis;

    public class OrionProgramInfo
    {

        public static string GetStevneNavn()
        {
            Process[] processes = Process.GetProcessesByName("Orion");
            if (processes.Length > 0)
            {
                using (ProcessMemoryReader mem = new ProcessMemoryReader())
                {
                    mem.ReadProcess = processes[0];
                    mem.OpenProcess();
                    int indeks = mem.ReadMultiLevelPointer(processes[0].MainModule.BaseAddress.ToInt32() + 0x2ADAA0, 4, new int[] { 0x2c4, 0x1f4 });
                    Log.Info("stevneindeks {0}",indeks);
                    string forsteStevne = mem.ReadMultiLevelPointerString(
                        processes[0].MainModule.BaseAddress.ToInt32() + 0x2ADA58,
                        4,
                        new int[] { 0x54, 0x50, (indeks - 1) * 4, 0x0 },
                        30);

                    return forsteStevne;
                }
            }
            else
            {
                Log.Warning("Fant Ikke Prosess");
                return null;
            }
        }
    }
}
