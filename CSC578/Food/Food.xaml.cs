using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static CSC578.Food.FoodTab;

namespace CSC578.Food
{
    /// <summary>
    /// Interaction logic for Food.xaml
    /// </summary>
    /// 

    

    public partial class FoodTab : UserControl
    {
        public FoodTab()
        {
            InitializeComponent();
            DataContext = this;
            populateFoodDropDown();
            populateExpDtDropDown();

        }

        OleDbConnection openDbConn()
        {
            OleDbConnection con = new OleDbConnection
            {
                ConnectionString =
                    ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString()
            };

            con.Open();
            return con;
        }

        public ObservableCollection<ComboBoxItem> cbFoodItems { get; set; }
        public ComboBoxItem SelectedcbFoodItem { get; set; }

        public ObservableCollection<ComboBoxItem> cbExpDts { get; set; }
        public ComboBoxItem SelectedcbExpDt { get; set; }

        public ObservableCollection<ComboBoxItem> cbFoodGroups { get; set; }
        public ComboBoxItem SelectedcbFoodGroup { get; set; }

        void populateExpDtDropDown()
        {
            cbExpDts = new ObservableCollection<ComboBoxItem>();
            var cbExpDt = new ComboBoxItem { Content = "Select Criteria" };
            SelectedcbExpDt = cbExpDt;
            cbExpDts.Add(cbExpDt);
            cbExpDts.Add(new ComboBoxItem { Content = "=" });
            cbExpDts.Add(new ComboBoxItem { Content = ">" });
            cbExpDts.Add(new ComboBoxItem { Content = "<" });

        }
        void populateFoodDropDown()
        {
            OleDbConnection con = openDbConn();
            OleDbCommand cmd = new OleDbCommand
            {
                CommandText = "select * from [FoodItemCategories]",
                Connection = con
            };

            OleDbDataReader rd = cmd.ExecuteReader();
            cbFoodItems = new ObservableCollection<ComboBoxItem>();
            cbFoodGroups = new ObservableCollection<ComboBoxItem>();
            var cbFoodItem = new ComboBoxItem { Content = "Select Food Type" };
            var cbFoodGroup = new ComboBoxItem { Content = "Select Food Group" };
            SelectedcbFoodItem = cbFoodItem;
            SelectedcbFoodGroup = cbFoodGroup;
            cbFoodItems.Add(cbFoodItem);
            cbFoodGroups.Add(cbFoodGroup);
            while (rd.Read())
            {
                cbFoodItems.Add(new ComboBoxItem { Content = rd.GetString(1) });
                cbFoodGroups.Add(new ComboBoxItem { Content = rd.GetString(1) });

            }
            rd.Close();
        }

        internal void WriteToDB(String query)
        {
            Debug.Write("Equipment:WriteDB");
            OleDbConnection dbConnection = new OleDbConnection
            {
                ConnectionString =
                    ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString()
            };

            try
            {
                dbConnection.Open();


                OleDbCommand dbCommand = new OleDbCommand
                {
                    CommandText = query,
                    Connection = dbConnection
                };

                dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save: " + ex.Message);
                Debug.Write("Food::WriteDB: Failed to save: " + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }


        }
        //Food Tabbed Menu Action methods Start

        private void foodAddSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Add>>Submit Button Clicked");
            string insertQuery = "INSERT INTO FoodItems " +
                "(FoodCategory, FoodName, Quantity, Threshold, ExpirationDate) VALUES " +
                $"('{foodTypeCombo.Text}'," +
                $" '{foodNameText.Text}'," +
                $" '{int.Parse(foodQtyText.Text)}'," +
                $"'{int.Parse(foodThresholdText.Text)}'," +
                $"'{foodExpirationDate.SelectedDate.Value}')";

            WriteToDB(insertQuery);
        }

        private void foodAddCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Add>>Cancel Button Clicked");            
        }

        private void foodSearchSubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Search>>Submit Button Clicked");
        }

        private void foodSearchCancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Debug.Write("Food>>Search>>Cancel Button Clicked");
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Food Tabbed Menu Action methods End
    }
}
