namespace Express_Voitures.Models
{
    public abstract class CommonObject 
    {
        public  DateTime DateCreate { get; set; }= DateTime.Now;
        public  DateTime DateUpdate { get; set; }= DateTime.Now;

    }
}
