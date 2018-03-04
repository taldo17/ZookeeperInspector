using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;
using static ZookeeperInspector.LoggingUtil;

namespace ZookeeperInspector
{
    public class ZkActionsImpl : IZkActions
    {
        public ZooKeeper Zk { get; set; }
        public string ConnectionString { get; set; }

        public TimeSpan Timeout { get; set; }

        

        public ZkActionsImpl(string conString, TimeSpan to, IWatcher watcher)
        {
            ConnectionString = conString;
            Timeout = to;
            Zk = new ZooKeeper(ConnectionString, Timeout, watcher);
        }

        public string Create(string path, byte[] data, List<ACL> acl, CreateMode createMode)
        {
            string pathValue = null;
            try
            {
                pathValue = Zk.Create(path, data, acl, createMode);
            }
            catch (Exception e)
            {
                log.Error(String.Format("An error occured while trying to create. Exception: {0}", e.ToString()));
            }
            return pathValue;
        }

        public byte[] GetData(string path, bool watch, Stat stat)
        {
            byte[] returnBytes = null;
            try
            {
                returnBytes = Zk.GetData(path, watch, stat);
            }
            catch (Exception e)
            {
                log.Error(String.Format("An error occured while trying to get data. Exception: {0}", e.ToString()));
            }
            return returnBytes;
        }

        public Stat SetData(string path, byte[] data, int version)
        {
            Stat returnBytes = null;
            try
            {
                returnBytes = Zk.SetData(path, data, version);
            }
            catch (Exception e)
            {
                log.Error(String.Format("An error occured while trying to set data. Exception: {0}", e.ToString()));
            }
            return returnBytes;
        }

        public List<string> GetChildren(string path, bool watch)
        {
            List<string> children = null;
            try
            {
                children = Zk.GetChildren(path, watch).ToList();
            }
            catch (Exception e)
            {
                log.Error(String.Format("An error occured while trying to get children. Exception: {0}", e.ToString()));
            }
            return children;
        }

        public void Delete(string path, int version)
        {
            try
            {
                Zk.Delete(path, version);
            }
            catch (Exception e)
            {
                log.Error(String.Format("An error occured while trying to delete. Exception: {0}", e.ToString()));
            }
        }

        public Stat Exists(string path, bool watch)
        {
            Stat stat = null;
            try
            {
                stat = Zk.Exists(path, watch);
            }
            catch (Exception e)
            {
                log.Error(String.Format("An error occured while trying to check if exists. Exception: {0}", e.ToString()));
            }
            return stat;
        }
    }
}
