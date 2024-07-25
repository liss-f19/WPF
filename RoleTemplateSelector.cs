using System.Windows;
using System.Windows.Controls;

namespace Employees_management
{
    public class RoleTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ComboBoxTemplate { get; set; }
        public DataTemplate TextBlockTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var employee = item as Employee;
            if (employee != null && employee.CompanyRole == Role.CEO)
            {
                return TextBlockTemplate;
            }
            return ComboBoxTemplate;
        }
    }
}

