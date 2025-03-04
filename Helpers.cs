using MongoDB.Driver;
using MovieVaultMaui.Models;
using System.Collections.ObjectModel;
using System.Globalization;

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

        public async static Task<Dictionary<string, Func<string, IEnumerable<Movie>>>> CreateSearchEngineDictionary(List<Movie> movies)
        {
            var searchDictionary = new Dictionary<string, Func<string, IEnumerable<Movie>>>
            {
                { "Titel", (word) => aplicationData.MoviesToSee.Where(movie => movie.Title != null && movie.Title.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Director", (word) => aplicationData.MoviesToSee.Where(movie => movie.Director != null && movie.Director.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Actor", (word) => aplicationData.MoviesToSee.Where(movie => movie.Actors != null && movie.Actors.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Genre", (word) => aplicationData.MoviesToSee.Where(movie => movie.Genre != null && movie.Genre.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "ImdbID", (word) => aplicationData.MoviesToSee.Where(movie => movie.imdbID != null && movie.imdbID.Contains(word, StringComparison.OrdinalIgnoreCase)) }
            };

            return searchDictionary;
        }

        public static ObservableCollection<Movie> SearchEngine
            (Dictionary<string, Func<string, IEnumerable<Movie>>> searchDictionary, string searchType, string searchWord, string sortBy)
        {


                var result = searchDictionary[searchType](searchWord);
                

            switch (sortBy)
            {
                case "Last added":
                    result = result.OrderBy(m => m.MovieRegisterdTime);
                    break;
                case "Rating":
                    result = result.OrderBy(m => m.imdbRating); // lägst först
                    break;
                case "Length":
                    result = result.OrderBy(m => m.Runtime);
                    break;
                case "Year":
                    result = result.OrderBy(m => m.Year);
                    break;
                case "Alphabetically":
                    result = result.OrderBy(m => m.Title);
                    break;

                default:
                    result = result.OrderBy(m => m.MovieRegisterdTime); // Standard sortering
                    break;
            }

            return new ObservableCollection<Movie>(result);

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
