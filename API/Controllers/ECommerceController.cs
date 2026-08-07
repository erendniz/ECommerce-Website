using API.Models;
using API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("ecommerce")]
    public class ECommerceController : Controller
    {

        protected List<Book> BookList = new List<Book>();

        private readonly ILogger<ECommerceController> _logger;



        protected string connect = @"Server=LAPTOP-AAKP1RFL\MSSQLSERVER01;Database=Selenium;Trusted_Connection=True;TrustServerCertificate=True;";

        public ECommerceController(ILogger<ECommerceController> logger)
        {
            _logger = logger;

        }

        private List<Cart> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("BasketKey");
            return cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
        }

        [HttpGet("books")]
        public ActionResult<List<Book>> GetBooks(int searchid)
        {
            string query;
            if (searchid == 0) query = "SELECT * FROM Books";
            else query = "Select * FROM Books WHERE CategoryId = " + searchid;

            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BookList.Add(new Book
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                Translator = reader["Translator"].ToString(),
                                Publisher = reader["Publisher"].ToString(),
                                Price = reader["Price"].ToString(),
                                Size = reader["Sizee"].ToString(),
                                Printyear = string.IsNullOrWhiteSpace(reader["Printyear"].ToString()) ? "0" : reader["Printyear"].ToString(),
                                Barcode = reader["Barcode"].ToString(),
                                Rating = reader["Rating"].ToString(),
                                Comments = reader["Comments"].ToString(),
                                Image = reader["Imagee"].ToString(),
                                Pages = reader["Pages"].ToString(),
                                Stock = Convert.ToInt32(reader["Stock"]),
                                CategoryId = Convert.ToInt32(reader["CategoryId"])
                            });
                        }
                    }
                }
            }
            return BookList;
        }
        [HttpGet("getreviews")]
        public ActionResult<List<Review>> GetReviews(int searchid)
        {
            string query = "Select * FROM Reviews WHERE BookId = " + searchid;
            List<Review> reviews = new List<Review>();

            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reviews.Add(new Review
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                BookId = Convert.ToInt32(reader["BookId"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Comment = Convert.ToString(reader["Comment"]),
                                Rating = Convert.ToInt32(reader["Rating"]),
                                ReviewDate = Convert.ToDateTime(reader["ReviewDate"])
                            });
                        }
                    }
                }
            }
            return reviews;
        }

        [HttpPost("addreview")]
        public string AddReview(Review review)
        {
            string query = "INSERT INTO Reviews (BookId, UserId, Comment, Rating, ReviewDate)" +
                " VALUES (@bookid, @userid, @comment,@rating,@revdate)";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.Add("@bookid", SqlDbType.NVarChar, 250).Value = review.BookId;
                    command.Parameters.Add("@userid", SqlDbType.NVarChar, 250).Value = review.UserId;
                    command.Parameters.Add("@comment", SqlDbType.NVarChar, 250).Value = review.Comment;
                    command.Parameters.Add("@rating", SqlDbType.NVarChar, 250).Value = review.Rating;
                    command.Parameters.Add("@revdate", SqlDbType.NVarChar, 250).Value = review.ReviewDate;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return "Item has been added successfully.";
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(int id)
        {
            string query = "SELECT * FROM Books Where Id = @id";
            Book book = new Book();

            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            book.Id = Convert.ToInt32(reader["Id"]);
                            book.Title = reader["Title"].ToString();
                            book.Author = reader["Author"].ToString();
                            book.Translator = reader["Translator"].ToString();
                            book.Publisher = reader["Publisher"].ToString();
                            book.Price = reader["Price"].ToString();
                            book.Size = reader["Sizee"].ToString();
                            book.Printyear = string.IsNullOrWhiteSpace(reader["Printyear"].ToString()) ? "0" : reader["Printyear"].ToString();
                            book.Barcode = reader["Barcode"].ToString();
                            book.Rating = reader["Rating"].ToString();
                            book.Comments = reader["Comments"].ToString();
                            book.Image = reader["Imagee"].ToString();
                            book.Pages = reader["Pages"].ToString();
                            book.Stock = Convert.ToInt32(reader["Stock"]);
                            book.Description = reader["Descript"].ToString();
                            book.CategoryId = Convert.ToInt32(reader["CategoryId"]);
                        }
                    }

                }

            }
            if (book.Id == 0) return NotFound();
            return book;
        }

        [HttpPost("addbook")]
        public string AddBook(Book book)
        {
            string query = "INSERT INTO Books (Title, Author, Translator, Publisher, Price, Sizee, Printyear, Barcode, Rating, Comments, Imagee, Pages, Stock)" +
                "VALUES (@title, @author, @translator, @publisher, @price, @size, @printyear, @barcode, @rating, @comments, @image, @pages, @stock)";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.Add("@title", SqlDbType.NVarChar, 250).Value = book.Title;
                    command.Parameters.Add("@author", SqlDbType.NVarChar, 250).Value = book.Author;
                    command.Parameters.Add("@translator", SqlDbType.NVarChar, 250).Value = book.Translator;
                    command.Parameters.Add("@publisher", SqlDbType.NVarChar, 250).Value = book.Publisher;
                    command.Parameters.Add("@price", SqlDbType.NVarChar, 250).Value = book.Price;
                    command.Parameters.Add("@size", SqlDbType.NVarChar, 250).Value = book.Size;
                    command.Parameters.Add("@printyear", SqlDbType.NVarChar, 250).Value = book.Printyear;
                    command.Parameters.Add("@barcode", SqlDbType.NVarChar, 250).Value = book.Barcode;
                    command.Parameters.Add("@rating", SqlDbType.NVarChar, 250).Value = book.Rating;
                    command.Parameters.Add("@comments", SqlDbType.NVarChar, 250).Value = book.Comments;
                    command.Parameters.Add("@image", SqlDbType.NVarChar, 250).Value = book.Image;
                    command.Parameters.Add("@pages", SqlDbType.NVarChar, 250).Value = book.Pages;
                    command.Parameters.Add("@stock", SqlDbType.NVarChar, 250).Value = book.Stock;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return "Item has been added successfully.";
        }

        [HttpDelete("{id}")]
        public ActionResult<string> DeleteBook(int id)
        {
            string query = "DELETE FROM Books WHERE Id = @id";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    int rowaffected = command.ExecuteNonQuery();

                    if (rowaffected == 1) return "Item Has Been Deleted.";
                    return NotFound($"Unable to find item with id {id}.");
                }
            }

        }
        [HttpPut("{id}")]
        public ActionResult<string> UpdateBook(int id, Book book)
        {
            string query = "UPDATE Books SET Title = @title, Author = @author, Translator = @translator, Publisher = @publisher, Price = @price, Sizee = @size, Printyear = @printyear, Barcode = @barcode, Rating = @rating, Comments = @comments, Imagee = @image, Pages = @pages, Stock= @stock WHERE Id = @id";
            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@title", book.Title);
                    command.Parameters.AddWithValue("@author", book.Author);
                    command.Parameters.AddWithValue("@translator", book.Translator);
                    command.Parameters.AddWithValue("@publisher", book.Publisher);
                    command.Parameters.AddWithValue("@price", book.Price);
                    command.Parameters.AddWithValue("@size", book.Size);
                    command.Parameters.AddWithValue("@printyear", book.Printyear);
                    command.Parameters.AddWithValue("@barcode", book.Barcode);
                    command.Parameters.AddWithValue("@rating", book.Rating);
                    command.Parameters.AddWithValue("@comments", book.Comments);
                    command.Parameters.AddWithValue("@image", book.Image);
                    command.Parameters.AddWithValue("@pages", book.Pages);
                    command.Parameters.AddWithValue("@stock", book.Stock);
                    connection.Open();
                    int rowaffected = command.ExecuteNonQuery();
                    if (rowaffected > 0) return "Item Has Been Updated.";
                    return NotFound($"Unable to find item with id {id}.");
                }
            }
        }
        [HttpGet("categories")]
        public ActionResult<List<Category>> GetCategories()
        {
            List<Category> categories = new List<Category>();

            string query = "SELECT * FROM Categories";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Namee"].ToString()
                            });
                        }
                    }
                }
            }

            return categories;
        }
        public async Task<IActionResult> Main()
        {
            var apidata = GetBooks(0).Value;
            var favdata = GetFavorites();
            var categories = GetCategories().Value;
            Random random = new Random();
            List<int> randomlist;
            List<Book> randombooks;

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("ButtonAction"))){
                randomlist = new List<int>();
                int value = -1;
                for (int i = 0; i < 5; i++)
                {
                    do
                    {
                        value = random.Next(apidata.Count);
                    } while (randomlist.Contains(value));
                    randomlist.Add(value);

                    
                    }

                var randomjson = JsonSerializer.Serialize<List<int>>(randomlist);
                HttpContext.Session.SetString("RandomList", randomjson);
            }
            else
            {
                var randomjson = HttpContext.Session.GetString("RandomList");
                randomlist = randomjson == null ? new List<int>() : JsonSerializer.Deserialize<List<int>>(randomjson);
                HttpContext.Session.Remove("ButtonAction");
            }

            randombooks = new List<Book>();
            for (int i = 0; i < 5; i++)
            {
                randombooks.Add(apidata[randomlist[i]]);
            }

            foreach (var item in randombooks)
            {
                if (favdata.Any(f => f.BookId == item.Id && f.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId"))))
                {
                    (randombooks[randombooks.FindIndex(c => c.Id == item.Id)]).FavoriteStatus = true;
                }
                else
                {
                    (randombooks[randombooks.FindIndex(c => c.Id == item.Id)]).FavoriteStatus = false;
                }
            }

            var recentJson = HttpContext.Session.GetString("RecentKey");
            var recent = recentJson == null ? new List<Book>() : JsonSerializer.Deserialize<List<Book>>(recentJson);

            if (recent.Count > 0)
            {
                foreach (var item in recent)
                {
                    if (favdata.Any(f => f.BookId == item.Id && f.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId"))))
                    {
                        (recent[recent.FindIndex(c => c.Id == item.Id)]).FavoriteStatus = true;
                    }
                    else
                    {
                        (recent[recent.FindIndex(c => c.Id == item.Id)]).FavoriteStatus = false;
                    }
                }
            }

            var cart = GetCartFromSession();
            ViewBag.TotalQuantity = cart.Sum(x => x.Quantity);
            ViewBag.TotalPrice = cart.Sum(x => x.Price * x.Quantity);

            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Surname = HttpContext.Session.GetString("Surname");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Phone = HttpContext.Session.GetString("Phone");
            ViewBag.LoginMessage = HttpContext.Session.GetString("LoginMessage");
            HttpContext.Session.Remove("PriceChoice");
            HttpContext.Session.Remove("YearChoice");
            HttpContext.Session.Remove("RateChoice");
            HttpContext.Session.Remove("TitleChoice");
            HttpContext.Session.Remove("PublisherChoice");
            HttpContext.Session.Remove("AuthorChoice");
            HttpContext.Session.Remove("LoginMessage");

            var tuple = Tuple.Create(apidata,categories,recent,randombooks);

            return View("Main", tuple);
        }
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromForm(Name = "title")] string name)
        {
            HttpContext.Session.SetString("TitleChoice",name);
            return RedirectToAction("Index");
        }
        [HttpGet("index/{id=0}")]
        public async Task<IActionResult> Index(int id)
        {
            var apidata = GetBooks(id);
            var favdata = GetFavorites();
            var categories = GetCategories().Value;
            if (id != 0) { ViewBag.CategoryName = categories.FirstOrDefault(c => c.Id == id).Name;
                HttpContext.Session.SetString("Category", id.ToString());
            }
            else
            {
                HttpContext.Session.Remove("Category");
            }
            List<Book> books = new List<Book>();
            List<Book> books2 = new List<Book>();
            List<Book> booksrate = new List<Book>();
            List<Book> bookstitle = new List<Book>();
            List<Book> bookspublisher = new List<Book>();
            List<Book> booksauthor = new List<Book>();

            var cart = GetCartFromSession();
            ViewBag.TotalQuantity = cart.Sum(x => x.Quantity);
            ViewBag.TotalPrice = cart.Sum(x => x.Price * x.Quantity);

            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Surname = HttpContext.Session.GetString("Surname");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Phone = HttpContext.Session.GetString("Phone");
            ViewBag.LoginMessage = HttpContext.Session.GetString("LoginMessage");
            HttpContext.Session.Remove("LoginMessage");

            string? sortOption = HttpContext.Session.GetString("SortOption");
            string? yearChoice = HttpContext.Session.GetString("YearChoice");
            string? priceChoice = HttpContext.Session.GetString("PriceChoice");
            string? rateChoice = HttpContext.Session.GetString("RateChoice");
            string? titleChoice = HttpContext.Session.GetString("TitleChoice");
            string? publisherChoice = HttpContext.Session.GetString("PublisherChoice");
            string? authorChoice = HttpContext.Session.GetString("AuthorChoice");

            ViewBag.PriceChoice = priceChoice;
            ViewBag.YearChoice = yearChoice;
            ViewBag.RateChoice = rateChoice;
            ViewBag.TitleChoice = titleChoice;
            ViewBag.PublisherChoice = publisherChoice;
            ViewBag.AuthorChoice = authorChoice;

            string ? show = HttpContext.Session.GetString("ShowAll");
            if (!string.IsNullOrEmpty(show) && show.Equals("yes"))
            {
                books2.AddRange(apidata.Value);
                HttpContext.Session.SetString("ShowAll","false");
            }
            else
            {
                if (apidata.Value != null)
                {
                    if ((string.IsNullOrEmpty(priceChoice) && string.IsNullOrEmpty(yearChoice) && string.IsNullOrEmpty(rateChoice)
                        && string.IsNullOrEmpty(titleChoice) && string.IsNullOrEmpty(authorChoice) && string.IsNullOrEmpty(publisherChoice)))
                    {
                        books2.AddRange(apidata.Value);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(priceChoice))
                        {

                            foreach (var book in apidata.Value)
                            {
                                string rawPriceString = book.Price.Split(' ')[0];
                                string standardizedPrice = rawPriceString.Replace(",", ".");
                                StringBuilder se = new StringBuilder(standardizedPrice);
                                if (standardizedPrice.Count(c => c == '.') > 1)
                                {
                                    int index = standardizedPrice.IndexOf('.');
                                    se.Remove(index, 1);

                                }
                              
                                standardizedPrice = se.ToString();
                                int lastindex = standardizedPrice.LastIndexOf('.');
                                standardizedPrice = standardizedPrice.Substring(0, lastindex);

                                switch (priceChoice)
                                {

                                    case "200":
                                        Console.WriteLine(standardizedPrice);
                                        if (Convert.ToInt32(standardizedPrice) < Convert.ToInt32(priceChoice))
                                        {
                                            books.Add(book);
                                            Console.WriteLine(Convert.ToInt32(standardizedPrice) + " < " + Convert.ToInt32(priceChoice));
                                        }
                                        break;
                                    case "300":
                                        if (Convert.ToInt32(standardizedPrice) >= Convert.ToInt32(priceChoice) - 100
                                            && Convert.ToInt32(standardizedPrice) <= Convert.ToInt32(priceChoice) + 100) books.Add(book);
                                        break;
                                    case "400":
                                        if (Convert.ToInt32(standardizedPrice) > Convert.ToInt32(priceChoice)) books.Add(book);
                                        break;
                                }
                            }
                        }
                        else books.AddRange(apidata.Value);
                        if (!string.IsNullOrEmpty(rateChoice))
                        {
                            foreach (var book in books)
                            {
                                int rating = 0;
                                switch (Convert.ToInt32(book.Rating))
                                {
                                    case 1:
                                    case 2:
                                        rating = 1;
                                        break;
                                    case 3:
                                    case 4:
                                        rating = 2;
                                        break;
                                    case 5:
                                    case 6:
                                        rating = 3;
                                        break;
                                    case 7:
                                    case 8:
                                        rating = 4;
                                        break;
                                    case 9:
                                    case 10:
                                        rating = 5;
                                        break;
                                    default:
                                        rating = 0;
                                        break;
                                }
                                if (rating == Convert.ToInt32(rateChoice)) booksrate.Add(book);
                            }
                        }
                        else booksrate.AddRange(books);

                        if (!string.IsNullOrEmpty(titleChoice))
                        {
                            bookstitle.AddRange(booksrate.FindAll(b => b.Title.Contains( titleChoice , StringComparison.OrdinalIgnoreCase)));
                        }
                        else bookstitle.AddRange(booksrate);

                        if (!string.IsNullOrEmpty(authorChoice))
                        {
                            booksauthor.AddRange(bookstitle.FindAll(b => b.Author.Contains( authorChoice, StringComparison.OrdinalIgnoreCase)));
                        }
                        else booksauthor.AddRange(bookstitle);

                        if (!string.IsNullOrEmpty(publisherChoice))
                        {
                            bookspublisher.AddRange(booksauthor.FindAll(b =>b.Publisher.Contains( publisherChoice, StringComparison.OrdinalIgnoreCase)));
                        }
                        else bookspublisher.AddRange(booksauthor);

                        if (!string.IsNullOrEmpty(yearChoice))
                        {
                            foreach (var book in bookspublisher)
                            {
                                switch (yearChoice)
                                {
                                    case "2010":
                                        if (Convert.ToInt32(book.Printyear) < Convert.ToInt32(yearChoice)) books2.Add(book);
                                        break;
                                    case "2015":
                                        if (Convert.ToInt32(book.Printyear) >= Convert.ToInt32(yearChoice) - 5
                                            && Convert.ToInt32(book.Printyear) <= Convert.ToInt32(yearChoice) + 5) books2.Add(book);
                                        break;
                                    case "2020":
                                        if (Convert.ToInt32(book.Printyear) > Convert.ToInt32(yearChoice)) books2.Add(book);
                                        break;
                                }
                            }
                        }
                        else books2.AddRange(bookspublisher);
                    }
                }
                else if (apidata.Result is OkObjectResult okResult)
                {
                    books2 = okResult.Value as List<Book>;
                }
            }

            if (!string.IsNullOrEmpty(sortOption))
            {
                switch (sortOption)
                {
                    case "name":
                        books2 = books2.OrderBy(b => b.Title).ToList();
                        break;
                    case "price":
                        books2 = books2.OrderBy(b => decimal.Parse(b.Price.Replace(",", ".").Substring(0, b.Price.IndexOf(" ")))).ToList();
                        break;
                    case "printyear":
                        books2 = books2.OrderBy(b => decimal.Parse(b.Printyear)).ToList();
                        break;
                    case "publisher":
                        books2 = books2.OrderBy(b => b.Publisher).ToList();
                        break;
                    case "author":
                        books2 = books2.OrderBy(b => b.Author).ToList();
                        break;
                    case "rating":
                        books2 = books2.OrderBy(b => decimal.Parse(b.Rating.Replace(",", "."), CultureInfo.InvariantCulture)).ToList();
                        break;
                }
            }


            if (books2.Count > 0)
            {

                foreach (var item in books2)
                {
                    if (favdata.Any(f => f.BookId == item.Id && f.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId"))))
                    {
                        (books2[books2.FindIndex(c => c.Id == item.Id)]).FavoriteStatus = true;
                    }
                    else
                    {
                        (books2[books2.FindIndex(c => c.Id == item.Id)]).FavoriteStatus = false;
                    }
                }
            }

            return View(books2);
        }
        [HttpPost("filter")]
        public async Task<IActionResult> Filtered([FromForm(Name = "price")] string? priceChoice, [FromForm(Name = "year")] string? yearChoice,
            [FromForm(Name = "rate")] string? rateChoice, [FromForm(Name = "title")] string? titleChoice, [FromForm(Name = "publisher")] string? publisherChoice
            ,[FromForm(Name = "author")] string? authorChoice)
        {
            if(!string.IsNullOrEmpty(priceChoice)) HttpContext.Session.SetString("PriceChoice", priceChoice);
            if (!string.IsNullOrEmpty(yearChoice)) HttpContext.Session.SetString("YearChoice", yearChoice);
            if (!string.IsNullOrEmpty(rateChoice)) HttpContext.Session.SetString("RateChoice", rateChoice);
            if (!string.IsNullOrEmpty(titleChoice)) HttpContext.Session.SetString("TitleChoice", titleChoice);
            if (!string.IsNullOrEmpty(publisherChoice)) HttpContext.Session.SetString("PublisherChoice", publisherChoice);
            if (!string.IsNullOrEmpty(authorChoice)) HttpContext.Session.SetString("AuthorChoice", authorChoice);

            return RedirectToAction("Index", new { id = Convert.ToInt32(HttpContext.Session.GetString("Category")) });
        }
        [HttpGet("showall")]
        public async Task<IActionResult> ShowALL()
        {

            HttpContext.Session.SetString("ShowAll", "yes");
            HttpContext.Session.Remove("PriceChoice");
            HttpContext.Session.Remove("YearChoice");
            HttpContext.Session.Remove("RateChoice");
            HttpContext.Session.Remove("TitleChoice");
            HttpContext.Session.Remove("PublisherChoice");
            HttpContext.Session.Remove("AuthorChoice");

            return RedirectToAction("Index", new { id = Convert.ToInt32(HttpContext.Session.GetString("Category")) });
        }

        [HttpPost("/ecommerce/sort")]
        public async Task<IActionResult> Sort([FromForm(Name = "sortoption")] string? option)
        {
            if (!string.IsNullOrEmpty(option))
            {
                HttpContext.Session.SetString("SortOption", option);
            }
            return RedirectToAction("Index", new {id = Convert.ToInt32(HttpContext.Session.GetString("Category"))});
        }

        [HttpPost("deleteconfirm")]
        public async Task<IActionResult> DeleteConfirm([FromForm(Name = "id")] int id)
        {
            Console.WriteLine("- " + id + " -");
            return View(id);
        }
        [HttpPost("delete")]

        public async Task<IActionResult> Delete([FromForm(Name = "id")] int id)
        {
            var result = DeleteBook(id);
            Console.WriteLine("* " + id + " *");

            return RedirectToAction("Main");
        }
    [HttpPost("/ecommerce/addbasket")]
    public async Task<IActionResult> AddtoBasket([FromForm(Name = "id")] int id, [FromForm(Name ="frompage")] string frompage)
        {
            Console.WriteLine("* id:" + id + " *");
            var result = GetBook(id);
            Book? book = result.Value ?? (result.Result is OkObjectResult ok ? ok.Value as Book : null);
            if (book == null)
            {
                return NotFound();
            }
            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(x => x.Id == book.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                string rawPriceString = book.Price.Split(' ')[0];
                string standardizedPrice = rawPriceString.Replace(",", ".");
                decimal cleanPrice=0;
                if (standardizedPrice.Count(c => c == '.') > 1)
                {
                    int index = standardizedPrice.IndexOf('.');
                    StringBuilder se = new StringBuilder(standardizedPrice);

                    se.Remove(index, 1);

                    standardizedPrice = se.ToString();
                } 
                cleanPrice = decimal.Parse(standardizedPrice, CultureInfo.InvariantCulture);
                cart.Add(new Cart
                {
                    Id = book.Id,
                    Name = book.Title,
                    Price = cleanPrice / 1m,
                    Quantity = 1,
                    Stock = book.Stock
                });
            }
            HttpContext.Session.SetString("BasketKey", JsonSerializer.Serialize(cart));
            if (string.Equals(frompage, "main")) {
                HttpContext.Session.SetString("ButtonAction", "true");
                return RedirectToAction("Main"); 
            }
            else if (string.Equals(frompage, "favorite")) return RedirectToAction("Favorites");
            else if (string.Equals(frompage, "details")) return RedirectToAction("Details", new { id = id });
            else return RedirectToAction("Index", new { id = Convert.ToInt32(HttpContext.Session.GetString("Category")) });
        }
        [HttpPost("favoritesbasket")]
        public IActionResult AddFavoritestoBasket()
        {
            List<Favorites> favorites = GetFavorites();
            List<Book> books = new List<Book>();
            if(favorites.Count == 0)
            {
                HttpContext.Session.SetString("FavoritesMessage", "Your Favorite List is Empty.");
                return RedirectToAction("Favorites");
            }
            foreach(var fav in favorites)
            {
                Book book = GetBook(fav.BookId).Value;
                if (book.Stock > 0) books.Add(book);
            }

            foreach(var book in books)
            {
                AddtoBasket(book.Id, "favorite");
            }


            return RedirectToAction("Index", "Cart");
        }
        [HttpPost("addfavorites")]
        public IActionResult AddFavorites([FromForm (Name="id")] int id, [FromForm(Name = "frompage")] string frompage)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            string query = "INSERT INTO Favorites (UserId,BookId) VALUES (@userid,@bookid)";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@bookid", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            if (string.Equals(frompage, "main")) {
                HttpContext.Session.SetString("ButtonAction", "true");
                return RedirectToAction("Main"); }
            else if (string.Equals(frompage, "favorite")) return RedirectToAction("Favorites");
            else if (string.Equals(frompage, "details")) return RedirectToAction("Details", new {id = id});
            else return RedirectToAction("Index", new { id = Convert.ToInt32(HttpContext.Session.GetString("Category")) });
        }

        [HttpGet("favorites")]
        public List<Favorites> GetFavorites()
        {
            List<Favorites> favorites = new List<Favorites>();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"))) return favorites;
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            string query = "SELECT * FROM Favorites WHERE UserId= @id";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using(SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", userId);
                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            favorites.Add(new Favorites
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                BookId = Convert.ToInt32(reader["BookId"])
                            });
                        }
                    }
                }
            }

            return favorites;
        }
        [HttpPost("removefavorites")]
        public IActionResult RemoveFavorites([FromForm(Name = "id")] int id, [FromForm(Name = "frompage")] string frompage)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            string query = "DELETE FROM Favorites Where UserId= @userid AND BookId= @bookid";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userId);
                    command.Parameters.AddWithValue("@bookid", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            if (string.Equals(frompage, "main"))
            {
                HttpContext.Session.SetString("ButtonAction", "true");
                return RedirectToAction("Main");
            }
            else if (string.Equals(frompage, "favorite")) return RedirectToAction("Favorites");
            else if (string.Equals(frompage, "details")) return RedirectToAction("Details", new { id = id });
            else return RedirectToAction("Index", new { id = Convert.ToInt32(HttpContext.Session.GetString("Category")) });
        }
        [HttpGet("showfavorites")]
        public IActionResult Favorites()
        {
            var favdata = GetFavorites();
            var books = new List<Book>();
            ViewBag.Message = HttpContext.Session.GetString("FavoritesMessage");
            HttpContext.Session.Remove("FavoritesMessage");

            foreach(var item in favdata)
            {
                books.Add(GetBook(item.BookId).Value);
            }

            return View("Favorites", books);
        }         
        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var book = GetBook(id).Value;
            var reviews = GetReviews(id).Value;
            var favorites = GetFavorites();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")) && !DbHelper.isReviewable(id, Convert.ToInt32(HttpContext.Session.GetString("UserId"))))
                ViewBag.ReviewMessage = "You need to buy this item first to be able to review it.";

            if (favorites.Any(f => f.BookId == id)) book.FavoriteStatus = true;
            else book.FavoriteStatus = false;

            var cart = GetCartFromSession();
            ViewBag.TotalQuantity = cart.Sum(x => x.Quantity);
            ViewBag.TotalPrice = cart.Sum(x => x.Price * x.Quantity);
            ViewBag.Name = HttpContext.Session.GetString("Name");

            var recentJson = HttpContext.Session.GetString("RecentKey");
            var recent = recentJson == null ? new List<Book>() : JsonSerializer.Deserialize<List<Book>>(recentJson);

            if(recent.Any(b => b.Id == id))
            {
                int index = recent.FindIndex(b => b.Id == id);
                if (index > 0)
                {
                    recent.RemoveAll(b => b.Id == id);
                    Console.WriteLine(recent.Count);
                    recent.Insert(0, book);
                } 
            }
            else
            {
                recent.Insert(0, book);
            }
            if (recent.Count > 5) recent.RemoveRange(5, 1);

            recentJson = JsonSerializer.Serialize<List<Book>>(recent);
            HttpContext.Session.SetString("RecentKey", recentJson);
            

            var users = new List<User>();
            Review returnreview = new Review();
            foreach (var review in reviews)
            {
                User user = DbHelper.GetUser(review.UserId).Value;
                if (user.Id == 0)
                {
                    user.Name = "Deleted";
                    user.Surname = "User";
                    users.Add(user);
                }
                else if (!users.Any(u => u.Id == review.UserId)) users.Add(user);
            }

            string reviewmessage = "";

            Console.WriteLine(HttpContext.Session.GetString("UserId"));

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"))) reviewmessage = "You need to login first to leave a review.";
            else if (users.Any(u => u.Id == Convert.ToInt32(HttpContext.Session.GetString("UserId")))) {
                reviewmessage = "false";
                returnreview = reviews.FirstOrDefault(r => r.UserId == Convert.ToInt32(HttpContext.Session.GetString("UserId"))
                && r.BookId == book.Id);
            }
            else reviewmessage = "true";

            ViewBag.ReviewControl = reviewmessage;

            var tuple = Tuple.Create(book, reviews, users, returnreview);

            return View("Details",tuple);
        }
        [HttpPost("submitreview")]
        public IActionResult SubmitReview([FromForm(Name = "book")]int id, [FromForm(Name = "rating")] int rating,
            [FromForm(Name = "comment")]string? comment)
        {
            DateTime time = DateTime.Now;
            Review review = new Review();
            Book updatedbook = GetBook(id).Value;
            review.UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            review.BookId = id;
            review.ReviewDate = time;
            if (string.IsNullOrEmpty(comment)) review.Comment = " ";
            else review.Comment = comment;
            review.Rating = rating;
            AddReview(review);
            int commentnumber = Convert.ToInt32(updatedbook.Comments);
            updatedbook.Comments = (commentnumber + 1).ToString();
            if (updatedbook.Rating == "0") updatedbook.Rating = rating.ToString();
            else updatedbook.Rating = (((Convert.ToInt32(updatedbook.Rating)*commentnumber) + rating)/(commentnumber+1)).ToString();
            UpdateBook(id, updatedbook);
        
            return RedirectToAction("Details", new { id = id });
        }
    }
}