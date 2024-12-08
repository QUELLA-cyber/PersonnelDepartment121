using PersonnelDepartment.ClassHelper;
using PersonnelDepartment.SraffAdmin;
using System;
using System.Collections.Generic;
using System.Data; // Для работы с базой данных
using System.Data.SqlClient; // Подключение к SQL Server
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PersonnelDepartment.AdminDataSource
{
    /// <summary>
    /// Логика взаимодействия для DataSourceForm.xaml
    /// </summary>
    public partial class DataSourceForm : Window
    {
        
        private Employee currentUser;
        private Personal_card _currentUser; // Хранение данных текущего пользователя

        // Список таблиц с переводами
        private Dictionary<string, string> tableNames = new Dictionary<string, string>()
        {
            { "Citizenship", "Гражданство" },
            { "Department", "Отдел" },
            { "Post", "Должность" },
            { "Education", "Образование" },
            { "Mixing", "Причины" },
            { "Salary_type", "Тип зарплаты" }
        };

       private readonly Dictionary<string, string> columnNameMap = new Dictionary<string, string>
        {
           //Гражданство
            { "ID", "Номер" },
            { "Country", "Страна" },
            //Отдел
            { "Title", "Наименование" },
            { "Responsibilities", "Описание" },
            { "Base_salary", "Базовая зарплата" },
            { "Id_department", "Номер отдела" },
            { "Levell", "Уровень" },
        };

        private string connectionString = "data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework";

        public DataSourceForm(Personal_card currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DisplayUserInfo(); // Отображение информации о текущем пользователе
            

            // Инициализация комбобокса
            foreach (var table in tableNames.Values)
            {
                ComboBox.Items.Add(table);
            }

            ComboBox.SelectionChanged += ComboBox_SelectionChanged;;
        }

        private void DisplayUserInfo()
        {
            if (_currentUser != null)
            {
                Namelbl.Content = $"{_currentUser.Surname} {_currentUser.Name}";
                Rolelbl.Content = _currentUser.Post.Title;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox.SelectedItem != null)
            {
                string selectedTableRus = ComboBox.SelectedItem.ToString();
                string selectedTableEng = tableNames.FirstOrDefault(t => t.Value == selectedTableRus).Key;

                LoadTableData(selectedTableEng);
            }
        }

        // Загрузка данных в ListView
        private void LoadTableData(string tableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM {tableName}";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Очистка предыдущих данных
                    ListView.Items.Clear();
                    GridView.Columns.Clear();

                    // Динамическое создание столбцов из DataTable
                    foreach (DataColumn column in dataTable.Columns)
                    { 
                        // Формируем ключ для перевода столбца в формате "TableName_ColumnName"
                        string columnKey = column.ColumnName;

                        // Получаем переведённое название столбца из словаря, если оно существует
                        string columnHeader = columnNameMap.ContainsKey(columnKey) ? columnNameMap[columnKey] : column.ColumnName;

                        // Добавляем столбец в GridView
                        GridView.Columns.Add(new GridViewColumn
                        {
                            Header = columnHeader, // Переведённый или оригинальный заголовок
                            DisplayMemberBinding = new System.Windows.Data.Binding($"[{column.ColumnName}]") // Привязка к данным
                        });
                    }

                    // Добавляем колонку для кнопки "Удалить"
                    GridView.Columns.Add(new GridViewColumn
                    {
                        Header = "Действия",
                        CellTemplate = CreateDeleteButtonTemplate()
                    });

                    // Добавляем строки в ListView
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var item = new Dictionary<string, object>();
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            item[column.ColumnName] = row[column];
                        }
                        item["ID"] = row["ID"]; // Сохраняем ID для удаления
                        ListView.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        // Шаблон кнопки "Удалить"
        private DataTemplate CreateDeleteButtonTemplate()
        {
            var template = new DataTemplate();

            // Стек для размещения кнопки
            var stackPanel = new FrameworkElementFactory(typeof(StackPanel));
            stackPanel.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            // Создание кнопки "Удалить"
            var button = new FrameworkElementFactory(typeof(Button));
            button.SetValue(Button.ContentProperty, "Удалить");
            button.SetValue(Button.WidthProperty, 75.0);
            button.SetValue(Button.HeightProperty, 25.0);
            button.SetValue(Button.MarginProperty, new Thickness(5));

            // Назначаем стиль кнопке
            button.SetValue(Button.StyleProperty, Application.Current.Resources["ExitButton"]);

            // Привязываем обработчик события
            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteRow_Click));

            // Добавляем кнопку в стек
            stackPanel.AppendChild(button);

            // Устанавливаем стек как шаблон
            template.VisualTree = stackPanel;

            return template;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ID строки из контекста кнопки
            if (sender is Button button)
            {
                var row = button.DataContext as Dictionary<string, object>;
                if (row != null && row.ContainsKey("ID"))
                {
                    int id = Convert.ToInt32(row["ID"]);
                    string tableName = tableNames.FirstOrDefault(t => t.Value == ComboBox.SelectedItem.ToString()).Key;

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = $"DELETE FROM {tableName} WHERE ID = @ID";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@ID", id);
                            command.ExecuteNonQuery();

                            MessageBox.Show("Запись успешно удалена.");
                            LoadTableData(tableName); // Обновляем данные
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewRow(string tableName, string[] inputData)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Генерация SQL-запроса для вставки данных
                    string query = GenerateInsertQuery(tableName, inputData.Length);

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Подстановка значений параметров
                        for (int i = 0; i < inputData.Length; i++)
                        {
                            command.Parameters.AddWithValue($"@param{i + 1}", inputData[i].Trim());
                        }

                        command.ExecuteNonQuery();
                        MessageBox.Show("Данные успешно добавлены!");

                        // Обновление отображаемых данных
                        LoadTableData(tableName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении данных: {ex.Message}");
            }
        }

        private string GenerateInsertQuery(string tableName, int paramCount)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Получение имен столбцов из таблицы
                string columnQuery = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
                SqlDataAdapter adapter = new SqlDataAdapter(columnQuery, connection);
                DataTable columns = new DataTable();
                adapter.Fill(columns);

                // Исключаем столбец ID, если он автоинкрементный
                var columnNames = columns.AsEnumerable()
                                         .Select(row => row.Field<string>("COLUMN_NAME"))
                                         .Skip(1) // Пропускаем первый столбец (ID)
                                         .Take(paramCount)
                                         .ToArray();

                if (columnNames.Length != paramCount)
                {
                    throw new Exception("Количество введенных данных не совпадает с количеством столбцов.");
                }

                string columnList = string.Join(", ", columnNames);
                string valueList = string.Join(", ", Enumerable.Range(1, paramCount).Select(i => $"@param{i}"));

                return $"INSERT INTO {tableName} ({columnList}) VALUES ({valueList})";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, выбрана ли таблица
            if (ComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите таблицу для добавления данных.");
                return;
            }

            // Проверяем, введены ли данные
            if (string.IsNullOrWhiteSpace(NewDataInput.Text))
            {
                MessageBox.Show("Введите данные для добавления.");
                return;
            }

            // Получаем выбранное имя таблицы
            string selectedTableRus = ComboBox.SelectedItem.ToString();
            string selectedTableEng = tableNames.FirstOrDefault(t => t.Value == selectedTableRus).Key;

            // Парсим данные из TextBox
            string[] inputData = NewDataInput.Text.Split(',');

            // Вызываем метод добавления
            AddNewRow(selectedTableEng, inputData);

            // Очищаем поле ввода
            NewDataInput.Clear();
        }

        private void StaffAdminBtn_Click(object sender, RoutedEventArgs e)
        {
            StaffAdminForm staffAdminForm = new StaffAdminForm(_currentUser);
            staffAdminForm.Show();
            Close();
        }

        private void MainWindowBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowAdmin mainWindowAdmin = new MainWindowAdmin(_currentUser);
            mainWindowAdmin.Show();
            Close();
        }
    }
}
