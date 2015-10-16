using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA
{
    class FFA
    {
        List<Firefly> fireflies;
        double left_border;
        double right_border;
        double gamma = 0.01;
        double alpha;
        long MaximumGenerations = 1000;
        bool looking_for_max;
        int f_range;

        int f_number = 2;

        System.IO.StreamWriter file = new System.IO.StreamWriter("results.txt");
        System.IO.StreamWriter file_moves = new System.IO.StreamWriter("trace_moves.txt");

        double f(List<double> x)
        {
            double res = 0;


            switch (f_number)
            {
                // Rosenbrok
                case 1:
                    for (int i = 0; i < x.Count; i++)
                        res += x[i] * x[i] - 10 * Math.Cos(2 * Math.PI * x[i]) + 10;
                    return res;

                // test
                case 2:
                    return x[0]*x[0];

                default:
                    return 0;
            }
        }

        public FFA(int number_of_fireflies, int f_range)
        {
            fireflies = new List<Firefly>(number_of_fireflies);
            alpha = (right_border - left_border) / 100.0;
            alpha = 0.1;
            this.f_range = f_range;

            switch (f_number)
            {
                case 1:
                    left_border = -5.12;
                    right_border = 5.12;
                    looking_for_max = false;
                    break;
                case 2:
                    left_border = -5;
                    right_border = 5;
                    looking_for_max = false;
                    break;
                default:
                    break;
            }
        }

        private void move_i_towards_j(int i, int j)
        {
            double r2 = Math.Pow(f(fireflies[i].x) - f(fireflies[j].x), 2);
            for (int h = 0; h < f_range; h++)
                r2 += Math.Pow(fireflies[i].x[h] - fireflies[j].x[h], 2);

            for (int h = 0; h < f_range; h++)
            {
                double ddd = fireflies[i].beta0 * Math.Exp(-gamma * r2) * (fireflies[j].x[h] - fireflies[i].x[h]);
                double dddd = fireflies[i].beta0 * Math.Exp(-gamma * r2) * (fireflies[j].x[h] - fireflies[i].x[h]) + alpha * ((new Random()).NextDouble() - .5);
                fireflies[i].x[h] += fireflies[i].beta0 * Math.Exp(-gamma * r2) * (fireflies[j].x[h] - fireflies[i].x[h]);
                //+ alpha * ((new Random()).NextDouble() - .5);
                if (fireflies[i].x[h] < left_border)
                    fireflies[i].x[h] = left_border;
                else
                    if (right_border < fireflies[i].x[h])
                    fireflies[i].x[h] = right_border;
            }
        }

        private void move_randomly(int i)
        {
            for (int h = 0; h < f_range; h++)
            {
                fireflies[i].x[h] += alpha * ((new Random()).NextDouble() - .5);
                if (fireflies[i].x[h] < left_border)
                    fireflies[i].x[h] = left_border;
                else
                    if (right_border < fireflies[i].x[h])
                    fireflies[i].x[h] = right_border;
            }
        }

        public double algorithm()
        {
            List<double> solutions = new List<double>();

            // Initiaizing population of fireflies
            double delta = Math.Abs(left_border - right_border) / (fireflies.Capacity - 1);
            for (int i = 0; i < fireflies.Capacity; i++)
            {
                List<double> x = new List<double>();
                double xi = left_border + delta * i;
                for (int j = 0; j < f_range; j++)
                    x.Add(xi);
                fireflies.Add(new Firefly(x));
            }

            Firefly the_best_firefly = fireflies[0];
            double best_ever = 0;
            bool best_ever_not_initialized = true;
            double best_iter;
            for (long t = 0; t < MaximumGenerations; t++)
            {
                //file.Write(string.Format("# {0,4}", t));
                //for (int i = 0; i < fireflies.Count; i++)
                //    file.Write(string.Format("{0,15:0.0000000000}", fireflies[i].x[0]));
                //file.WriteLine();

                for (int i = 0; i < fireflies.Count; i++)
                {
                    bool was_moved = false;
                    for (int j = 0; j < fireflies.Count; j++)
                    {
                        if (f(fireflies[i].x) < f(fireflies[j].x) && looking_for_max ||
                            f(fireflies[i].x) > f(fireflies[j].x) && !looking_for_max)
                        {
                            was_moved = true;
                            move_i_towards_j(i, j);

                            file_moves.Write(string.Format("# {0,4} {1,4} -> {2,4} ", t, i, j));
                            for (int ii = 0; ii < fireflies.Count; ii++)
                                file_moves.Write(string.Format("{0,15:0.0000000000}", fireflies[ii].x[0]));
                            file_moves.WriteLine();
                        }
                    }
                    if (!was_moved)
                        move_randomly(i);
                }

                best_iter = f(fireflies[0].x);
                for (int i = 1; i < fireflies.Count; i++)
                    if (best_iter < f(fireflies[i].x) && looking_for_max ||
                        best_iter > f(fireflies[i].x) && !looking_for_max)
                        best_iter = f(fireflies[i].x);

                if (best_ever_not_initialized)
                {
                    best_ever = best_iter;
                    best_ever_not_initialized = false;
                }
                else
                if (best_ever < best_iter && looking_for_max ||
                    best_ever > best_iter && !looking_for_max)
                    best_ever = best_iter;

                Console.WriteLine(string.Format("# {0,4}   Best iter {1,15:0.0000000000}    Best ever {2,15:0.0000000000}", t, best_iter, best_ever));
                file.WriteLine(string.Format("# {0,4}   Best iter {1,15:0.0000000000}    Best ever {2,15:0.0000000000}", t, best_iter, best_ever));
                file_moves.WriteLine("-----------------------------------------------------------------------------------------");
            }
            file.Close();
            file_moves.Close();

            return f(the_best_firefly.x);
        }
    }
}