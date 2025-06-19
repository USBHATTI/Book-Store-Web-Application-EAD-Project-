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
    /// Interaction logic for AdminPurchase.xaml
    /// </summary>
    public partial class AdminPurchase : Window
    {
        public AdminPurchase()
        {
            InitializeComponent();
            LoadMenuItems();
        }


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
                    // Fetch all purchases from the database
                    var purchases = context.Purchases.ToList();

                    // Filter purchases if a search text is provided
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        searchText = searchText.ToLower();
                        purchases = purchases.Where(p =>
                        {
                            // Get the associated item based on ItId
                            var item = context.Items.FirstOrDefault(i => i.ItId == p.ItId);
                            // Match item name with search text
                            return item != null && item.Name != null && item.Name.ToLower().Contains(searchText);
                        }).ToList();
                    }

                    // Clear any existing items
                    MenuItemsPanel.Children.Clear();

                    // Add each purchase to the MenuItemsPanel
                    foreach (var purchase in purchases)
                    {
                        // Create a button for the purchase item
                        Button purchaseButton = new Button
                        {
                            Width = 600,
                            Height = 65,
                            Margin = new Thickness(30, 10, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            Style = (Style)FindResource("CustomButtonStyle"), // Apply the reusable style
                            Content = CreateButtonContent(purchase),
                            Tag = purchase // Attach purchase for further use
                        };

                        // Add the button to the panel
                        MenuItemsPanel.Children.Add(purchaseButton);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading items: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private Grid CreateButtonContent(Purchase purchase)
        {
            // Create a Grid to act as the main layout
            Grid grid = new Grid();

            // Fetch the corresponding Item based on purchase.ItId
            string itemName = "Unknown Name";
            using (var context = new EadContext())
            {
                var item = context.Items.FirstOrDefault(i => i.ItId == purchase.ItId);
                if (item != null)
                {
                    itemName = item.Name ?? "Unknown Name";
                }
            }

            // Add one column to hold the flexible content
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Left section for Name, Description, and Quantity
            StackPanel leftSection = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add Purchase Name (use purchase.CusName or item name if available)
            leftSection.Children.Add(new TextBlock
            {
                Text = $"{itemName} purchased by {purchase.CusName ?? "Unknown Customer"} (ID: {purchase.CusId ?? 0})",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(30, 0, 10, 0),
                FontFamily = new FontFamily("Comic Sans MS")
                //Text = purchase.CusName ?? "Unknown Name",  // Assuming CusName is available in Purchase
                //FontSize = 16,
                //FontWeight = FontWeights.Bold,
                //Margin = new Thickness(30, 0, 10, 0),
                //FontFamily = new FontFamily("Comic Sans MS")
            });

            // Add Purchase Description
            leftSection.Children.Add(new TextBlock
            {
                Text = purchase.Description ?? "No Description",
                FontSize = 14,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Black,
                TextWrapping = TextWrapping.Wrap, // Enable wrapping for long descriptions
                Margin = new Thickness(10, 0, 10, 0),
            });

            // Add Purchase Quantity
            leftSection.Children.Add(new TextBlock
            {
                Text = $"Quantity: {purchase.Quatity ?? 0}",
                FontSize = 14,
                FontStyle = FontStyles.Italic,
                Foreground = Brushes.Black,
                Margin = new Thickness(10, 0, 10, 0),
            });

            // Add the left section to the grid
            Grid.SetColumn(leftSection, 0);
            grid.Children.Add(leftSection);

            

            // Absolute positioning for the price
            TextBlock priceText = new TextBlock
            {
                Text = $"Price: ${purchase.Price ?? 0}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(500, 0, 10, 0) // Add spacing from the right border
            };

            // Use Canvas.ZIndex to layer the price above everything else
            priceText.SetValue(Grid.ColumnProperty, 0); // Add to the same grid column
            priceText.SetValue(Panel.ZIndexProperty, 1);

            // Add the price to the grid, positioned over other content
            grid.Children.Add(priceText);

            return grid;
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
    }
}
