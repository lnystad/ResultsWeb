using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Ftp
{
    public class FtpDirectory
    {
        public FtpDirectory()
        {
            SubDir = new List<FtpDirectory>();
            Files = new List<FtpFile>();
        }

        public string Name { get; set; }
        List<FtpDirectory> SubDir { get; set; }
        List<FtpFile> Files { get; set; }
    }
}
