using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;

namespace ZookeeperInspector
{
    public class StressTestHandler
    {
        private static readonly string STRESSING = "Stressing";
        private readonly ITextConvertor textConvertor;
        public IZkActions ZkActions;
        private static readonly int INTERVAL_IN_MILLISECONDS = 50;
        public int NumberOfThreads { get; set; }

        public string Path { get; set; }

        public bool KeepRunning { get; set; }

        public StressTestHandler(int numberOfThreads, string path, ITextConvertor textConvertor)
        {
            this.textConvertor = textConvertor;
            NumberOfThreads = numberOfThreads;
            Path = path;
            ZkActions = MainWindow.ZkActions;
            KeepRunning = true;
        }

        public void CreateStressTest()
        {
            for (int i = 0; i < NumberOfThreads; i++)
            {
                Thread thread = new Thread(new ThreadStart(StressSetData));
                thread.Name = i.ToString();
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void StressSetData()
        {
            DeletePreviousData();
            ZkActions.Create(Path, textConvertor.GetBytesFromTextAscii(STRESSING), Ids.OPEN_ACL_UNSAFE,
                        CreateMode.Persistent);
            string path = string.Concat(Path, "/", Thread.CurrentThread.Name);
            while (KeepRunning)
            {
                Stat stat = ZkActions.Exists(path, false);
                if (stat != null)
                {
                    int version = stat.Version;
                    byte[] configBytes = textConvertor.GetBytesFromTextAscii(STRESSING);
                    ZkActions.SetData(path, configBytes, version);
                }
                else
                {
                    ZkActions.Create(path, textConvertor.GetBytesFromTextAscii(STRESSING), Ids.OPEN_ACL_UNSAFE,
                        CreateMode.Persistent);
                }
                Thread.Sleep(INTERVAL_IN_MILLISECONDS);
            }
        }

        private void DeletePreviousData()
        {
            Stat stat = ZkActions.Exists(Path, false);
            if (stat != null)
            {
                ZkActions.Delete(Path, stat.Version);
            }
        }
    }
}