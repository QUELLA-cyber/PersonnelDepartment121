using PersonnelDepartment.ViewModel;
using System.Windows;

namespace PersonnelDepartment
{

    public partial class MainWindowInspector : Window
    {
        private Personal_card _currentUser;

        public MainWindowInspector(Personal_card currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            var viewModel = new MainWindowInspectorViewModel(_currentUser, this);
            DataContext = viewModel;
        }

    }
}
