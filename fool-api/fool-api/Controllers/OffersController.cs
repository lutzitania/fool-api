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
    public class OffersController : ApiController
    {
        private FoolDataLocal db = new FoolDataLocal();

        /// <summary>
        /// This request returns all Offers with an Active status.
        /// </summary>
        /// <returns></returns>
        // GET: api/Offers
        public IQueryable<OfferResponse> GetOffers()
        {
            IQueryable<Offer> offers = db.Offers.Where(a => a.Status1.StatusName == "Active");
            List<OfferResponse> results = new List<OfferResponse>();
            foreach (Offer offer in offers)
            {
                results.Add(new OfferResponse(offer.OfferId, offer.ProductId, offer.Description, offer.Price, offer.Status1.StatusName));
            }

            return results.AsQueryable();
        }

        /// <summary>
        /// This request returns details for a specific Offer. Note that this returns details regardless of Offer order;
        /// this means this request can be safely used where it's necessary to display expired Offers (such as in an Order History page).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Offers/5
        [ResponseType(typeof(OfferResponse))]
        public async Task<IHttpActionResult> GetOffer(int id)
        {
            Offer offer = await db.Offers.FindAsync(id);

            //Error Handling.
            if (offer == null) { return NotFound(); }

            OfferResponse result = new OfferResponse(offer.OfferId, offer.ProductId, offer.Description, offer.Price, offer.Status1.StatusName);

            return Ok(result);
        }

        /// <summary>
        /// This request allows you to modify an Offer. The ProductId cannot change and you must submit the ProductId in the request.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        // PUT: api/Offers/5
        [ResponseType(typeof(OfferResponse))]
        public async Task<IHttpActionResult> PutOffer(int id, OfferRequest request)
        {
            Offer existingOffer = db.Offers.Find(id);

            //Error Handling.
            if (existingOffer == null) { return NotFound(); }
            if (request.OfferId != existingOffer.OfferId) { return BadRequest("The OfferId in the request body does not match the OfferId in the URL."); }
            if (request.ProductId != existingOffer.ProductId) { return BadRequest("You cannot change the ProductId of an Offer. If you really need to do that, delete the Offer and create a new one."); }

            if (request.Description != null) { existingOffer.Description = request.Description; }
            if (request.Price != 0) { existingOffer.Price = request.Price; }
            if (request.NumberOfTerms != 0) { existingOffer.NumberOfTerms = request.NumberOfTerms; }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(existingOffer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            OfferResponse result = new OfferResponse(existingOffer.OfferId, existingOffer.ProductId, existingOffer.Description, existingOffer.Price, existingOffer.Status1.StatusName);

            return Ok(result);
        }

        /// <summary>
        /// This method allows you to create a new Offer. A ProductId must be supplied 
        /// in the request. The Offer will be created with a default status of Active.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        // POST: api/Offers
        [ResponseType(typeof(OfferResponse))]
        public async Task<IHttpActionResult> PostOffer(OfferRequest request)
        {
            //Error Handling.
            if (request.OfferId != 0) { return BadRequest("OfferId must be 0."); }
            if (request.ProductId == 0) { return BadRequest("ProductId cannot be 0."); }
            if (request.NumberOfTerms == 0) { return BadRequest("The Number of Terms can't be equal to 0."); }

            Offer offer = new Offer(request.ProductId, request.Description, request.Price, request.NumberOfTerms);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Offers.Add(offer);
            await db.SaveChangesAsync();

            OfferResponse result = new OfferResponse(offer.OfferId, offer.ProductId, offer.Description, offer.Price, db.Status.Find(offer.Status).StatusName);

            return Ok(result);
        }

        /// <summary>
        /// This request allows you to update an Offer's status to Deleted.
        /// A successful request will return an empty response.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Offers/5
        [ResponseType(typeof(Offer))]
        public async Task<IHttpActionResult> DeleteOffer(int id)
        {
            Offer offer = await db.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }

            offer.Status = db.Status.First(a => a.StatusName == "Deleted").id;
            offer.DateModified = DateTime.Now;
            db.Entry(offer).State = EntityState.Modified;

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

        private bool OfferExists(int id)
        {
            return db.Offers.Count(e => e.OfferId == id) > 0;
        }
    }
}