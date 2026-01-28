using System.ComponentModel.DataAnnotations;

namespace Express_Voitures.Models
{
    public class Finition
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la finition est obligatoire")]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        public int ModeleId { get; set; }

        // Navigation property
        public Modele Modele { get; set; }
    }
}
