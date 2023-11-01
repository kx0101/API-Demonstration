using System.ComponentModel.DataAnnotations;

namespace apiprac
{
    public class VillaNumberDTO
    {
        [Required]
        public Guid VillaNo { get; set; }

        public Guid VillaId { get; set; }

        public string SpecialDetails { get; set; }
    }
}
