using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Express_Voitures.Models
{
    public class Voiture : CommonObject
    {
        public int Id { get; set; }

        // Identification du véhicule
        [StringLength(17, ErrorMessage = "Le VIN doit contenir exactement 17 caractères si renseigné")]
        [Display(Name = "Numéro VIN")]
        public string? Vin { get; set; }

        [Required(ErrorMessage = "L'année est obligatoire")]
        [Range(1990, 2030, ErrorMessage = "L'année doit être entre 1990 et 2030")]
        [Display(Name = "Année")]
        public int Annee { get; set; }

        [Required]
        [Display(Name = "Marque")]
        public int MarqueId { get; set; }

        [Required]
        [Display(Name = "Modèle")]
        public int ModeleId { get; set; }

        [Required]
        [Display(Name = "Finition")]
        public int FinitionId { get; set; }

        // Informations d'achat
        [Required(ErrorMessage = "La date d'achat est obligatoire")]
        [DataType(DataType.Date)]
        [Display(Name = "Date d'achat")]
        public DateTime DateAchat { get; set; }

        [Required(ErrorMessage = "Le prix d'achat est obligatoire")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 999999.99, ErrorMessage = "Le prix d'achat doit être positif")]
        [Display(Name = "Prix d'achat")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal PrixAchat { get; set; }

        // Informations de vente
        [DataType(DataType.Date)]
        [Display(Name = "Date de disponibilité")]
        public DateTime? DateDisponibilite { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de vente")]
        public DateTime? DateVente { get; set; }

        // Photo et description (optionnelles)
        [StringLength(500)]
        [Display(Name = "URL de la photo")]
        public string? PhotoUrl { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        // Navigation properties
        public Marque? Marque { get; set; }
        public Modele? Modele { get; set; }
        public Finition? Finition { get; set; }
        public ICollection<Reparation>? Reparations { get; set; }

        // Propriétés calculées (non stockées en base)
        [NotMapped]
        [Display(Name = "Coût total des réparations")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal CoutReparationsTotal
        {
            get
            {
                return Reparations?.Sum(r => r.Cout) ?? 0;
            }
        }

        [NotMapped]
        [Display(Name = "Prix de vente")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal PrixVente
        {
            get
            {
                return PrixAchat + CoutReparationsTotal + 500;
            }
        }

        [NotMapped]
        [Display(Name = "Statut")]
        public string Statut
        {
            get
            {
                if (DateVente.HasValue)
                    return "Vendue";
                if (DateDisponibilite.HasValue)
                    return "Disponible";
                return "En réparation";
            }
        }

        [NotMapped]
        [Display(Name = "Est disponible")]
        public bool EstDisponible
        {
            get
            {
                return DateDisponibilite.HasValue && !DateVente.HasValue;
            }
        }
    }
}
