using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CSC578
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void btnTasks_Click(object sender, RoutedEventArgs e)
        {
            TaskList taskListWindow = new TaskList();
            taskListWindow.Show();
        }

        private void btnEmployees_Click(object sender, RoutedEventArgs e)
        {
            EmployeeList employeeListWindow = new EmployeeList();
            employeeListWindow.Show();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
