using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFileUploader
{
    using FileUploaderService;

    class Program
    {
        static void Main(string[] args)
        {
            FileSnifferEngine sniff = new FileSnifferEngine();
            sniff.Start();
        }
    }
}
