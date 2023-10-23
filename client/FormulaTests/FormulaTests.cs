//(Tyler Wood)
//Version 1.0 (9/17/20)


using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void TestConstructorWorksSimple()
        {
            Formula f = new Formula("5 + 5");
        }

        [TestMethod]
        public void TestConstructorWorksComplex()
        {
            Formula f = new Formula("(5 + a5) - f3 / 18.0");
        }

        [TestMethod]
        public void TestConstructorWorksSciNotation()
        {
            Formula f = new Formula("(5 + 3e10) - f3 / 18.0");
        }

        [TestMethod]
        public void TestFullConstructorWorks()
        {
            Formula f = new Formula("(5 + a5) - f3 / 18.0", s => "a1", s => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorNoTokens()
        {
            Formula f = new Formula(" ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorFaultyTokens()
        {
            Formula f = new Formula("5 + %");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorMoreRightParen()
        {
            Formula f = new Formula("(5 + 3)) + 4");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorUnbalanced()
        {
            Formula f = new Formula("(5 + 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorStartingError()
        {
            Formula f = new Formula("+ 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorStartingErrorParen()
        {
            Formula f = new Formula(") 7+ 3");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorEndingError()
        {
            Formula f = new Formula("7 + ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorEndingErrorParen()
        {
            Formula f = new Formula("7 + 3 (");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorOpenParenClosingParen()
        {
            Formula f = new Formula("() ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorOpenParenOperator()
        {
            Formula f = new Formula("(+ ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorOperatorClosingParen()
        {
            Formula f = new Formula("(3 +) ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorOperatorDoubled()
        {
            Formula f = new Formula("(3 ++ ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorNumberDoubled()
        {
            Formula f = new Formula("(3 3 + 7) ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorNumberVar()
        {
            Formula f = new Formula("(3 a3 + 7) ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorVariableDoubled()
        {
            Formula f = new Formula("(a1 a1 + 3) ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorVariableNum()
        {
            Formula f = new Formula("(a1 1 + 3) ");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorCloseOpenParen()
        {
            Formula f = new Formula("(3 +3)(");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorCloseVar()
        {
            Formula f = new Formula("(3 +3)a1");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorCloseNum()
        {
            Formula f = new Formula("(3 +3)1");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorNormalizeErrorSymbol()
        {
            Formula f = new Formula("(3 +3) + a1", n => "#a", n => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorNormalizeErrorNumber()
        {
            Formula f = new Formula("(3 +3) + a1", n => "3a", n => true);
        }

        [TestMethod]
        public void TestConstructorOneLetterVar()
        {
            Formula f = new Formula("(3 +3) + b", n => "a", n => true);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestConstructorIsValidError()
        {
            Formula f = new Formula("(3 +3) + a1", n => "a3", n => false);
        }

        [TestMethod]
        public void TestEvaluateSimpleSubtraction()
        {
            Formula f = new Formula("15.6 - 5.6");
            Assert.AreEqual(10.0, (double)f.Evaluate(n => 1), 1e-9);
        }

        [TestMethod]
        public void TestEvaluateSimpleAddition()
        {
            Formula f = new Formula("15.6 + 5.6");
            Assert.AreEqual(21.2, (double)f.Evaluate(n => 1), 1e-9);
        }

        [TestMethod]
        public void TestEvaluateSimpleMultiplication()
        {
            Formula f = new Formula("15.0 * 3");
            Assert.AreEqual(45.0, (double)f.Evaluate(n => 1), 1e-9);
        }

        [TestMethod]
        public void TestEvaluateSimpleDivision()
        {
            Formula f = new Formula("15.0 / 3");
            Assert.AreEqual(5.0, (double)f.Evaluate(n => 1), 1e-9);
        }

        [TestMethod]
        public void TestEvaluateSimpleParenthesis()
        {
            Formula f = new Formula("(15.6 - 5.6) + 2");
            Assert.AreEqual(12.0, (double)f.Evaluate(n => 1), 1e-9);
        }

        [TestMethod]
        public void TestEvaluateComplex()
        {
            Formula f = new Formula("(5 + 5) / 6.3");
            Assert.AreEqual(1.587301587, (double)f.Evaluate(n => 1), 1e-9);
        }

        [TestMethod]
        public void TestEvaluateDivideByZero()
        {
            Formula f = new Formula("5 / 0");
            Assert.IsInstanceOfType(f.Evaluate(n => 1), typeof(FormulaError));
        }

        //TODO: Needs to be correctly written
        [TestMethod]
        public void TestEvaluateVarDoesntExist()
        {
            Formula f = new Formula("5 / a1");
            Assert.IsInstanceOfType(f.Evaluate(n => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod]
        public void TestEvaluateWithVar()
        {
            Formula f = new Formula("(5 + a1) / 2");
            Assert.AreEqual(3.0, (double)f.Evaluate(n => 1.0), 1e-9);
        }

        [TestMethod]
        public void TestEvaluateWithSciNotation()
        {
            Formula f = new Formula("(5 + 3e2)");
            Assert.AreEqual(305.0, (double)f.Evaluate(n => 1.0), 1e-9);
        }

        [TestMethod]
        public void TestGetVariablesNoDuplicates()
        {
            Formula f = new Formula("(5 + a1) - a3");
            List<string> vars = new List<string>(f.GetVariables());
            Assert.AreEqual(2, vars.Count);
        }

        [TestMethod]
        public void TestGetVariablesWithDuplicates()
        {
            Formula f = new Formula("(5 + a1) - a1");
            List<string> vars = new List<string>(f.GetVariables());
            Assert.AreEqual(1, vars.Count);
        }

        [TestMethod]
        public void TestGetVariablesEmpty()
        {
            Formula f = new Formula("(5 + 1) - 3");
            List<string> vars = new List<string>(f.GetVariables());
            Assert.AreEqual(0, vars.Count);
        }

        [TestMethod]
        public void TestToStringWithSpace()
        {
            Formula f = new Formula("5 + 5");
            Assert.AreEqual("5+5", f.ToString());
        }

        [TestMethod]
        public void TestToStringNoSpace()
        {
            Formula f = new Formula("5.3+5.0");
            Assert.AreEqual("5.3+5", f.ToString());
        }

        [TestMethod]
        public void TestToStringWithVariables()
        {
            Formula f = new Formula("a3 + a1");
            Assert.AreEqual("a3+a1", f.ToString());
        }

        [TestMethod]
        public void TestToStringWithVariablesAndNums()
        {
            Formula f = new Formula("a3 + 3.5");
            Assert.AreEqual("a3+3.5", f.ToString());
        }

        [TestMethod]
        public void TestEqualsTrue()
        {
            Formula f = new Formula("a3 + 3.5");
            Assert.IsTrue(f.Equals(new Formula("a3 + 3.5")));
        }

        [TestMethod]
        public void TestEqualsFalse()
        {
            Formula f = new Formula("a3 + 3.5");
            Assert.IsFalse(f.Equals(new Formula("a3 + 3.2")));
        }

        [TestMethod]
        public void TestEqualsTrueWithSpace()
        {
            Formula f = new Formula("a3+3.5");
            Assert.IsTrue(f.Equals(new Formula("a3 + 3.5")));
        }

        [TestMethod]
        public void TestEqualsWithLongDoubles()
        {
            Formula f = new Formula("a3 + 3.500");
            Assert.IsTrue(f.Equals(new Formula("a3 + 3.5")));
        }

        [TestMethod]
        public void TestEqualsWithNull()
        {
            Formula f = new Formula("a3 + 3.500");
            Assert.IsFalse(f.Equals(null));
        }

        [TestMethod]
        public void TestEqualsNotFormula()
        {
            Formula f = new Formula("a3 + 3.500");
            Assert.IsFalse(f.Equals(new List<string>()));
        }

        [TestMethod]
        public void TestEqualsFalseAfterNormalize()
        {
            Formula f = new Formula("a3 + 3.500");
            Assert.IsFalse(f.Equals(new Formula("a3 + 3.500", n => "a2", n => true)));
        }

        [TestMethod]
        public void TestEqualsTrueAfterNormalize()
        {
            Formula f = new Formula("a3 + 3.500");
            Assert.IsTrue(f.Equals(new Formula("a2 + 3.500", n => "a3", n => true)));
        }

        [TestMethod]
        public void TestEqualsOperatorDoubleNull()
        {
            Formula f = null;
            Formula f2 = null;
            Assert.IsTrue(f == f2);
        }

        [TestMethod]
        public void TestEqualsOperatorFirstNull()
        {
            Formula f = null;
            Assert.IsFalse(f == new Formula("3"));
        }

        [TestMethod]
        public void TestEqualsOperatorSecondNull()
        {
            Formula f = null;
            Assert.IsFalse(new Formula("3") == f );
        }

        [TestMethod]
        public void TestEqualsOperatorTrue()
        {
            Formula f = new Formula("3 + 2");
            Formula f2 = new Formula("3 + 2.0");
            Assert.IsTrue(f == f2);
        }

        [TestMethod]
        public void TestEqualsOperatorFalse()
        {
            Formula f = new Formula("3 + 2");
            Formula f2 = new Formula("3 + 3.0");
            Assert.IsFalse(f == f2);
        }

        [TestMethod]
        public void TestNotEqualsOperatorDoubleNull()
        {
            Formula f = null;
            Formula f2 = null;
            Assert.IsFalse(f != f2);
        }

        [TestMethod]
        public void TestNotEqualsOperatorFirstNull()
        {
          Formula f = null;
          Assert.IsTrue(f != new Formula("3"));
        }

        [TestMethod]
        public void TestNotEqualsOperatorSecondNull()
        {
           Formula f = null;
           Assert.IsTrue(new Formula("3") != f);
        }

        [TestMethod]
        public void TestNotEqualsOperatorFalse()
        {
            Formula f = new Formula("3 + 2");
            Formula f2 = new Formula("3 + 2.0");
            Assert.IsFalse(f != f2);
        }

        [TestMethod]
        public void TestNotEqualsOperatorTrue()
        {
            Formula f = new Formula("3 + 2");
            Formula f2 = new Formula("3 + 3.0");
            Assert.IsTrue(f != f2);
        }

        [TestMethod]
        public void TestGetHashCode()
        {
            Formula f = new Formula("3 + 2");
            Formula f2 = new Formula("3.0 + 2.0");
            Assert.AreEqual(f.GetHashCode(), f2.GetHashCode());
        }

        [TestMethod]
        public void TestGetHashCodeClose()
        {
            Formula f = new Formula("3 + 2");
            Formula f2 = new Formula("3.0 + 2.1");
            Assert.AreNotEqual(f.GetHashCode(), f2.GetHashCode());
        }

    }
}
