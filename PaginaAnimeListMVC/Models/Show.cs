using System.ComponentModel.DataAnnotations.Schema;

namespace PaginaAnimeListMVC.Models
{
    [NotMapped]
    public class Show
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Genre { get; set; }
        public string Studio { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }
    }
}
