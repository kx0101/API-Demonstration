using System.ComponentModel.DataAnnotations;

namespace apiprac
{
    public class VillaDTO
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int Rate { get; set; }

        [Required]
        public int Sqft { get; set; }
    }
}
