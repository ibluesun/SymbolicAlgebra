using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
    public partial class SymbolicVariable : ICloneable
    {

        public void RaiseToSymbolicPower(SymbolicVariable b)
        {
            var an = this;

            //if the value is only having coeffecient then there is no need to instantiate the powerterm

            if (b.IsOneTerm & b.IsThisTermCoeffecientOnly)
            {
                // hold every term and multiply its power with this value.
                double power = b.Coeffecient;

                // symbol power term

                // now check if the current powerterm exist or not
                if (an._SymbolPowerTerm != null)
                {
                    an._SymbolPowerTerm = an._SymbolPowerTerm * power;
                }
                else
                {
                    an._SymbolPower = an._SymbolPower * power;
                }

                //coeffecient power term
                if (an._CoeffecientPowerTerm != null)
                {
                    an._CoeffecientPowerTerm = an._CoeffecientPowerTerm * power;
                }
                else
                {
                    // change the coeffecient directly because it is only number
                    an.Coeffecient = Math.Pow(an.Coeffecient, power);
                }

                // raised the fused symbols 

                for (int i = 0; i < FusedSymbols.Count; i++ )
                {
                    FusedSymbols[FusedSymbols.ElementAt(i).Key] = FusedSymbols.ElementAt(i).Value * power;
                    
                }

                if (an._AddedTerms != null)
                {
                    foreach (var term in an._AddedTerms.Values)
                    {
                        term.RaiseToSymbolicPower(term.SymbolPowerTerm * power);
                    }
                }

            }
            else
            {
                // if all conditions failed then this value is a complex value
                if (an._SymbolPowerTerm == null)
                {
                    an._SymbolPowerTerm = an.SymbolPower * b;
                }
                else
                {
                    an._SymbolPowerTerm = an._SymbolPowerTerm * b;
                }

                if (an._CoeffecientPowerTerm == null)
                {
                    an._CoeffecientPowerTerm = b;
                }
                else
                {
                    an._CoeffecientPowerTerm = an._CoeffecientPowerTerm * b;
                }

                if (an._AddedTerms != null)
                {
                    // fill the extra added terms
                    foreach (var term in an._AddedTerms.Values)
                        term.RaiseToSymbolicPower(term.SymbolPowerTerm * b);
                }
            }
        }

        /// <summary>
        /// Power of symbolic variable.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SymbolicVariable SymbolicPower(SymbolicVariable a, SymbolicVariable b)
        {
            var an = (SymbolicVariable)a.Clone(); // clone the base value
            an.RaiseToSymbolicPower(b);
            return an;
        }

        public static SymbolicVariable operator ^(SymbolicVariable a, SymbolicVariable b)
        {
            return SymbolicPower(a, b);
        }


        public static SymbolicVariable Pow(SymbolicVariable a, SymbolicVariable b)
        {
            return SymbolicPower(a, b);
        }


    }
}
