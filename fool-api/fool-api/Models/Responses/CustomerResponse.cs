using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models
{
    public class CustomerResponse
    {
        public int CustomerId { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public List<int> OrderIDs { get; set; }

        public CustomerResponse(int customerid, string emailaddress, string firstname, string lastname, string status, List<int> orderids)
        {
            this.CustomerId = customerid;
            this.EmailAddress = emailaddress;
            this.FirstName = firstname;
            this.LastName = lastname;
            this.Status = status;
            this.OrderIDs = orderids;
        }
    }
}