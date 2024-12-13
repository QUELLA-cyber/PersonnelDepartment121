using System.Text.RegularExpressions;
using System;
using System.Windows.Controls;

namespace PersonnelDepartment.ClassHelper
{
    public class Validator
    {
        // Регулярные выражения для проверки полей
        private static readonly string NamePattern = "^[А-ЯЁ][а-яё]+$";  // Имя, Фамилия, Отчество
        //private static readonly string BirthDatePattern = "^(0[1-9]|[12][0-9]|3[01])\\.(0[1-9]|1[0-2])\\.(19|20)\\d{2}$";  // Дата рождения, Дата выдачи
        private static readonly string PlacePattern = "^[А-ЯЁа-яё\\s.,]+$";  //Кем выдан
        private static readonly string PlaceBirthPattern = "^[гпсд]\\. ?[А-ЯЁ][а-яё\\s.,]+$|^[А-ЯЁ][а-яё\\s.,]+$"; //Место рождения
        private static readonly string PlaceAddressPattern = "^(?=.*[а-яА-ЯЁё])[А-ЯЁа-яё0-9\\s.,]+$"; //Адрес регистрации
        private static readonly string PhonePattern = "^\\+?[0-9]{10,15}$";  // Телефон
        private static readonly string EmailPattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";  // Электронная почта
        private static readonly string NonEmptyPattern = "^(?!\\s*$).+";  // Отдел, Должность, Уровень образования, Гражданство
        private static readonly string PassportSeriesPattern = "^\\d{4}$";  // Серия паспорта
        private static readonly string PassportNumberPattern = "^\\d{6}$";  // Номер паспорта
        private static readonly string EducationInstitutionPattern = "^[А-ЯЁ][А-ЯЁа-яё0-9 №]+(?: [А-ЯЁа-яё0-9 №]+)*$"; // Учебное заведение
        private static readonly string datePattern = "^(0[1-9]|[12][0-9]|3[01])\\.(0[1-9]|1[0-2])\\.(19|20)\\d{2}$";

        // Валидация имени (Фамилия, Имя, Отчество)
        public static bool IsValidName(string name, out string errorMessage)
        {
            return IsValid(name, NamePattern, out errorMessage, "Некорректный ввод в поле «Имя»");
        }

        public static bool IsValidSurName(string name, out string errorMessage)
        {
            return IsValid(name, NamePattern, out errorMessage, "Некорректный ввод в поле «Фамилия»");
        }

        public static bool IsValidPatronymic(string name, out string errorMessage)
        {
            return IsValid(name, NamePattern, out errorMessage, "Некорректный ввод в поле «Отчество»");
        }

        public static bool IsValidBirthDate(string birthDateString, out string errorMessage)
        {
            // Проверка строки на соответствие формату даты
            if (string.IsNullOrWhiteSpace(birthDateString) || !Regex.IsMatch(birthDateString, datePattern))
            {
                errorMessage = "Некорректный ввод в поле «Дата рождения»";
                return false;
            }

            // Попытка преобразовать строку в DateTime
            DateTime birthDate;
            if (DateTime.TryParseExact(birthDateString, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out birthDate))
            {
                // Проверка на дату больше текущей
                if (birthDate > DateTime.Now.Date)
                {
                    errorMessage = "Некорректный ввод в поле «Дата рождения»";
                    return false;
                }
                errorMessage = null;
                return true;
            }

            errorMessage = "Некорректный ввод в поле «Дата рождения»";
            return false;
        }

        // Валидация места рождения
        public static bool IsValidPlace(string place, out string errorMessage)
        {
            return IsValid(place, PlaceBirthPattern, out errorMessage, "Некорректный ввод в поле «Место рождения»");
        }

        // Валидация адреса регистрации
        public static bool IsValidAddress(string address, out string errorMessage)
        {
            return IsValid(address, PlaceAddressPattern, out errorMessage, "Некорректный ввод в поле «Адрес регистрации»");
        }

        // Валидация телефона
        public static bool IsValidPhone(string phone, out string errorMessage)
        {
            return IsValid(phone, PhonePattern, out errorMessage, "Некорректный ввод в поле «Телефонный номер»");
        }

