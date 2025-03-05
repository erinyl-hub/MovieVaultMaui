namespace MovieVaultMaui
{
    public partial class MainPage : ContentPage
    {



        public MainPage()
        {
            InitializeComponent();

            UpdateConnectionStatus();

            Helpers.GetDataFromDbAsync();

            Connectivity.ConnectivityChanged += (s, e) => UpdateConnectionStatus();
        }

        private void UpdateConnectionStatus()
        {
            bool isConnected = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

            ConnectionImage.Source = isConnected ? "online.png" : "offline.png";
        }

        private async void InAddMoviePageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChooseMovieToAddPage());
        }

        private async void InSafeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SafePage());
        }

    }

}
