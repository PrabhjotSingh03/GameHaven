using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Gamers_Hub.Models
{
    public class Genre
    {
        [Key]
        public int GenreID { get; set; }
        public string GenreName { get; set; }
    }
    public class GenreDto
    {
        public int GenreID { get; set; }
        public string GenreName { get; set; }
    }
}