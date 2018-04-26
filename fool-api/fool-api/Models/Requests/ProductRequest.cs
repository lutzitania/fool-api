using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models.Requests
{
    public class ProductRequest
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Brand { get; set; }
        public int Status { get; set; }
        public string Term { get; set; }

        public ProductRequest(int productid, string name, int brand, int status, string term)
        {
            this.ProductId = productid;
            this.Name = name;
            this.Brand = brand;
            this.Status = status;
            this.Term = term;
        }
    }
}