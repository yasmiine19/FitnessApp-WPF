using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for VerwijderPage
    /// </summary>
    public partial class VerwijderPage : Page
    {
        private byte[] profilePhoto;  
        private Person person;

        public VerwijderPage(string login)
        {
            InitializeComponent();
            LoadPerson(login);  // Laad de gegevens van de persoon
        }

        // Laad de gegevens van de persoon op basis van de login
        private void LoadPerson(string login)
        {
            person = Person.GetPersonByLogin(login);
            if (person != null)
            {
                LoadPersonDetails();
            }
            else
            {
                ShowError("Persoon niet gevonden."); // Toon een foutmelding als de persoon niet gevonden is (versterking om crash te voorkomen)
            }
        }

        // Laad de gegevens van de persoon in de tekstblokken
        private void LoadPersonDetails()
        {
            if (person == null) return;

            FirstNameTextBox.Text = person.Firstname;
            LastNameTextBox.Text = person.Lastname;
            LoginTextBox.Text = person.Login;
            PasswordBox.Password = string.Empty; // Laat de PasswordBox leeg
            IsAdminCheckBox.IsChecked = person.IsAdmin;
            IsAdminCheckBox.IsEnabled = false; // Zorg ervoor dat de admin checkbox niet aanpasbaar is

            if (person.ProfilePhoto != null && person.ProfilePhoto.Length > 0)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(person.ProfilePhoto))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                ProfilePhotoImage.Source = bitmap;
                profilePhoto = person.ProfilePhoto;
            }
        }

        // Verwijder de persoon wanneer op de verwijderknop wordt geklikt
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Verwijderen van een persoon
            string login = LoginTextBox.Text;

            Person personToDelete = Person.GetPersonByLogin(login);
            if (personToDelete != null)
            {
                Person.DeletePerson(personToDelete.Id); // Verwijder de persoon
            }

            this.NavigationService.Navigate(new PersonenPage());
        }

        // De Annuleren knop
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ShowError(string message)
        {
            Console.WriteLine(message);
        }
    }
}
