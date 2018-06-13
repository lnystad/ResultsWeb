using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService
{
    [Flags]
    public enum UploadCommand
    {
        none = 0,
        Web = 1,
        Pdf = 2,
        PresseListe = 4,
        BitMap = 8,
        Reports = 16,
        All = 32
    }
}
