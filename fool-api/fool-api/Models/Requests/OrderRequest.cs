using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models.Requests
{
    public class OrderRequest
    {
        public int OrderId { get; set; }
        public int OfferId { get; set; }
        public int CustomerId { get; set; }
        public int Status { get; set; }
        public decimal PurchasePrice { get; set; }

        public OrderRequest(int orderid, int offerid, int customerid, int status, decimal purchaseprice)
        {
            this.OrderId = orderid;
            this.OfferId = offerid;
            this.CustomerId = customerid;
            this.Status = status;
            this.PurchasePrice = purchaseprice;
        }
    }
}