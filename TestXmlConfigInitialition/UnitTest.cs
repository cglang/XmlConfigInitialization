using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
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
            var decide = config.SetValue("key", "value");
            var text = config.GetValue("key");
            var delete = config.DeleteValue("key");
            var count = config.GetAllKey().Count;

            Assert.AreEqual(true, decide);
            Assert.AreEqual("value", text);
            Assert.AreEqual(true, delete);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public async Task TestAsync()
        {
            var names = new[] { "张三", "李四", "王五" };
            var decide = await config.SetValueAsync("key2", names);
            var names2 = await config.GetValueAsync<string[]>("key2");
            var delete = await config.DeleteValueAsync("key2");

            Assert.AreEqual(true, decide);
            Assert.AreEqual(true, delete);
            Assert.AreEqual(names[0], names2[0]);
        }

        [TestMethod]
        public void TestInit()
        {
            config.SetValue("name", "cglang");
            config.SetValue("age", "22");

            config.SetValue("test1", "value1", "node1");
            config.SetValue("test2", "value2", "node1");

            config.SetValue("test1", "value3", "node2");
            config.SetValue("test2", "value4", "node2");
        }
    }
}