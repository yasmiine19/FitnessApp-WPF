using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfAdmin
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
            if (Application.Current.Properties["CurrentUser"] is Person currentUser)
            {
                // Voeg de naam van de ingelogde gebruiker toe aan de titel
                this.Title = $"WPF Admin - welkom {currentUser.Firstname} {currentUser.Lastname}";

                // Implementatie voor het laten verschijnen van de profielfoto van de admin
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

        // Implementatie zodanig dat eens op de uitlogbutton gedrukt men terugkeert naar de loginWindow
        private void BtnUitloggen_Click(object sender, RoutedEventArgs e)
        {
            // Open LoginWindow en sluit huidige MainWindow
            LoginWindow loginWindow = new LoginWindow();
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

        // Navigeer naar de personenpagina
        private void PersonsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PersonenPage());
        }

        // Navigeer naar de oefeningenpagina
        private void BtnOefeningen_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OefeningenPage());
        }
    }
}