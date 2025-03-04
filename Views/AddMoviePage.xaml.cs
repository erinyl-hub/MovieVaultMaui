using MovieVaultMaui.Models;

namespace MovieVaultMaui;

public partial class AddMoviePage : ContentPage
{
    private Models.Movie _movie;
    public AddMoviePage(Models.Movie movie)
    {


        InitializeComponent();
        UpdateConnectionStatus();
        LoadMovie(movie);
        MovieIsInSafe();

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

    private string ConvertRuneTime(string time)
    {
        int allTime = int.Parse(time.Replace(" min", ""));
        int houers = allTime / 60;
        int minutes = allTime % 60;
        string convertedTime = $"{houers}h {minutes}m";

        return convertedTime;
    }

    private void MovieHasBeenSeenBox(object sender, CheckedChangedEventArgs e)
    {
        movieSeenGrid.IsVisible = e.Value;
        whatSafe.Text = e.Value ? "Watched movie" : "Watch later";
    }

    private void LoadMovie(Models.Movie movie)
    {
        _movie = movie;
        movieImage.Source = _movie.Poster;
        movieName.Text = _movie.Title;
        imdbScore.Text = "Rating: " + _movie.imdbRating + "/10";
        movieYear.Text = "Year: " + _movie.Year;
        movieRuntime.Text = ConvertRuneTime(_movie.Runtime);
        movieDirector.Text = _movie.Director;
        moviePlot.Text = _movie.Plot;
    }

    private void AddMovieBox(object sender, EventArgs e)
    {
        DataManager.MongoDbManager mongoDbManager = new DataManager.MongoDbManager();

        if (myCheckBox.IsChecked)
        {
            _movie.UserData.UserRating = valueSlider.Value.ToString();
            _movie.UserData.SeeAgain = SeeAgain.IsChecked;
            _movie.UserData.UserReview = userReviewEditor.Text;
            _movie.UserData.AmountTimeSeen = 1;
            _movie.UserData.LastTimeSeen = DateTime.Now;
            _movie.MovieRegisterdTime = DateTime.Now;
            
            mongoDbManager.ConnectToDb("WatchedMovies").InsertOne(_movie);
            aplicationData.SeenMovies.Add(_movie);
            Navigation.PopToRootAsync();


        }
        else
        {
            _movie.MovieRegisterdTime = DateTime.Now;
            mongoDbManager.ConnectToDb("WatchLaterMovies").InsertOne(_movie);
            aplicationData.MoviesToSee.Add(_movie);
            Navigation.PopToRootAsync();

        }
    }

    private void MovieIsInSafe()
    {
        if(Helpers.MovieAlreadyInSafeChecker(_movie.imdbID))
        {
            movieInSafeSign.IsVisible = true;
            checkboxSeenMovie.IsVisible = false;
            whatSafe.IsVisible = false;
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        sliderValueLabel.Text = e.NewValue.ToString("0.0");
    }


}