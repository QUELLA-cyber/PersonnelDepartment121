using PersonnelDepartment.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PersonnelDepartment
{

    public partial class AutorizationForm : Window
    {
        public AutorizationForm()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (DataContext is AuthorizationViewModel viewModel)
            {
                viewModel.Password = passwordBox?.Password;
            }
        }

    }
}
