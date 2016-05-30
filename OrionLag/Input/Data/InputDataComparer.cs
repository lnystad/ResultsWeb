using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Input.Data
{
    using System.Collections;

    public class InputDataComparer : IComparer<InputData>
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

            string valueA = inputA.Name.Substring(1, 1);
            string valueB = inputB.Name.Substring(1, 1);
            return valueA.CompareTo(valueB);
        }
    }
}
