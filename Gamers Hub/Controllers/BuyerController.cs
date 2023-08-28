using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Diagnostics;
using Gamers_Hub.Models;
using Gamers_Hub.Models.ViewModels;
using System.Web.Script.Serialization;

namespace Gamers_Hub.Controllers
{
    public class BuyerController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        // set up CRUD functions (and others) for Buyer 
        static BuyerController()
        {
            // set up the base url address
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44340/api/");
        }

        // GET: Buyer/List
        public ActionResult List()
        {
            // communicate with BuyerData api to retrieve a list of Buyers
            //e.g. curl https://localhost:44340/api/buyerdata/listbuyers

            string url = "buyerdata/listbuyers";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            IEnumerable<BuyerDto> Buyers = response.Content.ReadAsAsync<IEnumerable<BuyerDto>>().Result;
            //Debug.WriteLine(Buyers.Count());

            // return to the 'Buyers' view page
            return View(Buyers);
        }

        // GET: Buyer/Details/5
        public ActionResult Details(int id)
        {
            DetailsBuyer ViewModel = new DetailsBuyer();

            //communicate with BuyerData api to retrieve one Buyer
            //e.g. curl https://localhost:44340/api/buyerdata/findbuyer/{id}

            string url = "buyerdata/findbuyer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine(response.StatusCode);

            BuyerDto SelectedBuyer = response.Content.ReadAsAsync<BuyerDto>().Result;

            //Debug.WriteLine(SelectedBuyer.BuyerName);

            ViewModel.SelectedBuyer = SelectedBuyer;

            //show all games buyed by Buyer
            url = "gamedata/listgamesforbuyer/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<GameDto> KeptGames = response.Content.ReadAsAsync<IEnumerable<GameDto>>().Result;

            ViewModel.KeptGames = KeptGames;

            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Buyer/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Buyer/Create
        [HttpPost]
        public ActionResult Create(Buyer Client)
        {
            // add a new Buyer via POST
            //Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(Buyer.BuyerName);
            //curl -H "Content-Type:application/json" -d @Buyer.json https://localhost:44340/api/clientdata/addclient 
            string url = "buyerdata/addbuyer";

            string jsonpayload = jss.Serialize(Client);
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

        // GET: Buyer/Edit/5
        public ActionResult Edit(int id)
        {
            // choose a specific Buyer to edit
            string url = "buyerdata/findbuyer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BuyerDto selectedBuyer = response.Content.ReadAsAsync<BuyerDto>().Result;
            return View(selectedBuyer);
        }

        // POST: Buyer/Update/5
        [HttpPost]
        public ActionResult Update(int id, Buyer Buyer)
        {
            // update a specific Buyer
            string url = "buyerdata/updatebuyer/" + id;
            string jsonpayload = jss.Serialize(Buyer);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Debug.WriteLine(content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Buyer/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            // Buyer delete confirm
            string url = "buyerdata/findbuyer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            BuyerDto selectedBuyer = response.Content.ReadAsAsync<BuyerDto>().Result;

            return View(selectedBuyer);
        }

        // POST: Buyer/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // delete a specific Buyer
            string url = "buyerdata/deletebuyer/" + id;
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
