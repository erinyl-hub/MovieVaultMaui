namespace MovieVaultMaui.Views;
using MovieVaultMaui.Enums;
using System.Globalization;

public partial class PopupViewPage : ContentPage
{
    private Models.Movie _movie;
    public PopupViewPage(Models.Movie movie, PopupViewPageSettingsType pageSettings)
    {
        InitializeComponent();
        BindingContext = new ViewModels.AddMoviePageViewModel(movie);
        _movie = movie;
        MovieLength.Text = ConvertRuneTime(_movie.Runtime);
        GenresCollectionView.ItemsSource = Helpers.Spliter(_movie.Genre);
        ActorsCollectionView.ItemsSource = Helpers.Spliter(_movie.Actors);
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
        _movie.UserData = CreateUserInfoOnMovie();
        Managers.DataManager.MoveMovieLibrary(_movie);
        await Navigation.PopModalAsync();
        MessagingCenter.Send(this, "UppdateView", _movie);
    }

    private void MovieJustSeen(object sender, EventArgs e)
    {

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

        userInfoOnMovie.UserRating = double.Parse(ratingValueLabel.Text, CultureInfo.InvariantCulture);
        userInfoOnMovie.SeeAgain = SeeAgain.IsChecked;
        userInfoOnMovie.UserReview = userReviewEditor.Text;
        userInfoOnMovie.AmountTimeSeen = 1;
        userInfoOnMovie.LastTimeSeen = DateTime.Now;

        return userInfoOnMovie;
    }
}