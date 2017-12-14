using System.Windows;
using System.Data.OleDb;
using System.Data;
using System.Configuration;

namespace CSC578
{
    /// <summary>
    /// Interaction logic for EmployeeList.xaml
    /// </summary>
    public partial class EmployeeList : Window
    {
        public EmployeeList()
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
            if (employeeGrid.SelectedItem != null)
            {
                string data = ((System.Data.DataRowView)employeeGrid.SelectedItem).Row.ItemArray[0].ToString();
                OleDbCommand cmd = MainWindow.dbConnect();
                cmd.CommandText = "delete from employeeList where initials = @data";
                cmd.Parameters.Add(new OleDbParameter("@data", data));
                cmd.ExecuteNonQuery();
                MainWindow.con.Close();
                populateTaskList();
            }
        }

        private void btnAddOK_Click(object sender, RoutedEventArgs e)
        {
            if (newEmployeetxt.Text.Length != 2)
            {
                MessageBoxResult invalidDate = MessageBox.Show("Please Enter a Two-Letter Initial");
                newEmployeetxt.Text = "";
            }

            else
            {
                OleDbCommand cmd = MainWindow.dbConnect();
                cmd.CommandText = "insert into employeeList (initials) values (@init)";
                cmd.Parameters.Add(new OleDbParameter("@init", newEmployeetxt.Text));
                cmd.ExecuteNonQuery();
                MainWindow.con.Close();
                populateTaskList();
                addGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public void populateTaskList()
        {
            OleDbCommand cmd = MainWindow.dbConnect();
            cmd.CommandText = "select initials from employeeList";
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            MainWindow.dt = new DataTable();
            da.Fill(MainWindow.dt);
            employeeGrid.ItemsSource = MainWindow.dt.AsDataView();
            MainWindow.con.Close();
        }
    }
}
