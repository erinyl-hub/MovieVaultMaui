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

        public static List<Models.Movie> GetMovieList(MovieLibraryType listName)
        {
            return listName switch
            {
                MovieLibraryType.MoviesToSee => _moviesToSee,
                MovieLibraryType.SeenMovies => _seenMovies,
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

        public IMongoCollection<Models.Movie> ConnectToDb(MovieLibraryType libraryType)
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

        public async Task LoadDataFromDbAsync() // lägg till try
        {
            int delay = 2000;

            var moviesToSeeData = await ConnectToDb(MovieLibraryType.MoviesToSee).AsQueryable().ToListAsync();
            var seenMoviesData = await ConnectToDb(MovieLibraryType.SeenMovies).AsQueryable().ToListAsync();

            _moviesToSee = moviesToSeeData;
            _seenMovies = seenMoviesData;
            DataLoaded = true;
            

        }

        public async Task AddMovieToList(Models.Movie movie, MovieLibraryType listName)
        {
            switch (listName)
            {
                case MovieLibraryType.MoviesToSee:
                    _moviesToSee.Add(movie);
                    break;

                case MovieLibraryType.SeenMovies:
                    _seenMovies.Add(movie);
                    break;
            }
        }

        public async Task RemoveMovieFromList(Models.Movie movie, MovieLibraryType listName)
        {
            switch (listName)
            {
                case MovieLibraryType.MoviesToSee:
                    _moviesToSee.Remove(movie);
                    break;

                case MovieLibraryType.SeenMovies:
                    _seenMovies.Remove(movie);
                    break;
            }
            SearchFilterManager.SetDataUpdatedSearchFilterManager();
        }

        public void MoveMovieLibrary(Models.Movie movie)
        {
            ConnectToDb(Enums.MovieLibraryType.SeenMovies).InsertOne(movie);
            RemoveMovieDb(movie, MovieLibraryType.MoviesToSee);

            AddMovieToList(movie, MovieLibraryType.SeenMovies);
            RemoveMovieFromList(movie, MovieLibraryType.MoviesToSee);
            SearchFilterManager.SetDataUpdatedSearchFilterManager();
        }

        public async Task RemoveMovieDb(Models.Movie movie, MovieLibraryType libraryType)
        {
            var filter = Builders<Models.Movie>.Filter.Eq(m => m.Id, movie.Id);
            ConnectToDb(libraryType).DeleteOne(filter);
        }

        public async Task UpdateMovie(Models.Movie movie, MovieLibraryType libraryType)
        {
            var movieToUpdate = _seenMovies.FirstOrDefault(m => m.Id == movie.Id);
            movieToUpdate.UserData = movie.UserData;


            var filter = Builders<Models.Movie>.Filter.Eq(m => m.Id, movie.Id);
            var update = Builders<Models.Movie>.Update
                .Set(m => m.UserData, movie.UserData);

            await ConnectToDb(libraryType).UpdateOneAsync(filter, update);
        }
    }

    public class DatabaseFacade
    {
        private readonly DataManager _dbService;

        public DatabaseFacade()
        {
            _dbService = new DataManager();
        }

        public void Execute(Models.Movie movie, DatabaseAction action, MovieLibraryType libraryType)
        {
            switch (action)
            {
                case DatabaseAction.Add:
                    _dbService.ConnectToDb(libraryType).InsertOne(movie);
                    _dbService.AddMovieToList(movie, libraryType);

                    break;

                case DatabaseAction.Remove:

                    _dbService.RemoveMovieDb(movie, libraryType);
                    _dbService.RemoveMovieFromList(movie, libraryType);
                    break;

                case DatabaseAction.Update:

                    _dbService.UpdateMovie(movie, libraryType);

                    break;

                case DatabaseAction.Move:

                    _dbService.MoveMovieLibrary(movie);

                    break;

                case DatabaseAction.LoadData:

                    _dbService.LoadDataFromDbAsync();

                    break;
            }
        }
    }
}
