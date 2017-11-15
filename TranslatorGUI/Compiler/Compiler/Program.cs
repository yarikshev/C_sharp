using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using translator;

namespace Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                Executer.Init(args);            
            }
            else
                Console.WriteLine("No poliz in args parameters.\n");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
