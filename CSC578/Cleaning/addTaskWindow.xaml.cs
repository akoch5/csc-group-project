using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.OleDb;
using System.Data;
using System.Configuration;

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
            if (taskCombo.Text == "" || assignedCombo.Text == "" || frequencyCombo.Text == "" || startDate.Text == "" || endDate.Text == "")
            {
                MessageBoxResult invalidDate = MessageBox.Show("All fields must be filled in");
            }

            else
            {
                bool itemExists = false;

                foreach (Cleaning.Cleaning.CleaningObject t in Cleaning.Cleaning.cleaningList)
                {
                    if (t.getTaskName() == taskCombo.Text)
                    {
                        itemExists = true;
                    }
                }

                if (itemExists == true)
                {
                    //Edit the database to account for changes
                    OleDbCommand cmd = MainWindow.dbConnect();
                    cmd.CommandText = "update cleaningTasks set assignee = @assignee, frequency = @frequency, startDate = @startDate, endDate = @endDate where taskName = @task";
                    cmd.Parameters.Add(new OleDbParameter("@assignee", assignedCombo.Text));
                    cmd.Parameters.Add(new OleDbParameter("@frequency", frequencyCombo.Text));
                    cmd.Parameters.Add(new OleDbParameter("@startDate", Convert.ToDateTime(startDate.Text)));
                    cmd.Parameters.Add(new OleDbParameter("@endDate", Convert.ToDateTime(endDate.Text)));
                    cmd.Parameters.Add(new OleDbParameter("@task", taskCombo.SelectedValue.ToString()));
                    cmd.ExecuteNonQuery();
                    MainWindow.con.Close();

                    //Delete any data from the database for the specified task that is greater than or equal to the current date
                    cmd = MainWindow.dbConnect();
                    cmd.CommandText = "Delete from cleaningData where task = @task and DueDate >= DateValue (@date)";
                    cmd.Parameters.Add(new OleDbParameter("@task", taskCombo.Text));
                    cmd.Parameters.Add(new OleDbParameter("@date", DateTime.Today));

                    cmd.ExecuteNonQuery();
                    MainWindow.con.Close();

                    //Repopulate the cleaningData table with the new info starting from the current date.
                    Cleaning.Cleaning.CleaningObject newTask = new Cleaning.Cleaning.CleaningObject(taskCombo.Text, assignedCombo.Text, frequencyCombo.Text, DateTime.Today, Convert.ToDateTime(endDate.Text));
                    Cleaning.Cleaning.addCleaningData(newTask);
                    //Update CleaningList
                    Cleaning.Cleaning.cleaningList = Cleaning.Cleaning.getCleaningList();

                }

                else if (itemExists == false)
                {
                    OleDbCommand cmd = MainWindow.dbConnect();
                    cmd.CommandText = "insert into cleaningTasks (taskName, assignee, frequency, startDate, endDate) values(@task, @assignee, @frequency, @startDate, @endDate)";

                    cmd.Parameters.Add(new OleDbParameter("@task", taskCombo.SelectedValue.ToString()));
                    cmd.Parameters.Add(new OleDbParameter("@assignee", assignedCombo.Text));
                    cmd.Parameters.Add(new OleDbParameter("@frequency", frequencyCombo.Text));
                    cmd.Parameters.Add(new OleDbParameter("@startDate", Convert.ToDateTime(startDate.Text)));
                    cmd.Parameters.Add(new OleDbParameter("@endDate", Convert.ToDateTime(endDate.Text)));

                    cmd.ExecuteNonQuery();
                    MainWindow.con.Close();

                    Cleaning.Cleaning.CleaningObject newTask = new Cleaning.Cleaning.CleaningObject(taskCombo.Text, assignedCombo.Text, frequencyCombo.Text, Convert.ToDateTime(startDate.Text), Convert.ToDateTime(endDate.Text));
                    Cleaning.Cleaning.addCleaningData(newTask);

                }
                this.Close();
            }
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

        private void taskCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool itemExists = false;

            foreach (Cleaning.Cleaning.CleaningObject t in Cleaning.Cleaning.cleaningList)
            {
                string test = taskCombo.SelectedValue.ToString();
                if (t.getTaskName() == taskCombo.SelectedValue.ToString())
                {
                    itemExists = true;
                    frequencyCombo.Text = t.getFrequency();
                    startDate.Text = t.getStartDate().ToString();
                    endDate.Text = t.getEndDate().ToString();
                    assignedCombo.Text = t.getAssignee();
                }
            }

            if(itemExists == false)
            {
                frequencyCombo.Text = null;
                startDate.Text = null;
                endDate.Text = null;
                assignedCombo.Text = null;
            }
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(startDate.Text != "" && endDate.Text != "" && (Convert.ToDateTime(startDate.Text).CompareTo(Convert.ToDateTime(endDate.Text)) >= 1))
            {
                MessageBoxResult invalidDate = MessageBox.Show("End date must be after start date.");
                endDate.Text = "";
            }
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startDate.Text != "" && endDate.Text != "" && (Convert.ToDateTime(startDate.Text).CompareTo(Convert.ToDateTime(endDate.Text)) >= 1))
            {
                MessageBoxResult invalidDate = MessageBox.Show("Start date must be before end date.");
                startDate.Text = "";
            }
        }

    }
}