        // Валидация электронной почты
        public static bool IsValidEmail(string email, out string errorMessage)
        {
            return IsValid(email, EmailPattern, out errorMessage, "Некорректный ввод в поле «Электронная почта»");
        }

        // Валидация отдела
        public static bool IsValidDepartment(string department, out string errorMessage)
        {
            return IsValid(department, NonEmptyPattern, out errorMessage, "Поле со списком «Отдел» пустое");
        }

        // Валидация должности
        public static bool IsValidPost(string post, out string errorMessage)
        {
            return IsValid(post, NonEmptyPattern, out errorMessage, "Поле со списком «Должность» пустое");
        }

        // Валидация серии паспорта
        public static bool IsValidPassportSeries(string series, out string errorMessage)
        {
            return IsValid(series, PassportSeriesPattern, out errorMessage, "Некорректный ввод в поле «Серия паспорта»");
        }

        // Валидация номера паспорта
        public static bool IsValidPassportNumber(string number, out string errorMessage)
        {
            return IsValid(number, PassportNumberPattern, out errorMessage, "Некорректный ввод в поле «Номер паспорта»");
        }

        // Валидация даты выдачи паспорта
        public static bool IsValidDateOfIssue(string dateOfIssueString, out string errorMessage)
        {
            // Проверка строки на соответствие формату даты
            if (string.IsNullOrWhiteSpace(dateOfIssueString) || !Regex.IsMatch(dateOfIssueString, datePattern))
            {
                errorMessage = "Некорректный ввод в поле «Дата выдачи»";
                return false;
            }

            // Попытка преобразовать строку в DateTime
            DateTime dateOfIssue;
            if (DateTime.TryParseExact(dateOfIssueString, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dateOfIssue))
            {
                // Проверка на дату больше текущей
                if (dateOfIssue > DateTime.Now.Date)
                {
                    errorMessage = "Некорректный ввод в поле «Дата выдачи»";
                    return false;
                }
                errorMessage = null;
                return true;
            }

            errorMessage = "Некорректный ввод в поле «Дата выдачи»";
            return false;
        }

        // Валидация уровня образования
        public static bool IsValidEducationLevel(string educationLevel, out string errorMessage)
        {
            return IsValid(educationLevel, NonEmptyPattern, out errorMessage, "Поле со списком «Уровень» пустое");
        }

        // Валидация поля «Кем выдан»
        public static bool IsValidIssuedBy(string issuedBy, out string errorMessage)
        {
            return IsValid(issuedBy, PlacePattern, out errorMessage, "Некорректный ввод в поле «Кем выдан»");
        }

        //Валидация поля «Учебное заведение»
        public static bool IsValidEducationInstitution(string institution, out string errorMessage)
        {
            return IsValid(institution, EducationInstitutionPattern, out errorMessage, "Некорректный ввод в поле «Учебное заведение»");
        }

        // Валидация гражданства
        public static bool IsValidCitizenship(string citizenship, out string errorMessage)
        {
            return IsValid(citizenship, NonEmptyPattern, out errorMessage, "Поле со списком «Гражданство» пустое");
        }

        // Валидация поля с фото
        public static bool IsValidPhoto(string photoPath, out string errorMessage)
        {
            // Проверяем, что поле не пустое и путь к файлу корректный
            if (string.IsNullOrWhiteSpace(photoPath))
            {
                errorMessage = "Выберите значение в поле «Аватар»";
                return false;
            }

            // Проверяем, что файл существует
            if (!System.IO.File.Exists(photoPath))
            {
                errorMessage = "Выберите значение в поле «Аватар»";
                return false;
            }

            errorMessage = null;
            return true;
        }

        // Валидация поля с обязательным заполнением
        public static bool IsValidField(string field, out string errorMessage, string errorText)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                errorMessage = errorText;
                return false;
            }
            errorMessage = null;
            return true;
        }

        // Основной метод для проверки строк с регулярными выражениями
        private static bool IsValid(string value, string pattern, out string errorMessage, string errorText)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errorMessage = errorText;
                return false;
            }

            if (!Regex.IsMatch(value, pattern))
            {
                errorMessage = errorText;
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
