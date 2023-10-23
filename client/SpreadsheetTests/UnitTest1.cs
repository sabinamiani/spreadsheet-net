// Version 1.0 - September 24, 2020
// Written by Tyler Wood for CS3500
//
// Version 2.0 - October 2, 2020
// Written by Tyler Wood
// Updates: More tests written, XML methods for testing as well.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Linq;
using System.Xml;

namespace SpreadsheetTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestConstructor()
        {
            Spreadsheet s = new Spreadsheet();
        }

        [TestMethod]
        public void TestConstructorThreeVars()
        {
            Spreadsheet s2 = new Spreadsheet(s => true, s => s, "v1.1");
        }

        [TestMethod]
        public void TestConstructorFourVars()
        {
            XMLWrite("test");
            Spreadsheet s2 = new Spreadsheet("test.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        public void TestSaveThenLoadWorks()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "3");
            s.SetContentsOfCell("a2", "=a1 + 3");
            s.Save("savethenload.xml");
            Spreadsheet s2 = new Spreadsheet("savethenload.xml", s => true, s => s, "default");
            Assert.AreEqual(2, s2.GetNamesOfAllNonemptyCells().Count());
            Assert.AreEqual(6.0, s2.GetCellValue("a2"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFails()
        {
            Spreadsheet s2 = new Spreadsheet("Hest.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsNoContent()
        {
            XMLWriteBad("thisfails");
            Spreadsheet s2 = new Spreadsheet("thisfails.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsNoName()
        {
            XMLWriteBadNoName("noname");
            Spreadsheet s2 = new Spreadsheet("noname.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsDifferentElementType()
        {
            XMLWriteInsertState("stateexception");
            Spreadsheet s2 = new Spreadsheet("stateexception.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsCircular()
        {
            XMLWriteInsertCircular("circular");
            Spreadsheet s2 = new Spreadsheet("circular.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsWrongVersion()
        {
            XMLWrite("wrongv");
            Spreadsheet s2 = new Spreadsheet("wrongv.xml", s => true, s => s, "v2.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsCellNested()
        {
            XMLWriteCellInCell("cellnest");
            Spreadsheet s2 = new Spreadsheet("cellnest.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsDoubleName()
        {
            XMLWriteTwoName("twoname");
            Spreadsheet s2 = new Spreadsheet("twoname.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsDoubleContent()
        {
            XMLWriteTwoContent("twocont");
            Spreadsheet s2 = new Spreadsheet("twocont.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsNameOutsideCell()
        {
            XMLWriteNameOutsideCell("nameoutcell");
            Spreadsheet s2 = new Spreadsheet("nameoutcell.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsContentOutsideCell()
        {
            XMLWriteContentOutsideCell("contoutcell");
            Spreadsheet s2 = new Spreadsheet("contoutcell.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsNoNameInName()
        {
            XMLWriteNoNameinName("nonameinnamecell");
            Spreadsheet s2 = new Spreadsheet("nonameinnamecell.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorFourVarsFailsNoContInCont()
        {
            XMLWriteNoContentInContent("nocontincontcell");
            Spreadsheet s2 = new Spreadsheet("nocontincontcell.xml", s => true, s => s, "v1.1");
        }

        [TestMethod]
        public void TestGetVersion()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s, "1.1");
            Assert.AreEqual("1.1", s.Version);

        }

        [TestMethod]
        public void TestGetVersionDefault()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("default", s.Version);
        }

        [TestMethod]
        public void TestIsValid()
        {
            Func<string, bool> valid = s => true;
            Func<string, string> norm = s => s.ToUpper();
            Spreadsheet s = new Spreadsheet(valid, norm, "hi");
            Assert.AreEqual(valid, s.IsValid);
        }

        [TestMethod]
        public void TestNormalize()
        {
            Func<string, bool> valid = s => true;
            Func<string, string> norm = s => s.ToUpper();
            Spreadsheet s = new Spreadsheet(valid, norm, "hi");
            Assert.AreEqual(norm, s.Normalize);
        }

        [TestMethod]
        public void TestChanged()
        {
            Func<string, bool> valid = s => true;
            Func<string, string> norm = s => s.ToUpper();
            Spreadsheet s = new Spreadsheet(valid, norm, "hi");
            Assert.IsFalse(s.Changed);
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "3.5").Count);
            Assert.IsTrue(s.Changed);
            s.Save("test.xml");
            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        public void TestGetSavedVersion()
        {
            XMLWrite("test");
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("v1.1", s.GetSavedVersion("test.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionOpenError()
        {
            XMLWrite("test");
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("v1.1", s.GetSavedVersion("rest.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionNoVersion()
        {
            XMLWriteInsertNoVersion("noversion");
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("v1.1", s.GetSavedVersion("noversion.xml"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionNull()
        {
            XMLWriteNullVersionName("nullversion");
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("v1.1", s.GetSavedVersion("nullversion.xml"));
        }

        [TestMethod]
        public void TestSave()
        {
            Spreadsheet s = new Spreadsheet();
            s.Save("best.xml");
            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveNullVersion()
        {
            Spreadsheet s = new Spreadsheet(s => true, s => s, null);
            s.Save("best.xml");
        }

        [TestMethod]
        public void TestSaveFull()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "hello");
            s.SetContentsOfCell("a2", "3.0");
            s.SetContentsOfCell("a3", "= 3 + 4");
            s.Save("best.xml");
            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        public void StressTestJustAdd()
        {
            Spreadsheet s = new Spreadsheet();
            for(int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "" + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void StressTestAddAndReplace()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "" + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
            for (int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "" + 4.0 + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void StressTestAddAndReplaceWithString()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "" + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
            for (int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "" + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void StressTestAddAndReplaceWithFormula()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "" + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
            for (int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "=" + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void StressTestAddAndReplaceWithFormulaWithDependencies()
        {
            Spreadsheet s = new Spreadsheet();
            for (int i = 0; i < 400; i++)
            {
                s.SetContentsOfCell("a1" + i, "" + i);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
            for (int i = 0; i < 400; i++)
            {
                Assert.AreEqual(1, s.SetContentsOfCell("a1" + i,"=a1" + (i - 1)).Count);
            }
            Assert.AreEqual(400, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsSimple()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "" + 3.0).Count);
            Assert.AreEqual(1, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsTwoDec()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "" + 3.0).Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "" + 3.0).Count);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsEmpty()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(0, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsDecAndForm()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "" + 3.0).Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=3+5").Count);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsDecAndString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "" + 3.0).Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "Hi").Count);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsFormAndString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=3+5").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "Hi").Count);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsTwoString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "hello").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "Hi").Count);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsTwoForm()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=3+5").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=3 + 1").Count);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetNamesOfNonEmptyCellsTwoFormDependent()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=3+ a2").Count);
            Assert.AreEqual(2, s.SetContentsOfCell("a2", "=3 + 1").Count);
            Assert.AreEqual(2, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestGetCellContentsDouble()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "" + 3.0).Count);
            Assert.AreEqual(3.0, s.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestGetCellContentsString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "Hello").Count);
            Assert.AreEqual("Hello", s.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestGetCellContentsFormula()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=3 + 5").Count);
            Assert.IsTrue(new Formula("3 + 5").Equals(s.GetCellContents("a1")));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsNull()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(3.0, s.GetCellContents(null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalid()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(3.0, s.GetCellContents("1a"));
        }

        [TestMethod]
        public void TestGetCellContentsEmpty()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellContents("a1"));
        }

        [TestMethod]
        public void TestGetCellValueEmpty()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual("", s.GetCellValue("a1"));
        }

        [TestMethod]
        public void TestGetCellValueString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "hello").Count);
            Assert.AreEqual("hello", s.GetCellValue("a1"));
        }

        [TestMethod]
        public void TestGetCellValueDouble()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "3").Count);
            Assert.AreEqual(3.0, s.GetCellValue("a1"));
        }

        [TestMethod]
        public void TestGetCellValueFormula()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=3 + 7").Count);
            Assert.AreEqual(10.0, s.GetCellValue("a1"));
        }

        [TestMethod]
        public void TestGetCellValueFormulaDependent()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=3 + 7").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=a1 + 7").Count);
            Assert.AreEqual(10.0, s.GetCellValue("a1"));
            Assert.AreEqual(17.0, s.GetCellValue("a2"));
        }

        [TestMethod]
        public void TestGetCellValueFormulaError()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=3 + 7").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=a1 / 0").Count);
            Assert.AreEqual(10.0, s.GetCellValue("a1"));
            Assert.IsTrue(s.GetCellValue("a2").GetType().Equals(typeof(FormulaError)));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueInvalidName()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue("1a");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSetCellContentsFormulaFormatException()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "=a1 ++ 2");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsCircular()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=a2+7").Count);
            s.SetContentsOfCell("a2", "=a1+3");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsCircularWithinSelf()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a2", "=a2+3");
        }

        [TestMethod]
        public void TestSetCellContentsCircularDoesNotChange()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=a2+7").Count);
            try
            {
                s.SetContentsOfCell("a2", "=a1+3");
            }
            catch (CircularException)
            {
                Assert.AreEqual(1, s.GetNamesOfAllNonemptyCells().Count());
            }
            
        }

        [TestMethod]
        public void TestSetCellContentsCircularDoesNotChangeExtra()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a2", "=6+3");
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=a2+7").Count);
            try
            {
                s.SetContentsOfCell("a2", "=a1+3");
            }
            catch (CircularException)
            {
                
            }
            Assert.AreEqual(9.0, s.GetCellValue("a2"));
            Assert.AreEqual(2, s.SetContentsOfCell("a2", "" + 3.0).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsNullString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", (string)null);
        }

        [TestMethod]
        public void TestSetCellContentsDouble()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "" + 3.0).Count);
        }

        [TestMethod]
        public void TestSetCellContentsString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "hello").Count);
        }

        [TestMethod]
        public void TestSetCellContentsEmptyString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "").Count);
            Assert.AreEqual(0, s.GetNamesOfAllNonemptyCells().Count());
        }

        [TestMethod]
        public void TestSetCellContentsFormula()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
        }

        [TestMethod]
        public void TestSetCellContentsWipeOver()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "" + 7.0).Count);
        }

        [TestMethod]
        public void TestSetCellContentsWipeOverWithString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "hello").Count);
        }

        [TestMethod]
        public void TestSetCellContentsAddTwo()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "=8 + 3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            
        }

        [TestMethod]
        public void TestSetCellContentsAddThree()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "=7 + 3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a3", "=x3 + 9.0").Count);
            
        }

        [TestMethod]
        public void TestSetCellContentsAddThreeReplaceOne()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "=7 + 3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a3", "=x3 + 9.0").Count);
            
            Assert.AreEqual(3, s.SetContentsOfCell("x3", "" + 1.0).Count);
        }

        [TestMethod]
        public void TestSetCellContentsWipeSetsDependentsCorrectDec()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "=7 + 3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a3", "=x3 + 9.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "" + 2.0).Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a3", "" + 5.0).Count);
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "" + 1.0).Count);
        }

        [TestMethod]
        public void TestSetCellContentsWipeSetsDependentsCorrectString()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "=7 + 3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a3", "=x3 + 9.0").Count);
            
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "hi").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a3", "hello").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "" + 1.0).Count);
        }

        [TestMethod]
        public void TestSetCellContentsReplaceOneConnected()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "=4 + 3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a3", "=x3 + 9.0").Count);
            Assert.AreEqual(3, s.SetContentsOfCell("x3", "=5 + 4").Count);
        }

        [TestMethod]
        public void TestSetCellContentsReplaceTwoConnected()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "" + 3.0).Count);
            Assert.AreEqual(1, s.SetContentsOfCell("x3", "=a1 + 3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=x3 + 7.0").Count);
            Assert.AreEqual(2, s.SetContentsOfCell("x3", "=a1 + 2").Count);
            Assert.AreEqual(3, s.SetContentsOfCell("a1", "" + 2.0).Count);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameNullDec()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "" + 3.5);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameNullString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "Hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameNullForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "=3.5");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameInvalidDec()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1 ", "" + 3.0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameInvalidString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1 ", "hi");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameInvalidForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1 ", "=4");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameImproperDec()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("3_", "" + 3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameImproperString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("3_", "Hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameImproperForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("3_", "=3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameUnderscoreDec()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_3", "" + 3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameUnderscoreString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_3", "a");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameUnderscoreForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_3", "=3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameOneCharDec()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_", "" + 3);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameOneCharString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_", " ");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellNameOneCharForm()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("_", "=3");
        }

        [TestMethod]
        public void TestSetCellContentsNewValidatorAndNormalizerFormula()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=3").Count);
            Assert.AreEqual(3.0, s.GetCellValue("A2"));
            Assert.AreEqual(3.0, s.GetCellValue("a2"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNewValidatorAndNormalizerFormulaFails()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("aa2", "=3").Count);
        }

        [TestMethod]
        public void TestSetCellContentsNewValidatorAndNormalizerString()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "hello").Count);
            Assert.AreEqual("hello", s.GetCellValue("A2"));
            Assert.AreEqual("hello", s.GetCellValue("a2"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNewValidatorAndNormalizerStringFails()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("aa2", "hello").Count);
        }

        [TestMethod]
        public void TestSetCellContentsNewValidatorAndNormalizerDouble()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "3.0").Count);
            Assert.AreEqual(3.0, s.GetCellValue("A2"));
            Assert.AreEqual(3.0, s.GetCellValue("a2"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNewValidatorAndNormalizerDoubleFails()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("aa2", "3.0").Count);
        }

        [TestMethod]
        public void TestSetCellContentsNewValidatorAndNormalizerContentFormula()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "3").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=a1 + 3").Count);
            Assert.AreEqual(6.0, s.GetCellValue("A2"));
            Assert.AreEqual(6.0, s.GetCellValue("a2"));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSetCellContentsNewValidatorAndNormalizerContentFormulaFails()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=aa3 + 3").Count);
        }


        [TestMethod]
        public void TestSetCellContentsNewValidatorAndNormalizerDoubleUpdatesDependents()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "3.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=a1 + 3").Count);
            Assert.AreEqual(6.0, s.GetCellValue("A2"));
            Assert.AreEqual(2, s.SetContentsOfCell("a1", "5.0").Count);
            Assert.AreEqual(8.0, s.GetCellValue("A2"));
        }

        [TestMethod]
        public void TestSetCellContentsNewValidatorAndNormalizerFormulaUpdatesDependents()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=2.5 + 3.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=a1 + 3").Count);
            Assert.AreEqual(8.5, s.GetCellValue("A2"));
            Assert.AreEqual(2, s.SetContentsOfCell("a1", "=3 + 5.0").Count);
            Assert.AreEqual(11.0, s.GetCellValue("A2"));
        }

        [TestMethod]
        public void TestSetCellContentsNewValidatorAndNormalizerStringUpdatesDependents()
        {
            Spreadsheet s = new Spreadsheet(s => s.Length == 2, s => s.ToUpper(), "1.1");
            Assert.AreEqual(1, s.SetContentsOfCell("a1", "=2.5 + 3.0").Count);
            Assert.AreEqual(1, s.SetContentsOfCell("a2", "=a1 + 3").Count);
            Assert.AreEqual(8.5, s.GetCellValue("A2"));
            Assert.AreEqual(2, s.SetContentsOfCell("a1", "hello").Count);
            Assert.IsTrue(typeof(FormulaError).IsInstanceOfType(s.GetCellValue("A2")));
        }

        private void XMLWrite(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteBad(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteBadNoName(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("content", "a1");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteInsertState(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteElementString("state", "haha");

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteInsertCircular(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a2");
                writer.WriteElementString("contents", "=a1");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "=a2");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteInsertNoVersion(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a2");
                writer.WriteElementString("contents", "=a1");
                writer.WriteEndElement();

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "=a2");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteCellInCell(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteTwoName(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("name", "3.5");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteTwoContent(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("contents", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteNameOutsideCell(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteElementString("name", "a1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteContentOutsideCell(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteElementString("contents", "3.5");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteNoNameinName(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteStartElement("name");
                writer.WriteEndElement();
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void XMLWriteNoContentInContent(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "v1.1");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteStartElement("contents");
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }


        private void XMLWriteNullVersionName(string name)
        {
            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", null);

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "a1");
                writer.WriteElementString("contents", "3.5");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

    }
}
