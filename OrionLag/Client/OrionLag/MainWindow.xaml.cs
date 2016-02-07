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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrionLag
{
    using System.Configuration;

    using OrionLag.Common.Configuration;
    using OrionLag.Common.Diagnosis;
    using OrionLag.Common.Utils;
    using OrionLag.Input.ViewModel;
    using OrionLag.Input.Views;
    using OrionLag.Output.ViewModels;
    using OrionLag.Output.Views;
    using OrionLag.Result.ViewModel;
    using OrionLag.Result.Views;
    using OrionLag.Server.Services;
    using OrionLag.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var logfile = ConfigurationLoader.GetAppSettingsValue("LogFile");

            var LoggingLevelsString = ConfigurationLoader.GetAppSettingsValue("LoggingLevels");
            LoggingLevels enumLowestTrace = LoggingLevels.Info;
            if (!string.IsNullOrEmpty(LoggingLevelsString))
            {
                if (Enum.TryParse(LoggingLevelsString, true, out enumLowestTrace))
                {
                    enumLowestTrace = enumLowestTrace;
                }
                else
                {
                    enumLowestTrace = LoggingLevels.Info;
                }
            }

            var fileAppsender = new FileAppender(logfile, enumLowestTrace, LoggingLevels.Trace);
            Log.AddAppender(fileAppsender);
        }

        private void OpenWindow(object windowContent, string title = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = string.Concat("[No Name (", windowContent.GetType().Name, ")]");
            }

            var window = new Window { Content = windowContent, Height = 600, Width = 900, Title = title };
            window.Show();
        }

        private void ViewMapItems_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InputMenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            UserInputControlViewModel viewModel = new UserInputControlViewModel(new InputPatitionersService(), new LagGenerator());
            OpenWindow(new UserInputControlView(viewModel), "Data input");
        }


        private void InputMenuItemFinFeltOpen_Click(object sender, RoutedEventArgs e)
        {
            LagOppsettViewModel viewModel = new LagOppsettViewModel(new LagOppsettDataService(),new List<Common.DataModel.Lag>(), -1, DateTime.MinValue);
            OpenWindow(new LagOppsettView(viewModel), "Data input");
            
        }

        private void InputMenuItemGrovFeltOpen_Click(object sender, RoutedEventArgs e)
        {
          
        }
        private void InputMenuItemResultsFinFeltOpen_Click(object sender, RoutedEventArgs e)
        {
            ResultViewModel model = new ResultViewModel(new ResultDataService());
            OpenWindow(new ResultView(model), "Resultater");

        }

        
    }
}
