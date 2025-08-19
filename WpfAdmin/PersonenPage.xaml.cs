using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfAdmin
{
    /// <summary>
    /// Interactie logica voor PersonenPage.xaml
    /// </summary>
    public partial class PersonenPage : Page
    {
        public PersonenPage()
        {
            InitializeComponent();
            LoadPersons(); // Laad de lijst met personen bij het initialiseren
        }

        // Laad de lijst met personen in de ListBox
        private void LoadPersons()
        {
            PersonListBox.Items.Clear();
            List<Person> persons = Person.GetAllPersons();
            foreach (Person person in persons)
            {
                PersonListBox.Items.Add($"{person.Id}: {person.Firstname} {person.Lastname}");
            }
        }

        // Update de window wanneer een persoon wordt geselecteerd in de ListBox
        private void PersonListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PersonListBox.SelectedItem is string selectedItem)
            {
                // haal de personen op uit de database
                int selectedId = int.Parse(selectedItem.Split(':')[0]);
                List<Person> persons = Person.GetAllPersons();
                Person selectedPerson = persons.Find(p => p.Id == selectedId);

                if (selectedPerson != null)
                {
                    LoginTextBlock.Text = selectedPerson.Login;
                    RegDateTextBlock.Text = selectedPerson.RegDate.ToString("dd MMMM yyyy");
                    IsAdminTextBlock.Text = selectedPerson.IsAdmin ? "ja" : "nee";

                    if (selectedPerson.ProfilePhoto != null && selectedPerson.ProfilePhoto.Length > 0) // laad het profielfoto als die bestaat
                    {
                        BitmapImage bitmap = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(selectedPerson.ProfilePhoto))
                        {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                        }
                        ProfileImage.Source = bitmap;
                    }
                    else
                    {
                        ProfileImage.Source = null;
                    }
                }

                // Als een persoon op delete of bewerk druk zonder een persoon te drukken verschijnt deze foutmelding
                // Verberg de foutmelding indien een persoon geselecteed wordt 
                MessageTextBlock.Text = string.Empty;
            }
        }

        // Navigeren naar de bewerkpagina met de geselecteerde persoon
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (PersonListBox.SelectedItem is string selectedItem)
            {
                int selectedId = int.Parse(selectedItem.Split(':')[0]);
                List<Person> persons = Person.GetAllPersons();
                Person selectedPerson = persons.Find(p => p.Id == selectedId);

                if (selectedPerson != null) // Als een persoon geselecteerd is open het de bewerkpage
                {
                    BewerkPage bewerkPage = new BewerkPage(selectedPerson.Login);
                    this.NavigationService.Navigate(bewerkPage);
                }
            }
            else
            {
                // Is zichtbaar als een persoon niet wordt geselecteerd.
                MessageTextBlock.Text = "Selecteer een persoon om te bewerken.";
            }
        }

        // Navigeren naar de verwijderpagina met de geselecteerde persoon
        // zelfde werking als de bewerkpage het gaat gewoon de verwijderpage openen
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (PersonListBox.SelectedItem is string selectedItem)
            {
                int selectedId = int.Parse(selectedItem.Split(':')[0]);
                List<Person> persons = Person.GetAllPersons();
                Person selectedPerson = persons.Find(p => p.Id == selectedId);

                if (selectedPerson != null)
                {
                    VerwijderPage verwijderPage = new VerwijderPage(selectedPerson.Login);
                    this.NavigationService.Navigate(verwijderPage);
                }
            }
            else
            {
                // Is zichtbaar als een persoon niet wordt geselecteerd.
                MessageTextBlock.Text = "Selecteer een persoon om te verwijderen.";
            }
        }

        // Navigeren naar de nieuwe persoon aanmaakpagina
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            NieuwPage nieuwPage = new NieuwPage();
            this.NavigationService.Navigate(nieuwPage);
        }
    }
}
