using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAConsole
{
    class Program
    {
        static string Prompt = "SAC> ";
        static void Main(string[] args)
        {
            string line = string.Empty;


            string copyright =
@"Symbolic Algebra Console
Copyright 2012 at Lost Particles.
Version 0.881
All Rights Reserved for Ahmed Sadek the Auther of the library.

Ahmed.Sadek@LostParticles.net
Ahmed.Amara@Gmail.com
-----------------------------
Type :Q  to Quit

SA> x+x   will produce 2*x
    '|' Differentiation Operator

SA> (x^2+2*x)|x   will produce  2*x+x

Enjoy
";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(copyright);

            while (line.ToUpperInvariant() != ":Q")
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(Prompt);
                line = Console.ReadLine();

                if (!string.IsNullOrEmpty(line))
                {
                    if (line.ToUpperInvariant() == ":Q") break;

                    var result = SymbolicAlgebra.SymbolicVariable.Parse(line);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("     " + result);
                }
                else
                {
                    line = string.Empty;
                }
                

            }

        }
    }
}
