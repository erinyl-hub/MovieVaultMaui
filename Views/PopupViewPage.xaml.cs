namespace MovieVaultMaui.Views;

public partial class PopupViewPage : ContentPage
{
	public PopupViewPage(Models.Movie movie)
	{
		InitializeComponent();
        BindingContext = movie;
        MovieLength.Text = ConvertRuneTime(movie.Runtime);
        GenresCollectionView.ItemsSource = Helpers.Spliter(movie.Genre);
        ActorsCollectionView.ItemsSource = Helpers.Spliter(movie.Actors);

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
        sliderValueLabel.Text = e.NewValue.ToString("0.0");
    }

    private void AddToSeenMovies(object sender, EventArgs e)
    {


    }
}