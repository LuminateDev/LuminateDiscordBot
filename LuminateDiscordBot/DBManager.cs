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
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS WelcomeConfig(WelcomeChannelId BIGINT DEFAULT 0, WelcomeMessage TEXT)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketConfig(TicketCategoryId BIGING DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserStats(DiscordId BIGINT, MessagesSent BIGINT DEFAULT 0, Level INT DEFAULT 1, XP INT DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SelectionRoles(RoleId BIGINT)");


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
