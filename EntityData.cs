using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing.Imaging;
using System.Drawing;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using WebApplication1;

namespace IMDB.Models
{
    public class MovieData
    {
      [Required(ErrorMessage ="Name is Required")]
      public string Name{ get; set; }
      [GuidValidation]
      public Guid movie_id { get; set; }
      [Range(1913,2019)]
      public int Release { get; set; }
      [MaxLength(1000)]
      public string Plot { get; set; }
      public Byte[] Poster { get; set; }

   }
    public class ActorData
    {
        public Guid actor_id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        public string Sex { get; set; }
        [CustomDateValidation(ErrorMessage = "Future date entry not allowed")]
        public DateTime DOB{get;set;}
        public string Bio { get; set; }
        
    }
    public class ProducerData
    {
        public Guid producer_id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        public string Sex { get; set; }
        [CustomDateValidation(ErrorMessage = "Future date entry not allowed")]
        public DateTime DOB { get; set; }
        public string Bio { get; set; }
    }
    public class ActorMovieRelationData
    {
        public Guid movie_id { get; set; }
        public Guid actor_id { get; set; }
    }
    public class MovieProducerRelationData
    {
        public Guid movie_id { get; set; }
        public Guid producer_id { get; set; }
    }
}