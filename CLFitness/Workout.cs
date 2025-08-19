using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace CLFitness
{
    public class Workout
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double? Distance { get; set; }
        public int CustomerId { get; set; }
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        // Constructors
        public Workout()
        {
        }

        public Workout(int id, DateTime date, double? distance, int customerId, int exerciseId)
        {
            Id = id;
            Date = date;
            Distance = distance;
            CustomerId = customerId;
            ExerciseId = exerciseId;
        }

        // CRUD methods

        // Implementatie om een nieuwe workout toe te voegen aan de database
        public static void AddWorkout(Workout workout)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO Workout (date, distance, customer_id, exercise_id) VALUES (@date, @distance, @customer_id, @exercise_id)";
                    cmd.Parameters.AddWithValue("@date", workout.Date);
                    cmd.Parameters.AddWithValue("@customer_id", workout.CustomerId);
                    cmd.Parameters.AddWithValue("@exercise_id", workout.ExerciseId);

                    if (workout.Distance.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@distance", workout.Distance.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@distance", DBNull.Value);
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een bestaande workout bij te werken in de database
        public static void UpdateWorkout(Workout workout)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE Workout SET date = @date, distance = @distance, customer_id = @customer_id, exercise_id = @exercise_id WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", workout.Id);
                    cmd.Parameters.AddWithValue("@date", workout.Date);
                    cmd.Parameters.AddWithValue("@distance", workout.Distance);
                    cmd.Parameters.AddWithValue("@customer_id", workout.CustomerId);
                    cmd.Parameters.AddWithValue("@exercise_id", workout.ExerciseId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een workout uit de database te verwijderen
        public static void DeleteWorkout(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM Workout WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een workout op te halen uit de database
        public static Workout GetWorkout(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Workout WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Workout
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                Distance = reader["distance"] as double?,
                                CustomerId = (int)reader["customer_id"],
                                ExerciseId = (int)reader["exercise_id"],
                                Exercise = Exercise.GetExercise((int)reader["exercise_id"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Implementatie om workouts op te halen op basis van datum en klant ID
        public static List<Workout> GetWorkoutsByDate(DateTime date, int customerId)
        {
            List<Workout> workouts = new List<Workout>();
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Workout WHERE CAST(date AS DATE) = @date AND customer_id = @customerId";
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@customerId", customerId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            workouts.Add(new Workout
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                Distance = reader["distance"] as double?,
                                CustomerId = (int)reader["customer_id"],
                                ExerciseId = (int)reader["exercise_id"],
                                Exercise = Exercise.GetExercise((int)reader["exercise_id"])
                            });
                        }
                    }
                }
            }
            return workouts;
        }

        // Implementatie om workouts op te halen binnen een datumbereik
        public static List<Workout> GetWorkoutsByDateRange(DateTime startDatum, DateTime eindDatum, int customerId)
        {
            List<Workout> workouts = new List<Workout>();
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Workout WHERE date >= @startDatum AND date <= @eindDatum AND customer_id = @customerId";
                    cmd.Parameters.AddWithValue("@startDatum", startDatum);
                    cmd.Parameters.AddWithValue("@eindDatum", eindDatum);
                    cmd.Parameters.AddWithValue("@customerId", customerId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            workouts.Add(new Workout
                            {
                                Id = (int)reader["id"],
                                Date = (DateTime)reader["date"],
                                Distance = reader["distance"] as double?,
                                CustomerId = (int)reader["customer_id"],
                                ExerciseId = (int)reader["exercise_id"],
                                Exercise = Exercise.GetExercise((int)reader["exercise_id"])
                            });
                        }
                    }
                }
            }
            return workouts;
        }
    }
}
