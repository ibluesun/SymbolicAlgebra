using SymbolicAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics;

namespace SymbolicAlgebraUnitTesting
{
    [TestClass()]
    public class SymbolicVariableSolveTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// Information about and functionality for the current test run.
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        
        [TestMethod()]
        public void SolveTest()
        {
            var ss = SymbolicVariable.Parse("3*x-4");

            Assert.AreEqual(4.0 / 3, ss.Solve());

            var r = SymbolicVariable.Parse("5-3*x");
            Assert.AreEqual(5.0 / 3.0, r.Solve());
        }

        [TestMethod]
        public void SolveVariableTest()
        {
            var h = SymbolicVariable.Parse("2*x+4*y-4");
            var hs = h.Solve("x");

            Assert.AreEqual("-2*y+2", hs.ToString());

            var v = SymbolicVariable.Parse("5*u-4/r+3*h-2*x");

            Assert.AreEqual("2.5*u-2/r+1.5*h", v.Solve("x").ToString());


            Assert.AreEqual("-1.66666666666667*u+1.33333333333333/r+0.666666666666667*x", v.Solve("h").ToString());
            
        }
    }
}
