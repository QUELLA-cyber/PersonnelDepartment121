using ClosedXML.Excel;
using Microsoft.Win32;
using PersonnelDepartment.EmploymentRecord;
using PersonnelDepartment.RepostsForm;
using PersonnelDepartment.Staff;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PersonnelDepartment.OrdersForm
{

    public partial class MainOrders : Window
    {
        private HumanResourcesDepartmentEntities _context;
        private Personal_card _currentUser;
        private int _newEmployeeId;

        public MainOrders(Personal_card currentUser)
        {          
            _currentUser = currentUser;
            
            InitializeComponent();           
            LoadData();

            UserName.Content = $"{currentUser.Surname} {currentUser.Name}";
            UserRole.Content = currentUser.Post.Title;

           

            // Проверяем, что ComboBox и данные инициализированы
            if (EmployeeComboBox != null && EmployeeComboBox.ItemsSource != null)
            {
                EmployeeComboBox.SelectedValue = _newEmployeeId;
            }
            else
            {
                Console.WriteLine("Ошибка: EmployeeComboBox или данные не инициализированы.");
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

        private void LoadData()
        {
            _context = new HumanResourcesDepartmentEntities();

            // Загрузка сотрудников из таблицы personal_card
            var employees = _context.Personal_card
                .Select(e => new
                {
                    e.ID,
                    FullName = e.Surname + " " + e.Name + " " + e.Patronymic
                })
                .ToList();
            EmployeeComboBox.ItemsSource = employees;
            EmployeeComboBox.DisplayMemberPath = "FullName";
            EmployeeComboBox.SelectedValuePath = "ID";

            // Выбираем нового сотрудника по его ID
            EmployeeComboBox.SelectedValue = _newEmployeeId;

            // Загрузка типов приказов из таблицы Mixing
            var orderTypes = _context.Mixing
                .Select(o => new { o.ID, o.Title })
                .ToList();
            OrderTypeComboBox.ItemsSource = orderTypes;
            OrderTypeComboBox.DisplayMemberPath = "Title";
            OrderTypeComboBox.SelectedValuePath = "ID";

            // Выбираем по умолчанию первый тип приказа (например, Прием на работу)
            OrderTypeComboBox.SelectedIndex = 0;
        }
        private void StaffBtn_Click(object sender, RoutedEventArgs e)
        {
            MainStaff mainStaffwindow = new MainStaff(_currentUser);
            mainStaffwindow.Show();
            Close();
        }

        private void OrderTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrderTypeComboBox.SelectedValue is int selectedOrderTypeId)
            {               
                // Скрываем все дополнительные опции перед настройкой видимости
                gridOrders.Visibility = Visibility.Visible;
                OptionSalary.Visibility = Visibility.Collapsed;
                OptionPosition.Visibility = Visibility.Collapsed;
                OptionDepartment.Visibility = Visibility.Collapsed;
                OptionBonus.Visibility = Visibility.Collapsed;

                // Устанавливаем видимость в зависимости от выбранного типа приказа
                switch (selectedOrderTypeId)
                {
                    case 1: // Прием на работу
                        MessageBox.Show("Прием на работу - дополнительные поля не требуются");
                        break;

                    case 2: // Увольнение сотрудника
                        MessageBox.Show("Увольнение сотрудника - дополнительные поля не требуются");
                        break;

                    case 3: // Перевод на другую должность
                        MessageBox.Show("Отображение опций для перевода на другую должность");
                        OptionPosition.Visibility = Visibility.Visible;

                        // Загрузка текущей должности и списка новых должностей
                        if (EmployeeComboBox.SelectedValue is int selectedEmployeeId)
                        {
                            var employeeData = _context.Department.FirstOrDefault(emp => emp.ID == selectedEmployeeId);
                            if (employeeData != null)
                            {
                                var currentPosition = _context.Post.FirstOrDefault(pos => pos.ID == selectedEmployeeId);
                                CurrentPositionLabel.Content = $" {currentPosition?.Title ?? "Неизвестно"}";
                            }
                        }

                        NewPositionComboBox.ItemsSource = _context.Post.Select(p => p.Title).ToList();
                        break;

                    case 4: // Перевод в другое подразделение
                        MessageBox.Show("Отображение опций для перевода в другое подразделение");
                        OptionDepartment.Visibility = Visibility.Visible;

                        // Загрузка текущего отдела и списка новых отделов
                        if (EmployeeComboBox.SelectedValue is int selectedEmployeeIdDept)
                        {
                            var employeeData = _context.Personal_card.FirstOrDefault(emp => emp.ID == selectedEmployeeIdDept);
                            var post = _context.Post.FirstOrDefault(p => p.ID == employeeData.Id_post);
                            var currentDepartment = _context.Department.FirstOrDefault(d => d.ID == post.Id_department);

                            CurrentDepartmentLabel.Content = $"Текущий отдел: {currentDepartment?.Title ?? "Неизвестно"}";
                        }

                        // Загрузка списка отделов в NewDepartmentComboBox
                        NewDepartmentComboBox.ItemsSource = _context.Department.Select(d => d.Title).ToList();
                        NewPostCmd.ItemsSource = null; // Очищаем должности, так как отдел еще не выбран
                        break;

                    case 5: // Начисление заработной платы
                        MessageBox.Show("Отображение опций для начисления заработной платы");
                        OptionSalary.Visibility = Visibility.Visible;

                        if (EmployeeComboBox.SelectedValue is int selectedEmployeeIdSalary)
                        {
                            // Получение данных о текущей зарплате
                            var currentPostSalary = _context.Post.FirstOrDefault(p => p.ID == selectedEmployeeIdSalary);
                            if (currentPostSalary != null)
                            {
                                CurrentBaseSarary.Content = $"Текущая зарплата: {currentPostSalary.Base_salary}";
                            }
                            else
                            {
                                CurrentBaseSarary.Content = "Текущая зарплата: Неизвестно";
                            }
                        }
                        break;

                    case 6: // Поощрение
                        MessageBox.Show("Отображение опций для поощрения");
                        OptionBonus.Visibility = Visibility.Visible;
                        BonusTypeComboBox.ItemsSource = _context.Salary_type.Select(s => s.Title).ToList();
                        break;

                    default:
                        MessageBox.Show("Неизвестный тип приказа");
                        break;
                }
            }
        }

        private void SaveToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeComboBox.SelectedValue == null || OrderTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника и тип приказа.");
                return;
            }

            int selectedEmployeeId = (int)EmployeeComboBox.SelectedValue;
            int selectedOrderTypeId = (int)OrderTypeComboBox.SelectedValue;

            // Выбор шаблона на основе типа приказа
            string templatePath = selectedOrderTypeId switch
            {
                1 => "Templates/Приказоприёме.xlsx",
                2 => "Templates/Увольнение.xlsx",
                3 => "Templates/Перевод должность.xlsx",
                4 => "Templates/Перевод отдел.xlsx",
                5 => "Templates/зп.xlsx",
                6 => "Templates/приемии.xlsx",
                _ => throw new InvalidOperationException("Неизвестный тип приказа."),
            };
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Сохранить приказ",
                FileName = $"Приказ_{selectedOrderTypeId}.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    if (!File.Exists(templatePath))
                    {
                        MessageBox.Show("Шаблон Excel не найден.");
                        return;
                    }

                    using var workbook = new XLWorkbook(templatePath);
                    var worksheet = workbook.Worksheet(1);
                    var employeeData = _context.Personal_card.FirstOrDefault(x => x.ID == selectedEmployeeId);

                    if (employeeData == null)
                    {
                        MessageBox.Show("Сотрудник не найден.");
                        return;
                    }

                    var post = _context.Post.FirstOrDefault(p => p.ID == employeeData.Id_post);
                    Department department = null;

                    if (post != null)
                    {
                        department = _context.Department.FirstOrDefault(d => d.ID == post.Id_department);
                    }

                    switch (selectedOrderTypeId)
                    {
                        case 1: // Приказ о приеме на работу
                                // ФИО сотрудника
                            worksheet.Cell("A20").Value = $"{employeeData.Surname} {employeeData.Name} {employeeData.Patronymic}";
                            // Отдел
                            if (department != null)
                            {
                                worksheet.Cell("A22").Value = department.Title;
                            }

                            // Должность
                            if (post != null)
                            {
                                worksheet.Cell("A24").Value = post.Title; // Заполняем должность в строке A24
                            }
                            else
                            {
                                worksheet.Cell("A24").Value = "Должность не указана"; // Если должность не найдена
                            }

                            // Зарплата: берется из таблицы Salary с фильтром по salary_type = 1
                            var salary = _context.Salary
                                .Where(s => s.Id_personal_card == selectedEmployeeId && s.Id_salary_type == 3)
                                .OrderByDescending(s => s.Date)
                                .FirstOrDefault();

                            if (salary != null)
                            {
                                worksheet.Cell("D31").Value = salary.Amount; // Указываем зарплату
                            }
                            else
                            {
                                worksheet.Cell("D31").Value = "Зарплата не указана";
                            }

                            // Дата приема
                            var currentDate = DateTime.Now;
                            worksheet.Cell("D39").Value = $"{currentDate:dd.MM}";
                            worksheet.Cell("G39").Value = currentDate.Year;
                            break;

                        case 2: // Приказ об увольнении
                                // ФИО сотрудника
                            worksheet.Cell("A18").Value = $"{employeeData.Surname} {employeeData.Name} {employeeData.Patronymic}";
                            // Отдел
                            if (department != null)
                            {
                                worksheet.Cell("A20").Value = department.Title;
                            }
                            // Должность
                            if (post != null)
                            {
                                worksheet.Cell("A22").Value = post.Title;
                            }

                            // Дата увольнения
                            var terminationDate = DateTime.Now;
                            worksheet.Cell("C14").Value = $"{terminationDate:dd.MM}";
                            worksheet.Cell("G14").Value = terminationDate.Year;
                            break;

                        case 3: // Перевод на другую должность
                                // ФИО сотрудника
                            worksheet.Cell("A20").Value = $"{employeeData.Surname} {employeeData.Name} {employeeData.Patronymic}";

                            // Текущий отдел и должность
                            if (department != null)
                            {
                                worksheet.Cell("C23").Value = department.Title; // Отдел остается текущим
                            }
                            if (post != null)
                            {
                                worksheet.Cell("C25").Value = post.Title; // Текущая должность
                            }

                            // Получаем текущую должность из лейбла
                            string currentPositionTitle = CurrentPositionLabel.Content?.ToString() ?? post?.Title;
                            if (string.IsNullOrEmpty(currentPositionTitle))
                            {
                                MessageBox.Show("Не удалось получить текущую должность.");
                                return;
                            }

                            // Получаем пост и отдел на основе текущей должности
                            var currentPost = _context.Post.FirstOrDefault(p => p.Title == currentPositionTitle);
                            Department currentDepartment = null;

                            if (currentPost != null)
                            {
                                currentDepartment = _context.Department.FirstOrDefault(d => d.ID == currentPost.Id_department);
                            }

                            if (currentDepartment != null)
                            {
                                // Фильтрация должностей по найденному отделу
                                var departmentPosts = _context.Post.Where(p => p.Id_department == currentDepartment.ID).ToList();

                                // Очищаем комбобокс
                                NewPositionComboBox.Items.Clear();

                                // Добавляем должности, относящиеся к этому отделу
                                foreach (var departmentPost in departmentPosts)
                                {
                                    NewPositionComboBox.Items.Add(departmentPost.Title);
                                }

                                // При необходимости выбираем первую должность по умолчанию
                                if (NewPositionComboBox.Items.Count > 0)
                                {
                                    NewPositionComboBox.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Не удалось найти отдел для текущей должности.");
                            }

                            // Получаем новую должность
                            string newPositionTitle = NewPositionComboBox.SelectedItem?.ToString() ?? post?.Title ?? "Неизвестно";
                            worksheet.Cell("B31").Value = newPositionTitle; // Новая должность

                            // Извлечение новой зарплаты (base_salary) для выбранной должности
                            var newPositionData = _context.Post.FirstOrDefault(p => p.Title == newPositionTitle);
                            if (newPositionData != null)
                            {
                                worksheet.Cell("D34").Value = newPositionData.Base_salary; // Базовая зарплата новой должности
                            }
                            else
                            {
                                worksheet.Cell("D34").Value = "Зарплата не указана"; // Если данные отсутствуют
                            }

                            // Получаем новый отдел
                            var newDepartmentTitle = NewDepartmentComboBox.SelectedItem?.ToString() ?? currentDepartment?.Title ?? "Неизвестен";
                            worksheet.Cell("B29").Value = newDepartmentTitle;

                            // Дата перевода
                            var transferDate = DateTime.Now;
                            worksheet.Cell("D39").Value = $"{transferDate:dd.MM}";
                            worksheet.Cell("F39").Value = transferDate.Year;

                            break;

                        case 4: // Приказ на перевод в другое подразделение
                                // ФИО сотрудника
                            worksheet.Cell("A20").Value = $"{employeeData.Surname} {employeeData.Name} {employeeData.Patronymic}";

                            // Получаем текущий отдел и должность через Post и Department
                            var currentPost1 = _context.Post.FirstOrDefault(p => p.ID == employeeData.Id_post);
                            var currentDepartment1 = currentPost1 != null ? _context.Department.FirstOrDefault(d => d.ID == currentPost1.Id_department) : null;

                            // Заполнение текущего отдела и должности сотрудника
                            worksheet.Cell("C23").Value = currentDepartment1?.Title ?? "Неизвестно"; // Текущий отдел
                            worksheet.Cell("C25").Value = currentPost1?.Title ?? "Неизвестно";      // Текущая должность

                            // Получаем новый отдел и должность из ComboBox
                            string newDepartmentTitleForTransfer = NewDepartmentComboBox.SelectedItem?.ToString() ?? currentDepartment1?.Title ?? "Неизвестно";
                            string newPositionTitleForTransfer = NewPostCmd.SelectedItem?.ToString() ?? currentPost1?.Title ?? "Неизвестно";

                            worksheet.Cell("B29").Value = newDepartmentTitleForTransfer; // Новый отдел
                            worksheet.Cell("B31").Value = newPositionTitleForTransfer;   // Новая должность

                            // Получение базовой зарплаты для новой должности
                            var newDepartment = _context.Department.FirstOrDefault(d => d.Title == newDepartmentTitleForTransfer);
                            if (newDepartment == null)
                            {
                                MessageBox.Show($"Новый отдел '{newDepartmentTitleForTransfer}' не найден.");
                                break;
                            }

                            var newPost = _context.Post.FirstOrDefault(p => p.Title == newPositionTitleForTransfer && p.Id_department == newDepartment.ID);
                            if (newPost != null)
                            {
                                worksheet.Cell("D34").Value = newPost.Base_salary; // Новая зарплата
                            }
                            else
                            {
                                worksheet.Cell("D34").Value = "Зарплата не указана"; // Если данные не найдены
                            }

                            // Дата перевода
                            var transferDateForDepartmentChange = DateTime.Now;
                            worksheet.Cell("D39").Value = $"{transferDateForDepartmentChange:dd.MM}"; // День и месяц
                            worksheet.Cell("F39").Value = transferDateForDepartmentChange.Year;       // Год

                            break;

                        case 5: // Приказ о начислении заработной платы
                            if (EmployeeComboBox.SelectedValue == null)
                            {
                                MessageBox.Show("Выберите сотрудника.");
                                break;
                            }

                            // Получение данных сотрудника по выбранному ID
                            var selectedEmployeeIdSalary = (int)EmployeeComboBox.SelectedValue;
                            var employeeDataSalary = _context.Personal_card.FirstOrDefault(x => x.ID == selectedEmployeeIdSalary);

                            if (employeeDataSalary == null)
                            {
                                MessageBox.Show("Сотрудник не найден.");
                                break;
                            }

                            // Заполнение ФИО сотрудника в ячейку A16
                            worksheet.Cell("A16").Value = $"{employeeDataSalary.Surname} {employeeDataSalary.Name} {employeeDataSalary.Patronymic}";

                            // Отдел сотрудника (A18)
                            var currentPostForSalary = _context.Post.FirstOrDefault(p => p.ID == employeeDataSalary.Id_post);
                            var currentDepartmentForSalary = currentPostForSalary != null
                                ? _context.Department.FirstOrDefault(d => d.ID == currentPostForSalary.Id_department)
                                : null;
                            worksheet.Cell("A18").Value = currentDepartmentForSalary?.Title ?? "Отдел не указан";

                            // Должность сотрудника (A20)
                            if (currentPostForSalary != null)
                            {
                                worksheet.Cell("A20").Value = currentPostForSalary.Title; // Запись должности
                            }
                            else
                            {
                                worksheet.Cell("A20").Value = "Должность не указана";
                            }

                            // Базовая зарплата (D27) из таблицы Post
                            if (currentPostForSalary != null)
                            {
                                worksheet.Cell("D27").Value = currentPostForSalary.Base_salary; // Запись зарплаты
                            }
                            else
                            {
                                worksheet.Cell("D27").Value = 0.0f; // Если должность не найдена
                            }

                            // Новая зарплата (D29) из текстбокса
                            if (double.TryParse(NewSalaryTextBox.Text, out double newSalaryAmount))
                            {
                                worksheet.Cell("D29").Value = newSalaryAmount;
                            }

                            else
                            {
                                MessageBox.Show("Пожалуйста, введите корректную новую зарплату.");
                                worksheet.Cell("D29").Value = 0.0f; // Если введенные данные некорректны
                            }

                            // Дата составления приказа (D35) - полный формат (день, месяц, год)
                            var salaryChangeDate = DateTime.Now;
                            worksheet.Cell("D35").Value = $"{salaryChangeDate:dd.MM.yyyy}";

                            break;

                        case 6: // Поощрение
                            if (EmployeeComboBox.SelectedValue == null)
                            {
                                MessageBox.Show("Выберите сотрудника.");
                                break;
                            }

                            // Получение данных сотрудника по выбранному ID
                            var selectedEmployeeIdBonus = (int)EmployeeComboBox.SelectedValue;
                            var employeeDataBonus = _context.Personal_card.FirstOrDefault(x => x.ID == selectedEmployeeIdBonus);

                            if (employeeDataBonus == null)
                            {
                                MessageBox.Show("Сотрудник не найден.");
                                break;
                            }

                            // Заполнение ФИО сотрудника в ячейку A20
                            worksheet.Cell("A20").Value = $"{employeeDataBonus.Surname} {employeeDataBonus.Name} {employeeDataBonus.Patronymic}";

                            // Отдел сотрудника (A22)
                            var currentPostForBonus = _context.Post.FirstOrDefault(p => p.ID == employeeDataBonus.Id_post);
                            var currentDepartmentForBonus = currentPostForBonus != null
                                ? _context.Department.FirstOrDefault(d => d.ID == currentPostForBonus.Id_department)
                                : null;
                            worksheet.Cell("A22").Value = currentDepartmentForBonus?.Title ?? "Отдел не указан";

                            // Должность сотрудника (A24)
                            worksheet.Cell("A24").Value = currentPostForBonus?.Title ?? "Должность не указана";

                            // Заполнение ComboBox только типами "Премия" (1) и "Командировочные" (2)
                            BonusTypeComboBox.ItemsSource = _context.Salary_type
                                .Where(t => t.ID == 1 || t.ID == 2)
                                .Select(t => t.Title)
                                .ToList();

                            // Проверка выбранного типа премии
                            if (BonusTypeComboBox.SelectedItem != null)
                            {
                                // Тип премии (D31)
                                worksheet.Cell("D31").Value = BonusTypeComboBox.SelectedItem.ToString();
                            }
                            else
                            {
                                MessageBox.Show("Выберите тип премии.");
                                worksheet.Cell("D31").Value = "Тип не выбран";
                            }

                            // Сумма премии из текстбокса (D33)
                            if (double.TryParse(BonusAmountTextBox.Text, out double bonusAmount))
                            {
                                worksheet.Cell("D33").Value = bonusAmount;
                            }
                            else
                            {
                                MessageBox.Show("Введите корректную сумму премии.");
                                worksheet.Cell("D33").Value = 0.0f; // Если введенные данные некорректны
                            }

                            // Дата премии (D39) - текущая дата (день, месяц, год)
                            var bonusDate = DateTime.Now;
                            worksheet.Cell("D39").Value = $"{bonusDate:dd.MM.yyyy}";

                            break;
                    }

                    // Сохранение файла
                    workbook.SaveAs(saveFileDialog.FileName);
                    MessageBox.Show("Приказ успешно сохранен.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
                }
            }
        }

        private void MainWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowInspector mainWindowInspector = new MainWindowInspector(_currentUser);
            mainWindowInspector.Show();
            Close();
        }

        private void EmploymentrecordBtn_Click(object sender, RoutedEventArgs e)
        {
            MainEmploymentRecord mainEmploymentRecord = new MainEmploymentRecord(_currentUser);
            mainEmploymentRecord.Show();
            Close();
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            MainReports mainReports = new MainReports(_currentUser);
            mainReports.Show();
            Close();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                                                           "Подтверждение о закрытии",
                                                           MessageBoxButton.YesNo,
                                                           MessageBoxImage.Warning);
            Close();

        }

        private void NewDepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NewDepartmentComboBox.SelectedItem is string selectedDepartmentTitle)
            {
                // Получаем выбранный отдел
                var selectedDepartment = _context.Department.FirstOrDefault(d => d.Title == selectedDepartmentTitle);

                if (selectedDepartment != null)
                {
                    // Фильтруем должности по выбранному отделу и заполняем NewPostCmd
                    NewPostCmd.ItemsSource = _context.Post
                        .Where(p => p.Id_department == selectedDepartment.ID)
                        .Select(p => p.Title)
                        .ToList();
                }
                else
                {
                    // Если отдел не выбран или не найден, очищаем список должностей
                    NewPostCmd.ItemsSource = null;
                }
            }
        }

        private void EmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
