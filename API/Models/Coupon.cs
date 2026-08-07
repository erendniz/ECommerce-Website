namespace API.Models
{
    public class Coupon
    {
       public int Id { get; set; } = 0;
         
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public decimal MinAmount { get; set; }
        public int Stock { get; set; }
    }
}
