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
        public Animal Animal { get; set; } = null!;


    }
}
