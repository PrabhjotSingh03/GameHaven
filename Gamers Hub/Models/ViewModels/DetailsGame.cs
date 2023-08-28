using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamers_Hub.Models.ViewModels
{
    public class DetailsGame
    {
        public GameDto SelectedGame { get; set; }
        public IEnumerable<BuyerDto> LinkedBuyers { get; set; }
        public IEnumerable<BuyerDto> AvailableBuyers { get; set; }
    }
}