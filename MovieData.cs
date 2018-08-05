using Dapper;
using IMDB.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using WebApplication1.Common;

using WebApplication1.Models;

namespace WebApplication1.DAL.Repository
{
    public class MovieDataRepository
    {
        SQLSetup sqlsetup = new SQLSetup();
        public List<MovieListData> GetMovieList(string SearchText)
        {
            List<MovieListData> movieCollection = new List<MovieListData>();
            SqlConnection sqlconnection = sqlsetup.SqlConnectionSetup();
            try
            {
                
                SearchText.Trim();
                String search = "\'%" + SearchText + "%\'";
                sqlconnection.Open();
                SqlCommand sqlTableFetch = new SqlCommand("SELECT Name,movie_id from IMDB.dbo.Movie Where Name like " + search);
                sqlTableFetch.Connection = sqlconnection;
                SqlDataReader reader = sqlTableFetch.ExecuteReader();
                while (reader.Read())
                {
                    MovieListData movielist = new MovieListData();
                    movielist.Name = reader["Name"].ToString();
                    movielist.movie_id = Guid.Parse(reader["movie_id"].ToString());
                    movieCollection.Add(movielist);
                }

                reader.Close();

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sqlconnection.Close();
                
            }
            return movieCollection;
        }
        public MovieDetailsGetData GetMovieDetails(Guid movie_id)
        {
            SqlConnection sqlconnection = sqlsetup.SqlConnectionSetup();
            MovieDetailsGetData moviedetails = new MovieDetailsGetData();
            try
            {

                sqlconnection.Open();

                List<Task> TableFetch = new List<Task>();

                MovieData MovieDBList = new MovieData();
                MovieProducerRelationData MovieProducerMapping = new MovieProducerRelationData();
                List<ActorData> ActorDBList = new List<ActorData>();
                List<ProducerData> ProducerList = new List<ProducerData>();
                List<ActorMovieRelationData> MovieActorMapping = new List<ActorMovieRelationData>();

                Dictionary<Guid, ActorData> dicActorData = new Dictionary<Guid, ActorData>();
                Dictionary<Guid, ProducerData> dicProducer = new Dictionary<Guid, ProducerData>();



                string ImageLocation = "/src/Images/";


                TableFetch.Add(Task.Run(() => {
                    MovieDBList = sqlconnection.Query<MovieData>("SELECT * FROM IMDB.dbo.Movie WHERE movie_id = @movie_id;", new { movie_id }).FirstOrDefault();

                }));
                TableFetch.Add(Task.Run(() => {

                    ActorDBList = sqlconnection.Query<ActorData>("SELECT * FROM IMDB.dbo.Actors;").ToList();
                    if (ActorDBList?.Count > 0)
                    {
                        dicActorData = ActorDBList.ToDictionary(x => x.actor_id);
                    }
                }));
                TableFetch.Add(Task.Run(() => {
                    ProducerList = sqlconnection.Query<ProducerData>("SELECT * FROM IMDB.dbo.Producers;").ToList();
                    if (ProducerList?.Count > 0)
                    {
                        dicProducer = ProducerList.ToDictionary(x => x.producer_id);
                    }
                }));
                TableFetch.Add(Task.Run(() => {
                    MovieProducerMapping = sqlconnection.Query<MovieProducerRelationData>("SELECT * FROM IMDB.dbo.Mapping_Table_Producers_and_Movies WHERE movie_id = @movie_id;", new { movie_id }).FirstOrDefault();

                }));
                TableFetch.Add(Task.Run(() => {
                    MovieActorMapping = sqlconnection.Query<ActorMovieRelationData>("SELECT * FROM IMDB.dbo.Mapping_Table_Actors_and_movies WHERE movie_id = @movie_id;", new { movie_id }).ToList();

                }));
                Task.WaitAll(TableFetch.ToArray());

                String Format = ".jpg";


                moviedetails.MovieDetail = MovieDBList;
                moviedetails.ImageFilePath = moviedetails.MovieDetail.Poster != null? ImageLocation + MovieDBList.Name + Format:null;

                ProducerData producerBio = null;
                dicProducer.TryGetValue(MovieProducerMapping.producer_id, out producerBio);

                if (producerBio != null)
                {
                    moviedetails.producerList = producerBio;
                }
                object obj = new object();
                moviedetails.actorList = new List<ActorData>();
                Parallel.ForEach(MovieActorMapping, x =>
                {
                    ActorData actordata = null;
                    dicActorData.TryGetValue(x.actor_id, out actordata);
                    if (actordata != null)
                    {
                        lock (obj)
                        {
                            moviedetails.actorList.Add(actordata);
                        }
                    }
                });

            }
            catch (Exception e)
            {
            }
            finally
            {
                sqlconnection.Close();
            }
            return moviedetails;
        }
        public ActorProducerDetails getProducerActorMethod()
        {
            ActorProducerDetails actorproducerDetails = new ActorProducerDetails();
            SqlConnection sqlconnection = null;
            try
            {
                List<ActorData> actorList = new List<ActorData>();
                List<ProducerData> producerList = new List<ProducerData>();
                List<Task> tableFetch = new List<Task>();

                using (sqlconnection = sqlsetup.SqlConnectionSetup())
                {

                    sqlconnection.Open();

                    tableFetch.Add(Task.Run(() => {
                        actorList= sqlconnection.Query<ActorData>("SELECT * FROM IMDB.dbo.Actors;").ToList();
                    }));
                    tableFetch.Add(Task.Run(() =>
                    {
                        producerList = sqlconnection.Query<ProducerData>("SELECT * FROM IMDB.dbo.Producers;").ToList();
                    }));
                    Task.WaitAll(tableFetch.ToArray());

                 }
                actorproducerDetails.actorList = new List<ActorData>(actorList);
                actorproducerDetails.producerList = new List<ProducerData>(producerList);
             }
            catch {

            }
            finally
            {
                sqlconnection.Close();
            }
            return actorproducerDetails;
        }
        public string PostImageMethod(HttpPostedFile file, Guid movie_id)
        {
            SqlConnection sqlconnection = null;
            try
            {
                using (sqlconnection = sqlsetup.SqlConnectionSetup())
                {

                    sqlconnection.Open();

                    string ImageLocation = WebConfigurationManager.AppSettings["ImageLocation"];
                    string sql = "SELECT Name FROM IMDB.dbo.Movie WHERE movie_id = @movie_id;";

                    Stream stream = file.InputStream;

                    MemoryStream memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    byte[] Poster = memoryStream.ToArray();

                    sqlconnection.Execute("update IMDB.dbo.Movie set Poster = @Poster where movie_id = @movie_id", new { Poster, movie_id });

                    IEnumerable<MovieData> MovieDBList = sqlconnection.Query<MovieData>(sql, new { movie_id = movie_id });
                    string movie_name = MovieDBList.FirstOrDefault().Name;
                    string Format = ".jpg";
                    String path = Path.Combine(ImageLocation, movie_name);
                    if (File.Exists(path+Format))
                        File.Delete(path+Format);
                    file.SaveAs(path+Format);

                }
            }
            catch (Exception e)
            {
                return "Failure";
                throw e;
            }
            finally
            {
                sqlconnection.Close();
            }
            return "Success";

        }
        public string AddMovieMethod(MovieDetailsAddData movie)
        {
            SqlConnection sqlconnection = sqlsetup.SqlConnectionSetup();
            try
            {
                sqlconnection.Open();
                string ActorTable = "Insert into IMDB.dbo.Actors(actor_id,Name,Sex,DOB,Bio) values(@actor_id,@Name,@Sex,@DOB,@Bio);";
                string ProducerTable = "Insert into IMDB.dbo.Producers(producer_id,Name,Sex,DOB,Bio) values(@producer_id,@Name,@Sex,@DOB,@Bio);";

                string MovieTable = "Insert into IMDB.dbo.Movie(movie_id,Name,Release,Plot) values(@movie_id,@Name,@Release,@Plot);";
                string MovieProducerTable = "Insert into IMDB.dbo.Mapping_Table_Producers_and_Movies(movie_id,producer_id) values(@movie_id,@producer_id);";
                string MappingTableActorMovie = "Insert into IMDB.dbo.Mapping_Table_Actors_and_movies(movie_id,actor_id) values(@movie_id,@actor_id);";
                if (movie.movie_id == null || movie.movie_id == default(Guid))
                {
                    movie.movie_id = Guid.NewGuid();
                }
                if (movie.Producer.producer_id == null || movie.Producer.producer_id == default(Guid))
                {
                    movie.Producer.producer_id = Guid.NewGuid();
                }
                Parallel.ForEach(movie.actors, x =>
                {
                    if (x.actor_id == null || x.actor_id == default(Guid))
                    {
                        x.actor_id = Guid.NewGuid();
                        sqlconnection.Execute(ActorTable, new { x.actor_id, x.Name, x.Sex, x.DOB, x.Bio });
                    }
                    sqlconnection.Execute(MappingTableActorMovie, new { movie.movie_id, x.actor_id });

                });
                List<Task> tableInsert = new List<Task>();
                tableInsert.Add(Task.Run(() =>
                {
                    sqlconnection.Execute(MovieTable, new { movie.movie_id, movie.Name, movie.Release, movie.Plot });
                }));
                tableInsert.Add(Task.Run(() =>
                {
                    sqlconnection.Execute(MovieProducerTable, new { movie.movie_id, movie.Producer.producer_id });
                }));
                tableInsert.Add(Task.Run(() =>
                {
                    sqlconnection.Execute(ProducerTable, new { movie.Producer.producer_id, movie.Producer.Name, movie.Producer.Sex, movie.Producer.DOB, movie.Producer.Bio });
                }));
                Task.WaitAll(tableInsert.ToArray());
            }
            catch (Exception e)
            {
                return "Failure";
            }
            finally
            {

                sqlconnection.Close();
            }
            return "Success";
        }
        public string UpdateMovieMethod(MovieDetailsAddData movie)
        {

            SqlConnection sqlconnection = sqlsetup.SqlConnectionSetup();
            try
            {
                sqlconnection.Open();



                string ActorTable = "Insert into IMDB.dbo.Actors(actor_id,Name,Sex,DOB,Bio) values(@actor_id,@Name,@Sex,@DOB,@Bio);";
                string ProducerTable = "Insert into IMDB.dbo.Producers(producer_id,Name,Sex,DOB,Bio) values(@producer_id,@Name,@Sex,@DOB,@Bio);";

                string MovieTable = "Update IMDB.dbo.Movie set Name=@Name,Release=@Release,Plot=@Plot where movie_id=@movie_id;";

                string MovieProducerTable = "Insert into IMDB.dbo.Mapping_Table_Producers_and_Movies(movie_id,producer_id) values(@movie_id,@producer_id);";
                string MappingTableActorMovie = "Insert into IMDB.dbo.Mapping_Table_Actors_and_movies(movie_id,actor_id) values(@movie_id,@actor_id)";

                string DeleteExistingRecordsMappingActor = "Delete from IMDB.dbo.Mapping_Table_Actors_and_movies where movie_id=@movie_id;";
                string DeleteExistingRecordsMappingProducer = "Delete from IMDB.dbo.Mapping_Table_Producers_and_Movies where movie_id=@movie_id";


                if (movie.Producer.producer_id == null || movie.Producer.producer_id == default(Guid))
                {
                    movie.Producer.producer_id = Guid.NewGuid();

                }

                sqlconnection.Execute(DeleteExistingRecordsMappingActor, new { movie.movie_id });
                sqlconnection.Execute(DeleteExistingRecordsMappingProducer, new { movie.movie_id });

                Parallel.ForEach(movie.actors, x =>
                {
                    if (x.actor_id == null || x.actor_id == default(Guid))
                    {
                        x.actor_id = Guid.NewGuid();
                        sqlconnection.Execute(ActorTable, new { x.actor_id, x.Name, x.Sex, x.DOB, x.Bio });
                    }
                    sqlconnection.Execute(MappingTableActorMovie, new { movie.movie_id, x.actor_id });

                });

                List<Task> tableInsert = new List<Task>();
                tableInsert.Add(Task.Run(() =>
                {
                    sqlconnection.Execute(MovieTable, new { movie.Name, movie.Release, movie.Plot, movie.movie_id });
                }));
                tableInsert.Add(Task.Run(() =>
                {
                    sqlconnection.Execute(MovieProducerTable, new { movie.movie_id, movie.Producer.producer_id });
                }));
                tableInsert.Add(Task.Run(() =>
                {
                    sqlconnection.Execute(ProducerTable, new { movie.Producer.producer_id, movie.Producer.Name, movie.Producer.Sex, movie.Producer.DOB, movie.Producer.Bio });

                }));

                Task.WaitAll(tableInsert.ToArray());

            }
            catch (Exception e)
            {
                return "Failure";
            }
            finally
            {

                sqlconnection.Close();
            }
            return "Success";
        }
        public string DeleteMovieMethod(Guid movie_id)
        {
            SqlConnection sqlconnection = sqlsetup.SqlConnectionSetup();
            try
            {

                sqlconnection.Open();
                DynamicParameters deleteMovie = new DynamicParameters();
                deleteMovie.Add("@Movie_ID", movie_id, dbType: DbType.Guid);
                sqlconnection.Execute("imdb_schema.USP_MovieDetails_Delete", deleteMovie, commandType: CommandType.StoredProcedure);

            }
            catch (Exception e)
            {
                return "Failure";

            }
            finally
            {
                sqlconnection.Close();
            }
            return "Success";

        }


    }
}