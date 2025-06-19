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
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        private bool isPasswordVisible = false; // Flag to track visibility

        private int userId;

        // Constructor that accepts userId
        public Profile(int id)
        {
            InitializeComponent();
            userId = id;

            // Load user details from the database
            LoadUserProfile(userId);
            LoadTotalPurchases(userId);

        }
        private void LoadTotalPurchases(int userId)
        {
            try
            {
                using (var context = new EadContext())
                {
                    // Fetch all purchases for the specific customer (userId)
                    var purchases = context.Purchases.Where(p => p.CusId == userId).ToList();

                    // Calculate the total price from all purchases (using null-coalescing to avoid null values)
                    decimal totalAmount = purchases.Sum(p => p.Price ?? 0);

                    // Update the priceTextBlock with the total amount (formatted as currency)
                    priceTextBlock.Text = totalAmount.ToString("C");  // Formats the number as a currency (e.g., $30.00)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading total purchase amount: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void LoadUserProfile(int id)
        {
            try
            {
                using (var context = new EadContext())
                {
                    // Get the user details from CusDetail table
                    var user = context.CusDetails.FirstOrDefault(u => u.Id == id);

                    if (user != null)
                    {
                        // Bind the data to UI elements
                        nameTextBlock.Text = user.Name; // Display name
                        EmailTextBox.Text = Encoding.UTF8.GetString(user.Email); // Convert Email from byte[] to string
                        PasswordBox.Password = Encoding.UTF8.GetString(user.Password); // Password

                        // Assuming Price is stored in the same table or another related table, if available
                        // For example, if there's a Price property in CusDetail, use that
                        //priceTextBlock.Text = user.Price.ToString(); // Replace with actual field if it exists
                    }
                    else
                    {
                        MessageBox.Show("User not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler to toggle password visibility
        private void EyeButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                // Show TextBox with password text
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                // Show PasswordBox with password dots
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
            }
        }


        // Event handler for Change Password button
        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            forgetPass loginWindow = new forgetPass(userId); // Pass userId to the constructor
            loginWindow.Show(); // Show the login window
            this.Close(); // Close the current main window (optional)
        }

        // Minimize button functionality
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
            home homeWindow = new home(userId);
            homeWindow.Show(); // Show the Home window
            this.Close(); // Close the current window
        }
    }
}
