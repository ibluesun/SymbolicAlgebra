using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolicAlgebra
{
    public class ExtraTerm
    {
        public SymbolicVariable Term;
        public bool Negative;

        public ExtraTerm Clone()
        {
            return new ExtraTerm { Term = this.Term.Clone(), Negative = this.Negative };
        }
    }
}
