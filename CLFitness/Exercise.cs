using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace CLFitness
{
    public enum ExerciseType
    {
        Cardio = 1,
        Dumbell = 2,
        Yoga = 3
    }

    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ExerciseType Type { get; set; }
        public string Instruction { get; set; }
        public string Bodypart { get; set; }
        public string Pose { get; set; }
        public string Nickname { get; set; }
        public byte[] Photo { get; set; }
        public int Points { get; set; }
        public bool IsCardio { get; set; }

        // Constructors
        public Exercise()
        {
        }

        public Exercise(int id, string name, string description, ExerciseType type, string instruction, string bodypart, string pose, string nickname, byte[] photo, int points)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
            Instruction = instruction;
            Bodypart = bodypart;
            Pose = pose;
            Nickname = nickname;
            Photo = photo;
            Points = points;
        }

        // CRUD methods

        // Implementatie om een nieuwe oefening toe te voegen aan de database
        public static void AddExercise(Exercise exercise)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO Exercise (name, description, type, instruction, bodypart, pose, nickname, photo, points) VALUES (@name, @description, @type, @instruction, @bodypart, @pose, @nickname, @photo, @points)";
                    cmd.Parameters.AddWithValue("@name", exercise.Name);
                    cmd.Parameters.AddWithValue("@description", exercise.Description);
                    cmd.Parameters.AddWithValue("@type", (int)exercise.Type);
                    cmd.Parameters.AddWithValue("@instruction", exercise.Instruction);
                    cmd.Parameters.AddWithValue("@bodypart", exercise.Bodypart);
                    cmd.Parameters.AddWithValue("@pose", exercise.Pose);
                    cmd.Parameters.AddWithValue("@nickname", exercise.Nickname);
                    cmd.Parameters.AddWithValue("@photo", exercise.Photo);
                    cmd.Parameters.AddWithValue("@points", exercise.Points);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een bestaande oefening te updaten in de database
        public static void UpdateExercise(Exercise exercise)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE Exercise SET name = @name, description = @description, type = @type, instruction = @instruction, bodypart = @bodypart, pose = @pose, nickname = @nickname, photo = @photo, points = @points WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", exercise.Id);
                    cmd.Parameters.AddWithValue("@name", exercise.Name);
                    cmd.Parameters.AddWithValue("@description", exercise.Description);
                    cmd.Parameters.AddWithValue("@type", (int)exercise.Type);
                    cmd.Parameters.AddWithValue("@instruction", exercise.Instruction);
                    cmd.Parameters.AddWithValue("@bodypart", exercise.Bodypart);
                    cmd.Parameters.AddWithValue("@pose", exercise.Pose);
                    cmd.Parameters.AddWithValue("@nickname", exercise.Nickname);
                    cmd.Parameters.AddWithValue("@photo", exercise.Photo);
                    cmd.Parameters.AddWithValue("@points", exercise.Points);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een oefening uit de database te verwijderen
        public static void DeleteExercise(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Verwijder eerst de gerelateerde workouts
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Workout WHERE exercise_id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                // Verwijder daarna de oefening zelf
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Exercise WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een oefening op te halen uit de database
        public static Exercise GetExercise(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Exercise WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Exercise
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                Type = (ExerciseType)(int)reader["type"],
                                Instruction = reader["instruction"].ToString(),
                                Bodypart = reader["bodypart"].ToString(),
                                Pose = reader["pose"].ToString(),
                                Nickname = reader["nickname"].ToString(),
                                Photo = reader["photo"] as byte[],
                                Points = (int)reader["points"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Implementatie om alle oefeningen uit de database op te halen
        public static List<Exercise> GetAllExercises()
        {
            List<Exercise> exercises = new List<Exercise>();
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Exercise";
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Exercise exercise = new Exercise
                            {
                                Id = (int)reader["id"],
                                Name = reader["name"].ToString(),
                                Description = reader["description"].ToString(),
                                Type = (ExerciseType)(int)reader["type"],
                                Instruction = reader["instruction"].ToString(),
                                Bodypart = reader["bodypart"].ToString(),
                                Pose = reader["pose"].ToString(),
                                Nickname = reader["nickname"].ToString(),
                                Photo = reader["photo"] as byte[],
                                Points = (int)reader["points"]
                            };
                            exercises.Add(exercise);
                        }
                    }
                }
            }
            return exercises;
        }
    }
}