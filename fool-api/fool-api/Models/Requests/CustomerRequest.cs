using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fool_api.Models
{
    public class CustomerRequest
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public CustomerRequest(int customerid, string firstname, string lastname, string emailaddress)
        {
            this.CustomerId = customerid;
            this.FirstName = firstname;
            this.LastName = lastname;
            this.EmailAddress = emailaddress;
        }
    }
}