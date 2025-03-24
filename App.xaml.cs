
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace LFFSSK
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string _TraceCategory = "LFFSSK.App";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {

                if (IsCurrentProcessOpen())
                {
                    MessageBox.Show("This application is currently running!", Process.GetCurrentProcess().ProcessName, MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    throw new Exception(string.Format("[{0}]This application is currently running!", Process.GetCurrentProcess().ProcessName));
                }

                GeneralVar.MainWindowVM = new ViewModel.MainWindowViewModel();
                GeneralVar.MainWindowVW = new MainWindow();
                GeneralVar.MainWindowVW.DataContext = GeneralVar.MainWindowVM;
                
                GeneralVar.MainWindowVW.Show();

            }
            catch (Exception ex) 
            {
                App.Current.Shutdown(0);
                Trace.WriteLineIf(true, string.Format("[Error] Application_Startup = {0}", ex.ToString()), _TraceCategory);
            }
        }

        #region Prevent application double startup
        private bool IsCurrentProcessOpen()
        {
            Process currentProcess = Process.GetCurrentProcess();
            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();
            if (runningProcess != null)
                return true;

            return false;
        }
        #endregion


        private void Application_Exit(object sender, ExitEventArgs e)
        {

            try
            {

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(true, string.Format("[Error] Application_Exit: {0}", ex.ToString()), _TraceCategory);
            }
            finally
            {
                Environment.Exit(0);
            }

        }
    }
}
