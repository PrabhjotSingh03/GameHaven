using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gamers_Hub.Models.ViewModels
{
    public class UpdateGame
    {
        //This viewmodel is a class which stores information that we need to present to /Game/Update/{id}

        //the existing game information

        public GameDto SelectedGame { get; set; }

        // all genres to choose from when updating this game

        public IEnumerable<GenreDto> GenreOptions { get; set; }
    }
}