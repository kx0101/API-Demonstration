using System.ComponentModel.DataAnnotations;

namespace apiprac
{
    public class VillaNumberCreateDTO
    {
        public string SpecialDetails { get; set; }

        [Required]
        public Guid VillaId { get; set; }
    }
}

