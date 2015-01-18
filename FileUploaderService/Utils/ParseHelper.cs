using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.Utils
{
    public  class ParseHelper
    {
        public static string RemoveDirLetters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string retInput = input.Replace('/', ' ');
            retInput = retInput.Replace('\\', ' ');
            retInput = retInput.Replace(',', ' ');
            retInput = retInput.Replace('-', ' ');

            return retInput;
        }
         
    }
}
