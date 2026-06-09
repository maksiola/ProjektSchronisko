using Microsoft.EntityFrameworkCore;

namespace ProductsApi.Models
{
    public class Card
    {
        public int Id { get; set; }
        public long Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public int AnimalId { get; set; }

        public Animal? Animal { get; set; }



    }
}
