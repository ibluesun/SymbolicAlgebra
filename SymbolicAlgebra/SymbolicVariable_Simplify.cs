using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;


namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
    {

        class ParameterSquareTrig
        {
            public string Parameter;
            public int Sin_2_Count;
            public int Cos_2_Count;


            SymbolicVariable _NegativeSimplifyValue;
            public SymbolicVariable NegativeSimplifyValue
            {
                get
                {
                    if (_NegativeSimplifyValue == null)
                    {
                        string value = string.Format("-Sin({0})^2-Cos({0})^2+1", Parameter);
                        _NegativeSimplifyValue = SymbolicVariable.Parse(value);
                    }
                    return _NegativeSimplifyValue;
                }
            }
        }


        /// <summary>
        /// Simplify Trigonometric Functions
        /// </summary>
        /// <param name="sv"></param>
        public static SymbolicVariable PartialTrigSimplify(SymbolicVariable sv)
        {
            // please refer to TrigSimplification.txt for the algorithm

            Dictionary<string, ParameterSquareTrig> PSTS = new Dictionary<string, ParameterSquareTrig>();

            Func<string, ParameterSquareTrig> PST = (nm) =>
                {
                    ParameterSquareTrig p;
                    if (!PSTS.TryGetValue(nm, out p))
                    {
                        p = new ParameterSquareTrig { Parameter = nm };
                        PSTS.Add(nm, p);
                        return p;
                    }
                    else
                    {
                        return p;
                    }
                };

            Action<SymbolicVariable> Loop2 = (termPart) =>
                {
                    // first test the term then test its fused variables
                    var TermsToBeTested = termPart.GetMultipliedTerms();

                    foreach (var term in TermsToBeTested)
                    {
                        if (term.IsFunction && term._SymbolPower == 2 && term.RawFunctionParameters.Length == 1)
                        {
                            if (term.FunctionName.Equals("Sin", StringComparison.OrdinalIgnoreCase))
                            {
                                PST(term.RawFunctionParameters[0]).Sin_2_Count++;
                            }
                            else if (term.FunctionName.Equals("Cos", StringComparison.OrdinalIgnoreCase))
                            {
                                PST(term.RawFunctionParameters[0]).Cos_2_Count++;
                            }
                            else
                            {
                                // not Trigonometric function
                            }
                        }
                    }
                };

            Loop2(sv);
            if (sv._AddedTerms != null) foreach (var term in sv._AddedTerms.Values) Loop2(term);


            // coduct the third operation .. final calulcation
            SymbolicVariable SimplifiedResult = sv.Clone();
            

            foreach (var pst in PSTS.Values)
            {
                while (pst.Cos_2_Count > 0 && pst.Sin_2_Count > 0)
                {
                    SimplifiedResult = SymbolicVariable.Add(SimplifiedResult, pst.NegativeSimplifyValue);

                    pst.Cos_2_Count--;
                    pst.Sin_2_Count--;

                }
            }


            return SimplifiedResult;
        }



        public static SymbolicVariable TrigSimplify(SymbolicVariable sv)
        {
            var factorized = FactorWithCommonFactor(sv);

            SymbolicVariable total = SymbolicVariable.Zero.Clone();

            // facorized expressions contain their terms in the multiplied symbols
            for (int i=0; i<factorized.TermsCount; i++)
            {
                // go through each term 
                var term = factorized[i];

                for (int fui=0;fui <term.FusedSymbols.Count;fui++)
                {
                    var ss = term.GetFusedTerm(fui);
                    var sp = ss;
                    if(!ss.IsOneTerm)
                    {
                        sp = PartialTrigSimplify(ss);
                        if (sp.IsOne)
                        {
                            // remove this fused symbol from the structure
                            term._FusedSymbols.Remove(term.GetFusedKey(fui));
                        }
                        else
                        {
                                                        
                            // replace with the new value.
                            term._FusedSymbols.Add(sp.ToString(), term._FusedSymbols[term.GetFusedKey(fui)]);

                            term._FusedSymbols.Remove(term.GetFusedKey(fui));

                        }
                    }
                }

                total = total + term;
            }

            // so begin in the factorized 
            var tsimp = PartialTrigSimplify(total);

            return tsimp;
        }

        /*
         * 
         * Expression like   a*sin(x)+a*cos(x)  can be written as follow a*(sin(x)+cos(x))  
         * i want to know the common factor that can be multiplied in these terms
         * 
         * so the common factor in my case is a variable key in fused terms or whatever
         * 
         * the result should be factor and its existence in indices in the symblic variable added terms
         * 
         * in case of previous example          [a..>0,1]
         * 
         * and the result should be the common factor as symbolic variable
         * multiplied term
         * terms that cannot be factorized .. to be added later 
         * 
         */

        /// <summary>
        /// Returns a map of occurence of variable name in the terms of the symbolic variable expression
        /// </summary>
        /// <returns></returns>
        public Dictionary<SymbolicVariable, List<int>> GetCommonFactorsMap()
        {
            // a*sin(x)+a*cos(x) + c
            // loop through all terms 
            //   begin with primary term 
            //     see if it exists in added terms in primary or fusedsymbols
            // build a dictionary with the existence of this variable
            // repeat this for each fused variable
            // the dictionary will increase in its data
            //  once we are done       check the dictionary lenghs for each variable .. the one with more existence of 2 or more is what we need
            // begin an analysis for the existence indices 
            //   a =>  2,5,9
            //   b =>  2,3,4
            // then a*b => 2   and etc.

            List<SymbolicVariable[]> bo2bo2 = new List<SymbolicVariable[]>();

            bo2bo2.Add(this.GetMultipliedTerms());
            foreach (var aterm in _AddedTerms)
                bo2bo2.Add(aterm.Value.GetMultipliedTerms());

            // now i have the matrix of multiplied terms.
            /*
             * a*sin(x)+a*cos(x) + c
             * 
             * 0           1          2
             * -------------------------
             * a           a          c
             * sin(x)     cos(x)
             * 
             * 
             * a in columns 0, 1
             * 
             */

            // take each column components and compare to next columns components
            Dictionary<SymbolicVariable, List<int>> CommonFactorsMap = new Dictionary<SymbolicVariable, List<int>>();
            for (int column = 0; column < bo2bo2.Count; column++)
            {
                var components = bo2bo2[column];


                for (int ix = 0; ix < components.Length; ix++)
                {
                    // get the component.
                    SymbolicVariable src_component = components[ix];

                    // once we this component we should write its existence in the list
                    List<int> indices;
                    if (!CommonFactorsMap.TryGetValue(src_component, out indices))
                    {
                        // new creation of the list
                        indices = new List<int>();
                        indices.Add(column);
                        CommonFactorsMap.Add(src_component, indices);
                    }


                    for (int nxc = column + 1; nxc < bo2bo2.Count ; nxc++)
                    {
                        var targetColumn = bo2bo2[nxc];

                        // compare it to the next column components

                        for (int nx_i = 0; nx_i < targetColumn.Length; nx_i++)
                        {
                            var trg_nx_compnent = targetColumn[nx_i];
                            if (src_component.Equals(trg_nx_compnent))
                            {
                                // bingo we found a match   .. write this information
                                if(!indices.Contains(nxc)) indices.Add(nxc);
                            }
                        }
                    }
                }
            }


            return CommonFactorsMap;
        }


        /// <summary>
        /// Factor the expression.
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        public static SymbolicVariable FactorWithCommonFactor(SymbolicVariable sv)
        {
            var map = sv.GetCommonFactorsMap();

            
            // get all keys  sorted by number of elements

            var keys = (from mk in map
                       orderby mk.Value.Count descending
                       where mk.Key.Equals(One) == false && mk.Value.Count > 1
                       select mk.Key).ToArray();



            if (keys.Length == 0) return sv.Clone();
            // ignore the 1 key
            
            // imagine a*x+a*y  result is a*(x+y)

            SymbolicVariable result = SymbolicVariable.One.Clone();
            
            var term_key = keys[0];  // the biggest values count   as shown above in the query statement

            var term_indices = map[term_key];

            List<SymbolicVariable> Common_Factors_Keys = new List<SymbolicVariable>();
            Common_Factors_Keys.Add(term_key);

            // find the other keys that has the same indices like this key
            for (int i = 1; i < keys.Length; i++)
            {
                var target_indices = map[keys[i]];
                if (target_indices.Count == term_indices.Count)
                {
                    // compare indices toghether
                    foreach (var idc in term_indices)
                    {
                        if (!target_indices.Contains(idc)) continue;
                    }

                    // reaching this line indicates that the two arrays contains the same indices

                    // so we save this key as another common factor for the expression
                    Common_Factors_Keys.Add(keys[i]);
                }

                if (target_indices.Count < term_indices.Count) break; // the array is ordered from higher count to low count so there is no point in comparing more results .. break the loop
            }

            //Common_Factors_Keys  now contains the factors that I will work on it.

            SymbolicVariable common_multipliers = SymbolicVariable.Zero.Clone();
            SymbolicVariable rest_multipliers = SymbolicVariable.Zero.Clone();

            for (int i = 0; i < sv.TermsCount; i++)
            {
                if (term_indices.Contains(i))
                {
                    // remove the common factor and add the result into the common_multipliers
                    var term = sv[i].Clone();
                    foreach (SymbolicVariable ke in Common_Factors_Keys)
                    {
                        term = SymbolicVariable.Divide(term, ke);   // divide by factor to remove it from the term.
                        /*
                        if (term.Symbol == ke.Symbol)
                        {
                            term.SymbolPower = 0;
                            AdjustZeroPowerTerms(term);
                        }
                        else
                        {
                            // remove from the fused symbols
                            term._FusedSymbols.Remove(ke.ToString());
                        }
                        */
                    }

                    common_multipliers += term;
                }
                else
                {
                    // doesn't contain the variable we want to remove so we store the term into the rest of the expressions to be added at the end of this procedure
                    rest_multipliers += sv[i].Clone();
                }
            }

            // at last alter the symbolic variable instant so that it contains the new shape
            foreach (var ke in Common_Factors_Keys)
            {
                result = result * ke;
            }

            result.FusedSymbols.Add(common_multipliers.ToString(), new HybridVariable { NumericalVariable=1.0 });

            result = result + rest_multipliers;

            return result;
        }
    }
}