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
        long MaximumGenerations = 200;


        double f(double x)
        {
            //return x * x - 10 * Math.Cos(2 * Math.PI * x) + 10;
            return -Math.Pow(x - 2, 2);
        }

        public FFA(int number_of_fireflies/*, double gamma, double left_border, double right_border*/)
        {
            fireflies = new List<Firefly>(number_of_fireflies);
            alpha = (right_border - left_border) / 100.0;
            //this.gamma = gamma;
            //this.left_border = left_border;
            //this.right_border = right_border;
        }

        public double algorithm()
        {
            List<double> solutions = new List<double>();

            // Initiaizing population of fireflies
            double delta = Math.Abs(left_border - right_border) / (fireflies.Capacity - 1);
            for (int i = 0; i < fireflies.Capacity; i++)
                fireflies.Add(new Firefly(left_border + delta * i));

            Firefly the_best_firefly = fireflies[0];

            for (long t = 0; t < MaximumGenerations; t++)
            {
                for (int i = 0; i < fireflies.Count; i++)
                {
                    bool was_moved = false;
                    for (int j = 0; j < fireflies.Count; j++)
                    {
                        if (f(fireflies[i].x) < f(fireflies[j].x))
                        {
                            was_moved = true;
                            double r2 = Math.Pow(fireflies[i].x - fireflies[j].x, 2) + Math.Pow(f(fireflies[i].x) - f(fireflies[j].x), 2);
                            fireflies[i].x = fireflies[i].x
                                + fireflies[i].beta0 * Math.Exp(-gamma * r2) * (fireflies[j].x - fireflies[i].x)
                                + alpha * ((new Random()).NextDouble() - .5);
                            if (fireflies[i].x < left_border)
                                fireflies[i].x = left_border;
                            else
                                if (right_border < fireflies[i].x)
                                fireflies[i].x = right_border;
                        }
                    }
                    if (!was_moved)
                        fireflies[i].x = fireflies[i].x
                                + alpha * ((new Random()).NextDouble() - .5);
                }

                the_best_firefly = fireflies[0];
                for (int i = 1; i < fireflies.Count; i++)
                    if (f(the_best_firefly.x) < f(fireflies[i].x))
                        the_best_firefly = fireflies[i];

                Console.Write(String.Format("# {0,4}   ", t));
                for (int i = 0; i < fireflies.Count; i++)
                    Console.Write(String.Format("{0,6:.00}", fireflies[i].x));
                Console.WriteLine(String.Format("    Best {0,5:.00}, {1,5:.00}", the_best_firefly.x, f(the_best_firefly.x)));
            }


            return f(the_best_firefly.x);
        }
    }
}