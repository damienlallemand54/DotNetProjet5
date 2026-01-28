using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Express_Voitures.Models
{
    public class Reparation : CommonObject
    {
        public int Id { get; set; }

        [Required]
        public int VoitureId { get; set; }

        [Required(ErrorMessage = "La description de la réparation est obligatoire")]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le coût est obligatoire")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 99999.99, ErrorMessage = "Le coût doit être positif")]
        [Display(Name = "Coût")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Cout { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de réparation")]
        public DateTime? DateReparation { get; set; }

        // Navigation property
        public Voiture Voiture { get; set; }
    }
}
