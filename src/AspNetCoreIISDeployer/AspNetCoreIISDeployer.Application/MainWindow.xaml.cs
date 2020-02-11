using System.Windows;
using AspNetCoreIISDeployer.Application.ViewModels;

namespace AspNetCoreIISDeployer.Application
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}