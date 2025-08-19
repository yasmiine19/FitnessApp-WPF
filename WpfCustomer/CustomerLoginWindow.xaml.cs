using System;
using System.Text;
using System.Windows;
using CLFitness;

namespace WpfCustomer
{
    /// <summary>
    /// Interactielogica voor CustomerLoginWindow.xaml
    /// </summary>
    public partial class CustomerLoginWindow : Window
    {
        public CustomerLoginWindow()
        {
            InitializeComponent();
        }

        // de klik op de login-knop
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Haal de persoon op basis van de loginnaam
            Person person = Person.GetPersonByLogin(username); 

            // Controleer de gebruikersnaam en het wachtwoord
            if (person != null && person.Password == Person.HashPassword(password))
            {
                // als het geen admin is dan sluit de loginwindow en opent het de mainwindow
                if (!person.IsAdmin)
                {
                    Application.Current.Properties["CurrentUser"] = person;
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    // als het een admin is wordt er deze textblock getoond.
                    ErrorMessageTextBlock.Text = "Admins kunnen hier niet inloggen.";
                    ErrorMessageTextBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // als het geen admin is maar ook geen customer wordt deze textblock getoond.
                ErrorMessageTextBlock.Text = "Ongeldige gebruikersnaam of wachtwoord.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
