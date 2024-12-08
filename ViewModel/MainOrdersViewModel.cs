//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using ClosedXML.Excel;
//using Microsoft.Win32;
//using PersonnelDepartment.ViewModel;
//using PersonnelDepartment.ClassHelper;
//using PersonnelDepartment.ControlClasses;

//namespace PersonnelDepartment.ViewModels
//{
//    public class MainOrdersViewModel : BaseViewModel
//    {
//        private readonly HumanResourcesDepartmentEntities _context;

//        public MainOrdersViewModel()
//        {
//            _context = new HumanResourcesDepartmentEntities();
//            LoadEmployeeData();
//            LoadOrderTypeData();
//        }

//        #region Properties

//        public List<Personal_card> Employees { get; set; }

//        public int SelectedEmployeeId { get; set; }
//        public int SelectedOrderTypeId { get; set; }

//        public string NewPositionTitle { get; set; }
//        public string NewDepartmentTitle { get; set; }
//        public string NewPostTitle { get; set; }

//        public double NewSalary { get; set; }
//        public double BonusAmount { get; set; }
//        public string BonusType { get; set; }

//        #endregion

//        #region Methods

//        private void LoadEmployeeData()
//        {
//            // Загружаем сотрудников из таблицы Personal_card
//            var employees = _context.Personal_card
//                .Select(e => new
//                {
//                    e.ID,
//                    FullName = e.Surname + " " + e.Name + " " + e.Patronymic // Формируем полное имя
//                })
//                .ToList();

//            // Привязываем список сотрудников к ComboBox
//            EmployeeComboBox.ItemsSource = employees;
//            EmployeeComboBox.DisplayMemberPath = "FullName"; // Что будет отображаться в ComboBox
//            EmployeeComboBox.SelectedValuePath = "ID"; // ID будет сохраняться при выборе

//            // Выбираем первого сотрудника по умолчанию
//            EmployeeComboBox.SelectedIndex = 0;
//        }

//        private void LoadOrderTypeData()
//        {
//            // Получаем типы приказов из таблицы Mixing
//            var orderTypes = _context.Mixing
//                .Select(o => new
//                {
//                    o.ID,
//                    o.Title // Название типа приказа
//                })
//                .ToList();

//            // Привязываем полученные данные к ComboBox
//            OrderTypeComboBox.ItemsSource = orderTypes;
//            OrderTypeComboBox.DisplayMemberPath = "Title"; // Что будет отображаться в ComboBox
//            OrderTypeComboBox.SelectedValuePath = "ID"; // ID будет сохраняться при выборе

//            // Выбираем первый тип приказа по умолчанию
//            OrderTypeComboBox.SelectedIndex = 0;
//        }

//        // Метод для обработки выбора сотрудника
//        public void OnEmployeeSelectionChanged(int selectedEmployeeId)
//        {
//            SelectedEmployeeId = selectedEmployeeId;
//            // Обновление данных, если необходимо
//        }

//        // Метод для обработки выбора типа приказа
//        public void OnOrderTypeSelectionChanged(int selectedOrderTypeId)
//        {
//            SelectedOrderTypeId = selectedOrderTypeId;
//            UpdateOrderFormVisibility();
//        }

//        // Обновление формы в зависимости от типа приказа
//        private void UpdateOrderFormVisibility()
//        {
//            // Логика отображения разных частей формы в зависимости от выбранного типа приказа
//            switch (SelectedOrderTypeId)
//            {
//                case 1:
//                    ShowSalaryOption();
//                    break;
//                case 2:
//                    ShowTerminationOption();
//                    break;
//                case 3:
//                    ShowPositionTransferOption();
//                    break;
//                case 4:
//                    ShowDepartmentTransferOption();
//                    break;
//                case 5:
//                    ShowBonusOption();
//                    break;
//                case 6:
//                    ShowSalaryChangeOption();
//                    break;
//                default:
//                    HideAllOptions();
//                    break;
//            }
//        }

//        private void ShowSalaryOption()
//        {
//            // Логика для отображения блока для зарплаты
//        }

//        private void ShowTerminationOption()
//        {
//            // Логика для отображения блока для увольнения
//        }

