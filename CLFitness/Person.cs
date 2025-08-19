using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CLFitness
{
    public class Person
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public byte[] ProfilePhoto { get; set; }
        public DateTime RegDate { get; set; }
        public bool IsAdmin { get; set; }

        // Constructors
        public Person()
        {
        }

        public Person(int id, string firstname, string lastname, string login, string password, byte[] profilePhoto, DateTime regDate, bool isAdmin)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
            Login = login;
            Password = HashPassword(password);
            ProfilePhoto = profilePhoto;
            RegDate = regDate;
            IsAdmin = isAdmin;
        }

        // Implementatie om het wachtwoord te hashen met SHA256
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // CRUD

        // Implementatie om een nieuwe persoon toe te voegen aan de database
        public static void AddPerson(Person person)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Haal de hoogste bestaande ID op en verhoog deze met 1
                int newId;
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(id), 0) + 1 FROM Person", conn))
                {
                    newId = (int)cmd.ExecuteScalar();
                }

                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    // Zet IDENTITY_INSERT aan
                    cmd.CommandText = "SET IDENTITY_INSERT Person ON";
                    cmd.ExecuteNonQuery();

                    // Voeg de persoon in met expliciete ID
                    cmd.CommandText = "INSERT INTO Person (id, firstname, lastname, login, password, profilephoto, regdate, isadmin) VALUES (@id, @firstname, @lastname, @login, @password, @profilephoto, @regdate, @isadmin)";
                    cmd.Parameters.AddWithValue("@id", newId);
                    cmd.Parameters.AddWithValue("@firstname", person.Firstname);
                    cmd.Parameters.AddWithValue("@lastname", person.Lastname);
                    cmd.Parameters.AddWithValue("@login", person.Login);
                    cmd.Parameters.AddWithValue("@password", person.Password);
                    cmd.Parameters.AddWithValue("@profilephoto", person.ProfilePhoto ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@regdate", person.RegDate);
                    cmd.Parameters.AddWithValue("@isadmin", person.IsAdmin);
                    cmd.ExecuteNonQuery();

                    // Zet IDENTITY_INSERT weer uit
                    cmd.CommandText = "SET IDENTITY_INSERT Person OFF";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een bestaande persoon te updaten in de database
        public static void UpdatePerson(Person person)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append("UPDATE Person SET firstname = @firstname, lastname = @lastname, login = @login");

                    // Voegt het wachtwoord alleen toe aan de query als het is gewijzigd
                    if (!string.IsNullOrEmpty(person.Password))
                    {
                        queryBuilder.Append(", password = @password");
                        cmd.Parameters.AddWithValue("@password", person.Password);
                    }

                    queryBuilder.Append(", profilephoto = @profilephoto, regdate = @regdate, isadmin = @isadmin WHERE id = @id");

                    cmd.CommandText = queryBuilder.ToString();
                    cmd.Parameters.AddWithValue("@id", person.Id);
                    cmd.Parameters.AddWithValue("@firstname", person.Firstname);
                    cmd.Parameters.AddWithValue("@lastname", person.Lastname);
                    cmd.Parameters.AddWithValue("@login", person.Login);
                    cmd.Parameters.AddWithValue("@profilephoto", person.ProfilePhoto ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@regdate", person.RegDate);
                    cmd.Parameters.AddWithValue("@isadmin", person.IsAdmin);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om een persoon te verwijderen uit de database
        public static void DeletePerson(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "DELETE FROM Person WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Implementatie om het totale aantal personen in de database te krijgen
        public static int GetPersonCount()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT COUNT(*) FROM Person";
                    conn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        // Implementatie om een persoon op te halen op basis van hun login
        public static Person GetPersonByLogin(string login)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Person WHERE login = @login";
                    cmd.Parameters.AddWithValue("@login", login);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Person
                            {
                                Id = (int)reader["id"],
                                Firstname = reader["firstname"].ToString(),
                                Lastname = reader["lastname"].ToString(),
                                Login = reader["login"].ToString(),
                                Password = reader["password"].ToString(),
                                ProfilePhoto = reader["profilephoto"] as byte[],
                                RegDate = (DateTime)reader["regdate"],
                                IsAdmin = (bool)(reader["isadmin"] as byte? == 1)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Implementatie om alle personen op te halen
        public static List<Person> GetAllPersons()
        {
            List<Person> persons = new List<Person>();
            string connectionString = ConfigurationManager.ConnectionStrings["FitnessDB"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Person", conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Person person = new Person
                            {
                                Id = (int)reader["id"],
                                Firstname = reader["firstname"].ToString(),
                                Lastname = reader["lastname"].ToString(),
                                Login = reader["login"].ToString(),
                                Password = reader["password"].ToString(),
                                ProfilePhoto = reader["profilephoto"] as byte[],
                                RegDate = (DateTime)reader["regdate"],
                                IsAdmin = (bool)(reader["isadmin"] as byte? == 1)
                            };
                            persons.Add(person);
                        }
                    }
                }
            }
            return persons;
        }
    }
}
