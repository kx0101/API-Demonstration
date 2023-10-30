using System.ComponentModel.DataAnnotations;

namespace apiprac
{
    public class VillaDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string Rate { get; set; }

        [Required]
        public string Sqft { get; set; }
    }
}
