using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Gamers_Hub.Models
{
    public class Buyer
    {
        [Key]
        public int BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }

        // <Game>-<Buyer>  ==  M-M 
        public ICollection<Game> Games { get; set; }
    }
    public class BuyerDto
    {
        public int BuyerID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
    }
}