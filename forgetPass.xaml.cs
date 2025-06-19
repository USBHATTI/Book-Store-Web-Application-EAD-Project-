using myEAD.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
    /// Interaction logic for forgetPass.xaml
    /// </summary>
    public partial class forgetPass : Window
    {
        private int userId;

        // Constructor that accepts userId
        public forgetPass(int id)
        {
            InitializeComponent();
            userId = id; // Store the userId in a class-level variable
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

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            string recipientEmail = pb_changePass.Text; // Email entered by user

            // Validate the email input
            if (string.IsNullOrWhiteSpace(recipientEmail) || !IsValidEmail(recipientEmail))
            {
                MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Step 1: Generate a random 8-digit password
                string randomPassword = GenerateRandomPassword(8);

                // Step 2: Save the password in the database (as byte[] using UTF8 encoding)
                SavePasswordToDatabase(randomPassword);

                // Step 3: Send email with the exact password
                SendPasswordEmail(recipientEmail, randomPassword);

                MessageBox.Show("Password change successful and email sent!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to change password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateRandomPassword(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }

            return new string(password);
        }

        private void SavePasswordToDatabase(string password)
        {
            // Step 2: Save the password in the database (as byte[] using UTF8 encoding)
            using (var context = new EadContext())
            {
                var user = context.CusDetails.FirstOrDefault(u => u.Id == userId); // Assuming userId is available globally

                if (user != null)
                {
                    user.Password = Encoding.UTF8.GetBytes(password); // Store the password as byte[]
                    context.SaveChanges(); // Save changes to the database
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SendPasswordEmail(string recipientEmail, string password)
        {
            try
            {
                string fromEmail = "v3242696@gmail.com"; // Sender's email address
                string emailPassword = "fplw hdis ouqn qmlt"; // Email password (use App Password for Gmail)

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(fromEmail, emailPassword),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Password Change Request",
                    Body = $"Your new password is: {password}",
                    IsBodyHtml = false
                };

                mail.To.Add(recipientEmail);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send email: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Email Validation Method
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
