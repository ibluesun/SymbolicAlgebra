using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SymbolicAlgebra
{
    public class FunctionDiff
    {


        /// <summary>
        /// get the function derivation  like sin => cos
        /// </summary>
        /// <param name="function"></param>
        /// <param name="negative">indicates if the function returned should have negative value or not</param>
        /// <returns>string of the differentiated function</returns>
        public static string[] Diff(SymbolicVariable function, out bool negative)
        {

            

            string func = function.FunctionName;


            if (string.Equals(func, "sin", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = false;
                return new string[] { "cos" };
            }

            if (string.Equals(func, "sinh", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = false;
                return new string[] { "cosh" };
            }

            if (string.Equals(func, "cos", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = true;
                return new string[] { "sin" };
            }

            if (string.Equals(func, "cosh", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = false;
                return new string[] { "sinh" };
            }

            if (string.Equals(func, "tan", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = false;
                return new string[] { "sec", "sec" };
            }

            if (string.Equals(func, "tanh", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = false;
                return new string[] { "sech", "sech" };
            }

            if (string.Equals(func, "sec", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = false;
                return new string[] { "sec", "tan" };
            }

            if (string.Equals(func, "sech", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = true;
                return new string[] { "sech", "tanh" };
            }

            if (string.Equals(func, "csc", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = true;
                return new string[] { "csc", "cot" };
            }

            if (string.Equals(func, "csch", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = true;
                return new string[] { "csch", "coth" };
            }

            if (string.Equals(func, "cot", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = true;
                return new string[] { "csc", "csc" };
            }

            if (string.Equals(func, "coth", StringComparison.InvariantCultureIgnoreCase))
            {
                negative = true;
                return new string[] { "csch", "csch" };
            }

            negative = false;
            return null;

        }
    }
}
