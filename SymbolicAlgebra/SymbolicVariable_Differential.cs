using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SymbolicAlgebra
{
    public partial class SymbolicVariable : ICloneable
    {

        /// <summary>
        /// Derive one pure term.
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="parameter"></param>
        private void DiffTerm(ref SymbolicVariable sv, string parameter)
        {
            // x^3*y^2*z^5    , diff to x;
            if (sv.Symbol.Equals(parameter, StringComparison.OrdinalIgnoreCase))
            {
                if (sv._SymbolPowerTerm == null)
                {
                    sv.Coeffecient *= sv._SymbolPower;
                    sv._SymbolPower -= 1;

                    if (sv._SymbolPower == 0) sv._Symbol = "";
 
                }
                else
                {
                    // symbol power term exist
                    SymbolicVariable oldPower = sv._SymbolPowerTerm;
                    sv._SymbolPowerTerm = Subtract(sv._SymbolPowerTerm, One);
                    sv = Multiply(sv, oldPower);
                }
            }
            else
            {
                // try in the fused variables
                HybridVariable hv;
                if (this._FusedSymbols.TryGetValue(parameter, out hv))
                {
                    if (hv.SymbolicVariable == null)
                    {
                        sv.Coeffecient *= hv.NumericalVariable;
                        hv.NumericalVariable -= 1;
                        sv._FusedSymbols[parameter] = hv;
                    }
                    else
                    {
                        // symbol power term exist
                        SymbolicVariable oldPower = hv.SymbolicVariable;

                        hv.SymbolicVariable = Subtract(hv.SymbolicVariable, One);
                        sv._FusedSymbols[parameter] = hv;

                        sv = Multiply(sv, oldPower);
                    }
                }
                else
                {
                    // the whole term will be converted to zero.
                    // empty everything :)
                    sv._SymbolPowerTerm = null;
                    sv._CoeffecientPowerTerm = null;
                    sv._SymbolPower = 1;
                    sv.Coeffecient = 0;
                    //sv._DividedTerm = null;
                    sv._BaseVariable = null;
                    sv._Symbol = string.Empty;
                    if (sv._SymbolPowerTerm != null)
                    {
                        sv._FusedSymbols.Clear();
                        sv._FusedSymbols = null;
                    }
                }
            }
        }

        /// <summary>
        /// Differentiate the whole term based on the needed variables ... you know what I mean :)
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public SymbolicVariable Differentiate(string parameter)
        {
            SymbolicVariable result = (SymbolicVariable)this.Clone();

            Dictionary<string, SymbolicVariable> OtherTerms = result._AddedTerms;
            result._AddedTerms = null;

            // see the first part.
            DiffTerm(ref result, parameter);


            Dictionary<string, SymbolicVariable> extraTerms = null;  // may be come from inner operations like deriving 5*x^(y-1)
            if (result._AddedTerms != null)
            {
                // added terms was created because of the operation of derivation
                extraTerms = result._AddedTerms;
                result._AddedTerms = null;
            }

            // so in the case of operation extra terms
            if (extraTerms != null)
            {
                // sum every term to the result to keep our object stable.
                foreach (var tr in extraTerms)
                    result = Add(result, tr.Value);
            }


            // take the rest terms
            if (OtherTerms != null)
            {
                for (int ix = 0; ix < OtherTerms.Count; ix++)
                {
                    var term = OtherTerms.Values.ElementAt(ix);
                    DiffTerm(ref term, parameter);

                    result = Add(result, term);
                }
            }

            AdjustZeroPowerTerms(result);

            AdjustZeroCoeffecientTerms(ref result);

            return result;
        }
    }
}