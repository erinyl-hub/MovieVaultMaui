using MongoDB.Driver;
using MovieVaultMaui.Models;


namespace MovieVaultMaui
{
    class Helpers
    {
        public static bool MovieAlreadyInSafeChecker(string imdbId)
        {
            var moviesToSee = Managers.DataManager.GetMovieList(Enums.MovieLibraryType.MoviesToSee);
            var seenMovies = Managers.DataManager.GetMovieList(Enums.MovieLibraryType.SeenMovies);

            if (moviesToSee != null || seenMovies != null)
            {

                var combinedLibrary = moviesToSee
                    .Concat(seenMovies);

                var result = combinedLibrary.Where(movie => movie.imdbID.Contains(imdbId));


                return result.Any();
            }

            return false;
        }

        public async static Task<Dictionary<string, Func<string, IEnumerable<Movie>>>> CreateSearchEngineDictionary(List<Movie> movies)
        {
            var searchDictionary = new Dictionary<string, Func<string, IEnumerable<Movie>>>
            {
                { "Titel", (word) => movies.Where(movie => movie.Title != null && movie.Title.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Director", (word) => movies.Where(movie => movie.Director != null && movie.Director.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Actor", (word) => movies.Where(movie => movie.Actors != null && movie.Actors.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Genre", (word) => movies.Where(movie => movie.Genre != null && movie.Genre.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "ImdbID", (word) => movies.Where(movie => movie.imdbID != null && movie.imdbID.Contains(word, StringComparison.OrdinalIgnoreCase)) }
            };
            return searchDictionary;
        }

        public static IEnumerable<Movie> SearchEngine
            (Dictionary<string, Func<string, IEnumerable<Movie>>> searchDictionary, string searchType, string searchWord, string sortBy, bool changeOrder)
        {
            var result = searchDictionary[searchType](searchWord);

            switch (sortBy)
            {
                case "Last added":
                    result = result.OrderByDescending(m => m.MovieRegisterdTime); // Vissar fel, vissar senast först
                    break;
                case "Rating":
                    result = result.OrderByDescending(m => m.imdbRating); // lägst först
                    break;
                case "Length":
                    result = result.OrderBy(m => int.TryParse(m.Runtime.Split(' ')[0], out int runtime) ? runtime : 0); // Lägst först, 2 som inte stämde
                    break;
                case "Year":
                    result = result.OrderBy(m => m.Year); // äldst först
                    break;
                case "Alphabetically":
                    result = result.OrderBy(m => m.Title); // siffror först sedan bokstäver
                    break;
                case "Last Seen":
                    result = result.OrderByDescending(m => m.UserData.LastTimeSeen);
                    break;
                case "See Again":
                    result = result.OrderByDescending(m => m.UserData.SeeAgain);
                    break;
                case "Your Rating":
                    result = result.OrderByDescending(m => m.UserData.UserRating); // fixa när är 10 kommer först
                    break;
            }

            if (changeOrder) { result = result.Reverse(); }

            return result;
        }

        public static List<string> Spliter(string toSplit)
        {
            var splitList = toSplit.Split(new[] { ", " }, StringSplitOptions.None).ToList();

            return splitList;
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
