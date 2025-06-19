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
    /// Interaction logic for signup.xaml
    /// </summary>
    public partial class signup : Window
    {
        public signup()
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
            login loginWindow = new login(); // Create a new instance of the login window
            loginWindow.Show(); // Show the login window
            this.Close(); // Close the current main window (optional)
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            // Validation for Name, Email, and Password fields
            if (string.IsNullOrWhiteSpace(name_txt.Text) || string.IsNullOrWhiteSpace(email_txt.Text) || string.IsNullOrWhiteSpace(pass_txt.Password))
            {
                MessageBox.Show("All fields are required. Please fill them.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate Email
            if (!email_txt.Text.EndsWith("@gmail.com"))
            {
                MessageBox.Show("Email must end with '@gmail.com'.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate Password
            if (pass_txt.Password.Length < 8)
            {
                MessageBox.Show("Password must contain at least 8 characters.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Validate Password Match
            if (pass_txt.Password != pass1_txt.Password)
            {
                MessageBox.Show("Passwords do not match. Please make sure both passwords are identical.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Save Data to Database (without hashing)
                using (var context = new EadContext())
                {
                    CusDetail newCustomer = new CusDetail
                    {
                        Name = name_txt.Text,  // Save the Name as entered
                        Password = Encoding.UTF8.GetBytes(pass_txt.Password),  // Save the Password as entered (not hashed)
                        Email = Encoding.UTF8.GetBytes(email_txt.Text)  // Save the Email as entered (not hashed)
                    };

                    context.CusDetails.Add(newCustomer);
                    context.SaveChanges();

                    // Get the ID of the newly inserted record
                    int newCustomerId = newCustomer.Id;

                    // Pass the ID to the Home window
                    home homeWindow = new home(newCustomerId); // Pass the ID
                    homeWindow.Show(); // Show the Home window

                    // Show a success message with the entered name
                    MessageBox.Show($"Signup successful, welcome to Frosted Dreams, {newCustomer.Name}!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close(); // Close the current window
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
