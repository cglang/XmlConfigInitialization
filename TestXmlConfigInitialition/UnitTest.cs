using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlConfigInitialization;

namespace TestXmlConfigInitialition
{
    [TestClass]
    public class UnitTest
    {
        readonly XmlConfig config = XmlConfigFactory.GetSingletonInstance();

        [TestMethod]
        public void TestBasic()
        {
            var decide = config.SetValue("test", "test");
            var text = config.GetValue("test");
            var delete = config.DeleteValue("test");
            var count = config.GetAllKey().Count;

            Assert.AreEqual(true, decide);
            Assert.AreEqual("test", text);
            Assert.AreEqual(true, delete);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void TestAsync()
        {
            var names = new[] { "张三", "李四", "王五" };
            var decide = config.SetValue("test2", names);
            var names2 = config.GetValue<string[]>("test2");

            Assert.AreEqual(true, decide);
            Assert.AreEqual(names[0], names2[0]);
        }
    }
}