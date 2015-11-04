using System.Collections.Generic;

namespace FFA
{
    internal class Firefly
    {
        public const double Beta0 = 1;
        public readonly List<double> X;

        public Firefly(List<double> x)
        {
            X = x;
        }
    }
}
