using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.OleDb;
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

namespace CSC578.Maintenance
{
    /// <summary>
    /// Interaction logic for EquipmentMaintenance.xaml
    /// </summary>
    public partial class EquipmentMaintenance : UserControl
    {
        ObservableCollection<MaintenanceData> maintenanceDataList = new ObservableCollection<MaintenanceData>();

        public EquipmentMaintenance()
        {
            InitializeComponent();
            GetData();
        }


        public class MaintenanceData
        {
            private int? id;
            private string equipmentName;
            private string maintanceItem;
            private DateTime lastMaintenance;
            private int frequencyMonths;
            private string status;

            public MaintenanceData(int? id, string equipmentName, string maintanceItem, DateTime lastMaintenance, int frequency)
            {
                this.Id = id;
                EquipmentName = equipmentName;
                MaintanceItem = maintanceItem;
                LastMaintenance = lastMaintenance;
                Frequency = frequency;
            }

            public int? Id { get => id; set => id = value; }
            public string EquipmentName { get => equipmentName; set => equipmentName = value; }
            public string MaintanceItem { get => maintanceItem; set => maintanceItem = value; }
            public DateTime LastMaintenance { get => lastMaintenance; set => lastMaintenance = value; }
            public int Frequency { get => frequencyMonths; set => frequencyMonths = value; }
            public string Status { get => status; set => status = value; }
        }

        void GetData()
        {
            maintenanceDataList.Clear();
            // should capture db error exceptions and present a popup
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
                CommandText = "SELECT DISTINCT ei.ID," +
                                              "ei.EquipmentName, " +
                                              "em.MaintanceItem, " +
                                              "em.LastMainenance, " +
                                              "em.FrequencyMonths " +
                                     "FROM EquipmentMaintenance em " +
                                     "INNER JOIN EquipmentItems ei ON (em.EquipmentName = ei.ID)",
                Connection = con
            };
            
            OleDbDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                // Should try this and print equery popup error

                int id = rd.GetInt32(0);
                string equipment_name = rd.GetString(1);
                string maintenance_item = rd.GetString(2);
                DateTime last_maintenance = rd.GetDateTime(3);
                int frequency_months = rd.GetInt32(4);

                maintenanceDataList.Add(new MaintenanceData(id,
                                                            equipment_name,
                                                            maintenance_item,
                                                            last_maintenance,
                                                            frequency_months));
            }
            rd.Close();
            maintenance_listbox.ItemsSource = maintenanceDataList;
        }

        void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

            MaintenanceData equipmentData = new MaintenanceData(null,"","", new DateTime(), 0);
           // EditEquipmentForm editEquipmentForm = new EditEquipmentForm(equipmentData, this);
           // editEquipmentForm.Show();
            
        }

        void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            /*
            EquipmentData equipmentData = (EquipmentData)equipment_listbox.SelectedItem;
            if (equipmentData != null)
            {
                EditEquipmentForm editEquipmentForm = new EditEquipmentForm(equipmentData, this);
                editEquipmentForm.Show();
            }
            */
        }

        void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            /*
            EquipmentData equipmentData = (EquipmentData)equipment_listbox.SelectedItem;
            if (equipmentData != null)
            {
                DeleteItem(equipmentData);
            }
            */
        }


    }
}
