using System.Collections.Generic;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;

namespace ZookeeperInspector
{
    public interface IZkActions
    {
        string Create(string path, byte[] data, List<ACL> acl, CreateMode createMode);
        byte[] GetData(string path, bool watch, Stat stat);
        Stat SetData(string path, byte[] data, int version);
        List<string> GetChildren(string path, bool watch);
        void Delete(string path, int version);
        Stat Exists(string path, bool watch);
    }
}