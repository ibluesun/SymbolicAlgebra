using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
    {
        /// <summary>
        /// Solve Expression of one variable whatever it was.
        /// return value of is zero in case of the expression in constant.   {this needs review}
        /// </summary>
        /// <returns></returns>
        public double Solve()
        {
            if (InvolvedSymbols.Length == 0) return 0;

            return Solve(InvolvedSymbols[0]).Coeffecient;
        }


        /// <summary>
        /// Solve for the variable and gets the whole expression for that variable
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public SymbolicVariable Solve(string variable)
        {
            // "5*t1+3/t2"  ==>  5*t1 = -3/t2  ==>  t1  = (-3/t2)*5   

            if (_ExtraTerms != null && _ExtraTerms.Count > 0) 
                throw new SymbolicException("can't solve expression with extra terms that hold terms with different denominator");

            // Algorithm:
            // search for the term with variable in it 
            // don't accept more than one term 
            // don't accept terms with power other than 1
            // solve

            SymbolicVariable ExtractedTerm = null;
            SymbolicVariable RestTerm = this.Clone();

            // extract the needed term
            if (this.Symbol.Equals(variable, StringComparison.OrdinalIgnoreCase))
            {
                ExtractedTerm = this.Clone();
                ExtractedTerm._AddedTerms = null;

                RestTerm.Coeffecient = 0;
                AdjustZeroCoeffecientTerms(ref RestTerm);  // this will adjust the internal structure of symbolic variabl
            }
            else
            {   
                foreach (var t in RestTerm.AddedTerms)
                {
                    if (t.Value.Symbol.Equals(variable, StringComparison.OrdinalIgnoreCase))
                    {
                        ExtractedTerm = t.Value.Clone();
                        t.Value.Coeffecient = 0;
                        break;
                    }
                }
                AdjustZeroCoeffecientTerms(ref RestTerm);
            }

            if (ExtractedTerm == null) throw new SymbolicException("the variable (" + variable + ") of solving not found");

            if (ExtractedTerm.SymbolPower != 1) throw new SymbolicException("Variables with higher powers is not solvable for now");

            RestTerm = RestTerm * SymbolicVariable.NegativeOne; // this will transfer the rest value from left to right

            // now we have extracted term as  3*x [Extracted Term] =  5*y [Rest Term]

            SymbolicVariable result = RestTerm / ExtractedTerm.Coeffecient;
            return result;
        }


        /// <summary>
        /// Substitute variable with the value
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SymbolicVariable Substitute(string variable, double value)
        {
            
            var s = this.ToString();
            var ss = s.Replace(variable, "(" + value.ToString() + ")");

            return Parse(ss);
        }

        /// <summary>
        /// Substitute variable with another variable
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SymbolicVariable Substitute(string variable, SymbolicVariable value)
        {

            var s = this.ToString();
            var ss = s.Replace(variable, "(" + value.ToString() + ")");

            return Parse(ss);
        }


        /// <summary>
        /// Returns the coefficient of variable in expression or null in case variable not found
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public double? CoefficientOf(string variable)
        {
            double? cf = null;

            if (this.Symbol.Equals(variable, StringComparison.OrdinalIgnoreCase))
            {
                if (CoeffecientPowerTerm == null) cf = Coeffecient;
            }

            if (!cf.HasValue)
            {
                foreach (var t in AddedTerms)
                {
                    if (t.Value.Symbol.Equals(variable, StringComparison.OrdinalIgnoreCase))
                    {
                        if (t.Value.CoeffecientPowerTerm == null) cf = t.Value.Coeffecient;
                    }
                }
            }
            return cf;
        }

        /// <summary>
        /// Returs the coefficient of variable with certain power
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        public double? CoefficientOf(string variable, double power)
        {
            double? cf = null;

            if (this.Symbol.Equals(variable, StringComparison.OrdinalIgnoreCase) && this.SymbolPower == power)
            {
                if (CoeffecientPowerTerm == null) cf = Coeffecient;
            }

            if (!cf.HasValue)
            {
                foreach (var t in AddedTerms)
                {
                    if (t.Value.Symbol.Equals(variable, StringComparison.OrdinalIgnoreCase) && t.Value.SymbolPower == power)
                    {
                        if (t.Value.CoeffecientPowerTerm == null) cf = t.Value.Coeffecient;
                    }
                }
            }
            return cf;
        }
    }

}