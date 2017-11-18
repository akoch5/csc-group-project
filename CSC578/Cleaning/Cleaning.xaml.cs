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

namespace CSC578.Cleaning
{
    /// <summary>
    /// Interaction logic for Cleaning.xaml
    /// </summary>
    public partial class Cleaning : UserControl
    {
        public class CleaningObject
        {
            private String taskName;
            private String assignee;
            private String frequency;
            private String startDate;
            private String endDate;

            public CleaningObject(String taskName, String assignee, String frequency, String startDate, String endDate)
            {
                taskName = this.taskName;
                assignee = this.assignee;
                frequency = this.frequency;
                startDate = this.startDate;
                endDate = this.endDate;                
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
        }

        public static List<CleaningObject> cleaningList;

        public Cleaning()
        {
            InitializeComponent();
            cleaningList = getCleaningList();
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
            cmd.CommandText = "select * from ";
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            MainWindow.dt = new DataTable();
            da.Fill(MainWindow.dt);
            //cleaningGrid.ItemsSource = dt.AsDataView();
            MainWindow.con.Close();
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
                cleaningList.Add(new CleaningObject(rdr["taskName"].ToString(), rdr["assignee"].ToString(), rdr["frequency"].ToString(), rdr["startDate"].ToString(), rdr["endDate"].ToString()));
            }
            rdr.Close();
            MainWindow.con.Close();
            return cleaningList;
        }

        public static void addCleaningData()
        {
            DateTime currentDate = DateTime.Today;
            foreach (CleaningObject c in cleaningList)
            {
                switch (c.getFrequency())
                {
                    case "Daily":
                        break;
                    case "Weekly":
                        break;
                    case "Bi-Weekly":
                        break;
                    case "Monthly":
                        break;
                    case "One-Time":
                        break;
                    default:
                        break;
                }
               
                MainWindow.editDB("insert into cleaningData (Task, Assigned, Completed, CurrentDate) " +
                    "values " + $"('{c.getTaskName()}', '{c.getAssignee()}', {false}, {11/18/17})");

            }
           
        }

    }
}
