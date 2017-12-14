using System;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Windows.Controls;

namespace CSC578.Cleaning
{
    /// <summary>
    /// Interaction logic for Cleaning.xaml
    /// </summary>
    public partial class Cleaning : System.Windows.Controls.UserControl
    {
        public class CleaningObject
        {
            private String taskName;
            private String assignee;
            private String frequency;
            private DateTime startDate;
            private DateTime endDate;

            public CleaningObject(String taskName, String assignee, String frequency, DateTime startDate, DateTime endDate)
            {
                this.taskName = taskName;
                this.assignee = assignee;
                this.frequency = frequency;
                this.startDate = startDate;
                this.endDate = endDate;                
            }

            public String getFrequency()
            {
                return this.frequency;
            }
            public String getTaskName()
            {
                return this.taskName;
            }
            public String getAssignee()
            {
                return this.assignee;
            }
            public DateTime getStartDate()
            {
                return this.startDate;
            }
            public DateTime getEndDate()
            {
                return this.endDate;
            }
        }


        public static List<CleaningObject> cleaningList;


        public Cleaning()
        {
            InitializeComponent();
            cleaningList = getCleaningList();
            displayDate.Text = DateTime.Today.ToString();
            populateCleaningGrid();
            populateAlerts();

        }

       
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            addTaskWindow taskWindow = new addTaskWindow();
            taskWindow.Show();
        }

        private void settingButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }
  
        public void populateCleaningGrid()
        {
             OleDbCommand cmd = MainWindow.dbConnect();
            cmd.CommandText = "select * from cleaningData where DueDate = DateValue (@var1)";
            cmd.Parameters.Add(new OleDbParameter("@var1", Convert.ToDateTime(displayDate.Text)));
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            MainWindow.dt = new DataTable();
            da.Fill(MainWindow.dt);
            cleaningGrid.ItemsSource = MainWindow.dt.AsDataView();
            MainWindow.con.Close();

            if (cleaningGrid.Columns.Count > 0)
            {
                cleaningGrid.Columns[0].IsReadOnly = true;
                cleaningGrid.Columns[0].Visibility = System.Windows.Visibility.Hidden;
                cleaningGrid.Columns[1].IsReadOnly = true;
                cleaningGrid.Columns[2].IsReadOnly = true;
                cleaningGrid.Columns[4].IsReadOnly = true;
                cleaningGrid.Columns[5].IsReadOnly = true;
            }
        }


        public static List<CleaningObject> getCleaningList()
        {
            List<CleaningObject> cleaningList = new List<CleaningObject>();

            OleDbCommand cmd = MainWindow.dbConnect();
            cmd.CommandText = "select * from cleaningTasks";
            OleDbDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
               cleaningList.Add(new CleaningObject(rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetDateTime(4), rdr.GetDateTime(5)));
            }
            rdr.Close();
            MainWindow.con.Close();
            return cleaningList;
        }

        public static void addCleaningData(CleaningObject cleaningTask)
        {
            
            DateTime currentDate = cleaningTask.getStartDate();

            while (cleaningTask.getEndDate().CompareTo(currentDate) >= 0)
            {
                switch (cleaningTask.getFrequency())
                {
                    case "Daily":
                        addDataItem(cleaningTask.getTaskName(), cleaningTask.getAssignee(), false, currentDate);
                        break;
                    case "Weekly":
                        if ((currentDate.DayOfYear- cleaningTask.getStartDate().DayOfYear)%7 == 0)
                        {
                            addDataItem(cleaningTask.getTaskName(), cleaningTask.getAssignee(), false, currentDate);
                        }
                        break;
                    case "Bi-Weekly":
                        if ((currentDate.DayOfYear - cleaningTask.getStartDate().DayOfYear) % 14 == 0)
                        {
                            addDataItem(cleaningTask.getTaskName(), cleaningTask.getAssignee(), false, currentDate);
                        }
                        break;
                    case "Monthly":
                        if (currentDate.Day == cleaningTask.getStartDate().Day)
                        {
                            addDataItem(cleaningTask.getTaskName(), cleaningTask.getAssignee(), false, currentDate);
                        }
                        break;
                    case "One-Time":
                        if (currentDate == cleaningTask.getStartDate())
                        {
                            addDataItem(cleaningTask.getTaskName(), cleaningTask.getAssignee(), false, currentDate);
                        }
                        break;
                    default:
                        break;
                }
                currentDate = currentDate.AddDays(1);
            }
        }

        private static void addDataItem(String name, String assignee, bool complete, DateTime duedate)
        {
            OleDbCommand cmd = MainWindow.dbConnect();
            cmd.CommandText = "insert into cleaningData (Task, Assigned, Completed, DueDate) values (@name, @assignee, @complete, @duedate)";
            cmd.Parameters.Add(new OleDbParameter("@name", name));
            cmd.Parameters.Add(new OleDbParameter("@assignee", assignee));
            cmd.Parameters.Add(new OleDbParameter("@complete", complete));
            cmd.Parameters.Add(new OleDbParameter("@duedate", duedate));
            cmd.ExecuteNonQuery();
            MainWindow.con.Close();
        }


        private void rightButton_Click(object sender, RoutedEventArgs e)
        {
            displayDate.Text = Convert.ToDateTime(displayDate.Text).AddDays(1).ToString();
            populateCleaningGrid();
        }

        private void leftButton_Click(object sender, RoutedEventArgs e)
        {
            displayDate.Text = Convert.ToDateTime(displayDate.Text).AddDays(-1).ToString();
            populateCleaningGrid();
        }


        private void cleaningGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (cleaningGrid.CurrentCell.Column != null)
            {
                if (cleaningGrid.CurrentCell.Column.Header.ToString() == "Completed")
                {
                    if (cleaningGrid.CurrentCell.Item.ToString() != "{NewItemPlaceholder}")
                    {
                        DataRowView dataRow = (DataRowView)cleaningGrid.CurrentCell.Item;
                        if (dataRow.Row.ItemArray[3].Equals(false))
                        {
                            OleDbCommand cmd = MainWindow.dbConnect();
                            cmd.CommandText = "Update cleaningData set completed = true, CompletedTime = '" + DateTime.Now + "' where ID = " + dataRow.Row.ItemArray[0];
                            cmd.ExecuteNonQuery();
                            MainWindow.con.Close();
                            populateCleaningGrid();
                        }
                        else
                        {
                            OleDbCommand cmd = MainWindow.dbConnect();
                            cmd.CommandText = "Update cleaningData set completed = false, CompletedTime = null where ID = " + dataRow.Row.ItemArray[0];
                            cmd.ExecuteNonQuery();
                            MainWindow.con.Close();
                            populateCleaningGrid();
                        }

                        clearAlerts();
                        populateAlerts();
                    }
                }  
            }
        }

        private void populateAlerts()
        {
            OleDbCommand cmd = MainWindow.dbConnect();
            cmd.CommandText = "Parameters @date DateTime; select * from cleaningData where DueDate < DateValue (@date) and Completed = @completed" ;
            cmd.Parameters.Add(new OleDbParameter("@date", DateTime.Now));
            cmd.Parameters.Add(new OleDbParameter("@completed", false));
            OleDbDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                System.Windows.Controls.Label newLabel = new System.Windows.Controls.Label();
                newLabel.Content = "Task Overdue: " + rdr.GetString(1) + " Due: " + rdr.GetDateTime(5).ToString();
                alerts.Items.Add(newLabel);
            }
            rdr.Close();
            MainWindow.con.Close();
           
        }

        private void clearAlerts()
        {
            alerts.Items.Clear();
        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            populateCleaningGrid();
        }

       
    }
}
