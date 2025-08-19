using System;
using System.Text;
using System.Windows;
using CLFitness;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Implementatie van de loginlogica
        // Als een persoon wilt inloggen gaat het na of het een admin is of niet 
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Haal de persoon op basis van de loginnaam
            Person person = Person.GetPersonByLogin(username);

            // Controleert of de persoon bestaat en dat het wachtwoord klopt
            if (person != null && person.Password == Person.HashPassword(password))
            {
                if (person.IsAdmin) // als het een admin is opent de mainwindow zich en sluit de loginwindow
                {
                    // Sla de ingelogde gebruiker op en open het Mainwindow
                    Application.Current.Properties["CurrentUser"] = person;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    // Toon foutmelding als de gebruiker geen admin is
                    // Als het ziet dat het customer is geeft die deze message textblock weer
                    ErrorMessageTextBlock.Text = "Alleen admins kunnen inloggen.";
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Toon foutmelding bij ongeldige login
                // als het geen admin en customer is geeft het deze textbloxk weer
                ErrorMessageTextBlock.Text = "Ongeldige gebruikersnaam of wachtwoord.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
