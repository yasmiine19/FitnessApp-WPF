using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CLFitness;
using ScottPlot;

namespace WpfCustomer
{
    /// <summary>
    /// Interactielogica voor StatistiekenPage.xaml
    /// </summary>
    public partial class StatistiekenPage : Page
    {
        public StatistiekenPage()
        {
            InitializeComponent();
            Loaded += (s, e) => InitialiseerGrafiek(); // Het initialiseren van de grafiek wanneer de pagina geladen is
        }

        private void InitialiseerGrafiek()
        {
            // Initialiseer met lege waarden
            PlotStatistics(new double[0], new string[0]);
        }

        // als de datums geselecteerd zijn en pas dan werkt het button om de grafiek te tonen 
        // als de datum niet geselecteerd is is er een message
        private void ToonStatistiekenButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed; // Reset error message visibility

            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            // Controleer of beide data zijn geselecteerd
            if (!startDate.HasValue || !endDate.HasValue)
            {
                ErrorTextBlock.Text = "Selecteer een geldig datumbereik.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Controleer of de datums geldig zijn
            if (!IsValidDate(startDate.Value) || !IsValidDate(endDate.Value))
            {
                ErrorTextBlock.Text = "Foute datumbereik.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Controleer of het bereik geldig is
            if (startDate > endDate)
            {
                ErrorTextBlock.Text = "Selecteer een geldig datum.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            Person currentUser = Application.Current.Properties["CurrentUser"] as Person;
            if (currentUser != null)
            {
                int customerId = currentUser.Id;
                List<Workout> workouts = Workout.GetWorkoutsByDateRange(startDate.Value, endDate.Value, customerId);
                PlotStatistics(workouts);
            }
        }

        private bool IsValidDate(DateTime date)
        {
            // Controleer op ongeldige datumwaarden
            return date.Year >= 1900 && date.Year <= DateTime.Now.Year;
        }

        // implementatie gemaakt met behulp van scottplot 5 documentatie/voorbeeldoefeningen
        private void PlotStatistics(List<Workout> workouts)
        {
            if (workouts == null || workouts.Count == 0)
            {
                ErrorTextBlock.Text = "Geen workouts gevonden.";
                ErrorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            // Groeperen van de workouts per week zonder LINQ
            Dictionary<int, List<Workout>> groupedWorkoutsDict = new Dictionary<int, List<Workout>>();
            foreach (Workout workout in workouts)
            {
                int weekOfYear = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(workout.Date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

                if (!groupedWorkoutsDict.ContainsKey(weekOfYear))
                {
                    groupedWorkoutsDict[weekOfYear] = new List<Workout>();
                }
                groupedWorkoutsDict[weekOfYear].Add(workout);
            }

            // Sorteren van de groepen op weeknummer
            List<int> sortedKeys = new List<int>(groupedWorkoutsDict.Keys);
            sortedKeys.Sort();

            List<double> totalPointsList = new List<double>();
            List<string> labelsList = new List<string>();

            for (int i = 0; i < sortedKeys.Count; i++)
            {
                int key = sortedKeys[i];
                List<Workout> group = groupedWorkoutsDict[key];
                double weekTotalPoints = 0;
                foreach (Workout workout in group)
                {
                    weekTotalPoints += workout.Exercise.Points;
                }
                totalPointsList.Add(weekTotalPoints);
                labelsList.Add("Week " + (i + 1));
            }

            double[] totalPointsArray = totalPointsList.ToArray();
            string[] labels = labelsList.ToArray();

            // Plot de statistieken
            PlotStatistics(totalPointsArray, labels);

            double sumPoints = 0;
            for (int i = 0; i < totalPointsArray.Length; i++)
            {
                sumPoints += totalPointsArray[i];
            }
            double averagePoints = sumPoints / totalPointsArray.Length;

            List<DateTime> dates = new List<DateTime>();
            foreach (Workout workout in workouts)
            {
                dates.Add(workout.Date);
            }
            int longestStreak = CalculateStreak(dates);

            // Update UI elementen
            TotalPointsTextBlock.Text = $"{sumPoints} punten";
            AveragePointsTextBlock.Text = $"Gemiddelde punten per week: {averagePoints:F2}";
            LongestStreakTextBlock.Text = $"Langste reeks: {longestStreak} dagen";
        }

        private int CalculateStreak(List<DateTime> dates)
        {
            if (dates == null || dates.Count == 0)
                return 0;

            dates.Sort();
            int longestStreak = 0;
            int currentStreak = 1;

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days == 1)
                {
                    currentStreak++;
                }
                else
                {
                    if (currentStreak > longestStreak)
                    {
                        longestStreak = currentStreak;
                    }
                    currentStreak = 1;
                }
            }

            if (currentStreak > longestStreak)
            {
                longestStreak = currentStreak;
            }

            return longestStreak;
        }

        // implementatie gemaakt met behulp van scottplot 5 documentatie/voorbeeldoefeningen
        private void PlotStatistics(double[] values, string[] labels)
        {
            // Maak een nieuwe plot
            ScottPlot.Plot plt = WpfPlot1.Plot;
            plt.Clear(); // Verwijder oude plottables

            // Stel de x/y-as limieten in
            plt.Axes.SetLimits(-0.5, 10, 0, 900);

            // Maak de bar plot zichtbaar
            var barPlot = plt.Add.Bars(values);

            // customiseren van de x en y as en de grafiek
            plt.Axes.Title.Label.FontSize = 25;
            plt.Axes.Bottom.Label.FontSize = 16;
            plt.Axes.Left.Label.FontSize = 16;
            plt.DataBackground.Color = Colors.Beige;

            // Set x-axis labels using TickGenerator (gegevens aanpassen voor schuin opmaak)
            ScottPlot.Tick[] ticks = labels.Select((label, index) => new ScottPlot.Tick(index, label)).ToArray();
            ScottPlot.TickGenerators.NumericManual tickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
            plt.Axes.Bottom.TickGenerator = tickGenerator;
            plt.Axes.Bottom.TickLabelStyle.Rotation = 45;
            plt.Axes.Bottom.TickLabelStyle.OffsetX = 20;

            // Het opstellen van een titel voor de assen en de grafiek
            plt.Axes.Bottom.Label.Text = "Week";
            plt.Axes.Left.Label.Text = "Totale Punten";
            plt.Axes.Title.Label.Text = "Totale Punten per Week";

            WpfPlot1.Refresh();
        }
    }
}