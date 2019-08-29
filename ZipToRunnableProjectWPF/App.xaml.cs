using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ZipToRunnableProjectWPF.Common;

namespace ZipToRunnableProjectWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            //bool startMinimized = false;
            //for (int i = 0; i != e.Args.Length; ++i)
            //{
            //    if (e.Args[i] == "/StartMinimized")
            //    {
            //        startMinimized = true;
            //    }
            //}

            StaticVariables.SelectedFiles = e.Args;
            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            
            //if (startMinimized)
            //{
            //    mainWindow.WindowState = WindowState.Minimized;
            //}
            mainWindow.Show();
        }
    }
}
