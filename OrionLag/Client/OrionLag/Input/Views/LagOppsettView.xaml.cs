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
    using OrionLag.Input.ViewModel;

    /// <summary>
    /// Interaction logic for LagOppsett.xaml
    /// </summary>
    public partial class LagOppsettView : UserControl
    {
        public LagOppsettView()
        {
            InitializeComponent();
        }

        public LagOppsettView(LagOppsettViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.GridManager = dataGrid;
        }

        private void SortButton_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as LagOppsettViewModel;
            if (viewModel != null)
            {
                viewModel.SortButton_OnClick(sender, e);
            }
        }

        private void LeonExportButton_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as LagOppsettViewModel;
            if (viewModel != null)
            {
                viewModel.LeonExportButton_OnClick(sender, e);
            }
        }
        

        private void SetTimesButton_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as LagOppsettViewModel;
            if (viewModel != null)
            {
                viewModel.SetTimesButton_OnClick(sender, e);
            }
        }

        private void GenerateFilesButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as LagOppsettViewModel;
            if (viewModel != null)
            {
                viewModel.GenerateFilesButtonBase_OnClick(sender, e);
            }
        }

        private void ReadDatabaseButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as LagOppsettViewModel;
            if (viewModel != null)
            {
                viewModel.ReadDatabaseButtonBase_OnClick(sender, e);
            }
        }
        
    }
}
