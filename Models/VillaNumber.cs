using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apiprac
{
    public class VillaNumber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid VillaNo { get; set; }

        [ForeignKey("Villa")]
        public Guid VillaId { get; set; }

        public Villa Villa { get; set; }

        public string SpecialDetails { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
