using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SAConsole
{
    class Program
    {
        static string Prompt = "SAC> ";
        

        static void Main(string[] args)
        {
            string line = string.Empty;
            Type ts = typeof(SymbolicAlgebra.SymbolicVariable);

            var lib_ver = (AssemblyFileVersionAttribute)ts.Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0];

            string copyright = $@"Symbolic Algebra Console {lib_ver.Version}
Copyright 2012-{DateTime.Now.Year} at Lost Particles.
All Rights Reserved for Ahmed.Sadek@LostParticles.net the Author of the library.

Postfix Operators:
    '.'  Integration Operator       -- STILL EXPERIMENTAL -- 
    '|'  Differentiation Operator
    '^'  Power Operator
    '*'  Multiplication
    '/'  Division
    '+'  Addition
    '-'  Subtraction

Prefix Operators:
    '%'  Constants Prefix 

SA> x+x           will produce 2*x
SA> (x^2+2*x)|x   will produce  2*x+x
SA> (2*x).x       will produce  x^2

SA> :Q            to Quit  like VIM ;)
";

            string help2 = @$"
Custom Function Declaration:
    ':=' f(x) := x^3 

Using the library in your C# program directly:

Using SymbolicAlgebra;

// Assign variable immediately
var x = new SymbolicVariable(""x"");
var y = new SymbolicVariable(""y"");

// or parse the expression directly.
var myeq = SymbolicVariable.Parse(""(a+b)^2"");

// then use the language overloaded arithmatic opertors directly.
var total = (x*y*x).Differentiate(""x"");

Finally  .. Enjoy";

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

                    try
                    {
                        var result = SymbolicAlgebra.SymbolicVariable.Parse(line);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("     " + result);
                    }
                    catch(SymbolicAlgebra.SymbolicException se)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(se.Message);
                    }
                }
                else
                {
                    line = string.Empty;
                }
                

            }

        }
    }
}
