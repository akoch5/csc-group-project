using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using static CSC578.Maintenance.EquipmentMaintenance;

namespace CSC578.Maintenance
{
    /// <summary>
    /// Interaction logic for EditMaintenanceForm.xaml
    /// </summary>
    public partial class EditMaintenanceForm : Window
    {
        public EditMaintenanceForm()
        {
            InitializeComponent();
        }

        ObservableCollection<string> _availableEquipmentList;

        private EquipmentMaintenance _equipmentMaintenance;
        
        public EditMaintenanceForm(MaintenanceData data, 
                                   EquipmentMaintenance equipmentMaintenance,
                                   ObservableCollection<string> availableEquipmentList
            )
        {
            InitializeComponent();
            Debug.Write("EditMaintenanceForm");
            this._equipmentMaintenance = equipmentMaintenance;
            this._availableEquipmentList = availableEquipmentList;
            
            if (data.Id != null)
            {
                MId.Text = data.Id.ToString();
                M_LastMaintenace.SelectedDate = data.LastMaintenance;
                M_LastMaintenace.DisplayDateStart = data.LastMaintenance;
                M_Task.Text = data.MaintanceItem;
                M_Name.SelectedIndex = data.EquipmentID -1;
                M_Frequency.Text = data.Frequency.ToString();
            }
            M_Name.ItemsSource = _availableEquipmentList;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("EditMaintenanceForm ButtonOK_Click");

            try
            {
                int? id = null;
                if (MId.Text != "")
                    id = Int32.Parse(MId.Text);

                var maintenanceData = new MaintenanceData(id,
                                                      _equipmentMaintenance.EquipmentNameToId((string)M_Name.SelectedValue),
                                                      (string)M_Name.SelectedValue,
                                                      M_Task.Text,
                                                      M_LastMaintenace.SelectedDate.Value.Date,
                                                      Int32.Parse(M_Frequency.Text));

                maintenanceData.Validate();

                if (id == null)
                {
                    _equipmentMaintenance.AddItem(maintenanceData);
                }
                else
                {
                    _equipmentMaintenance.ModifyItem(maintenanceData);
                }
                this.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Input");
            }

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("EditMaintenanceForm ButtonCancel_Click");
            this.Close();
        }

    }


}

