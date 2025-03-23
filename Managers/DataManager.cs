using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MovieVaultMaui.Enums;


namespace MovieVaultMaui.Managers
{
    public class DataManager
    {
        private static readonly DataManager _instance = new DataManager();
        private static List<Models.Movie> _moviesToSee = new List<Models.Movie>();
        private static List<Models.Movie> _seenMovies = new List<Models.Movie>();

        public static bool DataLoaded { get; private set; } = false;


        public static DataManager Instance => _instance;

        public List<Models.Movie> GetMovieList(MovieLibraryType listName)
        {
            return listName switch
            {
                MovieLibraryType.MoviesToSee => _moviesToSee,
                MovieLibraryType.SeenMovies => _seenMovies,
                _ => null
            };
        }

        private MongoClient GetClient()
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

        public async Task LoadDataFromDbAsync()
        {
            int delay = 500;

            while (!DataLoaded)
            {
                try
                {
                    var moviesToSeeData = await ConnectToDb(MovieLibraryType.MoviesToSee).AsQueryable().ToListAsync();
                    var seenMoviesData = await ConnectToDb(MovieLibraryType.SeenMovies).AsQueryable().ToListAsync();

                    _moviesToSee = moviesToSeeData;
                    _seenMovies = seenMoviesData;
                    DataLoaded = true;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Faild to load data from database");
                }

               await Task.Delay(delay);
            }
        }

        public async Task AddMovieToLibrary(Models.Movie movie, MovieLibraryType libraryType)
        {
            try
            {
                ConnectToDb(libraryType).InsertOne(movie);

                switch (libraryType)
                {
                    case MovieLibraryType.MoviesToSee:
                        _moviesToSee.Add(movie);
                        break;

                    case MovieLibraryType.SeenMovies:
                        _seenMovies.Add(movie);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fail to add movie");
            }


        }

        public async Task RemoveMovieFromLibrary(Models.Movie movie, MovieLibraryType libraryType)
        {
            try
            {
                var filter = Builders<Models.Movie>.Filter.Eq(m => m.Id, movie.Id);
                ConnectToDb(libraryType).DeleteOne(filter);

                switch (libraryType)
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
            catch (Exception ex)
            {
                Console.WriteLine($"Faild to remove {movie}");
            }
        }
        public void MoveMovieLibrary(Models.Movie movie)
        {
            RemoveMovieFromLibrary(movie, MovieLibraryType.MoviesToSee);
            AddMovieToLibrary(movie, MovieLibraryType.SeenMovies);
            SearchFilterManager.SetDataUpdatedSearchFilterManager();
        }

        public async Task UpdateMovieInLibrary(Models.Movie movie, MovieLibraryType libraryType)
        {
            var movieToUpdate = _seenMovies.FirstOrDefault(m => m.Id == movie.Id);
            movieToUpdate.UserData = movie.UserData;


            var filter = Builders<Models.Movie>.Filter.Eq(m => m.Id, movie.Id);
            var update = Builders<Models.Movie>.Update
                .Set(m => m.UserData, movie.UserData);

            await ConnectToDb(libraryType).UpdateOneAsync(filter, update);
        }
    }

    public class MovieLibraryFacade
    {
        private readonly DataManager _dataManager;

        public MovieLibraryFacade(DataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public void AddMovie(Models.Movie movie, MovieLibraryType libraryType)
        {
            _dataManager.AddMovieToLibrary(movie, libraryType);
        }

        public void RemoveMovie(Models.Movie movie, MovieLibraryType libraryType)
        {
            _dataManager.RemoveMovieFromLibrary(movie, libraryType);
        }

        public void UpdateMovie(Models.Movie movie, MovieLibraryType libraryType)
        {
            _dataManager.UpdateMovieInLibrary(movie, libraryType);
        }

        public void MoveMovie(Models.Movie movie)
        {
            _dataManager.MoveMovieLibrary(movie);
        }

        public void LoadMoviesData()
        {
            _dataManager.LoadDataFromDbAsync();
        }
    }
}
