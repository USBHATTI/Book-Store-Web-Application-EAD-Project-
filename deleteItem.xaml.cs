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
    public partial class deleteItem : Window
    {
        private List<Item> items; // To store all items from the database
        public deleteItem()
        {
            InitializeComponent();

            LoadItems(); // Load items when the window initializes
        }
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



        // Event to load item names into item_box
        private void item_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // No specific logic for now; name selection is handled on delete.
        }

        // Delete button click event
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure an item is selected
            if (item_box.SelectedItem == null)
            {
                MessageBox.Show("Please select an item to delete.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the selected item name
            string selectedItemName = item_box.SelectedItem.ToString();

            try
            {
                using (var context = new EadContext())
                {
                    // Find the item in the database by name
                    var itemToDelete = context.Items.FirstOrDefault(item => item.Name == selectedItemName);

                    if (itemToDelete != null)
                    {
                        // Remove the item from the database
                        context.Items.Remove(itemToDelete);
                        context.SaveChanges();

                        MessageBox.Show($"Item '{selectedItemName}' has been deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh the ComboBox after deletion
                        LoadItems();
                    }
                    else
                    {
                        MessageBox.Show("Selected item could not be found in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting item: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
