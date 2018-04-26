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
    public class ProductsController : ApiController
    {
        private FoolDataLocal db = new FoolDataLocal();

        /// <summary>
        /// This request returns a list of all Products.
        /// </summary>
        /// <returns></returns>
        // GET: api/Products
        public IQueryable<ProductResponse> GetProducts()
        {
            IQueryable<Product> Products = db.Products;
            List<ProductResponse> result = new List<ProductResponse>();

            foreach (Product product in Products)
            {
                result.Add(new ProductResponse(product.ProductId, product.Name, product.Brand1.BrandName, product.Status1.StatusName, product.Term));
            }

            return result.AsQueryable();
        }

        /// <summary>
        /// This request will return a list of Offers associated with a given product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(OfferResponse))]
        [Route("api/products/{id}/offers")]
        //public List<OfferWebResult> GetOffersByProduct(int id)
        public async Task<IHttpActionResult> GetOffersByProduct(int id)
        {
            /*
             * Lutz Note:
             *  We could filter out by the Status and only show Active offers, but I'm making the decision
             *  (for now) to leave that up to the Front-End Developer. They might want to use it in a way
             *  where they can show the end user "Look at this Offer! The last one was two years ago! It
             *  won't last long, don't miss your chance!
             */
            Product product = db.Products.Find(id);

            //Error Handling.
            if (product == null) { return NotFound(); }

            List<OfferResponse> result = new List<OfferResponse>();
            foreach(Offer offer in product.Offers)
            {
                result.Add(new OfferResponse(offer.OfferId, offer.ProductId, offer.Description, offer.Price, offer.Status1.StatusName));
            }

            return Ok(result);
        }

        /// <summary>
        /// This request returns details for a specific product.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Products/5
        [ResponseType(typeof(ProductResponse))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            
            //Error Handling
            if (product == null) { return NotFound(); }

            ProductResponse result = new ProductResponse(product.ProductId, product.Name, product.Brand1.BrandName, product.Status1.StatusName, product.Term);
            return Ok(result);
        }

        /// <summary>
        /// This request allows updating of a Product. The response to this request is an
        /// object containing the updated Product record. The Status and Brand fields must be the
        /// ID of the new Status or Brand. The Term must be either "monthly" or "annually."
        /// All fields are optional.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Products/5
        [ResponseType(typeof(ProductResponse))]
        public async Task<IHttpActionResult> PutProduct(int id, ProductRequest request)
        {
            Product existingProduct = await db.Products.FindAsync(id);

            //Error Handling.
            if (existingProduct == null) { return NotFound(); }
            if (request.ProductId != existingProduct.ProductId) { return BadRequest("The ProductId in the request body is not equal to the ID in the URL."); }
            if (request.Term != "annually" && request.Term != "monthly") { return BadRequest("The Term in the request body must be either monthly or annually."); }
            if (db.Brands.Find(request.Brand) == null) { return BadRequest("No Brand exists with that BrandId."); }
            if (db.Status.Find(request.Status) == null) { return BadRequest("No Status exists with that StatusId."); }
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.Name != null) { existingProduct.Name = request.Name; }
            if (request.Brand != 0) { existingProduct.Brand = request.Brand; }
            if (request.Status != 0) { existingProduct.Status = request.Status; }
            if (request.Term != null && (request.Term == "annually" || request.Term == "monthly")) { existingProduct.Term = request.Term;  }
            existingProduct.DateModified = DateTime.Now;

            db.Entry(existingProduct).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ProductResponse result = new ProductResponse(existingProduct.ProductId, existingProduct.Name, db.Brands.Find(existingProduct.Brand).BrandName, db.Status.Find(existingProduct.Status).StatusName, existingProduct.Term);
            return Ok(result);
        }

        /// <summary>
        /// This request creates a new Product. The response contains the newly created Product object.
        /// The Product will be assigned a status of Active by default. You must provide a ProductId
        /// and Status of 0. The Term must be either "monthly" or "annually."
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Products
        [ResponseType(typeof(ProductResponse))]
        public async Task<IHttpActionResult> PostProduct(ProductRequest request)
        {
            if (request.ProductId != 0) { return BadRequest("The ProductId in the request must be 0."); }
            if (request.Status != 0) { return BadRequest("The Status in the request must be equal to 0."); }
            if (request.Term != "monthly" && request.Term != "annually") { return BadRequest("The Term must be either monthly or annually."); }

            Product product = new Product(request.Name, request.Brand, request.Status, request.Term);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            db.Products.Add(product);
            await db.SaveChangesAsync();

            ProductResponse result = new ProductResponse(product.ProductId, product.Name, db.Brands.Find(product.Brand).BrandName, db.Status.Find(product.Status).StatusName, product.Term);

            return Ok(result);
        }

        /// <summary>
        /// This request allows you to update a Product's status to Deleted.
        /// A successful request will return an empty response.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Products/5
        [ResponseType(typeof(ProductResponse))]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.Status = db.Status.First(a => a.StatusName == "Deleted").id;
            product.DateModified = DateTime.Now;
            db.Entry(product).State = EntityState.Modified;

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

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}