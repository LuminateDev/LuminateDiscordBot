using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuminateDiscordBot.Models.Database
{
    [Table("data_configs")]
    public class DataConfig
    {
        [Key]
        [Column("config_id")]
        [Required]
        public required string ConfigurationId { get; set; } = Guid.NewGuid().ToString("N");
        [Column("data_name")] public required string DataName { get; set; }
        [Column("data_value")] public required ulong DataValue { get; set; }
        [Column("data_type")] public required DataTypes DataType { get; set; }

        public enum DataTypes
        {
            ROLE,
            CHANNEL
        }
    }
}
