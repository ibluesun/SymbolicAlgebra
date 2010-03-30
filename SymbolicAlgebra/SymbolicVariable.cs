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



        private string _VariableName;

        /// <summary>
        /// the symbol name   a*x^2 I mean {x}
        /// Note: name will not contain any spaces
        /// </summary>
        public string VariableName 
        {
            private set
            {
                _VariableName = string.Empty; // remove trailing and begining spaces.
                foreach (var vc in value)
                {
                    if (!char.IsWhiteSpace(vc))
                        _VariableName += vc;
                }
            }
            get
            {
                return _VariableName;
            }
        }


        /// <summary>
        /// The symbolic power a*x^2  I mean {2}
        /// </summary>
        public double SymbolPower { private set; get; }


        /// <summary>
        /// The only constructor for the symbolic variable
        /// </summary>
        /// <param name="variable"></param>
        public SymbolicVariable(string variable)
        {
            double coe;
            if (double.TryParse(variable, out coe))
            {
                VariableName = string.Empty;
                Coeffecient = coe;
                SymbolPower = 0;
            }
            else
            {
                VariableName = variable;
                Coeffecient = 1;
                SymbolPower = 1;
            }
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
        /// a*x^2*y^3*z^7  I mean {y^3, and z^7}
        /// </summary>
        private Dictionary<string, double> _FusedVariables;

        public Dictionary<string, SymbolicVariable> AddedTerms
        {
            get
            {
                if (_AddedTerms == null) _AddedTerms = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                return _AddedTerms;
            }
        }

        public Dictionary<string, double> FusedVariables
        {
            get
            {
                if (_FusedVariables == null) _FusedVariables = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
                return _FusedVariables;
            }
        }


        /// <summary>
        /// from a*x^2  I mean {x^2} or x^-2
        /// </summary>
        private string GetSymbolBaseValue(int i)
        {
            
            string result = string.Empty;
            double power = FusedVariables.ElementAt(i).Value;
            string variableName = FusedVariables.ElementAt(i).Key;

            if ( power != 0)
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
            double power = FusedVariables.ElementAt(i).Value;
            string variableName = FusedVariables.ElementAt(i).Key;

            if (power != 0)
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
            return FusedVariables.ElementAt(i).Value;
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
                if (SymbolPower != 0)
                {
                    // variable exist 
                    if (SymbolPower != 1) result = VariableName + "^" + SymbolPower.ToString(CultureInfo.InvariantCulture);
                    else result = VariableName;
                }

                for (int i = 0; i < FusedVariables.Count; i++)
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
        /// for showing purposed only.
        /// not to be used in keys.
        /// used internally.
        /// </summary>
        private  string FormattedSymbolicValue
        {
            get
            {
                string result = string.Empty;
                if (SymbolPower != 0)
                {
                    // variable exist 
                    if (Math.Abs(SymbolPower) != 1)
                    {
                        result = VariableName + "^" + SymbolPower.ToString(CultureInfo.InvariantCulture);
                    }
                    else result = VariableName;
                }

                for (int i = 0; i < FusedVariables.Count; i++)
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
        }


        /// <summary>
        /// returns the whole symbolic
        /// </summary>
        private string SymbolTextValue
        {
            get
            {
                string result = FormattedSymbolicValue;

                if (Coeffecient == 0) return "0";

                if (Coeffecient == 1)
                {
                    if (string.IsNullOrEmpty(result)) result = "1";
                }

                if (Coeffecient != 1)
                {
                    string rr = Coeffecient.ToString(CultureInfo.InvariantCulture);
                    if (SymbolPower != 0)
                    {

                        if (SymbolPower < 0)
                        {
                            
                            result = rr + "/" + result;
                        }
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
                        if (FusedVariables.Count > 0)
                        {
                            //
                            result = rr + result;
                        }
                        else
                        {
                            result = rr;
                        }
                    }
                }

                return result;
            }
        }


        public override string ToString()
        {

            string result = SymbolTextValue;

            if (AddedTerms.Count > 0)
                foreach (var sv in AddedTerms.Values)
                {
                    if (sv.Coeffecient != 0)
                    {
                        if (sv.SymbolTextValue.StartsWith("-"))
                            result += sv.SymbolTextValue;
                        else
                            result += "+" + sv.SymbolTextValue;
                    }
                }

            return result;
        }


        
        public bool VariableEquals(SymbolicVariable sv)
        {

            if (this.FusedVariables.Count > 0 || sv.FusedVariables.Count > 0)
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

                Dictionary<string,double> vvs = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

                //variable name of this instance    THE VARIABLE NAME which is x or y or xy  
                if (!string.IsNullOrEmpty(this.VariableName))
                {
                    vvs.Add(this.VariableName, SymbolPower);
                }

                // variable part of the target instance
                if (!string.IsNullOrEmpty(sv.VariableName))
                {
                    if (vvs.ContainsKey(sv.VariableName)) vvs[sv.VariableName] -= sv.SymbolPower;
                    else vvs.Add(sv.VariableName,-1 * sv.SymbolPower);
                }

                //fused variables of this instance
                foreach (var cv in this.FusedVariables)
                {
                    if (vvs.ContainsKey(cv.Key)) vvs[cv.Key] += cv.Value;
                    else vvs.Add(cv.Key, cv.Value);
                }

                // fused variables of the target instance.
                foreach (var tv in sv.FusedVariables)
                {
                    if (vvs.ContainsKey(tv.Key)) vvs[tv.Key] -= tv.Value;
                    else vvs.Add(tv.Key, tv.Value);
                }

                foreach (var rv in vvs)
                {
                    if (rv.Value != 0) return false;
                }

                return true;

            }
            else
            {
                if (this.VariableName.Equals(sv.VariableName, StringComparison.OrdinalIgnoreCase))
                {
                    if (this.SymbolPower == sv.SymbolPower)
                    {

                        return true;

                    }
                }
            }
            return false;
        }



        public bool Equals(SymbolicVariable sv)
        {
            if (this.VariableName.Equals(sv.VariableName, StringComparison.OrdinalIgnoreCase))
            {
                if (this.SymbolPower == sv.SymbolPower)
                {
                    if (this.Coeffecient == sv.Coeffecient)
                    {
                        return true;
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
            return Coeffecient.GetHashCode() + VariableName.GetHashCode() + SymbolPower.GetHashCode();
        }



        public static SymbolicVariable Add(SymbolicVariable a, SymbolicVariable b)
        {
            return a + b;
        }

        public static SymbolicVariable Subtract(SymbolicVariable a, SymbolicVariable b)
        {
            return a - b;
        }

        public static SymbolicVariable Multiply(SymbolicVariable a, SymbolicVariable b)
        {
            return a * b;
        }

        public static SymbolicVariable Divide(SymbolicVariable a, SymbolicVariable b)
        {
            return a / b;
        }



        public static SymbolicVariable operator +(SymbolicVariable a, SymbolicVariable b)
        {

            SymbolicVariable subB = (SymbolicVariable)b.Clone();
            int sub = -1;

            SymbolicVariable sv = (SymbolicVariable)a.Clone();
            NewPart:
            

            // compare the first or primary part of this instance to the primary part of other instance.
            // if they are the same sum their coefficients.
            bool consumed = false;

            if (a.VariableEquals(subB))
            {
                sv.Coeffecient = a.Coeffecient + subB.Coeffecient;
                consumed = true;
            }
              
            //so the equality doesn't exits or this instance have other terms also

            // there are two cases now 
            //  1- the symbolic can be added to one of the existing terms primary and others in addedvariables (which will be perfect)
            //  2- there are no compatible term so we have to add it to the addedvariables of this instance.
            

            //try to add to the rest terms
            foreach (var av in a.AddedTerms)
            {
                if (av.Value.VariableEquals(subB))
                {
                    var iv = (SymbolicVariable)a.AddedTerms[av.Key].Clone();
                    iv.Coeffecient = iv.Coeffecient + subB.Coeffecient;
                    sv.AddedTerms[av.Key] = iv;
                    consumed = true;
                }
            }

            if (!consumed)
            {
                // add it to the positive variables.

                SymbolicVariable pv;

                sv.AddedTerms.TryGetValue(subB.SymbolBaseValue, out pv);

                if (pv == null)
                {
                    pv = (SymbolicVariable)subB.Clone();
                    sv.AddedTerms.Add(pv.SymbolBaseValue, pv);
                }
                else
                {
                    //exist before add it to this variable.
                    sv.AddedTerms[subB.SymbolBaseValue] += subB;
                }
            }

            if (b.AddedTerms.Count > 0)
            {
                sub = sub + 1;  //increase 
                if (sub < b.AddedTerms.Count)
                {
                    // there are still terms to be consumed 
                    //   this new term is a sub term in b and will be added to all terms of a.
                    subB = b.AddedTerms.ElementAt(sub).Value;
                    goto NewPart;
                }
            }

            AdjustZeroPowerTerms(sv);
            AdjustZeroCoeffecientTerms(sv);


            return sv;

        }

        public static SymbolicVariable operator -(SymbolicVariable a, SymbolicVariable b)
        {

            SymbolicVariable subB = (SymbolicVariable)b.Clone();
            int sub = -1;

            SymbolicVariable sv = (SymbolicVariable)a.Clone();
        NewPart:
            
            bool consumed = false;

            if (a.VariableEquals(subB))
            {
                sv.Coeffecient = a.Coeffecient - subB.Coeffecient;
                consumed = true;
            }

            //so the equality doesn't exits or this instance have other terms also

            // there are two cases now 
            //  1- the symbolic can be added to one of the existing terms (which will be perfect)
            //  2- there are no compatible term so we have to add it to the addedvariables of this instance.


            foreach (var av in a.AddedTerms)
            {
                if (av.Value.VariableEquals(subB))
                {
                    var iv = (SymbolicVariable)a.AddedTerms[av.Key].Clone();
                    iv.Coeffecient = iv.Coeffecient - subB.Coeffecient;
                    sv.AddedTerms[av.Key] = iv;
                    consumed = true;
                }
            }


            if (!consumed)
            {
                // add it to the positive variables.

                SymbolicVariable pv;

                sv.AddedTerms.TryGetValue(subB.SymbolBaseValue, out pv);

                if (pv == null)
                {
                    pv = (SymbolicVariable)subB.Clone();
                    pv.Coeffecient *= -1;

                    sv.AddedTerms.Add(pv.SymbolBaseValue, pv);
                }
                else
                {
                    //exist before add it to this variable.

                    sv.AddedTerms[subB.SymbolBaseValue] -= subB;
                }
            }

            if (b.AddedTerms.Count > 0)
            {
                sub = sub + 1;  //increase 
                if (sub < b.AddedTerms.Count)
                {
                    // there are still terms to be consumed 
                    //   this new term is a sub term in b and will be added to all terms of a.
                    subB = b.AddedTerms.ElementAt(sub).Value;
                    goto NewPart;
                }
            }


            AdjustZeroPowerTerms(sv);
            AdjustZeroCoeffecientTerms(sv);


            return sv;
        }


        public static SymbolicVariable operator *(SymbolicVariable a, SymbolicVariable b)
        {
            SymbolicVariable subB = (SymbolicVariable)b.Clone();
            
            subB._AddedTerms = null;   // remove added variables to prevent its repeated calculations in second passes
            // or to make sure nothing bad happens {my idiot design :S)

            int subIndex = 0;

            SymbolicVariable total = default(SymbolicVariable);

            SymbolicVariable sv = (SymbolicVariable)a.Clone();
            if (a.VariableEquals(subB))
            {
                sv.Coeffecient = sv.Coeffecient * subB.Coeffecient;
                sv.SymbolPower = sv.SymbolPower + subB.SymbolPower;

                //fuse the fusedvariables in b into sv
                foreach (var bfv in subB.FusedVariables)
                {
                    if (sv.FusedVariables.ContainsKey(bfv.Key))
                        sv.FusedVariables[bfv.Key] += bfv.Value;
                    else
                        sv.FusedVariables.Add(bfv.Key, bfv.Value);
                }

            }
            else
            {

                if (string.IsNullOrEmpty(sv.VariableName))
                {
                    // the instance have an empty primary variable so we should add it 
                    sv.VariableName = subB.VariableName;
                    sv.SymbolPower = subB.SymbolPower;

                    //fuse the fusedvariables in b into sv
                    foreach (var bfv in subB.FusedVariables)
                    {
                        if (sv.FusedVariables.ContainsKey(bfv.Key))
                            sv.FusedVariables[bfv.Key] += bfv.Value;
                        else
                            sv.FusedVariables.Add(bfv.Key, bfv.Value);

                    }
                }
                else
                {
                    if(sv.VariableName.Equals(subB.VariableName, StringComparison.OrdinalIgnoreCase))
                    {
                        sv.SymbolPower += subB.SymbolPower;
                    }
                    else if (sv.FusedVariables.ContainsKey(subB.VariableName))
                    {
                        sv.FusedVariables[subB.VariableName] += subB.SymbolPower;
                    }
                    else
                    {
                        sv.FusedVariables.Add(subB.VariableName, subB.SymbolPower);
                    }
                }

                sv.Coeffecient = a.Coeffecient * subB.Coeffecient;

            } 


            //here is a code to continue with other parts of a when multiplying them
            if (sv.AddedTerms.Count > 0)
            {
                Dictionary<string, SymbolicVariable> newAddedVariables = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                foreach (var vv in sv.AddedTerms)
                {                    
                    var newv = vv.Value * subB;

                    newAddedVariables.Add(newv.SymbolBaseValue, newv);
                }
                sv._AddedTerms = newAddedVariables;

            }

            np:
            if (subIndex < b.AddedTerms.Count)
            {
                // we should multiply other parts also 
                // then add it to the current instance

                // there are still terms to be consumed 
                //   this new term is a sub term in b and will be added to all terms of a.
                subB = b.AddedTerms.ElementAt(subIndex).Value;

                if (total != null) total = total + sv + (a * subB);
                else total = sv + (a * subB);

                subIndex = subIndex + 1;  //increase 
                goto np;
            }
            else
            {
                if (total == null) total = sv;
            }

            AdjustZeroPowerTerms(total);
            AdjustZeroCoeffecientTerms(total);

            return total;
        }

        public static SymbolicVariable operator /(SymbolicVariable a, SymbolicVariable b)
        {
            if (b.AddedTerms.Count > 0) throw new Exception("Can't divide over multi term number");
            SymbolicVariable subB = (SymbolicVariable)b.Clone();

            subB._AddedTerms = null;   // remove added variables to prevent its repeated calculations in second passes
            // or to make sure nothing bad happens {my idiot design :S)

            int subIndex = 0;

            SymbolicVariable total = default(SymbolicVariable);

            SymbolicVariable sv = (SymbolicVariable)a.Clone();
            if (a.VariableEquals(subB))
            {
                sv.Coeffecient = sv.Coeffecient / subB.Coeffecient;
                sv.SymbolPower = sv.SymbolPower - subB.SymbolPower;

                //fuse the fusedvariables in b into sv
                foreach (var bfv in subB.FusedVariables)
                {
                    if (sv.FusedVariables.ContainsKey(bfv.Key))
                        sv.FusedVariables[bfv.Key] -= bfv.Value;
                    else
                        sv.FusedVariables.Add(bfv.Key, -1 * bfv.Value);
                }
            }
            else
            {

                if (string.IsNullOrEmpty(sv.VariableName))
                {
                    // the instance have an empty primary variable so we should add it 
                    sv.VariableName = subB.VariableName;
                    sv.SymbolPower = -1 * subB.SymbolPower;

                    //fuse the fusedvariables in b into sv
                    foreach (var bfv in subB.FusedVariables)
                    {
                        if (sv.FusedVariables.ContainsKey(bfv.Key))
                            sv.FusedVariables[bfv.Key] -= bfv.Value;
                        else
                            sv.FusedVariables.Add(bfv.Key, -1 * bfv.Value);
                    }

                }
                else
                {
                    if (sv.VariableName.Equals(subB.VariableName, StringComparison.OrdinalIgnoreCase))
                    {
                        sv.SymbolPower -= subB.SymbolPower;
                    }
                    else if (sv.FusedVariables.ContainsKey(subB.VariableName))
                    {
                        sv.FusedVariables[subB.VariableName] -= subB.SymbolPower;
                    }
                    else
                    {
                        sv.FusedVariables.Add(subB.VariableName, -1 * subB.SymbolPower);
                    }
                }

                sv.Coeffecient = a.Coeffecient / subB.Coeffecient;

            }

            if (sv.AddedTerms.Count > 0)
            {
                Dictionary<string, SymbolicVariable> newAddedVariables = new Dictionary<string, SymbolicVariable>(StringComparer.OrdinalIgnoreCase);
                foreach (var vv in sv.AddedTerms)
                {
                    var newv = vv.Value / subB;
                    newAddedVariables.Add(newv.SymbolBaseValue, newv);

                }
                sv._AddedTerms = newAddedVariables;
            }

        np:
            if (subIndex < b.AddedTerms.Count)
            {
                // we should multiply other parts also 
                // then add it to the current instance

                // there are still terms to be consumed 
                //   this new term is a sub term in b and will be added to all terms of a.
                subB = b.AddedTerms.ElementAt(subIndex).Value;

                if (total != null) total = total + sv + (a / subB);
                else total = sv + (a / subB);

                subIndex = subIndex + 1;  //increase 
                goto np;
            }
            else
            {
                if (total == null) total = sv;
            }


            AdjustZeroPowerTerms(total);
            AdjustZeroCoeffecientTerms(total);

            return total; //RemoveZeroTerms(total);
        }




        private static void AdjustZeroPowerTerms(SymbolicVariable svar)
        {
            for (int i = svar.FusedVariables.Count - 1; i >= 0; i--)
            {
                if (svar.FusedVariables.ElementAt(i).Value == 0)
                    svar.FusedVariables.Remove(svar.FusedVariables.ElementAt(i).Key);
            }

            foreach (var r in svar.AddedTerms.Values) AdjustZeroPowerTerms(r);

        }

        private static void AdjustZeroCoeffecientTerms(SymbolicVariable svar)
        {
            for (int i = svar.AddedTerms.Count - 1; i >= 0; i--)
            {
                if (svar.AddedTerms.ElementAt(i).Value.Coeffecient == 0)
                    svar.AddedTerms.Remove(svar.AddedTerms.ElementAt(i).Key);
            }

        }





        #region ICloneable Members

        public object Clone()
        {
            SymbolicVariable clone = new SymbolicVariable(this.VariableName);
            clone.Coeffecient = this.Coeffecient;
            clone.SymbolPower = this.SymbolPower;

            foreach (var av in AddedTerms)
            {
                clone.AddedTerms.Add(av.Key, av.Value);
            }
            foreach (var fv in FusedVariables)
            {
                clone.FusedVariables.Add(fv.Key, fv.Value);

            }


            return clone;
        }

        #endregion
    }
}
