using SymbolicAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics;

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


        SymbolicVariable CosFunction = new SymbolicVariable("Cos(x)");
        SymbolicVariable UxyFunction = new SymbolicVariable("U(x,y)");
        SymbolicVariable VFuntion = new SymbolicVariable("m:V(m)");
        SymbolicVariable EmptyFuntion = new SymbolicVariable("Empty()");



        #endregion


        const string lnText = "log";

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

            Assert.AreEqual(r.ToString(), "(x+y)/(x-y)");

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

            var xp2_y = SymbolicVariable.Divide( xp2 , xpy);    // x^2 / x^y == x^(2-y)

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
            var u = Two * x * y;                        //2*x*y
            var v = Four * x.Power(2) * y.Power(3);     //4*x^2*y^3

            var uv = u * v;
            Assert.AreEqual("8*x^3*y^4", uv.ToString());

            #region testing with different orders of symbols with the same value terms
            u = Two * y * x;                        // 2*y*x
            v = Four * x.Power(2) * y.Power(3);     // 4*x^2*y^3

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

            var b = 2 * u * w + v + w;

            var tv1 = x.RaiseToSymbolicPower(b);

            Assert.AreEqual("x^(2*u*w+v+w)", tv1.ToString());

            var tv2 = (x + y + z).RaiseToSymbolicPower(b);

            // this was issue and fixed and its validation here
            Assert.AreEqual("(x+y+z)^(2*u*w+v+w)", tv2.ToString());

            var tv3 = tv2.Power(2);
            Assert.AreEqual("(x+y+z)^(4*u*w+2*v+2*w)", tv3.ToString());

            var tv4 = tv2.RaiseToSymbolicPower(3 * x);
            Assert.AreEqual("(x+y+z)^(6*u*w*x+3*v*x+3*w*x)", tv4.ToString());

            string[] gg = tv4.InvolvedSymbols;


            Assert.AreEqual("u", gg[0]);
            Assert.AreEqual("v", gg[1]);
            Assert.AreEqual("w", gg[2]);
            Assert.AreEqual("x", gg[3]);
            Assert.AreEqual("y", gg[4]);
            Assert.AreEqual("z", gg[5]);

            var o = new SymbolicVariable("o");
            var pp = o.RaiseToSymbolicPower(x);

            string[] pp_p = pp.InvolvedSymbols;
            Assert.AreEqual("o", pp_p[0]);
            Assert.AreEqual("x", pp_p[1]);

            var kk = Two;
            var kk_i = kk.InvolvedSymbols;
            Assert.AreEqual(0, kk_i.Length);

        }


        [TestMethod]
        public void ExtraPowerTesting()
        {
            var a = (x + y + z).RaiseToSymbolicPower(3 * x);

            var b = (x + y + z).RaiseToSymbolicPower(y);

            var a_b = a + b;

            Assert.AreEqual("(x+y+z)^(3*x)+(x+y+z)^y", a_b.ToString());

            var rs = y + 3 * x;
            var ls = 3 * x + y;

            Assert.AreEqual(rs.Equals(ls), true);

            var ot = x.RaiseToSymbolicPower(v) + x.RaiseToSymbolicPower(z) + x.RaiseToSymbolicPower(y);

            Assert.AreEqual("x^v+x^z+x^y", ot.ToString());

            var c = SymbolicVariable.Multiply(a_b, a_b);

            Assert.AreEqual("(x+y+z)^(6*x)+2*(x+y+z)^(y+3*x)+(x+y+z)^(2*y)", c.ToString());

        }


        /// <summary>
        ///A test for Diff
        ///</summary>
        [TestMethod()]
        public void DiffTest()
        {
            var a = x.Power(2).Differentiate("x");
            Assert.AreEqual("2*x", a.ToString());

            var b = (x.Power(2) + x.Power(3) + x.Power(4)).Differentiate("x");
            Assert.AreEqual("2*x+3*x^2+4*x^3", b.ToString());

            var c = (x.Power(2) * y.Power(3) * z.Power(4));

            var dc_z = c.Differentiate("z");

            Assert.AreEqual("4*x^2*y^3*z^3", dc_z.ToString());

            var d = (x.Power(3) * y.Power(-40))+(y.Power(3)*z.Power(-1));
            var dd_y = d.Differentiate("y");

            Assert.AreEqual("-40*x^3/y^41+3/z*y^2", dd_y.ToString());

            var e = x.RaiseToSymbolicPower(2*y);
            var de_x = e.Differentiate("x");

            Assert.AreEqual("2*x^(2*y-1)*y", de_x.ToString());

            var f = Two * x.RaiseToSymbolicPower(3 * z) * y.RaiseToSymbolicPower(2 * z);
            var df_x = f.Differentiate("y");

            Assert.AreEqual("4*x^(3*z)*y^(2*z-1)*z", df_x.ToString());

            var g = Two * y + 3 * x.RaiseToSymbolicPower(z) - 5 * x.RaiseToSymbolicPower(y - One);
            var dg_x = g.Differentiate("x");

            Assert.AreEqual("3*x^(z-1)*z-5*x^(y-2)*y+5*x^(y-2)", dg_x.ToString());
            
            var dfive_x = Five.Differentiate("u");
            Assert.AreEqual("0", dfive_x.ToString());

            var h = x.Power(3)+ x.RaiseToSymbolicPower(Two) + x; //x^2+x
            var dh_x = h.Differentiate("x");

            Assert.AreEqual("3*x^2+2*x+1", dh_x.ToString());

            var i = 18 * x * y.Power(3);
            var di_x = i.Differentiate("x");
            Assert.AreEqual("18*y^3", di_x.ToString());

            var j = 18 * x * y * z.Power(2);
            var dj_y = j.Differentiate("y");
            Assert.AreEqual("18*x*z^2", dj_y.ToString());

            var k = 18 * x.Power(2) + y.Power(3) * x + 2 * x * y;
            var dk_y = k.Differentiate("y");

            Assert.AreEqual("3*x*y^2+2*x", dk_y.ToString());
        }


        [TestMethod]
        public void ParseTest()
        {
            var s = SymbolicVariable.Parse("2*x+3*x");

            Assert.AreEqual(5.0, s.Coeffecient);

            var c = SymbolicVariable.Parse("2*x*y^3-3*x^2*y^2+6*x-3+5*x+6*x-4*y^x");

            Assert.AreEqual("2*x*y^3-3*x^2*y^2+17*x-3-4*y^x", c.ToString());

            var h = SymbolicVariable.Parse("3*(x+3*x)-4*y");
            Assert.AreEqual("12*x-4*y", h.ToString());

            var v = SymbolicVariable.Parse("sin(x)^2+cos(x)^2");

            var g = SymbolicVariable.Parse("sin(x^2)+cos(x^3)");

            var ns = SymbolicVariable.Parse("-6^w");
            Assert.AreEqual("-6^w", ns.ToString());

        }



        [TestMethod]
        public void FuncDiscoveryTest()
        {
            Assert.AreEqual(true, CosFunction.IsFunction);
            Assert.AreEqual(false, x.IsFunction);
            Assert.AreEqual(true, UxyFunction.IsFunction);

            Assert.AreEqual(true, VFuntion.IsFunction);

            Assert.AreEqual(true, EmptyFuntion.IsFunction);


            var cf = SymbolicVariable.Parse("sum(o, sum(a,b))");
            var g = cf.GetFunctionParameters();
            Assert.AreEqual("o", g[0].ToString());
            Assert.AreEqual("sum(a,b)", g[1].ToString());


            
        }

        [TestMethod]
        public void SpecialFunctionsDifftest()
        {
            var sin = new SymbolicVariable("sin(x)");
            var cos = new SymbolicVariable("cos(x)");
            var tan = new SymbolicVariable("tan(x)");
            var sec = new SymbolicVariable("sec(x)");
            var csc = new SymbolicVariable("csc(x)");
            var cot = new SymbolicVariable("cot(x)");

            Assert.AreEqual("cos(x)", sin.Differentiate("x").ToString());
            Assert.AreEqual("-sin(x)", cos.Differentiate("x").ToString());
            Assert.AreEqual("sec(x)^2", tan.Differentiate("x").ToString());
            Assert.AreEqual("sec(x)*tan(x)", sec.Differentiate("x").ToString());
            Assert.AreEqual("-csc(x)*cot(x)", csc.Differentiate("x").ToString());
            Assert.AreEqual("-csc(x)^2", cot.Differentiate("x").ToString());

            var sinh = new SymbolicVariable("sinh(x)");
            var cosh = new SymbolicVariable("cosh(x)");
            var tanh = new SymbolicVariable("tanh(x)");
            var sech = new SymbolicVariable("sech(x)");
            var csch = new SymbolicVariable("csch(x)");
            var coth = new SymbolicVariable("coth(x)");

            Assert.AreEqual("cosh(x)", sinh.Differentiate("x").ToString());
            Assert.AreEqual("sinh(x)", cosh.Differentiate("x").ToString());
            Assert.AreEqual("sech(x)^2", tanh.Differentiate("x").ToString());
            Assert.AreEqual("-sech(x)*tanh(x)", sech.Differentiate("x").ToString());
            Assert.AreEqual("-csch(x)*coth(x)", csch.Differentiate("x").ToString());
            Assert.AreEqual("-csch(x)^2", coth.Differentiate("x").ToString());


            var complexsin = SymbolicVariable.Parse("cos(x^2+x^2+x^2)");
            Assert.AreEqual("cos(3*x^2)", complexsin.ToString());

            Assert.AreEqual("-6*x*sin(3*x^2)", complexsin.Differentiate("x").ToString());


            var log = new SymbolicVariable(lnText + "(x^6*y^3)");

            Assert.AreEqual("6/x", log.Differentiate("x").ToString());

            Assert.AreEqual("3/y", log.Differentiate("y").ToString());

        }

        [TestMethod]
        public void SpecialFunctionHigherPowerTest()
        {
            var sin = new SymbolicVariable("sin(x)");
            var sin2y = sin.RaiseToSymbolicPower(SymbolicVariable.Parse("2-y"));
            Assert.AreEqual("sin(x)^(2-y)", sin2y.ToString());
            Assert.AreEqual("2*cos(x)*sin(x)^(1-y)-cos(x)*sin(x)^(1-y)*y", sin2y.Differentiate("x").ToString());

            sin = SymbolicVariable.Parse("2*sin(x)");
            Assert.AreEqual("2*cos(x)", sin.Differentiate("x").ToString());

        }




        [TestMethod]
        public void Issues5Testing()
        {
            var p = SymbolicVariable.Multiply( One, SymbolicVariable.Parse("2^x"));

            Assert.AreEqual("2^x", p.ToString());


            var p2 = SymbolicVariable.Multiply(SymbolicVariable.Parse("2^x"), One);
            Assert.AreEqual("2^x", p2.ToString());


            var xx = SymbolicVariable.Parse("x^2");
            var roro = SymbolicVariable.Multiply(p, xx);
            Assert.AreEqual("2^x*x^2", roro.ToString());

            var g = SymbolicVariable.Multiply(new SymbolicVariable(lnText + "(2)"), SymbolicVariable.Parse("2^x"));
            Assert.AreEqual(lnText + "(2)*2^x", g.ToString());

            g = SymbolicVariable.Multiply( g , SymbolicVariable.Parse("2^y"));
            Assert.AreEqual(lnText + "(2)*2^(x+y)", g.ToString());

            g = SymbolicVariable.Multiply(g, SymbolicVariable.Parse("3^y"));
            Assert.AreEqual(lnText + "(2)*2^(x+y)*3^y", g.ToString());

            g = SymbolicVariable.Multiply(g, SymbolicVariable.Parse("3^z"));
            Assert.AreEqual(lnText + "(2)*2^(x+y)*3^(y+z)", g.ToString());
        }


        [TestMethod]
        public void BaseMultiplicationTest()
        {
            var fp = SymbolicVariable.Parse("2^x");
            var sp = SymbolicVariable.Parse("2^y");

            var g = SymbolicVariable.Multiply(fp, sp);

            Assert.AreEqual("2^(x+y)", g.ToString());


            var fl = SymbolicVariable.Parse("3^x");
            g = SymbolicVariable.Multiply(fp, fl);
            Assert.AreEqual("2^x*3^x", g.ToString());

            g = SymbolicVariable.Multiply(g, SymbolicVariable.Parse("3^y"));
            Assert.AreEqual("2^x*3^(x+y)", g.ToString());

            g = SymbolicVariable.Multiply(g, SymbolicVariable.Parse("2^z"));
            Assert.AreEqual("2^(x+z)*3^(x+y)", g.ToString());


            g = SymbolicVariable.Multiply(g, SymbolicVariable.Parse("2^v"));
            Assert.AreEqual("2^(x+z+v)*3^(x+y)", g.ToString());

            g = SymbolicVariable.Multiply(g, SymbolicVariable.Parse("3^u"));
            Assert.AreEqual("2^(x+z+v)*3^(x+y+u)", g.ToString());

            
            var ns = SymbolicVariable.Parse("-6.6^w");
            Assert.AreEqual("-6.6^w", ns.ToString());

            g = SymbolicVariable.Multiply(g, ns);
            Assert.AreEqual("2^(x+z+v)*3^(x+y+u)*-6.6^w", g.ToString());

        }


        [TestMethod]
        public void Issues6Testing()
        {
            var px = SymbolicVariable.Parse("3^x");
            var tw7 = SymbolicVariable.Parse("27");

            var g = SymbolicVariable.Multiply(px, tw7);

            Assert.AreEqual("3^(x+3)", g.ToString());

            g = SymbolicVariable.Multiply(g, new SymbolicVariable("30"));
            Assert.AreEqual("3^(x+3)*30", g.ToString());

        }


        [TestMethod]
        public void BaseVariablePowerVariableDiffTest()
        {
            var p = SymbolicVariable.Parse("u^(x^2)");
            var g= p.Differentiate("x");
            Assert.AreEqual("2*"+ lnText + "(u)*x*u^(x^2)", g.ToString());
        }

        [TestMethod]
        public void Issues7Testing()
        {
            var s = new SymbolicVariable("---  4 5");
            Assert.AreEqual("-45", s.ToString());

            s = new SymbolicVariable("--- + sin (3- 4 + t * 9) + 4 - 5");
            Assert.AreEqual("-sin(3-4+t*9)+4-5", s.ToString());
        }

        [TestMethod]
        public void Issues8Testing()
        {
            //3^(2*x)*3
            var t2x = SymbolicVariable.Parse("3^(2*x)");
            var g = SymbolicVariable.Multiply(t2x, Three);
            Assert.AreEqual("3^(2*x+1)", g.ToString());

            var tx5 = SymbolicVariable.Parse("3^(x-5)");
            g = SymbolicVariable.Multiply(Eleven, tx5);
            Assert.AreEqual("11*3^(x-5)", g.ToString());

            g = SymbolicVariable.Multiply(new SymbolicVariable("27"), tx5); // 3^3*3^(x-5)
            Assert.AreEqual("3^(-2+x)", g.ToString());

            var fvx = SymbolicVariable.Parse("5^x");
            g = SymbolicVariable.Multiply(Five, fvx);

            Assert.AreEqual("5^(1+x)", g.ToString());

            var cst = new SymbolicVariable("-cos(t)");
            g = SymbolicVariable.Multiply(t, cst);
            Assert.AreEqual("-t*cos(t)", g.ToString());

        }


        [TestMethod]
        public void DifferentiateMultpliedSymbols()
        {
            var p = SymbolicVariable.Parse("x^2*y^(2*x)");
            var g = p.Differentiate("x");

            Assert.AreEqual("2*y^(2*x)*x+2*x^2*"+ lnText + "(y)*y^(2*x)", g.ToString());

            p = SymbolicVariable.Parse("2^(x^2)*x^3");
            g = p.Differentiate("x");
            Assert.AreEqual("x^4*log(2)*2^(1+x^2)+2^x^2*x^2*3", g.ToString());

            p = SymbolicVariable.Parse("sin(x)*cos(x)");
            g = p.Differentiate("x");

            Assert.AreEqual("cos(x)^2-sin(x)^2", g.ToString());


            p = SymbolicVariable.Parse("sin(2*x*y)*cos(x)");
            g = p.Differentiate("x");

            Assert.AreEqual("2*cos(x)*y*cos(2*x*y)-sin(2*x*y)*sin(x)", g.ToString());

            g = p.Differentiate("y");
            Assert.AreEqual("2*cos(x)*x*cos(2*x*y)", g.ToString());

            p = SymbolicVariable.Parse(lnText + "(x^(2*f*t)*y^(5*t))");
            Assert.AreEqual("2*f*t*log(x)+5*t*log(y)", p.ToString());

            g = p.Differentiate("t");
            Assert.AreEqual("2*f*log(x)+5*log(y)", g.ToString());

            g = p.Differentiate("x");
            Assert.AreEqual("2*f*t/x", g.ToString());

            g = p.Differentiate("y");
            Assert.AreEqual("5*t/y", g.ToString());

        }


        [TestMethod()]
        public void InvolvedSymbolsTest2()
        {
            var p = SymbolicVariable.Parse("2^(2*f*t)*t^(8*i*u)*g^(7^(h*l)*s*c)");
            Assert.AreEqual(9, p.InvolvedSymbols.Length);

            Assert.AreEqual("x", CosFunction.InvolvedSymbols[0]);

            var fon = SymbolicVariable.Parse("2*sin(x)*sin(cos(y))*log(z)+ g(f(t(u)))");
            Assert.AreEqual(4, fon.InvolvedSymbols.Length);

            var su = new SymbolicVariable("sum(x,y)");
            Assert.AreEqual(2, su.InvolvedSymbols.Length);

            var sq = SymbolicVariable.Parse("2*x*sqrt(u,G(F(v,c,x)))*l");
            Assert.AreEqual(5, sq.InvolvedSymbols.Length);

            var tt = SymbolicVariable.Parse("5*sin(x)");
            Assert.AreEqual("x", tt.InvolvedSymbols[0]);

            var popo = SymbolicVariable.Parse("sin(3*x)");
            Assert.AreEqual("x", popo.InvolvedSymbols[0]);

        }

        /// <summary>
        ///A test for ParseLambda
        ///</summary>
        [TestMethod()]
        public void ParseLambdaTest()
        {
            var TwoX = SymbolicVariable.Parse("x*x");

            Assert.AreEqual(16, TwoX.Execute(4));

            var pp = SymbolicVariable.Parse("2*x^60*x*y+z-3^u");

            List<Tuple<string, double>> foo2 = new List<Tuple<string, double>>(4);
            var x = new Tuple<string, double>("x", 0);
            var y= new Tuple<string, double>("y", 9);
            var z =new Tuple<string, double>("z", 8);
            var u =new Tuple<string, double>("u", 7);

            Assert.AreEqual(-2179.0, pp.Execute(y, x, z, u));

            double v = CosFunction.Execute(3.14159265358979);

            Assert.AreEqual(-1.0, v);

            v = SymbolicVariable.Parse("asin(x)").Execute(0.03);
            Assert.AreEqual(Math.Asin(0.03), v);

            v = SymbolicVariable.Parse("exp(x)*sin(x)").Execute(2);
            Assert.AreEqual(Math.Exp(2) * Math.Sin(2), v);

            v = SymbolicVariable.Parse("5*sin(x)").Execute(5);
            Assert.AreEqual(5 * Math.Sin(5), v);

            v = SymbolicVariable.Parse("20*cos(x)*sin(x)^3+x").Execute(3);
            Assert.AreEqual(20 * Math.Cos(3) * Math.Pow(Math.Sin(3), 3) + 3, v);

            v = SymbolicVariable.Parse("20*cos(x)*sin(x)^3-x").Execute(3);
            Assert.AreEqual(20 * Math.Cos(3) * Math.Pow(Math.Sin(3), 3) - 3, v);

            
            //v = SymbolicVariable.Parse("7*sin(x)*log(x)").Execute(3);
            //Assert.AreEqual(7 * Math.Sin(3) * Math.Log(3), v);
            
            v = SymbolicVariable.Parse("-1*x^2").Execute(8);
            Assert.AreEqual(-64, v);

            v = SymbolicVariable.Parse("sin(3*x)").Execute(2);
            Assert.AreEqual(Math.Sin(3 * 2), v);

        }

        [TestMethod()]
        public void ParseLambdaPerformanceTest()
        {
            var r = SymbolicVariable.Parse("3*sin(x)*x^3");
            r.Execute(0);

            Dictionary<string, double> xdic = new Dictionary<string, double>(1);

            xdic.Add("x", 0);

            int times = 100000;

            // Test using dictionary
            int t0 = Environment.TickCount;
            for (int i = 0; i < times; i++)
            {
                r.Execute(xdic);
                xdic["x"] = xdic["x"]++;
            }

            int tElapsed = Environment.TickCount - t0;

            Trace.WriteLine(string.Format("Dictionary One Parameter Elapsed Time {0}", tElapsed));

            // test without dictionary
            double pizo = 0;
            t0 = Environment.TickCount;
            for (int i = 0; i < times; i++)
            {
                r.Execute(pizo);
                pizo++;
            }

            tElapsed = Environment.TickCount - t0;
            Trace.WriteLine(string.Format("Native Offset One Parameter Elapsed Time {0}", tElapsed));
        }



        [TestMethod]
        public void Issues9Testing()
        {
            // the following issues has been fixed by using ExtraTerms in Symbolic Variable
            // Extra Term include the terms that is not divided by one 

            var v = SymbolicVariable.Parse("x+2/(x+8)");
            Assert.AreEqual("x+2/(x+8)", v.ToString());

            v = SymbolicVariable.Parse("x/(x+8)+x");
            Assert.AreEqual("x/(x+8)+x", v.ToString());

            v = SymbolicVariable.Parse("0 - 1/(1+x)");
            Assert.AreEqual("-1/(1+x)", v.ToString());

            v = SymbolicVariable.Parse("+6/(z*x*y)-(2+u+v)/(x+y)");
            Assert.AreEqual("6/z/x/y-(2+u+v)/(x+y)", v.ToString());


            v = SymbolicVariable.Parse("2/(x+2) + y/(x+2)");
            Assert.AreEqual("(2+y)/(x+2)", v.ToString());

            v = SymbolicVariable.Parse("2/(x+2) - y/(x+2)");
            Assert.AreEqual("(2-y)/(x+2)", v.ToString());

            v = SymbolicVariable.Parse("(1/(y+x)+2/y)*8");
            Assert.AreEqual("8/(y+x)+16/y", v.ToString());

            v = SymbolicVariable.Divide(v, Eight);
            Assert.AreEqual("1/(y+x)+2/y", v.ToString());

            v = SymbolicVariable.Parse("(2/(x+y)^2)|x");
            Assert.AreEqual("(-4*x-4*y)/(x^4+4*y*x^3+6*y^2*x^2+4*y^3*x+y^4)", v.ToString());

            v = SymbolicVariable.Parse("(2+3/x+5/(x-1))|x");
            Assert.AreEqual("-3/x^2-5/(x^2-2*x+1)", v.ToString());
        }

        [TestMethod]
        public void Issues10Testing()
        {
            var v = SymbolicVariable.Parse("4+5*t");
            v = v.Power(0.5);

            Assert.AreEqual("Sqrt(4+5*t)", v.ToString());


            v = v.Power(2);
            Assert.AreEqual("4+5*t", v.ToString());

            v = SymbolicVariable.Parse("sqrt(8+x)*sqrt(8+x)");
            Assert.AreEqual("8+x", v.ToString());
            
            v = SymbolicVariable.Parse("sqrt(5*x^2)*6*sqrt(x^2*5)");
            Assert.AreEqual("30*x^2", v.ToString());


            v = SymbolicVariable.Parse("(6*sqrt(x*2)+4*sqrt(x+x))*sqrt(2*x)");
            Assert.AreEqual("20*x", v.ToString());


        }

        [TestMethod]
        public void Issues11Testing()
        {
            var v = SymbolicVariable.Parse("acos(x^2+y^2)");
            Assert.AreEqual("-2*x/sqrt(1-x^4-2*y^2*x^2-y^4)", v.Differentiate("x").ToString());

            v = SymbolicVariable.Parse("z*x/(x^2+y^2+z^2)^(1.5)");
            Assert.AreEqual("z*x/(x^2+y^2+z^2)^(1.5)", v.ToString());

            v = SymbolicVariable.Parse("acos(z/sqrt(x^2+y^2+z^2))");
            Assert.AreEqual("z*x/(x^2+y^2+z^2)^(1.5)/sqrt(1-z^2/(x^2+y^2+z^2))", v.Differentiate("x").ToString());

        }

        [TestMethod]
        public void LogSimplificationTest()
        {
            var v = new SymbolicVariable("log(1)");
            Assert.AreEqual("0", v.ToString());

            v = new SymbolicVariable("log(exp(1))");
            Assert.AreEqual("1", v.ToString());

            v = new SymbolicVariable("log(exp(x^2/4))");
            Assert.AreEqual("0.25*x^2", v.ToString());

            v = new SymbolicVariable("log(x*y)");
            Assert.AreEqual("log(x)+log(y)", v.ToString());

            v = new SymbolicVariable("log(2*x*y)");
            Assert.AreEqual("log(2)+log(x)+log(y)", v.ToString());

            v = new SymbolicVariable("log(2*x)");
            Assert.AreEqual("log(2)+log(x)", v.ToString());

            v = new SymbolicVariable("log(y^x)");
            Assert.AreEqual("x*log(y)", v.ToString());

            v = new SymbolicVariable("log(y^(2*x+3))");
            Assert.AreEqual("2*x*log(y)+3*log(y)", v.ToString());

            v = new SymbolicVariable("log(3^y*i^5)");
            Assert.AreEqual("y*log(3)+5*log(i)", v.ToString());

            v = new SymbolicVariable("log(3^y/i^5)");
            Assert.AreEqual("y*log(3)-5*log(i)", v.ToString());


            v = new SymbolicVariable("log(3^y-4^u)");
            Assert.AreEqual("log(3^y-4^u)", v.ToString());
        }

        [TestMethod]
        public void Issues12Test()
        {
            // it is about addition and subtraction of coefficients with different or the same powers.

            var v = SymbolicVariable.Parse("3^y-4^u");
            Assert.AreEqual("3^y-4^u", v.ToString());

            v = SymbolicVariable.Parse("3^y-4^y");
            Assert.AreEqual("3^y-4^y", v.ToString());

            v = SymbolicVariable.Parse("3^y+4^y");
            Assert.AreEqual("3^y+4^y", v.ToString());

            v = SymbolicVariable.Parse("3^y+4^u");
            Assert.AreEqual("3^y+4^u", v.ToString());

            v = SymbolicVariable.Parse("3^u-5^x-3^u");
            Assert.AreEqual("-5^x", v.ToString());

            v = SymbolicVariable.Parse("3^u-5^x-3^u+6^x+2^x+5^y");
            Assert.AreEqual("-5^x+6^x+2^x+5^y", v.ToString());

            var pp = SymbolicVariable.Parse("2*(1-3*x)^cos(x)");   //issue is that parser consider the (1-3*x) is a whole parameter which is wrong

            Assert.AreEqual(pp.InvolvedSymbols.Length, 1);
            Assert.AreEqual(pp.InvolvedSymbols[0], "x");

        }

        [TestMethod]
        public void LogarithmicDifferentiationTest()
        {
            
            //LOGARITHMIC DIFFERENTIATION  was not correct

            var v = SymbolicVariable.Parse("x^x");
            v = v.Differentiate("x");
            Assert.AreEqual("x^x*log(x)+x^x", v.ToString());

            v = SymbolicVariable.Parse("2*x^(3*x^2)");
            v = v.Differentiate("x");
            Assert.AreEqual("12*x^(3*x^2+1)*log(x)+6*x^(3*x^2+1)", v.ToString());

            v = SymbolicVariable.Parse("x^exp(x)");
            v = v.Differentiate("x");
            Assert.AreEqual("x^(exp(x))*log(x)*exp(x)+x^(exp(x)-1)*exp(x)", v.ToString());

            v = SymbolicVariable.Parse("(1-3*x)^cos(x)");
            v = v.Differentiate("x");
            Assert.AreEqual("-(1-3*x)^(cos(x))*log((1-3*x))*sin(x)-3*(1-3*x)^(cos(x))*cos(x)/(1-3*x)", v.ToString());


            v = SymbolicVariable.Parse("log(2*x^(3*x^2))");
            v = v.Differentiate("x");
            Assert.AreEqual("6*log(x)*x+3*x", v.ToString());

        }

        [TestMethod]
        public void Issues13Test()
        {
            var v = new SymbolicVariable("log(sin(x)^x)");

            Assert.AreEqual("x*log(sin(x))", v.ToString());
            var dv = v.Differentiate("x");

            Assert.AreEqual("log(sin(x))+x*cos(x)/sin(x)", dv.ToString());
            v = SymbolicVariable.Parse("sin(x)^x");

            dv = v.Differentiate("x");
            Assert.AreEqual("sin(x)^x*log(sin(x))+sin(x)^(x-1)*x*cos(x)", dv.ToString());

        }

        [TestMethod]
        public void PowerRightAssociativity()
        {
            var d = SymbolicVariable.Parse("a^b^c");
            Assert.AreEqual("a^(b^c)", d.ToString());

            var r = d.Execute(3, 2, 4);
            Assert.AreEqual(43046721.0, r);
        }

        [TestMethod]
        public void Issues14Test()
        {
            // operations on zero when muliplied or divided should return zero 
            var bb = SymbolicVariable.Parse("0/(x+3)");
            Assert.AreEqual("0", bb.ToString());

            bb = SymbolicVariable.Parse("(2*cos(theta)^2*r^2*sin(theta)^2+sin(theta)^4*r^2+cos(theta)^4*r^2)/(2*cos(theta)^2*r^2*sin(theta)^2+sin(theta)^4*r^2+cos(theta)^4*r^2)");
            Assert.AreEqual("1", bb.ToString());

            var gg = SymbolicVariable.Parse("(cos(theta)^2+sin(theta)^2)/(2*cos(theta)^2*r^2*sin(theta)^2+sin(theta)^4*r^2+cos(theta)^4*r^2)");
            var cc = Zero * gg;
            Assert.AreEqual("0", cc.ToString());

            var kk = SymbolicVariable.Parse("(cos(theta)^2+sin(theta)^2)/(2*cos(theta)^2*r^2*sin(theta)^2+sin(theta)^4*r^2+cos(theta)^4*r^2)");
            var ll = One * kk;
            Assert.AreEqual(kk.ToString(), ll.ToString());

            var we = SymbolicVariable.Parse("5*sin(x)^4^x");
            var rs = we.Differentiate("x");
            Assert.AreEqual("5*sin(x)^(4^x)*log(sin(x))*log(4)*4^x+5*sin(x)^(4^x-1)*cos(x)*4^x", rs.ToString());
            Assert.AreEqual(1, rs.InvolvedSymbols.Length);
        }

        [TestMethod]
        public void Issues15Test()
        {
            // When parsing 4e-2 the parsing go wrong because it consider 4e a unit and -2 another unit

            var bb = SymbolicVariable.Parse("4e-2");

            Assert.AreEqual("0.04", bb.ToString());

            bb = SymbolicVariable.Parse("4.45e+2");
            Assert.AreEqual("445", bb.ToString());

            

            bb = SymbolicVariable.Parse("-120*t^3+180*t^2+1.06581410364015E-14*t+-30");
            Assert.AreEqual(bb.InvolvedSymbols.Length, 1);

            double r = bb.Execute(2);

        }


        [TestMethod]
        public void Issues16Test()
        {
            var v1 = SymbolicVariable.Parse("5*t1");
            var v2 = SymbolicVariable.Parse("3/t1");
            var vv = SymbolicVariable.Add(v1, v2);

            Assert.AreEqual("5*t1+3/t1", vv.ToString());

            var vm = SymbolicVariable.Subtract(v1, v2);
            Assert.AreEqual("5*t1-3/t1", vm.ToString());

            var g = SymbolicVariable.Add(vv, vm);
            Assert.AreEqual("10*t1", g.ToString());


        }



        [TestMethod]
        public void TestingInfinity()
        {
            var inf = new SymbolicVariable("inf");
            Assert.AreEqual(double.PositiveInfinity, inf.Coeffecient);

            inf = new SymbolicVariable("infinity");
            Assert.AreEqual(double.PositiveInfinity, inf.Coeffecient);

            inf = new SymbolicVariable("infinity") * SymbolicVariable.NegativeOne;
            Assert.AreEqual(double.NegativeInfinity, inf.Coeffecient);

        }



        [TestMethod]
        public void Issues17Test()
        {
            // when entering -sin(x)^2   produce sin(x)^2  which is wrong

            var ss = SymbolicVariable.Parse("-Sin(x)^2");

            Assert.AreEqual("-Sin(x)^2", ss.ToString());


        }



        /// <summary>
        ///A test for TrigSimplify
        ///</summary>
        [TestMethod]
        public void TrigSimplifyTest()
        {
            SymbolicVariable Test = SymbolicVariable.Parse("Cos(x)^2+Sin(x)^2");

            SymbolicVariable Simplified = SymbolicVariable.TrigSimplify(Test);

            Assert.AreEqual("1", Simplified.ToString());

            Test = SymbolicVariable.Parse("a^2*alpha^2*sin(alpha*t)^2+a^2*alpha^2*cos(alpha*t)^2+b^2");

            Simplified = SymbolicVariable.TrigSimplify(Test);

            Assert.AreEqual("a^2*alpha^2+b^2", Simplified.ToString());

            Test = SymbolicVariable.Parse("(sin(phi)^2+cos(phi)^2)*sin(theta)^2+cos(theta)^2");
        }

        [TestMethod]
        public void IIF_ExecuteTest()
        {
            SymbolicVariable iif = SymbolicVariable.Parse("IIF(x <= 5, x^2, x^3)");

            Assert.AreEqual<string>(iif.InvolvedSymbols[0], "x");

            Assert.AreEqual(iif.Execute(3), 3 * 3);
            Assert.AreEqual(iif.Execute(5), 5 * 5);
            Assert.AreEqual(iif.Execute(6), 6 * 6 * 6);


            var ComplexIIf = SymbolicVariable.Parse("IIF(x*y <= 20, 0, IIF(x<5, 1, 2))");

            Assert.AreEqual(ComplexIIf.Execute(4, 5), 0);
            Assert.AreEqual(ComplexIIf.Execute(4, 6), 1);
            Assert.AreEqual(ComplexIIf.Execute(6, 4), 2);


            var iif2 = SymbolicVariable.Parse("IIF(x+y<10, x*y, x/y)");

            Assert.AreEqual(iif2.Execute(2, 3), 2.0 * 3.0);
            Assert.AreEqual(iif2.Execute(8, 9), 8.0 / 9.0);

        }

        [TestMethod]
        public void Issue18Test()
        {
            // an error occur when expression begins with -ve sign
            // -27*x^3   had 2 issues
            //   a]  the negative sign has taken the 27  to be -27 as a word in InvolvedSymbols
            //   b]  the negative sign dropped in the evaluating of parameter expression

            var iif = SymbolicVariable.Parse("IIF(x>45,---27*x^3,-50)");

            var actual = iif.Execute(50);
            
            Assert.AreEqual(-3375000, actual);

            Assert.AreEqual(-50.0, iif.Execute(40));

        }


        [TestMethod]
        public void TestIntFunction()
        {

            var ii = SymbolicVariable.Parse("Int(x)");

            var actual = ii.Execute(3.0/4.0);

            Assert.AreEqual(0, actual);
        }
    }

}

