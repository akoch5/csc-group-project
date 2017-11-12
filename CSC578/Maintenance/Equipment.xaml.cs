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
        public List<int> equipmentList = new List<int>() { 1, 2, 3 };
        public Equipment()
        {
            InitializeComponent();
            fill_listbox();
            //FindProvider();
        }

        public void FindProvider()
        {
            var reader = OleDbEnumerator.GetRootEnumerator();

            var list = new List<String>();
            while (reader.Read())
            {
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i) == "SOURCES_NAME")
                    {
                        list.Add(reader.GetValue(i).ToString());

                    }
                }
            }
            reader.Close();
            foreach (var provider in list)
            {
                equipment_listbox.Items.Add(provider.ToString());
                if (provider.StartsWith("Microsoft.ACE.OLEDB"))
                {
                    //this.provider = provider.ToString();
                    equipment_listbox.Items.Add(provider.ToString());
                }
            }
        }


        void fill_listbox()
        {
            List<string> data = new List<string>();
            OleDbConnection con = new OleDbConnection();
            con.ConnectionString = 
                ConfigurationManager.ConnectionStrings["CSC578.Properties.Settings.DatabaseConnectionString"].ToString();
            con.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandText = "select * from [EquipmentItems]";
            cmd.Connection = con;
            OleDbDataReader rd = cmd.ExecuteReader();

            while(rd.Read())
            {
                data.Add(rd.GetString(1));
            }
            rd.Close();
            equipment_listbox.ItemsSource = data;
        }
    }
}
