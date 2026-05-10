namespace ProductsApi.Models
{
    public class Animal
    {
        public required int AnimalId { get; set; }
        public required string Species { get; set; }
        public required string Name { get; set; }
        public required int Age { get; set; }
        public required char Sex { get; set; }
        public string Description { get; set; } = string.Empty;
        public ICollection<Photo> Photo { get; set; } = new List<Photo>();
    }
}
