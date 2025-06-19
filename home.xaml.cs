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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace myEAD
{
    /// <summary>
    /// Interaction logic for home.xaml
    /// </summary>
    public partial class home : Window
    {
        public home()
        {
            InitializeComponent();
            LoadMenuItems();
        }

        private int userId;

        public home(int id)
        {
            InitializeComponent();
            userId = id;
            LoadMenuItems();
        }

        //private void LoadMenuItems()
        //{
        //    try
        //    {
        //        using (var context = new EadContext())
        //        {
        //            // Fetch items from the database
        //            var items = context.Items.ToList();

        //            // Clear any existing items
        //            MenuItemsPanel.Children.Clear();

        //            // Add each item to the MenuItemsPanel
        //            foreach (var item in items)
        //            {
        //                // Create a button for the item
        //                Button itemButton = new Button
        //                {
        //                    Width = 600,
        //                    Height = 65,
        //                    Margin = new Thickness(30, 10, 0, 0),
        //                    HorizontalAlignment = HorizontalAlignment.Left,
        //                    VerticalAlignment = VerticalAlignment.Top,
        //                    Style = (Style)FindResource("CustomButtonStyle"), // Apply the reusable style
        //                    Content = CreateButtonContent(item),
        //                    Tag = item // Attach item for further use
        //                };

        //                // Optional: Add click event handler
        //                itemButton.Click += ItemButton_Click;

        //                // Add to the panel
        //                MenuItemsPanel.Children.Add(itemButton);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error loading items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PlaceholderText.Visibility = Visibility.Collapsed; // Hide placeholder on focus
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                PlaceholderText.Visibility = Visibility.Visible; // Show placeholder if no input
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim();

            // Call the LoadMenuItems method with the search text
            LoadMenuItems(searchText);
        }



        private void LoadMenuItems(string searchText = "")
        {
            try
            {
                using (var context = new EadContext())
                {
                    // Fetch all items from the database
                    var items = context.Items.ToList();

                    // Perform case-insensitive filtering in memory
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        searchText = searchText.ToLower(); // Convert search text to lowercase
                        items = items.Where(item => item.Name != null && item.Name.ToLower().Contains(searchText)).ToList();
                    }

                    // Clear any existing items
                    MenuItemsPanel.Children.Clear();

                    // Add each matching item to the MenuItemsPanel
                    foreach (var item in items)
                    {
                        // Create a button for the item
                        Button itemButton = new Button
                        {
                            Width = 600,
                            Height = 65,
                            Margin = new Thickness(30, 10, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Style = (Style)FindResource("CustomButtonStyle"), // Apply the reusable style
                            Content = CreateButtonContent(item),
                            Tag = item // Attach item for further use
                        };

                        // Optional: Add click event handler
                        itemButton.Click += ItemButton_Click;

                        // Add to the panel
                        MenuItemsPanel.Children.Add(itemButton);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private Grid CreateButtonContent(Item item)
        {
            // Create a Grid to act as the main layout
            Grid grid = new Grid();

            // Add one column to hold the flexible content
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Left section for Name, Description, and Quantity
            StackPanel leftSection = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            leftSection.Children.Add(new TextBlock
            {
                Text = item.Name ?? "Unknown Name",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(30, 0, 10, 0),
                FontFamily = new FontFamily("Comic Sans MS")
            });

            leftSection.Children.Add(new TextBlock
            {
                Text = item.Description ?? "No Description",
                FontSize = 14,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap, // Enable wrapping for long descriptions
                Margin = new Thickness(10, 0, 10, 0),
                //FontFamily = new FontFamily("Comic Sans MS")
            });

            leftSection.Children.Add(new TextBlock
            {
                Text = $"Quantity: {item.Quantity ?? 0}",
                FontSize = 14,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Black,
                Margin = new Thickness(10, 0, 10, 0),
                //FontFamily = new FontFamily("Comic Sans MS") // Add the font family here
            });

            // Add the left section to the grid
            Grid.SetColumn(leftSection, 0);
            grid.Children.Add(leftSection);

            // Absolute positioning for the price
            TextBlock priceText = new TextBlock
            {
                Text = $"Price: ${item.Price ?? 0}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                //FontFamily = new FontFamily("Comic Sans MS"),
                Margin = new Thickness(500, 0, 10, 0) // Add spacing from the right border
            };

            // Use Canvas.ZIndex to layer the price above everything else
            priceText.SetValue(Grid.ColumnProperty, 0); // Add to the same grid column
            priceText.SetValue(Panel.ZIndexProperty, 1);

            // Add the price to the grid, positioned over other content
            grid.Children.Add(priceText);

            return grid;
        }



        private void ItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Item item)
            {

                MessageBox.Show($"Sending values:\nUserId: {userId}\nItemId: {item.ItId}",
                        "Debug Info", MessageBoxButton.OK, MessageBoxImage.Information);
                // Pass the userId and item.ItId (itemId) to the purchase window constructor
                purchase profileWindow = new purchase(userId, item.ItId);

                // Show the Profile window
                profileWindow.Show();

                // Optionally, close the current window
                this.Close();  // Close the current window (if desired)

                
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

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            
            Profile profileWindow = new Profile(userId);  // Assuming userId is a global or previously defined variable
            profileWindow.Show();  // Show the Profile window
            this.Close();  // Close the current window (if desired)
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            myPur profileWindow = new myPur(userId);  // Assuming userId is a global or previously defined variable
            profileWindow.Show();  // Show the Profile window
            this.Close();  // Close the current window (if desired)

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Show confirmation dialog
            var result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            // If the user clicked Yes
            if (result == MessageBoxResult.Yes)
            {
                login lo1 = new login();
                lo1.Show();
                this.Close();
            }
            // If the user clicked No, just stay on the current page
        }
    }
}
