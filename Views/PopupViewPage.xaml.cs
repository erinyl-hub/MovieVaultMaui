namespace MovieVaultMaui.Views;
using MovieVaultMaui.Enums;
using MovieVaultMaui.Managers;


public partial class PopupViewPage : ContentPage
{
    private ViewModels.AddMoviePageViewModel _movie;
    private MovieLibraryType _movielibraryType;
    public PopupViewPage(Models.Movie movie, PopupViewPageSettingsType pageSettings, MovieLibraryType movielibraryType)
    {
        InitializeComponent();

        _movie = new ViewModels.AddMoviePageViewModel(movie);
        BindingContext = _movie;
        _movielibraryType = movielibraryType;

        InitiatePageValues();
        AdjustPage(pageSettings);
    }

    private async void InitiatePageValues()
    {
        MovieLength.Text = ConvertRuneTime(_movie.Movie.Runtime);
        GenresCollectionView.ItemsSource = Helpers.Spliter(_movie.Movie.Genre);
        ActorsCollectionView.ItemsSource = Helpers.Spliter(_movie.Movie.Actors);
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

        var databaseFacade = new Managers.DatabaseFacade();
        databaseFacade.Execute(_movie.Movie, DatabaseAction.Move, MovieLibraryType.SeenMovies);

        await Navigation.PopModalAsync();
        MessagingCenter.Send(this, "UppdateView", _movie);
    }

    private void OnClickedMovieJustSeen(object sender, EventArgs e) // funkar ej
    {
        LastTimeSeen.Text = (_movie.Movie.UserData.LastTimeSeen = DateTime.Now).ToString("yyyy-MM-dd");
        _movie.Movie.UserData.AmountTimeSeen++;
        AmountTimeSeen.Text = _movie.Movie.UserData.AmountTimeSeen.ToString();

        var databaseFacade = new DatabaseFacade();
        databaseFacade.Execute(_movie.Movie, DatabaseAction.Update, _movielibraryType);
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

    private async void OnClickedRemoveMovie(object sender, EventArgs e)
    {
        var databaseFacade = new DatabaseFacade();
        databaseFacade.Execute(_movie.Movie, DatabaseAction.Remove, _movielibraryType);
        SearchFilterManager.updateDictionary(_movielibraryType);

        await Navigation.PopModalAsync();
        await Task.Delay(5000);
        MessagingCenter.Send(this as PopupViewPage, "UppdateView");
    }

    private void OnClickedSaveEditedMovie(object sender, EventArgs e)
    {
        _movie.Movie.UserData.UserRating = RatingValueLabel.Text.Replace(",", ".");
        _movie.Movie.UserData.SeeAgain = SeeAgainCheckBox.IsChecked;
        _movie.Movie.UserData.UserReview = userReviewEditor.Text;

        var databaseFacade = new DatabaseFacade();
        databaseFacade.Execute(_movie.Movie, DatabaseAction.Update, _movielibraryType);

        UserRatingView.Text = $"Your Rating: {RatingValueLabel.Text}";
        UserReviewView.Text = userReviewEditor.Text;

        AdjustPage(PopupViewPageSettingsType.ClearPageSettings);
        AdjustPage(PopupViewPageSettingsType.SeenMoviePageSettings);

    }

    private void OnClickedCancelEditOfMovie(object sender, EventArgs e)
    {
        AdjustPage(PopupViewPageSettingsType.ClearPageSettings);
        AdjustPage(PopupViewPageSettingsType.SeenMoviePageSettings);

    }
}