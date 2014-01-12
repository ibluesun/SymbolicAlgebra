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
        /// Converts the -ve trigonometric functions into corresponding +ve equivalent.
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        public static SymbolicVariable AlterNegativeSquareTrig(SymbolicVariable sv)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Simplify Trigonometric Functions
        /// </summary>
        /// <param name="sv"></param>
        public static SymbolicVariable TrigSimplify(SymbolicVariable sv)
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

            Action<SymbolicVariable> Loop2 = (term) =>
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

    }
}