namespace MovieVaultMaui.Views;
using MovieVaultMaui.Enums;
using System.Globalization;

public partial class PopupViewPage : ContentPage
{
    //private Models.Movie _movie;
    private ViewModels.AddMoviePageViewModel _movie;
    public PopupViewPage(Models.Movie movie, PopupViewPageSettingsType pageSettings)
    {
        InitializeComponent();


        _movie = new ViewModels.AddMoviePageViewModel(movie);
        BindingContext = _movie;
        
        MovieLength.Text = ConvertRuneTime(_movie.Movie.Runtime);
        GenresCollectionView.ItemsSource = Helpers.Spliter(_movie.Movie.Genre);
        ActorsCollectionView.ItemsSource = Helpers.Spliter(_movie.Movie.Actors);
        AdjustPage(pageSettings);

    }

    private async void ClosePopupClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    public static string ConvertRuneTime(string time)
    {
        int allTime = int.Parse(time.Replace(" min", ""));
        int houers = allTime / 60;
        int minutes = allTime % 60;
        string convertedTime = $"{houers}h {minutes}m";

        return convertedTime;
    }


    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        ratingValueLabel.Text = e.NewValue.ToString("0.0");

    }

    private async void AddToSeenMovies(object sender, EventArgs e)
    {
        _movie.Movie.UserData = CreateUserInfoOnMovie();
        Managers.DataManager.MoveMovieLibrary(_movie.Movie);
        await Navigation.PopModalAsync();
        MessagingCenter.Send(this, "UppdateView", _movie);
    }

    private void MovieJustSeen(object sender, EventArgs e)
    {
        _movie.Movie.UserData.AmountTimeSeen++;
        _movie.Movie.UserData.LastTimeSeen = DateTime.Now;
        _movie.Movie.Director = "Funkar";

        var test = _movie;

    }

    private void AdjustPage(PopupViewPageSettingsType settings)
    {

        switch (settings)
        {
            case PopupViewPageSettingsType.SeenMoviePageSettings:
                JustSeenView.IsVisible = true;
                UserRatingView.IsVisible = true;
                userInfoOnMovieView.IsVisible = true;
                break;

            case PopupViewPageSettingsType.WatchLaterPageSettings:

                userInfoOnMovieEditor.IsVisible = true;
               break;

        }

    }

    private Models.UserInfoOnMovie CreateUserInfoOnMovie()
    {
        Models.UserInfoOnMovie userInfoOnMovie = new Models.UserInfoOnMovie();

        userInfoOnMovie.UserRating = ratingValueLabel.Text.Replace(",", ".");
        userInfoOnMovie.SeeAgain = SeeAgain.IsChecked;
        userInfoOnMovie.UserReview = userReviewEditor.Text;
        userInfoOnMovie.AmountTimeSeen = 1;
        userInfoOnMovie.LastTimeSeen = DateTime.Now;

        return userInfoOnMovie;
    }
}