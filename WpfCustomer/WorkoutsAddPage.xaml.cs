using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfCustomer
{
    /// <summary>
    /// Interactielogica voor WorkoutsAddPage.xaml
    /// </summary>
    public partial class WorkoutsAddPage : Page
    {
        private DateTime selectedDate;
        public event EventHandler WorkoutAdded;

        public WorkoutsAddPage(DateTime date)
        {
            InitializeComponent();
            selectedDate = date;
            WorkoutDatePicker.SelectedDate = selectedDate;
            LoadExercises(); // haal de lijst van de oefeningen op 
        }

        // Laadt de lijst van oefeningen in de ComboBox
        private void LoadExercises()
        {
            List<Exercise> exercises = Exercise.GetAllExercises();
            ExerciseComboBox.ItemsSource = exercises;
            ExerciseComboBox.DisplayMemberPath = "Name";
        }

        // werkt eens er op de button opslaan wordt gedrukt
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExerciseComboBox.SelectedItem != null)
            {
                Exercise selectedExercise = (Exercise)ExerciseComboBox.SelectedItem; // haal de geselecteerde oefening op 
                double? distance = null;  // afstand is optioneel en wordt alleen gebruikt voor cardio oefening

                if (selectedExercise.Type == ExerciseType.Cardio)
                {
                    if (string.IsNullOrWhiteSpace(DistanceTextBox.Text) || !double.TryParse(DistanceTextBox.Text, out double parsedDistance))
                    {
                        // als de aftstand voor cardio oefeingen niet ingevuld is toont het een foutmelding
                        ErrorTextBlock.Text = "Vul een afstand in.";
                        ErrorTextBlock.Visibility = Visibility.Visible;
                        return;
                    }
                    else
                    {
                        distance = parsedDistance;
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(DistanceTextBox.Text) && double.TryParse(DistanceTextBox.Text, out double parsedDistance))
                    {
                        distance = parsedDistance;
                    }
                }

                // het aanmaken van de nieuwe workout aan
                Workout newWorkout = new Workout
                {
                    Date = WorkoutDatePicker.SelectedDate.HasValue ? WorkoutDatePicker.SelectedDate.Value : selectedDate,
                    Distance = distance,
                    CustomerId = (Application.Current.Properties["CurrentUser"] as Person).Id,
                    ExerciseId = selectedExercise.Id,
                    Exercise = selectedExercise
                };

                // Voeg de nieuwe workout toe
                Workout.AddWorkout(newWorkout);
                WorkoutAdded?.Invoke(this, EventArgs.Empty);
                this.NavigationService.GoBack();
            }
            else
            {
                ErrorTextBlock.Text = "Kies een oefening.";
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        // annuleer button om terug te gaan naar de workoutpage
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        // Verandert de afbeelding afhankelijk van de geselecteerde oefening
        private void ExerciseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExerciseComboBox.SelectedItem is Exercise selectedExercise && selectedExercise.Photo != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(selectedExercise.Photo))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                WorkoutImage.Source = bitmap;
            }
            else
            {
                WorkoutImage.Source = null;
            }
        }
    }
}
