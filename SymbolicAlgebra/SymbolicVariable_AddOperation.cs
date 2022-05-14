﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
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

            // check for the denominator part .. if it is not the same .. you will have to put the whole
            //  symbolic variable in the extra term.
            if (!a.DividedTerm.Equals(b.DividedTerm))
            {
                SymbolicVariable a_b = a.Clone();
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
                    SymbolicVariable positive_b = b.Clone();
                    a_b.ExtraTerms.Add(new ExtraTerm { Term = positive_b, Negative = false });
                }
                AdjustZeroCoeffecientTerms(ref a_b);
                return a_b;

            }


            SymbolicVariable subB = b.Clone();
            int sub = -1;

            SymbolicVariable sv = a.Clone();
        NewPart:


            // compare the first or primary part of this instance to the primary part of other instance.
            // if they are the same sum their coefficients.
            bool consumed = false;

            if (a.BaseEquals(subB))
            {
                if (a.CoeffecientPowerTerm == null && subB.CoeffecientPowerTerm == null)
                {
                    sv.Coeffecient = a.Coeffecient + subB.Coeffecient;
                    consumed = true;
                }
                else if (a.CoeffecientPowerTerm != null && subB.CoeffecientPowerTerm != null)
                {
                    if (a.CoeffecientPowerTerm.Equals(subB.CoeffecientPowerTerm))
                    {
                        var cr = a.Coeffecient + subB.Coeffecient;
                        if (cr == 0)
                        {
                            sv.Coeffecient = cr;
                            consumed = true;
                        }
                    }
                }
                else
                {
                    // nothing
                }

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
                    var iv = a.AddedTerms[av.Key].Clone();

                    if (iv.CoeffecientPowerTerm == null && subB.CoeffecientPowerTerm == null)
                    {
                        iv.Coeffecient = iv.Coeffecient + subB.Coeffecient;
                        sv.AddedTerms[av.Key] = iv;
                        consumed = true;
                    }
                    else if (iv.CoeffecientPowerTerm != null && subB.CoeffecientPowerTerm != null)
                    {
                        if (iv.CoeffecientPowerTerm.Equals(subB.CoeffecientPowerTerm))
                        {
                            var cr = iv.Coeffecient + subB.Coeffecient;
                            if (cr == 0)
                            {
                                iv.Coeffecient = cr;
                                sv.AddedTerms[av.Key] = iv;
                                consumed = true;
                            }
                        }
                    }
                    else
                    {
                        /* do nothing */
                    }
                }
            }

            if (!consumed)
            {
                // add it to the positive variables.

                SymbolicVariable pv = null;

                sv.AddedTerms.TryGetValue(subB.WholeValueBaseKey, out pv);

                if (pv == null)
                {
                    pv = subB.Clone();

                    var SubTerms = pv._AddedTerms; // store them for later use.
                    pv._AddedTerms = null;

                    pv.DividedTerm = One;
                    // then add the original term
                    sv.AddedTerms.Add(pv.WholeValueBaseKey, pv);

                    if (SubTerms != null)
                    {
                        //then add the value added terms if exist
                        foreach (var at in pv.AddedTerms)
                        {
                            sv.AddedTerms.Add(at.Value.WholeValueBaseKey, at.Value);
                        }
                    }
                }
                else
                {
                    //exist before add it to this variable.
                    sv.AddedTerms[subB.WholeValueBaseKey] = Add(sv.AddedTerms[subB.WholeValueBaseKey], subB);
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


            // reaching here indicates that we didn't sum up the value to any of the target extra terms
            // so we need to include extra terms here
            if (b._ExtraTerms != null && b._ExtraTerms.Count > 0)
            {
                // add extra terms of b into sv
                foreach (var eb in b._ExtraTerms)
                {
                    sv.ExtraTerms.Add(eb.Clone());
                }
            }

            AdjustSpecialFunctions(ref sv);

            AdjustZeroPowerTerms(sv);
            AdjustZeroCoeffecientTerms(ref sv);


            return sv;
        }

    }
}
