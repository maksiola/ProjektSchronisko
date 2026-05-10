namespace ProductsApi.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public bool Main { get; set; }

        public string ApiLink { get; set; } = string.Empty;

        public int AnimalId { get; set; }

        public Animal Animal { get; set; } = null!;
    }
}
