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
                foreach (var pf in sv._FunctionParameters)
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
                        var lnterm = new SymbolicVariable($"{FunctionOperation.LnText}(" + sv.ToString() + ")");
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
                var log = new SymbolicVariable(FunctionOperation.LnText + "(" + sv.Symbol + ")");
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

                        if (fv.FunctionName.Equals(FunctionOperation.ExpText, StringComparison.OrdinalIgnoreCase))
                        {
                            if (fv._FunctionParameters.Length != 1) throw new SymbolicException("Exp function must have one parameter for differentiation to be done.");

                            // d/dx e^x = e^x
                            // d/dx e^(x^2) = 2*x*e^(x^2)

                            var pa = fv._FunctionParameters[0];
                            var presult = pa.Differentiate(parameter);
                            fv = SymbolicVariable.Multiply(presult, fv);
                        }
                        else if (fv.FunctionName.Equals(FunctionOperation.LnText, StringComparison.OrdinalIgnoreCase))
                        {
                            if (fv._FunctionParameters.Length != 1) throw new SymbolicException("Log function must have one parameter for differentiation to be done.");

                            // d/dx ( ln( g(x) ) ) = g'(x)/g(x)

                            var pa = fv._FunctionParameters[0];
                            var dpa = pa.Differentiate(parameter);
                            fv = SymbolicVariable.Divide(dpa, pa);
                            
                        }
                        else if(fv.FunctionName.Equals(FunctionOperation.SqrtText, StringComparison.OrdinalIgnoreCase))
                        {
                            // d/dx ( sqrt( g(x) ) ) = g'(x) / 2* sqrt(g(x))
                            if (fv._FunctionParameters.Length != 1) throw new SymbolicException("Sqrt function must have one parameter for differentiation to be done.");

                            var pa = fv._FunctionParameters[0];
                            var dpa = pa.Differentiate(parameter);
                            var den = Multiply(Two, fv);
                            fv = Divide(dpa, den);

                        }
                        else if (FunctionOperation.InversedTrigFunctions.Contains(fv.FunctionName, StringComparer.OrdinalIgnoreCase))
                        {
                            fv = FunctionOperation.DiffInversedTrigFunction(fv, parameter);
                        }
                        else if (FunctionOperation.TrigFunctions.Contains(fv.FunctionName, StringComparer.OrdinalIgnoreCase))
                        {
                            // triogonometric functions 
                            bool IsNegativeResult;
                            string[] newfuntions = FunctionOperation.DiffTrigFunction(fv, out IsNegativeResult);

                            if (newfuntions != null)
                            {
                                //if(IsNegative)
                                // get the parameters in the function and differentiate them
                                if (fv._FunctionParameters.Length == 0)
                                {
                                    throw new SymbolicException("Special function without any parameters is not suitable for differentiation");
                                }
                                else if (fv._FunctionParameters.Length == 1)
                                {
                                    var pa = fv._FunctionParameters[0];
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
                        }
                        else
                        {

                            
                            // the function is not a special function like sin, cos, and log.
                            // search for the function in the running context.


                            var extendedFunction = Functions.Keys.FirstOrDefault(c => c.StartsWith(fv.FunctionName, StringComparison.OrdinalIgnoreCase));
                            if (!string.IsNullOrEmpty(extendedFunction))
                            {
                                string[] fps = extendedFunction.Substring(extendedFunction.IndexOf("(")).TrimStart('(').TrimEnd(')').Split(',');
                                    
                                if(fps.Length != fv._RawFunctionParameters.Length) throw new SymbolicException("Insufficient function parameters");

                                // replace parameters
                                var dsf = Functions[extendedFunction].ToString();

                                for(int ipxf=0; ipxf < fps.Length ;ipxf++)
                                {
                                    dsf = dsf.Replace(fps[ipxf], fv._RawFunctionParameters[ipxf]);
                                }

                                fv = SymbolicVariable.Parse(dsf).Differentiate(parameter);
                                    
                            }
                            else
                            {

                                throw new SymbolicException("This function is not a special function, and it is not being defined before .. to define a user function write f(x):= x^2  or h(u,v):=u-v^2");
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
                        var log = new SymbolicVariable(FunctionOperation.LnText + "(" + sv.Coeffecient.ToString(CultureInfo.InvariantCulture) + ")");
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


            SymbolicVariable SvDividedTerm = sv.DividedTerm;  // here we isolate the divided term for later calculations
            sv.DividedTerm = One;

            var MultipliedTerms = DeConstruct(sv);

            List<SymbolicVariable> CalculatedDiffs = new List<SymbolicVariable>(MultipliedTerms.Count);

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


            result = DiffBigTerm(result, parameter);

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