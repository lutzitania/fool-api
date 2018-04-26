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
    public class OrdersController : ApiController
    {
        private FoolDataLocal db = new FoolDataLocal();

        /// <summary>
        /// This request returns details for a specific order.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Orders/5
        [ResponseType(typeof(OrderResponse))]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            OrderResponse result = new OrderResponse(order.OrderId, order.OfferId, order.CustomerId, order.Status1.StatusName, order.PurchasePrice, order.PurchaseDate, order.ProductId);
            return Ok(result);
        }

        /// <summary>
        /// This request allows you to update an Order. Note that the Status field is the ONLY
        /// thing you may change for an existing Order. All other request parameters will be ignored.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Orders/5
        [ResponseType(typeof(OrderResponse))]
        public async Task<IHttpActionResult> PutOrder(int id, OrderRequest request)
        {
            Order existingOrder = db.Orders.Find(id);

            //Error Handling.
            if (existingOrder == null) { return NotFound(); }
            if (request.OrderId != existingOrder.OrderId) { return BadRequest("The OrderId provided in the Request Body does not match the OrderId provided in the URL."); }
            if (request.Status != 0) { existingOrder.Status = request.Status; }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(existingOrder).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            OrderResponse result = new OrderResponse(existingOrder.OrderId, existingOrder.OfferId, existingOrder.CustomerId, db.Status.Find(existingOrder.Status).StatusName, existingOrder.PurchasePrice, existingOrder.PurchaseDate, existingOrder.ProductId);
            return Ok(result);
        }

        /// <summary>
        /// This request allows you to create a new Order. The response will contain the new Order.
        /// The OrderId and Status in the request body must be 0.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Orders
        [ResponseType(typeof(OrderResponse))]
        public async Task<IHttpActionResult> PostOrder(OrderRequest request)
        {
            //Error handling.
            if (request.OrderId != 0) { return BadRequest("The OrderId in the request body must be equal to 0."); }
            if (request.Status != 0) { return BadRequest("The Status in the request body must be equal to 0."); }
            if (db.Offers.Find(request.OfferId) == null) { return BadRequest("No Offer exists with that OfferId."); }
            if (db.Offers.Find(request.OfferId).Status1.StatusName != "Active") { return BadRequest("This offer is no longer available.."); }

            /* Lutz Note:
             *  Since the schema has Offer dates directly in the Order table (which we should
             *  resolve later), we need to accurately fill out this information instead of
             *  inserting placeholders. The code below duplicates the logic I wrote in the SQL
             *  query to update the Status column in the Offer table.
             */
            DateTime StartDate = db.Offers.Find(request.OfferId).DateCreated;
            DateTime EndDate;
            string Term = db.Offers.Find(request.OfferId).Product.Term;
            int NumberOfTerms = db.Offers.Find(request.OfferId).NumberOfTerms.Value;
            if (Term == "monthly") { EndDate = StartDate.AddMonths(NumberOfTerms); }
            else if (Term == "annually") { EndDate = StartDate.AddYears(NumberOfTerms); }
            else { EndDate = StartDate.AddMonths(1); } //We'll assume we're buying subscriptions, and the default subscription period is one month.

            Order order = new Order(request.OrderId, request.OfferId, request.CustomerId, 2, request.PurchasePrice, StartDate, EndDate, DateTime.Now, db.Offers.Find(request.OfferId).ProductId);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            OrderResponse result = new OrderResponse(order.OrderId, order.OfferId, order.CustomerId, db.Status.Find(order.Status).StatusName, order.PurchasePrice, order.PurchaseDate, order.ProductId);

            return Ok(result);
        }

        /// <summary>
        /// This request will update an Order's staus to 0.
        /// A successfull request will return an empty response.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            
            //Error Handling.
            if (order == null) { return NotFound(); }

            order.Status = db.Status.First(a => a.StatusName == "Deleted").id;
            db.Entry(order).State = EntityState.Modified;

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

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }
}