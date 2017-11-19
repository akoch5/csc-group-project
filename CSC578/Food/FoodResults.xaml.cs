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
using System.Configuration;
using System.Data.OleDb;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace CSC578.Food
{
    /// <summary>
    /// Interaction logic for FoodResult.xaml
    /// </summary>
    public partial class FoodResults : Window 
    {
        private string foodCategory;
        public string FoodCategory { get => foodCategory; set => foodCategory = value; }
        public class FoodResult
        {
            private int? id;
            private string foodCategory;
            private string name;
            private int quantity;
            private int threshold;
            private DateTime expirationDate;

            public int? Id { get => id; set => id = value; }
            public string Name { get => name; set => name = value; }
            public string FoodCategory { get => foodCategory; set => foodCategory = value; }
            public int Quantity { get => quantity; set => quantity = value; }
            public int Threshold { get => threshold; set => threshold = value; }
            public DateTime ExpirationDate { get => expirationDate; set => expirationDate = value; }


            public FoodResult(int? id, string name, string foodCategory, int quantity, int threshold, DateTime expirationDate)
            {
                Id = id;
                Name = name;
                FoodCategory = foodCategory;
                Threshold = threshold;
                Quantity = quantity;
                ExpirationDate = expirationDate;
            }

        }


        ObservableCollection<FoodResult> foodResultList = new ObservableCollection<FoodResult>();

        public FoodResults(string foodCategory)
        {
            FoodCategory = foodCategory;
            InitializeComponent();
            GetData();

        }

        public void GetData()
        {
            foodResultList.Clear();
            // should capture db error exceptions and present a popup
            OleDbConnection con = new OleDbConnection
            {
                ConnectionString =
                    ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString()
            };

            try
            {
                con.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to db: " + ex.Message);
                con.Close();
                return;
            }

            OleDbCommand cmd = new OleDbCommand
            {
                CommandText = "select * from [FoodItems] where FoodCategory='"+foodCategory+"'",
                Connection = con
            };
            try
            {
                OleDbDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    // Should try this and print equery popup error
                    foodResultList.Add(new FoodResult(rd.GetInt32(0), rd.GetString(2), rd.GetString(1),
                                                            rd.GetInt32(3),
                                                            rd.GetInt32(4),
                                                            rd.GetDateTime(5)));
                }
                rd.Close();
                foodResults_list.ItemsSource = foodResultList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to read data: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void foodResultsOkButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
