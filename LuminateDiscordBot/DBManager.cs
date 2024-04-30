using Dapper;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;

namespace LuminateDiscordBot
{
    internal class DBManager
    {
        static string connectionString = "Data Source=LuminateConfig/database.db;Version=3;";

        public static void InitDB()
        {

            if (!File.Exists("LuminateConfig/database.db")) { SQLiteConnection.CreateFile("LuminateConfig/database.db"); }
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserStats(DiscordId BIGINT, MessagesSent BIGINT DEFAULT 0, Level INT DEFAULT 1, XP INT DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SelectionRoles(RoleId BIGINT)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketCategories(TicketTopic TEXT, TicketDataName TEXT UNIQUE, TicketDataDescription TEXT, TicketDataAutoResponse TEXT DEFAULT NULL, TicketKeywords TEXT DEFAULT [])");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS ChannelConfig(ChannelIdentifier TEXT, ChannelId BIGINT UNIQUE)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS RoleConfig(RoleIdentifier TEXT, RoleId BIGINT UNIQUE)");

        }

        public static List<Objects.TicketCategory> GetTicketCategories()
        {
            List<Objects.TicketCategory> cats = new List<Objects.TicketCategory>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                cats = connection.Query<Objects.TicketCategory>("SELECT * FROM TicketCategories").AsList();

                connection.Close();
            }

            return cats;
        }

        public static Objects.TicketCategory? GetTicketCategoryFromName(string ticketTopic)
        {
            Objects.TicketCategory? ticketCat = null;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Dictionary<string, object> parameters = new() { { "@ticketData", ticketTopic } };
                ticketCat = connection.Query<Objects.TicketCategory>($"SELECT * FROM TicketCategories WHERE TicketDataName=@ticketData", parameters).FirstOrDefault();

                connection.Close();
            }
            return ticketCat;
        }

        public static int AddTicketAlias(string ticketTopic, string ticketAlias)
        {
            int affected = 0;
            Objects.TicketCategory? TargetTicketCategory = GetTicketCategoryFromName(ticketTopic);
            if (TargetTicketCategory == null) { return affected; }
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE TicketCategories SET Keywords=@alias WHERE TicketDataName=@topic";
                    command.Parameters.AddWithValue("@alias", Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(TargetTicketCategory.GetCategoryAliases()))));
                    command.Parameters.AddWithValue("@topic", ticketTopic);
                    affected = command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return affected;
        }

        public static void AddOrUpdateTicketCategory(string ticketTopic, string ticketDataName, string ticketDataDescription)
        {
            int affected = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "INSERT OR IGNORE INTO TicketCategories(TicketTopic, TicketDataName, TicketDataDescription) VALUES(@topic, @dataname, @datadescription)";
                    command.Parameters.AddWithValue("@topic", ticketTopic);
                    command.Parameters.AddWithValue("@dataname", ticketDataName);
                    command.Parameters.AddWithValue("@datadescription", ticketDataDescription);
                    affected = command.ExecuteNonQuery();
                }
                connection.Close();
            }
            if (affected == 0)
            {
                SetTicketDataDescription(ticketDataName, ticketDataDescription);
                SetTicketTopicText(ticketDataName, ticketTopic);
            }
        }

        public static int SetTicketAutoResponse(string ticketDataName, string ticketAutoResponse)
        {
            int affected = 0;
            Objects.TicketCategory? TargetTicket = GetTicketCategoryFromName(ticketDataName);
            if (TargetTicket == null) { return affected; }
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE TicketCategories SET TicketDataAutoResponse=@response WHERE TicketDataName=@dataname";
                    command.Parameters.AddWithValue("@response", ticketAutoResponse);
                    command.Parameters.AddWithValue("@dataname", ticketDataName);
                    affected = command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return affected;
        }

        public static int SetTicketDataDescription(string ticketDataName, string ticketDataDescription)
        {
            int affected = 0;
            Objects.TicketCategory? TargetTicket = GetTicketCategoryFromName(ticketDataName);
            if (TargetTicket == null) { return affected; }
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE TicketCategories SET TicketDataDescription=@desc WHERE TicketDataName=@name";
                    command.Parameters.AddWithValue("@desc", ticketDataDescription);
                    command.Parameters.AddWithValue("@name", ticketDataName);
                    affected = command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return affected;
        }

        public static int SetTicketTopicText(string ticketDataName, string ticketCategoryText)
        {
            int affected = 0;
            Objects.TicketCategory TargetTicket = GetTicketCategoryFromName(ticketDataName);
            if (TargetTicket == null) { return affected; }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = "UPDATE TicketCategories SET TicketTopic=@topic WHERE TicketDataName=@dataname";
                    command.Parameters.AddWithValue("@topic", ticketCategoryText);
                    command.Parameters.AddWithValue("@dataname", ticketDataName);
                    affected = command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return affected;
        }

        public static void UpdateInternalChannelConfigs()
        {
            Utils.ChannelConfig.Clear();
            Utils.RoleConfig.Clear();
            using (SQLiteConnection connection = new(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new(connection))
                {
                    command.CommandText = "SELECT * FROM ChannelConfig; SELECT * FROM RoleConfig";
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Utils.ChannelConfig.Add(Convert.ToString(reader["ChannelIdentifier"]), Convert.ToUInt64(reader["ChannelId"]));
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        Utils.RoleConfig.Add(Convert.ToString(reader["RoleIdentifier"]), Convert.ToUInt64(reader["RoleId"]));
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
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        public static void ModifyRoleConfig(string identifier, ulong roleId)
        {
            try
            {
                using (SQLiteConnection connection = new(connectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = $"INSERT OR REPLACE INTO RoleConfig (RoleIdentifier, RoleId) VALUES (@identifier, @role)";
                        command.Parameters.AddWithValue("@identifier", identifier);
                        command.Parameters.AddWithValue("@role", roleId);
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }




        static void ExecuteNonQuery(string query)
        {
            using (SQLiteConnection connection = new(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
