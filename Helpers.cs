using MongoDB.Driver;
using MovieVaultMaui.Models;

namespace MovieVaultMaui
{
    class Helpers
    {

        public static async Task GetData()
        {
            DataManager.MongoDbManager mongoDbManager = new DataManager.MongoDbManager();
            Models.aplicationData.SeenMovies = mongoDbManager.ConnectToDb("WatchedMovies").AsQueryable().ToList();
            Models.aplicationData.MoviesToSee = mongoDbManager.ConnectToDb("WatchLaterMovies").AsQueryable().ToList();
        }


        public static bool MovieAlreadyInSafeChecker(string imdbId)
        {
            var combinedLibrary = aplicationData.MoviesToSee
                .Concat(aplicationData.SeenMovies);

            var result = combinedLibrary.Where(movie => movie.imdbID.Contains(imdbId));


            return result.Any();

        }

        //public static string ConvertRuneTime(string time)
        //{
        //    int allTime = int.Parse(time.Replace(" min", ""));
        //    int houers = allTime / 60;
        //    int minutes = allTime % 60;
        //    string convertedTime = $"{houers}h {minutes}m";

        //    return convertedTime;
        //}
    }
}
