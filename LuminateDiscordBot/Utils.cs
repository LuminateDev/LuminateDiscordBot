using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuminateDiscordBot
{
    internal class Utils
    {
        public static Objects.Config Config;

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
    }
}
