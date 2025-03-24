
namespace PaginaAnimeListMVC.Models{
    public class ShowLike{
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int ShowId { get; set; }
    }
}

