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
        double alpha = 1;
        double eps = .1;

        double function(double x)
        {
            //return x * x - 10 * Math.Cos(2 * Math.PI * x) + 10;
            return -x * x;
        }

        public FFA(int number_of_fireflies/*, double gamma, double left_border, double right_border*/)
        {
            fireflies = new List<Firefly>(number_of_fireflies);
            //this.gamma = gamma;
            //this.left_border = left_border;
            //this.right_border = right_border;
        }

        public List<double> algorithm()
        {
            List<double> solutions = new List<double>();

            // Initiaizing population of fireflies
            double delta = Math.Abs(left_border - right_border) / (fireflies.Capacity - 1);
            for (int i = 0; i < fireflies.Capacity; i++)
                fireflies.Add(new Firefly(left_border + delta * i));

            bool someone_moved;
            do
            {
                someone_moved = false;

                for (int i = 0; i < fireflies.Count; i++)
                {
                    bool was_moved = false;
                    for (int j = 0; j < fireflies.Count; j++)
                    {
                        if (function(fireflies[i].x) < function(fireflies[j].x))
                        {
                            was_moved = true;
                            someone_moved = true;
                            fireflies[i].x = fireflies[i].x
                                + fireflies[i].beta0 * Math.Exp(-gamma * Math.Sqrt(Math.Pow(fireflies[i].x - fireflies[j].x, 2) - Math.Pow(function(fireflies[i].x) - function(fireflies[j].x), 2))) * (fireflies[j].x - fireflies[i].x)
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
            }
            while (someone_moved);

            for (int i = 0; i < fireflies.Count; i++)
            {
                bool is_new_solution = true;
                for (int j = 0; j < i; j++)
                    if (Math.Abs(function(fireflies[i].x) - function(fireflies[j].x)) < eps)
                        is_new_solution = false;

                if (is_new_solution)
                    solutions.Add(function(fireflies[i].x));
            }

            return solutions;
        }
    }
}
