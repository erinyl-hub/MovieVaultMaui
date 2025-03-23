

using MovieVaultMaui.Enums;

namespace MovieVaultMaui.Managers
{
    class SearchFilterManager
    {
        private static TaskCompletionSource<bool> _tcs = new();
        private static Dictionary<string, Func<string, IEnumerable<Models.Movie>>> _movieToSeeSearchDictionary;
        private static Dictionary<string, Func<string, IEnumerable<Models.Movie>>> _seenMovieSearchDictionary;

        private async static Task<Dictionary<string, Func<string, IEnumerable<Models.Movie>>>> CreateSearchEngineDictionary(List<Models.Movie> movies)
        {
            var searchDictionary = new Dictionary<string, Func<string, IEnumerable<Models.Movie>>>
            {
                { "Titel", (word) => movies.Where(movie => movie.Title != null && movie.Title.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Director", (word) => movies.Where(movie => movie.Director != null && movie.Director.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Actor", (word) => movies.Where(movie => movie.Actors != null && movie.Actors.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "Genre", (word) => movies.Where(movie => movie.Genre != null && movie.Genre.Contains(word, StringComparison.OrdinalIgnoreCase)) },
                { "ImdbID", (word) => movies.Where(movie => movie.imdbID != null && movie.imdbID.Contains(word, StringComparison.OrdinalIgnoreCase)) }
            };
            return searchDictionary;
        }

        public async static Task FilSearchEngineDictionary(bool dataAvailable)
        {
            DataManager dataManager = new DataManager();

            while (!DataManager.DataLoaded)
            {
                await Task.Delay(500); 
            }

            _movieToSeeSearchDictionary = await CreateSearchEngineDictionary(
                dataManager.GetMovieList(Enums.MovieLibraryType.MoviesToSee)).ConfigureAwait(false);
            _seenMovieSearchDictionary = await CreateSearchEngineDictionary(
                dataManager.GetMovieList(Enums.MovieLibraryType.SeenMovies)).ConfigureAwait(false);

        }

        public async static Task<Dictionary<string, Func<string, IEnumerable<Models.Movie>>>> GetMovieSearchEngineDictionary(MovieLibraryType dictionaryTypeName)
        {
            return dictionaryTypeName switch
            {
                MovieLibraryType.MoviesToSee => _movieToSeeSearchDictionary,
                MovieLibraryType.SeenMovies => _seenMovieSearchDictionary,
                _ => null
            };
        }

        public async static void updateDictionary(MovieLibraryType dictionaryTypeName)
        {
            await _tcs.Task;
            DataManager dataManager = new DataManager();

            switch (dictionaryTypeName)
            {
                case MovieLibraryType.MoviesToSee:
                    _movieToSeeSearchDictionary = await CreateSearchEngineDictionary(
                dataManager.GetMovieList(Enums.MovieLibraryType.MoviesToSee)).ConfigureAwait(false);
                    WatchLaterPage.SetDataWatchLaterPage();
                    break;


                case MovieLibraryType.SeenMovies:
                    _seenMovieSearchDictionary = await CreateSearchEngineDictionary(
                dataManager.GetMovieList(Enums.MovieLibraryType.SeenMovies)).ConfigureAwait(false);
                    SeenMoviesPage.SetDataSeenMoviesPage();
                    break;

            }

            _tcs = new TaskCompletionSource<bool>();
        }
        public static void SetDataUpdatedSearchFilterManager()
        {
            _tcs.TrySetResult(true); 
        }



    }
}
