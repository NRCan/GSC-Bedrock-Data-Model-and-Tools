using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSC_ProjectEditor
{
    public class MathCustom
    {
        /// <summary>
        /// From a given decimal number, will return the number of decimals in it.
        /// </summary>
        /// <param name="inDecimal">The decimal number to calculate how many decimals is there in it.</param>
        /// <returns></returns>
        public static int GetDecimalPlaces(decimal inDecimal)
        {

            inDecimal = Math.Abs(inDecimal);//Make positive
            inDecimal -= (int)inDecimal; //Remove integer from it
            var decimalPlaces = 0;

            while (inDecimal> 0)
            {
                decimalPlaces++;
                inDecimal *= 10;
                inDecimal -= (int)inDecimal;
            }

            return decimalPlaces;
        }

    }
}
