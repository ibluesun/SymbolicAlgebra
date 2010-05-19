using SymbolicAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SymbolicAlgebraUnitTesting
{

    /// <summary>
    ///This is a test class for SymbolicVariableTest and is intended
    ///to contain all SymbolicVariableTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SymbolicVariableTest
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


        #region Testing variables
        SymbolicVariable x = new SymbolicVariable("x");

        SymbolicVariable y = new SymbolicVariable("y");

        SymbolicVariable z = new SymbolicVariable("z");

        SymbolicVariable vv = new SymbolicVariable("vv");

        SymbolicVariable vs = new SymbolicVariable("vs");

        SymbolicVariable Zero = new SymbolicVariable("0");
        SymbolicVariable One = new SymbolicVariable("1");
        SymbolicVariable Two = new SymbolicVariable("2");
        SymbolicVariable Three = new SymbolicVariable("3");
        SymbolicVariable Four = new SymbolicVariable("4");
        SymbolicVariable Five = new SymbolicVariable("5");
        SymbolicVariable Six = new SymbolicVariable("6");
        SymbolicVariable Seven = new SymbolicVariable("7");
        SymbolicVariable Eight = new SymbolicVariable("8");
        SymbolicVariable Nine = new SymbolicVariable("9");
        SymbolicVariable Ten = new SymbolicVariable("10");

        SymbolicVariable Eleven = new SymbolicVariable("11");
        SymbolicVariable Twelve = new SymbolicVariable("12");

        #endregion

        /// <summary>
        ///A test for Adds
        ///</summary>
        [TestMethod()]
        public void OperationsTest()
        {

            var pr = (Four / Two) * y * y * y / x;   //2*y^3/x

            Assert.AreEqual(pr.ToString(), "2*y^3/x");

            pr = pr - (Three * y * y / x);          // 2*y^3/x - 3*y^2/x

            Assert.AreEqual(pr.ToString(), "2*y^3/x-3*y^2/x");


            var rr = pr / (2 * y);                 // y^2/x - 1.5*y/x

            Assert.AreEqual(rr.ToString(), "y^2/x-1.5*y/x");


            var tr = rr * (x * x);                //  y^2*x - 1.5*y*x
            Assert.AreEqual(tr.ToString(), "y^2*x-1.5*y*x");


            var mx = tr + (x * y);                    // y^2*x - 0.5*y*x
            Assert.AreEqual(mx.ToString(), "y^2*x-0.5*y*x");


            mx = mx + (y * y * x);                    // 2*y^2*x - 0.5*y*x
            Assert.AreEqual(mx.ToString(), "2*y^2*x-0.5*y*x");


            var nx = tr - (y * x);                    // y^2*x - 2.5*y*x
            Assert.AreEqual(nx.ToString(), "y^2*x-2.5*y*x");


            var r = Two * (y * x * y);                // 2*y^2*x
            Assert.AreEqual(r.ToString(), "2*y^2*x");


            nx = nx - r;                              // -1*y^2*x - 2.5*y*x
            Assert.AreEqual(nx.ToString(), "-y^2*x-2.5*y*x");


            var tx = mx + nx;                         // y^2*x-3*y*x
            Assert.AreEqual(tx.ToString(), "y^2*x-3*y*x");


            var ll = Three / y;

            Assert.AreEqual(ll.ToString(), "3/y");


            ll *= x;
            Assert.AreEqual(ll.ToString(), "3/y*x");


            var h = (3 * x + 2 * y) / (2 * x);
            Assert.AreEqual(h.ToString(), "1.5+y/x");


            var a = (x + y) * (x + y);

            Assert.AreEqual(a.ToString(), "x^2+2*y*x+y^2");

            a = (x + y) * (x - y);

            Assert.AreEqual(a.ToString(), "x^2-y^2");

        }

        [TestMethod]
        public void MultiplyOneByTerms()
        {
            var term = x - y + z;
            var rRight = term * One;

            Assert.AreEqual(rRight.ToString(), "x-y+z");

            var rleft = One * term;
            Assert.AreEqual(rleft.ToString(), "x-y+z");
        }

        [TestMethod]
        public void DivisionTest()
        {

            var a = x + y;
            var b = x - y;

            // dividing by two terms
            var r = a / b;

            Assert.AreEqual(r.ToString(), "x+y/(x-y)");

            // dividing by one term
            var rx = a / x;
            Assert.AreEqual(actual: rx.ToString(), expected: "1+y/x");

            var ry = a / y;
            Assert.AreEqual(actual: ry.ToString(), expected: "x/y+1");


            // dividing one term by three terms

            var r3 = x * y / (x - y + z);

            Assert.AreEqual(r3.ToString(), "x*y/(x-y+z)");

        }

        [TestMethod]
        public void ZeroTest()
        {
            var r = x - x;

            Assert.AreEqual(r.IsZero, true);

            r = x * y - (y * x) + One;
            Assert.AreEqual(r.IsZero, false);

            r = r - One;
            Assert.AreEqual(r.IsZero, true);
        }


        [TestMethod]
        public void PowerTest()
        {
            var x_5 = (x + y).Power(-5);
            Assert.AreEqual(expected: "1/(x^5+5*y*x^4+10*y^2*x^3+10*y^3*x^2+5*y^4*x+y^5)", actual: x_5.ToString());

            var x0 = x.Power(0);
            Assert.AreEqual(expected: "1", actual: x0.ToString());

            var r = x.Power(5);
            Assert.AreEqual(r.ToString(), "x^5");

            var co = x - y;
            var ee = co.Power(3);
            Assert.AreEqual(actual: ee.ToString(), expected: "x^3-3*y*x^2+3*y^2*x-y^3");

            var re = ee / x.Power(3);

            Assert.AreEqual(actual: re.ToString(), expected: "1-3*y/x+3*y^2/x^2-y^3/x^3");

        }

        [TestMethod]
        public void SymbolicPowerTest()
        {
            var x0 = x ^ Zero;
            Assert.AreEqual(expected: "1", actual: x0.ToString());

            var x3 = x ^ Three;
            Assert.AreEqual(actual: x3.ToString(), expected: "x^3");


            var r = Three ^ x;
            Assert.AreEqual(actual: r.ToString(), expected:"3^x");

            var xx = x * x;
            var xx3 = Three * xx;

            var xx3y = xx3 ^ y;

            Assert.AreEqual(actual: xx3y.ToString(), expected: "3^y*x^(2*y)");

            var cplx = x * y * z * Four * y * z * z;

            Assert.AreEqual("4*x*y^2*z^3", cplx.ToString());

            var vpls = cplx ^ Two;

            Assert.AreEqual("16*x^2*y^4*z^6", vpls.ToString());

            // testing coeffecient part with   symbol part

            var cx = x * Seven;
            Assert.AreEqual("7*x", cx.ToString());

            var cx2y = cx ^ (Two * y);
            Assert.AreEqual("7^(2*y)*x^(2*y)", cx2y.ToString());


        }

        [TestMethod]
        public void MultiTermSymbolicPowerTest()
        {
            var xpy = x ^ y;

            var xp2 = x ^ Two;

            var xp2y = xp2 * xpy;

            Assert.AreEqual("x^(2+y)", xp2y.ToString());

            var xp2_y = xp2 / xpy;

            Assert.AreEqual("x^(2-y)", xp2_y.ToString());

            var xy = x * y;

            var xy2 = xy ^ Two;

            Assert.AreEqual("x^2*y^2", xy2.ToString());

            var t = new SymbolicVariable("t");
            var xyt = xy ^ t;

            Assert.AreEqual("x^t*y^t", xyt.ToString());

            var xy2t = xy2 ^ t;
            Assert.AreEqual("x^(2*t)*y^(2*t)", xy2t.ToString());

            var xxy2t = xy2t * x;
            Assert.AreEqual("x^(2*t+1)*y^(2*t)", xxy2t.ToString());

            var xe = xxy2t + (3 * t * z);
            Assert.AreEqual("x^(2*t+1)*y^(2*t)+3*t*z", xe.ToString());
            
            var p = new SymbolicVariable("p");

            var xe2 = xe.Power(2);
            Assert.AreEqual("x^(4*t+2)*y^(4*t)+6*t*x^(2*t+1)*y^(2*t)*z+9*t^2*z^2", xe.ToString());

        }

        /// <summary>
        /// Test the multiplication of two powered termed
        /// </summary>
        [TestMethod]
        public void SymbolicPowerMulDivTest()
        {

            var lx = x ^ y;

            var hx = x ^ Three;

            var hlx = hx * lx;

            Assert.AreEqual("x^(3+y)", hlx.ToString());
        }

        [TestMethod]
        public void IssuesTesting()
        {
            // Issue 1 when multiplying x * u^2-2*v*u+v^2  the middle term was -2*x*v because only 'v' from middle term attached to fused variables
            //   fixed with adding all fused variables from middle term.

            var a = x;

            var b = new SymbolicVariable("u") - new SymbolicVariable("v");

            var c = b.Power(2);

            var r = a * c;

            Assert.AreEqual(r.ToString(), "x*u^2-2*x*v*u+x*v^2");

            // Issue 2 when dividing x / u^2-2*v*u+v^2  the number changed.
            r = 1 / c;

            Assert.AreEqual(r.ToString(), "1/(u^2-2*v*u+v^2)");

        }
    }
}
