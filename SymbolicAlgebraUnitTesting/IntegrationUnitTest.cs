using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymbolicAlgebra;
using System;

namespace SymbolicAlgebraUnitTesting
{
    [TestClass()]
    public class IntegrationUnitTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [TestMethod]
        public void SimpleIntegrationTest()
        {
            var a = SymbolicVariable.Parse("2*x");
            var ia = a.Integrate("x");

            Assert.AreEqual("x^2", ia.ToString());

            a = SymbolicVariable.Parse("2*x*y");

            ia = a.Integrate("x");
            Assert.AreEqual("x^2*y", ia.ToString());

            ia = a.Integrate("y");
            Assert.AreEqual("x*y^2", ia.ToString());


            //////
            ///
            a = SymbolicVariable.Parse("2*y*x");

            ia = a.Integrate("x");
            Assert.AreEqual("y*x^2", ia.ToString());

            ia = a.Integrate("y");
            Assert.AreEqual("y^2*x", ia.ToString());


        }

        public void LogIntegrationTest()
        {
            var a = SymbolicVariable.Parse("log(x)");
            var ia = a.Integrate("x");
            Assert.AreEqual("log(x)*x-x", ia.ToString());


            a = SymbolicVariable.Parse("log(x+1)");
            ia = a.Integrate("x");
            Assert.AreEqual("(x+1)*log(x+1)-x-1", ia.ToString());

            a = SymbolicVariable.Parse("log(x^2)");
            ia = a.Integrate("x");
            Assert.AreEqual("2*x*log(x)-2*x", ia.ToString());

            a = SymbolicVariable.Parse("log(x^2+x)");
            ia = a.Integrate("x");
            Assert.AreEqual("still working on it", ia.ToString());


            a = SymbolicVariable.Parse("log(x^2)");
            ia = a.Integrate("y");                   // different parameter
            Assert.AreEqual("2*log(x)*y", ia.ToString());


        }
    }
}
