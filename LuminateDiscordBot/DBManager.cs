using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    internal class DBManager
    {
        static string connectionString = "Data Source=LuminateConfig/database.db;Version=3;";

        public static void InitDB()
        {
            
            if(!File.Exists("LuminateConfig/database.db")) { SQLiteConnection.CreateFile("LuminateConfig/database.db"); }
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserStats(DiscordId BIGINT, MessagesSent BIGINT DEFAULT 0, Level INT DEFAULT 1, XP INT DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SelectionRoles(RoleId BIGINT)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketCategories(TicketTopic TEXT, TicketDataName TEXT, TicketDataDescription TEXT, TicketDataAutoResponse TEXT DEFAULT NULL, TicketKeywords TEXT DEFAULT [])");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS ChannelConfig(ChannelIdentifier TEXT, ChannelId BIGINT)");
            
        }

        public static List<Objects.TicketCategory> GetTicketCategories()
        {
            List<Objects.TicketCategory> cats = new List<Objects.TicketCategory>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT * FROM TicketCategories";
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cats.Add(new((string)reader["TicketTopic"], (string)reader["Keywords"]));
                    }
                }

               connection.Close();
            }

            return cats;
        }

        public static Objects.TicketCategory GetTicketCategoryFromName(string ticketTopic)
        {
            Objects.TicketCategory ticketCat = null;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "SELECT * FROM TicketCategories WHERE TicketTopic=@topic";
                    command.Parameters.AddWithValue("@topic", ticketTopic);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        ticketCat = new((string)reader["TicketTopic"], (string)reader["TicketKeywords"]);
                    }
                }
                connection.Close();
            }
            return ticketCat;
        }

        public static Task<int> AddTicketAlias(string ticketTopic, string ticketAlias)
        {
            int affected = 0;
            Objects.TicketCategory TargetTicketCategorie = GetTicketCategoryFromName(ticketTopic);
            if(TargetTicketCategorie == null) { return Task.FromResult(affected); }
            TargetTicketCategorie.CategoryAliasList.Add(ticketAlias);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE TicketCategories SET Keywords=@alias WHERE TicketTopic=@topic";
                    command.Parameters.AddWithValue("@alias", Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(TargetTicketCategorie.CategoryAliasList))));
                    command.Parameters.AddWithValue("@topic", ticketTopic);
                    affected = command.ExecuteNonQuery();
                }
                connection.Close();
                
            }
            return Task.FromResult(affected);
        }

        public static void UpdateInternalChannelConfigs()
        {
            Utils.ChannelConfig.Clear();
            using (SQLiteConnection connection = new(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new(connection))
                {
                    command.CommandText = "SELECT * FROM ChannelConfig";
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Utils.ChannelConfig.Add(Convert.ToString(reader["ChannelIdentifier"]), Convert.ToUInt64(reader["ChannelId"]));
                    }
                }

                connection.Close();
            }
        }

        public static int RemoveChannelConfigRule(string identifier)
        {
            int affected = 0;
            using (SQLiteConnection connection = new(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new(connection))
                {
                    command.CommandText = "DELETE FROM ChannelConfig WHERE ChannelIdentifier=@identifier";
                    command.Parameters.AddWithValue("@identifier", identifier);
                    affected = command.ExecuteNonQuery();
                }

                connection.Close();
            }
            return affected;
        }
        public static void ModifyChannelConfig(string identifier, ulong channelId)
        {
            try
            {
                using (SQLiteConnection connection = new(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = $"INSERT OR REPLACE INTO ChannelConfig (ChannelIdentifier, ChannelId) VALUES (@identifier, @channel)"; ;
                        command.Parameters.AddWithValue("@identifier", identifier);
                        command.Parameters.AddWithValue("@channel", channelId);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
                UpdateInternalChannelConfigs();
            }
            catch(Exception e) { Console.WriteLine(e.Message); }

        }




        static void ExecuteNonQuery(string query)
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                connection.Open();

                using(SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }
    }
}
