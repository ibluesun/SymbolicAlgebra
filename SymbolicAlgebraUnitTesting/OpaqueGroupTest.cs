using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymbolicAlgebra;
using System;

namespace SymbolicAlgebra.Test
{
    /// <summary>
    /// Opaque bracket groups: {..} [..] &lt;..&gt;
    ///
    /// CONTRACT
    ///   An opener starts a group only if WillClose() finds a clean match ahead.
    ///   A group that closes is ONE atomic symbol and whitespace inside it is
    ///   PRESERVED (runs merged to one space, edges trimmed). Anything that does
    ///   not close cleanly is ordinary text, so its spaces are dropped like any
    ///   other whitespace outside a group.
    ///
    /// CONSEQUENCE (intentional, not a bug)
    ///   Preservation and spelling-independent identity cannot both hold.
    ///   {b - b} and {b-b} are therefore DIFFERENT symbols.
    ///   See GroupSpacingIsSignificant below.
    ///
    /// Targets .NET Framework 4.8 / MSTest v1 idioms ([ExpectedException]).
    /// </summary>
    [TestClass]
    public class OpaqueGroupTest
    {
        static string P(string s) { return SymbolicVariable.Parse(s).ToString(); }

        #region atomicity and whitespace preservation

        [TestMethod]
        public void GroupIsAtomic()
        {
            // separators inside a closed group are absorbed, not split on
            Assert.AreEqual("{1 1}", P("{1 1}"));
            Assert.AreEqual("[3 4 5]", P("[3 4 5]"));
            Assert.AreEqual("{1 - 1}", P("{1 - 1}"));
            Assert.AreEqual("{4 3 g h a + t r q}", P("{4 3 g h a + t r q}"));

            // and the atom takes part in normal algebra
            Assert.AreEqual("0", P("{1 1} - {1 1}"));
            Assert.AreEqual("9*{1 1}", P("{1 1} * 9"));
        }

        [TestMethod]
        public void WhitespaceRunsMergeAndEdgesTrim()
        {
            Assert.AreEqual("0", P("{1 1} - {1  1}"));     // run -> single space
            Assert.AreEqual("0", P("[1 2] - [ 1 2 ]"));    // both edges trimmed
            Assert.AreEqual("{1 1}", P("{  1   1  }"));
            Assert.AreEqual("{1 1}", P("{\t1\t1\t}"));     // tab is whitespace
        }

        [TestMethod]
        public void GroupSpacingIsSignificant()
        {
            // INTENTIONAL. Preservation means differing interior spelling is a
            // different symbol. Do not "fix" these to cancel.
            Assert.AreNotEqual("0", P("{a{b - b}a} - {a{b-b}a}"));
            Assert.AreNotEqual("0", P("{a{b - b}a} - {a{b - b} a}"));

            // identical spelling still cancels
            Assert.AreEqual("0", P("{a{b - b}a} - {a{b - b}a}"));
            Assert.AreEqual("0", P("{a{b - b} a} - {a{b - b} a}"));
        }

        [TestMethod]
        public void GroupAtAnyPositionInToken()
        {
            // regression: the Symbol setter used to recognise only a group
            // spanning the WHOLE token, so a leading char killed preservation
            Assert.AreEqual("a{1 1}", P("a{1 1}"));
            Assert.AreEqual("9*a{1 1}", P("a{1 1} * 9"));
            Assert.AreEqual("0", P("a{1 1} - a{1 1}"));
        }

        [TestMethod]
        public void NestedGroupsMatchByDepth()
        {
            Assert.AreEqual("[{1 2} {3 4}]", P("[{1 2} {3 4}]"));
            Assert.AreEqual("0", P("[{1 2} {3 4}] - [{1 2} {3 4}]"));
            Assert.AreEqual("{a{b - b}a}", P("{a{b - b}a}"));
        }

        #endregion

        #region malformed brackets -> ordinary text

        [TestMethod]
        public void UnclosedOpenerIsNotAGroup()
        {
            // WillClose finds no closer ahead, so the opener never opens.
            // '^' therefore stays a live separator: (a{b)^4, not one symbol.
            Assert.AreEqual("a{b^4", P("a{b^4"));
            Assert.AreEqual("a<b^4", P("a<b^4"));
            Assert.AreEqual("a[b^4", P("a[b^4"));

            // with no group open, spaces are dropped as ordinary trivia
            Assert.AreEqual("[02{", P("[0 2{"));
        }