//        private void ShowPositionTransferOption()
//        {
//            // Логика для отображения блока для перевода на должность
//        }

//        private void ShowDepartmentTransferOption()
//        {
//            // Логика для отображения блока для перевода в отдел
//        }

//        private void ShowBonusOption()
//        {
//            // Логика для отображения блока для премии
//        }

//        private void ShowSalaryChangeOption()
//        {
//            // Логика для отображения блока для изменения зарплаты
//        }

//        private void HideAllOptions()
//        {
//            // Скрыть все блоки
//        }

//        // Метод для сохранения приказа в Excel
//        public void SaveToExcel()
//        {
//            if (SelectedEmployeeId == 0 || SelectedOrderTypeId == 0)
//            {
//                MessageBox.Show("Пожалуйста, выберите сотрудника и тип приказа.");
//                return;
//            }

//            var selectedEmployee = _context.Personal_card.FirstOrDefault(e => e.ID == SelectedEmployeeId);
//            var selectedOrderType = _context.OrderType.FirstOrDefault(o => o.ID == SelectedOrderTypeId);
//            if (selectedEmployee == null || selectedOrderType == null)
//            {
//                MessageBox.Show("Ошибка при выборе данных.");
//                return;
//            }

//            string templatePath = GetTemplatePath();
//            var saveFileDialog = new SaveFileDialog
//            {
//                Filter = "Excel Files|*.xlsx",
//                Title = "Сохранить приказ",
//                FileName = $"Приказ_{selectedOrderType.Title}.xlsx"
//            };

//            if (saveFileDialog.ShowDialog() == true)
//            {
//                try
//                {
//                    if (!System.IO.File.Exists(templatePath))
//                    {
//                        MessageBox.Show("Шаблон Excel не найден.");
//                        return;
//                    }

//                    using var workbook = new XLWorkbook(templatePath);
//                    var worksheet = workbook.Worksheet(1);

//                    FillOrderDetailsInExcel(worksheet, selectedEmployee);

//                    workbook.SaveAs(saveFileDialog.FileName);
//                    MessageBox.Show("Приказ успешно сохранен.");
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
//                }
//            }
//        }

//        private string GetTemplatePath()
//        {
//            // Возвращаем путь к шаблону в зависимости от типа приказа
//            return SelectedOrderTypeId switch
//            {
//                1 => "Templates/Приказоприёме.xlsx",
//                2 => "Templates/Увольнение.xlsx",
//                3 => "Templates/Перевод должность.xlsx",
//                4 => "Templates/Перевод отдел.xlsx",
//                5 => "Templates/зп.xlsx",
//                6 => "Templates/поощрение.xlsx",
//                _ => throw new InvalidOperationException("Неизвестный тип приказа.")
//            };
//        }

//        private void FillOrderDetailsInExcel(IXLWorksheet worksheet, Personal_card employee)
//        {
//            // Заполнение Excel в зависимости от типа приказа
//            switch (SelectedOrderTypeId)
//            {
//                case 1: // Приказ о приеме на работу
//                    worksheet.Cell("A20").Value = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
//                    break;
//                case 2: // Приказ об увольнении
//                    worksheet.Cell("A18").Value = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
//                    break;
//                case 3: // Перевод на другую должность
//                    worksheet.Cell("A20").Value = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
//                    worksheet.Cell("B31").Value = NewPositionTitle;
//                    break;
//                case 4: // Перевод в другой отдел
//                    worksheet.Cell("A20").Value = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
//                    worksheet.Cell("B29").Value = NewDepartmentTitle;
//                    worksheet.Cell("B31").Value = NewPostTitle;
//                    break;
//                case 5: // Премия
//                    worksheet.Cell("A20").Value = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
//                    worksheet.Cell("D33").Value = BonusAmount;
//                    worksheet.Cell("D31").Value = BonusType;
//                    break;
//                case 6: // Изменение зарплаты
//                    worksheet.Cell("A16").Value = $"{employee.Surname} {employee.Name} {employee.Patronymic}";
//                    worksheet.Cell("D29").Value = NewSalary;
//                    break;
//                default:
//                    throw new InvalidOperationException("Неизвестный тип приказа.");
//            }
//        }

//        #endregion
//    }
//}
