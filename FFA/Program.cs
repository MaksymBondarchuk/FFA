using System;
using System.Linq;

namespace FFA
{
    internal static class Program
    {
        private static void Main()
        {
            // ReSharper disable once UnusedVariable
            var rastrigin = new Function
            {
                // Global min at 0*
                F = x => { return x.Sum(t => t * t - 10 * Math.Cos(2 * Math.PI * t) + 10); },
                Range = 5.12
            };

            // ReSharper disable once UnusedVariable
            var griewank = new Function
            {
                // Global min at 0*
                F = x =>
                {
                    var mul = 1.0;
                    for (var i = 0; i < x.Count; i++)
                        mul *= Math.Cos(x[i] / Math.Sqrt(i + 1));
                    return x.Sum(t => t * t / 4000) - mul + 1;
                },
                Range = 600
            };

            // ReSharper disable once UnusedVariable
            var levy = new Function
            {
                // Global min at 1*
                F = x =>
                {
                    var sum = 0.0;
                    for (var i = 0; i < x.Count - 1; i++)
                        sum += Math.Pow((x[i] - 1) * .25, 2) * (1 + 10 * Math.Pow(Math.Sin(Math.PI * (1 + (x[i] - 1) * .25) + 1), 2));
                    return Math.Pow(Math.Sin(Math.PI * (1 + (x.First() - 1) * .25)), 2) + sum +
                    Math.Pow((x.Last() - 1) * .25, 2) * (1 + Math.Pow(Math.Sin(2 * Math.PI * (1 + (x.Last() - 1) * .25)), 2));
                },
                Range = 10
            };

            // ReSharper disable once UnusedVariable
            var schwefel = new Function
            {
                // Global min at 420.9687*
                F = x => { return 418.9829 * x.Count - x.Sum(t => t * Math.Sin(Math.Sqrt(t))); },
                Range = 500
            };

            // ReSharper disable once UnusedVariable
            var sphere = new Function
            {
                // Global min at 420.9687*
                F = x => { return x.Sum(t => t * t); },
                Range = 100
            };

            //Console.WriteLine(schwefel.F(new List<double> { 420.9687, 420.9687 }));

            var fireflyOptimizationAlgorithm = new 
                FireflyOptimizationAlgorithm(numberOfFireflies: 50, fRange: 33, func: sphere);
            //for (var lambda = .5; lambda < 1.9; lambda += .01)
            //{
            //    Console.WriteLine(fireflyOptimizationAlgorithm.MantegnaRandom(lambda));
            //    Thread.Sleep(100);
            //}

            fireflyOptimizationAlgorithm.Algorithm();
        }
    }
}
