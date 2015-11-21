using System.Collections.Generic;

namespace FFA
{
    internal class Firefly
    {
        /// <summary>
        /// Initial brightness
        /// </summary>
        public const double Beta0 = .5;
        /// <summary>
        /// Coordinates in X.Count-dimensions
        /// </summary>
        public readonly List<double> X;
        /// <summary>
        /// Function value for firefly
        /// </summary>
        public double F;

        /// <summary>
        /// Creates firefly
        /// </summary>
        /// <param name="x">Coordinates</param>
        /// <param name="f">Function value</param>
        public Firefly(List<double> x, double f)
        {
            X = x;
            F = f;
        }

        //Comparer<> 
    }
}
