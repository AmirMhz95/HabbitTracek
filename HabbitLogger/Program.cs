using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Data.Sqlite;

namespace habit_tracker
{
    class Program
    {
        static string connectionString = @"Data Source=habbit_tracker.db";

        static void Main(string[] args)
        {

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
Id INTEGER PRIMARY KEY AUTOINCREMENT,
Date TEXT,
Quantity INTEGER)";

                tableCmd.ExecuteNonQuery();

                connection.Close();

            }

            GetUserInput();
        }

        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;

            while (closeApp == false)
            {
                Console.WriteLine("\nWhat would you like to do?\n");
                Console.WriteLine("0. Close");
                Console.WriteLine("1. View all records");
                Console.WriteLine("2. Insert record");
                Console.WriteLine("3. Delete record");
                Console.WriteLine("4. Update record");


                string commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\nGoodBye!\n");
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        Insert();
                        break;
                    case "3":
                        Delete();
                        break;
                    case "4":
                        Update();
                        break;
                }
            }
        }

        private static void GetAllRecords()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = "SELECT * FROM drinking_water";

                List<DrinkingWater> drinkingWaterTableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    {
                        while (reader.Read())
                        {
                            drinkingWaterTableData.Add(new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                                Quantity = reader.GetInt32(2)
                            });

                        }
                    }
                }
                else
                {
                    Console.WriteLine("No records found!");
                }

                connection.Close();

                foreach (var dw in drinkingWaterTableData)
                {
                    Console.WriteLine($"ID: {dw.Id}, Date: {dw.Date.ToString("dd-MM-yy")}, Quantity: {dw.Quantity}");
                }
            }

        }

        private static void Update()
        {
            Console.Clear();
            GetAllRecords();

            var recordID = GetNumberInput("\n\nPlease Enter the ID of the record you want to update!\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT Exists(SELECT 1 FROM drinking_water WHERE Id = {recordID})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine("Record not found!");
                    connection.Close();
                    Update();
                }

                string date = GetDateInput();

                int quantity = GetNumberInput("\n\nPlease Enter the new Quantity!\n\n");

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET Date = '{date}', Quantity = {quantity} WHERE Id = {recordID}";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static void Insert()
        {
            string date = GetDateInput();
            int quantity = GetNumberInput("\n\nPlease Insert Number of Glasses you Drank!\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"INSERT INTO drinking_water (Date, Quantity) VALUES ('{date}', {quantity})";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static string GetDateInput()
        {
            Console.WriteLine("Enter the date (dd-MM-yy): ");
            string date = Console.ReadLine();

            if (date == "0") GetUserInput();
            return date;
        }

        internal static int GetNumberInput(string message)
        {
            Console.WriteLine(message);
            string input = Console.ReadLine();

            if (input == "0") GetUserInput();

            int finalInput = Convert.ToInt32(input);
            return finalInput;
        }

        private static void Delete()
        {
            Console.WriteLine("Enter the ID of the record you want to delete: ");
            int id = GetNumberInput("");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id = {id}";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

    }
    public class DrinkingWater
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}






