using System;
using System.Collections.Generic;
using System.Drawing;

namespace FFA
{
    public class FireflyOptimisationAlgorithm
    {
        private readonly List<Firefly> _fireflies;
        private const double Gamma = .05; // bigger Gamma => lesser step
        private const long MaximumGenerations = 1000;
        private readonly int _fRange;

        private readonly System.IO.StreamWriter _file = new System.IO.StreamWriter("results.txt");
        private readonly System.IO.StreamWriter _fileMoves = new System.IO.StreamWriter("trace_moves.txt");

        private readonly Function _func;

        // Additional
        private const double LambdaMax = .5;
        private const double LambdaMin = 1.9;
        private const double AlphaMax = 1e-4;
        private const double AlphaMin = .5;
        private readonly double _delta;
        

        private void Initializiton()
        {
            const int bmpSize = 1000;
            var bmp = new Bitmap(bmpSize, bmpSize);

            var rnd = new Random();

            // Generating list of random points
            for (var i = 0; i < _fireflies.Capacity; i++)
            {
                var x = new List<double>();
                for (var j = 0; j < _fRange; j++)
                    x.Add(-_func.Range + rnd.NextDouble() * _func.Range * 2);
                _fireflies.Add(new Firefly(x));
            }

            //const int maxIterations = 1000;
            //for (var iteration = 0; iteration < maxIterations; iteration++)
            //{
                  //var q = rnd.Next(1, _fireflies.Count + 1);

            //}

            if (_fireflies[0].X.Count == 2)
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

        public FireflyOptimisationAlgorithm(int numberOfFireflies, int fRange, Function func)
        {
            _fireflies = new List<Firefly>(numberOfFireflies);
            _fRange = fRange;
            _func = func;

            // ReSharper disable once PossibleLossOfFraction
            _delta = Math.Pow(AlphaMin / AlphaMax, 1 / MaximumGenerations);
        }

        private void move_i_towards_j(int i, int j, double alpha, double lambda)
        {
            var r2 = Math.Pow(_func.F(_fireflies[i].X) - _func.F(_fireflies[j].X), 2);
            for (var h = 0; h < _fRange; h++)
                r2 += Math.Pow(_fireflies[i].X[h] - _fireflies[j].X[h], 2);

            for (var h = 0; h < _fRange; h++)
            {
                _fireflies[i].X[h] += Firefly.Beta0 * Math.Exp(-Gamma * r2) * (_fireflies[j].X[h] - _fireflies[i].X[h])
                + alpha * ((new Random()).NextDouble() - .5) + LevyRandom(lambda, alpha);
                if (_fireflies[i].X[h] < -_func.Range)
                    _fireflies[i].X[h] = -_func.Range;
                else
                    if (_func.Range < _fireflies[i].X[h])
                    _fireflies[i].X[h] = _func.Range;
            }
        }

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
                var alphaT = alpha_function(t);

                for (var i = 0; i < _fireflies.Count; i++)
                {
                    var wasMoved = false;
                    for (var j = 0; j < _fireflies.Count; j++)
                    {
                        if ((!(_func.F(_fireflies[i].X) < _func.F(_fireflies[j].X)) || !_func.LookingForMax) &&
                            (!(_func.F(_fireflies[i].X) > _func.F(_fireflies[j].X)) || _func.LookingForMax))
                            continue;
                        var lambdaI = lambda_function(i);
                        wasMoved = true;
                        move_i_towards_j(i, j, alphaT, lambdaI);

                        _fileMoves.Write("# {0,4} {1,4} -> {2,4} ", t, i, j);
                        foreach (var t1 in _fireflies)
                            _fileMoves.Write("{0,15:0.0000000000}", t1.X[0]);
                        _fileMoves.WriteLine();
                    }
                    if (!wasMoved)
                        move_randomly(i, alphaT);
                }

                RankSwarm();

                var bestIter = _func.F(_fireflies[0].X);
                for (var i = 1; i < _fireflies.Count; i++)
                    if (bestIter < _func.F(_fireflies[i].X) && _func.LookingForMax ||
                        bestIter > _func.F(_fireflies[i].X) && !_func.LookingForMax)
                        bestIter = _func.F(_fireflies[i].X);

                if (bestEverNotInitialized)
                {
                    bestEver = bestIter;
                    bestEverNotInitialized = false;
                }
                else
                if (bestEver < bestIter && _func.LookingForMax ||
                    bestEver > bestIter && !_func.LookingForMax)
                    bestEver = bestIter;

                Console.WriteLine("# {0,4}   Best iter {1,15:0.0000000000}    Best ever {2,15:0.0000000000}", t, bestIter, bestEver);
                _file.WriteLine("# {0,4}   Best iter {1,15:0.0000000000}    Best ever {2,15:0.0000000000}", t, bestIter, bestEver);
                _fileMoves.WriteLine("-----------------------------------------------------------------------------------------");
            }
            _file.Close();
            _fileMoves.Close();

            return _func.F(theBestFirefly.X);
        }

        private double lambda_function(int i)
        {
            return LambdaMax - i * (LambdaMax - LambdaMin) / (_fireflies.Count - 1);
        }

        private double alpha_function(long iteration)
        {
            return AlphaMax * Math.Pow(_delta, iteration);
        }

        private void RankSwarm()
        {
            for (var i = 0; i < _fireflies.Count; i++)
                for (var j = _fireflies.Count - 1; i < j; j--)
                    if (_func.F(_fireflies[j].X) < _func.F(_fireflies[i].X) && _func.LookingForMax ||
                        _func.F(_fireflies[j].X) > _func.F(_fireflies[i].X) && !_func.LookingForMax)
                    {
                        var tmp = _fireflies[i];
                        _fireflies[i] = _fireflies[j];
                        _fireflies[j] = tmp;
                    }
        }

        private static double LevyRandom(double lambda, double alpha)
        {
            var random = new Random();
            var rnd = random.NextDouble();
            var f = Math.Pow(rnd, -1 / lambda);
            return alpha * f * (random.NextDouble() - .5);
        }
    }
}