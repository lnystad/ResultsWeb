using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.KME
{
    public class TopListElement
    {
        ProgramType TypeProgram { get; set; }
        public string ReportName { get; set; }
        public List<string> Klasse { get; set; }
    }
}
