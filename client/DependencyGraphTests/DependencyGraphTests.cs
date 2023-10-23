//Testing class for the DependencyGraph class
//Version 1.0 - Tyler Wood

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }

        [TestMethod()]
        public void TestThisWithNone()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t["a"]);
        }

        [TestMethod()]
        public void TestThisWithOne()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(1, t["aa"]);
        }

        [TestMethod()]
        public void TestThisWithMultiple()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            t.AddDependency("a1", "a2");
            t.AddDependency("a1", "a3");
            t.AddDependency("a2", "a2");
            Assert.AreEqual(2, t["a2"]);
        }

        [TestMethod()]
        public void TestHasDependentsFalse()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.IsFalse(t.HasDependents("aa"));
        }

        [TestMethod()]
        public void TestHasDependentsTrue()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.IsTrue(t.HasDependents("a1"));
        }

        [TestMethod()]
        public void TestHasDependeesTrue()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.IsTrue(t.HasDependees("aa"));
        }

        [TestMethod()]
        public void TestHasDependeesFalse()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.IsFalse(t.HasDependees("a1"));
        }

        [TestMethod()]
        public void TestGetDependeesEmpty()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(0, t.GetDependees("a1").Count());
        }

        [TestMethod()]
        public void TestGetDependeesOne()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(1, t.GetDependees("aa").Count());
        }

        [TestMethod()]
        public void TestGetDependeesNotInGraph()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(0, t.GetDependees("a2").Count());
        }

        [TestMethod()]
        public void TestGetDependentsOne()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(1, t.GetDependents("a1").Count());
        }

        [TestMethod()]
        public void TestGetDependentsEmpty()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(0, t.GetDependents("aa").Count());
        }

        [TestMethod()]
        public void TestGetDependntsNotInGraph()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(0, t.GetDependents("a2").Count());
        }

        [TestMethod()]
        public void SimpleAddTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            Assert.AreEqual(1, t.Size);
        }

        [TestMethod()]
        public void DuplicateAddTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            t.AddDependency("a1", "aa");
            Assert.AreEqual(1, t.Size);
        }

        [TestMethod()]
        public void AddSameKeyTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            t.AddDependency("a1", "a2");
            Assert.AreEqual(2, t.Size);
            Assert.AreEqual(0, t["a1"]);
        }

        [TestMethod()]
        public void AddDifferentKeyTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            t.AddDependency("a3", "a2");
            Assert.AreEqual(2, t.Size);
            Assert.AreEqual(1, t["aa"]);
        }

        [TestMethod()]
        public void AddCheckDependee()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            t.AddDependency("a3", "a2");
            Assert.AreEqual(2, t.Size);
            Assert.AreEqual(1, t["aa"]);
            Assert.AreEqual(0, t.GetDependees("a1").Count());
            Assert.AreEqual(1, t.GetDependees("aa").Count());
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }

        [TestMethod()]
        public void SimpleRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("a1", "a2");
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(1, t.Size);
        }

        [TestMethod()]
        public void RemoveFromSameDependent()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("x", "a2");
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(1, t.Size);
        }

        [TestMethod()]
        public void RemoveNonexistant()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            t.AddDependency("x", "a2");
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("y", "y");
            Assert.AreEqual(2, t.Size);
        }

        [TestMethod()]
        public void RemoveCheckDependee()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a1", "aa");
            t.RemoveDependency("a1", "aa");
            Assert.AreEqual(0, t.Size);
            Assert.AreEqual(0, t["a1"]);
            Assert.AreEqual(0, t.GetDependees("a1").Count());
            Assert.AreEqual(0, t.GetDependees("aa").Count());
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }

        [TestMethod()]
        public void ReplaceDependentsSimpleTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            string[] replace = { "a", "b" };
            t.ReplaceDependents("x", replace);
            Assert.AreEqual(t.Size, 2);
        }

        [TestMethod()]
        public void ReplaceDependentsEmptyBothTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            string[] replace = { "a", "b" };
            t.ReplaceDependents("x", replace);
            Assert.AreEqual(2, t.Size);
        }

        [TestMethod()]
        public void ReplaceDependeesSimpleTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            string[] replace = { "a", "b" };
            t.ReplaceDependees("y", replace);
            Assert.AreEqual(t.Size, 2);
        }

        [TestMethod()]
        public void ReplaceDependeesEmptySetTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.ReplaceDependees("y", new HashSet<string>());
            Assert.AreEqual(t.Size, 0);
        }

        [TestMethod()]
        public void ReplaceDependeesEmptyBothTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            string[] replace = { "a", "b" };
            t.ReplaceDependees("y", replace);
            Assert.AreEqual(2, t.Size);
        }

        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

    }
}
