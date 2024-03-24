using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    internal class DBManager
    {
        static string connectionString = "Data Source=LuminateConfig/database.db;Version=3;";

        public static void InitDB()
        {
            SQLiteConnection.CreateFile("LuminateConfig/database.db");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketConfig(TicketCategoryId BIGING DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserStats(DiscordId BIGINT, MessagesSent BIGINT DEFAULT 0, Level INT DEFAULT 1, XP INT DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SelectionRoles(RoleId BIGINT)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketCategories(TicketTopic TEXT, TicketKeywords TEXT DEFAULT [])");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS ChannelConfig(ChannelIdentifier TEXT, ChannelId BIGINT)");
            
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
