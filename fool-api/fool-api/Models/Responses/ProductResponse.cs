using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models
{
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Status { get; set; }
        public string Term { get; set; }

        public ProductResponse(int productid, string name, string brand, string status, string term)
        {
            this.ProductId = productid;
            this.Name = name;
            this.Brand = brand;
            this.Status = status;
            this.Term = term;
        }

    }
}