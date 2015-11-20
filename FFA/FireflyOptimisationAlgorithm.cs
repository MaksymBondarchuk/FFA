using System;
using System.Collections.Generic;
//using System.Drawing;

namespace FFA
{
    public class FireflyOptimisationAlgorithm
    {
        private readonly List<Firefly> _fireflies;
        private const double Gamma = .0001; // bigger Gamma => lesser step
        private const long MaximumGenerations = 100;
        private readonly int _fRange;

        private readonly System.IO.StreamWriter _file = new System.IO.StreamWriter("results.txt");
        private const bool TraceMoves = true;
        private readonly System.IO.StreamWriter _fileMoves = new System.IO.StreamWriter("trace_moves.txt");

        private readonly Function _func;

        // Additional
        private const double LambdaMax = .5;
        private const double LambdaMin = 1.9;
        private const double AlphaMax = 1.09;
        private const double Alpha = 1e-4;
        private const double AlphaMin = .5;
        private readonly double _delta;


        private void Initializiton()
        {
            //const int bmpSize = 1000;
            //var bmp = new Bitmap(bmpSize, bmpSize);

            // Generating list of random points
            var rnd = new Random();
            for (var i = 0; i < _fireflies.Capacity; i++)
            {
                var x = new List<double>();
                for (var j = 0; j < _fRange; j++)
                    x.Add(-_func.Range + rnd.NextDouble() * _func.Range * 2);
                _fireflies.Add(new Firefly(x, _func.F(x)));
            }

            //// ReSharper disable once InvertIf
            //if (_fireflies[0].X.Count == 2)
            //{
            //    const int dotSize = 3;
            //    foreach (var t in _fireflies)
            //    {
            //        var x = Convert.ToInt32(t.X[0] * bmpSize * .5 / _func.Range + bmpSize * .5);
            //        var y = Convert.ToInt32(t.X[1] * bmpSize * .5 / _func.Range + bmpSize * .5);
            //        //Console.WriteLine($"{t.X[0],4} -> {x,4}");
            //        //Console.WriteLine($"{t.X[1],4} -> {y,4}");
            //        for (var i1 = x - dotSize; i1 <= x + dotSize; i1++)
            //            for (var i2 = y - dotSize; i2 <= y + dotSize; i2++)
            //                if (0 <= i1 && i1 < bmp.Size.Height && 0 <= i2 && i2 < bmp.Size.Height)
            //                    bmp.SetPixel(i1, i2, Color.Red);
            //    }
            //    //bmp.Save("Initial Generation.png");
            //}
            //var q = rnd.Next(_fireflies.Count * 5, _fireflies.Count * 10);

            //const int maxIterations = 10;
            //for (var iteration = 0; iteration < maxIterations; iteration++)
            //{


            //    for (var i = 0; i < _fireflies.Count; i++)
            //    {
            //        var moved = new List<int>();

            //        for (var j = 0; j < q; j++)
            //        {
            //            var min = double.MaxValue;
            //            var imin = -1;
            //            for (var h = 0; h < _fireflies.Count; h++)
            //                if (h != i && !moved.Contains(h) && _func.F(_fireflies[h].X) - _func.F(_fireflies[i].X) < min)
            //                {
            //                    min = _func.F(_fireflies[h].X);
            //                    imin = h;
            //                }

            //            //var r = Math.Sqrt(Math.Pow(_func.F(_fireflies[imin].X) - _func.F(_fireflies[i].X), 2) + _fireflies[imin].X.Select((t, h) => Math.Pow(t - _fireflies[i].X[h], 2)).Sum());
            //            for (var h = 0; h < _fireflies[imin].X.Count; h++)
            //                _fireflies[imin].X[h] += (_fireflies[i].X[h] - _fireflies[imin].X[h]) * .001;
            //            moved.Add(imin);

            //        }

            //        var a = new List<double>();
            //        for (var j = 0; j < _fireflies.First().X.Count; j++)
            //            a.Add(0);
            //        foreach (var t in _fireflies)
            //            for (var j = 0; j < _fireflies.First().X.Count; j++)
            //                a[j] += t.X[j];
            //        for (var j = 0; j < _fireflies.First().X.Count; j++)
            //            a[j] /= _fireflies.Count;

            //        //var ra = Math.Sqrt(Math.Pow(_func.F(_fireflies[i].X) - _func.F(a), 2) + _fireflies[i].X.Select((t, j) => Math.Pow(t - a[j], 2)).Sum());
            //        for (var h = 0; h < _fireflies[i].X.Count; h++)
            //            _fireflies[i].X[h] += (a[h] - _fireflies[i].X[h]) * .1;
            //    }


            //}

            //// ReSharper disable once InvertIf
            //if (_fireflies[0].X.Count == 2)
            //{
            //    const int dotSize = 3;
            //    foreach (var t in _fireflies)
            //    {
            //        var x = Convert.ToInt32(t.X[0] * bmpSize * .5 / _func.Range + bmpSize * .5);
            //        var y = Convert.ToInt32(t.X[1] * bmpSize * .5 / _func.Range + bmpSize * .5);
            //        //Console.WriteLine($"{t.X[0],4} -> {x,4}");
            //        //Console.WriteLine($"{t.X[1],4} -> {y,4}");
            //        for (var i1 = x - dotSize; i1 <= x + dotSize; i1++)
            //            for (var i2 = y - dotSize; i2 <= y + dotSize; i2++)
            //                if (0 <= i1 && i1 < bmp.Size.Height && 0 <= i2 && i2 < bmp.Size.Height)
            //                    bmp.SetPixel(i1, i2, Color.LawnGreen);
            //    }
            //    bmp.Save("Initial Generation.png");
            //}
        }

