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
            string data = ((System.Data.DataRowView)taskGrid.SelectedItem).Row.ItemArray[0].ToString();
            MainWindow.editDB("delete from cleaningList where taskName = (\"" + data + "\");");
            populateTaskList();
        }

       private void btnAddOK_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.editDB("insert into cleaningList (taskName) values (\"" + newTasktxt.Text + "\");");
            populateTaskList();
            addGrid.Visibility = System.Windows.Visibility.Hidden;
        }

        public void populateTaskList()
        {
            OleDbCommand cmd = new OleDbCommand();
            if (MainWindow.con.State != ConnectionState.Open)
                MainWindow.con.Open();
            cmd.Connection = MainWindow.con;
            cmd.CommandText = "select taskName from cleaningList";
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            MainWindow.dt = new DataTable();
            da.Fill(MainWindow.dt);
            taskGrid.ItemsSource = MainWindow.dt.AsDataView();
            MainWindow.con.Close();
        }
    }

}
