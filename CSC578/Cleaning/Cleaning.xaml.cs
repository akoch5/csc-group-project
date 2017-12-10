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
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;

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

        //Will populate the grid with specific cleaning tasks. Still need to add table to database.  
        public void populateCleaningGrid()
        {
            MainWindow.con = new OleDbConnection();
            MainWindow.con.ConnectionString = ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString();
            OleDbCommand cmd = new OleDbCommand();
            if (MainWindow.con.State != ConnectionState.Open)
                MainWindow.con.Open();
            cmd.Connection = MainWindow.con;
            cmd.CommandText = "select * from cleaningData where CurrentDate = DateValue (' " + Convert.ToDateTime(displayDate.Text) + "')";
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
                cleaningGrid.Columns[4].IsReadOnly = true;
                cleaningGrid.Columns[5].IsReadOnly = true;
            }
        }

        private static List<CleaningObject> getCleaningList()
        {
            List<CleaningObject> cleaningList = new List<CleaningObject>();
            MainWindow.con = new OleDbConnection();
            MainWindow.con.ConnectionString = ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString();
            OleDbCommand cmd = new OleDbCommand();
            if (MainWindow.con.State != ConnectionState.Open)
            MainWindow.con.Open();
            cmd.Connection = MainWindow.con;
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
                        MainWindow.editDB("insert into cleaningData (Task, Assigned, Completed, CurrentDate) " +
                                "values " + $"('{cleaningTask.getTaskName()}', '{cleaningTask.getAssignee()}', {false}, '{currentDate}')");
                        break;
                    case "Weekly":
                        if ((currentDate.DayOfYear- cleaningTask.getStartDate().DayOfYear)%7 == 0)
                        {
                            MainWindow.editDB("insert into cleaningData (Task, Assigned, Completed, CurrentDate) " +
                                "values " + $"('{cleaningTask.getTaskName()}', '{cleaningTask.getAssignee()}', {false}, '{currentDate}')");
                        }
                        break;
                    case "Bi-Weekly":
                        if ((currentDate.DayOfYear - cleaningTask.getStartDate().DayOfYear) % 14 == 0)
                        {
                            MainWindow.editDB("insert into cleaningData (Task, Assigned, Completed, CurrentDate) " +
                                "values " + $"('{cleaningTask.getTaskName()}', '{cleaningTask.getAssignee()}', {false}, '{currentDate}')");
                        }
                        break;
                    case "Monthly":
                        if (currentDate.Day == cleaningTask.getStartDate().Day)
                        {
                            MainWindow.editDB("insert into cleaningData (Task, Assigned, Completed, CurrentDate) " +
                                "values " + $"('{cleaningTask.getTaskName()}', '{cleaningTask.getAssignee()}', {false}, '{currentDate}')");
                        }
                        break;
                    case "One-Time":
                        if (currentDate == cleaningTask.getStartDate())
                        {
                            MainWindow.editDB("insert into cleaningData (Task, Assigned, Completed, CurrentDate) " +
                                "values " + $"('{cleaningTask.getTaskName()}', '{cleaningTask.getAssignee()}', {false}, '{currentDate}')");
                        }
                        break;
                    default:
                        break;
                }
                currentDate = currentDate.AddDays(1);
            }
           
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
                    DataRowView dataRow = (DataRowView)cleaningGrid.CurrentCell.Item;
                    if (dataRow.Row.ItemArray[3].Equals(false))
                    {
                
                        MainWindow.editDB("Update cleaningData set completed = true, CompletedTime = '" + DateTime.Now + "' where ID = " + dataRow.Row.ItemArray[0]);
                        populateCleaningGrid();
                    }
                    else
                    {
                        MainWindow.editDB("Update cleaningData set completed = false, CompletedTime = null where ID = " + dataRow.Row.ItemArray[0]);
                        populateCleaningGrid();
                    }
                                
                }

   
            }

        }

        private void populateAlerts()
        {
            MainWindow.con = new OleDbConnection();
            MainWindow.con.ConnectionString = ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString();
            OleDbCommand cmd = new OleDbCommand();
            if (MainWindow.con.State != ConnectionState.Open)
                MainWindow.con.Open();
            cmd.Connection = MainWindow.con;
            cmd.CommandText = "select * from cleaningData where CurrentDate < DateValue ('" + DateTime.Now + "') and Completed = false" ;
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

    }
}
