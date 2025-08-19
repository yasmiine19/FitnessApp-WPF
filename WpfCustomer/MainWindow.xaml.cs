using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfCustomer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Laad de ingelogde gebruiker
            Person currentUser = Application.Current.Properties["CurrentUser"] as Person;
            if (currentUser != null)
            {
                // Voeg de naam van de ingelogde gebruiker toe aan de titel
                this.Title = $"WPF Customer - Welkom {currentUser.Firstname} {currentUser.Lastname}";

                // Laad de profielfoto van de ingelogde customer
                if (currentUser.ProfilePhoto != null && currentUser.ProfilePhoto.Length > 0)
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(currentUser.ProfilePhoto))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                    ImgFoto.Source = bitmap;
                }
            }
        }

        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            // Open LoginWindow en sluit huidige MainWindow
            CustomerLoginWindow loginWindow = new CustomerLoginWindow();
            loginWindow.Show();

            // Sluit alle andere vensters
            for (int i = 0; i < Application.Current.Windows.Count; i++)
            {
                Window window = Application.Current.Windows[i];
                if (window != loginWindow)
                {
                    window.Close();
                }
            }
        }

        private void StatistiekenButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StatistiekenPage());
        }

        private void WorkoutsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WorkoutsPage());
        }
    }
}