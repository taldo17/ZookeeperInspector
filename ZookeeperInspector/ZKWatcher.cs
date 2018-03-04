using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;
using static ZookeeperInspector.LoggingUtil;

namespace ZookeeperInspector
{
    public class ZkWatcher : IWatcher
    {
        public IZkActions BasicEventListner { get; set; }

        public static AutoResetEvent ResetEvent = new AutoResetEvent(false);

        public ZkWatcher(string zkEnsambleHostList)
        {
            BasicEventListner = new ZkActionsImpl((string) zkEnsambleHostList, new TimeSpan(0, 0, 10), this);
            this.BasicEventListner.GetData("/", true, new Stat());
        }


        public void Process(WatchedEvent @event)
        {
            if (@event.State == KeeperState.SyncConnected)
            {
                log.Info(String.Format("An event of state {0} arrived", @event.State));
                ResetEvent.Set();
            }
            BasicEventListner.GetData("/", true, new Stat());
        }
    }
}
