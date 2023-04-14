using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using WebApp.Models;


namespace WebApp.Controllers
{
    public class MovieController : Controller
    {
        IConfiguration _configuration;
        SqlConnection _Connection;


        public MovieController(IConfiguration configuration)
        {
            _configuration = configuration;
            _Connection = new SqlConnection(_configuration.GetConnectionString("Practice"));
        }


        public List<MovieModel> GetMovies()
        {
            List<MovieModel> Movies = new();
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("FETCH_MOVIES",_Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                MovieModel movie = new();
                movie.MovieID = (int)reader["MovieID"];
                movie.Title = (string)reader["Title"];
                movie.ReleaseYear = (int)reader["ReleaseYear"];
                movie.Genre = (string)reader["Genre"];
                movie.Director = (string)reader["Director"];
                Movies.Add(movie);
            }

            reader.Close();
            _Connection.Close();

            return Movies;
            
        }

        // GET: MovieController
        public ActionResult Index()
        {

            return View(GetMovies());
        }

        // GET: MovieController/Details/5
        public ActionResult Details(int id)
        {
            return View(GetMovie(id));
        }

        // GET: MovieController/Create
        public ActionResult Create()
        {
            return View();
        }

        void InsertMovie(MovieModel Movie)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("ADD_MOVIE", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Title", Movie.Title);
            cmd.Parameters.AddWithValue("@ReleaseYear", Movie.ReleaseYear);
            cmd.Parameters.AddWithValue("@Genre", Movie.Genre);
            cmd.Parameters.AddWithValue("@Director", Movie.Director);

            cmd.ExecuteNonQuery();

            _Connection.Close();

        }

        // POST: MovieController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MovieModel Movie)
        {
            try
            {
                InsertMovie(Movie);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return View();
            }
        }

        MovieModel GetMovie(int id)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("GET_MOVIE", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MovieID", id);

            SqlDataReader reader = cmd.ExecuteReader();

            MovieModel movie = new();

            while (reader.Read())
            {
                
                movie.MovieID = (int)reader["MovieID"];
                movie.Title = (string)reader["Title"];
                movie.ReleaseYear = (int)reader["ReleaseYear"];
                movie.Genre = (string)reader["Genre"];
                movie.Director = (string)reader["Director"];
            }
            return movie;
        }


            // GET: MovieController/Edit/5
            public ActionResult Edit(int id)
        {
            //Console.WriteLine(id);
            return View(GetMovie(id));
        }

        void UpdateMovie(int id, MovieModel Movie)
        {
            _Connection.Open();
            SqlCommand cmd = new SqlCommand("EDIT_MOVIE", _Connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Title", Movie.Title);
            cmd.Parameters.AddWithValue("@ReleaseYear", Movie.ReleaseYear);
            cmd.Parameters.AddWithValue("@Genre", Movie.Genre);
            cmd.Parameters.AddWithValue("@Director", Movie.Director);
            cmd.Parameters.AddWithValue("@MovieID", id);

            cmd.ExecuteNonQuery();

            _Connection.Close();
        }

        // POST: MovieController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MovieModel Movie)
        {
            try
            {
                Console.WriteLine(id);
                UpdateMovie(id, Movie);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MovieController/Delete/5
        public ActionResult Delete(int id)
            {
            return View(GetMovie(id));
            }

        // POST: MovieController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, MovieModel Movie)
            {
            try
                {
                _Connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Movies WHERE MovieID=@MovieID",
                    _Connection);

                cmd.Parameters.AddWithValue("@MovieID", id);
                cmd.ExecuteNonQuery();

                _Connection.Close();
                return RedirectToAction(nameof(Index));
                }
            catch
                {
                return View();
                }
            }
        }
    }

