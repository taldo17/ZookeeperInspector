using System;
using System.Collections.Generic;
using NUnit.Framework;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;


namespace ZookeeperInspector
{
    [TestFixture]
    class ZkActionImplTest : ZKBaseTest
    {
        [Test]
        public void NewZNodesChildrenShouldExistsAfterCreation()
        {
            String path = "/TestChildren";
            DeletePreviousData(path);
            CreateNodeWithdata(path, "Test Children");
            string path1 = path + "/Test1";
            string path2 = path + "/Test2";
            string path3 = path + "/Test3";
            string testData1 = "Test Children1";
            string testData2 = "Test Children2";
            string testData3 = "Test Children3";
            CreateNodeWithdata(path1, testData1);
            CreateNodeWithdata(path2, testData2);
            CreateNodeWithdata(path3, testData3);
            List<string> children = ZkActions.GetChildren("/TestChildren", false);
            Assert.AreEqual(3, children.Count);
            Assert.AreEqual(true, children.Contains("Test1"));
            Assert.AreEqual(true, children.Contains("Test2"));
            Assert.AreEqual(true, children.Contains("Test3"));
        }


        [Test]
        public void NewZNodeShouldExistsWithTheInsertedDataAfterCreation()
        {
            string path = "/TestCreate";
            DeletePreviousData(path);
            byte[] testRawData = TextConvertor.GetBytesFromTextAscii("Test Create");
            ZkActions.Create(path, testRawData, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);
            Stat stat = ZkActions.Exists(path, false);
            byte[] returnedRawData = ZkActions.GetData(path, false, stat);
            Assert.AreEqual("Test Create", TextConvertor.GetSTextFromBytesAscii(returnedRawData));
        }

        [Test]
        public void ZNodeShouldBeDeletedWhenDeleting()
        {
            string path = "/TestDelete";
            DeletePreviousData(path);
            Stat stat = CreateNodeWithdata(path, "Test Delete");
            byte[] returnedRawData = ZkActions.GetData(path, false, stat);
            Assert.AreEqual("Test Delete", TextConvertor.GetSTextFromBytesAscii(returnedRawData));
            ZkActions.Delete(path, stat.Version);
        }

        [Test]
        public void ExisitingdataShouldBeUpdatedOnUpdate()
        {
            string path = "/TestSetData";
            DeletePreviousData(path);
            Stat stat = CreateNodeWithdata(path, "Test Update");
            byte[] returnedRawData = ZkActions.GetData(path, false, stat);
            Assert.AreEqual("Test Update", TextConvertor.GetSTextFromBytesAscii(returnedRawData));
            ZkActions.SetData(path, TextConvertor.GetBytesFromTextAscii("Updated"), stat.Version);
            byte[] returnedUpdatedRawData = ZkActions.GetData(path, false, stat);
            Assert.AreEqual("Updated", TextConvertor.GetSTextFromBytesAscii(returnedUpdatedRawData));
        }

        private void DeletePreviousData(string path)
        {
            Stat stat = ZkActions.Exists(path, false);
            if (stat != null)
            {
                ZkActions.Delete(path, stat.Version);
            }
        }

        private Stat CreateNodeWithdata(string path, string testData)
        {
            byte[] testRawData = TextConvertor.GetBytesFromTextAscii(testData);
            ZkActions.Create(path, testRawData, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
            return ZkActions.Exists(path, false);
        }
    }
}