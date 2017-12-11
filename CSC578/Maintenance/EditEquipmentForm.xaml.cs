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


            try
            {
                int? id = null;
                if (EId.Text != "")
                    id = Int32.Parse(EId.Text);

                var equipmentData = new EquipmentData(id,
                                                 EName.Text,
                                                 EDate.SelectedDate.Value.Date,
                                                 Int32.Parse(EWarranty.Text));

                equipmentData.Validate();

                if(id == null)
                {
                    equipmentControl.AddItem(equipmentData);
                }
                else
                {
                    equipmentControl.ModifyItem(equipmentData);
                }

                this.Close();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Invalid Input");
                TraceMessage("Invalid Input: " + exception.Message);
            }
                                  
        }

        public void TraceMessage(string message,
                                [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
                                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
                                [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            System.Diagnostics.Trace.WriteLine("message: " + message);
            System.Diagnostics.Trace.WriteLine("member name: " + memberName);
            System.Diagnostics.Trace.WriteLine("file: " + sourceFilePath + ": " + sourceLineNumber);
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("EditEquipmentForm ButtonCancel_Click");
            this.Close();
        }

    }


}
