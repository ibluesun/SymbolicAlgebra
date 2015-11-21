using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
    {
        const string lnText = "log";

        /// <summary>
        /// Derive one pure term.
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="parameter"></param>
        private static SymbolicVariable DiffTerm(SymbolicVariable term, string parameter)
        {
            var sv = term.Clone();

            bool symbolpowercontainParameter = false;
            if (sv._SymbolPowerTerm != null)
            {
                if (sv._SymbolPowerTerm.InvolvedSymbols.Contains(parameter, StringComparer.OrdinalIgnoreCase))
                {
                    symbolpowercontainParameter = true;
                }
                else
                {
                    int prcount = sv._SymbolPowerTerm.FusedSymbols.Count(p => p.Key.Equals(parameter, StringComparison.OrdinalIgnoreCase));
                    symbolpowercontainParameter = prcount > 0;
                }
            }

            bool cc = false;
            if(sv.BaseVariable!=null) cc = sv.BaseVariable.InvolvedSymbols.Contains(parameter, StringComparer.OrdinalIgnoreCase); // case of base variable
            else if (sv.IsFunction && symbolpowercontainParameter == true)
            {
                // search if a parameter contains the same parameter
                foreach (var pf in sv.FunctionParameters)
                    if (pf.Symbol.Equals(parameter, StringComparison.OrdinalIgnoreCase))
                    {
                        cc = true;
                        break;
                    }
            }
            else
            {
                cc = sv.Symbol.Equals(parameter, StringComparison.OrdinalIgnoreCase);
            }

            // x^3*y^2*z^5    , diff to x;
            if (cc)
            {
                if (sv._SymbolPowerTerm == null)
                {
                    sv.Coeffecient *= sv._SymbolPower;
                    sv._SymbolPower -= 1;
                    if (sv._SymbolPower == 0) sv._Symbol = "";
                }
                else
                {
                    if (symbolpowercontainParameter)
                    {
                        // here is the case when base and power are the same with the parameter we are differentiating with
                        //  i.e.  x^x|x
                        // Logarithmic Differentiation   
                        var lnterm = new SymbolicVariable("log(" + sv.ToString() + ")");
                        var dlnterm = lnterm.Differentiate(parameter);
                        sv = Multiply(sv, dlnterm);

                    }
                    else
                    {
                        // symbol power term exist
                        SymbolicVariable oldPower = sv._SymbolPowerTerm;
                        sv._SymbolPowerTerm = Subtract(sv._SymbolPowerTerm, One);
                        sv = Multiply(sv, oldPower);
                    }
                }
            }
            else if (symbolpowercontainParameter)
            {
                // this case is when the power term is the same 
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
                        var fv = sv.Clone();

                        // remove the power term in this copied function term
                        fv._SymbolPowerTerm = null;
                        fv.SymbolPower = 1.0;
                        fv.Coeffecient = 1.0;


                        if (fv.FunctionName.Equals(lnText, StringComparison.OrdinalIgnoreCase))
                        {
                            if (fv.FunctionParameters.Length != 1) throw new SymbolicException("Log function must have one parameter for differentiation to be done.");

                            // d/dx ( ln( g(x) ) ) = g'(x)/g(x)

                            var pa = fv.FunctionParameters[0];
                            var dpa = pa.Differentiate(parameter);
                            fv = SymbolicVariable.Divide(dpa, pa);
                            
                        }
                        else if(fv.FunctionName.Equals("sqrt", StringComparison.OrdinalIgnoreCase))
                        {
                            // d/dx ( sqrt( g(x) ) ) = g'(x) / 2* sqrt(g(x))
                            if (fv.FunctionParameters.Length != 1) throw new SymbolicException("Sqrt function must have one parameter for differentiation to be done.");

                            var pa = fv.FunctionParameters[0];
                            var dpa = pa.Differentiate(parameter);
                            var den = Multiply(Two, fv);
                            fv = Divide(dpa, den);

                        }
                        else if (FunctionDiff.BFunctions.Contains(fv.FunctionName, StringComparer.OrdinalIgnoreCase))
                        {
                            fv = FunctionDiff.DiffBFunction(fv, parameter);
                        }
                        else
                        {
                            // triogonometric functions 
                            bool IsNegativeResult;
                            string[] newfuntions = FunctionDiff.DiffFFunction(fv, out IsNegativeResult);

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


                                var extendedFunction = Functions.Keys.FirstOrDefault(c => c.StartsWith(fv.FunctionName, StringComparison.OrdinalIgnoreCase));
                                if (!string.IsNullOrEmpty(extendedFunction))
                                {
                                    string[] fps = extendedFunction.Substring(extendedFunction.IndexOf("(")).TrimStart('(').TrimEnd(')').Split(',');
                                    
                                    if(fps.Length != fv.RawFunctionParameters.Length) throw new SymbolicException("Insufficient function parameters");

                                    // replace parameters
                                    var dsf = Functions[extendedFunction].ToString();

                                    for(int ipxf=0; ipxf < fps.Length ;ipxf++)
                                    {
                                        dsf = dsf.Replace(fps[ipxf], fv.RawFunctionParameters[ipxf]);
                                    }

                                    fv = SymbolicVariable.Parse(dsf).Differentiate(parameter);
                                    
                                }
                                else
                                {



                                    throw new SymbolicException("This function is not a special function, and I haven't implemented storing user functions in the running context");
                                }
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
                        var log = new SymbolicVariable(lnText + "(" + sv.Coeffecient.ToString(CultureInfo.InvariantCulture) + ")");
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

        private struct MultipliedTerm
        {
            
            public SymbolicVariable Term;
            public bool Divided;
            public MultipliedTerm(SymbolicVariable term)
            {
                Term = term;
                Divided = false;
            }

            public MultipliedTerm(SymbolicVariable term, bool divided)
            {
                Term = term;
                Divided = divided;
            }
        }

        private static SymbolicVariable DiffBigTerm(SymbolicVariable sv, string parameter)
        {
            // now result contains only one term
            // -----------------------------------

            // we need to differentiate multiplied terms   in this  ONE TERM
            // every term is differentiated and multiplied by other terms  if  x*y*z  then == dx*y*z+x*dy+z+x*y*dz

            int MultipliedTermsCount = sv.FusedSymbols.Count + sv.FusedConstants.Count + 1; // last one is the basic symbol and coeffecient in the instant

            SymbolicVariable SvDividedTerm = sv.DividedTerm;  // here we isolate the divided term for later calculations
            sv.DividedTerm = One;

            // separate all terms into array by flatting them
            List<MultipliedTerm> MultipliedTerms = new List<MultipliedTerm>(MultipliedTermsCount);

            Action<SymbolicVariable> SpliBaseTerm = (rr) =>
            {
                var basicterm = rr.Clone();
                basicterm._FusedConstants = null;
                basicterm._FusedSymbols = null;
                
                // split coeffecient and its associated symbol

                if (basicterm.CoeffecientPowerTerm != null)
                {
                    // coefficient
                    SymbolicVariable CoeffecientOnly = new SymbolicVariable("");
                    CoeffecientOnly._CoeffecientPowerTerm = basicterm.CoeffecientPowerTerm;
                    CoeffecientOnly.Coeffecient = basicterm.Coeffecient;
                    MultipliedTerms.Add(new MultipliedTerm(CoeffecientOnly));

                    // multiplied symbol
                    if (!string.IsNullOrEmpty(basicterm.SymbolBaseKey))
                        MultipliedTerms.Add(new MultipliedTerm(SymbolicVariable.Parse(basicterm.WholeValueBaseKey)));
                }
                else
                {
                    MultipliedTerms.Add(new MultipliedTerm(basicterm));
                }

            };

            Action<SymbolicVariable> SpliFusedConstants = (rr) =>
            {
                var basicterm = rr.Clone();

                var FCConstants = basicterm._FusedConstants;

                // Key  is the coefficient
                //  value contains the power  which always will be symbolic power or null
                foreach (var FC in FCConstants)
                {
                    SymbolicVariable CoeffecientOnly = new SymbolicVariable("");
                    CoeffecientOnly._CoeffecientPowerTerm = FC.Value.SymbolicVariable.Clone();
                    CoeffecientOnly.Coeffecient = FC.Key;

                    MultipliedTerms.Add(new MultipliedTerm( CoeffecientOnly));
                }
            };

            Action<SymbolicVariable> SplitFusedSymbols = (rr) =>
            {
                var basicterm = rr.Clone();

                var FSymbols = basicterm._FusedSymbols;

                // Key  is the coefficient
                //  value contains the power  which always will be symbolic power or null
                foreach (var FS in FSymbols)
                {
                    var ss = new SymbolicVariable(FS.Key);
                    ss.SymbolPower = FS.Value.NumericalVariable;
                    if (FS.Value.SymbolicVariable != null)
                        ss._SymbolPowerTerm = FS.Value.SymbolicVariable.Clone();

                    MultipliedTerms.Add(new MultipliedTerm( ss));
                }
            };

            SpliBaseTerm(sv);
            if (sv.FusedConstants.Count > 0) SpliFusedConstants(sv);
            if (sv.FusedSymbols.Count > 0) SplitFusedSymbols(sv);

            List<SymbolicVariable> CalculatedDiffs = new List<SymbolicVariable>(MultipliedTermsCount);

            // get all differentials of all terms                       // x*y*z ==>  dx  dy  dz 
            for (int ix = 0; ix < MultipliedTerms.Count; ix++)
            {
                CalculatedDiffs.Add(DiffTerm(MultipliedTerms[ix].Term, parameter));
            }

            // what about divided term ??
            if (!SvDividedTerm.IsOne)
            {
                /*
                 * diff(f(x)*g(x)/h(x),x);
                 *      -(f(x)*g(x)*('diff(h(x),x,1)))/h(x)^2       <== notice the negative sign here and the squared denominator
                 *      +(f(x)*('diff(g(x),x,1)))/h(x)
                 *      +(g(x)*('diff(f(x),x,1)))/h(x)
                 */
                var dvr = Subtract(Zero, SvDividedTerm.Differentiate(parameter));  //differential of divided takes minus sign because it wil
                CalculatedDiffs.Add(dvr);

                // add the divided term but in negative  value because it is divided and in differentiation it will have -ve power
                MultipliedTerms.Add(new MultipliedTerm(SvDividedTerm, true));
            }

            // every result of calculated differentials should be multiplied by the other terms.
            for (int ix = 0; ix < CalculatedDiffs.Count; ix++)
            {
                var term = CalculatedDiffs[ix];

                if (term.IsZero) continue;
                var mt = One;
                int mltc = MultipliedTerms.Count;

                //if (!SvDividedTerm.IsOne) mltc++;

                for (int iy = 0; iy < mltc; iy++)
                {
                    if (iy == ix) continue;

                    if (MultipliedTerms[iy].Divided)
                        mt = SymbolicVariable.Divide(mt, MultipliedTerms[iy].Term);
                    else
                        mt = SymbolicVariable.Multiply(mt, MultipliedTerms[iy].Term);
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

            if (!SvDividedTerm.IsOne) total.DividedTerm  = Multiply(SvDividedTerm, SvDividedTerm);

            return total;
        }


        /// <summary>
        /// Differentiate the whole term based on the needed variables ... you know what I mean :)
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public SymbolicVariable Differentiate(string parameter)
        {


            SymbolicVariable result = this.Clone();

            Dictionary<string, SymbolicVariable> OtherAddedTerms = result._AddedTerms;
            result._AddedTerms = null;

            List<ExtraTerm> OtherExtraTerms = result._ExtraTerms;
            result._ExtraTerms = null;



            
            // see the first part.
            //DiffTerm(ref result, parameter);
            result = DiffBigTerm(result, parameter);


            /* Removed because I used extra terms in the anatomy of the symbolic variable class
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
            */

            // take the rest terms
            if (OtherAddedTerms != null)
            {
                for (int ix = 0; ix < OtherAddedTerms.Count; ix++)
                {
                    var term = OtherAddedTerms.Values.ElementAt(ix);
                    term = DiffBigTerm(term, parameter);

                    result = Add(result, term);
                }
            }

            if (OtherExtraTerms != null)
            {
                for (int ix = 0; ix < OtherExtraTerms.Count; ix++)
                {
                    var term = OtherExtraTerms[ix].Term;
                    term = DiffBigTerm(term, parameter);
                    result = Add(result, term);
                }
            }


            AdjustSpecialFunctions(ref result);

            AdjustZeroPowerTerms(result);

            AdjustZeroCoeffecientTerms(ref result);

            return result;
        }
    }
}