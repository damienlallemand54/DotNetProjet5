using System.ComponentModel.DataAnnotations;

namespace Express_Voitures.Models
{
    public class Marque
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom de la marque est obligatoire")]
        [StringLength(100)]
        public string Nom { get; set; }

        // Navigation property : liste des modèles de cette marque
        public ICollection<Modele> Modeles { get; set; }
    }
}
