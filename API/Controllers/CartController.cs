using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CartController : Controller
    {
        [HttpGet("checkout")]
        public async Task <IActionResult> Index()
        {
            bool BasketAction = string.IsNullOrEmpty(HttpContext.Session.GetString("BasketAction")) ? true : false;
            HttpContext.Session.Remove("BasketAction");
            string? user = HttpContext.Session.GetString("Name");
            if(user == null)
            {
                HttpContext.Session.SetString("LoginMessage", "You need to login first to see your cart.");
                return RedirectToAction("Index", "User");
            }
            var cartJson = HttpContext.Session.GetString("BasketKey");
            var cart = cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
            string stockChange = "";
            if (cart.Count == 0)
            {
                if (BasketAction) HttpContext.Session.SetString("LoginMessage", "Your basket is empty. Add some items first to see your basket.");
                return RedirectToAction("Main","ECommerce");
            }
            foreach(var item in cart)
            {
                if(item.Quantity > item.Stock)
                {
                    int oldquantity = item.Quantity;
                    int index = cart.FindIndex(i => item.Id == i.Id);
                    cart[index].Quantity = cart[index].Stock;
                    stockChange += ("\n" + item.Name +": " + oldquantity + " ---> " + item.Stock);
                }
            }
            cartJson = JsonSerializer.Serialize<List<Cart>>(cart);
            HttpContext.Session.SetString("BasketKey", cartJson);

            ViewBag.stockChange = stockChange;
            ViewBag.TotalPrice = cart.Sum(x => x.Price * x.Quantity);
            HttpContext.Session.SetString("CheckoutPrice", cart.Sum(x => x.Price * x.Quantity).ToString());
            return View(cart);
        }
        [HttpPost("removeone")]
        public async Task <IActionResult> RemoveOne([FromForm (Name ="id")] int id ,[FromForm(Name ="quantity")] int quantity)
        {
            var cartJson = HttpContext.Session.GetString("BasketKey");
            var cart = cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
            Cart item = cart.Find(c => c.Id == id);

            item.Quantity = quantity;

            cartJson = JsonSerializer.Serialize<List<Cart>>(cart);
            HttpContext.Session.SetString("BasketKey", cartJson);

            return RedirectToAction("Index");

        }

        [HttpPost("removeall")]
        public async Task<IActionResult> RemoveAll([FromForm(Name = "id")] int id)
        {
            var cartJson = HttpContext.Session.GetString("BasketKey");
            var cart = cartJson == null ? new List<Cart>() : JsonSerializer.Deserialize<List<Cart>>(cartJson);
            Cart item = cart.Find(c => c.Id == id);
            cart.Remove(item);

            cartJson = JsonSerializer.Serialize<List<Cart>>(cart);
            HttpContext.Session.SetString("BasketKey", cartJson);
            HttpContext.Session.SetString("BasketAction", "true");

            return RedirectToAction("Index");
        }

        [HttpPost("clearbasket")]
        public async Task<IActionResult> ClearBasket()
        {
            HttpContext.Session.Remove("BasketKey");
            HttpContext.Session.SetString("BasketAction", "true");
            return RedirectToAction("Index");
        }

        [HttpGet("goback")]
        public async Task<IActionResult> GoBack()
        {
            return RedirectToAction("Main","ECommerce");
        }

        [HttpGet("gopay")]
        public async Task<IActionResult> GoPay()
        {
            return RedirectToAction("Index", "Payment");
        }
    }
}
