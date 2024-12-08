using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace PersonnelDepartment.EmploymentRecord
{
    /// <summary>
    /// Логика взаимодействия для AddRecordWindow.xaml
    /// </summary>
    public partial class AddRecordWindow : Window
    {
        public event EventHandler RecordAdded;
        private int _employeeId;

        public AddRecordWindow(int employeeId)
        {
            InitializeComponent();
            _employeeId = employeeId;
            LoadMixingData();
        }

        public AddRecordWindow()
        {
            InitializeComponent();
            LoadMixingData();
        }

        private void LoadMixingData()
        {
            List<string> mixingTitles = new List<string>();

            using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                connection.Open();
                var command = new SqlCommand("SELECT TOP 4 Title FROM Mixing", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mixingTitles.Add(reader.GetString(0));
                    }
                }
            }

            MixingComboBox.ItemsSource = mixingTitles;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedMixing = MixingComboBox.SelectedItem as string;
            DateTime? selectedDate = DatePicker1.SelectedDate;
            string reason = ReasonTextBox.Text;

            if (string.IsNullOrEmpty(selectedMixing) || selectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            using (var connection = new SqlConnection("data source=KSESHA;initial catalog=HumanResourcesDepartment;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework"))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Entry_in_the_work_book (Id_personal_card, Date, Reason, Id_mixing) VALUES (@Id_personal_card, @Date, @Reason, (SELECT ID FROM Mixing WHERE Title = @MixingTitle))", connection);

                command.Parameters.AddWithValue("@Id_personal_card", _employeeId);
                command.Parameters.AddWithValue("@Date", selectedDate);
                command.Parameters.AddWithValue("@Reason", reason);
                command.Parameters.AddWithValue("@MixingTitle", selectedMixing);

                command.ExecuteNonQuery();
            }

            MessageBox.Show("Запись добавлена успешно!");

            // Вызываем событие RecordAdded, чтобы основное окно узнало об успешном добавлении
            RecordAdded?.Invoke(this, EventArgs.Empty);

            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
