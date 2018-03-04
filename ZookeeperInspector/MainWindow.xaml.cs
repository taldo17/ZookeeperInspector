using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Org.Apache.Zookeeper.Data;
using ZooKeeperNet;
using static ZookeeperInspector.LoggingUtil;

namespace ZookeeperInspector
{
    public partial class MainWindow : Window
    {
        public static IZkActions ZkActions { get; set; }
        public ITextConvertor TextConvertor = new BasicTextConvertor();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnUpdateData_Click(object sender, RoutedEventArgs e)
        {
            string path = tbxPath.Text;
            string data = tbxData.Text;
            if (ShowConfigrmationDialog(path, data, false))
            {
                try
                {
                    Stat stat = ZkActions.Exists(path, false);
                    if (stat != null)
                    {
                        UpdateData(stat, data, path);
                        tblUpdateData.Text = string.Format("Data {0} was inserted to {1}", data, path);
                    }
                }
                catch (Exception ex)
                {
                    tblUpdateData.Text = string.Format("Somekind of error occurred {0}", ex.ToString());
                }
            }
        }

        private void UpdateData(Stat stat, string data, string path)
        {
            int version = stat.Version;
            byte[] configBytes = TextConvertor.GetBytesFromTextAscii(data);
            ZkActions.SetData(path, configBytes, version);
        }

