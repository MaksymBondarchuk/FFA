using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace FFA
{
    public class FireflyOptimizationAlgorithm
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


        private void Initialization()
        {
            var rnd = new Random();
            for (var i = 0; i < _fireflies.Capacity; i++)
            {
                var x = new List<double>();
                for (var j = 0; j < _fRange; j++)
                    x.Add(-_func.Range + rnd.NextDouble() * _func.Range * 2);
                _fireflies.Add(new Firefly { X = x, F = _func.F(x) });
            }

            CentroidalVoronoiTessellations();
        }


        public void CentroidalVoronoiTessellations()
        {
            // Show result for function of 2 variables
            const int bmpSize = 1000;
            var bmp = new Bitmap(bmpSize, bmpSize);
            if (_fRange == 2)
            {
                const int dotSize = 3;
                foreach (var t in _fireflies)
                {
                    var x = Convert.ToInt32(t.X[0] * bmpSize * .5 / _func.Range + bmpSize * .5);
                    var y = Convert.ToInt32(t.X[1] * bmpSize * .5 / _func.Range + bmpSize * .5);
                    for (var i1 = x - dotSize; i1 <= x + dotSize; i1++)
                        for (var i2 = y - dotSize; i2 <= y + dotSize; i2++)
                            if (0 <= i1 && i1 < bmp.Size.Height && 0 <= i2 && i2 < bmp.Size.Height)
                                bmp.SetPixel(i1, i2, Color.Blue);
                }
            }

            var q = _fireflies.Count * 100;
            var rnd = new Random();

            const int centroidalVoronoiTessellationsIterations = 1000;
            for (var iteration = 0; iteration < centroidalVoronoiTessellationsIterations; iteration++)
            {
                // Choose q random points 
                var qList = new List<Firefly>(q);
                for (var i = 0; i < q; i++)
                {
                    qList.Add(new Firefly());
                    for (var j = 0; j < _fRange; j++)
                        qList[i].X.Add(-_func.Range + rnd.NextDouble() * _func.Range * 2);
                }

                var qShortestDistanceTo = new List<int>(q);
                foreach (var point in qList)
                {
                    var minDist = double.MaxValue;
                    var minDistFirefly = 0;
                    for (var j = 0; j < _fireflies.Count; j++)
                    {
                        var r2 = _fireflies[j].X.Select((t, h) => Math.Pow(_fireflies[j].X[h] - point.X[h], 2)).Sum();
                        if (r2 < minDist)
                        {
                            minDist = r2;
                            minDistFirefly = j;
                        }
                    }
                    qShortestDistanceTo.Add(minDistFirefly);
                }

                // Calculate ai
                for (var i = 0; i < _fireflies.Count; i++)
                {
                    var ai = new Firefly();
                    for (var h = 0; h < _fRange; h++)
                        ai.X.Add(0);

                    var isGeneratorForSomeone = false;
                    var numberOfPoints = 0;
                    for (var idx = 0; idx < qShortestDistanceTo.Count; idx++)
                        if (qShortestDistanceTo[idx] == i)
                        {
                            isGeneratorForSomeone = true;
                            numberOfPoints++;
                            for (var h = 0; h < _fRange; h++)
                                ai.X[h] += qList[idx].X[h];
                        }

                    for (var h = 0; h < _fRange; h++)
                        ai.X[h] /= numberOfPoints;

                    if (isGeneratorForSomeone)
                        for (var j = 0; j < _fireflies[i].X.Count; j++)
                            _fireflies[i].X[j] += (ai.X[j] - _fireflies[i].X[j]) * .1;
                }

                Console.WriteLine($"Voronoi Tessellations Iteration #{iteration}");
            }


            // Show result for function of 2 variables
            if (_fRange == 2)
            {
                const int dotSize = 3;
                foreach (var t in _fireflies)
                {
                    var x = Convert.ToInt32(t.X[0] * bmpSize * .5 / _func.Range + bmpSize * .5);
                    var y = Convert.ToInt32(t.X[1] * bmpSize * .5 / _func.Range + bmpSize * .5);
                    //Console.WriteLine($"{t.X[0],4} -> {x,4}");
                    //Console.WriteLine($"{t.X[1],4} -> {y,4}");
                    for (var i1 = x - dotSize; i1 <= x + dotSize; i1++)
                        for (var i2 = y - dotSize; i2 <= y + dotSize; i2++)
                            if (0 <= i1 && i1 < bmp.Size.Height && 0 <= i2 && i2 < bmp.Size.Height)
                                bmp.SetPixel(i1, i2, Color.Red);
                }
                bmp.Save("Initial Generation.png");
            }
        }


        /// <summary>
        /// Creates object for algorithm
        /// </summary>
        /// <param name="numberOfFireflies">Size of fireflies generation</param>
        /// <param name="fRange">Range of researched function</param>
        /// <param name="func">Function</param>
        public FireflyOptimizationAlgorithm(int numberOfFireflies, int fRange, Function func)
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
                var randomPart = alpha * (new Random().NextDouble() - .5) + LevyRandom(lambda, alpha);
                //var randomPart = alpha*(new Random().NextDouble() - .5) + MantegnaRandom(lambda);
                _fireflies[i].X[h] += brightness * (_fireflies[j].X[h] - _fireflies[i].X[h]) + randomPart;
                //_fireflies[i].X[h] += (_fireflies[j].X[h] - _fireflies[i].X[h])*.1;
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
            Initialization();

            RankSwarm();

            var theBestFirefly = _fireflies[0];
            double bestEver = 0;
            var bestEverNotInitialized = true;
            for (long t = 0; t < MaximumGenerations; t++)
            {
                var alphaT = AlphaFunction(t);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (TraceMoves)
                {
                    foreach (var ff in _fireflies)
                        _fileMoves.Write($"{ff.F,20:0.0000000000}");
                    _fileMoves.WriteLine();
                }

                for (var i = 0; i < _fireflies.Count; i++)
                {
                    var wasMoved = false;
                    for (var j = 0; j < _fireflies.Count; j++)
                    {
                        if (i == j || _fireflies[i].F < _fireflies[j].F)
                            continue;

                        // ReSharper disable once RedundantLogicalConditionalExpressionOperand
                        if (TraceMoves && !wasMoved)
                        {
                            _fileMoves.Write($"# {t,4} {i,4} ->");
                            wasMoved = true;
                        }

                        var lambdaI = LambdaFunction(i);

                        // ReSharper disable once InvertIf
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        if (TraceMoves)
                        {
                            _fileMoves.Write($" {j,2} ");

                            //foreach (var ff in _fireflies)
                            //    _fileMoves.Write($"{ff.F,20:0.0000000000}");
                            //_fileMoves.WriteLine();
                        }

                        MoveITowardsJ(i, j, alphaT, lambdaI);

                        // ReSharper disable once InvertIf
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        //if (TraceMoves)
                        //{
                        //    _fileMoves.Write("A # {0,4} {1,4} -> {2,4} ", t, i, j);
                        //    foreach (var t1 in _fireflies)
                        //        _fileMoves.Write("{0,20:0.0000000000}", t1.F);
                        //    _fileMoves.WriteLine();
                        //}
                    }

                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (TraceMoves)
                        _fileMoves.WriteLine();
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (TraceMoves)
                {
                    foreach (var ff in _fireflies)
                        _fileMoves.Write($"{ff.F,20:0.0000000000}");
                    _fileMoves.WriteLine();
                }

                RankSwarm();

                var bestIter = _func.F(_fireflies[0].X);
                for (var i = 1; i < _fireflies.Count; i++)
                    if (bestIter < _fireflies[i].F)
                        bestIter = _fireflies[i].F;

                if (bestEverNotInitialized)
                {
                    bestEver = bestIter;
                    bestEverNotInitialized = false;
                }
                else
                if (bestIter < bestEver)
                    bestEver = bestIter;

                Console.WriteLine("# {0,3} Best iter {1,15:0.0000000000} Best ever {2,15:0.0000000000} Alpha {3,11:0.00000000}", t, bestIter, bestEver, alphaT);
                _file.WriteLine("# {0,3}    Best iter {1,20:0.0000000000}    Best ever {2,20:0.0000000000}    Alpha {3,11:0.00000000}", t, bestIter, bestEver, alphaT);
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

        // ReSharper disable once UnusedMember.Local
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
