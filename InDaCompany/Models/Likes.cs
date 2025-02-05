namespace InDaCompany.Models
{
    public class Likes
    {
        public int Id { get; set;}
        public int UtenteID { get; set;}
        public int PostID { get; set;}
        public bool IsLiked { get; set;}
    }
}