        /// <summary>
        /// Creates object for algorithm
        /// </summary>
        /// <param name="numberOfFireflies">Size of fireflies generation</param>
        /// <param name="fRange">Range of researched function</param>
        /// <param name="func">Function</param>
        public FireflyOptimisationAlgorithm(int numberOfFireflies, int fRange, Function func)
        {
            _fireflies = new List<Firefly>(numberOfFireflies);
            _fRange = fRange;
            _func = func;

            // ReSharper disable once PossibleLossOfFraction
            //var MaximumGenerations1 = 1.0 / MaximumGenerations;
            //var AminAmax = AlphaMin/AlphaMax;
            _delta = Math.Pow(AlphaMin / AlphaMax, 1.0 / MaximumGenerations);
        }

        /// <summary>
        /// Moves firefly with index i towards firefly with index j
        /// </summary>
        /// <param name="i">Firfly to move</param>
        /// <param name="j">Firefly move to</param>
        /// <param name="alpha">Alpha</param>
        /// <param name="lambda">Lambda</param>
        private void MoveITowardsJ(int i, int j, double alpha, double lambda)
        {
            var r2 = 0.0;
            for (var h = 0; h < _fRange; h++)
                r2 += Math.Pow(_fireflies[i].X[h] - _fireflies[j].X[h], 2);

            for (var h = 0; h < _fRange; h++)
            {
                //_fireflies[i].X[h] += Firefly.Beta0 * Math.Exp(-Gamma * r2) * (_fireflies[j].X[h] - _fireflies[i].X[h]);
                //var brightness = Firefly.Beta0*Math.Exp(-Gamma*r2)*(_fireflies[j].X[h] - _fireflies[i].X[h]);
                var brightness = Firefly.Beta0 / (1 + Gamma * r2);
                //var randomPart = alpha * ((new Random()).NextDouble() - .5) + LevyRandom(lambda, alpha);
                var randomPart = alpha * ((new Random()).NextDouble() - .5) + MantegnaRandom(lambda);
                _fireflies[i].X[h] += brightness + randomPart;
                if (_fireflies[i].X[h] < -_func.Range)
                    _fireflies[i].X[h] = -_func.Range;
                else
                    if (_func.Range < _fireflies[i].X[h])
                    _fireflies[i].X[h] = _func.Range;
            }

            _fireflies[i].F = _func.F(_fireflies[i].X);
        }

        /*
                private void move_randomly(int i, double alpha)
                {
                    for (var h = 0; h < _fRange; h++)
                    {
                        _fireflies[i].X[h] += alpha * ((new Random()).NextDouble() - .5);
                        if (_fireflies[i].X[h] < -_func.Range)
                            _fireflies[i].X[h] = -_func.Range;
                        else
                            if (_func.Range < _fireflies[i].X[h])
                            _fireflies[i].X[h] = _func.Range;
                    }
                }
        */

