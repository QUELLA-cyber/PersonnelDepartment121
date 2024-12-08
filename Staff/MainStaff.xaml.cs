using PersonnelDepartment.ClassHelper;
using PersonnelDepartment.ViewModel;
using System.Windows;

namespace PersonnelDepartment.Staff
{
    /// <summary>
    /// Логика взаимодействия для MainStaff.xaml
    /// </summary>
    public partial class MainStaff : Window
    {
        private Personal_card _currentUser;
        public MainStaff(Personal_card currentUser)
        {           
            _currentUser = currentUser;

            var viewModel = new MainStaffViewModel(currentUser, this);
            DataContext = viewModel;

            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EmployeeListView.SelectedItem is Employee selectedEmployee)
            {
                // Вызываем команду для редактирования сотрудника
                var viewModel = DataContext as MainStaffViewModel;
                viewModel?.OpenEditEmployeeWindow(selectedEmployee);
            }
        }
    }
}
