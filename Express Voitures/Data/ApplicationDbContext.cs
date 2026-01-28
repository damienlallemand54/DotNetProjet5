using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Express_Voitures.Models;

namespace Express_Voitures.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets (tables de la base de données)
        public DbSet<Marque> Marques { get; set; }
        public DbSet<Modele> Modeles { get; set; }
        public DbSet<Finition> Finitions { get; set; }
        public DbSet<Voiture> Voitures { get; set; }
        public DbSet<Reparation> Reparations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relations
            // 1 Marque → N Modeles
            modelBuilder.Entity<Marque>()
                .HasMany(m => m.Modeles)
                .WithOne(mod => mod.Marque)
                .HasForeignKey(mod => mod.MarqueId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression d'une marque si des modèles l'utilisent

            // 1 Modele → N Finitions
            modelBuilder.Entity<Modele>()
                .HasMany(mod => mod.Finitions)
                .WithOne(f => f.Modele)
                .HasForeignKey(f => f.ModeleId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression d'un modèle si des finitions l'utilisent

            // 1 Voiture → N Reparations
            modelBuilder.Entity<Voiture>()
                .HasMany(v => v.Reparations)
                .WithOne(r => r.Voiture)
                .HasForeignKey(r => r.VoitureId)
                .OnDelete(DeleteBehavior.Cascade); // Si on supprime une voiture, ses réparations sont supprimées aussi

            // 1 Voiture → 1 Marque
            modelBuilder.Entity<Voiture>()
                .HasOne(v => v.Marque)
                .WithMany()
                .HasForeignKey(v => v.MarqueId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1 Voiture → 1 Modele
            modelBuilder.Entity<Voiture>()
               .HasOne(v => v.Modele)
               .WithMany()
               .HasForeignKey(v => v.ModeleId)
               .OnDelete(DeleteBehavior.Restrict);

            // 1 Voiture → 1 Finition
            modelBuilder.Entity<Voiture>()
                .HasOne(v => v.Finition)
                .WithMany()
                .HasForeignKey(v => v.FinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index et contraintes
            // Index unique sur le VIN si renseigné (pas de doublons)
            // Note : l'index unique autorise plusieurs valeurs NULL
            modelBuilder.Entity<Voiture>()
                .HasIndex(v => v.Vin)
                .IsUnique()
                .HasFilter("[Vin] IS NOT NULL"); // Uniquement si VIN renseigné

            // Index unique sur le nom de la marque (pas de doublons)
            modelBuilder.Entity<Marque>()
                .HasIndex(m => m.Nom)
                .IsUnique();

        }
    }
}

