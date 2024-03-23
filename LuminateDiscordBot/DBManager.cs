using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    internal class DBManager
    {
        static string connectionString = "Data Source=LuminateConfig/database.db;Version=3;";

        public static void InitDB()
        {
            SQLiteConnection.CreateFile(connectionString);
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketConfig(TicketCategoryId BIGING DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserStats(DiscordId BIGINT, MessagesSent BIGINT DEFAULT 0, Level INT DEFAULT 1, XP INT DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SelectionRoles(RoleId BIGINT)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketCategories(TicketTopic TEXT, TicketKeywords TEXT DEFAULT [])");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS ChannelConfig(ChannelIdentifier TEXT, ChannelId BIGINT)");
            
        }


        public static void ModifyChannelConfig(string identifier, ulong channelId)
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new(connection))
                {
                    command.CommandText = $"INSERT INTO ChannelConfig (ChannelIdentifier, ChannelId) VALUES (@identifier, @channel) ON DUPLICATE KEY UPDATE ChannelIdentifier=@identifier";
                    command.Parameters.AddWithValue("@identifier", identifier);
                    command.Parameters.AddWithValue("@channel", channelId);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
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
