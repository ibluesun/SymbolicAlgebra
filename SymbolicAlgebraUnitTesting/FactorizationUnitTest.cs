using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymbolicAlgebra;
using System;

namespace SymbolicAlgebraUnitTesting
{
    [TestClass()]
    public class FactorizationUnitTest
    {

        [TestMethod()]
        public void FactorTest()
        {
            var ooo = SymbolicVariable.FactorWithCommonFactor(SymbolicVariable.Parse("x^2+x"));
            Assert.AreEqual("x*(x+1)", ooo.ToString());

        }
    }
}
