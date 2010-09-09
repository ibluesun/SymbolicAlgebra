using System;
namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
    {
        
        /// <summary>
        /// Notice that the any group of charachters you enter will be treated as a whole token.
        /// This is like "Sin(x)" "R_Y" etc.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static SymbolicVariable Parse(string symbol)
        {
            return new SymbolicVariable(symbol);
        }



        /// <summary>
        /// Raise to specified power.
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public SymbolicVariable Power(int power)
        {
            if (power == 0) return SymbolicVariable._One;

            SymbolicVariable total = (SymbolicVariable)this.Clone();
            int pw = Math.Abs(power);
            while (pw > 1)
            {
                total = SymbolicVariable.Multiply(total, this);
                pw--;
            }

            if (power < 0)
            {
                total = SymbolicVariable.Divide(SymbolicVariable._One, total);
            }

            return total;
        }



        public static SymbolicVariable Power(SymbolicVariable a, int power)
        {
            return a.Power(power);
        }

        public static SymbolicVariable  operator +(SymbolicVariable a, SymbolicVariable b)
        {
            return SymbolicVariable.Add(a, b);
        }

        public static SymbolicVariable operator -(SymbolicVariable a, SymbolicVariable b)
        {
            return SymbolicVariable.Subtract(a, b);
        }

        public static SymbolicVariable operator *(SymbolicVariable a, SymbolicVariable b)
        {
            return SymbolicVariable.Multiply(a, b);
        }

        public static SymbolicVariable operator /(SymbolicVariable a, SymbolicVariable b)
        {
            return SymbolicVariable.Divide(a, b);
        }
    }
}