        private void btnConnectToZK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread connectionThread = new Thread(new ParameterizedThreadStart(CreateZkActions));
                connectionThread.Start(tbxConnectionString.Text);
                Mouse.OverrideCursor = Cursors.Wait;
                if (!connectionThread.Join(TimeSpan.FromSeconds(15000)))
                {
                    connectionThread.Abort();
                    throw new Exception("More than 15 seconds had passed with no successfull connection. Please check zookeeper's status");
                }
                Mouse.OverrideCursor = Cursors.Arrow;
                tblConnection.Text = string.Format("Connected to: {0}", tbxConnectionString.Text);
            }
            catch (Exception ex)
            {
                tblConnection.Text = string.Format("Somekind of Error occurred {0}", ex.ToString());
            }
        }

        private void CreateZkActions(object zkEnsambleHostList)
        {
            try
            {
                ZkWatcher watcher = new ZkWatcher((string)zkEnsambleHostList);
                ZkActions = new ZkActionsImpl((string)zkEnsambleHostList, new TimeSpan(0, 0, 10), watcher);
                ZkWatcher.ResetEvent.WaitOne();
            }
            catch (Exception ex)
            {
                tblConnection.Text = string.Format("Somekind of Error occurred {0}", ex.ToString());
            }
        }

        private void btnCreation_Click(object sender, RoutedEventArgs e)
        {
            string path = tbxPath.Text;
            string data = tbxDataForNewNode.Text;

            if (ShowConfigrmationDialog(path, data, false))
            {
                try
                {
                    Stat stat = ZkActions.Exists(path, false);
                    if (stat == null)
                    {
                        byte[] configBytes = TextConvertor.GetBytesFromTextAscii(path);
                        string newPath = ZkActions.Create(path, configBytes,Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                        tblCreation.Text = string.Format("Path: {0} was created with data: {1}", newPath, data);
                    }
                    else
                    {
                        MessageBox.Show("The node already exists, you can update its data using the 'Update Data' Button");
                    }
                }
                catch (Exception ex)
                {
                    tblCreation.Text = string.Format("Somekind of Error occurred {0}", ex.ToString());
                }
            }

        }

        private void btnGetChildren_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = tbxPath.Text;
                Stat stat = ZkActions.Exists(path, false);
                List<string> children = null;
                if (stat != null)
                {
                    children = ZkActions.GetChildren(path, false);

                    StringBuilder sb = new StringBuilder();
                    foreach (string child in children)
                    {
                        sb.AppendLine(string.Format("{0}/{1}", tbxPath.Text, child));
                    }
                    tblGetChildren.Text = sb.ToString();
                }
                else
                {
                    tblGetChildren.Text = "The node does not exist";
                }
            }
            catch (Exception ex)
            {
                tblGetChildren.Text = string.Format("The node probably has no children - Got Exception: {0}", ex.ToString());
            }
        }


        private void btnDeleteNode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = tbxPath.Text;
                string configText = tbxData.Text;
                Stat stat = ZkActions.Exists(path, false);
                if (stat != null)
                {
                    int version = stat.Version;
                    ZkActions.Delete(path, version);
                    tblDeleteNode.Text = string.Format("Path {0} was deleted", path);
                }
                else
                {
                    tblDeleteNode.Text = "Node does not exist";
                }
            }
            catch (Exception ex)
            {
                tblDeleteNode.Text = string.Format("Somekind of Error occurred {0}", ex.ToString());
            }
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = tbxPath.Text;
                Stat stat = ZkActions.Exists(path, false);
                if (stat != null)
                {
                    byte[] data = ZkActions.GetData(path, false, stat);
                    string textData = TextConvertor.GetSTextFromBytesAscii(data);
                    if (textData != path)
                    {
                        tblGetData.Text = string.Format("Data on path {0} is {1}", path, textData);
                    }
                    else
                    {
                        tblGetData.Text = string.Format("The data is identical to the path (probably have not changed since creation)");
                    }
                }
                else
                {
                    tblGetData.Text = "Node does not exist";
                }
            }
            catch (Exception ex)
            {
                tblGetData.Text = string.Format("Somekind of Error occurred {0}", ex.ToString());
            }
        }

        private void btnUploadDataFromFile_Click(object sender, RoutedEventArgs e)
        {
            string path = tbxPath.Text;
            string filePath = tbxUploadDataFromFilePath.Text;
            if (ShowConfigrmationDialog(path, filePath, true))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        string data = reader.ReadToEnd();
                        byte[] rawData = TextConvertor.GetBytesFromTextAscii(data);
                        Stat stat = ZkActions.Exists(path, false);
                        if (stat != null)
                        {
                            int version = stat.Version;
                            byte[] configBytes = TextConvertor.GetBytesFromTextAscii(data);
                            ZkActions.SetData(path, configBytes, version);
                            tblUploadDataFromFile.Text = string.Format("Data {0} was inserted to {1}", filePath, path);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tblUploadDataFromFile.Text = string.Format("Somekind of Error occurred {0}", ex.ToString());
                }
            }
        }

        private void btnBrowseDataFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                string filter = "Text documents (*.txt)|*.txt |Configuration files(*.config, *.xml, *.json)|*.config;*.xml;*.json|All files (*.*)|*.*";
                dlg.Filter = filter;

                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    string filename = dlg.FileName;
                    tbxUploadDataFromFilePath.Text = filename;
                }
            }
            catch (Exception ex)
            {
                tblUploadDataFromFile.Text = string.Format("Somekind of Error occurred {0}", ex.ToString());
            }
        }

        private void btnSaveDataToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.FileName = "file.txt";
                string filter = "Text documents (*.txt)|*.txt |Configuration files(*.config, *.xml, *.json)|*.config;*.xml;*.json|All files (*.*)|*.*";
                dlg.Filter = filter;

                if (dlg.ShowDialog() == true)
                {
                    string filename = dlg.FileName;
                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        string data = tblGetData.Text;
                        sw.Write(data);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Somekind of Error occurred {0}", ex));
            }
        }

        private bool ShowConfigrmationDialog(string zkPath, string dataPath, bool realPath)
        {
            string dialogText = string.Empty;
            if (realPath)
            {
                dialogText = string.Format("Do you want to insert data from '{0}' to zookeeper's path {1}", dataPath, zkPath);
            }
            else
            {
                dialogText = string.Format("Do you want to insert data '{0}' to zookeeper's path: {1}", dataPath, zkPath);
            }
            string caption = "Confirmation";
            MessageBoxButton dialogue = MessageBoxButton.OKCancel;
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(dialogText, caption, dialogue, icon);

            switch (result)
            {
                case MessageBoxResult.OK:
                    return true;

                case MessageBoxResult.Cancel:
                    return false;
            }
            return false;
        }

        private void btnPerformance_Click(object sender, RoutedEventArgs e)
        {
            PerformanceWindow pw = new PerformanceWindow();
            pw.Show();
        }
    }
}
