using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models
{
    public class OfferResponse
    {
        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }

        public OfferResponse(int offerid, int productid, string description, decimal price, string status)
        {
            this.OfferId = offerid;
            this.ProductId = productid;
            this.Description = description;
            this.Price = price;
            this.Status = status;
        }
    }
}