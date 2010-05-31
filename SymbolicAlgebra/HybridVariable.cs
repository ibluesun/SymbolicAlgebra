using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
    public struct HybridVariable
    {
        public SymbolicVariable SymbolicVariable;
        public double NumericalVariable;


        public static HybridVariable operator -(HybridVariable hv, HybridVariable num)
        {
            HybridVariable nhv;
            nhv.NumericalVariable = hv.NumericalVariable - num.NumericalVariable;
            nhv.SymbolicVariable = hv.SymbolicVariable - num.SymbolicVariable;
            return nhv;
        }

        public static HybridVariable operator -(HybridVariable hv, double num)
        {
            HybridVariable nhv;
            nhv.NumericalVariable = hv.NumericalVariable - num;
            nhv.SymbolicVariable = hv.SymbolicVariable ;
            return nhv;
        }

        public static HybridVariable operator +(HybridVariable hv, HybridVariable num)
        {
            HybridVariable nhv;
            nhv.NumericalVariable = hv.NumericalVariable + num.NumericalVariable;
            nhv.SymbolicVariable = hv.SymbolicVariable + num.SymbolicVariable;
            return nhv;
        }


        public static HybridVariable operator +(HybridVariable hv, double num)
        {
            HybridVariable nhv;
            nhv.NumericalVariable = hv.NumericalVariable + num;
            nhv.SymbolicVariable = hv.SymbolicVariable;
            return nhv;
        }

        public static HybridVariable operator *(HybridVariable hv, HybridVariable num)
        {
            HybridVariable nhv;
            nhv.NumericalVariable = hv.NumericalVariable * num.NumericalVariable;
            nhv.SymbolicVariable = hv.SymbolicVariable * num.SymbolicVariable;
            return nhv;
        }

        public static HybridVariable operator *(HybridVariable hv, double num)
        {
            HybridVariable nhv;
            nhv.NumericalVariable = hv.NumericalVariable * num;
            nhv.SymbolicVariable = hv.SymbolicVariable * num;
            return nhv;
        }



        public static HybridVariable operator /(HybridVariable hv, HybridVariable num)
        {
            HybridVariable nhv;
            nhv.NumericalVariable = hv.NumericalVariable - num.NumericalVariable;
            nhv.SymbolicVariable = hv.SymbolicVariable - num.SymbolicVariable;
            return nhv;
        }

        
        public bool IsZero
        {
            get
            {
                if (SymbolicVariable != null)
                {
                    //if (SymbolicVariable.Coeffecient == 0)
                    //{
                    //    bool zz = true;
                    //    foreach (SymbolicVariable vv in SymbolicVariable.AddedTerms.Values)
                    //        if (vv.Coeffecient != 0) zz = false;

                    //    return zz;

                    //}
                    return SymbolicVariable.IsZero;
                }
                else if (NumericalVariable == 0)
                {
                    return true;
                }
                else
                    return false;
            }

        }
    }
}
