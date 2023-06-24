using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gamers_Hub.Models
{
    public class Game
    {
        [Key]
        public int GameID { get; set; }
        public string GameName { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        //Game images uploaded data for tracking
        //images will be stored into /Content/Images/Games/{id}.{extension}
        public bool GameHasPic { get; set; }
        public string PicExtension { get; set; }

        // <Genre>-<Game>  ==  1-M 
        [ForeignKey("Genre")]
        public int GenreID { get; set; }
        public virtual Genre Genre { get; set; }


        // <Game>-<Buyer>  ==  M-M 
        public ICollection<Buyer> Buyers { get; set; }
    }

    public class GameDto
    {
        public int GameID { get; set; }
        public string GameName { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int GenreID { get; set; }
        public string GenreName { get; set; }
        //Game images uploaded data for tracking
        //images will be stored into /Content/Images/Games/{id}.{extension}
        public bool GameHasPic { get; set; }
        public string PicExtension { get; set; }
    }
}