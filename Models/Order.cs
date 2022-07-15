using System;
using System.Collections.Generic;

namespace Swagger.Clone.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public int OrderId { get; set; }
        public float OrderFloat { get; set; }
        public long OrderLong { get; set; }
        public double OrderDouble { get; set; }
        public bool OrderBool { get; set; }
        public char OrderChar { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }

        public Address Address { get; set; }
        public DateTime ProductDateTime { get; set; }

        public List<string> ProductNames { get; set; }

        public List<User> Users { get; set; }
    }
}
