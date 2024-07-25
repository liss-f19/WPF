using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Employees_management
{
    public enum Currency { PLN, USD, EUR, GBP, NOK }
    public enum Role { Worker, SeniorWorker, IT, Manager, Director, CEO }

    public class Employee : INotifyPropertyChanged
    {
        private string firstName;
        private string lastName;
        private string birthCountry;
        private string sex;
        private DateTime birthDate;
        private int salary=5000;
        private Currency salaryCurrency;
        private Role companyRole;

        public string FirstName
        {
            get => firstName;
            set { firstName = value; OnPropertyChanged(nameof(FirstName)); }
        }
        public string LastName
        {
            get => lastName;
            set { lastName = value; OnPropertyChanged(nameof(LastName)); }
        }
        public string BirthCountry
        {
            get => birthCountry;
            set { birthCountry = value; OnPropertyChanged(nameof(BirthCountry)); }
        }
        public DateTime BirthDate
        {
            get => birthDate;
            set { birthDate = value; OnPropertyChanged(nameof(BirthDate)); }
        }
        public int Salary
        {
            get => salary;
            set
            {
                if (salary != value)
                {
                    salary = value;
                    OnPropertyChanged(nameof(Salary));
                }
            }
        }
        public Currency SalaryCurrency
        {
            get => salaryCurrency;
            set { salaryCurrency = value; OnPropertyChanged(nameof(SalaryCurrency)); }
        }
        public Role CompanyRole
        {
            get => companyRole;
            set { companyRole = value; OnPropertyChanged(nameof(CompanyRole)); }
        }
        public string Sex
        {
            get => sex;
            set { sex = value; OnPropertyChanged(nameof(Sex)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SalaryValidationRule : ValidationRule
    {
        public double MinSalary { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Salary is required.");
            }

            if (double.TryParse(value.ToString(), out double salary))
            {
                if (salary < MinSalary)
                {
                    return new ValidationResult(false, $"Must be >= {MinSalary}.");
                }
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Invalid salary.");
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if (value is bool hasError && hasError)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }


}
