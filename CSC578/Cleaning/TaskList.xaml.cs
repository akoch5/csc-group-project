using System.Windows;
using System.Data.OleDb;
using System.Data;
using System.Configuration;

namespace CSC578
{
    /// <summary>
    /// Interaction logic for TaskList.xaml
    /// </summary>
    public partial class TaskList : Window
    {
        public TaskList()
        {
            InitializeComponent();
            populateTaskList();

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            addGrid.Visibility = System.Windows.Visibility.Visible; 
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (taskGrid.SelectedItem != null)
            {
                string data = ((System.Data.DataRowView)taskGrid.SelectedItem).Row.ItemArray[0].ToString();
                OleDbCommand cmd = MainWindow.dbConnect();
                cmd.CommandText = "delete from cleaningList where taskName = @data";
                cmd.Parameters.Add(new OleDbParameter("@data", data));

                cmd.ExecuteNonQuery();
                MainWindow.con.Close();

                populateTaskList();
            }
        }

       private void btnAddOK_Click(object sender, RoutedEventArgs e)
        {
            OleDbCommand cmd = MainWindow.dbConnect();
            cmd.CommandText = "insert into cleaningList (taskName) values (@task)";
            cmd.Parameters.Add(new OleDbParameter("@task", newTasktxt.Text));
            cmd.ExecuteNonQuery();
            MainWindow.con.Close();

            populateTaskList();
            addGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        public void populateTaskList()
        {
            OleDbCommand cmd = MainWindow.dbConnect();
            cmd.CommandText = "select taskName from cleaningList";
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            MainWindow.dt = new DataTable();
            da.Fill(MainWindow.dt);
            taskGrid.ItemsSource = MainWindow.dt.AsDataView();
            MainWindow.con.Close();
        }
    }

}
