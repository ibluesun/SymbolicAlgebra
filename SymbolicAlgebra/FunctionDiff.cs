using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SymbolicAlgebra
{
    internal static class FunctionDiff
    {

        public static readonly string[] FFunctions = { "exp", "sin", "sinh", "cos", "cosh", "tan", "tanh", "sec", "sech", "csc", "csch", "cot", "coth" };

        public static readonly string[] BFunctions = { "asin", "asinh", "acos", "acosh", "atan", "atanh", "asec", "asech", "acsc", "acsch", "acot", "acoth" };

        /// <summary>
        /// get the function derivation  like sin => cos
        /// </summary>
        /// <param name="function"></param>
        /// <param name="negative">indicates if the function returned should have negative value or not</param>
        /// <returns>string of the differentiated function</returns>
        public static string[] DiffFFunction(SymbolicVariable function, out bool negative)
        {

            string func = function.FunctionName;

            if (string.Equals(func, "exp", StringComparison.OrdinalIgnoreCase))
            {
                negative = false;
                return new string[] { "exp" };
            }


            if (string.Equals(func, "sin", StringComparison.OrdinalIgnoreCase))
            {
                negative = false;
                return new string[] { "cos" };
            }

            if (string.Equals(func, "sinh", StringComparison.OrdinalIgnoreCase))
            {
                negative = false;
                return new string[] { "cosh" };
            }

            if (string.Equals(func, "cos", StringComparison.OrdinalIgnoreCase))
            {
                negative = true;
                return new string[] { "sin" };
            }

            if (string.Equals(func, "cosh", StringComparison.OrdinalIgnoreCase))
            {
                negative = false;
                return new string[] { "sinh" };
            }

            if (string.Equals(func, "tan", StringComparison.OrdinalIgnoreCase))
            {
                negative = false;
                return new string[] { "sec", "sec" };
            }

            if (string.Equals(func, "tanh", StringComparison.OrdinalIgnoreCase))
            {
                negative = false;
                return new string[] { "sech", "sech" };
            }

            if (string.Equals(func, "sec", StringComparison.OrdinalIgnoreCase))
            {
                negative = false;
                return new string[] { "sec", "tan" };
            }

            if (string.Equals(func, "sech", StringComparison.OrdinalIgnoreCase))
            {
                negative = true;
                return new string[] { "sech", "tanh" };
            }

            if (string.Equals(func, "csc", StringComparison.OrdinalIgnoreCase))
            {
                negative = true;
                return new string[] { "csc", "cot" };
            }

            if (string.Equals(func, "csch", StringComparison.OrdinalIgnoreCase))
            {
                negative = true;
                return new string[] { "csch", "coth" };
            }

            if (string.Equals(func, "cot", StringComparison.OrdinalIgnoreCase))
            {
                negative = true;
                return new string[] { "csc", "csc" };
            }

            if (string.Equals(func, "coth", StringComparison.OrdinalIgnoreCase))
            {
                negative = true;
                return new string[] { "csch", "csch" };
            }

            negative = false;
            return null;

        }

        public static SymbolicVariable DiffBFunction(SymbolicVariable fv, string parameter)
        {

            var pa = fv.GetFunctionParameters()[0];

            var ps = SymbolicVariable.Multiply(pa, pa);

            var func = fv.FunctionName;

            var dpa = pa.Differentiate(parameter);


            if (string.Equals(func, "asin", StringComparison.OrdinalIgnoreCase))
            {                
                //asin(x) → 1 / sqrt(1-x^2) 
                
                return SymbolicVariable.Parse(dpa.ToString() + "/sqrt(1-(" + ps.ToString() + "))");
            }

            if (string.Equals(func, "acos", StringComparison.OrdinalIgnoreCase))
            {
                
                return SymbolicVariable.Parse("-" + dpa.ToString() + "/sqrt(1-(" + ps.ToString() + "))");
            }

            if (string.Equals(func, "atan", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse(dpa.ToString() + "/(" + ps.ToString() + "+1)");
            }

            if (string.Equals(func, "acot", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse("-" + dpa.ToString() + "/(" + ps.ToString() + "+1)");
            }

            if (string.Equals(func, "asec", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse(dpa.ToString() + "/(sqrt(1-1/(" + ps.ToString() + "))*" + ps.ToString() + ")");
            }

            if (string.Equals(func, "acsc", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse("-" + dpa.ToString() + "/(sqrt(1-1/(" + ps.ToString() + "))*" + ps.ToString() + ")");
            }



            #region hyperbolic functions
            if (string.Equals(func, "asinh", StringComparison.OrdinalIgnoreCase))
            {
                //asin(x) → 1 / sqrt(x^2+1) 

                return SymbolicVariable.Parse(dpa.ToString() + "/sqrt(" + ps.ToString() + "+1)");
            }

            if (string.Equals(func, "acosh", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse("-" + dpa.ToString() + "/sqrt(" + ps.ToString() + "-1)");
            }

            if (string.Equals(func, "atanh", StringComparison.OrdinalIgnoreCase))
            {
                
                return SymbolicVariable.Parse(dpa.ToString() + "/(1-(" + ps.ToString() + "))");
            }

            if (string.Equals(func, "acoth", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse("-" + dpa.ToString() + "/(" + ps.ToString() + "-1)");
            }

            if (string.Equals(func, "asech", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse("-" + dpa.ToString() + "/(sqrt(1/" + ps.ToString() + "-1)*" + ps.ToString() + ")");
            }

            if (string.Equals(func, "acsch", StringComparison.OrdinalIgnoreCase))
            {
                return SymbolicVariable.Parse("-" + dpa.ToString() + "/(sqrt(1/" + ps.ToString() + "+1)*" + ps.ToString() + ")");
            }

            #endregion
            throw new SymbolicException(fv.FunctionName + " differentiation not implemented yet");

            
        }
    
    }
}
