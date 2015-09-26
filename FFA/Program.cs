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
            FFA ffa = new FFA(8, 10);

            ffa.algorithm();
        }
    }
}
