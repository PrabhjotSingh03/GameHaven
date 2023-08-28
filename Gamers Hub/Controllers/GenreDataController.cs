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
    public class GenreDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// Return all Genres in the system
        /// GET: api/GenreData/ListGenre
        [HttpGet]
        [ResponseType(typeof(GenreDto))]
        public IHttpActionResult ListGenre()
        {
            List<Genre> Genres = db.Genres.ToList();
            List<GenreDto> GenreDtos = new List<GenreDto>();

            Genres.ForEach(s => GenreDtos.Add(new GenreDto()
            {
                GenreID = s.GenreID,
                GenreName = s.GenreName,
            }));

            return Ok(GenreDtos);
        }

        /// Return all Genres in the system.
        /// <param name="id">The primary key of the Genre</param>
        /// GET: api/GenreData/FindGenre/5
        [HttpGet]
        [ResponseType(typeof(GenreDto))]
        public IHttpActionResult FindGenre(int id)
        {
            Genre Genre = db.Genres.Find(id);
            GenreDto GenreDto = new GenreDto()
            {
                GenreID = Genre.GenreID,
                GenreName = Genre.GenreName,
            };
            if (Genre == null)
            {
                return NotFound();
            }

            return Ok(GenreDto);
        }

        /// Update a particular Genre in the system via POST
        /// <param name="id">Genre ID primary key</param>
        /// <param name="Genre">Genre json format data</param>
        /// PUT: api/GenreData/UpdateGenre/5
        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateGenre(int id, Genre Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Genre.GenreID)
            {
                return BadRequest();
            }

            db.Entry(Genre).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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

        /// Add an new Genre to the system via POST
        /// <param name="Genre">Genre json format data</param>
        /// POST: api/GenreData/AddGenre
        [HttpPost]
        [ResponseType(typeof(Genre))]
        public IHttpActionResult AddGenre(Genre Genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Genres.Add(Genre);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Genre.GenreID }, Genre);
        }

        /// Delete a Genre from the system via POST
        /// <param name="id">primary key of the Genre</param>
        /// api/GenreData/DeleteGenre/5
        [ResponseType(typeof(Genre))]
        [HttpPost]
        public IHttpActionResult DeleteGenre(int id)
        {
            Genre Genre = db.Genres.Find(id);
            if (Genre == null)
            {
                return NotFound();
            }

            db.Genres.Remove(Genre);
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

        private bool GenreExists(int id)
        {
            return db.Genres.Count(e => e.GenreID == id) > 0;
        }
    }
}