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
using System.Data.OleDb;
using System.Data;

namespace CSC578
{
    /// <summary>
    /// Interaction logic for addTaskWindow.xaml
    /// </summary>
    public partial class addTaskWindow : Window
    {
        public addTaskWindow()
        {
            InitializeComponent();

            populateTaskCombo();
            populateEmployeeCombo();
            populateFrequency();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.editDB("insert into cleaningTasks (taskName, assignee, frequency, startDate, endDate) " +
              "values " + $" ('{taskCombo.Text}', '{assignedCombo.Text}', '{frequencyCombo.Text}', '{Convert.ToDateTime(startDate.Text)}' , '{Convert.ToDateTime(endDate.Text)}')");
            Cleaning.Cleaning.CleaningObject newTask = new Cleaning.Cleaning.CleaningObject(taskCombo.Text, assignedCombo.Text, frequencyCombo.Text, Convert.ToDateTime(startDate.Text), Convert.ToDateTime(endDate.Text));
            Cleaning.Cleaning.addCleaningData(newTask);
            this.Close();
        }

        private void populateTaskCombo()
        {
            MainWindow.con.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter(new OleDbCommand("select * from cleaningList;", MainWindow.con));
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            taskCombo.DataContext = ds.Tables[0];
            taskCombo.DisplayMemberPath = "taskName";
            taskCombo.SelectedValuePath = "taskName";
            MainWindow.con.Close();

        }

        private void populateEmployeeCombo()
        {
            MainWindow.con.Open();
            OleDbDataAdapter adapter = new OleDbDataAdapter(new OleDbCommand("select * from employeeList;", MainWindow.con));
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            assignedCombo.DataContext = ds.Tables[0];
            assignedCombo.DisplayMemberPath = "initials";
            assignedCombo.SelectedValuePath = "initials";
            MainWindow.con.Close();

        }

        private void populateFrequency()
        {
            String[] frequency = { "Daily", "Weekly", "Bi-Weekly", "Monthly", "One-Time" };
            frequencyCombo.ItemsSource = frequency;
        }
    }
}