        [TestMethod]
        public void MismatchedBracketsAreOrdinaryText()
        {
            // consistent regardless of position in the token
            Assert.AreEqual("[25}", P("[2 5}"));
            Assert.AreEqual("a[12}", P("a[1 2}"));
            Assert.AreEqual("a{b]c}", P("a{b]c}"));

            // still usable as plain atoms in algebra
            Assert.AreEqual("[25}+[02{", P("[2 5} + [0 2{"));
            Assert.AreEqual("0", P("[2 5} - [2 5}"));
        }

        [TestMethod]
        public void UnmatchedCloserIsIgnored()
        {
            Assert.AreEqual("a>b", P("a>b"));
            Assert.AreEqual("a}b", P("a}b"));
        }

        [TestMethod]
        public void MalformedGroupsStillExpand()
        {
            // (a{b - g]a)^2 : no group opens, so '-' splits into two atoms
            var r = P("(a{b - g]a)^2");
            StringAssert.Contains(r, "a{b^2");
            StringAssert.Contains(r, "g]a^2");
        }

        [TestMethod]
        public void NestedMismatchFailsAtTheOutermostOpener()
        {
            // WillClose validates the WHOLE subtree, not just the outer pair.
            // Scanning "{a[b}": push '{', push '[', then '}' pops '[' -> false.
            // So the OUTER '{' never opens and the stack stays empty, which is
            // why the inside-group opener branch is unreachable by construction.
            var v = new SymbolicVariable("{a[b}");
            Assert.AreEqual("{a[b}", v.Symbol);            // no character dropped
            Assert.IsTrue(v.ParsedTokenHadUnmatchedBrackets);
        }

        #endregion

        #region ParsedTokenHadUnmatchedBrackets

