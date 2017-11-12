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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CSC578
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OleDbConnection con;
        DataTable dt;

        public MainWindow()
        {
            InitializeComponent();
            con = new OleDbConnection();
            //Need to change the Data Source to match wherever the DB is stored on local machine.
            // In final version, should check to see if DB exists in a specific folder. If not, create folder and add DB to make it consistent across all machines. 
            con.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=C:\\Users\\admin\\Documents\\csc-group-project\\Database.accdb";

        }



        //Enter SQL command as paramter to edit DB. 
        private void addData(String command)
        {
            OleDbCommand cmd = new OleDbCommand();
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd.Connection = con;
            cmd.CommandText = command;
            cmd.ExecuteNonQuery();
            con.Close();

        }


         //Food Tabbed Menu Action methods Start

        private void foodSearchSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Search>>Submit Button Clicked");
        }

        private void foodSearchCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Search>>Cancel Button Clicked");
        }

        private void foodAddSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Add>>Submit Button Clicked");
        }

        private void foodAddCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Add>>Cancel Button Clicked");
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Food Tabbed Menu Action methods End

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


    }
}
