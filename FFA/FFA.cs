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

        double function(List<double> xs)
        {


            return 0;
        }

        FFA(int number_of_fireflies, double left_border, double right_border)
        {
            fireflies = new List<Firefly>(number_of_fireflies);
            this.left_border = left_border;
            this.right_border = right_border;
        }

        public void algorithm()
        {
            // Initiaizing population of fireflies
            List<double> deltas = new List<double>(fireflies[0].coordinates.Count);
            for (int i = 0; i < deltas.Count; i++)

                //for (int i = 0; i < fireflies.Count; i++)
                //{ }

        }

    }
}
