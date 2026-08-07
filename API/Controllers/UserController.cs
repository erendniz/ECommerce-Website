using API.Models;
using API.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace API.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : Controller
    {
        protected User user = new User();
        protected string connect = @"Server=LAPTOP-AAKP1RFL\MSSQLSERVER01;Database=Selenium;Trusted_Connection=True;TrustServerCertificate=True;";

        public ActionResult<User> SearchUser(string mail, string password, bool choice)
        {
            string querytrue = " AND Passwordd = @password";
            string query = "SELECT * FROM USERS WHERE Email = @mail";
            if (choice) query = query + querytrue;
            user.Id = 0;
            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@mail", mail);
                    if (choice) command.Parameters.AddWithValue("@password", password);
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read())
                        {
                            user.Id = Convert.ToInt32(reader["Id"]);
                            user.Email = reader["Email"].ToString();
                            user.Name = reader["Namee"].ToString();
                            user.Surname = reader["Surname"].ToString();
                            user.Password = reader["Passwordd"].ToString();
                            user.Phone = reader["Phone"].ToString();
                            user.Confirmed = Convert.ToBoolean(reader["Confirmed"]);
                        }
                    }
                }
            }
            return user;
        }
        public string AddUser(User loginuser)
        {
            string query = "INSERT INTO USERS (Namee, Surname, Email, Passwordd, Phone) VALUES (@name, @surname, @email, @password, @phone)";
            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.Add("@name", SqlDbType.NVarChar, 250).Value = loginuser.Name;
                    command.Parameters.Add("@surname", SqlDbType.NVarChar, 250).Value = loginuser.Surname;
                    command.Parameters.Add("@email", SqlDbType.NVarChar, 250).Value = loginuser.Email;
                    command.Parameters.Add("@password", SqlDbType.NVarChar, 250).Value = loginuser.Password;
                    command.Parameters.Add("@phone", SqlDbType.NVarChar, 250).Value = loginuser.Phone;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return "User has been registered successfully.";
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            ViewBag.LoginMessage = HttpContext.Session.GetString("LoginMessage");
            HttpContext.Session.Remove("LoginMessage");
            return View();
        }
        [HttpGet("register")]
        public IActionResult RegisterPage()
        {
            return View("Register");
        }
        [HttpPost("sendregister")]
        public IActionResult Register([FromForm(Name = "email")] string mail,
         [FromForm(Name = "password")] string password, [FromForm(Name = "name")] string name,
         [FromForm(Name = "surname")] string surname, [FromForm(Name = "phone")] string phone)
        {
            User controluser = new User();
            controluser = SearchUser(mail, password, false).Value;
            if (controluser.Id != 0) return View("RegisterPage");
            else
            {
                controluser.Name = name;
                controluser.Surname = surname;
                controluser.Email = mail;
                controluser.Password = password;
                controluser.Phone = phone;
            

                AddUser(controluser);
                return View("Index");
            }
        }
        [HttpPost("sendlogin")]
        public IActionResult Login([FromForm(Name = "email")] string mail,
         [FromForm(Name = "password")] string password)
        {

            User controluser = new User();
            controluser = SearchUser(mail, password, true).Value;
            if (controluser.Id == 0)
            {
                HttpContext.Session.SetString("LoginMessage", "Wrong e-mail or password. Please try again.");
                return RedirectToAction("Index");
            }
            else if (!controluser.Confirmed)
            {
                HttpContext.Session.SetString("ConfirmEmail", mail);
                return RedirectToAction("ConfirmAccountIndex");
            }
            else
            {
                HttpContext.Session.SetString("Name", controluser.Name);
                HttpContext.Session.SetString("Surname", controluser.Surname);
                HttpContext.Session.SetString("Phone", controluser.Phone);
                HttpContext.Session.SetString("Email", controluser.Email);
                HttpContext.Session.SetString("UserId", controluser.Id.ToString());
                HttpContext.Session.SetString("Confirmed", Convert.ToString(controluser.Confirmed));
                return RedirectToAction("Main", "ECommerce");
            }
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("Surname");
            HttpContext.Session.Remove("Phone");
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("Confirmed");
            HttpContext.Session.Remove("BasketKey");
            HttpContext.Session.Remove("RecentKey");
            return RedirectToAction("Main", "ECommerce");
        }
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Email"))) return RedirectToAction("Index");
            user = SearchUser(HttpContext.Session.GetString("Email"), "", false).Value;
            user.Addresses = GetAddresses().Value;
            ViewBag.Name = HttpContext.Session.GetString("Name");
            ViewBag.Surname = HttpContext.Session.GetString("Surname");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Phone = HttpContext.Session.GetString("Phone");
            return View(user);
        }
        [HttpPost("address")]
        public IActionResult Address([FromForm(Name = "frompage")] string page)
        {
            HttpContext.Session.SetString("FromPage", page);
            return View("Address");
        }
        [HttpPost("addaddress")]
        public IActionResult AddAddress([FromForm(Name = "name")] string name,
         [FromForm(Name = "city")] string city, [FromForm(Name = "district")] string district,
         [FromForm(Name = "neighbourhood")] string neighbourhood, [FromForm(Name = "street")] string street, [FromForm(Name = "buildingno")] string buildingno,
         [FromForm(Name = "doorno")] string doorno, [FromForm(Name = "desc")] string? desc)
        {
            string query = "INSERT INTO ADDRESSES (Namee, City, District, Neighbourhood, Street, BuildingNumber," +
                "DoorNumber, Descriptionn, UserId, IsDeleted) VALUES(@name, @city, @district, @neighbourhood, @street, @buildingno, @doorno, @desc, @id, 0)";

            user = SearchUser(HttpContext.Session.GetString("Email"), "", false).Value;

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.Add("@name", SqlDbType.NVarChar, 250).Value = name;
                    command.Parameters.Add("@city", SqlDbType.NVarChar, 250).Value = city;
                    command.Parameters.Add("@district", SqlDbType.NVarChar, 250).Value = district;
                    command.Parameters.Add("@neighbourhood", SqlDbType.NVarChar, 250).Value = neighbourhood;
                    command.Parameters.Add("@street", SqlDbType.NVarChar, 250).Value = street;
                    command.Parameters.Add("@buildingno", SqlDbType.NVarChar, 250).Value = buildingno;
                    command.Parameters.Add("@doorno", SqlDbType.NVarChar, 250).Value = doorno;
                    if (string.IsNullOrEmpty(desc)) command.Parameters.Add("@desc", SqlDbType.NVarChar, 250).Value = "No Description";
                    else command.Parameters.Add("@desc", SqlDbType.NVarChar, 250).Value = desc;
                    command.Parameters.Add("@id", SqlDbType.Int).Value = user.Id;

                    connection.Open();
                    command.ExecuteNonQuery();

                }
            }

            if (string.Equals(HttpContext.Session.GetString("FromPage"), "profile")) return RedirectToAction("Profile");
            else return RedirectToAction("GoPay", "Cart");
        }
        public ActionResult<List<Address>> GetAddresses()
        {
            string query = "SELECT * FROM Addresses WHERE UserId = @id AND IsDeleted = 0";
            List<Address> addresses = new List<Address>();

            using (SqlConnection connection = new SqlConnection(connect))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    user = SearchUser(HttpContext.Session.GetString("Email"), "", false).Value;
                    command.Parameters.AddWithValue("@id", user.Id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            addresses.Add(new Address
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
        [HttpPost("removeaddress")]
        public async Task<IActionResult> DeleteAddress([FromForm(Name = "id")] int id)
        {
            string query = "UPDATE Addresses SET IsDeleted = 1 WHERE Id = @id";

            using (SqlConnection connection = new SqlConnection(connect))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Profile");
        }
        [HttpGet("passwordchange")]
        public IActionResult ChangePasswordIndex()
        {
            ViewBag.PasswordMessage = HttpContext.Session.GetString("PasswordMessage");
            HttpContext.Session.Remove("PasswordMessage");
            return View("PasswordChange");
        }
        [HttpPost("changepassword")]
        public IActionResult ChangePassword([FromForm(Name = "currentpassword")] string cpassword,
            [FromForm(Name = "newpassword")] string npassword, [FromForm(Name = "againpassword")] string apassword)
        {
            string message = "";
            string query = "UPDATE Users SET Passwordd = @password Where Id=@id";
            User controluser = SearchUser(HttpContext.Session.GetString("Email"), cpassword, true).Value;

            if(controluser.Id == 0)
            {
                message = "Wrong password, please enter your current password again.";
            }
            else if (!string.Equals(npassword, apassword))
            {
                message = "Passwords don't match please check your new password and confirm password again.";
            }
            else
            {
                using(SqlConnection connection = new SqlConnection(connect))
                {
                    using(SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@password", npassword);
                        command.Parameters.AddWithValue("@id", HttpContext.Session.GetString("UserId"));
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                message = "Your Password haas been changed successfully!";
            }
            HttpContext.Session.SetString("PasswordMessage", message);
            return RedirectToAction("ChangePasswordIndex");
        }
        [HttpGet("confirm")]
        public IActionResult ConfirmAccountIndex()
        {
            ViewBag.ConfirmMessage = HttpContext.Session.GetString("ConfirmMessage");
            HttpContext.Session.Remove("ConfirmMessage");
            return View("Confirmation");
        }
        [HttpPost("sendconfirmation")]
        public IActionResult ConfirmAccount([FromForm(Name ="code")]string code)
        {
            string query = "UPDATE Users SET Confirmed = 1 Where Email=@email";
            if (string.Equals(code, "12345"))
            {
                using (SqlConnection connection = new SqlConnection(connect))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@email", HttpContext.Session.GetString("ConfirmEmail"));
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                HttpContext.Session.SetString("ConfirmMessage", "You've successfully confirmed your account!");
                HttpContext.Session.SetString("Confirmed", "true");
                HttpContext.Session.Remove("ConfirmEmail");
                return RedirectToAction("Index");
            }
            else
            {
                HttpContext.Session.SetString("ConfirmMessage", "Wrong code! Please try again...");
                return RedirectToAction("ConfirmAccountIndex");
            }
        }
        [HttpGet("showcoupons")]
        public IActionResult ShowCoupons()
        {
            List<Coupon> coupons = DbHelper.GetCoupons();
            List<Coupon> usedcoupons = new List<Coupon>();
            List<UserCoupon> userCoupons = DbHelper.GetUserCoupons(Convert.ToInt32(HttpContext.Session.GetString("UserId")));

            foreach (var usercoupon in userCoupons)
            {
                if (coupons.Any(c => c.Id == usercoupon.CouponId))
                {
                    Coupon coupon = coupons.FirstOrDefault(c => c.Id == usercoupon.CouponId);
                    usedcoupons.Add(coupon);
                    coupons.Remove(coupon);
                }
            }

            var tuple = Tuple.Create(coupons, usedcoupons);

            return View("Coupons", tuple);
        }
    }
}
