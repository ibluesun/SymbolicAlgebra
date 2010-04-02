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
            SymbolicVariable total = (SymbolicVariable)this.Clone();
            while (power > 1)
            {
                total = total * this;
                power--;
            }


            return total;
        }



        public static SymbolicVariable Power(SymbolicVariable a, int power)
        {
            return a.Power(power);
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
    }
}