using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Input.Data
{
    using System.Collections;

    public class InputDataComparerLinksFelt : IComparer<InputData>
    {
        public int Compare(Object inputnr1, Object inputnr2)
        {
            InputData inputA = (InputData)inputnr1;
            InputData inputB = (InputData)inputnr2;

            string valueA = inputA.Name.Substring(1,1);
            string valueB = inputB.Name.Substring(1,1);
            return valueA.CompareTo(valueB);
        }

        public int Compare(InputData x, InputData y)
        {
            InputData inputA = (InputData)x;
            InputData inputB = (InputData)y;
            //-1(less than), 0(equal), or 1(greater)
            bool valueA = inputA.Links;
            bool valueB = inputB.Links;
            if (valueA)
            {
                if (valueB)
                {
                    return 0;
                }

                return 1;
            }

            if (valueB)
            {
                return -1;
            }
            else
            {
                return 0;
            }

        }
    }
}
