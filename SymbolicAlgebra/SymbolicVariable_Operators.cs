using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
    public partial class SymbolicVariable : ICloneable
    {

        public static SymbolicVariable operator +(SymbolicVariable a, SymbolicVariable b)
        {

            SymbolicVariable subB = (SymbolicVariable)b.Clone();
            int sub = -1;

            SymbolicVariable sv = (SymbolicVariable)a.Clone();
            NewPart:
            

            // compare the first or primary part of this instance to the primary part of other instance.
            // if they are the same sum their coefficients.
            bool consumed = false;

            if (a.SymbolsEquals(subB))
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
                if (av.Value.SymbolsEquals(subB))
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

            if (a.SymbolsEquals(subB))
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
                if (av.Value.SymbolsEquals(subB))
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



    }
}
