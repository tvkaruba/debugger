using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Debugger.Tests
{
    [TestClass()]
    public class DebuggerTests
    {
        private string simpleTest = "sub main\n" +
                                    "  set a 2\n" +
                                    "  print a\n";

        private string subTest = "sub foo" +
                                 "  print a" +
                                 "" +
                                 "sub main" +
                                 "  set a 2" +
                                 "  call foo";

        private string complexTest = "sub foo" +
                                     "  set a 3" +
                                     "" +
                                     "sub main" +
                                     "  set a 2" +
                                     "  print a" +
                                     "  call foo" +
                                     "  print a" +
                                     "  call foo2" +
                                     "" +
                                     "sub foo2" +
                                     "  set b 5" +
                                     "  print b";

        private string recurciveTest = "sub main" +
                                       "  set a 2" +
                                       "  call foo" +
                                       "" +
                                       "sub foo" +
                                       "  print a" +
                                       "  call foo";

        private string grammarTest = "sub main" +
                                     "  set a 2" +
                                     "  prnt a";

        private string undefVarTest = "sub main" +
                                     "  set a 2" +
                                     "  print b";

        [TestMethod()]
        public void StepIntoTest()
        {
            Debugger dbg = new Debugger(simpleTest);
            Assert.IsTrue(dbg.StepInto());
            Assert.AreEqual("\na: 2", dbg.GetVariablesList());
            Assert.AreEqual("\n1:   set a 2 (main)", dbg.GetStackTrace());
            Assert.IsTrue(dbg.StepInto());
            Assert.IsFalse(dbg.StepInto());

            //dbg = new Debugger(subTest);
            //Assert.IsTrue(dbg.StepInto());
            //Assert.IsTrue(dbg.StepInto());
            //Console.WriteLine(dbg.GetStackTrace());
            //Assert.Equals(dbg.GetStackTrace(), "\n1:   print a (foo)" +
            //                                            "\n2:   call foo (main)" +
            //                                            "\n1:   set a 2 (main)");
            //Assert.Equals(dbg.GetVariablesList(), "\na: 2");
            //Assert.IsFalse(dbg.StepInto());

            //dbg = new Debugger(complexTest);
            //Assert.IsTrue(dbg.StepInto());
            //Assert.IsTrue(dbg.StepInto());
            //Assert.Equals(dbg.GetVariablesList(), "\na: 2");
            //Assert.IsTrue(dbg.StepInto());
            //Assert.IsTrue(dbg.StepInto());
            //Assert.Equals(dbg.GetVariablesList(), "\na: 3");
            //Assert.IsTrue(dbg.StepInto());
            //Assert.IsTrue(dbg.StepInto());
            //Assert.Equals(dbg.GetVariablesList(), "\na: 3" +
            //                                               "\nb: 5");
            //Assert.Equals(dbg.GetStackTrace(), "\n1:   print b (foo2)" +
            //                                            "\n1:   set b 5 (foo2)" + 
            //                                            "\n2:   call foo2 (main)" + 
            //                                            "\n1:   print a (main)" + 
            //                                            "\n2:   call foo (main)" +
            //                                            "\n1:   print a (main)" +
            //                                            "\n1:   set a 2 (main)");
            //Assert.IsFalse(dbg.StepInto());
        }

        //[TestMethod()]
        //public void StepOverTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void DisplayStackTraceTest()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod()]
        //public void DisplayVariablesTest()
        //{
        //    Assert.Fail();
        //}
    }
}