using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Express_Voitures.Models
{
    public class Modele
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du modèle est obligatoire")]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        public int MarqueId { get; set; }

        // Navigation properties
        public Marque Marque { get; set; }
        public ICollection<Finition> Finitions { get; set; }
    }
}
