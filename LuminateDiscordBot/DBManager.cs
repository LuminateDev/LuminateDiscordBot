using System.Data.SQLite;

namespace LuminateDiscordBot
{
    internal class DBManager
    {
        private static readonly string _connectionString = "Data Source=LuminateConfig/database.db;Version=3;";

        public static void InitDB()
        {
            if (!File.Exists("LuminateConfig/database.db"))
            {
                SQLiteConnection.CreateFile("LuminateConfig/database.db");
            }
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketConfig(TicketCategoryId BIGINT DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS UserStats(DiscordId BIGINT, MessagesSent BIGINT DEFAULT 0, Level INT DEFAULT 1, XP INT DEFAULT 0)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS SelectionRoles(RoleId BIGINT)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS TicketCategories(TicketTopic TEXT, TicketKeywords TEXT DEFAULT [])");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS ChannelConfig(ChannelIdentifier TEXT, ChannelId BIGINT)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS CreateVoice_Data(category_id LONG, channel_id LONG, guild_id LONG)");
            ExecuteNonQuery("CREATE TABLE IF NOT EXISTS CreateVoice_CreatedChannels(guild_id LONG, channel_id LONG, user_id LONG)");
        }

        public static void UpdateInternalChannelConfigs()
        {
            Utils.ChannelConfig.Clear();
            using (SQLiteConnection connection = new(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new(connection))
                {
                    command.CommandText = "SELECT * FROM ChannelConfig";
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Utils.ChannelConfig.Add(Convert.ToString(reader["ChannelIdentifier"]), Convert.ToUInt64(reader["ChannelId"]));
                        }
                    }
                }
            }
        }

        public static int RemoveChannelConfigRule(string identifier)
        {
            int affected = 0;
            using (SQLiteConnection connection = new(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new(connection))
                {
                    command.CommandText = "DELETE FROM ChannelConfig WHERE ChannelIdentifier=@identifier";
                    command.Parameters.AddWithValue("@identifier", identifier);
                    affected = command.ExecuteNonQuery();
                }
            }
            UpdateInternalChannelConfigs();
            return affected;
        }

        public static ulong CheckOwnershipForPrivateChannel(ulong guildId)
        {
            ulong userId = 0;
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "SELECT user_id FROM CreateVoice_CreatedChannels WHERE guild_id=@guild_id";
                        command.Parameters.AddWithValue("@guild_id", guildId);
                        command.ExecuteNonQuery();
                        
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long userIdLong = reader.GetInt64(0);
                                userId = (ulong)userIdLong;
                            }
                        }
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ModifyVoiceChannelConfig: {e.Message}");
            } 
            return userId;
        }
        
        public static void ModifyVoiceChannelConfig(ulong categoryId, ulong channelId, ulong guildId)
        {
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "INSERT OR REPLACE INTO CreateVoice_Data (category_id, channel_id, guild_id) VALUES (@category_id, @channel_id, @guild_id)";
                        command.Parameters.AddWithValue("@category_id", categoryId);
                        command.Parameters.AddWithValue("@channel_id", channelId);
                        command.Parameters.AddWithValue("@guild_id", guildId);
                        command.ExecuteNonQuery();
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ModifyVoiceChannelConfig: {e.Message}");
            }
        }
        
        public static ulong GetCategoryId(ulong guildId)
        {
            ulong catId = 0;
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "SELECT category_id FROM CreateVoice_Data WHERE guild_id=@guild_id";
                        command.Parameters.AddWithValue("@guild_id", guildId);
                        command.ExecuteNonQuery();
                        
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long catIdLong = reader.GetInt64(0);
                                catId = (ulong)catIdLong;
                            }
                        }
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ModifyVoiceChannelConfig: {e.Message}");
            } 
            return catId;
        }
        
        public static ulong GetPrivateJoinVoiceChannel(ulong guildId)
        {
            ulong channelId = 0;
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "SELECT channel_id FROM CreateVoice_Data WHERE guild_id=@guild_id";
                        command.Parameters.AddWithValue("@guild_id", guildId);
                        command.ExecuteNonQuery();

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                long channelId_long = reader.GetInt64(0);
                                channelId = (ulong)channelId_long;
                            }
                        }
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ModifyVoiceChannelConfig: {e.Message}");
            }
            return channelId;
        }

        public static bool SearchPrivateVoiceChannels(ulong guildId, ulong channel_id)
        {
            bool entryFound = false;
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "SELECT channel_id FROM CreateVoice_CreatedChannels WHERE guild_id=@guild_id AND channel_id=@channel_id";
                        command.Parameters.AddWithValue("@guild_id", guildId);
                        command.Parameters.AddWithValue("@channel_id", channel_id);

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                entryFound = true;
                            }
                        }
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ModifyVoiceChannelConfig: {e.Message}");
            }
            return entryFound;
        }
        
        public static void AddVoiceChannel(ulong channelId, ulong guildId, ulong userId)
        {
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "INSERT INTO CreateVoice_CreatedChannels(guild_id, channel_id, user_id) VALUES (@guild_id, @channel_id, @user_id)";
                        command.Parameters.AddWithValue("@channel_id", channelId);
                        command.Parameters.AddWithValue("@guild_id", guildId);
                        command.Parameters.AddWithValue("@user_id", userId);
                        command.ExecuteNonQuery();
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in AddVoiceChannel: {e.Message}");
            }
        }
        public static void DeleteVoiceChannel(ulong channelId)
        {
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "DELETE FROM CreateVoice_CreatedChannels WHERE channel_id=@channel_id";
                        command.Parameters.AddWithValue("@channel_id", channelId);
                        command.ExecuteNonQuery();
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in AddVoiceChannel: {e.Message}");
            }
        }
        

        public static void ModifyChannelConfig(string identifier, ulong channelId)
        {
            try
            {
                using (SQLiteConnection connection = new(_connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new(connection))
                    {
                        command.CommandText = "INSERT OR REPLACE INTO ChannelConfig (ChannelIdentifier, ChannelId) VALUES (@identifier, @channel)";
                        command.Parameters.AddWithValue("@identifier", identifier);
                        command.Parameters.AddWithValue("@channel", channelId);
                        command.ExecuteNonQuery();
                    }
                }
                UpdateInternalChannelConfigs();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ModifyChannelConfig: {e.Message}");
            }
        }

        private static void ExecuteNonQuery(string query)
        {
            using (SQLiteConnection connection = new(_connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = new(connection))
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
