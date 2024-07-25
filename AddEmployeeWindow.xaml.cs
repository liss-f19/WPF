using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;


namespace Employees_management
{
    public partial class AddEmployeeWindow : Window
    {
        public event EventHandler<Employee> EmployeeAdded;

        public AddEmployeeWindow()
        {
            InitializeComponent();
            MaleRadioButton.IsChecked = true; 
            BirthDatePicker.SelectedDate = DateTime.Now.AddYears(-30); 
            this.DataContext = new Employee();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (Validation.GetHasError(SalaryTextBox))
            {
                MessageBox.Show("Salary must be at least 5000 and a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(SalaryTextBox.Text, out int salary) || salary < 5000)
            {
                MessageBox.Show("Salary must be a valid number and at least 5000.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newEmployee = new Employee
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Sex = MaleRadioButton.IsChecked == true ? "Male" : "Female",
                BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now,
                BirthCountry = BirthCountryTextBox.Text,
                Salary = salary,
                SalaryCurrency = (Currency)SalaryCurrencyComboBox.SelectedItem,
                CompanyRole = (Role)CompanyRoleComboBox.SelectedItem
            };

            EmployeeAdded?.Invoke(this, newEmployee);

            MessageBox.Show("New employee added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void SalaryTextBox_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                ((Control)sender).ToolTip = e.Error.ErrorContent.ToString();
            }
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                ((Control)sender).ToolTip = null;
            }
        }
    }
}

