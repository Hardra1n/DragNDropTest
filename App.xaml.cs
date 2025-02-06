using System.Windows;
using NLog;

namespace DragNDropTask
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public App()
        {
            logger.Info("app started");
        }
    }

}
