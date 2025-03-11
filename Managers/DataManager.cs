using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MovieVaultMaui.Enums;


namespace MovieVaultMaui.Managers
{
    internal class DataManager
    {

        private static List<Models.Movie> _moviesToSee = new List<Models.Movie>();
        private static List<Models.Movie> _seenMovies = new List<Models.Movie>();
        public static bool DataLoaded { get; private set; } = false;

        public DataManager() { }


        public static List<Models.Movie> GetMovieList(MovieListType listName)
        {

            return listName switch
            {
                MovieListType.MoviesToSee => _moviesToSee,
                MovieListType.SeenMovies => _seenMovies,
                _ => null
            };

        }

        private static MongoClient GetClient()
        {
            const string connectionString = "mongodb+srv://eriknylund:SystemTILL2026!@ecluster.7zit7.mongodb.net/?retryWrites=true&w=majority&appName=ECluster";

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));

            var client = new MongoClient(settings);

            return client;
        }

        public static IMongoCollection<Models.Movie> ConnectToDb(MovieLibraryType libraryType)
        {
            var client = GetClient();

            var database = client.GetDatabase("MovieSafe");

            string collectionName = libraryType switch
            {
                MovieLibraryType.MoviesToSee => "MoviesToSee",
                MovieLibraryType.SeenMovies => "SeenMovies",
                _ => throw new ArgumentException("Invalid library type")
            };

            return database.GetCollection<Models.Movie>(collectionName);
        }

        public async static Task LoadDataFromDbAsync()
        {
            int delay = 2000;
            while (true)
            {
                try
                {
                    var moviesToSeeData = await ConnectToDb(MovieLibraryType.MoviesToSee).AsQueryable().ToListAsync();
                    var seenMoviesData = await ConnectToDb(MovieLibraryType.SeenMovies).AsQueryable().ToListAsync();

                    if (moviesToSeeData.Any() && seenMoviesData.Any())
                    {
                        _moviesToSee = moviesToSeeData;
                        _seenMovies = seenMoviesData;
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

        public async static Task AddMovieToList(Models.Movie movie, MovieListType listName)
        {
            switch (listName)
            {
                case MovieListType.MoviesToSee:
                    _moviesToSee.Add(movie);
                    break;

                case MovieListType.SeenMovies:
                    _seenMovies.Add(movie);
                    break;
            }
        }

        public async static Task RemoveMovieFromList(Models.Movie movie, MovieListType listName)
        {

            switch (listName)
            {
                case MovieListType.MoviesToSee:
                    _moviesToSee.Add(movie);
                    break;

                case MovieListType.SeenMovies:
                    _seenMovies.Add(movie);
                    break;
            }

        }


    }
}
