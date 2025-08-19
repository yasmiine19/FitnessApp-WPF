using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for OefeningenDetailsPage.xaml
    /// </summary>
    public partial class OefeningenDetailsPage : Page
    {
        private Exercise _exercise;

        public OefeningenDetailsPage(Exercise exercise)
        {
            InitializeComponent();
            _exercise = exercise;
            LoadExerciseData(); // laad de gegevens van de oefeningen
        }

        private void LoadExerciseData()
        {
            // vul de tekstblokken met de gegevens van de oefening uit de databank
            NameTextBlock.Text = _exercise.Name;
            TypeTextBlock.Text = _exercise.Type.ToString();
            InstructionTextBlock.Text = _exercise.Instruction;
            BodypartTextBlock.Text = _exercise.Bodypart;
            PoseTextBlock.Text = _exercise.Pose;
            NicknameTextBlock.Text = _exercise.Nickname;
            PointsTextBlock.Text = _exercise.Points + " points";
            DescriptionTextBlock.Text = _exercise.Description;

            if (_exercise.Photo != null)
            {
                using (MemoryStream stream = new MemoryStream(_exercise.Photo))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    ExerciseImage.Source = bitmap;
                }
            }
        }

        // methode om terug naar de vorige pagina te navigeren
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
