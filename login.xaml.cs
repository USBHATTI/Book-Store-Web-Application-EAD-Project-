using myEAD.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Window
    {
        public login()
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

        public class StringNullOrEmptyToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

        }

        private void Login_MouseDown(object sender, MouseButtonEventArgs e)
        {
            signup loginWindow = new signup(); // Create a new instance of the login window
            loginWindow.Show(); // Show the login window
            this.Close(); // Close the current main window (optional)
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            // Validate Password
            if (pass_txt.Password.Length < 8)
            {
                MessageBox.Show("Password must contain at least 8 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check for Admin credentials
            if (name_txt.Text == "ADMIN" && pass_txt.Password == "ADMIN098")
            {
                admin adminWindow = new admin(); // Open the Admin window
                adminWindow.Show();
                this.Close(); // Close the current window
            }
            else
            {
                try
                {
                    // Convert the entered password to bytes
                    byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(pass_txt.Password);

                    using (var context = new EadContext())
                    {
                        // Find the customer record by matching Name and Password (compare byte arrays)
                        var customer = context.CusDetails
                            .FirstOrDefault(c => c.Name == name_txt.Text && c.Password.SequenceEqual(enteredPasswordBytes));

                        // If a matching user is found
                        if (customer != null)
                        {
                            // Pass the ID of the matching customer to the Home window
                            home homeWindow = new home(customer.Id);
                            homeWindow.Show(); // Show the Home window
                            this.Close(); // Close the current window
                        }
                        else
                        {
                            // If no match is found
                            MessageBox.Show("Invalid username or password. Please try again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



    }
}
