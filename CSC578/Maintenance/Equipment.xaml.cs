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

namespace CSC578.Maintenance
{
    /// <summary>
    /// Interaction logic for Equipment.xaml
    /// </summary>
    public partial class Equipment : UserControl
    {
        public class EquipmentData
        {
            private string name;
            private DateTime purchaseDate;
            private int warrantyLengthInMonths;

            public string Name { get => name; set => name = value; }
            public DateTime PurchaseDate { get => purchaseDate; set => purchaseDate = value; }
            public int WarrantyLengthInMonths { get => warrantyLengthInMonths; set => warrantyLengthInMonths = value; }
            public bool WarrantyExpired { get => false; }

            public EquipmentData(string name, DateTime purchaseDate, int warrantyLengthInMonths)
            {
                Name = name;
                PurchaseDate = purchaseDate;
                WarrantyLengthInMonths = warrantyLengthInMonths;
            }
        }

        List<EquipmentData> equipmentDataList = new List<EquipmentData>();

        public Equipment()
        {
            InitializeComponent();
            getData();
            equipment_listbox.ItemsSource = equipmentDataList;
        }

        void getData()
        {
            OleDbConnection con = new OleDbConnection
            {
                ConnectionString =
                    ConfigurationManager.
                        ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].
                            ToString()
            };

            con.Open();
            OleDbCommand cmd = new OleDbCommand
            {
                CommandText = "select * from [EquipmentItems]",
                Connection = con
            };

            OleDbDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                equipmentDataList.Add(new EquipmentData(rd.GetString(1), 
                                                        new DateTime(), 
                                                        rd.GetInt32(3)));
            }
            rd.Close();
        }

    }
}
