namespace MovieVaultMaui.Views;

public partial class PopupViewPage : ContentPage
{
	public PopupViewPage(Models.Movie movie)
	{
		InitializeComponent();
        BindingContext = movie;

    }

    private async void ClosePopupClicked(object sender, TappedEventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}