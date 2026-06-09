using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProductsApi.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public bool Main { get; set; }

        public byte[]? ImageData { get; set; }

        [NotMapped]
        public string? Base64Data { get; set; }

        public int AnimalId { get; set; }
        public string ImageExtension { get; set; } = string.Empty;

        [ValidateNever]
        [JsonIgnore]
        public Animal? Animal { get; set; }
    }
}
