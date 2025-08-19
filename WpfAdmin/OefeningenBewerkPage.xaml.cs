using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CLFitness;
using Microsoft.Win32;

namespace WpfAdmin
{
    /// <summary>
    /// Interaction logic for OefeningenBewerkPage.xaml
    /// </summary>
    public partial class OefeningenBewerkPage : Page
    {
        private Exercise _exercise;
        private byte[] _imageData;

        public OefeningenBewerkPage(Exercise exercise)
        {
            InitializeComponent();
            _exercise = exercise;
            LoadExerciseData();
        }

        private void LoadExerciseData()
        {
            // Voeg alle eigenschappen van de oefening toe aan de tekstvakken en andere UI-elementen
            NameTextBox.Text = _exercise.Name;
            PointsTextBox.Text = _exercise.Points.ToString();
            DescriptionTextBox.Text = _exercise.Description;
            InstructionTextBox.Text = _exercise.Instruction;
            BodypartTextBox.Text = _exercise.Bodypart;
            PoseTextBox.Text = _exercise.Pose;
            NicknameTextBox.Text = _exercise.Nickname;

            // Stel de afbeelding in, indien aanwezig
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
                    _imageData = _exercise.Photo;
                }
            }
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                ExerciseImage.Source = bitmap;

                using (MemoryStream stream = new MemoryStream())
                {
                    BitmapEncoder encoder = null;
                    if (openFileDialog.FileName.EndsWith(".jpg") || openFileDialog.FileName.EndsWith(".jpeg"))
                    {
                        encoder = new JpegBitmapEncoder();
                    }
                    else if (openFileDialog.FileName.EndsWith(".png"))
                    {
                        encoder = new PngBitmapEncoder();
                    }

                    if (encoder != null)
                    {
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        encoder.Save(stream);
                        _imageData = stream.ToArray();
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _exercise.Name = NameTextBox.Text;
            _exercise.Points = int.Parse(PointsTextBox.Text);
            _exercise.Description = DescriptionTextBox.Text;
            _exercise.Instruction = InstructionTextBox.Text;
            _exercise.Bodypart = BodypartTextBox.Text;
            _exercise.Pose = PoseTextBox.Text;
            _exercise.Nickname = NicknameTextBox.Text;
            _exercise.Photo = _imageData;

            Exercise.UpdateExercise(_exercise);
            NavigationService.Navigate(new OefeningenPage());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}