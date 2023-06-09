using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Net.Http;
using Gamers_Hub.Models;
using Gamers_Hub.Models.ViewModels;
using System.Web.Script.Serialization;

namespace Gamers_Hub.Controllers
{
    public class GameController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static GameController()
        {
            // set up the base url address
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44340/api/");
        }

        // GET: Game/List
        public ActionResult List()
        {
            //communicate with BuyerData api to retrieve a list of Buyers
            //curl https://localhost:44340/api/GameData/ListGames


            string url = "GameData/ListGames";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            IEnumerable<GameDto> Games = response.Content.ReadAsAsync<IEnumerable<GameDto>>().Result;

            //Debug.WriteLine(games.Count());

            // return to the 'Games' view page
            return View(Games);
        }

        // GET: Game/Details/5
        public ActionResult Details(int id)
        {
            DetailsGame ViewModel = new DetailsGame();

            //communicate with Gamedata api to retrieve one specific game
            //curl https://localhost:44340/api/Gamedata/findGame/{id}

            string url = "Gamedata/findGame/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            GameDto SelectedGame = response.Content.ReadAsAsync<GameDto>().Result;
            //Debug.WriteLine("Game received : ");
            //Debug.WriteLine(SelectedGame.GameName);

            // set up a ViewModel to show the relationship between games and buyers
            ViewModel.SelectedGame = SelectedGame;

            //show associated buyers with this game via ViewModel
            url = "buyerdata/listbuyersforgame/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<BuyerDto> LinkedBuyers = response.Content.ReadAsAsync<IEnumerable<BuyerDto>>().Result;

            ViewModel.LinkedBuyers = LinkedBuyers;

            // show unlinked buyer to this games
            url = "buyerdata/listbuyersnotinterestedgame/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<BuyerDto> AvailableBuyers = response.Content.ReadAsAsync<IEnumerable<BuyerDto>>().Result;

            ViewModel.AvailableBuyers = AvailableBuyers;
            return View(ViewModel);
        }

        //POST: Game/Associate/{Gameid}
        [HttpPost]
        public ActionResult Associate(int id, int BuyerID)
        {
            //Debug.WriteLine("Associate Game :" + id + " with Buyer " + BuyerID);

            //call api to link Game with Buyer
            string url = "gamedata/associategamewithbuyer/" + id + "/" + BuyerID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //Get: Game/UnAssociate/{id}?BuyerID={BuyerID}
        [HttpGet]
        public ActionResult UnAssociate(int id, int BuyerID)
        {
            //Debug.WriteLine("Unassociate Game :" + id + " with Buyer: " + BuyerID);

            //call api to link Game with Buyer
            string url = "moviedata/unassociategamewithbuyer/" + id + "/" + BuyerID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            // error view page
            return View();
        }


        public ActionResult New()
        {
            //show a list of genres
            //GET api/Genredata/listGenre

            string url = "genredata/listgenre";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<GenreDto> GenreOptions = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            return View(GenreOptions);
        }

        // POST: Game/Create
        [HttpPost]
        public ActionResult Create(Game Game)
        {
            //Debug.WriteLine("json payload:");
            //Debug.WriteLine(Game.GameName);
            //add a new game
            string url = "gamedata/addgame";

            // json
            string jsonpayload = jss.Serialize(Game);
            //Debug.WriteLine(jsonpayload);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Game/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateGame ViewModel = new UpdateGame();

            //the existing Game information
            string url = "gamedata/findgame/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GameDto SelectedGame = response.Content.ReadAsAsync<GameDto>().Result;
            ViewModel.SelectedGame = SelectedGame;

            // all genres to choose from when updating this game
            // the existing game information
            url = "genredata/listgenre/";
            response = client.GetAsync(url).Result;
            IEnumerable<GenreDto> GenreOptions = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            ViewModel.GenreOptions = GenreOptions;

            return View(ViewModel);
        }

        // GET: Movie/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "gamedata/findgame/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            GameDto selectedGame = response.Content.ReadAsAsync<GameDto>().Result;
            return View(selectedGame);
        }

        // POST: Game/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "gamedata/deletegame/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
