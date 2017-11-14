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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static CSC578.Maintenance.Equipment;

namespace CSC578.Maintenance
{
    /// <summary>
    /// Interaction logic for EditEquipmentForm.xaml
    /// </summary>
  //  public partial class EditEquipmentForm : UserControl
    public partial class EditEquipmentForm : Window
    {

        private Equipment equipmentControl;

        public EditEquipmentForm(EquipmentData data, Equipment equipmentControl)
        {
            InitializeComponent();
            Debug.Write("EditEquipmentForm");
            this.equipmentControl = equipmentControl;
            if (data.Id != null)
            {
                EId.Text=data.Id.ToString();
                EName.Text = data.Name;
                EDate.SelectedDate = data.PurchaseDate;
                EDate.DisplayDateStart = data.PurchaseDate;
                EWarranty.Text = data.WarrantyLengthInMonths.ToString();
            }
            
            
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("EditEquipmentForm ButtonOK_Click");
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
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("EditEquipmentForm ButtonCancel_Click");
            this.Close();
        }

    }


}
