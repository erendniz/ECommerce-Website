namespace API.Models
{
    public class Book
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Stock { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Translator { get; set; }
        public string Publisher { get; set; }
        public string Price { get; set; }
        public string Size { get; set; }
        public string Printyear { get; set; }
        public string Barcode { get; set; }
        public string Rating { get; set; }
        public string Comments { get; set; }
        public string Image { get; set; }
        public string Pages { get; set; }
        public string Description { get; set; }
        public bool FavoriteStatus { get; set; }
    }
}
