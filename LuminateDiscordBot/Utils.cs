using System.Text.Json;

namespace LuminateDiscordBot
{
    internal class Utils
    {
        public static Objects.Config Config;

        public static string DefaultSloganText = "Your ideas shine bright";

        public static Dictionary<string, ulong> ChannelConfig = new Dictionary<string, ulong>();
        
        public static void FileCheck()
        {
            if(!Directory.Exists("LuminateConfig")) { CreateFiles(); }
            if(!File.Exists("LuminateConfig/config.json")) { CreateFiles(); }
        }

        public static void ReadFiles()
        {
            Config = JsonSerializer.Deserialize<Objects.Config>(File.ReadAllText("LuminateConfig/config.json"));
        }

        static void CreateFiles()
        {
            Directory.CreateDirectory("LuminateConfig");
            using (StreamWriter sw = File.CreateText("LuminateConfig/config.json")) { sw.Write(JsonSerializer.Serialize(new Objects.Config(), new JsonSerializerOptions { WriteIndented = true })); }
        }

        static void WriteConfig()
        {
            using (StreamWriter sw = File.CreateText("LuminateConfig/config.json")) { sw.Write(JsonSerializer.Serialize(Config), new JsonSerializerOptions() { WriteIndented = true }); }
        }

    }
}
