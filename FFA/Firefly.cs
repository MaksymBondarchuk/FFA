using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA
{
    class Firefly
    {
        public double beta0 = 1;
        public List<double> x;

        public Firefly(List<double> x)
        {
            this.x = x;
        }
    }
}
