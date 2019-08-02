using System;
using DotNetStorage.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetStorage.Test
{
    [TestClass]
    public class UnitTestStorage
    {
        [TestMethod]
        public void StorageTest()
        {
            try
            {
                var config = new StorageConfig()
                {
                    BaseDirectory = "C:\\Windows\\Temp",
                    Filename = ".testStorage"
                };

                using (var localstorage = new Storage(config))
                {

                    localstorage.Store("Test", "TestValue", TimeSpan.FromMinutes(60));
                }

                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void StorageAppConfigTest()
        {
            try
            {
                using (var localstorage = new Storage())
                {
                    localstorage.Store("TestAppConfig", "TestTestAppConfigValue");

                    localstorage.Persist();
                }

                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
