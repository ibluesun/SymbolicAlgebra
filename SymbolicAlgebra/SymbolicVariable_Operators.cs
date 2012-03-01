using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
#if SILVERLIGHT
    public partial class SymbolicVariable
#else
    public partial class SymbolicVariable : ICloneable
#endif
    {

        /// <summary>
        /// Add to symbolic variables.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SymbolicVariable Add(SymbolicVariable a, SymbolicVariable b)
        {
            if (a == null || b == null) return null;

            if (!a.DividedTerm.Equals( b.DividedTerm))
            {
                SymbolicVariable a_b = (SymbolicVariable)a.Clone();
                bool econsumed = false;
                if (a_b.ExtraTerms.Count > 0)
                {
                    // find if in extra terms there is an equality  of b into it
                    for (int iet = 0; iet < a_b.ExtraTerms.Count; iet++)
                    {
                        if (a_b.ExtraTerms[iet].Term.DividedTerm.Equals(b.DividedTerm))
                        {
                            a_b.ExtraTerms[iet].Term = Add(a_b.ExtraTerms[iet].Term, b);
                            econsumed = true;
                            break;
                        }
                    }
                }

                if (!econsumed)
                {
                    // add in the extra terms
                    SymbolicVariable positive_b = (SymbolicVariable)b.Clone();
                    a_b.ExtraTerms.Add(new ExtraTerm { Term = positive_b, Negative = false });
                }
                AdjustZeroCoeffecientTerms(ref a_b);
                return a_b;
                
            }


            SymbolicVariable subB = (SymbolicVariable)b.Clone();
            int sub = -1;

            SymbolicVariable sv = (SymbolicVariable)a.Clone();
            NewPart:
            

            // compare the first or primary part of this instance to the primary part of other instance.
            // if they are the same sum their coefficients.
            bool consumed = false;

            if (a.BaseEquals(subB))
            {
                sv.Coeffecient = a.Coeffecient + subB.Coeffecient;
                consumed = true;
            }
              
            //so the equality doesn't exits or this instance have other terms also

            // there are two cases now 
            //  1- the symbolic can be added to one of the existing terms primary and others in addedvariables (which will be perfect)
            //  2- there are no compatible term so we have to add it to the AddedTerms of this instance.
            

            //try to add to the rest terms
            foreach (var av in a.AddedTerms)
            {
                if (av.Value.BaseEquals(subB))
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

                    var SubTerms = pv._AddedTerms; // store them for later use.
                    pv._AddedTerms = null;

                    pv.DividedTerm = One;
                    // then add the original term
                    sv.AddedTerms.Add(pv.SymbolBaseValue, pv);

                    if (SubTerms != null)
                    {
                        //then add the value added terms if exist
                        foreach (var at in pv.AddedTerms)
                        {
                            sv.AddedTerms.Add(at.Value.SymbolBaseValue, at.Value);
                        }
                    }
                }
                else
                {
                    //exist before add it to this variable.
                    sv.AddedTerms[subB.SymbolBaseValue] = Add(sv.AddedTerms[subB.SymbolBaseValue], subB);
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
            AdjustZeroCoeffecientTerms(ref sv);
            return sv;

        }


        /// <summary>
        /// Subtracts symbolic variable b from symbolic variable a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SymbolicVariable Subtract(SymbolicVariable a, SymbolicVariable b)
        {
            if (a == null || b == null) return null;

            if (!a.DividedTerm.Equals(b.DividedTerm))
            {
                SymbolicVariable a_b = (SymbolicVariable)a.Clone();
                bool econsumed = false;
                if (a_b.ExtraTerms.Count > 0)
                {
                    // find if in extra terms there is an equality  of b into it
                    for (int iet = 0; iet < a_b.ExtraTerms.Count; iet++)
                    {
                        if (a_b.ExtraTerms[iet].Term.DividedTerm.Equals(b.DividedTerm))
                        {
                            a_b.ExtraTerms[iet].Term = Subtract(a_b.ExtraTerms[iet].Term, b);
                            econsumed = true;
                            break;
                        }
                    }
                }

                if (!econsumed)
                {
                    // add in the extra terms
                    var negative_b = ((SymbolicVariable)b.Clone());
                    //negative_b.Coeffecient *= -1;
                    a_b.ExtraTerms.Add(new ExtraTerm { Term = negative_b, Negative = true });
                }
                AdjustZeroCoeffecientTerms(ref a_b);
                return a_b;

            }


            SymbolicVariable subB = (SymbolicVariable)b.Clone();
            int sub = -1;

            SymbolicVariable sv = (SymbolicVariable)a.Clone();
        NewPart:
            
            bool consumed = false;

            if (a.BaseEquals(subB))
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
                if (av.Value.BaseEquals(subB))
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

                    var SubTerms = pv._AddedTerms; // store them for later use.
                    pv._AddedTerms = null;

                    pv.DividedTerm = One;

                    sv.AddedTerms.Add(pv.SymbolBaseValue, pv);

                    if (SubTerms != null)
                    {
                        //then add the value added terms if exist
                        foreach (var at in pv.AddedTerms)
                        {
                            at.Value.Coeffecient *= -1;
                            sv.AddedTerms.Add(at.Value.SymbolBaseValue, at.Value);
                        }
                    }

                }
                else
                {
                    //exist before add it to this variable.

                    sv.AddedTerms[subB.SymbolBaseValue] = Subtract(sv.AddedTerms[subB.SymbolBaseValue], subB);
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
            AdjustZeroCoeffecientTerms(ref sv);


            return sv;
        }



    }
}
