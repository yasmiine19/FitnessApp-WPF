using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;
using ScottPlot.Plottables;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for OefeningenVerwijderPage.xaml
    /// </summary>
    public partial class OefeningenVerwijderPage : Page
    {
        private Exercise _exercise;
        private OefeningenPage _oefeningenPage;

        public OefeningenVerwijderPage(Exercise exercise, OefeningenPage oefeningenPage)
        {
            InitializeComponent();
            _exercise = exercise; // De oefening die moet worden verwijder
            _oefeningenPage = oefeningenPage;
            LoadExerciseDetails();
        }

        private void LoadExerciseDetails()
        {
            // Vul de tekstvelden met de gegevens van de oefening
            NameTextBlock.Text = _exercise.Name;
            PointsTextBlock.Text = $"Punten: {_exercise.Points}";
            DescriptionTextBlock.Text = _exercise.Description;

            // Als de oefening een foto heeft wordt deze geladen op de page
            if (_exercise.Photo != null)
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(_exercise.Photo))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }
                ExerciseImage.Source = bitmap;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Verwijder de oefening uit de databank
            Exercise.DeleteExercise(_exercise.Id);

            // Werk de oefeningenpagina bij
            _oefeningenPage.RemoveExerciseCard(_exercise);
            NavigationService.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
