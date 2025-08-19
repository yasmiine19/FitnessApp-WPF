using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;
using Microsoft.Win32;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction PersonenPage : Page
    ///  </summary>
    ///  
    public partial class BewerkPage : Page
    {
        private byte[] profilePhoto; // opdslag voor de profielfoto
        private Person person; // opslag voor de huidige persoon
        private string login;

        // Constructoren
        // ontvangt de login en login en laadt de persoon
        public BewerkPage(string login)
        {
            InitializeComponent();
            LoadPerson(login);
            this.login = login;
        }

        // Implementatie om een persoon op te halen op basis van hun login
        private void LoadPerson(string login)
        {
            person = Person.GetPersonByLogin(login);
            if (person != null)
            {
                LoadPersonDetails();
            }
            else
            {
                ShowError("Persoon niet gevonden.");
            }
        }

        // Implementatie om de details van een persoon in de velden te laden
        private void LoadPersonDetails()
        {
            if (person == null) return;

            // Vul de tekstblokken met de gegevens van de persoon
            FirstNameTextBox.Text = person.Firstname;
            LastNameTextBox.Text = person.Lastname;
            LoginTextBox.Text = person.Login;
            PasswordBox.Password = string.Empty; // Laat de PasswordBox leeg
            IsAdminCheckBox.IsChecked = person.IsAdmin;

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

        // Implementatie om wijzigingen van de persoon op te slaan
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (person == null)
            {
                ShowError("Persoon is niet geladen.");
                return;
            }

            // Controleer of alle verplichte velden zijn ingevuld
            if (string.IsNullOrEmpty(FirstNameTextBox.Text) ||
                string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(LoginTextBox.Text))
            {
                ShowError("Vul alle verplichte velden in.");
                return;
            }

            // update de gegevens van de persoon
            person.Firstname = FirstNameTextBox.Text;
            person.Lastname = LastNameTextBox.Text;
            person.Login = LoginTextBox.Text;

            // Hier wordt het wachtwoord alleen gehasht als het  gewijzigd wordt
            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                person.Password = Person.HashPassword(PasswordBox.Password);
            }

            person.IsAdmin = IsAdminCheckBox.IsChecked ?? false;
            person.ProfilePhoto = profilePhoto;

            try
            {
                // slaat de wijzigingen in de databank
                Person.UpdatePerson(person);
                this.NavigationService.Navigate(new PersonenPage());
            }
            catch (Exception ex)
            {
                ShowError($"Er is een fout opgetreden: {ex.Message}");
            }
        }
        
        // weergeeft een foutmelding
        private void ShowError(string message)
        {
            ErrorMessageTextBlock.Text = message;
            ErrorMessageTextBlock.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        // profielfoto kiezen en opslaan
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
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(stream);
                    profilePhoto = stream.ToArray();
                }
            }
        }
    }
}