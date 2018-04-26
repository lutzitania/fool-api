using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using fool_api.Models;
using fool_api.Models.Requests;


namespace fool_api.Controllers
{
    public class CustomersController : ApiController
    {
        private FoolDataLocal db = new FoolDataLocal();

        /* Lutz Note:
         *  The Get All Customers method has been intentionally removed,
         *  as I can't imagine a use case where it would need to be displayed
         *  in the front-end (outside of an administrative capacity, which we'd
         *  tackle elsewhere).
         */

        /// <summary>
        /// Get Customer detail by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Customers/5
        [ResponseType(typeof(CustomerResponse))]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);

            //Error Handling.
            if (customer == null) { return NotFound(); }

            List<int> orderids = new List<int>();
            foreach (Order order in customer.Orders)
            {
                orderids.Add(order.OrderId);
            }

            CustomerResponse result = new CustomerResponse(customer.CustomerId, customer.EmailAddress, customer.FirstName, customer.LastName, customer.Status1.StatusName, orderids);
            return Ok(result);
        }

        /// <summary>
        /// This request will return a list of Orders placed by the Customer. Customer Details are not included in the response to this request.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/customers/{id}/orders")]
        [ResponseType(typeof(OrderResponse))]
        public async Task<IHttpActionResult> GetOrdersByCustomer(int id)
        {
            Customer customer = db.Customers.Find(id);

            //Error Handling.
            if (customer == null) { return NotFound(); }

            List<OrderResponse> result = new List<OrderResponse>();
            foreach (Order order in customer.Orders)
            {
                result.Add(new OrderResponse(order.OrderId, order.OfferId, order.CustomerId, order.Status1.StatusName, order.PurchasePrice, order.PurchaseDate, order.ProductId));
            }
            return Ok(result);
        }

        /// <summary>
        /// Use this method to update a Customer record. You may only update the Customer's FirstName, LastName, or EmailAddress. Each of these fields is optional in the request.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Customers/5
        [ResponseType(typeof(CustomerResponse))]
        public async Task<IHttpActionResult> PutCustomer(int id, CustomerRequest request)
        {
            Customer existingCustomer = await db.Customers.FindAsync(id);

            //Error Handling.
            if (existingCustomer == null) { return NotFound(); }
            if (existingCustomer.CustomerId != request.CustomerId) { return BadRequest("The CustomerId in the request body does not match the CustomerId in the URL."); }
            
            if (existingCustomer.FirstName != null) { existingCustomer.FirstName = request.FirstName; }
            if (existingCustomer.LastName != null) { existingCustomer.LastName = request.LastName; }
            if (existingCustomer.EmailAddress != null) { existingCustomer.EmailAddress = request.EmailAddress; }
            if (!ModelState.IsValid) { return BadRequest(ModelState); }


            db.Entry(existingCustomer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            List<int> orderids = new List<int>();
            foreach (Order order in existingCustomer.Orders)
            {
                orderids.Add(order.OrderId);
            }

            CustomerResponse result = new CustomerResponse(existingCustomer.CustomerId, existingCustomer.EmailAddress, existingCustomer.FirstName, existingCustomer.LastName, existingCustomer.Status1.StatusName, orderids);

            return Ok(result);
        }

        /// <summary>
        /// This action creates a new Customer. The Customer will have their status automatically set to "Active." The CustomerId must be set to 0 in the request to successfully create a new Customer.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Customers
        [ResponseType(typeof(CustomerResponse))]
        public async Task<IHttpActionResult> PostCustomer(CustomerRequest request)
        {
            //Error Handling.
            if (request.CustomerId != 0) { return BadRequest("The CustomerId in the request body must be equal to 0."); }

            Customer customer = new Customer(request.FirstName, request.LastName, request.EmailAddress);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            db.Customers.Add(customer);
            await db.SaveChangesAsync();

            /*
             * Lutz Note:
             *  As written, this will always be an empty LinkedList, but leaving the
             *  scaffolded method here may allow for a future system enhancement where
             *  a user is created at the same time an order is placed. (e.g. "Checkout
             *  as Guest" versus "Create an Account for Pirate Points!") [And who doesn't
             *  want Pirate Points? I mean, I would.]
             */
            List<int> orderids = new List<int>();
            foreach (Order order in customer.Orders)
            {
                orderids.Add(order.OrderId);
            }

            CustomerResponse result = new CustomerResponse(customer.CustomerId, customer.EmailAddress, customer.FirstName, customer.LastName, db.Status.Find(customer.Status).StatusName, orderids);
            return Ok(result);
        }

        /// <summary>
        /// This method will update the Customer Status to "Deleted." An empty response to this request indicates success.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Customers/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> DeleteCustomer(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);

            //Error Handling.
            if (customer == null) { return NotFound(); }

            customer.Status = db.Status.First(a => a.StatusName == "Deleted").id;
            customer.DateModified = DateTime.Now;
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(int id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}