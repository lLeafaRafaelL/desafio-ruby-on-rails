using System.ComponentModel.DataAnnotations;

namespace ByCoders.CNAB.API.Configurations;

public record FileStorageConfiguration
{
    [Required]
    public string StoragePath { get; set; }
}
