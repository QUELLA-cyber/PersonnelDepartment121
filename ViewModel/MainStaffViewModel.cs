using PersonnelDepartment.ClassHelper;
using PersonnelDepartment.Commands;
using PersonnelDepartment.EmploymentRecord;
using PersonnelDepartment.OrdersForm;
using PersonnelDepartment.RepostsForm;
using PersonnelDepartment.Staff;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PersonnelDepartment.ViewModel
{
    public class MainStaffViewModel : BaseViewModel
    {
        private ObservableCollection<Employee> _employees;
        private ObservableCollection<Employee> _filteredEmployees;
        private string _searchText;
        private Employee _selectedEmployee;
        private Personal_card _currentUser;
        private Window _currentWindow;
        private string _userName;
        private string _userRole;
        private BitmapImage _userImage;

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
                FilterEmployees();
            }
        }

        public ObservableCollection<Employee> FilteredEmployees
        {
            get => _filteredEmployees;
            set
            {
                _filteredEmployees = value;
                OnPropertyChanged(nameof(FilteredEmployees));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterEmployees();
            }
        }

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));

                // Когда выбран сотрудник, активируем команду редактирования
                //if (_selectedEmployee != null)
                //{
                //    EditEmployeeCommand.Execute(null);
                //}
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddStaffCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand AssignRewardCommand { get; }
        public ICommand NavigateToMainWindowCommand { get; }
        public ICommand NavigateToEmploymentRecordCommand { get; }
        public ICommand NavigateToOrdersCommand { get; }
        public ICommand NavigateToReportsCommand { get; }
        private ICommand _editEmployeeCommand;
        public ICommand EditEmployeeCommand => _editEmployeeCommand ??= new RelayCommand<Employee>(OpenEditEmployeeWindow);

        public MainStaffViewModel(Personal_card currentUser, Window currentWindow)
        {
            _currentUser = currentUser;
            _currentWindow = currentWindow;
            _employees = LoadEmployeesFromDatabase();
            _filteredEmployees = new ObservableCollection<Employee>(_employees);

            UserName = $"{currentUser.Surname} {currentUser.Name}";
            UserRole = currentUser.Post.Title;

            if (currentUser.Photo != null)
            {
                UserImage = ByteArrayToBitmapImage(currentUser.Photo);
            }

            SearchCommand = new RelayCommand(Search);
            AddStaffCommand = new RelayCommand(AddStaff);
           // ExitCommand = new RelayCommand(Exit);
            AssignRewardCommand = new RelayCommand(AssignReward);
            NavigateToMainWindowCommand = new RelayCommand(NavigateToMainWindow);
            NavigateToEmploymentRecordCommand = new RelayCommand(NavigateToEmploymentRecord);
            NavigateToOrdersCommand = new RelayCommand(NavigateToOrders);
            NavigateToReportsCommand = new RelayCommand(NavigateToReports);
        }

        public void OpenEditEmployeeWindow(Employee employee)
        {
            if (employee != null)
            {
                // Открытие окна редактирования сотрудника
                EditStaff editWindow = new EditStaff(employee, _currentUser);
                editWindow.Show();
                _currentWindow.Close();
            }
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null) return null;

            using var stream = new System.IO.MemoryStream(byteArray);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }

        public string UserRole
        {
            get => _userRole;
            set
            {
                _userRole = value;
                OnPropertyChanged(nameof(UserRole));
            }
        }

        public BitmapImage UserImage
        {
            get => _userImage;
            set
            {
                _userImage = value;
                OnPropertyChanged(nameof(UserImage));
            }
        }

        private void Search()
        {
            FilterEmployees();
        }

        private void FilterEmployees()
        {
            string searchText = SearchText?.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                FilteredEmployees.Clear();
                foreach (var emp in Employees)
                {
                    FilteredEmployees.Add(emp);
                }
            }
            else
            {
                var filteredList = Employees.Where(emp =>
                    (!string.IsNullOrEmpty(emp.FirstName) && emp.FirstName.ToLower().Contains(searchText)) ||
                    (!string.IsNullOrEmpty(emp.LastName) && emp.LastName.ToLower().Contains(searchText)) ||
                    ($"{emp.FirstName} {emp.LastName}".ToLower().Contains(searchText))
                ).ToList();

                FilteredEmployees.Clear();
                foreach (var emp in filteredList)
                {
                    FilteredEmployees.Add(emp);
                }
            }
        }

        private ObservableCollection<Employee> LoadEmployeesFromDatabase()
        {
            ObservableCollection<Employee> employees = new ObservableCollection<Employee>();

            using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                connection.Open();
                var command = new SqlCommand("SELECT p.ID, p.Name AS FirstName, p.Surname AS LastName, p.Patronymic, po.Title AS Position, d.Title AS Department, p.Telephone AS Phone, p.Date_of_birth AS BirthDate, p.Photo FROM Personal_card p JOIN Post po ON p.Id_post = po.ID JOIN Department d ON po.Id_department = d.ID;", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Patronymic = reader.GetString(3),
                            Position = reader.GetString(4),
                            Department = reader.GetString(5),
                            Phone = reader.GetString(6),
                            BirthDate = reader.GetDateTime(7),
                            PhotoData = reader.IsDBNull(8) ? null : (byte[])reader[8]
                        });
                    }
                }
            }

            return employees;
        }

        private void AddStaff()
        {
            var addStaff = new AddStaff(_currentUser);
            addStaff.Show();
            _currentWindow.Close();
        }

        //private void Exit()
        //{
        //    var result = MessageBox.Show("Вы уверены, что хотите выйти из приложения?", "Подтверждение о закрытии", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        //    if (result == MessageBoxResult.Yes)
        //    {
        //        Application.Current.Shutdown();
        //    }
        //}

        private void AssignReward()
        {
            if (SelectedEmployee == null)
            {
                MessageBox.Show("Выберите сотрудника из списка.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var assignRewardForm = new AssignRewardForm(SelectedEmployee.ID, $"{SelectedEmployee.LastName} {SelectedEmployee.FirstName} {SelectedEmployee.Patronymic}");
            assignRewardForm.ShowDialog();
        }

        private void NavigateToMainWindow()
        {
            new MainWindowInspector(_currentUser).Show();
            Application.Current.Windows.Cast<Window>().FirstOrDefault(w => w is MainStaff)?.Close();
        }

        private void NavigateToEmploymentRecord()
        {
            new MainEmploymentRecord(_currentUser).Show();
            _currentWindow.Close();
        }

        private void NavigateToOrders()
        {
            new MainOrders(_currentUser).Show();
            _currentWindow.Close();
        }

        private void NavigateToReports()
        {
            new MainReports(_currentUser).Show();
            _currentWindow.Close();
        }
    }
}
