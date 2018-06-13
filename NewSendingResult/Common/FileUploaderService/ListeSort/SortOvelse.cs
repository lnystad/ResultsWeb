using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploaderService.ListeSort
{
    using FileUploaderService.KME;

    public class SortOvelse : IComparer<fileOpprop>
    {
        public int Compare(Object x, Object y)
        {
            fileOpprop inputA = (fileOpprop)x;
            fileOpprop inputB = (fileOpprop)y;

            return this.Compare(inputA, inputB);
        }

        public int Compare(fileOpprop x, fileOpprop y)
        {
            if (x.OvelseNo == y.OvelseNo) 
                return 0;
            if (x.OvelseNo < y.OvelseNo)
                return -1;

            return 1;


        }
    }
}
