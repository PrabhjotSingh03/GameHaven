using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Gamers_Hub.Models;
using System.Diagnostics;

namespace Gamers_Hub.Controllers
{
    public class BuyerDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Return all Buyers in the system
        /// </summary>
        /// <returns>
        /// all Buyers in the database, including their associated games 
        /// </returns>
        /// GET: api/BuyerData/ListBuyers
        [HttpGet]
        [ResponseType(typeof(BuyerDto))]
        public IHttpActionResult ListBuyers()
        {
            List<Buyer> Buyers = db.Buyers.ToList();
            List<BuyerDto> BuyerDtos = new List<BuyerDto>();

            Buyers.ForEach(k => BuyerDtos.Add(new BuyerDto()
            {
                BuyerID = k.BuyerID,
                BuyerName = k.BuyerName,
                BuyerEmail = k.BuyerEmail
            }));

            return Ok(BuyerDtos);
        }

        /// <summary>
        /// Return all Buyers in the system associated with particular games
        /// </summary>
        /// <returns> M--M relationship
        /// </returns>
        /// <param name="id">Game Primary Key</param>
        /// GET: api/BuyerData/ListBuyersForGame/5
        [HttpGet]
        [ResponseType(typeof(BuyerDto))]
        public IHttpActionResult ListBuyersForGame(int id)
        {
            List<Buyer> Buyers = db.Buyers.Where(
                k => k.Games.Any(
                    a => a.GameID == id)
                ).ToList();
            List<BuyerDto> BuyerDtos = new List<BuyerDto>();

            Buyers.ForEach(k => BuyerDtos.Add(new BuyerDto()
            {
                BuyerID = k.BuyerID,
                BuyerName = k.BuyerName,
                BuyerEmail = k.BuyerEmail
            }));

            return Ok(BuyerDtos);
        }

        /// <summary>
        /// Return buyers that are not associated with a particular game id
        /// </summary>
        /// <param name="id">Game Primary Key</param>
        /// GET: api/BuyerData/ListBuyersNotInterestedGame/5
        [HttpGet]
        [ResponseType(typeof(BuyerDto))]
        public IHttpActionResult ListBuyersNotInterestedGame(int id)
        {
            List<Buyer> Buyers = db.Buyers.Where(
                k => !k.Games.Any(
                    a => a.GameID == id)
                ).ToList();
            List<BuyerDto> BuyerDtos = new List<BuyerDto>();

            Buyers.ForEach(k => BuyerDtos.Add(new BuyerDto()
            {
                BuyerID = k.BuyerID,
                BuyerName = k.BuyerName,
                BuyerEmail = k.BuyerEmail
            }));

            return Ok(BuyerDtos);
        }

        /// <summary>
        /// Return a specific Buyer in the system
        /// </summary>
        /// <param name="id">Buyer primary key</param>
        /// GET: api/BuyerData/FindBuyer/5
        [HttpGet]
        [ResponseType(typeof(BuyerDto))]
        public IHttpActionResult FindBuyer(int id)
        {
            Buyer Buyer = db.Buyers.Find(id);
            BuyerDto BuyerDto = new BuyerDto()
            {
                BuyerID = Buyer.BuyerID,
                BuyerName = Buyer.BuyerName,
                BuyerEmail = Buyer.BuyerEmail
            };
            if (Buyer == null)
            {
                return NotFound();
            }

            return Ok(BuyerDto);
        }

        /// <summary>
        /// Update a particular buyer via POST data
        /// </summary>
        /// <param name="id">buyer ID primary key</param>
        /// <param name="Buyer">buyer json format data</param>
        /// api/BuyerData/UpdateBuyer/5
        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateBuyer(int id, Buyer Buyer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Buyer.BuyerID)
            {
                return BadRequest();
            }

            db.Entry(Buyer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BuyerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Add a new buyer to the system via POST
        /// </summary>
        /// <param name="Buyer">buyer json format data</param>
        /// api/BuyerData/AddBuyer
        
        [ResponseType(typeof(Buyer))]
        [HttpPost]
        public IHttpActionResult AddBuyer(Buyer Buyer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Buyers.Add(Buyer);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Buyer.BuyerID }, Buyer);
        }

        /// <summary>
        /// Delete an buyer from the system
        /// </summary>
        /// <param name="id">buyer primary key</param>
        /// <returns>
        /// DELETE: api/BuyerData/DeleteBuyer/5
        [HttpPost]
        [ResponseType(typeof(Buyer))]
        public IHttpActionResult DeleteBuyer(int id)
        {
            Buyer Buyer = db.Buyers.Find(id);
            if (Buyer == null)
            {
                return NotFound();
            }

            db.Buyers.Remove(Buyer);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BuyerExists(int id)
        {
            return db.Buyers.Count(e => e.BuyerID == id) > 0;
        }
    }
}