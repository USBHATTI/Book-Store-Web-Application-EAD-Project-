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
    /// Interaction logic for AddItem.xaml
    /// </summary>
    public partial class AddItem : Window
    {
        
        public AddItem()
        {
            InitializeComponent();
        }

        private void CloseImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close(); // Close the application
        }

        private void MinimizeImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimize the application
        }

        // Event handler for Change Password button


        private void backImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            admin adminWindow = new admin(); // Create an instance of the Home window
            adminWindow.Show(); // Show the Home window
            this.Close(); // Optional: Close the current window
        }


        

        private void ADDButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation for required fields
            if (string.IsNullOrWhiteSpace(name_txt.Text) || string.IsNullOrWhiteSpace(des_txt.Text) || string.IsNullOrWhiteSpace(price_txt.Text))
            {
                MessageBox.Show("Please fill in all the fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if the quantity slider value is selected
            int quantity = (int)quatity_slider.Value;
            if (quantity == 0)
            {
                MessageBox.Show("Please select a valid quantity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check if the price is a valid decimal and convert it to int
            if (!decimal.TryParse(price_txt.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int priceAsInt = (int)price; // Convert decimal to int (truncate decimal part)

            // Validate if the image data is set
           

            // Create a new item object
            Item newItem = new Item
            {
                Name = name_txt.Text,
                Description = des_txt.Text,
                Quantity = quantity,
                Price = priceAsInt  // Store the price as an integer
            };

            try
            {
                using (var context = new EadContext())
                {
                    context.Items.Add(newItem); // Add the item to the context
                    context.SaveChanges(); // Save the changes to the database

                    MessageBox.Show("Item added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear the fields after successful addition
                    name_txt.Clear();
                    des_txt.Clear();
                    price_txt.Clear();
                    quatity_slider.Value = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
