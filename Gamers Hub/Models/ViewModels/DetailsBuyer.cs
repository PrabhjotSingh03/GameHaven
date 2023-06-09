using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamers_Hub.Models.ViewModels
{
    public class DetailsBuyer
    {
        public BuyerDto SelectedBuyer { get; set; }
        public IEnumerable<GameDto> KeptGames { get; set; }
    }
}