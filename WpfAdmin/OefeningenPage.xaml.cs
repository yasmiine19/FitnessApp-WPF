using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for OefeningenPage.xaml
    /// </summary>
    public partial class OefeningenPage : Page
    {
        private Dictionary<int, Border> exerciseCards = new Dictionary<int, Border>();

        // Constructor die de pagina initialiseert en de oefeningen laadt
        public OefeningenPage()
        {
            InitializeComponent();
            LoadExercises(); // Laadt alle oefeningen bij het openen van de pagina
        }

        // Methode om alle oefeningen op te halen en te tonen
        private void LoadExercises()
        {
            // Maak de panelen leeg voordat nieuwe items worden toegevoegd
            CardioPanel.Children.Clear();
            DumbellPanel.Children.Clear();
            YogaPanel.Children.Clear();

            // Haal alle oefeningen op via de class library exercise
            List<Exercise> exercises = Exercise.GetAllExercises();

            // Verdeelt de oefeningen over de juiste panelen op basis van hun type
            for (int i = 0; i < exercises.Count; i++)
            {
                Exercise exercise = exercises[i];
                Border exerciseCard = CreateExerciseCard(exercise);

                // Voeg de oefening toe aan het juiste onderdeel
                if (exercise.Type == ExerciseType.Cardio)
                {
                    CardioPanel.Children.Add(exerciseCard);
                }
                else if (exercise.Type == ExerciseType.Dumbell)
                {
                    DumbellPanel.Children.Add(exerciseCard);
                }
                else if (exercise.Type == ExerciseType.Yoga)
                {
                    YogaPanel.Children.Add(exerciseCard);
                }

                // Voeg de kaart toe aan de dictionary
                exerciseCards[exercise.Id] = exerciseCard;
            }
        }

        // Methode om een kaart voor een oefening te maken
        private Border CreateExerciseCard(Exercise exercise)
        {
            // Maak een kaart met rand en padding
            Border card = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Width = 200,
            };

            // Maak een grid layout voor de kaart
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Voeg een afbeelding toe aan de kaart
            Image image = new Image
            {
                Width = 90,
                Height = 90,
                Margin = new Thickness(5)
            };

            // Controleer of de oefening een foto heeft en voeg deze toe aan de afbeelding
            if (exercise.Photo != null)
            {
                using (MemoryStream stream = new MemoryStream(exercise.Photo))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    image.Source = bitmap;
                }
            }

            // Stel de positie van de afbeelding in de grid in
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);

            // Maak een paneel voor de knoppen
            StackPanel buttonPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5)
            };

            // Voeg een details-knop toe
            Button detailsButton = new Button
            {
                Content = "📋",
                Width = 30,
                Height = 30,
                Margin = new Thickness(5),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };

            detailsButton.Click += (sender, e) => NavigationService.Navigate(new OefeningenDetailsPage(exercise));

            // Voeg een bewerk-knop toe
            Button editButton = new Button
            {
                Content = "✏",
                Width = 30,
                Height = 30,
                Margin = new Thickness(5),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };
            editButton.Click += (sender, e) => NavigationService.Navigate(new OefeningenBewerkPage(exercise));

            // Voeg een verwijder-knop toe
            Button deleteButton = new Button
            {
                Content = "🗑",
                Width = 30,
                Height = 30,
                Margin = new Thickness(5),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };
            deleteButton.Click += (sender, e) => NavigationService.Navigate(new OefeningenVerwijderPage(exercise, this));

            // Voeg de knoppen toe aan het knoppenpaneel
            buttonPanel.Children.Add(detailsButton);
            buttonPanel.Children.Add(editButton);
            buttonPanel.Children.Add(deleteButton);

            // Stel de positie van het knoppenpaneel in de grid in
            Grid.SetRow(buttonPanel, 0);
            Grid.SetColumn(buttonPanel, 1);

            // Maak een tekstpaneel voor de beschrijving van de oefening
            StackPanel textPanel = new StackPanel();

            // Voeg de naam van de oefening toe
            TextBlock nameTextBlock = new TextBlock
            {
                Text = exercise.Name,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            // Voeg de punten van de oefening toe
            TextBlock pointsTextBlock = new TextBlock
            {
                Text = exercise.Points + " points",
                Margin = new Thickness(5)
            };

            // Voeg de beschrijving van de oefening toe
            TextBlock descriptionTextBlock = new TextBlock
            {
                Text = exercise.Description,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5)
            };

            // Voeg de tekstblokken toe aan het tekstpaneel
            textPanel.Children.Add(nameTextBlock);
            textPanel.Children.Add(pointsTextBlock);
            textPanel.Children.Add(descriptionTextBlock);

            // Stel de positie van het tekstpaneel in de grid in
            Grid.SetRow(textPanel, 1);
            Grid.SetColumn(textPanel, 0);
            Grid.SetColumnSpan(textPanel, 2);

            // Voeg alle onderdelen toe aan de grid
            grid.Children.Add(image);
            grid.Children.Add(buttonPanel);
            grid.Children.Add(textPanel);

            // Stel de grid in als de inhoud van de kaart
            card.Child = grid;

            return card;
        }

        // Methode om een oefeningkaart te verwijderen
        public void RemoveExerciseCard(Exercise exercise)
        {
            if (exerciseCards.ContainsKey(exercise.Id))
            {
                Border card = exerciseCards[exercise.Id];

                if (exercise.Type == ExerciseType.Cardio)
                {
                    CardioPanel.Children.Remove(card);
                }
                else if (exercise.Type == ExerciseType.Dumbell)
                {
                    DumbellPanel.Children.Remove(card);
                }
                else if (exercise.Type == ExerciseType.Yoga)
                {
                    YogaPanel.Children.Remove(card);
                }

                exerciseCards.Remove(exercise.Id);
            }
        }
    }
}