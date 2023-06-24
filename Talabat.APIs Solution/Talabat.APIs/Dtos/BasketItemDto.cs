using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.Dtos
{
    public class BasketItemDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string PictureUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Type { get; set; }

    }
}