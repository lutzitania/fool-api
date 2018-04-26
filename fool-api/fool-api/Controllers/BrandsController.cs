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
    public class BrandsController : ApiController
    {
        private FoolDataLocal db = new FoolDataLocal();

        /// <summary>
        /// This request returns a list of all brands.
        /// </summary>
        /// <returns></returns>
        // GET: api/Brands
        public IQueryable<BrandResponse> GetBrands()
        {
            IQueryable<Brand> brands = db.Brands;
            List<BrandResponse> result = new List<BrandResponse>();
            foreach(Brand brand in brands)
            {
                result.Add(new BrandResponse(brand.id, brand.BrandName));
            }

            return result.AsQueryable();
        }

        /// <summary>
        /// This request returns a specific brand.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Brands/5
        [ResponseType(typeof(BrandResponse))]
        public async Task<IHttpActionResult> GetBrand(int id)
        {
            Brand brand = await db.Brands.FindAsync(id);
            
            //Error Handling.
            if (brand == null) { return NotFound(); }

            BrandResponse result = new BrandResponse(brand.id, brand.BrandName);
            return Ok(result);
        }

        /// <summary>
        /// This request provides products associated with a given BrandId.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ProductResponse))]
        [Route("api/brands{id}/products")]
        public async Task<IHttpActionResult> GetProductsByBrand(int id)
        {
            Brand brand = db.Brands.Find(id);

            //Error Handling.
            if (brand == null) { return NotFound(); }

            List<ProductResponse> result = new List<ProductResponse>();
            foreach (Product product in brand.Products)
            {
                result.Add(new ProductResponse(product.ProductId, product.Name, product.Brand1.BrandName, product.Status1.StatusName, product.Term));
            }
            return Ok(result);
        }

        /// <summary>
        /// This request allows you to update a Brand name.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Brands/5
        [ResponseType(typeof(BrandResponse))]
        public async Task<IHttpActionResult> PutBrand(int id, BrandRequest request)
        {
            Brand existingBrand = db.Brands.Find(id);

            //Error Handling.
            if (existingBrand == null) { return NotFound(); }
            if (request.BrandId != existingBrand.id) { return BadRequest("The BrandId in the request body does not match the BrandId referenced in the URL."); }
            if (request.BrandName.Length > 2) { return BadRequest("The BrandName cannot be longer than two characters."); }

            existingBrand.BrandName = request.BrandName;
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            
            db.Entry(existingBrand).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            BrandResponse result = new BrandResponse(existingBrand.id, existingBrand.BrandName);

            return Ok(result);
        }

        /// <summary>
        /// This request allows you to create a new Brand.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Brands
        [ResponseType(typeof(BrandResponse))]
        public async Task<IHttpActionResult> PostBrand(BrandRequest request)
        {
            //Error Handling.
            if (request.BrandId != 0) { return BadRequest("The BrandId in the request body must be equal to 0."); }
            if (request.BrandName.Length > 2) { return BadRequest("The BrandName cannot be longer than two characters."); }

            Brand brand = new Brand(request.BrandId, request.BrandName);
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            
            db.Brands.Add(brand);
            await db.SaveChangesAsync();

            BrandResponse result = new BrandResponse(brand.id, brand.BrandName);

            return Ok(result);
        }

        /// <summary>
        /// This request allows you to delete a Brand.
        /// Note that the brand will be DELETED, rather than having its status updated.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Brands/5
        [ResponseType(typeof(Brand))]
        public async Task<IHttpActionResult> DeleteBrand(int id)
        {
            Brand brand = await db.Brands.FindAsync(id);
            
            //Error Handling.
            if (brand == null) { return NotFound(); }

            db.Brands.Remove(brand);
            await db.SaveChangesAsync();

            return Ok(brand);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BrandExists(int id)
        {
            return db.Brands.Count(e => e.id == id) > 0;
        }
    }
}