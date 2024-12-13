using System;

namespace PersonnelDepartment.ClassHelper
{
    public class PersonalCard
    {
        public int Id { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string SeriesAndNumber { get; set; }
        public string IssuedByWhom { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string RegistrationAddress { get; set; }
        public string Telephone { get; set; }
        public bool Children { get; set; }
        public bool MilitaryService { get; set; }
        public int IdCitizenship { get; set; }
        public int IdPost { get; set; }
        public int IdDepartment { get; set; }
        public int IdEducation { get; set; }
        public string Birthplace { get; set; }
        public string Email { get; set; }
        public string EducationInstitution { get; set; }
        public byte[] Photo { get; set; }
        public string GetSeries
        {
            get => !string.IsNullOrEmpty(SeriesAndNumber) && SeriesAndNumber.Length >= 4
                    ? SeriesAndNumber.Substring(0, 4)
                    : string.Empty;
            set =>
                // Устанавливаем новую серию, объединяя её с номером, если он уже есть
                SeriesAndNumber = value + (SeriesAndNumber?.Substring(4) ?? "");
        }
        public string GetNumber
        {
            get => !string.IsNullOrEmpty(SeriesAndNumber) && SeriesAndNumber.Length > 4
                    ? SeriesAndNumber.Substring(4)
                    : string.Empty;
            set =>
                // Устанавливаем новый номер, объединяя его с серией, если она уже есть
                SeriesAndNumber = (SeriesAndNumber?.Substring(0, 4) ?? "") + value;
        }
        public void SetSeriesAndNumber(string series, string number)
        {
            SeriesAndNumber = series + number;
        }
    }
}
