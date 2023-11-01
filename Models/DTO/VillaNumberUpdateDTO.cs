using System.ComponentModel.DataAnnotations;

namespace apiprac
{
    public class VillaNumberUpdateDTO
    {
        public string SpecialDetails { get; set; }

        [Required]
        public Guid VillaId { get; set; }
    }
}

