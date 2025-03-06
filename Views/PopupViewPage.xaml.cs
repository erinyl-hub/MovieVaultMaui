namespace MovieVaultMaui.Views;

public partial class PopupViewPage : ContentPage
{
	public PopupViewPage(Models.Movie movie)
	{
		InitializeComponent();
        BindingContext = movie;
        MovieLength.Text = ConvertRuneTime(movie.Runtime);
        GenresCollectionView.ItemsSource = Helpers.SplitGenres(movie.Genre);
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
}