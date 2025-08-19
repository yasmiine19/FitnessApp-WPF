using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfCustomer
{
    /// <summary>
    /// Interaction logic for WorkoutsVerwijderPage.xaml
    /// </summary>
    public partial class WorkoutsVerwijderPage : Page
    {
        private Workout workout;
        private WorkoutsPage _workoutsPage;

        public WorkoutsVerwijderPage(Workout workout, WorkoutsPage workoutsPage)
        {
            InitializeComponent();
            this.workout = workout;
            this._workoutsPage = workoutsPage;
            LoadWorkoutDetails();
        }

        // Laad de details van de workout
        private void LoadWorkoutDetails()
        {
            NameTextBlock.Text = workout.Exercise.Name;
            DateTextBlock.Text = $"Datum: {workout.Date:dd MMMM yyyy}";
            DistanceTextBlock.Text = $"Afstand: {workout.Distance} km";
            PointsTextBlock.Text = $"Punten: {workout.Exercise.Points}";

            // Laad de foto van de workout
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
                WorkoutImage.Source = bitmap;
            }
        }

        // Verwijder de workout en keer terug naar de vorige pagina
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Workout.DeleteWorkout(workout.Id);
            _workoutsPage.LoadWorkouts(workout.Date);
            this.NavigationService.GoBack();
        }

        // De annuleer button keert naar de workoutspage
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
