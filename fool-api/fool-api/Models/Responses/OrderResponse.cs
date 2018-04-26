using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models
{
    public class OrderResponse
    {
        public int OrderId { get; set; }
        public int  OfferId { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
        public decimal? PurchasePrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public int? ProductId { get; set; }

        public OrderResponse(int orderid, int offerid, int customerid, string status, decimal? purchaseprice, DateTime? purchasedate, int? productid)
        {
            this.OrderId = orderid;
            this.OfferId = offerid;
            this.CustomerId = customerid;
            this.Status = status;
            this.PurchasePrice = purchaseprice;
            this.PurchaseDate = purchasedate;
            this.ProductId = productid;
        }
    }
}