using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
    public static class MathConstants
    {

        /// <summary>
        /// returns the corresponding double value of the constant
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double Constant(string name)
        {

            if (name.Equals("pi", StringComparison.OrdinalIgnoreCase)) return Math.PI;

            if (name.Equals("e", StringComparison.OrdinalIgnoreCase)) return Math.E;

            if (name.Equals("phi", StringComparison.OrdinalIgnoreCase)) return (1 + Math.Sqrt(5)) / 2;

            // if(name.Equals("i", StringComparison.OrdinalIgnoreCase)) return  complex value


            // return unknown constant.
            return 0;

        }
    }
}
