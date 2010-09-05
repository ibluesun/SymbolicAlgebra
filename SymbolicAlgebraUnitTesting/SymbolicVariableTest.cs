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

        SymbolicVariable t = new SymbolicVariable("t");

        SymbolicVariable u = new SymbolicVariable("u");
        SymbolicVariable v = new SymbolicVariable("v");
        SymbolicVariable w = new SymbolicVariable("w");

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
            var x0 = x.RaiseToSymbolicPower(Zero);
            Assert.AreEqual(expected: "1", actual: x0.ToString());

            var x3 = x.RaiseToSymbolicPower(Three);
            Assert.AreEqual(actual: x3.ToString(), expected: "x^3");


            var r = Three.RaiseToSymbolicPower(x);
            Assert.AreEqual(actual: r.ToString(), expected:"3^x");

            var xx = x * x;
            var xx3 = Three * xx;

            var xx3y = xx3.RaiseToSymbolicPower(y);

            Assert.AreEqual(actual: xx3y.ToString(), expected: "3^y*x^(2*y)");

            var cplx = x * y * z * Four * y * z * z;

            Assert.AreEqual("4*x*y^2*z^3", cplx.ToString());

            var vpls = cplx.RaiseToSymbolicPower(Two);

            Assert.AreEqual("16*x^2*y^4*z^6", vpls.ToString());

            // testing coeffecient part with   symbol part

            var cx = x * Seven;
            Assert.AreEqual("7*x", cx.ToString());

            var cx2y = cx.RaiseToSymbolicPower((Two * y));
            Assert.AreEqual("7^(2*y)*x^(2*y)", cx2y.ToString());

        }

        [TestMethod]
        public void MultiTermSymbolicPowerTest()
        {
            var xpy = x.RaiseToSymbolicPower(y); 

            var xp2 = x.RaiseToSymbolicPower(Two);

            var xp2y = xp2 * xpy;  // x^2 * x^y == x^(2+y)

            Assert.AreEqual("x^(2+y)", xp2y.ToString());

            var xp2_y = xp2 / xpy;    // x^2 / x^y == x^(2-y)

            Assert.AreEqual("x^(2-y)", xp2_y.ToString());

            var xy = x * y;

            var xy2 = xy.RaiseToSymbolicPower(Two);

            Assert.AreEqual("x^2*y^2", xy2.ToString());

            var t = new SymbolicVariable("t");
            var xyt = xy.RaiseToSymbolicPower(t);

            Assert.AreEqual("x^t*y^t", xyt.ToString());

            var xy2t = xy2.RaiseToSymbolicPower(t);
            Assert.AreEqual("x^(2*t)*y^(2*t)", xy2t.ToString());

            var xxy2t = xy2t * x;
            Assert.AreEqual("x^(2*t+1)*y^(2*t)", xxy2t.ToString());


            var xxy2t2 = xxy2t.Power(2);
            Assert.AreEqual("x^(4*t+2)*y^(4*t)", xxy2t2.ToString());


            var xe = xxy2t + (3 * t * z);
            Assert.AreEqual("x^(2*t+1)*y^(2*t)+3*t*z", xe.ToString());


            var xe2 = xe.Power(2);
            Assert.AreEqual("x^(4*t+2)*y^(4*t)+6*t*z*x^(2*t+1)*y^(2*t)+9*t^2*z^2", xe2.ToString());

        }

        /// <summary>
        /// Test the multiplication of two powered termed
        /// </summary>
        [TestMethod]
        public void SymbolicPowerMulDivTest()
        {

            var lx = x.RaiseToSymbolicPower(y);

            var hx = x.RaiseToSymbolicPower(Three);

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

        [TestMethod]
        public void MiscTesting()
        {
            
            var t = new SymbolicVariable("t");
            var po = Two * t + One;

            Assert.AreEqual("2*t+1", po.ToString());
            
            var po2 = po + po;
            Assert.AreEqual("4*t+2", po2.ToString());

            var xp = x.RaiseToSymbolicPower(po);
            var xp2 = xp * xp;
            Assert.AreEqual("x^(4*t+2)", xp2.ToString());


            var xp2v2 = xp.Power(2);
            Assert.AreEqual("x^(4*t+2)", xp2v2.ToString());



        }

        [TestMethod]
        public void Issues2Testing()
        {
            #region main issue in division
            var a = -1 * Eight * x.Power(2) * y.Power(2);
            var b = Four * x.Power(3) * y;

            var c = b.Power(2) / a;

            Assert.AreEqual("-2*x^4", c.ToString());
            #endregion

            #region same issue in multiplication
            var u = Two * x * y;
            var v = Four * x.Power(2) * y.Power(3);

            var uv = u * v;
            Assert.AreEqual("8*x^3*y^4", uv.ToString());

            #region testing with different orders of symbols with the same value terms
            u = Two * y * x;
            v = Four * x.Power(2) * y.Power(3);

            uv = u * v;
            Assert.AreEqual("8*y^4*x^3", uv.ToString());


            u = Three * y * x * z;
            v = Five * y.Power(2) * x.Power(3) * z.Power(7);
            uv = u * v;

            Assert.AreEqual("15*y^3*x^4*z^8", uv.ToString());

            u = Three * z * x * y;
            v = Five * y.Power(2) * z.Power(3) * x.Power(7);
            uv = u * v;

            Assert.AreEqual("15*z^4*x^8*y^3", uv.ToString());
            #endregion
            #endregion


            #region division to the same value term with different orders
            var l = Eight * z.Power(3) * x.Power(2) * y.Power(5);
            var m = Two * x * y.Power(3) * z.Power(7);
            var lm = l / m;
            Assert.AreEqual("4/z^4*x*y^2", lm.ToString());

            #endregion

        }

        [TestMethod]
        public void IssuesRaisedFromIssues2Testing()
        {
            var a = Eight * x.Power(2);
            var b = Four * x.Power(4);
            var ab = a / b;
            Assert.AreEqual("2/x^2", ab.ToString());


            var rx = (x + y) / x.Power(2);   // x+y/x^2  ==  x/x^2 + y/x^2 == 1/x + y/x^2

            Assert.AreEqual(actual: rx.ToString(), expected: "1/x+y/x^2");

        }

        [TestMethod]
        public void Issues3Testing()
        {
            // x^(2*t+1)*y^(2*t)+3*t*z   in power three :S  not the same as maxima my reference program.

            var aa = x.RaiseToSymbolicPower(2 * t + One) * y.RaiseToSymbolicPower(2 * t) + 3 * t * z;

            var aa2 = aa.Power(2);

            string ex2 = "x^(4*t+2)*y^(4*t)+6*t*z*x^(2*t+1)*y^(2*t)+9*t^2*z^2";

            Assert.AreEqual(ex2, aa2.ToString());

            var aaa = aa * aa2;

            string maxima =
            "x^(6*t+3)*y^(6*t)+9*t*z*x^(4*t+2)*y^(4*t)+27*t^2*z^2*x^(2*t+1)*y^(2*t)+27*t^3*z^3";
            Assert.AreEqual(maxima, aaa.ToString());

            var aal = aa2 * aa;
            Assert.AreEqual(maxima, aal.ToString());


            var aa3 = aa.Power(3);
            Assert.AreEqual(maxima, aa3.ToString());
        }

        [TestMethod]
        public void Issues4Testing()
        {
            var a = new SymbolicVariable("-10") * y + new SymbolicVariable("50");
            var b = new SymbolicVariable("-1") * a;

            var big = x * y + x;

            var err1 = big + b;

            foreach (var at in err1.AddedTerms)
                if (at.Value.AddedTerms.Count > 0) throw new Exception("No more than sub terms required");

            var err2 = big - b;

            foreach (var at in err2.AddedTerms)
                if (at.Value.AddedTerms.Count > 0) throw new Exception("No more than sub terms required");

            Assert.AreEqual(1, 1);


        }



        [TestMethod]
        public void RemovingZeroTerms()
        {
            var a = x + y + z;
            var b = -1 * x + y + z;

            var tot = a + b;

            Assert.AreEqual(tot.ToString(), "2*y+2*z");

        }


        [TestMethod]
        public void Issue5Testing()
        {
            SymbolicVariable d = new SymbolicVariable("d");
            SymbolicVariable r = new SymbolicVariable("r");

            var a = d.RaiseToSymbolicPower(r);
            var b = d.RaiseToSymbolicPower(2*r);

            var p = x + x.Power(5);

            var c = a + b;

            Assert.AreEqual(c.ToString(), "d^r+d^(2*r)");



            
            

        }

        /// <summary>
        ///A test for InvolvedSymbols
        ///</summary>
        [TestMethod()]
        public void InvolvedSymbolsTest()
        {
            var v = x * y;
            string[] actual = {"x","y"};

            Assert.AreEqual(actual[0], v.InvolvedSymbols[0]);

            var b = 2 * u * w + v + w;

            var v2 = (x + y + z).RaiseToSymbolicPower(b);

            // this was issue and fixed and its validation here
            Assert.AreEqual("x^(2*u*w+x*y+w)+y^(2*u*w+x*y+w)+z^(2*u*w+x*y+w)", v2.ToString());

            Assert.AreEqual("x", v2.InvolvedSymbols[0]);
            Assert.AreEqual("u", v2.InvolvedSymbols[1]);
            Assert.AreEqual("w", v2.InvolvedSymbols[2]);
            Assert.AreEqual("y", v2.InvolvedSymbols[3]);
            Assert.AreEqual("z", v2.InvolvedSymbols[4]);

        }
    }
}
