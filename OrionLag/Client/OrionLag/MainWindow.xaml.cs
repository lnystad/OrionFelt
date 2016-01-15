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
    using OrionLag.Common.Utils;
    using OrionLag.Input.Views;
    using OrionLag.Output.ViewModels;
    using OrionLag.Output.Views;
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

            TargetOutputControlViewModel viewModel = new TargetOutputControlViewModel(new OppropDataService());
            OpenWindow(new TargetOutputControlView(viewModel), "Data input");
            
        }

        private void InputMenuItemGrovFeltOpen_Click(object sender, RoutedEventArgs e)
        {
          
        }
    }
}
