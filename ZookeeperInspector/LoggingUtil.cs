using log4net;

namespace ZookeeperInspector
{
    public class LoggingUtil
    {
        public static readonly ILog log = LogManager.GetLogger(typeof (MainWindow)) ;
    }
}