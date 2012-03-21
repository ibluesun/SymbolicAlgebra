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

        public static SymbolicVariable Divide(SymbolicVariable a, SymbolicVariable b)
        {
            if (a == null || b == null) return null;

            SymbolicVariable SourceTerm = (SymbolicVariable)a.Clone();

            // if the divided term is more than on term
            // x^2/(y-x)  ==>  
            if (b.AddedTerms.Count > 0)
            {

                //multiply divided term by this value
                SourceTerm.DividedTerm = Multiply(SourceTerm.DividedTerm, b);

                return SourceTerm;
            }


            SymbolicVariable TargetSubTerm = (SymbolicVariable)b.Clone();


            TargetSubTerm._AddedTerms = null;   // remove added variables to prevent its repeated calculations in second passes
            // or to make sure nothing bad happens {my idiot design :S)

            if (a.BaseEquals(TargetSubTerm))
            {
                #region symbols are equal (I mean 2*x^3 = 2*X^3)

                DivideCoeffecients(ref SourceTerm, TargetSubTerm);

                if (a.SymbolPowerTerm != null || TargetSubTerm.SymbolPowerTerm != null)
                {
                    SourceTerm._SymbolPowerTerm = a.SymbolPowerTerm - TargetSubTerm.SymbolPowerTerm;
                }
                else
                {
                    SourceTerm.SymbolPower = SourceTerm.SymbolPower - TargetSubTerm.SymbolPower;
                }

                

                //fuse the fused symbols in b into sv
                foreach (var bfv in TargetSubTerm.FusedSymbols)
                {
                    if (SourceTerm.FusedSymbols.ContainsKey(bfv.Key))
                        SourceTerm.FusedSymbols[bfv.Key] -= bfv.Value;
                    else
                        SourceTerm.FusedSymbols.Add(bfv.Key,  bfv.Value * -1);
                }
                #endregion
            }
            else
            {
                #region Symbols are different
                if (string.IsNullOrEmpty(SourceTerm.Symbol))
                {
                    #region First Case: Source primary symbol doesn't exist
                    // the instance have an empty primary variable so we should add it 
                    
                    if (TargetSubTerm._BaseVariable != null) SourceTerm._BaseVariable = TargetSubTerm._BaseVariable;
                    else SourceTerm.Symbol = TargetSubTerm.Symbol; 
                    
                    SourceTerm.SymbolPower = -1 * TargetSubTerm.SymbolPower;
                    if (TargetSubTerm.SymbolPowerTerm != null) SourceTerm._SymbolPowerTerm = -1 * (SymbolicVariable)TargetSubTerm.SymbolPowerTerm.Clone();

                    //fuse the fusedvariables in b into sv
                    foreach (var bfv in TargetSubTerm.FusedSymbols)
                    {
                        if (SourceTerm.FusedSymbols.ContainsKey(bfv.Key))
                            SourceTerm.FusedSymbols[bfv.Key] -= bfv.Value;
                        else
                            SourceTerm.FusedSymbols.Add(bfv.Key,  bfv.Value * -1);
                    }
                    #endregion
                }
                else
                {
                    if (SourceTerm.Symbol.Equals(TargetSubTerm.Symbol, StringComparison.OrdinalIgnoreCase))
                    {
                        #region Second Case: Primary symbol in both source and target exist and equal
                        if (SourceTerm._SymbolPowerTerm != null || TargetSubTerm._SymbolPowerTerm != null)
                        {
                            // make sure the object of symbol power term have values if they don't
                            if (SourceTerm._SymbolPowerTerm == null)
                            {
                                // transfer the numerical power into symbolic variable mode
                                SourceTerm._SymbolPowerTerm = new SymbolicVariable(SourceTerm.SymbolPower.ToString());

                                // also revert the original symbol power into 1  for validation after this
                                SourceTerm.SymbolPower = 1;
                            }

                            if (TargetSubTerm._SymbolPowerTerm == null)
                            {
                                TargetSubTerm._SymbolPowerTerm = new SymbolicVariable(TargetSubTerm.SymbolPower.ToString());

                                TargetSubTerm.SymbolPower = -1; // the target will always be dominator
                            }

                            SourceTerm._SymbolPowerTerm -= TargetSubTerm._SymbolPowerTerm;

                            if (SourceTerm._SymbolPowerTerm.IsCoeffecientOnly)
                            {
                                SourceTerm._SymbolPower = SourceTerm._SymbolPowerTerm.Coeffecient;
                                SourceTerm._SymbolPowerTerm = null;  // remove it because we don't need symbolic when we reach co
                            }
                            else
                            {
                                // correct the source symbol power (because after division the power may still be positive
                                if (SourceTerm._SymbolPowerTerm.IsNegative)
                                {
                                    SourceTerm._SymbolPower = -1;

                                    SourceTerm._SymbolPowerTerm *= NegativeOne;

                                }
                            }
                        }
                        else
                        {
                            SourceTerm.SymbolPower -= TargetSubTerm.SymbolPower;
                        }

                        // also subtract the fused variables
                        foreach (var fv in TargetSubTerm.FusedSymbols)
                        {
                            if (SourceTerm.FusedSymbols.ContainsKey(fv.Key))
                                SourceTerm.FusedSymbols[fv.Key] -= fv.Value;
                            else
                                SourceTerm.FusedSymbols.Add(fv.Key, fv.Value * -1);
                        }
                        #endregion

                    }
                    else if (SourceTerm.FusedSymbols.ContainsKey(TargetSubTerm.Symbol))
                    {
                        #region Third Case: Target primary symbol exist in source fused variables

                        if (TargetSubTerm.SymbolPowerTerm != null)
                        {
                            SourceTerm.FusedSymbols[TargetSubTerm.Symbol] -= TargetSubTerm.SymbolPowerTerm;
                        }
                        else
                        {
                            SourceTerm.FusedSymbols[TargetSubTerm.Symbol] -= TargetSubTerm.SymbolPower;
                        }

                        // however primary symbol in source still the same so we need to add it to the value in target (if applicable)

                        // also 

                        // there are still some fused variables in source that weren't altered by the target fused symbols
                        // go through every symbol in fused symbols and add it to the source fused symbols
                        foreach (var tfs in TargetSubTerm.FusedSymbols)
                        {
                            if (SourceTerm.FusedSymbols.ContainsKey(tfs.Key))
                            {
                                // symbol exist so accumulate it
                                SourceTerm._FusedSymbols[tfs.Key] -= tfs.Value;
                            }
                            else
                            {
                                // two cases here
                                // 1 the fused key equal the primary symbol in source
                                if (SourceTerm.Symbol.Equals(tfs.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (tfs.Value.SymbolicVariable != null)
                                    {
                                        if (SourceTerm._SymbolPowerTerm != null)
                                        {
                                            SourceTerm._SymbolPowerTerm -= tfs.Value.SymbolicVariable;
                                        }
                                        else
                                        {
                                            // sum the value in the numerical part to the value in symbolic part
                                            SourceTerm._SymbolPowerTerm = new SymbolicVariable(SourceTerm._SymbolPower.ToString(CultureInfo.InvariantCulture)) - tfs.Value.SymbolicVariable;
                                            // reset the value in numerical part
                                            SourceTerm._SymbolPower = -1;
                                        }

                                        if(SourceTerm._SymbolPowerTerm.IsCoeffecientOnly) 
                                        {
                                            SourceTerm._SymbolPower = SourceTerm._SymbolPowerTerm.Coeffecient;
                                            SourceTerm._SymbolPowerTerm = null;
                                        }
                                    }
                                    else
                                        SourceTerm._SymbolPower -= tfs.Value.NumericalVariable;
                                }
                                else
                                {
                                    // 2 no matching at all which needs to add the symbol in target into the fused symbols in source.
                                    SourceTerm.FusedSymbols.Add(tfs.Key, tfs.Value * -1);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region Fourth Case: Target primary symbol doesn't exist in Source Primary Symbol nor Source Fused symbols

                        // Add Target primary symbol to the fused symbols in source
                        SourceTerm.FusedSymbols.Add(
                            TargetSubTerm.Symbol,
                            new HybridVariable
                            {
                                NumericalVariable = -1 * TargetSubTerm.SymbolPower,
                                SymbolicVariable = TargetSubTerm.SymbolPowerTerm == null ? null : -1 * (SymbolicVariable)TargetSubTerm.SymbolPowerTerm
                            });

                        // But the primary symbol of source may exist in the target fused variables.

                        foreach (var fsv in TargetSubTerm.FusedSymbols)
                        {
                            if (SourceTerm.FusedSymbols.ContainsKey(fsv.Key))
                                SourceTerm.FusedSymbols[fsv.Key] -= fsv.Value;
                            else
                            {
                                // 1- if symbol is the same as priamry source 
                                if (SourceTerm.Symbol.Equals(fsv.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (fsv.Value.SymbolicVariable != null)
                                    {
                                        if (SourceTerm._SymbolPowerTerm != null)
                                            SourceTerm._SymbolPowerTerm -= fsv.Value.SymbolicVariable;
                                        else
                                        {
                                            // sum the value in the numerical part to the value in symbolic part
                                            SourceTerm._SymbolPowerTerm =
                                                new SymbolicVariable(SourceTerm._SymbolPower.ToString(CultureInfo.InvariantCulture)) - fsv.Value.SymbolicVariable;
                                            // reset the value in numerical part
                                            SourceTerm._SymbolPower = -1;
                                        }
                                    }
                                    else
                                    {
                                        if (SourceTerm.SymbolPowerTerm != null)
                                        {
                                            SourceTerm._SymbolPowerTerm -= new SymbolicVariable(fsv.Value.ToString());
                                        }
                                        else
                                        {
                                            SourceTerm._SymbolPower -= fsv.Value.NumericalVariable;
                                        }
                                    }

                                }
                                else
                                {
                                    SourceTerm.FusedSymbols.Add(fsv.Key, fsv.Value * -1);
                                }
                            }
                        }
                        #endregion
                    }
                }

                DivideCoeffecients(ref SourceTerm, TargetSubTerm);
                #endregion
            }

            if (SourceTerm.AddedTerms.Count > 0)
            {
                Dictionary<string, SymbolicVariable> newAddedVariables = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                foreach (var vv in SourceTerm.AddedTerms)
                {
                    var newv = Divide (vv.Value , TargetSubTerm);
                    newAddedVariables.Add(newv.WholeValueBaseKey, newv);

                }
                SourceTerm._AddedTerms = newAddedVariables;
            }

            // Extra Terms of a
            if (SourceTerm.ExtraTerms.Count > 0)
            {
                List<ExtraTerm> newExtraTerms = new List<ExtraTerm>();
                foreach (var et in SourceTerm.ExtraTerms)
                {
                    var newe = Divide(et.Term, TargetSubTerm);
                    newExtraTerms.Add(new ExtraTerm { Term = newe });
                }
                SourceTerm._ExtraTerms = newExtraTerms;
            }


            int subIndex = 0;
            SymbolicVariable total = SourceTerm;

            while (subIndex < b.AddedTerms.Count)
            {
                // we should multiply other parts also 
                // then add it to the current instance

                // there are still terms to be consumed 
                //   this new term is a sub term in b and will be added to all terms of a.
                TargetSubTerm = b.AddedTerms.ElementAt(subIndex).Value;

                total = total + Divide(a, TargetSubTerm);
                subIndex = subIndex + 1;  //increase     
            }

            // for extra terms  {terms that has different divided term}
            int extraIndex = 0;
            while (extraIndex < b.ExtraTerms.Count)
            {
                TargetSubTerm = b.ExtraTerms[extraIndex].Term;
                var TargetTermSubTotal = Divide(a, TargetSubTerm);
                total = Add(total, TargetTermSubTotal);
                extraIndex++;
            }


            AdjustSpecialFunctions(ref total);

            AdjustZeroPowerTerms(total);
            AdjustZeroCoeffecientTerms(ref total);


            return total; 
        }



        /// <summary>
        /// Multiply Coeffecients of second argument into first argument.
        /// </summary>
        /// <param name="SourceTerm"></param>
        /// <param name="TargetSubTerm"></param>
        private static void DivideCoeffecients(ref SymbolicVariable SourceTerm, SymbolicVariable TargetSubTerm)
        {
            // Note: I will try to avoid the primary coeffecient so it doesn't hold power
            //      and only hold coeffecient itself.
            foreach (var cst in TargetSubTerm.Constants)
            {

                if (SourceTerm._CoeffecientPowerTerm == null && cst.ConstantPower == null)
                {
                    SourceTerm.Coeffecient /= cst.ConstantValue;
                }
                // there is a coeffecient power term needs to be injected into the source
                else
                {
                    // no the coeffecient part is not empty so we will test if bases are equal
                    // then make use of the fusedsymbols to add our constant


                    double sbase = Math.Log(SourceTerm.Coeffecient, cst.ConstantValue);

                    if (SourceTerm.Coeffecient == cst.ConstantValue)
                    {
                        // sample: 2^x/2^y = 2^(x-y)
                        var cpower = cst.ConstantPower;
                        if (cpower == null) cpower = One;

                        if (SourceTerm._CoeffecientPowerTerm == null) SourceTerm._CoeffecientPowerTerm = One;
                        SourceTerm._CoeffecientPowerTerm -= cpower;
                    }
                    else if ((sbase - Math.Floor(sbase)) == 0.0 && sbase > 0)   // make sure there are no
                    {
                        // then we can use the target base
                        // 27/3^x = 3^3/3^x = 3^(3-x)
                        var cpower = cst.ConstantPower;
                        if (cpower == null) cpower = new SymbolicVariable("1");
                        SourceTerm._CoeffecientPowerTerm = SymbolicVariable.Subtract(new SymbolicVariable(sbase.ToString()), cpower);
                        SourceTerm.Coeffecient = cst.ConstantValue;

                    }
                    else
                    {
                        // sample: 2^(x+y)*log(2)*3^y * 3^z   can't be summed.
                        HybridVariable SameCoeffecient;
                        if (SourceTerm.FusedConstants.TryGetValue(cst.ConstantValue, out SameCoeffecient))
                        {
                            SameCoeffecient.SymbolicVariable -= cst.ConstantPower;
                            SourceTerm.FusedConstants[cst.ConstantValue] = SameCoeffecient;
                        }
                        else
                        {
                            // Add Target coefficient symbol to the fused symbols in source, with key as the coefficient number itself.
                            if (cst.ConstantPower != null)
                            {
                                SourceTerm.FusedConstants.Add(
                                    cst.ConstantValue,
                                    new HybridVariable
                                    {
                                        NumericalVariable = -1, // power
                                        SymbolicVariable = Subtract(Zero, cst.ConstantPower)
                                    });
                            }
                            else
                            {
                                // coeffecient we working with is number only
                                // First Attempt: to try to get the power of this number with base of available coeffecients
                                // if no base produced an integer power value then we add it into fused constants as it is.

                                if (cst.ConstantValue == 1.0) continue;  // ONE doesn't change anything so we bypass it

                                double? SucceededConstant = null;
                                double ower = 0;
                                foreach (var p in SourceTerm.Constants)
                                {
                                    ower = Math.Log(cst.ConstantValue, p.ConstantValue);
                                    if (ower == Math.Floor(ower))
                                    {
                                        SucceededConstant = p.ConstantValue;
                                        break;
                                    }
                                }

                                if (SucceededConstant.HasValue)
                                {
                                    if (SourceTerm.Coeffecient == SucceededConstant.Value)
                                    {
                                        SourceTerm._CoeffecientPowerTerm -= new SymbolicVariable(ower.ToString());
                                    }
                                    else
                                    {
                                        var rr = SourceTerm.FusedConstants[SucceededConstant.Value];

                                        rr.SymbolicVariable -= new SymbolicVariable(ower.ToString());
                                        SourceTerm.FusedConstants[SucceededConstant.Value] = rr;
                                    }

                                }
                                else
                                {
                                    SourceTerm.FusedConstants.Add(
                                        cst.ConstantValue,
                                        new HybridVariable
                                        {
                                            NumericalVariable = -1, // power
                                        });
                                }
                            }
                        }
                    }
                }
            }
        }    

    }
}
