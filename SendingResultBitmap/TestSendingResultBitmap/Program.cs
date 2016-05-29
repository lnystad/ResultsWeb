using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSendingResultBitmap
{
    using BitmapSnifferEngine;

    class Program
    {
        static void Main(string[] args)
        {
            BitmapSniffer sniff = new BitmapSniffer();
            sniff.Start();
        }
    }
}
