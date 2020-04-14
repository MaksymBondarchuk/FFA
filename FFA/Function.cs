using System;
using System.Collections.Generic;

namespace FFA
{
    public class Function
    {
        /// <summary>
        /// Function
        /// </summary>
        public Func<double[], double> F;

        /// <summary>
        /// Range of function (left and right borders)
        /// </summary>
        public double Range;


        public double FitnessFunction(double[] x)
        {
            double f = F(x);
            if (Math.Abs(f + 1) < 1e-20)
                return 0;
            return 1 / (1 + f);
        }
    }
}
