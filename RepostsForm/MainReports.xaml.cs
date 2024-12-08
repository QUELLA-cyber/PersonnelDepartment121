using ClosedXML.Excel;
using Dapper;
using PersonnelDepartment.ClassHelper;
using PersonnelDepartment.EmploymentRecord;
using PersonnelDepartment.OrdersForm;
using PersonnelDepartment.Staff;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace PersonnelDepartment.RepostsForm
{
    /// <summary>
    /// Логика взаимодействия для MainReports.xaml
    /// </summary>
    public partial class MainReports : Window
    {
        private string connectionString = "data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework";
        private Personal_card _currentUser; // Хранение данных текущего пользователя
        public MainReports(Personal_card currentUser)
        {
            _currentUser = currentUser;
            InitializeComponent();
            LoadDepartmentsIntoComboBox();
            DisplayUserInfo(); // Отображение информации о текущем пользователе
        }

        private void DisplayUserInfo()
        {
            if (_currentUser != null)
            {
                Namelbl.Content = $"{_currentUser.Surname} {_currentUser.Name}";
                Rolelbl.Content = _currentUser.Post.Title;
            }
        }

        public class Department
        {
            public int ID { get; set; }
            public string Title { get; set; }
        }

        private void LoadDepartmentsIntoComboBox()
        {
            List<Department> departments = LoadDepartments();

            // Привязка данных к ComboBox
            PostCmd.ItemsSource = departments;
            PostCmd.DisplayMemberPath = "Title";   // Отображаемое название
            PostCmd.SelectedValuePath = "ID";      // Значение элемента (ID отдела)
        }

        public List<Department> LoadDepartments()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID, Title FROM Department";
                return connection.Query<Department>(query).AsList();
            }
        }

        private void StaffBtn_Click(object sender, RoutedEventArgs e)
        {
            new MainStaff(_currentUser).Show();
            Close();
        }

        private void ReportsCmd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReportsCmd.SelectedIndex == 1)
            {
                PostSkP.Visibility = Visibility.Visible;
            }
            else
            {
                PostSkP.Visibility = Visibility.Collapsed;
            }
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int reportType = ReportsCmd.SelectedIndex + 1;
                DateTime startDate = StartDatePicker.SelectedDate ?? DateTime.Now;
                DateTime endDate = EndDatePicker.SelectedDate ?? DateTime.Now;

                string department = null;
                int? departmentId = null;
                if (reportType == 2 && PostCmd.SelectedValue != null)
                {
                    departmentId = (int)PostCmd.SelectedValue;
                    department = (PostCmd.SelectedItem as Department)?.Title;
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    List<Employee> employees = GetEmployeeData(connection, reportType, startDate, endDate, departmentId);
                    GenerateReport(employees, reportType, startDate, endDate, department);
                }

                MessageBox.Show("Отчёт успешно создан и сохранён как 'Отчёт.xlsx'.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public List<Employee> GetEmployeeData(IDbConnection connection, int reportType, DateTime startDate, DateTime endDate, int? departmentId)
        {
            string query;
            switch (reportType)
            {
                case 1:
                    query = @"SELECT Personal_card.ID,
                         Personal_card.Name AS FirstName,
                         Personal_card.Surname AS LastName,
                         Personal_card.Patronymic,
                         Post.Title AS Position,
                         Department.Title AS Department,
                         Personal_card.Telephone AS Phone,
                         Personal_card.Date_of_birth AS BirthDate
                  FROM Personal_card
                  JOIN Post ON Personal_card.Id_post = Post.ID
                  JOIN Department ON Post.Id_department = Department.ID";
                    break;
                case 2:
                    query = @"SELECT Personal_card.ID,
                         Personal_card.Name AS FirstName,
                         Personal_card.Surname AS LastName,
                         Personal_card.Patronymic,
                         Post.Title AS Position,
                         Department.Title AS Department,
                         Personal_card.Telephone AS Phone,
                         Personal_card.Date_of_birth AS BirthDate,
                         Entry_in_the_work_book.Date AS DateOfHire
                  FROM Personal_card
                  JOIN Post ON Personal_card.Id_post = Post.ID
                  JOIN Department ON Post.Id_department = Department.ID
                  JOIN Entry_in_the_work_book ON Entry_in_the_work_book.Id_personal_card = Personal_card.ID
                  JOIN Mixing ON Entry_in_the_work_book.Id_mixing = Mixing.ID
                  WHERE Department.ID = @DepartmentId AND Mixing.Title = 'Приём'";
                    break;
                case 3:
                    query = @"SELECT Personal_card.ID,
                         Personal_card.Name AS FirstName,
                         Personal_card.Surname AS LastName,
                         Personal_card.Patronymic,
                         Post.Title AS Position,
                         Department.Title AS Department,
                         Personal_card.Telephone AS Phone,
                         Personal_card.Date_of_birth AS BirthDate
                  FROM Personal_card
                  JOIN Post ON Personal_card.Id_post = Post.ID
                  JOIN Department ON Post.Id_department = Department.ID
                  WHERE Personal_card.Children > 0";
                    break;
                case 4:
                    query = @"SELECT Personal_card.ID,
                         Personal_card.Name AS FirstName,
                         Personal_card.Surname AS LastName,
                         Personal_card.Patronymic,
                         Post.Title AS Position,
                         Department.Title AS Department,
                         Personal_card.Telephone AS Phone,
                         Personal_card.Date_of_birth AS BirthDate
                  FROM Personal_card
                  JOIN Post ON Personal_card.Id_post = Post.ID
                  JOIN Department ON Post.Id_department = Department.ID
                  WHERE Personal_card.Military_service = 1";
                    break;
                default:
                    throw new ArgumentException("Неверный тип отчёта");
            }

            return connection.Query<Employee>(query, new { DepartmentId = departmentId }).AsList();
        }

        public void GenerateReport(List<Employee> employees, int reportType, DateTime startDate, DateTime endDate, string department)
        {
            string templatePath = GetTemplatePath(reportType);
            var workbook = new XLWorkbook(templatePath);
            var worksheet = workbook.Worksheet(1);

            // Установка заголовка отчёта с указанием номера кейса
            worksheet.Cell("A1").Value = $"Отчёт ({reportType})";

            // Установка начала и конца периода
            worksheet.Cell("B9").Value = startDate.ToShortDateString();
            worksheet.Cell("C9").Value = endDate.ToShortDateString();

            // Установка названия отдела, если это отчёт по отделам
            if (reportType == 2 && department != null)
            {
                worksheet.Cell("C6").Value = department;
            }

            // Начальная строка для записей сотрудников
            int currentRow = 15;

            foreach (var employee in employees)
            {
                worksheet.Cell($"A{currentRow}").Value = $"{employee.LastName} {employee.FirstName} {employee.Patronymic}";

                if (reportType == 1) // Список сотрудников
                {
                    worksheet.Range($"B{currentRow}:C{currentRow}").Merge().Value = employee.Position;
                    worksheet.Range($"D{currentRow}:E{currentRow}").Merge().Value = employee.Department;
                }
                else if (reportType == 2) // Список сотрудников по отделам
                {
                    worksheet.Cell($"B{currentRow}").Value = employee.Position;
                    worksheet.Cell($"C{currentRow}").Value = employee.BirthDate.ToShortDateString();
                    // Убираем заполнение столбца D для названия отдела
                }
                else if (reportType == 3) // Список сотрудников с детьми
                {
                    worksheet.Cell($"B{currentRow}").Value = employee.BirthDate.ToShortDateString();
                    worksheet.Cell($"C{currentRow}").Value = employee.Position;
                    worksheet.Cell($"D{currentRow}").Value = employee.Department;
                }
                else if (reportType == 4) // Список военнообязанных сотрудников
                {
                    worksheet.Cell($"B{currentRow}").Value = employee.BirthDate.ToShortDateString();
                    worksheet.Cell($"C{currentRow}").Value = employee.Department;
                    worksheet.Cell($"D{currentRow}").Value = employee.Position;
                }

                currentRow++;
            }

            // Добавление строки с общим количеством сотрудников в отделе для отчета типа 2
            if (reportType == 2)
            {
                worksheet.Cell($"C{currentRow}").Value = "Общее количество:";
                worksheet.Cell($"D{currentRow}").Value = employees.Count;
            }

            // Диалоговое окно для сохранения файла
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Сохранить отчёт",
                FileName = $"Отчёт_{reportType}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                workbook.SaveAs(saveFileDialog.FileName);
                MessageBox.Show("Отчёт успешно сохранён.", "Сохранение отчёта", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Сохранение отменено.", "Сохранение отчёта", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static string GetTemplatePath(int reportType)
        {
            string templatePath;

            switch (reportType)
            {
                case 1:
                    templatePath = "Templates/ОтчётСотрудники.xlsx";
                    break;
                case 2:
                    templatePath = "Templates/ОтчётОтдел.xlsx";
                    break;
                case 3:
                    templatePath = "Templates/ОтчётДети.xlsx";
                    break;
                case 4:
                    templatePath = "Templates/ОтчётВоеннообязанные.xlsx";
                    break;
                default:
                    throw new ArgumentException("Неверный тип отчёта");
            }

            return templatePath;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                                                          "Подтверждение о закрытии" +
                                                          "",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning);
            Close();
        }

        private void OrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            MainOrders mainOrders = new MainOrders(_currentUser);
            mainOrders.Show();
            Close();
        }

        private void EmploymentRecordBtn_Click(object sender, RoutedEventArgs e)
        {
            MainEmploymentRecord mainEmploymentRecord = new MainEmploymentRecord(_currentUser);
            mainEmploymentRecord.Show();
            Close();
        }

        private void MainWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowInspector mainWindowInspector = new MainWindowInspector(_currentUser);
            mainWindowInspector.Show();
            Close();
        }
    }
}

