using IMDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class MovieDetailsGetData
    {
        public string ImageFilePath { get; set; }
        public List<ActorData> actorList {get;set;}
        public ProducerData producerList { get; set; }
        public MovieData MovieDetail { get; set; }
    }
    public class ActorProducerDetails
    {
        public List<ActorData> actorList { get; set; }
        public List<ProducerData> producerList { get; set; }
    }
    public class MovieListData
    {
        public string Name { get; set; }
        public Guid movie_id { get; set; }

    }
    public class MovieDetailsAddData
    {
        [Required(ErrorMessage = "ID is Required")]
        public Guid movie_id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Range(1913,2019)]
        public int Release { get; set; }
        [MaxLength(1000)]
        public string Plot { get; set; }
        public List<ActorData> actors { get; set; }
        public ProducerData Producer { get; set; }
    }
    public class MoviePosterData
    {
        [Required(ErrorMessage = "ID is Required")]
        public Guid movie_id { get; set; }
        public Image Poster { get; set; }

    }
    public class ProducerDetail
    {
        [Required(ErrorMessage = "ID is Required")]
        public Guid producer_id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
    }
    public class ActorsList
    {
        [Required(ErrorMessage = "ID is Required")]
        public Guid actor_id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
    }


}