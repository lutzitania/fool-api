//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace fool_api.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Offer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Offer()
        {
            this.Orders = new HashSet<Order>();
        }

        public Offer(int productid, string description, decimal price, int numberofterms)
        {
            this.ProductId = productid;
            this.Description = description;
            this.Price = price;
            this.NumberOfTerms = numberofterms;
            this.DateCreated = DateTime.Now;
            this.DateModified = DateTime.Now;
            this.Status = 1;
            this.Orders = new HashSet<Order>();
        }

        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public System.DateTime DateCreated { get; set; }
        public Nullable<int> NumberOfTerms { get; set; }
        public System.DateTime DateModified { get; set; }
        public int Status { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual Status Status1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}