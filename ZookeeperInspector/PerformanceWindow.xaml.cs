using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZookeeperInspector
{
    /// <summary>
    /// Interaction logic for PerformanceWindow.xaml
    /// </summary>
    public partial class PerformanceWindow : Window
    {
        private StressTestHandler stressTestHandler;
        public PerformanceWindow()
        {
            InitializeComponent();
        }

        private void btnCreateLoad_Click(object sender, RoutedEventArgs e)
        {
            int numberOfThreads = Convert.ToInt32(tbxNumberOfThreads.Text);
            string zookeeperTestPath = tbxZookeeperTestPath.Text;
            tblStressStatus.Text = string.Format("Started stressing Zookeeper on path {0}. Number of threads: {1}", zookeeperTestPath, numberOfThreads);
            stressTestHandler = new StressTestHandler(numberOfThreads,  zookeeperTestPath, new BasicTextConvertor());
            stressTestHandler.CreateStressTest();
        }

        private void btnAbortLoad_Click(object sender, RoutedEventArgs e)
        {
            if (stressTestHandler != null)
            {
                stressTestHandler.KeepRunning = false;
                tblStressStatus.Text = string.Format("Aborting all executing {0} threads", tbxNumberOfThreads.Text);
            }
        }
    }
}
