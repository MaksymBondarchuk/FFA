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
        double left_border = -5.12;
        double right_border = 5.12;
        double gamma = 1.5;
        double alpha;
        long MaximumGenerations = 300;
        bool looking_for_max = false;
        int f_range;


        double f(List<double> x)
        {
            double res = 0;
            for (int i = 0; i < x.Count; i++)
                res += x[i] * x[i] - 10 * Math.Cos(2 * Math.PI * x[i]) + 10;
            return res;
        }

        public FFA(int number_of_fireflies, int f_range)
        {
            fireflies = new List<Firefly>(number_of_fireflies);
            alpha = (right_border - left_border) / 100.0;
            this.f_range = f_range;
        }

        private void move_i_towards_j(int i, int j)
        {
            double r2 = Math.Pow(f(fireflies[i].x) - f(fireflies[j].x), 2);
            for (int h = 0; h < f_range; h++)
                r2 += Math.Pow(fireflies[i].x[h] - fireflies[j].x[h], 2);

            for (int h = 0; h < f_range; h++)
            {
                fireflies[i].x[h] += fireflies[i].beta0 * Math.Exp(-gamma * r2) * (fireflies[j].x[h] - fireflies[i].x[h])
                + alpha * ((new Random()).NextDouble() - .5);
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
            for (long t = 0; t < MaximumGenerations; t++)
            {
                for (int i = 0; i < fireflies.Count; i++)
                {
                    bool was_moved = false;
                    for (int j = 0; j < fireflies.Count; j++)
                    {
                        if (f(fireflies[i].x) < f(fireflies[j].x) && looking_for_max ||
                            f(fireflies[j].x) < f(fireflies[i].x) && !looking_for_max)
                        {
                            was_moved = true;
                            move_i_towards_j(i, j);
                        }
                    }
                    //if (!was_moved)
                    //    move_randomly(i);
                }

                the_best_firefly = fireflies[0];
                int best_idx = 0;
                for (int i = 1; i < fireflies.Count; i++)
                    if (f(the_best_firefly.x) < f(fireflies[i].x) && looking_for_max ||
                        f(the_best_firefly.x) > f(fireflies[i].x) && !looking_for_max)
                    {
                        the_best_firefly = fireflies[i];
                        best_idx = i;
                    }

                Console.Write(String.Format("# {0,4}   ", t));
                for (int i = 0; i < fireflies.Count; i++)
                {
                    if (i == best_idx)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write(String.Format("{0,6:.00}", f(fireflies[i].x)));
                        Console.ResetColor();
                    }
                    else
                        Console.Write(String.Format("{0,6:.00}", f(fireflies[i].x)));
                }
                Console.WriteLine();
            }


            return f(the_best_firefly.x);
        }
    }
}