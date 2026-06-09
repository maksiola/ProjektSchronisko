using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace ProductsApi.Models
{
    public class Form
    {
        public int Id { get; set; }
        public string Date { get; set; } = String.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public int AnimalId { get; set; }
        [ValidateNever]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Animal? Animal { get; set; }


    }
}
