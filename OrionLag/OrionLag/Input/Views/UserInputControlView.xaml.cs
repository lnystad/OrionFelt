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

namespace OrionLag.Input.Views
{
    using OrionLag.ViewModel;

    /// <summary>
    /// Interaction logic for UserInputControlView.xaml
    /// </summary>
    public partial class UserInputControlView : UserControl
    {
        public UserInputControlView()
        {
            InitializeComponent();
        }

        public UserInputControlView(UserInputControlViewModel vieModel)
        {
            InitializeComponent();
            DataContext = vieModel;
        }

        private void ReadInput_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as UserInputControlViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.OnReadInputbutton_OnClick(sender, e);
        }

        private void GenerateLag_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as UserInputControlViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.OnWriteInputbutton_OnClickOn(sender, e);
        }
    }
}
