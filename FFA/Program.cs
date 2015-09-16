using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFA
{
    class Program
    {
        static void Main(string[] args)
        {
            FFA ffa = new FFA(5);

            List<double> solutions = ffa.algorithm();

            for (int i = 0; i < solutions.Count; i++)
                Console.WriteLine(solutions[i]);
        }
    }
}
