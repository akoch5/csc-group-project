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
using System.Configuration;

namespace CSC578
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static OleDbConnection con;
        public static DataTable dt;

        public MainWindow()
        {
            InitializeComponent();
            
        }



        //Enter SQL command as paramter to edit DB. 
       // public static void editDB(String command)
       // {
           // OleDbCommand cmd = new OleDbCommand();
         //   if (con.State != ConnectionState.Open)
           //     con.Open();
           // cmd.Connection = con;
           //// cmd.CommandText = command;
          //  cmd.ExecuteNonQuery();
          //  con.Close();

       // }

            public static OleDbCommand dbConnect()
        {
            con = new OleDbConnection();
            con.ConnectionString = ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString();
            OleDbCommand cmd = new OleDbCommand();
            if (con.State != ConnectionState.Open)
                con.Open();
            cmd.Connection = con;
            return cmd;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
