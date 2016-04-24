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
        public List<double> X = new List<double>();

        /// <summary>
        /// Function value for firefly
        /// </summary>
        //private double _f;

        ///// <summary>
        ///// Fitness function of firefly
        ///// </summary>
        //public double FitnessFunctionValue;

        /// <summary>
        /// Function value for firefly
        /// </summary>
        public double F;

        //{
        //    get { return _f; }
        //    set
        //    {
        //        _f = value;
        //        FitnessFunctionValue = 1 / (1 + _f);
        //    }
        //}
    }
}
