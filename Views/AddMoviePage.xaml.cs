using MovieVaultMaui.Models;
using MovieVaultMaui.ViewModels;
using System.Globalization;
using MovieVaultMaui.Managers;
using MovieVaultMaui.Enums;

namespace MovieVaultMaui;

public partial class AddMoviePage : ContentPage
{
    private Models.Movie _movie;
    public AddMoviePage(Models.Movie movie)
    {
        InitializeComponent();
        UpdateConnectionStatus();
        

        _movie = movie;
        BindingContext = new ViewModels.AddMoviePageViewModel(movie);

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
        var databaseFacade = new DatabaseFacade();

        if (myCheckBox.IsChecked)
        {
            _movie.UserData = CreateUserInfoOnMovie();
            _movie.MovieRegisterdTime = DateTime.Now;

            databaseFacade.Execute(_movie, DatabaseAction.Add, MovieLibraryType.SeenMovies);
            Navigation.PopToRootAsync();

        }
        else
        {
            _movie.MovieRegisterdTime = DateTime.Now;
            databaseFacade.Execute(_movie, DatabaseAction.Add, MovieLibraryType.MoviesToSee);
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
        ratingSliderLabel.Text = e.NewValue.ToString("0.0");
    }

    private string ConvertRuneTime(string time)
    {
        int allTime = int.Parse(time.Replace(" min", ""));
        int houers = allTime / 60;
        int minutes = allTime % 60;
        string convertedTime = $"{houers}h {minutes}m";

        return convertedTime;
    }

    private Models.UserInfoOnMovie CreateUserInfoOnMovie()
    {
        Models.UserInfoOnMovie userInfoOnMovie = new Models.UserInfoOnMovie();

        userInfoOnMovie.UserRating = ratingSliderLabel.Text.Replace(",", ".");
        userInfoOnMovie.SeeAgain = SeeAgain.IsChecked;
        userInfoOnMovie.UserReview = userReviewEditor.Text;
        userInfoOnMovie.AmountTimeSeen = 1;
        userInfoOnMovie.LastTimeSeen = DateTime.Now;

        return userInfoOnMovie;
    }
}