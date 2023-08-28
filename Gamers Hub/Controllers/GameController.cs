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
        private static readonly HttpClient Client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static GameController()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44340/api/");
        }
   
        // GET: Game/List
        public ActionResult List()
        {
            //communicate with BuyerData api to retrieve a list of Buye
            //curl: https://localhost:44340/api/GameData/ListGames

            string url = "GameData/ListGames";
            HttpResponseMessage response = Client.GetAsync(url).Result;
            IEnumerable<GameDto> Games = response.Content.ReadAsAsync<IEnumerable<GameDto>>().Result;

            return View(Games);
        }

        // GET: Game/Details/5
        public ActionResult Details(int id)
        {
            DetailsGame ViewModel = new DetailsGame();

            //communicate with Gamedata api to retrieve one specific game
            //curl https://localhost:44340/api/Gamedata/findGame/{id}
            string url = "Gamedata/findGame/" + id;
            HttpResponseMessage response = Client.GetAsync(url).Result;

            GameDto SelectedGame = response.Content.ReadAsAsync<GameDto>().Result;

            // set up a ViewModel to show the relationship between games and buyers
            ViewModel.SelectedGame = SelectedGame;

            //show associated buyers with this game via ViewModel
            url = "buyerdata/listbuyersforgame/" + id;
            response = Client.GetAsync(url).Result;
            IEnumerable<BuyerDto> LinkedBuyers = response.Content.ReadAsAsync<IEnumerable<BuyerDto>>().Result;

            ViewModel.LinkedBuyers = LinkedBuyers;

            // show unlinked buyer to this games
            url = "buyerdata/listbuyersnotinterestedgame/" + id;
            response = Client.GetAsync(url).Result;
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
            HttpResponseMessage response = Client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //Get: Game/UnAssociate/{id}?BuyerID={BuyerID}
        [HttpGet]
        public ActionResult UnAssociate(int id, int BuyerID)
        {
            //Debug.WriteLine("Unassociate Game :" + id + " with Buyer: " + BuyerID);

            //call api to link Game with Buyer
            string url = "gamedata/unassociategamewithbuyer/" + id + "/" + BuyerID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = Client.PostAsync(url, content).Result;

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
            HttpResponseMessage response = Client.GetAsync(url).Result;
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

            HttpResponseMessage response = Client.PostAsync(url, content).Result;
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
            HttpResponseMessage response = Client.GetAsync(url).Result;
            GameDto SelectedGame = response.Content.ReadAsAsync<GameDto>().Result;
            ViewModel.SelectedGame = SelectedGame;

            // all genres to choose from when updating this game
            // the existing game information
            url = "genredata/listgenre/";
            response = Client.GetAsync(url).Result;
            IEnumerable<GenreDto> GenreOptions = response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            ViewModel.GenreOptions = GenreOptions;

            return View(ViewModel);
        }

        // GET: Movie/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "gamedata/findgame/" + id;
            HttpResponseMessage response = Client.GetAsync(url).Result;
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
            HttpResponseMessage response = Client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Game/Update/1
        /// <summary>
        /// Add upload game picture funtion
        /// </summary>
        /// Updated games information and redirect to the game List page
        /// User can update the game without uploading a picture 
        [HttpPost]
        public ActionResult Update(int id, Game Game, HttpPostedFileBase GamePic)
        {
            // upload Game pictures method
            // add a feature to uplaod image file to the server using POST request(in the Update function)

            string url = "gamedata/updategame/" + id;
            string jsonpayload = jss.Serialize(Game);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = Client.PostAsync(url, content).Result;
            Debug.WriteLine(content);


            //server response is OK, and we have Game picture data(file exists)
            if (response.IsSuccessStatusCode && GamePic != null)
            {
                //Seperate request for updating the Game picture (when user update Game without providing pictures) 
                //Debug.WriteLine("Update picture");

                //set up picture url
                url = "GameData/UploadGamePic/" + id;

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(GamePic.InputStream);
                requestcontent.Add(imagecontent, "GamePic", GamePic.FileName);
                response = Client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
            {
                //server response is OK, but no picture uploaded(upload picture is a seperate add-on feature)
                return RedirectToAction("List");
            }
            else
            {

                return RedirectToAction("Error");
            }
        }
    }
}
