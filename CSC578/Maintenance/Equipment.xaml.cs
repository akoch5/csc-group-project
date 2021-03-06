﻿using System;
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

namespace CSC578.Maintenance
{
    /// <summary>
    /// Interaction logic for Equipment.xaml
    /// </summary>
    public partial class Equipment : UserControl
    {
        public class EquipmentData
        {
            private int? id;
            private string name;
            private DateTime purchaseDate;
            private int warrantyLengthInMonths;

            public int? Id { get => id; set => id = value; }
            public string Name { get => name; set => name = value; }
            public DateTime PurchaseDate { get => purchaseDate; set => purchaseDate = value; }
            public int WarrantyLengthInMonths { get => warrantyLengthInMonths; set => warrantyLengthInMonths = value; }
            public string WarrantyExpired { get => GetWarrantyExpired(); }

            public EquipmentData(int? id, string name, DateTime purchaseDate, int warrantyLengthInMonths)
            {
                Id = id;
                Name = name;
                PurchaseDate = purchaseDate;
                WarrantyLengthInMonths = warrantyLengthInMonths;
            }

            string GetWarrantyExpired()
            {
                DateTime expiration_date = new DateTime(purchaseDate.Year, 
                                                        purchaseDate.Month, 
                                                        purchaseDate.Day)
                                                            .AddMonths(WarrantyLengthInMonths);

                return (expiration_date.CompareTo(DateTime.Today) > 0) ? "No" : "Yes";
            }

            internal void Validate()
            {
                if(Name == "" || ContainsForbiddenCharacters(Name) || 
                   PurchaseDate == null)
                        throw new Exception();
               

            }

            bool ContainsForbiddenCharacters(string str)
            {

                return str.IndexOfAny("~!@#$%^&*()_+-=`{}[]:\"';,./<>?".ToCharArray()) != -1;
            }

        }


        ObservableCollection<EquipmentData> equipmentDataList = new ObservableCollection<EquipmentData>();

        public Equipment()
        {
            InitializeComponent();         
            GetData();
            
        }

        public void GetData()
        {
            equipmentDataList.Clear();
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
                Debug.Write("Equipment::WriteDB ButtonOK_Click: Failed to save: " + ex.Message);
                con.Close();
                return;
            }
           
            OleDbCommand cmd = new OleDbCommand
                {
                    CommandText = "select * from [EquipmentItems]",
                    Connection = con
                };
            try
            { 
                OleDbDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    // Should try this and print equery popup error
                    equipmentDataList.Add(new EquipmentData(rd.GetInt32(0),
                                                            rd.GetString(1),
                                                            rd.GetDateTime(2),
                                                            rd.GetInt32(3)));
                }
                rd.Close();
                equipment_listbox.ItemsSource = equipmentDataList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to read data: " + ex.Message);
                Debug.Write("Equipment::WriteDB ButtonOK_Click: Failed to save: " + ex.Message);
            }
            finally
            {
                con.Close();
            }
        }


        void ButtonAdd_Click(object sender, RoutedEventArgs e)        
        {
            EquipmentData equipmentData = new EquipmentData(null, "", new DateTime(), 0); 
            EditEquipmentForm editEquipmentForm = new EditEquipmentForm(equipmentData, this);
            editEquipmentForm.Show();
        }

        void ButtonModify_Click(object sender, RoutedEventArgs e)
        {
            EquipmentData equipmentData = (EquipmentData)equipment_listbox.SelectedItem;
            if(equipmentData != null)
            {
                EditEquipmentForm editEquipmentForm = new EditEquipmentForm(equipmentData, this);
                editEquipmentForm.Show();
            }
            
        }

        void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            EquipmentData equipmentData = (EquipmentData)equipment_listbox.SelectedItem;
            if (equipmentData != null)
            {
                DeleteItem(equipmentData);
            }
        }

        internal void AddItem(EquipmentData equipmentData)
        {

            string query = "INSERT INTO EquipmentItems " +
                                "(EquipmentName, DatePurchased, WarrantyLengthMonths) VALUES " +
                               $" ('{equipmentData.Name}', '{equipmentData.PurchaseDate.ToString()}', {equipmentData.WarrantyLengthInMonths})"; 
                                         
            WriteDB(query);
        }

        internal void ModifyItem(EquipmentData equipmentData)
        {
            Debug.Write("Equipment::ModifyItem");

            string query = $"update EquipmentItems set  EquipmentName        = '{equipmentData.Name}', " +
                                         $"         DatePurchased        = '{equipmentData.PurchaseDate.ToString()}'," +
                                         $"         WarrantyLengthMonths = {equipmentData.WarrantyLengthInMonths}" +
                                         $"         WHERE ID             = {equipmentData.Id}";
            WriteDB(query);
        }

        internal void DeleteItem(EquipmentData equipmentData)
        {
            Debug.Write("Equipment::DeleteItem");

            string query = $"DELETE * from EquipmentItems WHERE ID = {equipmentData.Id}";

            WriteDB(query);
        }

        internal void WriteDB(String query)
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
                Debug.Write("Equipment::WriteDB ButtonOK_Click: Failed to save: " + ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }

            GetData();
        }

        private void equipment_listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
