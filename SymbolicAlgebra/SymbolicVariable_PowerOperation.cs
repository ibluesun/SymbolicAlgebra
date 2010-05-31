using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
    public partial class SymbolicVariable : ICloneable
    {

        public SymbolicVariable RaiseToSymbolicPower(SymbolicVariable b)
        {
            var an = (SymbolicVariable)this.Clone();

            //if the value is only having coeffecient then there is no need to instantiate the powerterm

            if (b.IsOneTerm & b.IsThisTermCoeffecientOnly)
            {
                #region number only 

                // hold every term and multiply its power with this value.
                double power = b.Coeffecient;

                // symbol power term

                // now check if the current powerterm exist or not
                if (_SymbolPowerTerm != null)
                {
                    an._SymbolPowerTerm = _SymbolPowerTerm * power;
                }
                else
                {
                    an._SymbolPower = _SymbolPower * power;
                }

                //Coeffecient power term: 2^(x+2)  the x+2 is the coeffecient power term expressed in symbolic variable
                if (_CoeffecientPowerTerm != null)
                {
                    an._CoeffecientPowerTerm = _CoeffecientPowerTerm * power;
                }
                else
                {
                    // change the coeffecient directly because it is only number
                    an.Coeffecient = Math.Pow(Coeffecient, power);
                }

                // raised the fused symbols 

                for (int i = 0; i < FusedSymbols.Count; i++ )
                    an.FusedSymbols[FusedSymbols.ElementAt(i).Key] = FusedSymbols.ElementAt(i).Value * power;

                an._AddedTerms = null;
                if (_AddedTerms != null)
                {
                    foreach (var term in _AddedTerms.Values)
                    {
                        var tpw = term.RaiseToSymbolicPower(term.SymbolPowerTerm * power);
                        an._AddedTerms.Add(tpw.SymbolBaseValue, tpw);
                    }
                }
                #endregion

            }
            else
            {
                #region full symbolic variable
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
                    if(Math.Abs(an.Coeffecient) != 1) an._CoeffecientPowerTerm = b;
                }
                else
                {
                    an._CoeffecientPowerTerm = an._CoeffecientPowerTerm * b;
                }

                // raised the fused symbols 

                for (int i = 0; i < FusedSymbols.Count; i++)
                {
                    var fusedItem = FusedSymbols[FusedSymbols.ElementAt(i).Key];
                    if (fusedItem.SymbolicVariable == null)
                    {
                        fusedItem.SymbolicVariable = b * fusedItem.NumericalVariable;
                        fusedItem.NumericalVariable = 1;  // set it to one because it has gone to the symbolic part
                    }
                    else
                        fusedItem.SymbolicVariable = fusedItem.SymbolicVariable * b;

                    an.FusedSymbols[FusedSymbols.ElementAt(i).Key] = fusedItem;
                }

                an._AddedTerms = null;
                if (_AddedTerms != null)
                {
                    // fill the extra added terms
                    foreach (var term in _AddedTerms.Values)
                    {
                        var tpw = term.RaiseToSymbolicPower(term.SymbolPowerTerm * b);
                        an.AddedTerms.Add(tpw.SymbolBaseValue, tpw);
                    }
                }
                #endregion
            }

            return an;
        }

        /// <summary>
        /// Power of symbolic variable.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SymbolicVariable SymbolicPower(SymbolicVariable a, SymbolicVariable b)
        {
            return a.RaiseToSymbolicPower(b);
        }

        public static SymbolicVariable Pow(SymbolicVariable a, SymbolicVariable b)
        {
            return SymbolicPower(a, b);
        }
    }
}
