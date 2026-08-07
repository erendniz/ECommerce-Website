using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Neighbourhood { get; set; }
        public string Street { get; set; }
        public string Description { get; set; }
        public string BuildingNumber { get; set; }
        public string DoorNumber { get; set; }
        public int UserId { get; set; }
    }
}
