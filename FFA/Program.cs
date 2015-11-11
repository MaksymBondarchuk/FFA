using System;
using System.Linq;

namespace FFA
{
    internal static class Program
    {
        private static void Main()
        {
            var Rastrigin = new Function
            {
                F = x => { return x.Sum(t => t*t - 10*Math.Cos(2*Math.PI*t) + 10); },
                Range = 5.12,
                LookingForMax = false
            };

            var fireflyOptimisationAlgorithm = new FireflyOptimisationAlgorithm(50, 2, Rastrigin);


            fireflyOptimisationAlgorithm.Algorithm();
        }
    }
}
