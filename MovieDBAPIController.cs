
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebApplication1.DAL.Repository;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MovieDBAPIController : ApiController
    {
        MovieDataRepository moviedata = new MovieDataRepository();
        [HttpGet]
        [Route("api/movieList")]
        public IHttpActionResult GetMovieMethod(string searchText)
        {
            List<MovieListData> movieList = moviedata.GetMovieList(searchText);
            return Ok(movieList);
        }
        [HttpGet]
        [Route("api/moviedetails")]
        public IHttpActionResult getMovieDetails(Guid movie_id)
        {

            MovieDetailsGetData moviedetails = moviedata.GetMovieDetails(movie_id);
            return Ok(moviedetails);
        }
        [HttpGet]
        [Route("api/actorproducerdetails")]
        public IHttpActionResult getActorProducerDetails()
        {
            ActorProducerDetails actorproducerdetails = moviedata.getProducerActorMethod();
            return Ok(actorproducerdetails);
        }
        [HttpPost]
        [Route("api/postImagePoster")]
        public IHttpActionResult UploadImagePoster(Guid movie_id)
        {

            HttpFileCollection file=HttpContext.Current.Request.Files;

            
            string status = moviedata.PostImageMethod(file[0],movie_id);
            return Ok(status);
        }
        [HttpPost]
        [Route("api/addnewmovieDetails")]
        public IHttpActionResult AddMovie(MovieDetailsAddData movie)
        {
            string status = moviedata.AddMovieMethod(movie);
            return Ok(status);
        }
        [HttpPut]
        [Route("api/updatemovieDetails")]
        public IHttpActionResult UpdateMovie(MovieDetailsAddData movie)
        {
            string status = moviedata.UpdateMovieMethod(movie);
            return Ok(status);
        }
        [HttpGet]
        [Route("api/deletemoviedetails")]
        public IHttpActionResult DeleteMovie(Guid movie_id)
        {
            string Status = moviedata.DeleteMovieMethod(movie_id);
            return Ok(Status);
        }


    }
}
