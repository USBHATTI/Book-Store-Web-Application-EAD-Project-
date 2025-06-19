using Microsoft.Win32;
using myEAD.Models;
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
using System.Windows.Shapes;

namespace myEAD
{
    /// <summary>
    /// Interaction logic for update.xaml
    /// </summary>
    /// <summary>
    /// Interaction logic for deleteItem.xaml
    /// </summary>
    public partial class update : Window
    {
        private List<Item> items; // To store all items from the database
        
        private Item selectedItem; // Keep track of the currently selected item
        public update()
        {

            InitializeComponent();

            LoadItems(); // Load items when the window initializes
        }

        // Show the Popup when the TextBox gains focus



        private void CloseImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close(); // Close the application
        }

        private void MinimizeImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimize the application
        }

        private void backImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            admin adminWindow = new admin(); // Create an instance of the Home window
            adminWindow.Show(); // Show the Home window
            this.Close(); // Optional: Close the current window
        }

        private void LoadItems()
        {
            try
            {
                using (var context = new EadContext())
                {
                    items = context.Items.ToList(); // Fetch all items from the database
                    item_box.ItemsSource = items.Select(item => item.Name).ToList(); // Bind only names to the ComboBox
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void item_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (item_box.SelectedItem == null) return;

            // Get the selected item's name
            string selectedItemName = item_box.SelectedItem.ToString();

            // Find the selected item in the global list
            selectedItem = items.FirstOrDefault(item => item.Name == selectedItemName);

            if (selectedItem != null)
            {
                // Populate the fields with the selected item's details
                des_Box.Text = selectedItem.Description;
                quantity_slider.Value = (double)selectedItem.Quantity; // Cast Quality to double
                price_box.Text = selectedItem.Price.ToString();
                
            }
        }





        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItem == null)
            {
                MessageBox.Show("Please select an item to update.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate fields
            if (string.IsNullOrWhiteSpace(des_Box.Text) || string.IsNullOrWhiteSpace(price_box.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(price_box.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int priceAsInt = (int)price;

            try
            {
                using (var context = new EadContext())
                {
                    // Fetch the item from the database to update
                    var itemToUpdate = context.Items.FirstOrDefault(item => item.Name == selectedItem.Name);

                    if (itemToUpdate != null)
                    {
                        // Update the item's properties
                        itemToUpdate.Description = des_Box.Text;
                        itemToUpdate.Quantity = (int)quantity_slider.Value;
                        itemToUpdate.Price = priceAsInt;
                        

                        // Save changes to the database
                        context.SaveChanges();

                        MessageBox.Show("Item updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh the global list and ComboBox
                        LoadItems();
                        item_box.SelectedItem = null; // Clear selection
                    }
                    else
                    {
                        MessageBox.Show("Selected item not found in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
