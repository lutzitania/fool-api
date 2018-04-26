using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models.Requests
{
    public class OfferRequest
    {
        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int NumberOfTerms { get; set; }

        public OfferRequest(int offerid, int productid, string description, decimal price, int numberofterms)
        {
            this.OfferId = offerid;
            this.ProductId = productid;
            this.Description = description;
            this.Price = price;
            this.NumberOfTerms = numberofterms;
        }
    }
}