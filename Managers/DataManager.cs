using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MovieVaultMaui.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieVaultMaui.Managers
{
    internal class DataManager
    {

        private static List<Models.Movie> MoviesToSee;
        private static List<Models.Movie> SeenMovies;
        public static bool DataLoaded { get; private set; } = false;


        public static List<Models.Movie> GetMovieList(MovieListType listName)
        {
            List<Models.Movie> movieData;

            switch (listName)
            {
                case MovieListType.MoviesToSee:
                    movieData = MoviesToSee;
                    return movieData;

                case MovieListType.SeenMovies:
                    movieData = SeenMovies;
                    return movieData;
                default:
                    return null;

            }
        }



        private static MongoClient GetClient()
        {
            const string connectionString = "mongodb+srv://eriknylund:SystemTILL2026!@ecluster.7zit7.mongodb.net/?retryWrites=true&w=majority&appName=ECluster";

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));

            var client = new MongoClient(settings);

            return client;
        }

        private static IMongoCollection<Models.Movie> ConnectToDb(string libraryName)
        {
            var client = GetClient();

            var database = client.GetDatabase("MovieSafe");

            var movieInfo = database.GetCollection<Models.Movie>($"{libraryName}");


            return movieInfo;
        }

        public async static Task LoadDataFromDbAsync()
        {
            int delay = 2000;
            while (true)
            {
                try
                {
                    var moviesToSeeData = await ConnectToDb("WatchedMovies").AsQueryable().ToListAsync();
                    var seenMoviesData = await ConnectToDb("WatchLaterMovies").AsQueryable().ToListAsync();

                    if (moviesToSeeData.Any() && seenMoviesData.Any())
                    {
                        MoviesToSee = moviesToSeeData;
                        SeenMovies = seenMoviesData;
                        DataLoaded = true;
                        break; 
                    }
                }
                catch (Exception ex)
                {

                   
                    await Task.Delay(delay);

                }
            }
        }


    }
}
