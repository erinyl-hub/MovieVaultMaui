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

        InitiatePageValues(pageSettings);
        AdjustPage(pageSettings);

    }

    private async void InitiatePageValues(PopupViewPageSettingsType pageSettings)
    {
        MovieLength.Text = ConvertRuneTime(_movie.Movie.Runtime);
        GenresCollectionView.ItemsSource = Helpers.Spliter(_movie.Movie.Genre);
        ActorsCollectionView.ItemsSource = Helpers.Spliter(_movie.Movie.Actors);

        //if (pageSettings == PopupViewPageSettingsType.SeenMoviePageSettings)
        //{
        //    RatingValueSlider.Value = double.Parse((_movie.Movie.UserData.UserRating.Replace(".", ",")));
        //}
        //else { RatingValueLabel.Text = "0.0"; }


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
        RatingValueLabel.Text = e.NewValue.ToString("0.0");

    }

    private async void OnClickedAddToSeenMovies(object sender, EventArgs e)
    {
        _movie.Movie.UserData = CreateUserInfoOnMovie();
        Managers.DataManager.MoveMovieLibrary(_movie.Movie);
        await Navigation.PopModalAsync();
        MessagingCenter.Send(this, "UppdateView", _movie);
    }

    private void OnClickedMovieJustSeen(object sender, EventArgs e) // funkar ej
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
                EditMovieReviewButton.IsVisible = true;
                RemoveMovieButton.IsVisible = true;
                break;

            case PopupViewPageSettingsType.WatchLaterPageSettings:

                userInfoOnMovieEditor.IsVisible = true;
                AddToSeenMoviesButton.IsVisible = true;
                RemoveMovieButton.IsVisible = true;
                break;

            case PopupViewPageSettingsType.SeenMoviePageEditSettings:

                userInfoOnMovieEditor.IsVisible = true;
                SaveEditedMovieButton.IsVisible = true;
                CancelEditOfMovie.IsVisible = true;

                break;

            case PopupViewPageSettingsType.ClearPageSettings:

                JustSeenView.IsVisible = false;
                UserRatingView.IsVisible = false;
                userInfoOnMovieView.IsVisible = false;
                EditMovieReviewButton.IsVisible = false;
                AddToSeenMoviesButton.IsVisible = false;
                SaveEditedMovieButton.IsVisible = false;
                userInfoOnMovieEditor.IsVisible = false;
                RemoveMovieButton.IsVisible = false;
                CancelEditOfMovie.IsVisible = false;

                break;

        }
    }

    private Models.UserInfoOnMovie CreateUserInfoOnMovie()
    {
        Models.UserInfoOnMovie userInfoOnMovie = new Models.UserInfoOnMovie();

        userInfoOnMovie.UserRating = RatingValueLabel.Text.Replace(",", ".");
        userInfoOnMovie.SeeAgain = SeeAgainCheckBox.IsChecked;
        userInfoOnMovie.UserReview = userReviewEditor.Text;
        userInfoOnMovie.AmountTimeSeen = 1;
        userInfoOnMovie.LastTimeSeen = DateTime.Now;

        return userInfoOnMovie;
    }

    private void OnClickedEditMovieReview(object sender, EventArgs e)
    {
        RatingValueLabel.Text = _movie.Movie.UserData.UserRating;
        RatingValueSlider.Value = double.Parse((_movie.Movie.UserData.UserRating.Replace(".", ",")));
        userReviewEditor.Text = _movie.Movie.UserData.UserReview;
        SeeAgainCheckBox.IsChecked = _movie.Movie.UserData.SeeAgain;

        AdjustPage(PopupViewPageSettingsType.ClearPageSettings);
        AdjustPage(PopupViewPageSettingsType.SeenMoviePageEditSettings);
    }

    private void OnClickedRemoveMovie(object sender, EventArgs e)
    {

    }

    private void OnClickedSaveEditedMovie(object sender, EventArgs e)
    {
        
    }

    private void OnClickedCancelEditOfMovie(object sender, EventArgs e)
    {
        AdjustPage(PopupViewPageSettingsType.ClearPageSettings);
        AdjustPage(PopupViewPageSettingsType.SeenMoviePageSettings);

    }
}