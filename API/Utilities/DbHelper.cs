using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace API.Utilities
{
    public static class DbHelper
    {
        private static string connect = @"Server=LAPTOP-AAKP1RFL\MSSQLSERVER01;Database=Selenium;Trusted_Connection=True;TrustServerCertificate=True;";

        public static ActionResult<Address> GetAddress(int id)
        {
            string query = "SELECT * FROM Addresses WHERE Id = @id";
            Address address = new Address();

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

                            address.Name = Convert.ToString(reader["Namee"], CultureInfo.InvariantCulture);
                            address.City = Convert.ToString(reader["City"], CultureInfo.InvariantCulture);
                            address.District = Convert.ToString(reader["District"], CultureInfo.InvariantCulture);
                            address.Neighbourhood = Convert.ToString(reader["Neighbourhood"], CultureInfo.InvariantCulture);
                            address.Street = Convert.ToString(reader["Street"], CultureInfo.InvariantCulture);
                            address.Description = Convert.ToString(reader["Descriptionn"], CultureInfo.InvariantCulture);
                            address.BuildingNumber = Convert.ToString(reader["BuildingNumber"], CultureInfo.InvariantCulture);
                            address.DoorNumber = Convert.ToString(reader["DoorNumber"], CultureInfo.InvariantCulture);
                            address.UserId = Convert.ToInt32(reader["UserId"]);
                            address.Id = Convert.ToInt32(reader["Id"]);
                            
                        }
                    }
                }
            }

            return address;
        }
        public static ActionResult<Book> GetBook(int id,string price)
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
                            book.Price = price;
                            book.Size = reader["Sizee"].ToString();
                            book.Printyear = reader["Printyear"].ToString();
                            book.Barcode = reader["Barcode"].ToString();
                            book.Rating = reader["Rating"].ToString();
                            book.Comments = reader["Comments"].ToString();
                            book.Image = reader["Imagee"].ToString();
                            book.Pages = reader["Pages"].ToString();
                            book.Stock = Convert.ToInt32(reader["Stock"]);
                            book.Description = reader["Descript"].ToString();
                        }
                    }

                }

            }
            return book;
        }
        public static ActionResult<User> GetUser(int id)
        {
            User user = new User();
            string query = "SELECT * FROM USERS WHERE Id = @id";
            user.Id = 0;
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
                            user.Id = Convert.ToInt32(reader["Id"]);
                            user.Email = reader["Email"].ToString();
                            user.Name = reader["Namee"].ToString();
                            user.Surname = reader["Surname"].ToString();
                            user.Phone = reader["Phone"].ToString();
                            user.Confirmed = Convert.ToBoolean(reader["Confirmed"]);
                        }
                    }
                }
            }
            return user;
        }
        public static List<Order> GetOrders(int userid)
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
                                OrderDate = Convert.ToDateTime(reader["OrderDate"])
                            }
                            );
                        }
                    }

                }
            }
            return orderlist;
        }
        public static List<Coupon> GetCoupons()
        {
            List<Coupon> couponlist = new List<Coupon>();
            string query = "SELECT * FROM Coupons WHERE Stock > 0 OR Code LIKE 'HOSGELDIN'";
            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            couponlist.Add(new Coupon
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Code = reader["Code"].ToString(),
                                DiscountAmount = Convert.ToDecimal(reader["DiscountAmount"]),
                                MinAmount = Convert.ToDecimal(reader["MinAmount"]),
                                Stock = Convert.ToInt32(reader["Stock"])
                            }
                            );
                        }
                    }

                }
            }
            return couponlist;
        }
        public static List<UserCoupon> GetUserCoupons(int id)
        {
            List<UserCoupon> couponlist = new List<UserCoupon>();
            string query = "SELECT * FROM UserCoupon Where UserId = @id";
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
                            couponlist.Add(new UserCoupon
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                UserId = Convert.ToInt32(reader["UserId"]),
                                CouponId = Convert.ToInt32(reader["CouponId"])
                            }
                            );
                        }
                    }

                }
            }
            return couponlist;
        }
        public static bool isReviewable(int bookid, int userid)
        {
            OrderItem orderItem = new OrderItem();
            orderItem.Id = 0;
            string query = "Select * from OrderItem Where BookId = @id And OrderId In(Select Id From Orders Where UserId = @userId)";
            using(SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", bookid);
                    command.Parameters.AddWithValue("@userId", userid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orderItem.Id = Convert.ToInt32(reader["Id"]);
                            orderItem.BookId= Convert.ToInt32(reader["BookId"]);
                            orderItem.OrderId= Convert.ToInt32(reader["OrderId"]);
                            orderItem.Quantity= Convert.ToInt32(reader["Quantity"]);
                            orderItem.Price= Convert.ToDecimal(reader["Price"]);
                        }
                    }
                }
            }
            if (orderItem.Id == 0) return false;
            else return true;
        }
    }
}
