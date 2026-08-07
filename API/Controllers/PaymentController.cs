using API.Models;
using API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Stripe;
using Stripe.Checkout;
using System.Globalization;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;


        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected string connect = @"Server=LAPTOP-AAKP1RFL\MSSQLSERVER01;Database=Selenium;Trusted_Connection=True;TrustServerCertificate=True;";

        [HttpGet("searchcoupon")]
        public Models.Coupon SearchCoupon(string code)
        {
            Models.Coupon coupon = new Models.Coupon();
            string query = "SELECT * FROM Coupons WHERE Code = @code";

            coupon.Id = 0;
            
            using(SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@code", code);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coupon.Id = Convert.ToInt32(reader["Id"]);
                            coupon.Code = reader["Code"].ToString();
                            coupon.DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"]);
                            coupon.MinAmount = Convert.ToDecimal(reader["MinAmount"]);
                            coupon.Stock = Convert.ToInt32(reader["Stock"]);
                        }
                    }

                }
            }

            return coupon;
        }

        [HttpGet("searchusercoupon")]
        public UserCoupon SearchUserCoupon(int userid,int codeid)
        {
            UserCoupon coupon = new UserCoupon();
            string query = "SELECT * FROM UserCoupon WHERE UserId = @userid AND CouponId = @couponid";

            coupon.Id = 0;

            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userid);
                    command.Parameters.AddWithValue("@couponid", codeid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coupon.Id = Convert.ToInt32(reader["Id"]);
                            coupon.UserId = Convert.ToInt32(reader["UserId"]);
                            coupon.CouponId = Convert.ToInt32(reader["CouponId"]);
                        }
                    }

                }
            }

            return coupon;
        }
        [HttpPut("addusercoupon")]
        public void CreateUserCoupon(int userid, int couponid)
        {
            string query = "INSERT INTO UserCoupon (UserId, CouponId) Values (@userid, @couponid)"; 

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userid);
                    command.Parameters.AddWithValue("@couponid", couponid);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpPut]
        public void UpdateStock(int id, int quantity, int stock)
        {
            int updatedStock = stock - quantity;
            string query = "UPDATE Books SET Stock = @stock WHERE Id = @id";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using(SqlCommand command = new SqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@stock", updatedStock);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        [HttpPut("updatecoupon")]
        public void UpdateCouponStock(int id, int stock)
        {
            int updatedStock = stock--;
            string query = "UPDATE Coupons SET Stock = @stock WHERE Id = @id";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@stock", updatedStock);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        [HttpPost("OrderItem")]
        public void CreateOrderItem(Cart item,int id)
        {
            string query = "INSERT INTO OrderItem (OrderId, BookId, Quantity, Price) VALUES (@orderid, @bookid, @quantity, @price)";
            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderid", id);
                    command.Parameters.AddWithValue("@bookid", item.Id);
                    command.Parameters.AddWithValue("@quantity", item.Quantity);
                    command.Parameters.AddWithValue("@price", item.Price);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

        }
        [HttpGet("getorderitems")]
        public List<OrderItem> GetOrderItems(int id)
        {
            List<OrderItem> orderitems = new List<OrderItem>();
            string query = "SELECT * FROM OrderItem WHERE OrderId = @orderid";
            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@orderid", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderitems.Add(new OrderItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                OrderId = Convert.ToInt32(reader["OrderId"]),
                                BookId = Convert.ToInt32(reader["BookId"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Price = Convert.ToDecimal(reader["Price"])
                            }
                            );
                        }
                    }

                }
            }

            return orderitems;
        }

        [HttpPost("Order")]
        public void CreateOrder(int userid, int addressid, DateTime time, decimal discount)
        {
            string query = "INSERT INTO Orders (UserId, AddressId, OrderDate, DiscountAmount) Values (@userid, @addressid, @orderdate, @disc)";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userid);
                    command.Parameters.AddWithValue("@addressid", addressid);
                    command.Parameters.AddWithValue("@orderdate", time);
                    command.Parameters.AddWithValue("@disc", discount);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        [HttpGet("GetOrder")]
        public Order GetOrder(int userid, int addressid, DateTime time)
        {
            Order order = new Order();
            string query = "SELECT * FROM Orders WHERE UserId = @userid AND AddressId = @addressid AND " +
                "OrderDate >= DATEADD(second, -10, @orderdate) AND OrderDate <= DATEADD(second, 10, @orderdate);";
            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userid);
                    command.Parameters.AddWithValue("@addressid", addressid);
                    command.Parameters.AddWithValue("@orderdate", time);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            order.Id = Convert.ToInt32(reader["Id"]);
                            order.UserId = Convert.ToInt32(reader["UserId"]);
                            order.AddressId = Convert.ToInt32(reader["AddressId"]);
                            order.OrderDate = Convert.ToDateTime(reader["OrderDate"]);
                            order.DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"]);
                        }
                    }

                }
            }
            return order;
        }
        [HttpGet("GetOrders")]
        public List<Order> GetOrders(int userid)
        {
            List<Order> orderlist = new List<Order>();
            string query = "SELECT * FROM Orders WHERE UserId = @userid";
            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userid", userid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderlist.Add(new Order
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                AddressId = Convert.ToInt32(reader["AddressId"]),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"])
                            }
                            );
                        }
                    }

                }
            }
            return orderlist;
        }

        [HttpGet("addresses")]
        public ActionResult<List<Models.Address>> GetAddresses(int id)
        {
            string query = "SELECT * FROM Addresses WHERE UserId = @id AND IsDeleted = 0";
            List<Models.Address> addresses = new List<Models.Address>();

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
                            addresses.Add(new Models.Address
                            {
                                Name = Convert.ToString(reader["Namee"], CultureInfo.InvariantCulture),
                                City = Convert.ToString(reader["City"], CultureInfo.InvariantCulture),
                                District = Convert.ToString(reader["District"], CultureInfo.InvariantCulture),
                                Neighbourhood = Convert.ToString(reader["Neighbourhood"], CultureInfo.InvariantCulture),
                                Street = Convert.ToString(reader["Street"], CultureInfo.InvariantCulture),
                                Description = Convert.ToString(reader["Descriptionn"], CultureInfo.InvariantCulture),
                                BuildingNumber = Convert.ToString(reader["BuildingNumber"], CultureInfo.InvariantCulture),
                                DoorNumber = Convert.ToString(reader["DoorNumber"], CultureInfo.InvariantCulture),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Id = Convert.ToInt32(reader["Id"]),
                            });
                        }
                    }
                }
            }

            return addresses;
        }

        [HttpGet("pay")]
        public IActionResult Index()
        {
            var cartJson = HttpContext.Session.GetString("BasketKey");
            var cart = cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("OldPrice"))) ViewBag.TotalPrice = cart.Sum(x => x.Price * x.Quantity);
            else ViewBag.TotalPrice = HttpContext.Session.GetString("TotalPrice");
            ViewBag.CouponCode = HttpContext.Session.GetString("CouponCode");
            ViewBag.OldPrice = HttpContext.Session.GetString("OldPrice");
            ViewBag.Error = HttpContext.Session.GetString("PaymentError");
            ViewBag.CouponMessage = HttpContext.Session.GetString("CouponMessage");
            List<Models.Address> addresses = new List<Models.Address>();
            addresses = GetAddresses(Convert.ToInt32(HttpContext.Session.GetString("UserId"))).Value;
            HttpContext.Session.Remove("ErrorMessage");
            HttpContext.Session.Remove("CouponMessage");
            return View("Pay", addresses);
        }
        [HttpPost("applycoupon")]
        public IActionResult ApplyCoupon([FromForm(Name = "code")] string code)
        {
            var cartJson = HttpContext.Session.GetString("BasketKey");
            var cart = cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
            decimal cartamount = cart.Sum(x => x.Price * x.Quantity);


            Models.Coupon coupon = SearchCoupon(code);

            if(coupon.Id == 0)
            {
                HttpContext.Session.SetString("CouponMessage", "This Coupon Does Not Exist.");
                return RedirectToAction("Index");
            }
           
                else if(coupon.Stock > 0 || coupon.Code == "HOSGELDIN")
                {
                    if (cartamount < coupon.MinAmount)
                    {
                        HttpContext.Session.SetString("CouponMessage", "You need to buy at least" + coupon.MinAmount + " ₺ worth of item to apply this coupon.");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                    UserCoupon usercoupon = SearchUserCoupon(Convert.ToInt32(HttpContext.Session.GetString("UserId")), coupon.Id);
                    if (usercoupon.Id == 0)
                    {
                        HttpContext.Session.SetString("OldPrice", cartamount.ToString());
                        HttpContext.Session.SetString("CouponCode",coupon.Code);
                        HttpContext.Session.SetString("TotalPrice",(cartamount-coupon.DiscountAmount).ToString());
                    }
                    else
                    {
                        HttpContext.Session.SetString("CouponMessage", "You have already used this coupon.");
                        return RedirectToAction("Index");
                    }
                }
                }
                else
                {
                    HttpContext.Session.SetString("CouponMessage", "This Coupon Is Invalid.");
                    return RedirectToAction("Index");
                }


            return RedirectToAction("Index");
        }
        [HttpPost("removecoupon")]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("CouponCode");
            HttpContext.Session.Remove("OldPrice");
            HttpContext.Session.Remove("TotalPrice");
            
            return RedirectToAction("Index");
        }

        [HttpPost("completetransaction")]
        public async Task<IActionResult> CompletePayment([FromForm(Name = "addressId")] string id )
        {
            HttpContext.Session.SetString("AddressId",id);
            var cartJson = HttpContext.Session.GetString("BasketKey");
            var cart = cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
            Models.Coupon coupon = new Models.Coupon();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("CouponCode"))) coupon = SearchCoupon(HttpContext.Session.GetString("CouponCode"));


            var lineItems = new List<SessionLineItemOptions>();
                        
            foreach (var item in cart)
            {
                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "try",
                        UnitAmount = Convert.ToInt32(item.Price * 100m),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name,
                            Metadata = new Dictionary<string, string> { { "itemId", $"{item.Id}" } }
                        }
                    },
                    Quantity = item.Quantity
                };
                lineItems.Add(lineItem);
            }

            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                Metadata = new Dictionary<string, string> { { "addressId", $"{id}" } },
                SuccessUrl = "https://localhost:7009/payment/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:7009/cart/checkout"
            };

            if (coupon.Id != 0)
            {
                var couponOptions = new CouponCreateOptions
                {
                    AmountOff = Convert.ToInt64(coupon.DiscountAmount * 100m),
                    Currency = "try",
                    Duration = "once",
                    Name = coupon.Code
                };
                var couponService = new CouponService();
                Stripe.Coupon stripeCoupon = await couponService.CreateAsync(couponOptions);

                options.Discounts = new List<SessionDiscountOptions>
                    {
                    new SessionDiscountOptions
                        {
                    Coupon = stripeCoupon.Id
                         }
                     };
                     }

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);

        }
        [HttpGet("success")]
        public async Task<IActionResult> SuccessfullPayment([FromQuery(Name = "session_id")] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Couldn't find the session Id.");
            }

            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"]; ;

                var service = new SessionService();
                Session session = await service.GetAsync(id);

                if (session.PaymentStatus == "paid")
                {
                    DateTime time = DateTime.Now;
                    Models.Coupon coupon = new Models.Coupon();

                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("CouponCode"))) { coupon = SearchCoupon(HttpContext.Session.GetString("CouponCode"));
                        UpdateCouponStock(coupon.Id, coupon.Stock);
                    }
                    else {
                        coupon.Id = 0;
                    }
                    CreateOrder(Convert.ToInt32(HttpContext.Session.GetString("UserId")),
                    Convert.ToInt32(HttpContext.Session.GetString("AddressId")), time, coupon.DiscountAmount);

                    Order order = GetOrder(Convert.ToInt32(HttpContext.Session.GetString("UserId")),
                    Convert.ToInt32(HttpContext.Session.GetString("AddressId")), time);

                    var cartJson = HttpContext.Session.GetString("BasketKey");
                    var cart = cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
                    foreach(var item in cart)
                    {
                        UpdateStock(item.Id, item.Quantity, item.Stock);
                        CreateOrderItem(item,order.Id);
                    }

                    HttpContext.Session.Remove("OldPrice");
                    cart.Clear();
                    cartJson = JsonSerializer.Serialize<List<Cart>>(cart);
                    HttpContext.Session.SetString("BasketKey", cartJson);
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("CouponCode")))
                    {
                        CreateUserCoupon(Convert.ToInt32(HttpContext.Session.GetString("UserId")), coupon.Id);
                        HttpContext.Session.Remove("CouponCode");
                    }
                   return View("Success");
                }
                else
                {
                    HttpContext.Session.SetString("ErrorMessage", "Payment Has Been Failed, Please Try Again...");
                    return RedirectToAction("PaymentFailed");
                }
            }
            catch (StripeException ex)
            {
                return BadRequest($"Ödeme doğrulanamadı: {ex.Message}");
            }
        }

        [HttpGet("showorders")]
        public IActionResult ShowOrders()
        {
            List<Order> itemlessorders = GetOrders(Convert.ToInt32(HttpContext.Session.GetString("UserId")));
            List<Order> orders = new List<Order>();
            List<Models.Address> addresses = new List<Models.Address>();
            List<Book> books = new List<Book>();

            foreach (var order in itemlessorders)
            {
                order.OrderItems = GetOrderItems(order.Id);
                orders.Add(order);
                if(addresses.Count == 0 || !(addresses.Any(a => a.Id == order.AddressId)))
                {
                    addresses.Add(DbHelper.GetAddress(order.AddressId).Value);
                }
            }

            foreach(var order in orders)
            {
                foreach(var orderitem in order.OrderItems)
                {
                    books.Add(DbHelper.GetBook(orderitem.BookId,(orderitem.Price.ToString())).Value);
                }
            }

            var tuple = Tuple.Create(orders, addresses, books);

            return View("Orders",tuple);
        }
    }
}