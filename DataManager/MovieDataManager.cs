using System.Text.Json;


namespace MovieVaultMaui.DataManager
{

    internal class MovieDataManager
    {
        private readonly string apiKey = "123521ec";

        public async Task<Models.Movie> ConnectToMovieApi(string movieUrl)
        {
            string urlApi = $"http://www.omdbapi.com/?apikey={apiKey}&i={movieUrl}";
            Models.Movie movie = null;

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(urlApi);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();

                        movie = JsonSerializer.Deserialize<Models.Movie>(responseString);

                    }

                }
                catch (Exception error)
                {
                    Console.WriteLine($"Error: {error.Message}");
                }
            }

            return movie;
        }
    }
}




  