using MongoDB.Driver;
using MovieVaultMaui.Models;

namespace MovieVaultMaui.Managers
{
    class MongoDbManager
    {
        private static MongoClient GetClient()
        {
            const string connectionString = "mongodb+srv://eriknylund:SystemTILL2026!@ecluster.7zit7.mongodb.net/?retryWrites=true&w=majority&appName=ECluster";

            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));

            var client = new MongoClient(settings);

            return client;
        }

        public IMongoCollection<Movie> ConnectToDb(string libraryName)
        {
            var client = GetClient();

            var database = client.GetDatabase("MovieSafe");

            var movieInfo = database.GetCollection<Movie>($"{libraryName}");

            return movieInfo;
        }
        

    }
}