        // ReSharper disable once UnusedMethodReturnValue.Global
        public double Algorithm()
        {
            Initializiton();

            RankSwarm();

            var theBestFirefly = _fireflies[0];
            double bestEver = 0;
            var bestEverNotInitialized = true;
            for (long t = 0; t < MaximumGenerations; t++)
            {
                var alphaT = AlphaFunction(t);

                for (var i = 0; i < _fireflies.Count; i++)
                {
                    for (var j = 0; j < _fireflies.Count; j++)
                    {
                        if (i == j || _fireflies[i].F > _fireflies[j].F)
                            continue;

                        var lambdaI = LambdaFunction(i);
                        MoveITowardsJ(i, j, alphaT, lambdaI);

                        // ReSharper disable once InvertIf
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        if (TraceMoves)
                        {
                            _fileMoves.Write("# {0,4} {1,4} -> {2,4} ", t, i, j);
                            foreach (var t1 in _fireflies[i].X)
                                _fileMoves.Write("{0,15:0.0000000000}", t1);
                            //foreach (var t1 in _fireflies)
                                //_fileMoves.Write("{0,15:0.0000000000}", t1.X[0]);
                            _fileMoves.WriteLine();
                        }

                    }
                }

                RankSwarm();

                var bestIter = _func.F(_fireflies[0].X);
                for (var i = 1; i < _fireflies.Count; i++)
                    if (bestIter > _fireflies[i].F)
                        bestIter = _fireflies[i].F;

                if (bestEverNotInitialized)
                {
                    bestEver = bestIter;
                    bestEverNotInitialized = false;
                }
                else
                if (bestEver > bestIter)
                    bestEver = bestIter;

                Console.WriteLine("# {0,3} Best iter {1,15:0.0000000000} Best ever {2,15:0.0000000000} Alpha {3,11:0.00000000}", t, bestIter, bestEver, alphaT);
                _file.WriteLine("# {0,3} Best iter {1,15:0.0000000000} Best ever {2,15:0.0000000000} Alpha {3,11:0.00000000}", t, bestIter, bestEver, alphaT);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (TraceMoves)
                {
                    _fileMoves.WriteLine("-----------------------------------------------------------------------------------------");
                }
            }
            _file.Close();
            _fileMoves.Close();

            return _func.F(theBestFirefly.X);
        }

        private double LambdaFunction(int i)
        {
            return LambdaMax - i * (LambdaMax - LambdaMin) / (_fireflies.Count - 1);
        }

        private double AlphaFunction(long iteration)
        {
            return Alpha * Math.Pow(_delta, iteration);
        }

        private void RankSwarm()
        {
            for (var i = 0; i < _fireflies.Count; i++)
                for (var j = _fireflies.Count - 1; i < j; j--)
                    if (_fireflies[j].F > _fireflies[i].F)
                    {
                        var tmp = _fireflies[i];
                        _fireflies[i] = _fireflies[j];
                        _fireflies[j] = tmp;
                    }
        }

        // ReSharper disable once UnusedMember.Local
        private static double LevyRandom(double lambda, double alpha)
        {
            var random = new Random();
            var rnd = random.NextDouble();
            var f = Math.Pow(rnd, -1 / lambda);
            return alpha * f * (random.NextDouble() - .5);
        }

        private double GaussianRandom(double mue, double sigma)
        {
            double x1;
            double w;
            var rand = new Random();
            const int randMax = 0x7fff;
            do
            {
                x1 = 2.0 * rand.Next(randMax) / (randMax + 1) - 1.0;
                var x2 = 2.0 * rand.Next(randMax) / (randMax + 1) - 1.0;
                w = x1 * x1 + x2 * x2;
            } while (w >= 1.0);
            var llog = Math.Log(w);
            w = Math.Sqrt((-2.0 * llog) / w);
            var y = x1 * w;
            return mue + sigma * y;
        }

        private double MantegnaRandom(double lambda)
        {
            SpecialFunction.lgamma(lambda + 1);
            var sigmaX = SpecialFunction.lgamma(lambda + 1) * Math.Sin(Math.PI * lambda / 2);
            var divider = SpecialFunction.lgamma((lambda) / 2) * lambda * Math.Pow(2.0, (lambda - 1) / 2);
            sigmaX /= divider;
            var lambda1 = 1.0 / lambda;
            sigmaX = Math.Pow(Math.Abs(sigmaX), lambda1);
            var x = GaussianRandom(0, sigmaX);
            var y = Math.Abs(GaussianRandom(0, 1.0));
            return x / Math.Pow(y, 1.0 / lambda);
        }
    }
}