        [TestMethod]
        public void FlagClearOnWellFormedTokens()
        {
            Assert.IsFalse(new SymbolicVariable("{1 1}").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(new SymbolicVariable("[3 4 5]").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(new SymbolicVariable("<kg.m/s^2>").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(new SymbolicVariable("a{1 1}").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(new SymbolicVariable("{a{b - b}a}").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(new SymbolicVariable("[{1 2} {3 4}]").ParsedTokenHadUnmatchedBrackets);

            // no brackets at all
            Assert.IsFalse(new SymbolicVariable("x").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(new SymbolicVariable("2.5").ParsedTokenHadUnmatchedBrackets);
        }

        [TestMethod]
        public void FlagSetOnMismatchedPair()
        {
            Assert.IsTrue(new SymbolicVariable("[2 5}").ParsedTokenHadUnmatchedBrackets);
            Assert.IsTrue(new SymbolicVariable("a[1 2}").ParsedTokenHadUnmatchedBrackets);
            Assert.IsTrue(new SymbolicVariable("{a j]").ParsedTokenHadUnmatchedBrackets);
        }

        [TestMethod]
        public void FlagSetOnUnclosedOpener()
        {
            Assert.IsTrue(new SymbolicVariable("[0 2{").ParsedTokenHadUnmatchedBrackets);
            Assert.IsTrue(new SymbolicVariable("a{b").ParsedTokenHadUnmatchedBrackets);
        }

        [TestMethod]
        public void FlagIgnoresUnmatchedCloser()
        {
            // a stray closer is just a character; only OPENERS are validated
            Assert.IsFalse(new SymbolicVariable("a}b").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(new SymbolicVariable("a>b").ParsedTokenHadUnmatchedBrackets);
        }

        [TestMethod]
        public void FlagIsPerTokenAndDoesNotPropagate()
        {
            // DOCUMENTS CURRENT BEHAVIOUR. The flag records what the parsed
            // TOKEN looked like; arithmetic builds fresh instances, so it does
            // not survive. If propagation is ever added, this test flips and
            // should be updated deliberately.
            var bad = SymbolicVariable.Parse("[2 5} * 2");
            Assert.IsTrue(bad.ParsedTokenHadUnmatchedBrackets);
            Assert.IsTrue(SymbolicVariable.Parse("[2 5} + x").ParsedTokenHadUnmatchedBrackets);
            Assert.IsFalse(SymbolicVariable.Parse("x + y").ParsedTokenHadUnmatchedBrackets);
        }

        [TestMethod]
        public void FlagPositionIndependent()
        {
            Assert.AreEqual(
                new SymbolicVariable("[2 5}").ParsedTokenHadUnmatchedBrackets,
                new SymbolicVariable("a[2 5}").ParsedTokenHadUnmatchedBrackets);
        }

        #endregion

        #region angle brackets vs comparison operators

        [TestMethod]
        public void UnitAnnotationIsOneAtom()
        {
            // the '^' inside used to be read as a power and swallow the '>'
            Assert.AreEqual("<kg.m/s^2>", P("<kg.m/s^2>"));
            Assert.AreEqual("0", P("3<kg.m/s^2> - 3<kg.m/s^2>"));
            Assert.AreEqual("0", P("<m> - <m>"));
        }

        [TestMethod]
        public void ComparisonInsideParensStillWorks()
        {
            // '<' only opens a group OUTSIDE parentheses; inside, the condition
            // parser owns it as a comparison operator. This is the case the
            // original suite had zero coverage of: all four existing IIF tests
            // contain a '<' or a '>' but never both in one expression.
            var mixed = SymbolicVariable.Parse("IIF(x<5, 1, 2) + IIF(y>3, 4, 5)");
            Assert.AreEqual(2, mixed.InvolvedSymbols.Length,
                "x and y must remain distinct symbols, not one swallowed blob");
        }

        #endregion

        #region differentiation

        [TestMethod]
        public void UnitIsInertUnderDifferentiation()
        {
            // the nabla '^' blocker: unit must not leak in as bare symbols
            var d = SymbolicVariable.Parse("(3<kg.m/s^2> * t^2)|t");
            Assert.IsTrue(Array.IndexOf(d.InvolvedSymbols, "t") >= 0);
            Assert.IsTrue(Array.IndexOf(d.InvolvedSymbols, "kg") < 0);
            Assert.IsTrue(Array.IndexOf(d.InvolvedSymbols, "m") < 0);
            Assert.IsTrue(Array.IndexOf(d.InvolvedSymbols, "s") < 0);

            // group as a coefficient survives the round trip
            Assert.AreEqual("{1 1}", P("({1 1}*g)|g"));

            // a group is not a function of anything inside its text
            Assert.AreEqual("0", P("(3<kg.m/s^2>)|s"));
        }

        [TestMethod]
        public void BinomialOverGroups()
        {
            var r = P("({4 3 g h a + t r q} - t)^3");
            StringAssert.Contains(r, "{4 3 g h a + t r q}^3");
            StringAssert.Contains(r, "t^3");
        }

        #endregion

        #region line breaks

        [TestMethod]
        public void LineBreaksAreTriviaOutsideGroups()
        {
            Assert.AreEqual("5*x", P("2*x\n+ 3*x"));
            Assert.AreEqual("5*x", P("2*x\r\n+ 3*x"));

            // verbatim string: carries \r\n in a CRLF source file
            Assert.AreEqual("5*x", P(@"2*x
+ 3*x"));
        }

        [TestMethod]
        public void LineBreaksNormaliseInsideGroups()
        {
            // must not split symbol identity, and must not collapse to nothing
            Assert.AreEqual("0", P("{1\n1} - {1 1}"));
            Assert.AreEqual("0", P("[1\r\n2] - [1 2]"));
            Assert.AreEqual("{1 1}", P("{1\n\n1}"));
        }

        [TestMethod]
        [ExpectedException(typeof(SymbolicException))]
        public void DirectConstructionRejectsNewline()
        {
            // Parse normalises line breaks at entry; the ctor guard is the
            // backstop for direct construction (Qs, log simplification, etc.)
            new SymbolicVariable("a\nb");
        }

        [TestMethod]
        [ExpectedException(typeof(SymbolicException))]
        public void DirectConstructionRejectsCarriageReturn()
        {
            new SymbolicVariable("a\rb");
        }

        #endregion
    }
}
