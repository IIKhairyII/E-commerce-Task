using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_commerce.Models
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public int Key { get; set; }
        [Required]
        public string ProuctName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Brand { get; set; }
        public string? Type { get; set; }
        [Required]
        public string Description { get; set; }

        //Propert used to show messages in createion operation
        [NotMapped]
        public string? ResponseMessage { get; set; }
        [NotMapped]
        public bool isCreated { get; set; }
        [NotMapped, Required]
        public IFormFile Image { get; set; }
    }
}
