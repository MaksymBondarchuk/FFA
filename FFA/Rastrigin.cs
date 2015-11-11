using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA
{
    public class Rastrigin : Function
    {
        public Rastrigin()
        {
            F = (x) =>
            {
                return x.Sum(t => t * t - 10 * Math.Cos(2 * Math.PI * t) + 10);
            };
            Range = 5.12;
            LookingForMax = false;
            //F = new Func<List<double>, double>();
        }

    }
}
