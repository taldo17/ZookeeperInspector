using System;
using System.Threading;
using NUnit.Framework;

namespace ZookeeperInspector
{
    public class ZKBaseTest
    {
        public ITextConvertor TextConvertor = new BasicTextConvertor();
        public ZkActionsImpl ZkActions { get; set; }

        [SetUp]
        public void Setup()
        {
            ConnectToLocalTestZookeeper();
        }

        private void ConnectToLocalTestZookeeper()
        {
            Thread connectionThread = new Thread(new ParameterizedThreadStart(CreateZkActions));
            connectionThread.Start("127.0.0.1:2181");
            if (!connectionThread.Join(TimeSpan.FromSeconds(15000)))
            {
                connectionThread.Abort();
                throw new Exception(
                    "More than 15 seconds had passed with no successfull connection. Please check zookeeper's status");
            }
        }

        private void CreateZkActions(object connectionString)
        {
            string connectionStringStr = (string) connectionString;
            ZkWatcher watcher = new ZkWatcher(connectionStringStr);
            ZkActions = new ZkActionsImpl(connectionStringStr, new TimeSpan(0, 0, 10), watcher);
            ZkWatcher.ResetEvent.WaitOne();
        }
    }
}