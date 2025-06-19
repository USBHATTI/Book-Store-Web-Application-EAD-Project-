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
    /// Interaction logic for admin.xaml
    /// </summary>
    public partial class admin : Window
    {
        public admin()
        {
            InitializeComponent();
            LoadTotalBalance();
        }

        private void LoadTotalBalance()
        {
            try
            {
                using (var context = new EadContext())
                {
                    // Fetch all purchases (no userId filter)
                    var purchases = context.Purchases.ToList();

                    // Calculate the total balance by summing the prices of all purchases
                    decimal totalBalance = purchases.Sum(p => p.Price ?? 0);

                    // Update the TextBlock with the total balance (formatted as currency)
                    TotalBalanceTextBlock.Text = $"Total Balance: {totalBalance:C}"; // Shows total balance with currency formatting
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading total balance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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



        

        private void ViewSells_Click(object sender, RoutedEventArgs e)
        {
            AdminPurchase sell = new AdminPurchase(); // Create an instance of the Home window
            sell.Show(); // Show the Home window
            this.Close(); // Optional: Close the current window
        }

        private void UpdateBook_Click(object sender, RoutedEventArgs e)
        {
            update up = new update(); // Create an instance of the Home window
            up.Show(); // Show the Home window
            this.Close(); // Optional: Close the current window
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            AddItem add = new AddItem(); // Create an instance of the Home window
            add.Show(); // Show the Home window
            this.Close(); // Optional: Close the current window
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            deleteItem del = new deleteItem(); // Create an instance of the Home window
            del.Show(); // Show the Home window
            this.Close(); // Optional: Close the current window

        }
    }
}
