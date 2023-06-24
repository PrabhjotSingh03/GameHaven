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
using System.IO;    // needed for updating pictures 
using System.Web;

namespace Gamers_Hub.Controllers
{
    public class GameDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        /// <returns> all games in the database</returns>
        /// GET: api/GameData/ListGames
        [HttpGet]
        [ResponseType(typeof(GameDto))]
        public IHttpActionResult ListGames()
        {
            List<Game> Games = db.Games.ToList();
            List<GameDto> GameDtos = new List<GameDto>();

            // loop through the database game table to get all information
            Games.ForEach(a => GameDtos.Add(new GameDto()
            {
                GameID = a.GameID,
                GameName = a.GameName,
                ReleaseYear = a.ReleaseYear,
                Description = a.Description,
                Price = a.Price,
                GenreID = a.Genre.GenreID,
                GenreName = a.Genre.GenreName,
                GameHasPic = a.GameHasPic,
                PicExtension = a.PicExtension,
            }));
            return Ok(GameDtos);
        }

        /// Get information about all games related to a particular genre ID
        /// For simple design, one game only has one genre type, but one genre can have many games.  1--M relationship
        /// <returns>
        ///  all games in the database, including their associated genre matched with a particular genre ID
        /// </returns>
        /// <param name="id">Genre ID.</param>
        /// api/GameData/ListgamesForGenre/2
        [HttpGet]
        [ResponseType(typeof(GameDto))]
        public IHttpActionResult ListGamesForGenre(int id)
        {
            List<Game> Games = db.Games.Where(a => a.GenreID == id).ToList();
            List<GameDto> GameDtos = new List<GameDto>();

            Games.ForEach(a => GameDtos.Add(new GameDto()
            {
                GameID = a.GameID,
                GameName = a.GameName,
                ReleaseYear = a.ReleaseYear,
                Description = a.Description,
                Price = a.Price,
                GenreID = a.Genre.GenreID,
                GenreName = a.Genre.GenreName,
                GameHasPic = a.GameHasPic,
                PicExtension = a.PicExtension,
            }));

            return Ok(GameDtos);
        }

        /// Get information about games related to a particular buyer
        /// <returns>
        /// all games in the database, including their associated Genre that match to a particular buyerid
        /// </returns>
        /// <param name="id">buyer ID.</param>
        /// GET: api/MovieData/ListGamesForBuyer/3
        /// Click on buyer name will direct to the '/buyer/details' page, which lists this buyer with associated games
        [HttpGet]
        [ResponseType(typeof(GameDto))]
        public IHttpActionResult ListGamesForBuyer(int id)
        {
            //all games that have buyers which match with ID
            List<Game> Games = db.Games.Where(
                a => a.Buyers.Any(
                    k => k.BuyerID == id)).ToList();
            List<GameDto> GameDtos = new List<GameDto>();

            Games.ForEach(a => GameDtos.Add(new GameDto()
            {
                GameID = a.GameID,
                GameName = a.GameName,
                ReleaseYear = a.ReleaseYear,
                Description = a.Description,
                Price = a.Price,
                GenreID = a.Genre.GenreID,
                GenreName = a.Genre.GenreName
            }));

            return Ok(GameDtos);
        }

        /// Associates a particular buyer with a particular game
        /// <param name="Gameid">The game ID primary key</param>
        /// <param name="Buyerid">The buyer ID primary key</param>
        /// POST api/GameData/AssociateGameWithBuyer/9/3
        [HttpPost]
        [Route("api/GameData/AssociateGameWithBuyer/{Gameid}/{Buyerid}")]
        public IHttpActionResult AssociateGameWithBuyer(int Gameid, int Buyerid)
        {

            Game SelectedGame = db.Games.Include(a => a.Buyers).Where(a => a.GameID == Gameid).FirstOrDefault();
            Buyer SelectedBuyer = db.Buyers.Find(Buyerid);

            if (SelectedGame == null || SelectedBuyer == null)
            {
                return NotFound();
            }

            //Debug.WriteLine("game id: " + Gameid);
            //Debug.WriteLine("Selected Game: " + SelectedGame.GameTitle);

            //Debug.WriteLine("Buyer id: " + Buyerid);
            //Debug.WriteLine("Selected Buyer: " + SelectedBuyer.BuyerName);

            SelectedGame.Buyers.Add(SelectedBuyer);
            db.SaveChanges();

            return Ok();
        }

