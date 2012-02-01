using System;
namespace SymbolicAlgebra
{
#if SILVERLIGHT
    public partial class SymbolicVariable
#else
    public partial class SymbolicVariable : ICloneable
#endif
    {
        
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

        public SymbolicVariable Power(double power)
        {
            if (Math.Floor(power) == power) return Power((int)power);

            SymbolicVariable p = (SymbolicVariable)this.Clone();
            if (p.IsOneTerm)
            {
                // raise the coeffecient and symbol
                if (!string.IsNullOrEmpty(p.Symbol)) p.SymbolPower = power;
                p.Coeffecient = Math.Pow(p.Coeffecient, power);
            }
            else
            {
                // multi term that we can't raise it to the double
                return p.RaiseToSymbolicPower(new SymbolicVariable(power.ToString()));
            }

            return  p;
        }


        public static SymbolicVariable Pow(SymbolicVariable a, int power)
        {
            return a.Power(power);
        }

        public static SymbolicVariable Pow(SymbolicVariable a, double power)
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