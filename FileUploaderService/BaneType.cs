using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    [Flags]
    public enum BaneType
    {
        Undefined = 0,
        Femtenmeter = 1,
        Hundremeter = 2,
        Tohundremeter = 4,
        GrovFelt = 8,
        FinFelt = 16,
        All = 32
    }
}
