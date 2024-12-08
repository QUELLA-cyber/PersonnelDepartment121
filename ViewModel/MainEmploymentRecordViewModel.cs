using PersonnelDepartment.ClassHelper;
using PersonnelDepartment.Commands;
using PersonnelDepartment.EmploymentRecord;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Imaging;
using PersonnelDepartment.Staff;
using PersonnelDepartment.OrdersForm;
using PersonnelDepartment.RepostsForm;

namespace PersonnelDepartment.ViewModel
{
    public class MainEmploymentRecordViewModel : BaseViewModel
    {
        private Personal_card _currentUser;
        private Window _currentWindow;
        private string _userName;
        private string _userRole;
        private BitmapImage _userImage;
        private Employee _selectedEmployee;
        private ObservableCollection<Employee> _employees;
        private ObservableCollection<Employee> _filteredEmployees;
        private ObservableCollection<EntryInWorkBook> _employmentRecords;

        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
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

        public ObservableCollection<EntryInWorkBook> EmploymentRecords
        {
            get => _employmentRecords;
            set
            {
                _employmentRecords = value;
                OnPropertyChanged(nameof(EmploymentRecords));
            }
        }

        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                LoadEmploymentRecordsForSelectedEmployee();
            }
        }

        public ICommand FilterEmployeesCommand { get; set; }
        public ICommand AddRecordCommand { get; set; }
        public ICommand NavigateToMainWindowCommand { get; set; }
        public ICommand NavigateToOrdersCommand { get; set; }
        public ICommand NavigateToReportsCommand { get; set; }
        public ICommand NavigateToStaffCommand { get; set; }
        public ICommand ExitApplicationCommand { get; set; }

        public MainEmploymentRecordViewModel(Personal_card currentUser, Window currentWindow)
        {
            _currentUser = currentUser;
            _currentWindow = currentWindow;
            UserName = $"{currentUser.Surname} {currentUser.Name}";
            UserRole = currentUser.Post.Title;

            if (currentUser.Photo != null)
            {
                UserImage = ByteArrayToBitmapImage(currentUser.Photo);
            }

            Employees = LoadEmployeesFromDatabase();
            FilteredEmployees = new ObservableCollection<Employee>(Employees);
            EmploymentRecords = new ObservableCollection<EntryInWorkBook>();

            FilterEmployeesCommand = new RelayCommand<string>(FilterEmployees);
            AddRecordCommand = new RelayCommand(AddRecord);
            NavigateToMainWindowCommand = new RelayCommand(NavigateToMainWindow);
            NavigateToOrdersCommand = new RelayCommand(NavigateToOrders);
            NavigateToReportsCommand = new RelayCommand(NavigateToReports);
            NavigateToStaffCommand = new RelayCommand(NavigateToStaff);
            ExitApplicationCommand = new RelayCommand(ExitApplication);
            
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

        public void FilterEmployees(string searchText)
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

        private ObservableCollection<Employee> LoadEmployeesFromDatabase()
        {
            var employees = new ObservableCollection<Employee>();

            using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                connection.Open();
                var command = new SqlCommand("SELECT p.ID, p.Name AS FirstName, p.Surname AS LastName, p.Patronymic, po.Title AS Position, d.Title AS Department, p.Telephone AS Phone, p.Date_of_birth AS BirthDate, p.Photo FROM Personal_card p JOIN Post po ON p.Id_post = po.ID JOIN Department d ON po.Id_department = d.ID;", connection);
                using var reader = command.ExecuteReader();
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

            return employees;
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

        private void LoadEmploymentRecordsForSelectedEmployee()
        {
            if (SelectedEmployee == null) return;

            EmploymentRecords.Clear();
            using var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework");
            connection.Open();
            var command = new SqlCommand(@"SELECT e.ID, e.Date, e.Reason, m.Title AS MixingTitle FROM Entry_in_the_work_book e LEFT JOIN Mixing m ON e.Id_mixing = m.ID WHERE e.Id_personal_card = @EmployeeId", connection);
            command.Parameters.AddWithValue("@EmployeeId", SelectedEmployee.ID);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                EmploymentRecords.Add(new EntryInWorkBook
                {
                    ID = reader.GetInt32(0),
                    Date = reader.GetDateTime(1),
                    Reason = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    MixingTitle = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                });
            }
        }

        private void AddRecord()
        {
            if (SelectedEmployee == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника.");
                return;
            }

            AddRecordWindow addRecordWindow = new AddRecordWindow(SelectedEmployee.ID);
            addRecordWindow.RecordAdded += (s, args) =>
            {
                LoadEmploymentRecordsForSelectedEmployee();
            };
            addRecordWindow.Show();
        }

        private void NavigateToMainWindow()
        {
            MainStaff mainStaffWindow = new MainStaff(_currentUser);
            mainStaffWindow.Show();
            _currentWindow.Close();
        }

        private void NavigateToOrders()
        {
            MainOrders mainOrdersWindow = new MainOrders(_currentUser);
            mainOrdersWindow.Show();
            _currentWindow.Close();
        }

        private void NavigateToReports()
        {
            MainReports mainReportsWindow = new MainReports(_currentUser);
            mainReportsWindow.Show();
            _currentWindow.Close();
        }

        private void NavigateToStaff()
        {
            MainStaff mainStaffWindow = new MainStaff(_currentUser);
            mainStaffWindow.Show();
            _currentWindow.Close();
        }

        private void ExitApplication()
        {
            MessageBox.Show("Вы уверены, что хотите выйти из приложения?", "Подтверждение о закрытии", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            Application.Current.Shutdown();
        }
    }
}
