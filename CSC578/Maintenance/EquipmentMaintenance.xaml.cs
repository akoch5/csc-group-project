using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.OleDb;
using System.Diagnostics;
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
        ObservableCollection<MaintenanceData> _maintenanceDataList = new ObservableCollection<MaintenanceData>();
        
        IDictionary<string, int> _availableEquipment = new Dictionary<string, int>();
        public EquipmentMaintenance()
        {
            InitializeComponent();
            GetData();
        }


        public class MaintenanceData
        {
            private int? id;
            private string equipmentName;
            private int equipmentID;
            private string maintanceItem;
            private DateTime lastMaintenance;
            private int frequencyMonths;
            private string status;

            public MaintenanceData(int? id, int equipmentID, string equipmentName, string maintanceItem, DateTime lastMaintenance, int frequency)
            {
                this.Id = id;
                EquipmentName = equipmentName;
                MaintanceItem = maintanceItem;
                LastMaintenance = lastMaintenance;
                Frequency = frequency;
                EquipmentID = equipmentID;
            }

            public int? Id { get => id; set => id = value; }
            public string EquipmentName { get => equipmentName; set => equipmentName = value; }
            public string MaintanceItem { get => maintanceItem; set => maintanceItem = value; }
            public DateTime LastMaintenance { get => lastMaintenance; set => lastMaintenance = value; }
            public int Frequency { get => frequencyMonths; set => frequencyMonths = value; }
            public string Status { get => CalculateStatus(); set => status = value; }
            public int EquipmentID { get => equipmentID; set => equipmentID = value; }

            string CalculateStatus()
            {
                DateTime next_maintenance = new DateTime(LastMaintenance.Year, LastMaintenance.Month, LastMaintenance.Day)
                    .AddMonths(Frequency);

                int compareExpired = next_maintenance.CompareTo(DateTime.Today);
                if (compareExpired < 0)
                {
                    return "Overdue";
                }

                int compareOverDueSoon = next_maintenance.CompareTo(DateTime.Today.AddMonths(1));
                if (compareOverDueSoon < 0)
                {
                    return "Due Soon";
                }
                return "Good";
            }

            internal void Validate()
            {
                if (EquipmentName == "" || ContainsForbiddenCharacters(EquipmentName) ||
                    MaintanceItem == "" || ContainsForbiddenCharacters(MaintanceItem) ||
                   LastMaintenance == null)
                    throw new Exception();
            }

            bool ContainsForbiddenCharacters(string str)
            {

                return str.IndexOfAny("~!@#$%^&*()_+-=`{}[]:\"';,./<>?".ToCharArray()) != -1;
            }
        }

        public void GetData()
        {
            _maintenanceDataList.Clear();
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
                CommandText = "SELECT DISTINCT " +
                                              "em.ID, " +
                                              "ei.ID," +
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
                int equipment_id = rd.GetInt32(1);
                string equipment_name = rd.GetString(2);
                string maintenance_item = rd.GetString(3);
                DateTime last_maintenance = rd.GetDateTime(4);
                int frequency_months = rd.GetInt32(5);

                _maintenanceDataList.Add(new MaintenanceData(id,
                                                            equipment_id,
                                                            equipment_name,
                                                            maintenance_item,
                                                            last_maintenance,
                                                            frequency_months));
            }
            rd.Close();
            maintenance_listbox.ItemsSource = _maintenanceDataList;


            cmd.CommandText = "SELECT DISTINCT " +
                                              "ID," +
                                              "EquipmentName " +
                                        "FROM EquipmentItems " +
                                        "ORDER BY ID"
                                   ;
            rd = cmd.ExecuteReader();
            _availableEquipment.Clear();
            while (rd.Read())
            {
                int id = rd.GetInt32(0);
                string equipment_name = rd.GetString(1);
                _availableEquipment[equipment_name] = id;
            }
        }

        void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {

            MaintenanceData maintenanceData = new MaintenanceData(null, 0, "", "", new DateTime(), 0);
            EditMaintenanceForm editMaintenanceForm = new EditMaintenanceForm(maintenanceData, 
                                                                              this,
                                                                              GenerateEquipmentList());
            editMaintenanceForm.Show();
        }
        
        public int EquipmentNameToId(string name)
        {
            int id;
            try
            {
                id = _availableEquipment[name];
            }
            catch
            {
                id = -1;
            }
            return id;
        }

        ObservableCollection<string> GenerateEquipmentList()
        {
            return new ObservableCollection<string>(_availableEquipment.Keys);
        }

        void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            
            MaintenanceData maintenanceData = (MaintenanceData)maintenance_listbox.SelectedItem;
            if (maintenanceData != null)
            {
                EditMaintenanceForm editMaintenanceForm = new EditMaintenanceForm(maintenanceData, this, GenerateEquipmentList());
                editMaintenanceForm.Show();
            }
            
        }

        void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            MaintenanceData maintenanceData = (MaintenanceData)maintenance_listbox.SelectedItem;
            if (maintenanceData != null)
            {
                DeleteItem(maintenanceData);
            }
        }


        internal void AddItem(MaintenanceData maintenanceData)
        {
            string query = "INSERT INTO EquipmentMaintenance " +
                                "(EquipmentName, MaintanceItem, LastMainenance, FrequencyMonths) VALUES " +
                               $" ('{maintenanceData.EquipmentID}', " +
                               $"  '{maintenanceData.MaintanceItem}', " +
                               $"  '{maintenanceData.LastMaintenance.ToString()}'," +
                               $"   {maintenanceData.Frequency.ToString()})";
            Debug.Print(query);
            WriteDB(query);
            GetData();
            
        }

        internal void ModifyItem(MaintenanceData maintenanceData)
        {
            Debug.Write("EquipmentMaintenance::ModifyItem");

            string query = $"update EquipmentMaintenance set  " +
                                         $"         MaintanceItem        = '{maintenanceData.MaintanceItem}', " +
                                         $"         EquipmentName        =  {maintenanceData.EquipmentID}," +
                                         $"         LastMainenance       = '{maintenanceData.LastMaintenance.ToString()}'," +
                                         $"         FrequencyMonths      =  {maintenanceData.Frequency}" +
                                         $"         WHERE ID             =  {maintenanceData.Id}";
            WriteDB(query);
            GetData();
        }

        internal void DeleteItem(MaintenanceData maintenanceData)
        {
            Debug.Write("EquipmentMaintenance::DeleteItem");

            string query = $"DELETE * from EquipmentMaintenance WHERE ID = {maintenanceData.Id}";

            WriteDB(query);
            GetData();
        }

        // This should not live here, it is duplicated twice in my code, and other code exists for this in the app. 
        internal void WriteDB(String query)
        {
            Debug.Write("EquipmentMaintenance:WriteDB");
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
                Debug.Write("Equipment::WriteDB ButtonOK_Click: Failed to save: " + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }


        }

        private void Maintenance_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
    }
}