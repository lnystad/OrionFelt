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

namespace OrionLag.Output.Views
{
    using OrionLag.Output.ViewModels;

    /// <summary>
    /// Interaction logic for TargetOutputControlView.xaml
    /// </summary>
    public partial class TargetOutputControlView : Page
    {
        public TargetOutputControlView()
        {
            InitializeComponent();
        }
        public TargetOutputControlView(TargetOutputControlViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
