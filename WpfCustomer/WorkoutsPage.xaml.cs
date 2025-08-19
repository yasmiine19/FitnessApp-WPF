using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfCustomer
{
    /// <summary>
    /// Interactielogica voor WorkoutsPage.xaml
    /// </summary>
    public partial class WorkoutsPage : Page
    {
        public WorkoutsPage()
        {
            InitializeComponent();
        }

        // Wordt aangeroepen wanneer de geselecteerde datum in de kalender verandert
        private void WorkoutCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WorkoutCalendar.SelectedDate.HasValue)
            {
                // Verberg de foutmelding
                MessageTextBlock.Visibility = Visibility.Collapsed;

                DateTime selectedDate = WorkoutCalendar.SelectedDate.Value;
                LoadWorkouts(selectedDate);
            }
        }

        // Laadt de workouts van een specifieke dag als die dag geselecteerd is
        public void LoadWorkouts(DateTime date)
        {
            WorkoutsPanel.Children.Clear();

            int customerId = (Application.Current.Properties["CurrentUser"] as Person).Id;
            List<Workout> workouts = Workout.GetWorkoutsByDate(date, customerId);
            WorkoutsCountTextBlock.Text = $"{workouts.Count} workouts gevonden op {date:dd MMMM yyyy}";

            // voeg elke workout toe 
            foreach (Workout workout in workouts)
            {
                Border workoutCard = CreateWorkoutCard(workout);
                WorkoutsPanel.Children.Add(workoutCard);
            }
        }

        // dit gedeelte zorgt voor het aanamaken van een kaart voor een workout
        private Border CreateWorkoutCard(Workout workout)
        {
            Border card = new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Width = 300
            };

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Image image = new Image
            {
                Width = 90,
                Height = 90,
                Margin = new Thickness(5)
            };

            // Laad de afbeelding van de oefening 
            if (workout.Exercise.Photo != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream(workout.Exercise.Photo))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                image.Source = bitmap;
            }

            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            Grid.SetRowSpan(image, 2);

            StackPanel textPanel = new StackPanel();

            TextBlock nameTextBlock = new TextBlock
            {
                Text = workout.Exercise.Name,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            TextBlock distanceTextBlock = new TextBlock
            {
                Text = $"Afstand: {workout.Distance} km",
                Margin = new Thickness(5)
            };

            TextBlock pointsTextBlock = new TextBlock
            {
                Text = $"Punten: {workout.Exercise.Points}",
                Margin = new Thickness(5)
            };

            textPanel.Children.Add(nameTextBlock);
            textPanel.Children.Add(distanceTextBlock);
            textPanel.Children.Add(pointsTextBlock);

            Grid.SetRow(textPanel, 0);
            Grid.SetColumn(textPanel, 1);
            Grid.SetColumnSpan(textPanel, 2);

            Button deleteButton = new Button
            {
                Content = "🗑",
                Width = 30,
                Height = 30,
                Margin = new Thickness(5),
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent
            };

            deleteButton.Click += (sender, e) => NavigateToDeletePage(workout);

            Grid.SetRow(deleteButton, 0);
            Grid.SetColumn(deleteButton, 2);
            Grid.SetRowSpan(deleteButton, 2);

            grid.Children.Add(image);
            grid.Children.Add(textPanel);
            grid.Children.Add(deleteButton);

            card.Child = grid;

            return card;
        }

        // Navigeer naar de verwijderpagina om de workout te verwijderen
        private void NavigateToDeletePage(Workout workout)
        {
            WorkoutsVerwijderPage deletePage = new WorkoutsVerwijderPage(workout, this);
            this.NavigationService.Navigate(deletePage);
        }

        // wanneer de knop 'Voeg workout toe...' wordt aangeklikt navigeer je op die pagina
        // als je geen datum selecteert dan kan je niet naar de volgende pagina
        private void AddWorkoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (WorkoutCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = WorkoutCalendar.SelectedDate.Value;
                WorkoutsAddPage addWorkoutPage = new WorkoutsAddPage(selectedDate);
                addWorkoutPage.WorkoutAdded += (s, e) => LoadWorkouts(selectedDate);
                this.NavigationService.Navigate(addWorkoutPage);
            }
            else
            {
                MessageTextBlock.Text = "Selecteer eerst een datum in de kalender.";
                MessageTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
