using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace SymbolicAlgebra
{
#if SILVERLIGHT
    public partial class SymbolicVariable
#else
    public partial class SymbolicVariable : ICloneable
#endif
    {
        const string lnText = "log";

        /// <summary>
        /// Derive one pure term.
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="parameter"></param>
        private static SymbolicVariable DiffTerm(SymbolicVariable term, string parameter)
        {
            var sv = (SymbolicVariable)term.Clone();

            bool symbolpowercontainParameter = false;
            if (sv._SymbolPowerTerm != null)
            {
                if (sv._SymbolPowerTerm.Symbol.Equals(parameter, StringComparison.OrdinalIgnoreCase))
                    symbolpowercontainParameter = true;
                else
                {
                    int prcount = sv._SymbolPowerTerm.FusedSymbols.Count(p => p.Key.Equals(parameter, StringComparison.OrdinalIgnoreCase));
                    symbolpowercontainParameter = prcount > 0;
                }
            }

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
            else if (symbolpowercontainParameter)
            {
                var log = new SymbolicVariable(lnText + "(" + sv.Symbol + ")");
                var dp = sv._SymbolPowerTerm.Differentiate(parameter);
                sv = SymbolicVariable.Multiply(log, SymbolicVariable.Multiply(dp, sv));
            }
            else
            {
                // try in the fused variables
                HybridVariable hv;
                if (sv.FusedSymbols.TryGetValue(parameter, out hv))
                {
                    if (hv.SymbolicVariable == null)
                    {
                        sv.Coeffecient *= hv.NumericalVariable;
                        hv.NumericalVariable -= 1;
                        if (hv.NumericalVariable == 0)
                            sv._FusedSymbols.Remove(parameter);
                        else
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
                    if (sv.IsFunction)
                    {
                        var fv = (SymbolicVariable)sv.Clone();

                        // remove the power term in this copied function term
                        fv._SymbolPowerTerm = null;
                        fv.SymbolPower = 1.0;
                        fv.Coeffecient = 1.0;


                        if (fv.FunctionName.Equals(lnText, StringComparison.OrdinalIgnoreCase))
                        {
                            
                            if (fv.FunctionParameters.Length != 1) throw new SymbolicException("log function must have one parameter for differentiation to be done.");

                            // d/dx ( ln( g(x) ) ) = g'(x)/g(x)

                            var pa = fv.FunctionParameters[0];
                            var dpa = pa.Differentiate(parameter);
                            fv = SymbolicVariable.Divide(dpa, pa);

                        }
                        else
                        {
                            bool IsNegativeResult;
                            string[] newfuntions = FunctionDiff.Diff(fv, out IsNegativeResult);

                            if (newfuntions != null)
                            {
                                //if(IsNegative)
                                // get the parameters in the function and differentiate them
                                if (fv.FunctionParameters.Length == 0)
                                {
                                    throw new SymbolicException("Special function without any parameters is not suitable for differentiation");
                                }
                                else if (fv.FunctionParameters.Length == 1)
                                {
                                    var pa = fv.FunctionParameters[0];
                                    var presult = pa.Differentiate(parameter);
                                    fv.SetFunctionName(newfuntions);

                                    if (IsNegativeResult)
                                        fv = SymbolicVariable.Multiply(presult, SymbolicAlgebra.SymbolicVariable.NegativeOne * fv);
                                    else
                                        fv = SymbolicVariable.Multiply(presult, fv);
                                }
                                else
                                {
                                    throw new SymbolicException("more than one parameter is not normal for this special function");
                                }
                            }
                            else
                            {
                                // the function is not a special function like sin, cos, and log.
                                // search for the function in the running context.
                                throw new SymbolicException("This function is not a special function, and I haven't implemented storing user functions in the running context");
                            }
                        }

                        // second treat the function normally as if it is one big symbol
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


                        sv = SymbolicVariable.Multiply(fv, sv);
                    }
                    else if (sv.IsCoeffecientOnly && sv._CoeffecientPowerTerm != null)
                    {
                        // hint: the coeffecient only term has power of 1 or symbolic power should exist in case of raise to symbolic power

                        // get log(coeffeniect)
                        var log = new SymbolicVariable(lnText + "(" + sv.Coeffecient.ToString() + ")");
                        var dp = sv._CoeffecientPowerTerm.Differentiate(parameter);
                        sv = SymbolicVariable.Multiply(log, SymbolicVariable.Multiply(dp, sv));
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

            return sv;
        }

        private static SymbolicVariable DiffBigTerm(SymbolicVariable sv, string parameter)
        {
            // now result contains only one term
            // -----------------------------------

            // we need to differentiate multiplied terms   in this  ONE TERM
            // every term is differentiated and multiplied by other terms  if  x*y*z  then == dx*y*z+x*dy+z+x*y*dz

            int MultipliedTermsCount = sv.FusedSymbols.Count + sv.FusedConstants.Count + 1; // last one is the basic symbol and coeffecient in the instant

            // separate all terms into array by flatting them
            List<SymbolicVariable> MultipliedTerms = new List<SymbolicVariable>(MultipliedTermsCount);

            Action<SymbolicVariable> SpliBaseTerm = (rr) =>
            {
                var basicterm = (SymbolicVariable)rr.Clone();
                basicterm._FusedConstants = null;
                basicterm._FusedSymbols = null;
                
                // split coeffecient and its associated symbol

                if (basicterm.CoeffecientPowerTerm != null)
                {
                    // coefficient
                    SymbolicVariable CoeffecientOnly = new SymbolicVariable("");
                    CoeffecientOnly._CoeffecientPowerTerm = basicterm.CoeffecientPowerTerm;
                    CoeffecientOnly.Coeffecient = basicterm.Coeffecient;
                    MultipliedTerms.Add(CoeffecientOnly);

                    // multiplied symbol
                    if (!string.IsNullOrEmpty(basicterm.SymbolBaseValue))
                        MultipliedTerms.Add(SymbolicVariable.Parse(basicterm.SymbolBaseValue));
                }
                else
                {
                    MultipliedTerms.Add(basicterm);
                }

            };

            Action<SymbolicVariable> SpliFusedConstants = (rr) =>
            {
                var basicterm = (SymbolicVariable)rr.Clone();

                var FCConstants = basicterm._FusedConstants;

                // Key  is the coefficient
                //  value contains the power  which always will be symbolic power or null
                foreach (var FC in FCConstants)
                {
                    SymbolicVariable CoeffecientOnly = new SymbolicVariable("");
                    CoeffecientOnly._CoeffecientPowerTerm = (SymbolicVariable)FC.Value.SymbolicVariable.Clone();
                    CoeffecientOnly.Coeffecient = FC.Key;

                    MultipliedTerms.Add(CoeffecientOnly);
                }
            };

            Action<SymbolicVariable> SplitFusedSymbols = (rr) =>
            {
                var basicterm = (SymbolicVariable)rr.Clone();

                var FSymbols = basicterm._FusedSymbols;

                // Key  is the coefficient
                //  value contains the power  which always will be symbolic power or null
                foreach (var FS in FSymbols)
                {
                    var ss = new SymbolicVariable(FS.Key);
                    ss.SymbolPower = FS.Value.NumericalVariable;
                    if (FS.Value.SymbolicVariable != null)
                        ss._SymbolPowerTerm = (SymbolicVariable)FS.Value.SymbolicVariable.Clone();

                    MultipliedTerms.Add(ss);
                }
            };

            SpliBaseTerm(sv);
            if (sv.FusedConstants.Count > 0) SpliFusedConstants(sv);
            if (sv.FusedSymbols.Count > 0) SplitFusedSymbols(sv);

            List<SymbolicVariable> CalculatedDiffs = new List<SymbolicVariable>(MultipliedTermsCount);

            // get all differentials of all terms                       // x*y*z ==>  dx  dy  dz 
            for (int ix = 0; ix < MultipliedTerms.Count; ix++)
            {
                CalculatedDiffs.Add(DiffTerm(MultipliedTerms[ix], parameter));
            }

            // every result of calculated differentials should be multiplied by the other terms.
            for(int ix =0; ix<CalculatedDiffs.Count; ix++)
            {
                var term = CalculatedDiffs[ix];

                if (term.IsZero) continue;
                var mt = One;

                for(int iy=0;iy<MultipliedTerms.Count;iy++)
                {
                    if (iy == ix) continue;
                    mt = SymbolicVariable.Multiply(mt, MultipliedTerms[iy]);
                }

                // term *mt
                CalculatedDiffs[ix] = SymbolicVariable.Multiply(mt, term);

            }

            //       dx*y*z     dy*x*z       dz*x*y

            var total = Zero;

            foreach (var cc in CalculatedDiffs)
            {
                if (cc.IsZero) continue;

                total = SymbolicVariable.Add(total, cc);
            }


            return total;
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
            //DiffTerm(ref result, parameter);
            result = DiffBigTerm(result, parameter);



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
                    term = DiffBigTerm(term, parameter);

                    result = Add(result, term);
                }
            }

            AdjustZeroPowerTerms(result);

            AdjustZeroCoeffecientTerms(ref result);

            return result;
        }
    }
}