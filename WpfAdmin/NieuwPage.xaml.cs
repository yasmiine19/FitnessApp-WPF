using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;
using Microsoft.Win32;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for NieuwPage.xaml
    /// </summary>
    public partial class NieuwPage : Page
    {
        private byte[] profilePhoto;  // Profielfoto

        public NieuwPage()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Controleer of alle velden en de afbeelding zijn ingevuld
            if (string.IsNullOrEmpty(FirstNameTextBox.Text) ||
                string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(LoginTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password) ||
                profilePhoto == null)
            {
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Verberg foutmelding als alle velden zijn ingevuld
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;

            // Haal het aantal personen op en stel de ID in voor de nieuwe persoon
            int newId = Person.GetPersonCount() + 1;

            // Logica voor het toevoegen van een nieuwe persoon
            string firstName = FirstNameTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string login = LoginTextBox.Text;
            string password = Person.HashPassword(PasswordBox.Password); // gebruik hashpassword van person
            bool isAdmin = IsAdminCheckBox.IsChecked == true;

            // Voeg de nieuwe persoon toe aan de database en lijst
            Person newPerson = new Person
            {
                Id = newId,  // Stel de nieuwe ID in
                Firstname = firstName,
                Lastname = lastName,
                Login = login,
                Password = password,
                ProfilePhoto = profilePhoto,
                RegDate = DateTime.Now,
                IsAdmin = isAdmin
            };

            Person.AddPerson(newPerson); // een methode om de persoon toe te voegen

            this.NavigationService.Navigate(new PersonenPage());
        }

        // Annuleren knop
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ChoosePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                ProfilePhotoImage.Source = bitmap;

                using (MemoryStream stream = new MemoryStream())
                {
                    BitmapEncoder encoder = null;
                    if (openFileDialog.FileName.EndsWith(".jpg") || openFileDialog.FileName.EndsWith(".jpeg"))
                    {
                        encoder = new JpegBitmapEncoder();
                    }
                    else if (openFileDialog.FileName.EndsWith(".png"))
                    {
                        encoder = new PngBitmapEncoder();
                    }

                    if (encoder != null)
                    {
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        encoder.Save(stream);
                        profilePhoto = stream.ToArray();
                    }
                }
            }
        }
    }
}
