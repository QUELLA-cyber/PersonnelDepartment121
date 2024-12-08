using PersonnelDepartment.ViewModel;
using System.Windows;

namespace PersonnelDepartment.EmploymentRecord
{
    public partial class MainEmploymentRecord : Window
    {
        private Personal_card _currentUser;

        public MainEmploymentRecord(Personal_card currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            var viewModel = new MainEmploymentRecordViewModel(_currentUser, this);
            DataContext = viewModel;
        }
    }
}
