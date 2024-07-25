using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Employees_management
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();
        private string _currentFilePath;
        private bool _isModified;
        private AddEmployeeWindow _addEmployeeWindow;

        public MainWindow()
        {
            InitializeComponent();
            listViewEmployees.ItemsSource = Employees;
            Employees.CollectionChanged += Employees_CollectionChanged;
        }

        private void Employees_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _isModified = true;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForUnsavedChanges())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _currentFilePath = openFileDialog.FileName;
                    LoadEmployees(_currentFilePath);
                    _isModified = false;
                }
            }
        }

        private void LoadEmployees(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1);
            Employees.Clear();
            foreach (var line in lines)
            {
                var data = line.Split(';');
                if (data.Length >= 8)
                {
                    var employee = new Employee
                    {
                        FirstName = data[0],
                        LastName = data[1],
                        Sex = data[2],
                        BirthDate = DateTime.ParseExact(data[3], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                        BirthCountry = data[4],
                        Salary = int.Parse(data[5]),
                        SalaryCurrency = (Currency)Enum.Parse(typeof(Currency), data[6]),
                        CompanyRole = (Role)Enum.Parse(typeof(Role), data[7])
                    };
                    Employees.Add(employee);
                }
            }
        }

        private void SaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _currentFilePath = saveFileDialog.FileName;
                SaveEmployees(_currentFilePath);
                _isModified = false;
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveFileAs_Click(sender, e);
            }
            else
            {
                SaveEmployees(_currentFilePath);
                _isModified = false;
            }
        }

        private void SaveEmployees(string filePath)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("FirstName;LastName;Sex;BirthDate;BirthCountry;Salary;SalaryCurrency;CompanyRole");

            foreach (var employee in Employees)
            {
                csv.AppendLine($"{employee.FirstName};{employee.LastName};{employee.Sex};{employee.BirthDate:dd.MM.yyyy};{employee.BirthCountry};{employee.Salary};{employee.SalaryCurrency};{employee.CompanyRole}");
            }

            File.WriteAllText(filePath, csv.ToString());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForUnsavedChanges())
            {
                Application.Current.Shutdown();
            }
        }

        private bool CheckForUnsavedChanges()
        {
            if (_isModified)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes before opening new file?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile_Click(null, null);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listViewEmployees.SelectedIndex;
            if (selectedIndex > 0)
            {
                var itemToMoveUp = Employees[selectedIndex];
                Employees.RemoveAt(selectedIndex);
                Employees.Insert(selectedIndex - 1, itemToMoveUp);
                listViewEmployees.SelectedIndex = selectedIndex - 1;
            }
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = listViewEmployees.SelectedIndex;
            if (selectedIndex < Employees.Count - 1)
            {
                var itemToMoveDown = Employees[selectedIndex];
                Employees.RemoveAt(selectedIndex);
                Employees.Insert(selectedIndex + 1, itemToMoveDown);
                listViewEmployees.SelectedIndex = selectedIndex + 1;
            }
        }

        private void AddNewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (_addEmployeeWindow == null)
            {
                _addEmployeeWindow = new AddEmployeeWindow
                {
                    Owner = this
                };
                _addEmployeeWindow.EmployeeAdded += AddEmployeeWindow_EmployeeAdded;
                _addEmployeeWindow.Closed += (s, args) => _addEmployeeWindow = null;
                _addEmployeeWindow.Show();
            }
            else
            {
                if (_addEmployeeWindow.WindowState == WindowState.Minimized)
                {
                    _addEmployeeWindow.WindowState = WindowState.Normal;
                }
                _addEmployeeWindow.Activate();
            }
        }

        private void AddEmployeeWindow_EmployeeAdded(object sender, Employee e)
        {
            Employees.Add(e);
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (listViewEmployees.SelectedItem is Employee selectedEmployee)
            {
                Employees.Remove(selectedEmployee);
            }
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
