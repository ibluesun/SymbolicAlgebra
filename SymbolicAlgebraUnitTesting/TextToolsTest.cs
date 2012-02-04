using SymbolicAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SymbolicAlgebraUnitTesting
{
    
    
    /// <summary>
    ///This is a test class for TextToolsTest and is intended
    ///to contain all TextToolsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TextToolsTest
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


        /// <summary>
        ///A test for ComaSplit
        ///</summary>
        [TestMethod()]
        public void ComaSplitTest()
        {
            var f = TextTools.ComaSplit("r, t,  sun ( e, f, g(4,5),2),o,p");
            Assert.AreEqual("r", f[0]);
            Assert.AreEqual("t", f[1]);
            Assert.AreEqual("sun(e,f,g(4,5),2)", f[2]);
            Assert.AreEqual("o", f[3]);
            Assert.AreEqual("p", f[4]);

        }
    }
}
