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
        /// Multiply two symbolic variables
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SymbolicVariable Multiply(SymbolicVariable a, SymbolicVariable b)
        {
            if (a == null || b == null) return null;

            SymbolicVariable TargetSubTerm = (SymbolicVariable)b.Clone();

            TargetSubTerm._AddedTerms = null;   // remove added variables to prevent its repeated calculations in second passes
            // or to make sure nothing bad happens {my idiot design :S)

            SymbolicVariable SourceTerm = (SymbolicVariable)a.Clone();
            if (a.BaseEquals(TargetSubTerm))
            {
                #region Symbols are Equal (I mean 2*x^3 = 2*X^3)
                

                MultiplyCoeffecients(ref SourceTerm, TargetSubTerm);

                    
                if (a.SymbolPowerTerm != null || TargetSubTerm.SymbolPowerTerm != null)
                {
                    SourceTerm._SymbolPowerTerm = a.SymbolPowerTerm + TargetSubTerm.SymbolPowerTerm;
                }
                else
                {
                    SourceTerm.SymbolPower = SourceTerm.SymbolPower + TargetSubTerm.SymbolPower;
                }

                //fuse the fusedvariables in b into sv
                foreach (var bfv in TargetSubTerm.FusedSymbols)
                {
                    if (SourceTerm.FusedSymbols.ContainsKey(bfv.Key))
                        SourceTerm.FusedSymbols[bfv.Key] += bfv.Value;
                    else
                        SourceTerm.FusedSymbols.Add(bfv.Key, bfv.Value);
                }
                
                #endregion
            }
            else
            {
                #region Symbols are Different
                
                if (string.IsNullOrEmpty(SourceTerm.Symbol))
                {
                    #region First Case: Source primary symbol doesn't exist
                    
                    //  so take the primary symbol from the target into source
                    //  and copy the symbol power to it  and symbolic power
                    // 

                    // the instance have an empty primary variable so we should add it 
                    SourceTerm.Symbol = TargetSubTerm.Symbol;
                    SourceTerm.SymbolPower = TargetSubTerm.SymbolPower;
                    if (TargetSubTerm.SymbolPowerTerm != null) 
                        SourceTerm._SymbolPowerTerm = (SymbolicVariable)TargetSubTerm.SymbolPowerTerm.Clone();
                    else 
                        SourceTerm._SymbolPowerTerm = null;

                    


                    //fuse the fused variables in target into source
                    foreach (var fv in TargetSubTerm.FusedSymbols)
                    {
                        if (SourceTerm.FusedSymbols.ContainsKey(fv.Key))
                            SourceTerm.FusedSymbols[fv.Key] += fv.Value;
                        else
                            SourceTerm.FusedSymbols.Add(fv.Key, fv.Value);
                    }
                    #endregion
                }
                else
                {
                    #region Testing against symbol of targetsubterm
                    if (SourceTerm.Symbol.Equals(TargetSubTerm.Symbol, StringComparison.OrdinalIgnoreCase))
                    {
                        #region Second Case: Primary symbol in both source and target exist and equal

                        // which means the difference is only in fused variables.

                        // test for complex power (power that contains other symbolic variable) 
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
                                TargetSubTerm.SymbolPower = 1;
                            }

                            // we used symbol power term
                            SourceTerm._SymbolPowerTerm += TargetSubTerm._SymbolPowerTerm;
                        }
                        else
                        {
                            SourceTerm.SymbolPower += TargetSubTerm.SymbolPower;
                        }

                        // then add the fused variables
                        foreach (var fv in TargetSubTerm.FusedSymbols)
                        {
                            if (SourceTerm.FusedSymbols.ContainsKey(fv.Key))
                                SourceTerm.FusedSymbols[fv.Key] += fv.Value;
                            else
                                SourceTerm.FusedSymbols.Add(fv.Key, fv.Value);
                        }
                        #endregion

                    }
                    else if (SourceTerm.FusedSymbols.ContainsKey(TargetSubTerm.Symbol))
                    {
                        #region Third Case: Target primary symbol exist in source fused variables

                        // fill the source symbol in fused variables from the primary symbol in Target term.
                        if (TargetSubTerm.SymbolPowerTerm != null)
                            SourceTerm.FusedSymbols[TargetSubTerm.Symbol] +=
                                new HybridVariable
                                {
                                    NumericalVariable = TargetSubTerm.SymbolPower,
                                    SymbolicVariable = TargetSubTerm.SymbolPowerTerm
                                };
                        else
                            SourceTerm.FusedSymbols[TargetSubTerm.Symbol] += TargetSubTerm.SymbolPower;

                        // however primary symbol in source still the same so we need to add it to the value in target
                        //    (if exist in fused variables in target)

                        // also 

                        // there are still some fused variables in source that weren't altered by the target fused symbols
                        // go through every symbol in fused symbols and add it to the source fused symbols
                        foreach (var tfs in TargetSubTerm.FusedSymbols)
                        {
                            if (SourceTerm.FusedSymbols.ContainsKey(tfs.Key))
                            {
                                // symbol exist so accumulate it
                                SourceTerm._FusedSymbols[tfs.Key] += tfs.Value;
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
                                            SourceTerm._SymbolPowerTerm += tfs.Value.SymbolicVariable;
                                        else
                                        {
                                            // sum the value in the numerical part to the value in symbolic part
                                            SourceTerm._SymbolPowerTerm = new SymbolicVariable(SourceTerm._SymbolPower.ToString(CultureInfo.InvariantCulture)) + tfs.Value.SymbolicVariable;
                                            // reset the value in numerical part
                                            SourceTerm._SymbolPower = 1;
                                        }
                                    }
                                    else
                                        SourceTerm._SymbolPower += tfs.Value.NumericalVariable;
                                }
                                else
                                {
                                    // 2 no matching at all which needs to add the symbol from target into the fused symbols in source.
                                    SourceTerm.FusedSymbols.Add(tfs.Key, tfs.Value);
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
                                NumericalVariable = TargetSubTerm.SymbolPower,
                                SymbolicVariable = TargetSubTerm.SymbolPowerTerm == null ? null : (SymbolicVariable)TargetSubTerm.SymbolPowerTerm.Clone()
                            });                            
                        

                        // But the primary symbol of source may exist in the target fused variables.

                        foreach (var fsv in TargetSubTerm.FusedSymbols)
                        {
                            if (SourceTerm.FusedSymbols.ContainsKey(fsv.Key))
                                SourceTerm.FusedSymbols[fsv.Key] += fsv.Value;
                            else
                            {
                                // 1- if symbol is the same as priamry source 
                                if (SourceTerm.Symbol.Equals(fsv.Key, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (fsv.Value.SymbolicVariable != null)
                                    {
                                        if (SourceTerm._SymbolPowerTerm != null)
                                            SourceTerm._SymbolPowerTerm += fsv.Value.SymbolicVariable;
                                        else
                                        {
                                            // sum the value in the numerical part to the value in symbolic part
                                            SourceTerm._SymbolPowerTerm = new SymbolicVariable(SourceTerm._SymbolPower.ToString(CultureInfo.InvariantCulture)) + fsv.Value.SymbolicVariable;
                                            // reset the value in numerical part
                                            SourceTerm._SymbolPower = 1;
                                        }
                                    }
                                    else
                                        SourceTerm._SymbolPower += fsv.Value.NumericalVariable;

                                }
                                else
                                {
                                    SourceTerm.FusedSymbols.Add(fsv.Key, fsv.Value);
                                }
                            }
                        }
                        #endregion

                    
                    }

                    #endregion
                }

                MultiplyCoeffecients(ref SourceTerm, TargetSubTerm);

                #endregion
            }

            //here is a code to continue with other parts of a when multiplying them
            if (SourceTerm.AddedTerms.Count > 0)
            {
                Dictionary<string, SymbolicVariable> newAddedVariables = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                foreach (var vv in SourceTerm.AddedTerms)
                {
                    var newv = Multiply(vv.Value, TargetSubTerm);

                    newAddedVariables.Add(newv.SymbolBaseValue, newv);
                }
                SourceTerm._AddedTerms = newAddedVariables;
            }

            // now source term which is the first parameter cloned, have the new calculated value.
            int subIndex = 0;
            SymbolicVariable total = SourceTerm;

            while (subIndex < b.AddedTerms.Count)
            {
                // we should multiply other parts also 
                // then add it to the current instance

                // there are still terms to be consumed 
                //   this new term is a sub term in b and will be added to all terms of a.
                TargetSubTerm = b.AddedTerms.ElementAt(subIndex).Value;

                var TargetTermSubTotal = Multiply(a, TargetSubTerm);
                total = Add(total, TargetTermSubTotal);

                subIndex = subIndex + 1;  //increase 
            }

            
            AdjustZeroPowerTerms(total);

            AdjustZeroCoeffecientTerms(ref total);

            return total;
        }

        public struct CoeffecienttValue
        {
            public double ConstantValue;
            public SymbolicVariable ConstantPower;
            
        }

        public CoeffecienttValue[] Constants
        {
            get
            {
                var primary = new CoeffecienttValue
                {
                    ConstantValue = this.Coeffecient,
                    ConstantPower = this.CoeffecientPowerTerm
                };

                CoeffecienttValue[] cvs = new CoeffecienttValue[this.FusedConstants.Count + 1];
                cvs[0] = primary;

                for (int i = 0; i < this.FusedConstants.Count; i++)
                {
                    var hb = FusedConstants.ElementAt(i);
                    cvs[i + 1] = new CoeffecienttValue
                    {
                        
                        ConstantValue = hb.Key,
                        ConstantPower = hb.Value.SymbolicVariable
                    };
                }

                return cvs;
            }
        }

        /// <summary>
        /// Multiply Coeffecients of second argument into first argument.
        /// </summary>
        /// <param name="SourceTerm"></param>
        /// <param name="TargetSubTerm"></param>
        private static void MultiplyCoeffecients(ref SymbolicVariable SourceTerm, SymbolicVariable TargetSubTerm)
        {
            // Note: I will try to avoid the primary coeffecient so it doesn't hold power
            //      and only hold coeffecient itself.
            foreach (var cst in TargetSubTerm.Constants)
            {
                
                if (SourceTerm._CoeffecientPowerTerm == null && cst.ConstantPower == null)
                {
                    SourceTerm.Coeffecient *= cst.ConstantValue;
                }
                // there is a coeffecient power term needs to be injected into the source
                else
                {
                    // no the coeffecient part is not empty so we will test if bases are equal
                    // then make use of the fusedsymbols to add our constant

                    if (SourceTerm.Coeffecient == cst.ConstantValue)
                    {
                        // sample: 2^x*2^y = 2^(x+y)
                        SourceTerm._CoeffecientPowerTerm += cst.ConstantPower;
                    }
                    else
                    {
                        // sample: 2^(x+y)*log(2)*3^y * 3^z   can't be summed.
                        HybridVariable SameCoeffecient;
                        if (SourceTerm.FusedConstants.TryGetValue(cst.ConstantValue, out SameCoeffecient))
                        {
                            SameCoeffecient.SymbolicVariable += cst.ConstantPower;
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
                                        NumericalVariable = 1, // power
                                        SymbolicVariable = (SymbolicVariable)cst.ConstantPower.Clone()
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
                                        SourceTerm._CoeffecientPowerTerm += new SymbolicVariable(ower.ToString());
                                    }
                                    else
                                    {
                                        var rr = SourceTerm.FusedConstants[SucceededConstant.Value];

                                        rr.SymbolicVariable += new SymbolicVariable(ower.ToString());
                                        SourceTerm.FusedConstants[SucceededConstant.Value] = rr;
                                    }

                                }
                                else
                                {
                                    SourceTerm.FusedConstants.Add(
                                        cst.ConstantValue,
                                        new HybridVariable
                                        {
                                            NumericalVariable = 1, // power
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
