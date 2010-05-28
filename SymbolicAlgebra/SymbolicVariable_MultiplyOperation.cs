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
        public static SymbolicVariable operator *(SymbolicVariable a, SymbolicVariable b)
        {
            if (a == null || b == null) return null;

            SymbolicVariable TargetSubTerm = (SymbolicVariable)b.Clone();

            TargetSubTerm._AddedTerms = null;   // remove added variables to prevent its repeated calculations in second passes
            // or to make sure nothing bad happens {my idiot design :S)

            int subIndex = 0;

            SymbolicVariable total = default(SymbolicVariable);

            SymbolicVariable SourceTerm = (SymbolicVariable)a.Clone();
            if (a.SymbolsEquals(TargetSubTerm))
            {
                #region symbols are equal (I mean 2*x^3 = 2*X^3)  
                SourceTerm.Coeffecient = SourceTerm.Coeffecient * TargetSubTerm.Coeffecient;
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
                #region symbols are different
                if (string.IsNullOrEmpty(SourceTerm.Symbol))
                {
                    // the instance have an empty primary variable so we should add it 
                    SourceTerm.Symbol = TargetSubTerm.Symbol;
                    SourceTerm.SymbolPower = TargetSubTerm.SymbolPower;

                    //fuse the fusedvariables in b into sv
                    foreach (var bfv in TargetSubTerm.FusedSymbols)
                    {
                        if (SourceTerm.FusedSymbols.ContainsKey(bfv.Key))
                            SourceTerm.FusedSymbols[bfv.Key] += bfv.Value;
                        else
                            SourceTerm.FusedSymbols.Add(bfv.Key, bfv.Value);

                    }
                }
                else
                {
                    if (SourceTerm.Symbol.Equals(TargetSubTerm.Symbol, StringComparison.OrdinalIgnoreCase))
                    {
                        #region primary symbol in both source and target exist and the same

                        // symbol of source match the symbol in target 
                        //   which will permit us to add the two powers  x^2 * x^3 = x^(2+3)

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

                            // also subtract the fused variables
                            foreach (var fv in TargetSubTerm.FusedSymbols)
                            {
                                SourceTerm.FusedSymbols[fv.Key] += TargetSubTerm.FusedSymbols[fv.Key];
                            }
                        }
                        #endregion
                    }
                    else if (SourceTerm.FusedSymbols.ContainsKey(TargetSubTerm.Symbol))
                    {
                        // the target symbol exist in the source fused variables
                        //   which means that source symbol exist in the target fused symbols

                        SourceTerm.FusedSymbols[TargetSubTerm.Symbol] += TargetSubTerm.SymbolPower; // fill the source symbol in fused variables.

                        // however primary symbol in source still the same so we need to add it to the value in target (if applicable)

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
                                    // 2 no matching at all which needs to add the symbol in target into the fused symbols in source.
                                    SourceTerm.FusedSymbols.Add(tfs.Key, tfs.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        SourceTerm.FusedSymbols.Add(TargetSubTerm.Symbol, new HybridVariable { NumericalVariable = TargetSubTerm.SymbolPower, SymbolicVariable= TargetSubTerm.SymbolPowerTerm });
                        foreach (var fsv in TargetSubTerm.FusedSymbols)
                            SourceTerm.FusedSymbols.Add(fsv.Key, fsv.Value);
                    }
                }

                SourceTerm.Coeffecient = a.Coeffecient * TargetSubTerm.Coeffecient;
                #endregion
            }


            //here is a code to continue with other parts of a when multiplying them
            if (SourceTerm.AddedTerms.Count > 0)
            {
                Dictionary<string, SymbolicVariable> newAddedVariables = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                foreach (var vv in SourceTerm.AddedTerms)
                {
                    var newv = vv.Value * TargetSubTerm;

                    newAddedVariables.Add(newv.SymbolBaseValue, newv);
                }
                SourceTerm._AddedTerms = newAddedVariables;

            }

        np:
            if (subIndex < b.AddedTerms.Count)
            {
                // we should multiply other parts also 
                // then add it to the current instance

                // there are still terms to be consumed 
                //   this new term is a sub term in b and will be added to all terms of a.
                TargetSubTerm = b.AddedTerms.ElementAt(subIndex).Value;

                if (total != null) total = total + (a * TargetSubTerm);
                else total = SourceTerm + (a * TargetSubTerm);

                subIndex = subIndex + 1;  //increase 
                goto np;
            }
            else
            {
                if (total == null) total = SourceTerm;
            }

            AdjustZeroPowerTerms(total);
            AdjustZeroCoeffecientTerms(total);

            return total;
        }

    }
}
