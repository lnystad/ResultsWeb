using OrionLag.Input.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// <summary>
    /// Interaction logic for EditViewControl.xaml
    /// </summary>
    public partial class EditViewControl : UserControl
    {
        public EditViewControl()
        {
            InitializeComponent();
        }
        public EditViewControl(EditViewControlViewModel vieModel)
        {
            InitializeComponent();
            DataContext = vieModel;
        }

        private void FinFeltButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EditViewControlViewModel;
            if (viewModel != null)
            {
                viewModel.FinFeltButtonBase_OnClick(sender, e);
            }
        }

        private void Bane100mButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EditViewControlViewModel;
            if (viewModel != null)
            {
                viewModel.Bane100mButtonBase_OnClick(sender, e);
            }
        }

        private void GrovFeltButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EditViewControlViewModel;
            if (viewModel != null)
            {
                viewModel.GrovFeltButtonBase_OnClick(sender, e);
            }
        }

        private void Bane200mButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EditViewControlViewModel;
            if (viewModel != null)
            {
                viewModel.Bane200mButtonBase_OnClick(sender, e);
            }
        }

        private void CheckTotalButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as EditViewControlViewModel;
            if (viewModel != null)
            {
                viewModel.CheckTotalButtonBase_OnClick(sender, e);
            }
        }
    }
}
