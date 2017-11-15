using System;
using System.Collections.Generic;
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



        private Equipment equipmentControl; // todo 

        public EditMaintenanceForm(EditMaintenanceForm data, Equipment equipmentControl)
        {
            InitializeComponent();
            Debug.Write("EditMaintenanceForm");
            this.equipmentControl = equipmentControl;
          //  if (data.Id != null)
            {
 
            }


        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("EditMaintenanceForm ButtonOK_Click");
            /*
            if (EId.Text != "")
            {
                
                int id = Int32.Parse(EId.Text);
                int warrantyMonths = Int32.Parse(EWarranty.Text);
                equipmentControl.ModifyItem(new EquipmentData(id,
                                                   EName.Text,
                                                   EDate.SelectedDate.Value.Date,
                                                   warrantyMonths));
                                            
            }
            else
            {
                equipmentControl.AddItem(new EquipmentData(null,
                                                   EName.Text,
                                                   EDate.SelectedDate.Value.Date,
                                                   Int32.Parse(EWarranty.Text)));
            }
            this.Close();
            */
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("EditMaintenanceForm ButtonCancel_Click");
            this.Close();
        }

    }


}

