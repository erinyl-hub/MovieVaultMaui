namespace MovieVaultMaui;


public partial class SafePage : ContentPage
{
    public SafePage()
    {
        InitializeComponent();
        UpdateConnectionStatus();
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

    private async void InWatchLaterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WatchLaterPage());
    }

    private async void InSeenMoviesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SeenMoviesPage());
    }




}
