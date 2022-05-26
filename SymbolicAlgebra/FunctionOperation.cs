using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SymbolicAlgebra
{
    internal static class FunctionOperation
    {

        public const string ExpText = "exp";
        public const string LnText = "log";
        public const string SqrtText = "sqrt";


        public static readonly string[] TrigFunctions = { "sin", "sinh", "cos", "cosh", "tan", "tanh", "sec", "sech", "csc", "csch", "cot", "coth" };

        public static readonly string[] InversedTrigFunctions = { "asin", "asinh", "acos", "acosh", "atan", "atanh", "asec", "asech", "acsc", "acsch", "acot", "acoth" };

        /// <summary>
        /// get the function derivation  like sin => cos
        /// </summary>
        /// <param name="function"></param>
        /// <param name="negative">indicates if the function returned should have negative value or not</param>
        /// <returns>string of the differentiated function</returns>
        public static string[] DiffTrigFunction(SymbolicVariable function, out bool negative)
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


        /// <summary>
        /// This is the differentiation of inversed trigonometric and hyperbolic functions
        /// </summary>
        /// <param name="fv"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// <exception cref="SymbolicException"></exception>
        public static SymbolicVariable DiffInversedTrigFunction(SymbolicVariable fv, string parameter)
        {

            var pa = fv.FunctionParameters[0];

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
    


        public static SymbolicVariable IntegTrigFunction(SymbolicVariable fv, string parameter)
        {
            string func = fv.FunctionName;
            
            var spara = new SymbolicVariable(parameter);

            var pa =  fv.FunctionParameters[0];

            var divo = SymbolicVariable.Divide(pa, spara);

            SymbolicVariable result = null;

            if (string.Equals(func, "sin", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"cos({pa.ToString()})");
                result = SymbolicVariable.Multiply(SymbolicVariable.NegativeOne, result);
            }

            else if (string.Equals(func, "sinh", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"cosh({pa.ToString()})");
            }

            else if (string.Equals(func, "cos", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"sin({pa.ToString()})");
            }

            else if (string.Equals(func, "cosh", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"sinh({pa.ToString()})");
            }

            else if (string.Equals(func, "tan", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"log(cos({pa.ToString()}))");
                result = SymbolicVariable.Multiply(SymbolicVariable.NegativeOne, result);

            }

            else if (string.Equals(func, "tanh", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"log(cosh({pa.ToString()}))");
            }

            else if (string.Equals(func, "sec", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"log(cos({pa.ToString()}/2) + sin({pa.ToString()}/2)) - log(cos({pa.ToString()}/2) - sin({pa.ToString()}/2))");
            }

            else if (string.Equals(func, "sech", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"2*atan(tanh({pa.ToString()}/2))");
            }

            else if (string.Equals(func, "csc", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"log(sin({pa.ToString()}/2))-log(cos({pa.ToString()}/2))");
            }

            else if (string.Equals(func, "csch", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"log(tanh({pa.ToString()}/2))");
            }

            else if (string.Equals(func, "cot", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"log(sin({pa.ToString()}))");
            }

            else if (string.Equals(func, "coth", StringComparison.OrdinalIgnoreCase))
            {
                result = SymbolicVariable.Parse($"log(sinh({pa.ToString()}))");
            }
            else 
            throw new SymbolicException(fv.FunctionName + " integration not implemented yet");

            if (divo.IsOne) return result;
            else
                return SymbolicVariable.Divide(result, divo);

        }


        public static SymbolicVariable IntegInversedTrigFunction(SymbolicVariable fv, string parameter)
        {
            string func = fv.FunctionName;

            var spara = new SymbolicVariable(parameter);

            var pa = fv.FunctionParameters[0];

            var divo = SymbolicVariable.Divide(pa, spara);

            SymbolicVariable result = null;

            if (string.Equals(func, "asin", StringComparison.OrdinalIgnoreCase))
            {
                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa.ToString()}*asin({pa.ToString()})+sqrt(1-{pa2.ToString()})");
            }
            else if (string.Equals(func, "acos", StringComparison.OrdinalIgnoreCase))
            {
                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa.ToString()}*acos({pa.ToString()})-sqrt(1-{pa2.ToString()})");

            }
            else if (string.Equals(func, "atan", StringComparison.OrdinalIgnoreCase))
            {
                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa.ToString()}*atan({pa.ToString()})-log({pa2.ToString()}+1)/2");

            }
            else if (string.Equals(func, "asinh", StringComparison.OrdinalIgnoreCase))
            {
                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa.ToString()}*asinh({pa.ToString()})-sqrt({pa2.ToString()}+1)");
            }
            else if (string.Equals(func, "acosh", StringComparison.OrdinalIgnoreCase))
            {
                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa.ToString()}*acosh({pa.ToString()})-sqrt({pa2.ToString()}-1)");
            }
            else if (string.Equals(func, "atanh", StringComparison.OrdinalIgnoreCase))
            {
                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa.ToString()}*atan({pa.ToString()})-log(1-{pa2.ToString()})/2");

            }

            else if (string.Equals(func, "asec", StringComparison.OrdinalIgnoreCase))
            {
                //x*asec(x)-log(sqrt(1-1/x^2)+1)/2+log(1-sqrt(1-1/x^2))/2

                //(x*y*asec(x*y)-log(sqrt(1-1/(x^2*y^2))+1)/2+log(1-sqrt(1-1/(x^2*y^2)))/2)/y

                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa}*asec({pa})-log(sqrt(1-1/({pa2}))+1)/2+log(1-sqrt(1-1/({pa2})))/2");

            }
            else if (string.Equals(func, "acsc", StringComparison.OrdinalIgnoreCase))
            {
                //(x*y*acsc(x*y)+log(sqrt(1-1/(x^2*y^2))+1)/2-log(1-sqrt(1-1/(x^2*y^2)))/2)/y

                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa}*acsc({pa})+log(sqrt(1-1/({pa2}))+1)/2-log(1-sqrt(1-1/({pa2})))/2");

            }
            else if (string.Equals(func, "acot", StringComparison.OrdinalIgnoreCase))
            {
                //(log(x^2*y^2+1)/2+x*y*acot(x*y))/y

                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa}*acot({pa})+(log({pa2}+1)/2");

            }


            else if (string.Equals(func, "asech", StringComparison.OrdinalIgnoreCase))
            {

                //(x*y*asech(x*y)-atan(sqrt(1/(x^2*y^2)-1)))/y

                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa}*asech({pa})-atan(sqrt(1/({pa2})-1))");

            }
            else if (string.Equals(func, "acsch", StringComparison.OrdinalIgnoreCase))
            {
                //(x*y*acsch(x*y)+log(sqrt(1/(x^2*y^2)+1)+1)/2-log(sqrt(1/(x^2*y^2)+1)-1)/2)/y

                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa}*acsch({pa})+log(sqrt(1/({pa2})+1)+1)/2-log(1-sqrt(1/({pa2})+1)-1)/2");

            }
            else if (string.Equals(func, "acoth", StringComparison.OrdinalIgnoreCase))
            {
                //(log(1-x^2*y^2)/2+x*y*acoth(x*y))/y

                var pa2 = pa.Power(2);

                result = SymbolicVariable.Parse($"{pa}*acoth({pa})+(log(1-{pa2})/2");

            }
            else
                throw new SymbolicException(fv.FunctionName + " integration not implemented yet");

            if (divo.IsOne) return result;
            else
                return SymbolicVariable.Divide(result, divo);

        }

    }
}
