using System.Text.RegularExpressions;

namespace MovieVaultMaui;

public partial class ChooseMovieToAddPage : ContentPage
{
	public ChooseMovieToAddPage()
	{
		InitializeComponent();
        UpdateConnectionStatus();
        Connectivity.ConnectivityChanged += (s, e) => UpdateConnectionStatus();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
       
    }

    private void UpdateConnectionStatus()
    {
        bool isConnected = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

        ConnectionImage.Source = isConnected ? "online.png" : "offline.png";
    }

    private async void ImdbEntryButtonClicked(object sender, EventArgs e) // skriv om 
    {
        string inputImdbUrl = ImdbEntry.Text;
        string ImdbId = RightFormatImdbUrl(inputImdbUrl);

        if (ImdbId == null)
        {
            ErrorMsg.Text = "Invalid Imdb Url";
        }
        else
        {
            DataManager.MovieDataManager movieDataManager = new DataManager.MovieDataManager();
            Models.Movie movie = await movieDataManager.ConnectToMovieApi(ImdbId);
            if(movie == null)
            {
                ErrorMsg.Text = "Invalid Imdb Url";
            }
            else
            {
                await Navigation.PushAsync(new AddMoviePage(movie));

            }
        }
    }

    private string RightFormatImdbUrl(string ImdbUrl)
    {
        if(ImdbUrl == null) { return null; }

        string ImdbId = "(?<=title\\/)(tt\\d{7,8})(?=\\b)";
        Regex data = new Regex(ImdbId);
        MatchCollection matches = data.Matches(ImdbUrl);

        if (matches.Count == 1) 
        { 
            return matches[0].Value; 
        }

        else{ return null; }
    }
}