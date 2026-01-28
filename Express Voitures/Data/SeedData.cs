using Express_Voitures.Models;

namespace Express_Voitures.Data

{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Marques.Any())
                return;

            var marques = new List<Marque>
            {
                new Marque { Nom = "Toyota" },
                new Marque { Nom = "Peugeot" },
                new Marque { Nom = "Ford" },
                new Marque { Nom = "Volkswagen" },
                new Marque { Nom = "Renault" },
                new Marque { Nom = "Citroën" },
                new Marque { Nom = "Fiat" },
                new Marque { Nom = "Dodge" },
                new Marque { Nom = "Porsche" }
            };
            context.Marques.AddRange(marques);
            context.SaveChanges();

            var modeles = new List<Modele>
            {
                new Modele { Nom = "Corolla", MarqueId = marques[0].Id },
                new Modele { Nom = "Yaris", MarqueId = marques[0].Id },
                new Modele { Nom = "208", MarqueId = marques[1].Id },
                new Modele { Nom = "3008", MarqueId = marques[1].Id },
                new Modele { Nom = "Focus", MarqueId = marques[2].Id },
                new Modele { Nom = "Mustang", MarqueId = marques[2].Id },
                new Modele { Nom = "Golf", MarqueId = marques[3].Id },
                new Modele { Nom = "Passat", MarqueId = marques[3].Id },
                new Modele { Nom = "Clio", MarqueId = marques[4].Id },
                new Modele { Nom = "Mégane", MarqueId = marques[4].Id },
                new Modele { Nom = "C3", MarqueId = marques[5].Id },
                new Modele { Nom = "C4", MarqueId = marques[5].Id },
                new Modele { Nom = "500", MarqueId = marques[6].Id },
                new Modele { Nom = "Panda", MarqueId = marques[6].Id },
                new Modele { Nom = "Challenger", MarqueId = marques[7].Id },
                new Modele { Nom = "Charger", MarqueId = marques[7].Id },
                new Modele { Nom = "911", MarqueId = marques[8].Id },
                new Modele { Nom = "Cayenne", MarqueId = marques[8].Id }

            };
            context.Modeles.AddRange(modeles);
            context.SaveChanges();

            var finitions = new List<Finition>
            {
                new Finition { Nom = "Standard", ModeleId = modeles[0].Id },
                new Finition { Nom = "GR Sport", ModeleId = modeles[0].Id },
                new Finition { Nom = "Active", ModeleId = modeles[2].Id },
                new Finition { Nom = "Allure", ModeleId = modeles[2].Id },
                new Finition { Nom = "Trend", ModeleId = modeles[4].Id },
                new Finition { Nom = "ST-Line", ModeleId = modeles[4].Id },
                new Finition { Nom = "Style", ModeleId = modeles[6].Id },
                new Finition { Nom = "R-Line", ModeleId = modeles[6].Id },
                new Finition { Nom = "Evolution", ModeleId = modeles[8].Id },
                new Finition { Nom = "Esprit Alpine", ModeleId = modeles[8].Id },
                new Finition { Nom = "You", ModeleId = modeles[10].Id },
                new Finition { Nom = "Max", ModeleId = modeles[10].Id },
                new Finition { Nom = "Pop", ModeleId = modeles[12].Id },
                new Finition { Nom = "La Prima", ModeleId = modeles[12].Id },
                new Finition { Nom = "SXT", ModeleId = modeles[14].Id },
                new Finition { Nom = "GT", ModeleId = modeles[14].Id },
                new Finition { Nom = "Carrera", ModeleId = modeles[16].Id },
                new Finition { Nom = "GT3 RS", ModeleId = modeles[16].Id }


            };
            context.Finitions.AddRange(finitions);
            context.SaveChanges();
        }
            
    }
}