        /// Unlink a particular buyer from a game
        /// <param name="Gameid">The game ID primary key</param>
        /// <param name="Buyerid">The Buyer ID primary key</param>
        /// POST api/GameData/AssociateGameWithBuyer/9/5
        [HttpPost]
        [Route("api/MovieData/UnAssociateGameWithBuyer/{Gameid}/{Buyerid}")]
        public IHttpActionResult UnAssociateGameWithBuyer(int Gameid, int Buyerid)
        {

            Game SelectedGame = db.Games.Include(a => a.Buyers).Where(a => a.GameID == Gameid).FirstOrDefault();
            Buyer SelectedBuyer = db.Buyers.Find(Buyerid);

            if (SelectedGame == null || SelectedBuyer == null)
            {
                return NotFound();
            }

            //Debug.WriteLine("Game id: " + Gameid);
            //Debug.WriteLine("Selected Game: " + SelectedGame.GameTitle);

            //Debug.WriteLine("Buyer id: " + Buyerid);
            //Debug.WriteLine("Selected Buyer: " + SelectedBuyer.BuyerName);

            SelectedGame.Buyers.Remove(SelectedBuyer);
            db.SaveChanges();

            return Ok();
        }

        /// Get all Games in the system.
        /// <param name="id">The primary key of the Game</param>
        /// GET: api/GamesData/FindGame/5
        [HttpGet]
        [ResponseType(typeof(GameDto))]
        public IHttpActionResult FindGame(int id)
        {
            Game Game = db.Games.Find(id);
            GameDto GameDto = new GameDto()
            {
                GameID = Game.GameID,
                GameName = Game.GameName,
                ReleaseYear = Game.ReleaseYear,
                Description = Game.Description,
                Price = Game.Price,
                GenreID = Game.Genre.GenreID,
                GenreName = Game.Genre.GenreName,
                GameHasPic = Game.GameHasPic,
                PicExtension = Game.PicExtension,
            };
            if (Game == null)
            {
                return NotFound();
            }

            return Ok(GameDto);
        }

        /// Update a particular Game
        /// <param name="id">Game ID primary key</param>
        /// <param name="Game">Game json data</param>
        /// PUT: api/GamesData/UpdateGame/5
        [HttpPost]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateGame(int id, Game Game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Game.GameID)
            {
                return BadRequest();
            }

            db.Entry(Game).State = EntityState.Modified;
            db.Entry(Game).Property(a => a.GameHasPic).IsModified = false;
            db.Entry(Game).Property(a => a.PicExtension).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
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
        /// upload to the server, set GameHasPic status
        /// POST: api/GameData/UpdateGamePic/1
        [HttpPost]
        public IHttpActionResult UploadGamePic(int id)
        {

            bool gameHaspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                // check the number of image files received 
                int numfiles = HttpContext.Current.Request.Files.Count;

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var gamePic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (gamePic.ContentLength > 0)
                    {
                        //set up valid image file types (file extensions)
                        var imgTypes = new[] { "jpeg", "jpg", "png", "gif", "jfif", "webp" };
                        var extension = Path.GetExtension(gamePic.FileName).Substring(1);
                        //Check the extension of the file
                        if (imgTypes.Contains(extension))
                        {
                            try
                            {
                                string fileName = id + "." + extension;

                                //file path to ~/Content/Images/Games/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Games/"), fileName);

                                //save the file to the path with 'fileName'
                                gamePic.SaveAs(path);

                                //game image was uploaded correctly
                                gameHaspic = true;
                                picextension = extension;

                                //update the game picture related data
                                Game Selectedgame = db.Games.Find(id);
                                Debug.WriteLine("Selectedgame.GameHasPic before update: " + Selectedgame.GameHasPic);
                                Selectedgame.GameHasPic = gameHaspic;
                                Selectedgame.PicExtension = picextension;
                                db.Entry(Selectedgame).State = EntityState.Modified;
                                Debug.WriteLine("Selectedgame.GameHasPic after update: " + Selectedgame.GameHasPic);

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                // check for error
                                Debug.WriteLine(" save failed");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

        }


        /// Add a new Game
        /// <param name="Game">Game json data</param>
        /// POST: api/GameData/AddGame
        [HttpPost]
        [ResponseType(typeof(Game))]
        public IHttpActionResult AddGame(Game Game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Games.Add(Game);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Game.GameID }, Game);
        }

        /// Delete a Game
        /// DELETE: api/GamesData/DeleteGame/5
        [HttpPost]
        [ResponseType(typeof(Game))]
        public IHttpActionResult DeleteGame(int id)
        {
            Game Game = db.Games.Find(id);
            if (Game == null)
            {
                return NotFound();
            }
            if (Game.GameHasPic && Game.PicExtension != "")
            {
                //delete game picture from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Images/Games/" + id + "." + Game.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            db.Games.Remove(Game);
            db.SaveChanges();

            return Ok(Game);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.GameID == id) > 0;
        }
    }
}