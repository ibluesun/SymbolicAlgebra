using System;
namespace SymbolicAlgebra
{
    public partial class SymbolicVariable
    {
        
        /// <summary>
        /// Raise to specified power.
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        public SymbolicVariable Power(int power)
        {
            if (power == 0) return SymbolicVariable._One;

            SymbolicVariable total = this.Clone();
            int pw = Math.Abs(power);
            while (pw > 1)
            {
                if (this.IsFunction && this.FunctionName.Equals("Sqrt", StringComparison.OrdinalIgnoreCase))
                {
                    //
                    var parameterpower = power * 0.5;

                    total = this.FunctionParameters[0].Power(parameterpower);

                    pw = 0; // to end the loop
                }
                else
                {
                    total = SymbolicVariable.Multiply(total, this);
                    pw--;
                }
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

            SymbolicVariable p = this.Clone();
            if (p.IsOneTerm)
            {
                // raise the coeffecient and symbol
                if (!string.IsNullOrEmpty(p.Symbol)) p.SymbolPower = power;
                p.Coeffecient = Math.Pow(p.Coeffecient, power);
            }
            else
            {
                if (power == 0.5)
                {
                    // return sqrt function of the multi term

                    return new SymbolicVariable("Sqrt(" + p.ToString() + ")");
                }
                else if (power > 0 && power < 1)
                {
                    // I don't have solution for this now
                    throw new SymbolicException("I don't have solution for this type of power " + p.ToString() + "^ (" + power.ToString() + ")");
                }
                else
                {
                    // multi term that we can't raise it to the double
                    return p.RaiseToSymbolicPower(new SymbolicVariable(power.ToString()));
                }
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