using MovieVaultMaui.Models;

namespace MovieVaultMaui;

public partial class AddMoviePage : ContentPage
{
    private Models.Movie _movie;
    public AddMoviePage(Models.Movie movie)
    {


        InitializeComponent();
        UpdateConnectionStatus();
        BindingContext = movie;
        _movie = movie;
        MovieIsInSafe();

        movieRuntime.Text = ConvertRuneTime(movie.Runtime);
        ActorsCollectionView.ItemsSource = Helpers.Spliter(movie.Actors);
        GenresCollectionView.ItemsSource = Helpers.Spliter(movie.Genre);

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



    private void MovieHasBeenSeenBox(object sender, CheckedChangedEventArgs e)
    {
        movieSeenGrid.IsVisible = e.Value;
        whatSafe.Text = e.Value ? "Watched movie" : "Watch later";
    }


    private void AddMovieBox(object sender, EventArgs e)
    {
        
        

        if (myCheckBox.IsChecked)
        {
            _movie.UserData.UserRating = valueSlider.Value;
            _movie.UserData.SeeAgain = SeeAgain.IsChecked;
            _movie.UserData.UserReview = userReviewEditor.Text;
            _movie.UserData.AmountTimeSeen = 1;
            _movie.UserData.LastTimeSeen = DateTime.Now;
            _movie.MovieRegisterdTime = DateTime.Now;

            Managers.DataManager.ConnectToDb(Enums.MovieLibraryType.SeenMovies).InsertOne(_movie);
            Managers.DataManager.AddMovieToList(_movie, Enums.MovieListType.SeenMovies);
            Navigation.PopToRootAsync();

        }
        else
        {
            _movie.MovieRegisterdTime = DateTime.Now;
            Managers.DataManager.ConnectToDb(Enums.MovieLibraryType.MoviesToSee).InsertOne(_movie);
            Managers.DataManager.AddMovieToList(_movie, Enums.MovieListType.MoviesToSee);
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

    public static string ConvertRuneTime(string time)
    {
        int allTime = int.Parse(time.Replace(" min", ""));
        int houers = allTime / 60;
        int minutes = allTime % 60;
        string convertedTime = $"{houers}h {minutes}m";

        return convertedTime;
    }
}