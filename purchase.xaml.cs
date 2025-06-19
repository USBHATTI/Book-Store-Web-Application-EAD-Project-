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
    /// Interaction logic for purchase.xaml
    /// </summary>
    public partial class purchase : Window
    {
        // Declare properties to store passed data
        private int customerId;
        private int itemId;

        // Constructor that receives customer ID and item ID
        public purchase(int cusId, int itId)
        {
            InitializeComponent();

            // Store the passed customer ID and item ID
            customerId = cusId;
            itemId = itId;

            // Initialize the form with item details
            InitializeItemDetails();
        }

        // Method to initialize the item details (name, description, price)
        private void InitializeItemDetails()
        {
            using (var context = new EadContext())
            {
                // Fetch the item details from the database based on item ID
                var selectedItem = context.Items.FirstOrDefault(item => item.ItId == itemId);

                if (selectedItem != null)
                {
                    // Fill the text boxes with item details
                    name_txt.Text = selectedItem.Name;
                    des_txt.Text = selectedItem.Description;
                    price_txt.Text = selectedItem.Price.ToString();

                    // Set the maximum value of the quantity slider to the available quantity
                    quatity_slider.Maximum = selectedItem.Quantity ?? 0;
                    quatity_slider.Minimum = 1;
                    quatity_slider.Value = 1;
                }
            }
        }

        private void CloseImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close(); // Close the application
        }

        private void MinimizeImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimize the application
        }

        // Event handler for the back button
        private void backImage_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            home adminWindow = new home(customerId); // Create an instance of the Home window
            adminWindow.Show(); // Show the Home window
            this.Close(); // Optional: Close the current window
        }

        // Event handler for Buy button click
        private void buyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new EadContext())
                {
                    // Fetch the item based on item ID
                    var selectedItem = context.Items.FirstOrDefault(item => item.ItId == itemId);

                    if (selectedItem == null)
                    {
                        MessageBox.Show("Item not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Get the quantity from the slider
                    int quantityToPurchase = (int)quatity_slider.Value;

                    if (quantityToPurchase > (selectedItem.Quantity ?? 0))
                    {
                        MessageBox.Show("Not enough quantity available.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Update item quantity
                    selectedItem.Quantity -= quantityToPurchase;

                    // Add purchase details to the History table
                    Purchase purchaseHistory = new Purchase
                    {
                        CusId = customerId,
                        ItId = selectedItem.ItId,
                        CusName = context.CusDetails.FirstOrDefault(c => c.Id == customerId)?.Name,
                        Quatity = quantityToPurchase,
                        Price = selectedItem.Price * quantityToPurchase,
                        Description = selectedItem.Description
                    };

                    context.Purchases.Add(purchaseHistory);

                    // Save changes to the database
                    context.SaveChanges();

                    MessageBox.Show("Purchase successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Update quantity slider max value
                    quatity_slider.Maximum = selectedItem.Quantity ?? 0;

                    // Optionally clear fields
                    name_txt.Text = string.Empty;
                    des_txt.Text = string.Empty;
                    price_txt.Text = string.Empty;
                    quatity_slider.Value = 1;

                    home n = new home(customerId);
                    n.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
