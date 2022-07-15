namespace Swagger.Clone.Models
{
    public class Address
    {
        public Guid Id { get; set; }
        public string City { get; set; }

        public int RegionId { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }
    }
}
