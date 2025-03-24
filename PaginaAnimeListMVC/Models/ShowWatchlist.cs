namespace PaginaAnimeListMVC.Models{
    public class ShowWatchlist{
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int ShowId { get; set; }
        public Show Show { get; set; }
    }
}