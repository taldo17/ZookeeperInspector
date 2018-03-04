using System.Threading;
using NUnit.Framework;

namespace ZookeeperInspector
{
    [TestFixture]
    public class StressTestHandlerTest : ZKBaseTest
    {
        [Test]
        public void StressTestZnodesShouldBeCreatedWhenRunningTest()
        {
            MainWindow.ZkActions = ZkActions;
            string path = "/StressTest";
            Assert.Null(ZkActions.Exists(path, false));
            StressTestHandler stressTestHandler = new StressTestHandler(2, path, TextConvertor);
            stressTestHandler.CreateStressTest();
            Thread.Sleep(1000);
            Assert.NotNull(ZkActions.Exists(path + "/0", false));
            Assert.NotNull(ZkActions.Exists(path + "/1", false));

            stressTestHandler.KeepRunning = false;
        } 
    }
}