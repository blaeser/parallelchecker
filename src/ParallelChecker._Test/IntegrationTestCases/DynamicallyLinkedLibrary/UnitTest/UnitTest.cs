using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace UnitTestSample
{
    [TestClass]
    public class UnitTest
    {
        private object sync = new object();
        private int race;

        // TODO: Support TestInitialize/TestCleanup

        [TestMethod]
        public void TestMethod1()
        {
            for (int i = 0; i < 10; i++)
            {
                Task.Run(() =>
                {
                    lock (sync)
                    {
                        race++;
                    }
                });
            }
            Assert.AreEqual(1, race);
        }

        [TestMethod]
        public void TestMethod2()
        {
            race++;
        }
    }
}
