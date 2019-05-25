using Debugger;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Debugger.Tests
{
    [TestClass()]
    public class DebuggerTests
    {
        private string simpleTest = "sub main\n" +
                                    "  set a 2\n" +
                                    "  print a\n";

        private string subTest = "sub foo\n" +
                                 "  print a\n" +
                                 "\n" +
                                 "sub main\n" +
                                 "  set a 2\n" +
                                 "  call foo\n";

        private string complexTest = "sub foo\n" +
                                     "  set a 3\n" +
                                     "\n" +
                                     "sub main\n" +
                                     "  set a 2\n" +
                                     "  print a\n" +
                                     "  call foo\n" +
                                     "  print a\n" +
                                     "  call foo2\n" +
                                     "\n" +
                                     "sub foo2\n" +
                                     "  set b 5\n" +
                                     "  print b\n";

        private string recurciveTest = "sub main\n" +
                                       "  set a 2\n" +
                                       "  call foo\n" +
                                       "\n" +
                                       "sub foo\n" +
                                       "  print a\n" +
                                       "  call foo\n";

        private string grammarTest = "sub main\n" +
                                     "  set a 2\n" +
                                     "  prnt a\n";

        private string undefVarTest = "sub main\n" +
                                     "  set a 2\n" +
                                     "  print b\n";

        [TestMethod()]
        public void StepIntoTest()
        {
            Debugger dbg = new Debugger(simpleTest);
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsFalse(dbg.StepInto());

            dbg = new Debugger(subTest);
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsFalse(dbg.StepInto());

            dbg = new Debugger(complexTest);
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsFalse(dbg.StepInto());

            dbg = new Debugger(recurciveTest);
            Assert.IsTrue(dbg.StepInto());
            Assert.IsTrue(dbg.StepInto());
            System.Exception expectedException = null;
            try
            {
                dbg.StepInto();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(grammarTest);
            Assert.IsTrue(dbg.StepInto());
            expectedException = null;
            try
            {
                dbg.StepInto();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(undefVarTest);
            Assert.IsTrue(dbg.StepInto());
            expectedException = null;
            try
            {
                dbg.StepInto();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);
        }

        [TestMethod()]
        public void StepOverTest()
        {
            Debugger dbg = new Debugger(simpleTest);
            Assert.IsTrue(dbg.StepOver());
            Assert.IsTrue(dbg.StepOver());
            Assert.IsFalse(dbg.StepOver());

            dbg = new Debugger(subTest);
            Assert.IsTrue(dbg.StepOver());
            Assert.IsTrue(dbg.StepOver());
            Assert.IsFalse(dbg.StepOver());

            dbg = new Debugger(complexTest);
            Assert.IsTrue(dbg.StepOver());
            Assert.IsTrue(dbg.StepOver());
            Assert.IsTrue(dbg.StepOver());
            Assert.IsTrue(dbg.StepOver());
            Assert.IsTrue(dbg.StepOver());
            Assert.IsFalse(dbg.StepOver());

            dbg = new Debugger(recurciveTest);
            Assert.IsTrue(dbg.StepOver());
            System.Exception expectedException = null;
            try
            {
                dbg.StepOver();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(grammarTest);
            Assert.IsTrue(dbg.StepOver());
            expectedException = null;
            try
            {
                dbg.StepOver();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(undefVarTest);
            Assert.IsTrue(dbg.StepOver());
            expectedException = null;
            try
            {
                dbg.StepOver();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);
        }

        [TestMethod()]
        public void GetStackTraceTest()
        {
            Debugger dbg = new Debugger(simpleTest);
            dbg.StepInto();
            Assert.AreEqual("\n1:   set a 2 (main)", dbg.GetStackTrace());
            dbg.StepInto();
            dbg.StepInto();

            dbg = new Debugger(subTest);
            dbg.StepInto();
            dbg.StepInto();
            Assert.AreEqual("\n1:   print a (foo)" +
                            "\n2:   call foo (main)" +
                            "\n1:   set a 2 (main)",
                            dbg.GetStackTrace());
            dbg.StepInto();

            dbg = new Debugger(complexTest);
            dbg.StepInto();
            dbg.StepInto();
            dbg.StepInto();
            dbg.StepInto();
            dbg.StepInto();
            dbg.StepInto();
            Assert.AreEqual("\n2:   print b (foo2)" +
                            "\n1:   set b 5 (foo2)" +
                            "\n5:   call foo2 (main)" +
                            "\n4:   print a (main)" +
                            "\n3:   call foo (main)" +
                            "\n2:   print a (main)" +
                            "\n1:   set a 2 (main)",
                            dbg.GetStackTrace());
            dbg.StepInto();

            dbg = new Debugger(recurciveTest);
            dbg.StepInto();
            System.Exception expectedException = null;
            try
            {
                dbg.StepOver();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(grammarTest);
            dbg.StepInto();
            expectedException = null;
            try
            {
                dbg.StepInto();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(undefVarTest);
            dbg.StepInto();
            expectedException = null;
            try
            {
                dbg.StepInto();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);
        }

        [TestMethod()]
        public void GetVariablesListTest()
        {
            Debugger dbg = new Debugger(simpleTest);
            dbg.StepInto();
            Assert.AreEqual("\na: 2", dbg.GetVariablesList());
            dbg.StepInto();
            dbg.StepInto();

            dbg = new Debugger(subTest);
            dbg.StepInto();
            dbg.StepInto();
            Assert.AreEqual("\na: 2", dbg.GetVariablesList());
            dbg.StepInto();

            dbg = new Debugger(complexTest);
            dbg.StepInto();
            dbg.StepInto();
            Assert.AreEqual("\na: 2", dbg.GetVariablesList());
            dbg.StepInto();
            dbg.StepInto();
            Assert.AreEqual("\na: 3", dbg.GetVariablesList());
            dbg.StepInto();
            dbg.StepInto();
            Assert.AreEqual("\na: 3" +
                            "\nb: 5",
                            dbg.GetVariablesList());
            dbg.StepInto();

            dbg = new Debugger(recurciveTest);
            dbg.StepInto();
            System.Exception expectedException = null;
            try
            {
                dbg.StepOver();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(grammarTest);
            dbg.StepInto();
            expectedException = null;
            try
            {
                dbg.StepInto();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);

            dbg = new Debugger(undefVarTest);
            dbg.StepInto();
            expectedException = null;
            try
            {
                dbg.StepInto();
            }
            catch (System.Exception ex)
            {
                expectedException = ex;
            }
            Assert.IsNotNull(expectedException);
        }
    }
}