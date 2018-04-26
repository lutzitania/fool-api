using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models.Requests
{
    public class BrandRequest
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }

        public BrandRequest(int brandid, string brandname)
        {
            this.BrandId = brandid;
            this.BrandName = brandname;
        }
    }
}