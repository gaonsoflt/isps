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
using System.Windows.Shapes;

namespace AsyncSocketServerWPF
{
    /// <summary>
    /// ConfWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ConfWindow : Window
    {

        public ConfWindow()
        {
            InitializeComponent();
            this.DataContext = new ConfWindowModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckVendor();
        }

        private void CheckVendor()
        {
            var viewModel = this.DataContext as ConfWindowModel;
            if (viewModel.Vendor == "PostgreSql")
            {
                tbPort.IsEnabled = false;
            }
            else
            {
                tbPort.IsEnabled = true;

            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("저장하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var viewModel = this.DataContext as ConfWindowModel;
                viewModel.SaveForm();
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("취소하시겠습니까?", "알림", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void cbVendor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckVendor();
        }
    }
}
