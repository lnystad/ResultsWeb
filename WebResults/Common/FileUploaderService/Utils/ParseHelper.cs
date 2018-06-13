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

            if (input.EndsWith("\0"))
            {
               // input = input.TrimEnd("\0");
            }
            string retInput = input.Replace('/', ' ');
            retInput = retInput.Replace('\\', ' ');
            retInput = retInput.Replace('<', ' ');
            retInput = retInput.Replace('>', ' ');
            retInput = retInput.Replace(':', ' ');
            retInput = retInput.Replace('*', ' ');
            retInput = retInput.Replace('?', ' ');
            retInput = retInput.Replace('|', ' ');
            return retInput;
        }

        internal static string RemoveAllSpecialLetters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            string retVal = RemoveDirLetters(input);
            if (string.IsNullOrEmpty(retVal))
            {
                return retVal;
            }

            retVal = retVal.Replace(',', ' ');
            retVal = retVal.Replace('%', ' ');
            retVal = retVal.Replace('&', ' ');
            retVal = retVal.Replace(';', ' ');
            retVal = retVal.Replace('~', ' ');
            retVal = retVal.Replace('$', ' ');
            retVal = retVal.Replace('!', ' ');
            retVal = retVal.Replace(',', ' ');
            retVal = retVal.Replace('.', ' ');
            return retVal;
        }
    }

}
