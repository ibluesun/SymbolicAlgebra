using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


namespace SymbolicAlgebra
{
    public partial class SymbolicVariable : ICloneable
    {


        #region coeffecient multiplication

        #region number precede symbol
        public static SymbolicVariable operator *(int a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }

        public static SymbolicVariable operator *(float a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }

        public static SymbolicVariable operator *(double a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }

        public static SymbolicVariable operator *(short a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }
        public static SymbolicVariable operator *(long a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }
        #endregion


        #region number after symbol
        public static SymbolicVariable operator *(SymbolicVariable b, int a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;

            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;
            

            return sv;
        }

        public static SymbolicVariable operator *(SymbolicVariable b, float a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }

        public static SymbolicVariable operator *(SymbolicVariable b, double a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }

        public static SymbolicVariable operator *(SymbolicVariable b, short a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }
        public static SymbolicVariable operator *(SymbolicVariable b, long a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient *= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient *= a;

            return sv;
        }
        #endregion

        #endregion



        #region coeffecient division

        #region number precede symbol
        public static SymbolicVariable operator /(int a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient = a / sv.Coeffecient;

            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient = a / v.Value.Coeffecient;

            return sv;
        }

        public static SymbolicVariable operator /(float a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient = a / sv.Coeffecient;

            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient = a / v.Value.Coeffecient;

            return sv;
        }

        public static SymbolicVariable operator /(double a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient = a / sv.Coeffecient;

            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient = a / v.Value.Coeffecient;

            return sv;
        }

        public static SymbolicVariable operator /(short a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient = a / sv.Coeffecient;

            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient = a / v.Value.Coeffecient;


            return sv;
        }
        public static SymbolicVariable operator /(long a, SymbolicVariable b)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient = a / sv.Coeffecient;

            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient = a / v.Value.Coeffecient;


            return sv;
        }
        #endregion


        #region number after symbol
        public static SymbolicVariable operator /(SymbolicVariable b, int a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient /= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient /= a;

            return sv;
        }

        public static SymbolicVariable operator /(SymbolicVariable b, float a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient /= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient /= a;

            return sv;
        }

        public static SymbolicVariable operator /(SymbolicVariable b, double a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient /= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient /= a;

            return sv;
        }

        public static SymbolicVariable operator /(SymbolicVariable b, short a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient /= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient /= a;

            return sv;
        }
        public static SymbolicVariable operator /(SymbolicVariable b, long a)
        {
            var sv = (SymbolicVariable)b.Clone();

            sv.Coeffecient /= a;
            if (sv.AddedTerms.Count > 0)
                foreach (var v in sv.AddedTerms) v.Value.Coeffecient /= a;

            return sv;
        }
        #endregion

        #endregion

    }
}