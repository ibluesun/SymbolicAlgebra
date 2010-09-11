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
        /// The  number multiplied by the symbol  a*x^2  I mean {a}
        /// </summary>
        public double Coeffecient { private set; get; }


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
                if (_SymbolPower == 0) _Symbol = string.Empty;
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


        /// <summary>
        /// The only constructor for the symbolic variable
        /// </summary>
        /// <param name="variable"></param>
        public SymbolicVariable(string variable)
        {
            double coe;

            //try the numbers first
            if (double.TryParse(variable, out coe))
            {
                Symbol = string.Empty;
                Coeffecient = coe;
                SymbolPower = 0;
            }
            else
            {
                // not number then take the whole string as a symbol
                Symbol = variable;
                Coeffecient = 1;
                SymbolPower = 1;
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


        /// <summary>
        /// Terms that couldn't be divide on this term like  x/(x^2-y^5)
        /// </summary>
        SymbolicVariable _DividedTerm;



        /// <summary>
        /// a*x^2*y^3*z^7  I mean {y^3, and z^7}
        /// </summary>
        private Dictionary<string, HybridVariable> _FusedSymbols;

        /// <summary>
        /// Extra terms that couldn't be added to the current term.
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
        /// Multiplied terms in the term other that original symbol letter.
        /// </summary>
        public Dictionary<string, HybridVariable> FusedSymbols
        {
            get
            {
                if (_FusedSymbols == null) _FusedSymbols = new Dictionary<string, HybridVariable>(StringComparer.OrdinalIgnoreCase);
                return _FusedSymbols;
            }
        }



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
                else result = variableName;
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
                string powerTermText = powerTerm.ToString();
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


        /// <summary>
        /// from a*x^2  I mean {x^2}
        /// used as a key also.
        /// </summary>
        public string SymbolBaseValue
        {
            get
            {
                string result = string.Empty;

                if (_SymbolPowerTerm != null)
                {
                    result = Symbol + "^(" + SymbolPowerText + ")";
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

            if (!string.IsNullOrEmpty(Symbol))
            {
                // include the power of instance 
                if (string.IsNullOrEmpty(SymbolPowerText))
                {
                    if (_SymbolPower == 0)
                        result = "1";
                    else
                        result = Symbol;
                }
                else
                {
                    // add parenthesis if the text more than one charachter

                    if (SymbolPower >= 0)
                    {
                        if (SymbolPowerText.Length > 1)
                            result = Symbol + "^(" + SymbolPowerText + ")";
                        else
                            result = Symbol + "^" + SymbolPowerText;
                    }
                    else
                    {
                        if (_SymbolPowerTerm == null)
                            result = Symbol + "^" + Math.Abs(SymbolPower).ToString(CultureInfo.InvariantCulture);
                        else
                        {
                            if (SymbolPowerText.Length > 1)
                                result = Symbol + "^(" + SymbolPowerText + ")";
                            else
                                result = Symbol + "^" + SymbolPowerText;
                        }
                    }
                }
            }

            for (int i = 0; i < FusedSymbols.Count; i++)
            {

                double pp = GetFusedPower(i);

                if (string.IsNullOrEmpty(result))
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


        /// <summary>
        /// Returns the whole symbolic variable   symobl part with coeffecient part
        /// </summary>
        private string FormSymbolTextValue()
        {
            string result = GetFormattedSymbolicValue();

            if (Coeffecient == 0) return "0";

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
                    if (CoeffecientPowerTerm.IsMultiValue)
                        rr = rr + "^(" + CoeffecientPowerText + ")";
                    else
                        rr = rr + "^" + CoeffecientPowerText;  

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
                            result = rr + result;
                        }
                        else
                        {
                            result = rr;
                        }
                    }
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
                            result += "+" + sv.FormSymbolTextValue();
                    }
                }
            }

            if (DividedTerm.FormSymbolTextValue() != "1") result = result + "/(" + DividedTerm.ToString() + ")";
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
        /// 
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

                    var c = (SymbolicVariable)this.Clone();
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


        /// <summary>
        /// Compare the symbol of current instance to the symbol of parameter instance
        /// either in primary symbol part and in fused symbols.
        /// No compare occure on the added terms part.
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        public bool BaseEquals(SymbolicVariable sv)
        {

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
                if (this.Symbol.Equals(sv.Symbol, StringComparison.OrdinalIgnoreCase))
                {
                    if (this._SymbolPowerTerm != null && sv._SymbolPowerTerm != null)
                    {
                        if (_SymbolPowerTerm.Equals(sv._SymbolPowerTerm)) return true;
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
            if (this.IsMultiValue && sv.IsMultiValue)
            {
                // make sure added terms are equal in count.
                if (this._AddedTerms.Count == sv._AddedTerms.Count)
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

        private static void AdjustZeroPowerTerms(SymbolicVariable svar)
        {
            for (int i = svar.FusedSymbols.Count - 1; i >= 0; i--)
            {
                if (svar.FusedSymbols.ElementAt(i).Value.IsZero)
                    svar.FusedSymbols.Remove(svar.FusedSymbols.ElementAt(i).Key);
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
                    svar.AddedTerms.Remove(svar.AddedTerms.ElementAt(i).Key);
            }

            // then check the priamry term.
            if (svar.Coeffecient == 0)
            {
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
        public bool IsThisTermCoeffecientOnly
        {
            get
            {
                if (string.IsNullOrEmpty(this.SymbolBaseValue))
                {
                    return true;
                }
                return false;
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
                if(_AddedTerms==null) return false;
                if (_AddedTerms.Count > 0) return true;
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
                if (this.CoeffecientPowerTerm == null)
                {
                    if (this.Coeffecient < 0) return true;
                    else return false;
                }
                else
                {
                    return false;   //coeffecient may be raised
                }
            }
        }

        /// <summary>
        /// returns all the symbols involved in this object
        /// </summary>
        public string[] InvolvedSymbols
        {
            get
            {
                List<string> symbols = new List<string>();

                if (_BaseVariable == null)
                {
                    symbols.Add(this.Symbol);
                }
                else
                {
                    symbols.AddRange(this._BaseVariable.InvolvedSymbols);
                }

                foreach (string fs in FusedSymbols.Keys)
                {
                    if (!symbols.Contains(fs, StringComparer.OrdinalIgnoreCase))
                        symbols.Add(fs);
                }

                if (this.SymbolPowerTerm != null)
                {
                    foreach (string ss in this.SymbolPowerTerm.InvolvedSymbols)
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

                return symbols.ToArray();
            }
        }

        #region ICloneable Members

        public object Clone()
        {
            SymbolicVariable clone = new SymbolicVariable(this.Symbol);
            clone.Coeffecient = this.Coeffecient;
            clone._SymbolPower = this._SymbolPower;

            if (this._SymbolPowerTerm != null)
                clone._SymbolPowerTerm = (SymbolicVariable)this._SymbolPowerTerm.Clone();

            if (this._CoeffecientPowerTerm != null)
                clone._CoeffecientPowerTerm = (SymbolicVariable)this._CoeffecientPowerTerm.Clone();

            if (this._BaseVariable != null)
                clone._BaseVariable = (SymbolicVariable)this._BaseVariable.Clone();

            foreach (var av in AddedTerms)
            {
                clone.AddedTerms.Add(av.Key, (SymbolicVariable)av.Value.Clone());
            }

            foreach (var fv in FusedSymbols)
            {
                clone.FusedSymbols.Add(fv.Key, (HybridVariable)fv.Value.Clone());
            }

            if(this._DividedTerm!=null) 
                clone._DividedTerm = (SymbolicVariable)this._DividedTerm.Clone();

            return clone;
        }

        #endregion
    }
}
