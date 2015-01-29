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

        /// <summary>
        /// The  number multiplied by the symbol  a*x^2  I mean {a}
        /// </summary>
        public double Coeffecient { private set; get; }

        /// <summary>
        /// ["function"]  group for name
        /// ["parameters"] group for the inner between brackets (*)  including any commas also.
        /// </summary>
        static Regex FunctionRegex = new Regex(@"^(?<function>[0-9a-zA-Z:]+)\((?<parameters>.*)\)$");


        private string _Symbol;

        /// <summary>
        /// the symbol name   a*x^2 I mean {x}
        /// Note: name will not contain any spaces
        /// </summary>
        public string Symbol 
        {
            private set
            {
                if (_BaseVariable == null)
                {
                    _Symbol = string.Empty; // remove trailing and begining spaces.
                    foreach (var vc in value)
                    {
                        if (!char.IsWhiteSpace(vc))
                            _Symbol += vc;
                    }
                }
                else
                {
                    throw new SymbolicException("Symbol is the base value of symbolic variable and cannot be changed.");
                }
            }
            get
            {
                if (_BaseVariable == null)
                {
                    // Default implentation
                    return _Symbol;
                }
                else
                {
                    return "(" + _BaseVariable.ToString() + ")";
                }
            }
        }


        /// <summary>
        /// This power term is only for the variable part of the instance.
        /// </summary>
        SymbolicVariable _SymbolPowerTerm;

        /// <summary>
        /// 2^(x+y)  (x+y) is the power term
        /// </summary>
        public SymbolicVariable SymbolPowerTerm
        {
            get
            {
                return _SymbolPowerTerm;
            }
        }




        private double _SymbolPower = 1;

        /// <summary>
        /// The symbolic power a*x^2  I mean {2}
        /// Although when the SymbolPowerTerm used this property will not included in calculations any more
        /// however it will indicate the current term sign and should be reset to 1 or -1
        ///     1 means +ve
        ///     -1 means -ve
        /// so it has a functionality after all
        /// </summary>
        public double SymbolPower 
        {
            private set
            {
                _SymbolPower = value;
            }
            get
            {
                return _SymbolPower;   
            }
        }

        /*
         * in the case of coeffecient symbol power will be fused directly to the coeffecient part
         * I mean 2*x to power two  will equal to 4*x^2 
         * however when raise to power y the result will be 2^y*x^y 
         * and the CoeffecientPowerTerm will appear.
         */

        private SymbolicVariable _CoeffecientPowerTerm;

        public SymbolicVariable CoeffecientPowerTerm
        {
            get
            {
                return _CoeffecientPowerTerm;
            }
        }

        public string CoeffecientPowerText
        {
            get
            {
                if (_CoeffecientPowerTerm != null)
                {
                    return _CoeffecientPowerTerm.ToString();
                }
                else
                {
                    // always return one 
                    return "1";
                }
            }
        }


        private bool? _IsFunction;
        private SymbolicVariable[] FunctionParameters;
        private string[] RawFunctionParameters;

        /// <summary>
        /// method for accessing the inner parameters if the symbol defined is function 
        /// I use it only for testing.
        /// </summary>
        public SymbolicVariable[] GetFunctionParameters()
        {
            return FunctionParameters;
        }
        
        string _FunctionName;

        public string FunctionName
        {
            get
            {
                return _FunctionName;
            }
        }

        


        /// <summary>
        /// The only constructor for the symbolic variable
        /// </summary>
        /// <param name="expression">Could be number, string, or function form</param>
        public SymbolicVariable(string token)
        {
            StringBuilder sb = new StringBuilder();
            int minuscount = 0;
            bool inStart = true;  //because i am testing at the begining of string.
            foreach (var t in token)
            {
                if (t != ' ')
                {
                    if (t == '-' && inStart==true) minuscount++;
                    else if (t == '+' && inStart == true)
                    {
                        /*ignore*/
                    }
                    else
                    {
                        sb.Append(t);
                        inStart = false;
                    }
                }
            }
            
            double rem = Math.IEEERemainder(minuscount, 2);

            string expression = sb.ToString();
            double coe;

            // try infinity first
            if (expression.Equals("Infinity", StringComparison.OrdinalIgnoreCase) || expression.Equals("inf", StringComparison.OrdinalIgnoreCase))
            {
                coe = double.PositiveInfinity;
                Symbol = string.Empty;
                if (rem != 0.0) Coeffecient = -1 * coe;
                else Coeffecient = coe;
                SymbolPower = 0;
            }
            // then try Not a Number NaN   second
            else if (expression.Equals("NaN", StringComparison.OrdinalIgnoreCase))
            {
                Symbol = string.Empty;
                Coeffecient = double.NaN;
                SymbolPower = 0;
            }
            //then the numbers at last :) :)
            else if (double.TryParse(expression, out coe))
            {
                Symbol = string.Empty;
                if (rem != 0.0) Coeffecient = -1 * coe;
                else Coeffecient = coe;
                SymbolPower = 0;
            }
            else
            {
                // not number then take the whole string as a symbol
                Symbol = expression;
                if (rem != 0.0) Coeffecient = -1;     // in case of minus before symbol  -x or -sin(x)
                else Coeffecient = 1;

                SymbolPower = 1;
            }

            // test the function form thing

            if (_IsFunction == null)
            {
                var m = FunctionRegex.Match(Symbol);

                if (m.Success)
                {
                    _IsFunction = true;
                    _FunctionName = m.Groups["function"].Value;

                    var parms = m.Groups["parameters"].Value;
                    if (!string.IsNullOrEmpty(parms))
                    {
                        RawFunctionParameters = TextTools.ComaSplit(parms);

                        FunctionParameters = new SymbolicVariable[RawFunctionParameters.Length];
                        for (int i = 0; i < RawFunctionParameters.Length; i++)
                        {
                            FunctionParameters[i] = Parse(RawFunctionParameters[i]);
                        }
                    }

                    // the following is the process of simplifieng known functions to its simplified form
                    if (FunctionParameters != null)
                    {
                        string[] oddfuncs = { "sin", "csc", "tan", "cot" };
                        string[] evenfuncs = { "cos", "sec" };
                        

                        // for some triogonmetric functions like sin  sin(-x) = -sin(x)  // this is important

                        // sin(-x) = -sin(x)
                        if (oddfuncs.Contains(_FunctionName, StringComparer.OrdinalIgnoreCase))
                        {
                            // 
                            if (FunctionParameters[0].IsNegative)
                            {
                                Coeffecient *= -1;
                                FunctionParameters[0].Coeffecient = FunctionParameters[0].Coeffecient * (-1);
                            }
                        }

                        // cos(-x) == cos(x)
                        if (evenfuncs.Contains(_FunctionName, StringComparer.OrdinalIgnoreCase))
                        {
                            // 
                            if (FunctionParameters[0].IsNegative)
                            {
                                FunctionParameters[0].Coeffecient = FunctionParameters[0].Coeffecient * (-1);
                            }
                        }

                        bool log = false;
                        if (_FunctionName.Equals(lnText, StringComparison.OrdinalIgnoreCase))
                        {
                            #region log simplification region
                            if (!FunctionParameters[0].IsMultiTerm)
                            {
                                if (FunctionParameters[0].IsOne)
                                {
                                    _Symbol = string.Empty;
                                    Coeffecient = 0;
                                    _SymbolPower = 0;
                                    FunctionParameters = null;
                                }

                                if (FunctionParameters != null)
                                {
                                    if (!string.IsNullOrEmpty(FunctionParameters[0].FunctionName))
                                    {
                                        if (FunctionParameters[0].FunctionName.Equals("exp", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // take the inner of exp and put it as a symbolic variable
                                            ReplaceCurrentValuesWith(FunctionParameters[0].FunctionParameters[0]);

                                            FunctionParameters = null;
                                        }
                                    }
                                }

                                if (FunctionParameters != null)
                                {
                                    if (FunctionParameters[0].FusedSymbols.Count > 0 || FunctionParameters[0].FusedConstants.Count > 0 ||
                                        (FunctionParameters[0].CoeffecientPowerTerm != null && FunctionParameters[0].SymbolPowerTerm != null) ||
                                        ((FunctionParameters[0].CoeffecientPowerTerm != null) && (FunctionParameters[0].SymbolPower > 1 || FunctionParameters[0].SymbolPower < 0)) ||
                                        (FunctionParameters[0].Coeffecient!=1 && !string.IsNullOrEmpty(FunctionParameters[0].Symbol) && FunctionParameters[0].SymbolPower!=0)

                                        )
                                    {
                                        #region log(x*y)
                                        // we have parameter on the form x*y or y/x or whatever
                                        // we take the power of each term and also fused constants 
                                        // and we prepare an expanded expression of term1_power*log(term1)+ term2_power*log(term2)+...

                                        var logparam = FunctionParameters[0];

                                        // Constants part
                                        var first_constant = new HybridVariable { NumericalVariable = 1, SymbolicVariable = logparam.CoeffecientPowerTerm };
                                        logparam._CoeffecientPowerTerm = null;

                                        HybridVariable[] fusedConstants = new HybridVariable[logparam.FusedConstants.Count];
                                        for (int i = 0; i < logparam.FusedConstants.Count; i++)
                                        {
                                            // get the power we need
                                            fusedConstants[i] = (HybridVariable)logparam.FusedConstants.ElementAt(i).Value.Clone();

                                            // make the poewr of fused variable one  {it means if it was 2^x or 2^2  now it will be 2^1
                                            logparam.FusedConstants[logparam.FusedConstants.ElementAt(i).Key] = new HybridVariable { NumericalVariable = 1 };
                                        }

                                        // symbols part
                                        var first_symbol = new HybridVariable { NumericalVariable = logparam.SymbolPower, SymbolicVariable = logparam.SymbolPowerTerm };
                                        logparam._SymbolPowerTerm = null;
                                        logparam.SymbolPower = 1;

                                        HybridVariable[] fusedPowers = new HybridVariable[logparam.FusedSymbols.Count];
                                        for (int i = 0; i < logparam.FusedSymbols.Count; i++)
                                        {
                                            // get the power we need
                                            fusedPowers[i] = (HybridVariable)logparam.FusedSymbols.ElementAt(i).Value.Clone();

                                            // make the poewr of fused variable one  {it means if it was y^x or y^2  now it will be y^1
                                            logparam.FusedSymbols[logparam.FusedSymbols.ElementAt(i).Key] = new HybridVariable { NumericalVariable = 1 };
                                        }

                                        SymbolicVariable final = null;

                                        // first coefficient power * log(coefficient)
                                        final = Multiply(first_constant, new SymbolicVariable("log(" + logparam.Coeffecient.ToString(CultureInfo.InvariantCulture) + ")"));
                                        for (int i = 0; i < logparam.FusedConstants.Count; i++)
                                        {
                                            final = Add(final, Multiply(fusedConstants[i], new SymbolicVariable("log(" + logparam.FusedConstants.ElementAt(i).Key.ToString(CultureInfo.InvariantCulture) + ")")));
                                        }

                                        // continue with symbols
                                        final = Add(final, Multiply(first_symbol, new SymbolicVariable("log(" + logparam.Symbol + ")")));
                                        for (int i = 0; i < logparam.FusedSymbols.Count; i++)
                                        {
                                            final = Add(final, Multiply(fusedPowers[i], new SymbolicVariable("log(" + logparam.FusedSymbols.ElementAt(i).Key + ")")));
                                        }

                                        ReplaceCurrentValuesWith(final);
                                        #endregion


                                    }
                                    else
                                    {
                                        if (FunctionParameters[0].SymbolPowerTerm != null || FunctionParameters[0].SymbolPower > 1 || FunctionParameters[0].SymbolPower < 0)
                                        {
                                            // like log(y^x) for example

                                            // get the power term
                                            var spt = new HybridVariable { NumericalVariable = FunctionParameters[0].SymbolPower, SymbolicVariable = FunctionParameters[0].SymbolPowerTerm };


                                            // make the log(parameter without its power)
                                            FunctionParameters[0]._SymbolPowerTerm = null;
                                            FunctionParameters[0].SymbolPower = 1;

                                            string pwop = "log(" + FunctionParameters[0] + ")";
                                            FunctionParameters = null;
                                            var t = new SymbolicVariable(pwop);

                                            // replace the primary symbol with the information of the power in parameter
                                            var rs = Multiply(spt, t);
                                            ReplaceCurrentValuesWith(rs);
                                        }
                                        else if (FunctionParameters[0].CoeffecientPowerTerm != null)
                                        {
                                            // like log(4^x)

                                            // get the power term
                                            var spt = FunctionParameters[0].CoeffecientPowerTerm;

                                            // make the log(parameter without its power)
                                            FunctionParameters[0]._CoeffecientPowerTerm = null;
                                            string pwop = "log(" + FunctionParameters[0] + ")";
                                            FunctionParameters = null;
                                            var t = new SymbolicVariable(pwop);

                                            // replace the primary symbol with the information of the power in parameter
                                            var rs = Multiply(spt, t);
                                            ReplaceCurrentValuesWith(rs);
                                        }
                                        else
                                        {
                                            //nothing
                                            Symbol = _FunctionName + "(" + FunctionParameters[0].ToString() +")";
                                        }
                                    }
                                }
                            }

                            log = true;
                            #endregion
                        }
                        if (log)
                        {
                        }
                        else 
                        {
                            Symbol = _FunctionName + "(";
                            for (int i = 0; i < FunctionParameters.Length; i++)
                            {
                                Symbol += FunctionParameters[i].ToString() + ",";
                            }

                            if (Symbol.EndsWith(",")) Symbol = Symbol.TrimEnd(',');
                            Symbol += ")";
                        }
                    }
                }
                else
                {
                    _IsFunction = false;
                }
            }


        }

        /// <summary>
        /// Replace the current instance with all values from the parameter.
        /// Note: this is not cloning, but it is taking the whole content into the current instance
        /// </summary>
        /// <param name="spt"></param>
        void ReplaceCurrentValuesWith(SymbolicVariable spt)
        {
            _Symbol = spt.Symbol;
            Coeffecient = spt.Coeffecient;
            _CoeffecientPowerTerm = spt.CoeffecientPowerTerm;
            _FusedConstants = spt.FusedConstants;
            _SymbolPower = spt.SymbolPower;
            _SymbolPowerTerm = spt.SymbolPowerTerm;
            _AddedTerms = spt.AddedTerms;
            _ExtraTerms = spt.ExtraTerms;
            _DividedTerm = spt.DividedTerm;
            _FusedSymbols = spt.FusedSymbols;
        }

        /// <summary>
        /// Change the function name of this symbolic variable instance.
        /// </summary>
        /// <param name="functionNames"></param>
        private void SetFunctionName(string[] functionNames)
        {
            string f1 = functionNames[0];

            Symbol = f1 + "(";

            string iparms = string.Empty;

            foreach (var p in FunctionParameters)
            {
                Symbol += p.ToString() + ",";
                iparms += p.ToString() + ",";

            }

            if (Symbol.EndsWith(",")) 
            {
                Symbol = Symbol.TrimEnd(',');
                iparms = iparms.TrimEnd(',');

            }

            Symbol += ")";

            // use the fusedsymbols to add the extra functions.
            if (functionNames.Length > 1)
            {
                for (int i = 1; i < functionNames.Length; i++)
                {
                    if (this.Symbol.Equals(functionNames[i] + "(" + iparms + ")", StringComparison.OrdinalIgnoreCase))
                    {
                        this.SymbolPower += 1;
                    }
                    else
                    {
                        if (FusedSymbols.ContainsKey(functionNames[i] + "(" + iparms + ")"))
                        {
                            var pp = FusedSymbols[functionNames[i] + "(" + iparms + ")"];
                            pp.NumericalVariable += 1;
                            FusedSymbols[functionNames[i] + "(" + iparms + ")"] = pp;
                        }
                        else
                        {
                            HybridVariable hfv = new HybridVariable();
                            hfv.NumericalVariable = 1.0;

                            FusedSymbols.Add(functionNames[i] + "(" + iparms + ")", hfv);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// When using this field, the symbol property will be altered to use it.
        /// </summary>
        SymbolicVariable _BaseVariable;
        public SymbolicVariable BaseVariable
        {
            get
            {
                return _BaseVariable;
            }
            internal set
            {
                _BaseVariable = value;
            }
        }

        /// <summary>
        /// Create a symbolic variable with another symbolic variable as a base.
        /// </summary>
        /// <param name="baseVariable"></param>
        public SymbolicVariable(SymbolicVariable baseVariable)
        {
            _BaseVariable = baseVariable;
            Coeffecient = 1;
            SymbolPower = 1;
        }



        /* 
         * when adding or subtracting symbolic variables that are not the same variable like the current 
         * I would like to store these variables inside the current instance.
         * the storage will be in positive operation array and negative operation array
         * multiplication and derivation will make another symbolic variable.
         * 
         */


        /// <summary>
        /// Terms that can't be fused into the instance a*x^2 + b*y^3  I mean {b*y^3}
        /// </summary>
        private Dictionary<string, SymbolicVariable> _AddedTerms;


        /*
         * 
         * When adding or subtracting terms that has different denominator then we need to store this terms as an extended terms
         */

        private List<ExtraTerm> _ExtraTerms;


        /// <summary>
        /// Terms that couldn't be divide on this term like  x/(x^2-y^5)
        /// </summary>
        SymbolicVariable _DividedTerm;


        /*
         in _FusedSymbols  the string key contains the mathmatical expression
          while the value of HybridVariable type contains its power  wether it was numerical or symbolic
         
         so calculation is done on the string part
         the string key part can be function or normal variable.
         
         */

        /// <summary>
        /// a*x^2*y^3*z^7  I mean {y^3, and z^7}
        /// </summary>
        private Dictionary<string, HybridVariable> _FusedSymbols;

        /// <summary>
        /// 2^x*4^t*5^y   I mean {4^t, 5^y}
        /// </summary>
        private Dictionary<double, HybridVariable> _FusedConstants;

        /// <summary>
        /// Added terms that couldn't be added to the current term (symbol) and share the same Divided Term.
        /// </summary>
        public Dictionary<string, SymbolicVariable> AddedTerms
        {
            get
            {
                if (_AddedTerms == null) _AddedTerms = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                return _AddedTerms;
            }
        }


        /// <summary>
        /// Extra Terms that doesn't share the same denominator  (take care that 1/x + 1/y share the same denominator because the symbol power here is -1)
        /// so representaion of (1/y+1/x) = y^-1+x^-1
        /// however 1/x+1/(x+Y)  doesn't share the same denominator so that ExtraTerms now contain 1/(x+y)
        /// </summary>
        public List<ExtraTerm> ExtraTerms
        {
            get
            {
                if (_ExtraTerms == null) _ExtraTerms = new List<ExtraTerm>();
                return _ExtraTerms;
            }
        }

        
        /// <summary>
        /// Extra terms that couldn't be divided into the term.
        /// </summary>
        public SymbolicVariable DividedTerm
        {
            get
            {
                if (_DividedTerm == null) _DividedTerm = new SymbolicVariable("1");
                return _DividedTerm;
            }
            internal set
            {
                _DividedTerm = value;
            }
        }

        /// <summary>
        /// Gets the current variable without divided term part and without extra terms.
        /// </summary>
        public SymbolicVariable Numerator
        {
            get
            {
                var h = this.Clone(true);
                h._DividedTerm = null;
                return h;
            }
        }

        public SymbolicVariable Denominator
        {
            get
            {
                return DividedTerm;
            }
        }

        /// <summary>
        /// Multiplied terms in the term other that original symbol letter.
        /// </summary>
        internal Dictionary<string, HybridVariable> FusedSymbols
        {
            get
            {
                if (_FusedSymbols == null) _FusedSymbols = new Dictionary<string, HybridVariable>(StringComparer.OrdinalIgnoreCase);
                return _FusedSymbols;
            }
        }

        /// <summary>
        /// Multiplied terms in the term other than the original coeffecient
        /// </summary>
        internal Dictionary<double, HybridVariable> FusedConstants
        {
            get
            {
                if (_FusedConstants == null) _FusedConstants = new Dictionary<double, HybridVariable>();
                return _FusedConstants;
            }
        }


        #region Fused Constants Functions
        private string GetConstantBaseValue(int i)
        {

            string result = string.Empty;

            double power = FusedConstants.ElementAt(i).Value.NumericalVariable;
            SymbolicVariable powerTerm = FusedConstants.ElementAt(i).Value.SymbolicVariable;

            string variableName = FusedConstants.ElementAt(i).Key.ToString(CultureInfo.InvariantCulture);
            if (powerTerm != null)
            {
                string powerTermText = powerTerm.ToString();
                if (powerTermText.Length > 1)
                    result = variableName + "^(" + powerTermText + ")";
                else
                    result = variableName + "^" + powerTermText;
            }
            else if (power != 0)
            {
                // variable exist 
                if (power != 1) result = variableName + "^" + power.ToString(CultureInfo.InvariantCulture);
                else result = variableName;
            }

            return result;
        }

        private string GetConstantAbsoluteBaseValue(int i)
        {

            string result = string.Empty;

            double power = FusedConstants.ElementAt(i).Value.NumericalVariable;
            SymbolicVariable powerTerm = FusedConstants.ElementAt(i).Value.SymbolicVariable;

            string variableName = FusedConstants.ElementAt(i).Key.ToString(CultureInfo.InvariantCulture);

            if (powerTerm != null)
            {
                string powerTermText = powerTerm.IsNegative ? SymbolicVariable.Multiply(NegativeOne, powerTerm).ToString() : powerTerm.ToString();
                if (powerTermText.Length > 1)
                    result = variableName + "^(" + powerTermText + ")";
                else
                    result = variableName + "^" + powerTermText;
            }
            else if (power != 0)
            {
                // variable exist 
                if (Math.Abs(power) != 1) result = variableName + "^" + Math.Abs(power).ToString(CultureInfo.InvariantCulture);
                else result = variableName;
            }

            return result;
        }
        private double GetFusedConstantPower(int i)
        {
            return FusedConstants.ElementAt(i).Value.NumericalVariable;
        }

        

        #endregion

        #region Fused Symbols Functions

        /// <summary>
        /// from a*x^2  I mean {x^2} or x^-2
        /// </summary>
        private string GetSymbolBaseValue(int i)
        {
            
            string result = string.Empty;
            
            double power = FusedSymbols.ElementAt(i).Value.NumericalVariable;
            SymbolicVariable powerTerm = FusedSymbols.ElementAt(i).Value.SymbolicVariable;

            string variableName = FusedSymbols.ElementAt(i).Key;
            if (powerTerm != null)
            {
                string powerTermText = powerTerm.ToString();
                if (powerTermText.Length > 1)
                    result = variableName + "^(" + powerTermText + ")";
                else
                    result = variableName + "^" + powerTermText;
            }
            else if (power != 0)
            {
                // variable exist 
                if (power != 1) result = variableName + "^" + power.ToString(CultureInfo.InvariantCulture);
                else
                {
                    if (TextTools.TopLevelExist(variableName, '+') == true || TextTools.TopLevelExist(variableName, '-') == true)
                        result = "(" + variableName + ")";
                    else
                        result = variableName;
                }
            }
            
            return result;    
        }


        /// <summary>
        /// from a*x^-2-y^-3  will return y^3  but from fused variables
        /// used internally.
        /// </summary>
        private string GetSymbolAbsoluteBaseValue(int i)
        {

            string result = string.Empty;

            double power = FusedSymbols.ElementAt(i).Value.NumericalVariable;
            SymbolicVariable powerTerm = FusedSymbols.ElementAt(i).Value.SymbolicVariable;

            string variableName = FusedSymbols.ElementAt(i).Key;

            if (powerTerm != null)
            {
                string powerTermText = powerTerm.IsNegative ? SymbolicVariable.Multiply(NegativeOne, powerTerm).ToString() : powerTerm.ToString();

                if (powerTermText.Length > 1)
                    result = variableName + "^(" + powerTermText + ")";
                else
                    result = variableName + "^" + powerTermText;
            }
            else if (power != 0)
            {
                // variable exist 
                if (Math.Abs(power) != 1) result = variableName + "^" + Math.Abs(power).ToString(CultureInfo.InvariantCulture);
                else result = variableName;
            }

            return result;    
        }

        /// <summary>
        /// returns the power of the fused variables.
        /// used internally.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private double GetFusedPower(int i)
        {
            return FusedSymbols.ElementAt(i).Value.NumericalVariable;
        }



        public string GetFusedKey(int i)
        {
            var fs = FusedSymbols.ElementAt(i);

            return fs.Key;
        }

        /// <summary>
        /// Gets a fused term as symbolic variable with its power
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public SymbolicVariable GetFusedTerm(int i)
        {
            var fs = FusedSymbols.ElementAt(i);
            SymbolicVariable ft = SymbolicVariable.Parse(fs.Key);

            if (fs.Value.SymbolicVariable != null) return ft.RaiseToSymbolicPower(fs.Value.SymbolicVariable);
            else return ft.Power(fs.Value.NumericalVariable);
        }

        /// <summary>
        /// Gets fused constants as symbolic variables with their powers
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public SymbolicVariable GetConstantTerm(int i)
        {
            var fs = FusedConstants.ElementAt(i);
            SymbolicVariable ft = new SymbolicVariable(fs.Key.ToString());

            if (fs.Value.SymbolicVariable != null) return ft.RaiseToSymbolicPower(fs.Value.SymbolicVariable);
            else return ft.Power(fs.Value.NumericalVariable);
        }




        /// <summary>
        /// gets the current term without any additional fused or added terms 
        /// </summary>
        /// <returns></returns>
        public SymbolicVariable GetTheStrippedVariable()
        {
            var stripped = new SymbolicVariable(this.Symbol);

            stripped._SymbolPower = this._SymbolPower;

            if (this._SymbolPowerTerm != null)
                stripped._SymbolPowerTerm = this._SymbolPowerTerm.Clone();


            return stripped;
        }

        /// <summary>
        /// gets the coefficient in its own instance.
        /// </summary>
        /// <returns></returns>
        public SymbolicVariable GetTheStrippedCoefficient()
        {
            var stripped =  new SymbolicVariable(Coeffecient.ToString());

            if (this._CoeffecientPowerTerm != null)
                stripped._CoeffecientPowerTerm = this._CoeffecientPowerTerm.Clone();

            return stripped;

        }

        /// <summary>
        /// gets this term multiplied variables but as a list of variables
        /// </summary>
        /// <returns></returns>
        public SymbolicVariable[] GetMultipliedTerms()
        {
            List<SymbolicVariable> MultilpiedTerms = new List<SymbolicVariable>();

            MultilpiedTerms.Add(this.GetTheStrippedCoefficient());

            MultilpiedTerms.Add(this.GetTheStrippedVariable());

            for (int ifsx = 0; ifsx < this.FusedConstants.Count; ifsx++)
            {
                MultilpiedTerms.Add(this.GetConstantTerm(ifsx));
            }

            for (int ifsx = 0; ifsx < this.FusedSymbols.Count; ifsx++)
            {
                MultilpiedTerms.Add(this.GetFusedTerm(ifsx));
            }


            return MultilpiedTerms.ToArray();
        }

        #endregion

        /// <summary>
        /// from a*x^2  I mean {x^2} 
        /// a^3*x^2  ===>  a^3|x^2
        /// used as a key in AddedTerms hashtable.
        /// </summary>
        public string WholeValueBaseKey
        {
            get
            {
                string result = string.Empty;

                
                // add the key of the coefficient only  when it has power term and coefficient itself is not zero.
                if (CoeffecientPowerTerm != null && Coeffecient != 0) result = Coeffecient.ToString(CultureInfo.InvariantCulture) + "^" + CoeffecientPowerTerm.WholeValueBaseKey + "|";

                if (_SymbolPowerTerm != null)
                {
                    if (_SymbolPowerTerm.IsZero)
                    {
                    }
                    else
                    {
                        result = Symbol + "^(" + SymbolPowerText + ")";
                    }
                }
                else
                {
                    if (SymbolPower != 0)
                    {
                        // variable exist 
                        if (SymbolPower != 1) result = Symbol + "^" + SymbolPower.ToString(CultureInfo.InvariantCulture);
                        else result = Symbol;
                    }
                }

                for (int i = 0; i < FusedSymbols.Count; i++)
                {

                    double pp = GetFusedPower(i);
                   
                    if(string.IsNullOrEmpty(result))
                    {
                        if (pp >= 0)
                            result += GetSymbolBaseValue(i);
                        else
                            result += "1/" + GetSymbolAbsoluteBaseValue(i);
                    }
                    else
                    {
                        if (pp >= 0)
                            result += "*" + GetSymbolBaseValue(i);
                        else
                            result += "/" + GetSymbolAbsoluteBaseValue(i);
                    }
                }

                return result;
            }
        }



        public string SymbolBaseKey
        {
            get
            {
                string g = WholeValueBaseKey;

                if (g.Contains("|")) return g.Substring(g.IndexOf('|') + 1);
                else return g;
                
            }
        }
        

        /// <summary>
        /// Gets the power part of this term
        /// return empty text when power = 0 or 1 or -1
        /// </summary>
        public string SymbolPowerText
        {
            get
            {
                if (_SymbolPowerTerm != null)
                {
                    return _SymbolPowerTerm.ToString();
                }
                else
                {
                    if (SymbolPower != 0)
                    {
                        // variable exist 
                        if (Math.Abs(SymbolPower) != 1)
                        {
                            return SymbolPower.ToString(CultureInfo.InvariantCulture);
                        }
                        else return string.Empty;
                    }
                    else return string.Empty;
                }
            }
        }

        /// <summary>
        /// for showing purposed only.
        /// not to be used in keys.
        /// used internally.
        /// Shows variables symbols with their powers
        /// </summary>
        private  string GetFormattedSymbolicValue()
        {
            
            string result = string.Empty;

            // first check the primary storage of the symbol and if it has value or noy

            if (!string.IsNullOrEmpty(Symbol))
            {
                // include the power of instance 
                if (string.IsNullOrEmpty(SymbolPowerText))
                {
                    if (_SymbolPower == 0)
                        result = "1";                  // x^0     will result in 1
                    else
                        result = Symbol;               //  x  the symbol itself
                }
                else
                {
                    // add parenthesis if the text more than one charachter

                    if (SymbolPower >= 0)
                    {
                        if (SymbolPowerText.Length > 1)
                            result = Symbol + "^(" + SymbolPowerText + ")";               // x^(power)
                        else
                            result = Symbol + "^" + SymbolPowerText;                      //  x^power
                    }
                    else
                    {
                        if (_SymbolPowerTerm == null)
                            result = Symbol + "^" + Math.Abs(SymbolPower).ToString(CultureInfo.InvariantCulture);    // x^power
                        else
                        {
                            if (SymbolPowerText.Length > 1)
                                result = Symbol + "^(" + SymbolPowerText + ")";                                   // x^(multi power term)
                            else
                                result = Symbol + "^" + SymbolPowerText;
                        }
                    }
                }
            }


            for (int i = 0; i < FusedSymbols.Count; i++)
            {

                double pp = GetFusedPower(i);         // fused symbol power

                if (string.IsNullOrEmpty(result))
                {
                    // if there were no powers in the main symbol storage  {this case shouldn't happen from calculation} because I always transfer any fused symbol inside the main storage
                    if (pp >= 0)
                        result += GetSymbolBaseValue(i);
                    else
                        result += "1/" + GetSymbolAbsoluteBaseValue(i);
                }
                else
                {
                    if (pp >= 0)
                        result += "*" + GetSymbolBaseValue(i) + "";
                    else
                    {
                        var sabv = GetSymbolAbsoluteBaseValue(i);
                        result += "/" + sabv;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Returns the whole symbolic variable   symobl part with coeffecient part
        /// </summary>
        private string FormSymbolTextValue()
        {
            string result = GetFormattedSymbolicValue();

            if (Coeffecient == 0) return "0";                 // this cancel all the symbolic variable because of 0 in coeffecient

            if (Coeffecient == 1)
            {
                if (string.IsNullOrEmpty(result)) result = "1";
                else
                {
                    if (SymbolPower < 0) result = "1/" + result;
                }
            }

            if (Coeffecient != 1)
            {
                string rr = Coeffecient.ToString(CultureInfo.InvariantCulture);
                if (_CoeffecientPowerTerm != null)
                {
                    // coeffecient part  like 3^x  {remember coeffecient may be raised to symbol}
                    if (CoeffecientPowerTerm.IsMultiValue || CoeffecientPowerTerm.IsMultiTerm)
                    {
                        rr = rr + "^(" + CoeffecientPowerText + ")";
                    }
                    else
                    {
                        rr = rr + "^" + CoeffecientPowerText;
                    }

                    if (!string.IsNullOrEmpty(result))
                        result = rr + "*" + result;
                    else
                        result = rr;
                }
                else
                {
                    // in case there are no power term.
                    if (SymbolPower != 0)
                    {
                        if (SymbolPower < 0)
                            result = rr + "/" + result;
                        else
                        {
                            if (rr == "-1")
                                result = "-" + result;
                            else
                                result = rr + "*" + result;
                        }
                    }
                    else
                    {
                        if (FusedSymbols.Count > 0)
                        {
                            result = rr + "*" +  result;
                        }
                        else
                        {
                            result = rr;
                        }
                    }
                }
            }

            if (FusedConstants.Count > 0) if (result == "1") result = string.Empty;
            
            // continue with the rest of constants
            for (int i = 0; i < FusedConstants.Count; i++)
            {
                double pp = GetFusedConstantPower(i);

                if (string.IsNullOrEmpty(result))
                {
                    if (pp >= 0)
                        result += GetConstantBaseValue(i);
                    else
                        result += "1/" + GetConstantAbsoluteBaseValue(i);
                }
                else
                {
                    if (pp >= 0)
                        result += "*" + GetConstantBaseValue(i);
                    else
                        result += "/" + GetConstantAbsoluteBaseValue(i);
                }
            }


            return result;
            
        }

        public string FinalText()
        {
            string result = FormSymbolTextValue();

            if (AddedTerms.Count > 0)
            {
                foreach (var sv in AddedTerms.Values)
                {
                    if (sv.Coeffecient != 0)
                    {
                        if (sv.FormSymbolTextValue().StartsWith("-"))
                            result += sv.FormSymbolTextValue();
                        else
                        {
                            result += "+" + sv.FormSymbolTextValue();

                        }
                        if (!sv.DividedTerm.IsOne) result += "/(" + sv.DividedTerm.FinalText() + ")";
                    }
                }
            }

            if (DividedTerm.IsOneTerm == false)
            {
                if (TermsCount > 1)
                    result = "(" + result + ")/(" + DividedTerm.ToString() + ")";
                else
                    result = result + "/(" + DividedTerm.ToString() + ")";
            }
            else if (DividedTerm.FormSymbolTextValue() != "1")
            {
                if (TermsCount > 1)
                    result = "(" + result + ")/(" + DividedTerm.ToString() + ")";
                else
                    result = result + "/(" + DividedTerm.ToString() + ")";
            }

            foreach (var eterm in ExtraTerms)
            {
                if (eterm.Negative)
                {
                    
                    result += "-" + eterm.Term.FinalText();
                }
                else
                {
                    string etv = eterm.Term.FinalText();
                    if(etv.StartsWith("-")) 
                        result+=etv;
                    else 
                        result += "+" + etv ;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the text of the whole instance terms.
        /// serve as the final point.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string final = FinalText();
            return final;
        }



        /// <summary>
        /// Returns the symbolic variable as separated terms that share the same divisor  (this doesn't include extra terms)
        /// </summary>
        /// <param name="ix"></param>
        /// <returns></returns>
        public SymbolicVariable this[int ix]
        {
            get
            {
                if (ix > 0)
                {
                    if (_AddedTerms == null) throw new IndexOutOfRangeException();
                    return _AddedTerms.Values.ElementAt(ix - 1);
                }
                else if (ix < 0) throw new IndexOutOfRangeException();
                else
                {
                    // ix ==== 0

                    var c = this.Clone();
                    c._AddedTerms = null;
                    return c;
                }
            }
        }

        /// <summary>
        /// Return number of terms in this instance.
        /// </summary>
        public int TermsCount
        {
            get
            {
                if (_AddedTerms == null) return 1;
                else
                    return 1 + _AddedTerms.Count;
            }
        }



        public static Dictionary<string, SymbolicVariable> _Functions;

        /// <summary>
        /// Contains a static dictionary of function names that may be used inside the context of differentiation
        /// on the form of "u(x)" := "x^2"
        /// </summary>
        public static Dictionary<string, SymbolicVariable> Functions
        {
            get
            {
                if (_Functions == null) _Functions = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                return _Functions;
            }
        }

        

        /// <summary>
        /// Compare the symbol of current instance to the symbol of parameter instance
        /// either in primary symbol part and in fused symbols.
        /// No compare occure on the added terms part.
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        public bool BaseEquals(SymbolicVariable sv)
        {

            if (!this.DividedTerm.Equals(sv.DividedTerm)) return false;

            if (this.FusedSymbols.Count > 0 || sv.FusedSymbols.Count > 0)
            {
                // the trick is to make sure that the same variables are exist in this instance and the target instance.
                //
                // Anatomy of symbolic value is VariableName  ==> hold the instance variable name
                //    Fused variable  for any extra multiplied variables 
                // The algorithm is to compare all variables names of this instance to the target instance
                // make a dictionary array 
                // put primary variable name with its power from this instance.
                //   put the primart variable name with its power (as negative) from target instance.
                // proceed with fuse variables in this and target instances  +ve  for this instance  -ve for target instance
                //  test every variable to see its zero value
                //  any item that has zero value will break the equality and we should return a false result.

                Dictionary<string, HybridVariable> vvs = new Dictionary<string, HybridVariable>(StringComparer.OrdinalIgnoreCase);

                //variable name of this instance    THE VARIABLE NAME which is x or y or xy  
                if (!string.IsNullOrEmpty(this.Symbol))
                {
                    HybridVariable hv = new HybridVariable { NumericalVariable = SymbolPower, SymbolicVariable = SymbolPowerTerm };
                    vvs.Add(this.Symbol, hv);
                }

                // variable part of the target instance
                if (!string.IsNullOrEmpty(sv.Symbol))
                {
                    HybridVariable hv = new HybridVariable { NumericalVariable = sv.SymbolPower, SymbolicVariable = sv.SymbolPowerTerm };

                    if (vvs.ContainsKey(sv.Symbol))
                    {
                        vvs[sv.Symbol] -= hv;
                    }
                    else
                    {
                        vvs.Add(sv.Symbol, hv * -1);
                    }
                }

                //fused variables of this instance
                foreach (var cv in this.FusedSymbols)
                {
                    if (vvs.ContainsKey(cv.Key)) vvs[cv.Key] += cv.Value;
                    else vvs.Add(cv.Key, cv.Value);
                }

                // fused variables of the target instance.
                foreach (var tv in sv.FusedSymbols)
                {
                    if (vvs.ContainsKey(tv.Key)) vvs[tv.Key] -= tv.Value;
                    else vvs.Add(tv.Key, tv.Value);
                }

                foreach (var rv in vvs)
                {
                    if (!rv.Value.IsZero) return false;
                }
                return true;
            }
            else
            {
                if (this.Symbol.Equals(sv.Symbol, StringComparison.OrdinalIgnoreCase) && this.DividedTerm.Equals( sv.DividedTerm))
                {
                    if (this.Symbol == string.Empty) return true;  //because there is no symbol or empty symbol which indicates that the term is coefficient only
                    else if (this._SymbolPowerTerm != null && sv._SymbolPowerTerm != null)
                    {
                        if (_SymbolPowerTerm.Equals(sv._SymbolPowerTerm)) return true;
                    }
                    else if (this.SymbolPower != sv.SymbolPower)
                    {
                        return false;
                    }
                    else
                    {
                        // one of the terms is  null or the both are numbers.

                        if (this.SymbolPowerText.Equals(sv.SymbolPowerText, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }

                }
            }
            return false;
        }


        /// <summary>
        /// Tells if this instance equal the other instance.
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        public bool Equals(SymbolicVariable sv)
        {
            if (this.IsMultiTerm && sv.IsMultiTerm)
            {
                // make sure added terms are equal in count.
                if (this._AddedTerms.Count == sv._AddedTerms.Count && this._AddedTerms.Count > 0)
                {
                    //ok there are another tests to be done.
                    int EqualTerms = 0;

                    int count = this.TermsCount;

                    for (int ix = 0; ix < count; ix++)
                    {
                        for (int iy = 0; iy < count; iy++)
                        {
                            if (this[ix].Equals(sv[iy])) EqualTerms++;
                        }
                    }

                    if (EqualTerms == count) return true;
                    else return false;   //whether it was more or less.
                }
            }   
            else
            {
                if (this.IsMultiTerm == true || sv.IsMultiTerm == true) return false;
                if (this.Symbol.Equals(sv.Symbol, StringComparison.OrdinalIgnoreCase))
                {
                    if (this.SymbolPower == sv.SymbolPower)
                    {
                        if (this.Coeffecient == sv.Coeffecient)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(SymbolicVariable))
            {
                var sv = (SymbolicVariable)obj;
                return Equals(sv);
            }
            else
            {
                return false;
            }
        }


        public override int GetHashCode()
        {
            return Coeffecient.GetHashCode() + Symbol.GetHashCode() + SymbolPower.GetHashCode();
        }


        /// <summary>
        /// This function is mainly taking the sqrt(x)^2 and product x  instead of it.
        /// </summary>
        /// <param name="sv"></param>
        private static void AdjustSpecialFunctions(ref SymbolicVariable sv)
        {
            // if you found sqrt(x)^2 get the parameter of the function

            var aterms = sv._AddedTerms;
            sv._AddedTerms = null;
            var eterms = sv._ExtraTerms;
            sv._ExtraTerms = null;

            if (sv.IsFunction && sv.FunctionName.Equals("sqrt", StringComparison.OrdinalIgnoreCase)&&sv.SymbolPower>1)
            {
                var parameterPower = 0.5 * sv.SymbolPower;
                
                var newvalue = sv.FunctionParameters[0].Power(parameterPower);
                newvalue.Coeffecient *= sv.Coeffecient;

                sv = newvalue;
                
            }

            if (aterms != null)
            {
                for (int i = 0; i < aterms.Count; i++)
                {
                    var sa = aterms.ElementAt(i).Value;

                    AdjustSpecialFunctions(ref sa);

                    sv.AddedTerms.Add(sa.WholeValueBaseKey,sa);
                }
                
            }

            if (eterms != null)
            {
                foreach (var r in eterms)
                {
                    AdjustSpecialFunctions(ref r.Term);
                    sv.ExtraTerms.Add(r);
                }
                
            }
        }

        private static void AdjustZeroPowerTerms(SymbolicVariable svar)
        {
            for (int i = svar.FusedSymbols.Count - 1; i >= 0; i--)
            {
                if (svar.FusedSymbols.ElementAt(i).Value.IsZero || svar.FusedSymbols.ElementAt(i).Key == string.Empty)
                    svar.FusedSymbols.Remove(svar.FusedSymbols.ElementAt(i).Key);
                
            }

            if (svar._SymbolPower == 0)
            {
                // symbol should be reset or replace with one variable from the fused variables.
                if (svar.FusedSymbols.Count > 0)
                {
                    // get the first iem that its key doesn't begin with number or -ve or +ve  because these keys is used for coeffecients constants that were fused
                    KeyValuePair<string, HybridVariable>  firstFused = svar.FusedSymbols.FirstOrDefault();

                    // if the returned value is not the default
                    svar._Symbol = firstFused.Key;
                    if (firstFused.Value.SymbolicVariable != null)
                    {
                        svar._SymbolPowerTerm = firstFused.Value.SymbolicVariable;
                    }
                    else
                    {
                        svar._SymbolPower = firstFused.Value.NumericalVariable;
                    }

                    //remove the first fused variable from the list.
                    svar.FusedSymbols.Remove(firstFused.Key);
                }
                else
                {
                    svar._Symbol = ""; //make sure to empty the symbol because its power is zero
                }
            }
            foreach (var r in svar.AddedTerms.Values) AdjustZeroPowerTerms(r);
        }


        /// <summary>
        /// Removes the terms with coeffiecient of zero.
        /// </summary>
        /// <param name="svar"></param>
        private static void AdjustZeroCoeffecientTerms(ref SymbolicVariable svar)
        {
            // check the added terms for zero coefficients and remove if necessary.
            for (int i = svar.AddedTerms.Count - 1; i >= 0; i--)
            {
                if (svar.AddedTerms.ElementAt(i).Value.Coeffecient == 0)
                    svar.AddedTerms.Remove (svar.AddedTerms.ElementAt(i).Key);
            }

            for (int i = svar.ExtraTerms.Count - 1; i >= 0; i--)
            {
                if (svar.ExtraTerms.ElementAt(i).Term.Coeffecient == 0)
                    svar.ExtraTerms.RemoveAt(i);
            }

            // then check the priamry term.
            if (svar.Coeffecient == 0)
            {
                svar.Symbol = string.Empty;
                if (svar.AddedTerms.Count > 0)
                {
                    // The AddedTerms are collection from one level like this
                    //  PrimaryTerm
                    //      |--  AddedTerms  1, 2, 3, .., n
                    //  so we need the first term to be the AddedTerm and rest of terms to be in the same collection

                    SymbolicVariable priamry = svar.AddedTerms.ElementAt(0).Value;
                    svar.AddedTerms.Remove(svar.AddedTerms.ElementAt(0).Key);

                    Dictionary<string, SymbolicVariable> NewAddedTerms = svar.AddedTerms;

                    priamry._AddedTerms = NewAddedTerms;

                    svar = priamry;
                }
                else if (svar.ExtraTerms.Count > 0)
                {
                    SymbolicVariable priamry = svar.ExtraTerms.ElementAt(0).Term;
                    if (svar.ExtraTerms[0].Negative) priamry.Coeffecient *= -1;
                    svar.ExtraTerms.RemoveAt(0);

                    var NewExtraTerms = svar.ExtraTerms;

                    priamry._ExtraTerms = NewExtraTerms;

                    svar = priamry;

                }
                else
                {
                    // nothing to replace
                    // the primary is zerooooooooo     big zeroooooooooooooooo
                }
            }
        }


        /// <summary>
        /// Test if the whole term equals zero or not.
        /// </summary>
        public bool IsZero
        {
            get
            {
                if (this.ToString() == "0") return true;
                
                return false;
            }
        }

        /// <summary>
        /// Test if the whole term equals One or not ==1.
        /// </summary>
        public bool IsOne
        {
            get
            {
                if (this.ToString() == "1") return true;

                return false;
            }
        }


        /// <summary>
        /// Test if -1
        /// </summary>
        public bool IsNegativeOne
        {
            get
            {
                if (this.ToString() == "-1") return true;

                return false;
            }
        }

        /// <summary>
        /// Tells if this instance is only one term.
        /// </summary>
        public bool IsOneTerm
        {
            get
            {
                if (_AddedTerms != null)
                {
                    if (_AddedTerms.Count > 0)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// tells if the this term is having coeffecient as number only.
        /// </summary>
        public bool IsCoeffecientOnly
        {
            get
            {
                if (string.IsNullOrEmpty(this.WholeValueBaseKey) && IsMultiTerm == false)
                {
                    return true;
                }
                else if (IsZero) return true;

                return false;
            }
        }

        /// <summary>
        /// True if the variable starts with '%'
        /// </summary>
        public bool IsConstant
        {
            get
            {
                if (Symbol.StartsWith("%")) return true;
                return false;
            }
        }

        public bool IsMultiTerm
        {
            get
            {
                if (AddedTerms.Count == 0 && ExtraTerms.Count == 0) return false;
                return true;
            }
        }

        /// <summary>
        /// Indicates that there are more than coeffecient
        /// i.e. Coeffecient and variable and or multi fused variables
        /// </summary>
        public bool IsMultiValue
        {
            get
            {
                if (Math.Abs(Coeffecient) != 1)
                {
                    if (!string.IsNullOrEmpty(Symbol)) return true;
                }

                return false;
            }
        }

        public bool IsNegative
        {
            get
            {
                if (this.Coeffecient < 0) return true;
                else return false;
            }
        }



        /// <summary>
        /// Match the term if it contains function on the form f() or e:f(y,x)
        /// </summary>
        public bool IsFunction
        {
            get
            {
                if (_IsFunction == null) _IsFunction = false;
                return _IsFunction.Value;
            }
        }


        /// <summary>
        /// returns all the symbols involved in this object
        /// </summary>
        public string[] InvolvedSymbols
        {
            get
            {
                Func<string, string[]> pgh = (key) =>
                    {
                        List<string> fs = new List<string> { key };
                        int i = 0;
                        while (i < fs.Count)
                        {
                            // this loop extract the inner parameters from the enclosed function calls as f(g(n(x)))
                            while (FunctionRegex.Match(fs[i]).Success)
                            {
                                fs[i] = FunctionRegex.Match(fs[i]).Groups["parameters"].Value;

                                var vps = TextTools.ExtractFunctionParameters(fs[i]);

                                if (vps.Length > 0)
                                    fs[i] = vps[0]; //first parameter
                                else 
                                    fs[i] = string.Empty;

                                // many parameters may be  u, e, sin(f)  etc..
                                // add the extra parameter to the list of discovered symbols
                                for (int vpi = 1; vpi < vps.Length; vpi++)
                                    fs.Add(vps[vpi]);

                                // we note here that the dynamic list of fs has been increased  and we 
                                // will conduct the test again of current fs[i] to see if it is a function or not.
                                // so we are adding undiscovered symbols to later passes when i is increasing after this loop.
                            }
                            i++;
                        }

                        // return all words that are not null and doesn't start with percentage sign
                        return fs.Where(s => (string.IsNullOrEmpty(s) == false && s.StartsWith("%") == false)).ToArray();
                    };

                List<string> symbols = new List<string>();

                if (_BaseVariable == null)
                {
                    if (!string.IsNullOrEmpty(this.Symbol))
                    {
                        var fs = pgh(this.Symbol);
                        foreach (var f in fs)
                        {
                            if (!symbols.Contains(f, StringComparer.OrdinalIgnoreCase))
                                symbols.Add(f);
                        }
                    }
                }
                else
                {
                    symbols.AddRange(this._BaseVariable.InvolvedSymbols);
                }

                // fused or multiplied symbols
                foreach (var fsm in FusedSymbols)
                {
                    var fs = pgh(fsm.Key);
                    foreach (var f in fs)
                    {
                        if (!symbols.Contains(f, StringComparer.OrdinalIgnoreCase))
                            symbols.Add(f);
                    }

                    if (fsm.Value.SymbolicVariable != null)
                    {
                        foreach (string ss in fsm.Value.SymbolicVariable.InvolvedSymbols)
                        {
                            if (!symbols.Contains(ss, StringComparer.OrdinalIgnoreCase))
                                symbols.Add(ss);
                        }
                    }
                }

                // primary symbol
                if (this.SymbolPowerTerm != null)
                {
                    foreach (string ss in this.SymbolPowerTerm.InvolvedSymbols)
                    {
                        if (!symbols.Contains(ss, StringComparer.OrdinalIgnoreCase))
                            symbols.Add(ss);
                    }
                }

                // fused constants powers
                foreach (var fc in FusedConstants)
                {
                    if (fc.Value.SymbolicVariable != null)
                    {
                        foreach (string ss in fc.Value.SymbolicVariable.InvolvedSymbols)
                        {
                            if (!symbols.Contains(ss, StringComparer.OrdinalIgnoreCase))
                                symbols.Add(ss);
                        }
                    }
                }

                // coeffiecient power term
                if (this.CoeffecientPowerTerm != null)
                {
                    foreach (string ss in this.CoeffecientPowerTerm.InvolvedSymbols)
                    {
                        if (!symbols.Contains(ss, StringComparer.OrdinalIgnoreCase))
                            symbols.Add(ss);
                    }
                }

                foreach (SymbolicVariable term in this.AddedTerms.Values)
                {
                    foreach (string ss in term.InvolvedSymbols)
                    {
                        if (!symbols.Contains(ss, StringComparer.OrdinalIgnoreCase))
                            symbols.Add(ss);
                    }
                }
                symbols.Sort();
                return symbols.ToArray();
            }
        }

        #region ICloneable Members

        public SymbolicVariable Clone(bool excludeExtraTerms = false)
        {
            
            SymbolicVariable clone = new SymbolicVariable(this.Symbol);

            //if (this._BaseVariable == null)  clone._Symbol = this._Symbol;

            clone.Coeffecient = this.Coeffecient;
            clone._SymbolPower = this._SymbolPower;

            if (this._SymbolPowerTerm != null)
                clone._SymbolPowerTerm = this._SymbolPowerTerm.Clone();

            if (this._CoeffecientPowerTerm != null)
                clone._CoeffecientPowerTerm = this._CoeffecientPowerTerm.Clone();

            if (this._BaseVariable != null)
                clone._BaseVariable = this._BaseVariable.Clone();

            foreach (var av in AddedTerms)
            {
                clone.AddedTerms.Add(av.Key, (SymbolicVariable)av.Value.Clone());
            }

            foreach (var fv in FusedSymbols)
            {
                clone.FusedSymbols.Add(fv.Key, (HybridVariable)fv.Value.Clone());
            }

            foreach (var fc in FusedConstants)
            {
                clone.FusedConstants.Add(fc.Key, (HybridVariable)fc.Value.Clone());
            }

            if (excludeExtraTerms == false)
            {
                foreach (var et in ExtraTerms)
                {
                    clone.ExtraTerms.Add(et.Clone());
                }
            }

            if(this._DividedTerm!=null) 
                clone._DividedTerm = this._DividedTerm.Clone();

            return clone;
        }

        #endregion
    }
}
