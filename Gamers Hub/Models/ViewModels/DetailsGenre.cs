using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamers_Hub.Models.ViewModels
{
    public class DetailsGenre
    {
        //the Genre itself that we want to display
        public GenreDto SelectedGenre { get; set; }

        //all of the related games to that particular genre
        public IEnumerable<GameDto> RelatedGames { get; set; }
    }
}