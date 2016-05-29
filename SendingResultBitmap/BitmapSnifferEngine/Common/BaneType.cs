using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitmapSnifferEngine.Common
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

    public enum HoldType
    {
        Undefined = 0,
        Minne = 1,
        Felt = 2,
    }
}
