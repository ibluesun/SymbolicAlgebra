using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;


namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
    {

        #region coeffecient multiplication

        #region number precede symbol
        public static SymbolicVariable operator *(int a, SymbolicVariable b)
        {

            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) * b;

            return sv;
        }

        public static SymbolicVariable operator *(float a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) * b;

            return sv;
        }

        public static SymbolicVariable operator *(double a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) * b;
            return sv;
        }

        public static SymbolicVariable operator *(short a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) * b;
            return sv;
        }
        public static SymbolicVariable operator *(long a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) * b;
            return sv;
        }
        #endregion


        #region number after symbol
        public static SymbolicVariable operator *(SymbolicVariable b, int a)
        {
            var sv = b * new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));
            return sv;
        }

        public static SymbolicVariable operator *(SymbolicVariable b, float a)
        {
            var sv = b * new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));
            return sv;
        }

        public static SymbolicVariable operator *(SymbolicVariable b, double a)
        {
            var sv = b * new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));
            return sv;
        }

        public static SymbolicVariable operator *(SymbolicVariable b, short a)
        {
            var sv = b * new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));
            return sv;
        }
        public static SymbolicVariable operator *(SymbolicVariable b, long a)
        {
            var sv = b * new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));
            return sv;
        }
        #endregion

        #endregion

        #region coeffecient division

        #region number precede symbol
        public static SymbolicVariable operator /(int a, SymbolicVariable b)
        {

            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) / b;

            return sv;
        }

        public static SymbolicVariable operator /(float a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) / b;

            return sv;
        }

        public static SymbolicVariable operator /(double a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) / b;

            return sv;
        }

        public static SymbolicVariable operator /(short a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) / b;

            return sv;
        }
        public static SymbolicVariable operator /(long a, SymbolicVariable b)
        {
            var sv = new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture)) / b;

            return sv;
        }
        #endregion


        #region number after symbol
        public static SymbolicVariable operator /(SymbolicVariable b, int a)
        {
            var sv = b / new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));

            return sv;
        }

        public static SymbolicVariable operator /(SymbolicVariable b, float a)
        {
            var sv = b / new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));

            return sv;
        }

        public static SymbolicVariable operator /(SymbolicVariable b, double a)
        {
            var sv = b / new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));

            return sv;
        }

        public static SymbolicVariable operator /(SymbolicVariable b, short a)
        {
            var sv = b / new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));

            return sv;
        }

        public static SymbolicVariable operator /(SymbolicVariable b, long a)
        {
            var sv = b / new SymbolicVariable(a.ToString(CultureInfo.InvariantCulture));

            return sv;
        }

        #endregion

        #endregion

    }
}