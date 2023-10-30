using System.ComponentModel.DataAnnotations;

namespace apiprac
{
    public class VillaDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int Rate { get; set; }

        [Required]
        public int Sqft { get; set; }
    }
}
