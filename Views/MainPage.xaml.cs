using MovieVaultMaui.Enums;
using MovieVaultMaui.Managers;

namespace MovieVaultMaui
{
    public partial class MainPage : ContentPage
    {
        private bool _dataLoaded;

        public MainPage()
        {
            InitializeComponent();

            UpdateConnectionStatus();

            var dataManager = new DataManager();
            var libraryFacade = new MovieLibraryFacade(dataManager);
            libraryFacade.LoadMoviesData();
            SearchFilterManager.FilSearchEngineDictionary(DataManager.DataLoaded);

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

        private async void InWatchLaterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new WatchLaterPage());
        }

        private async void InSeenMoviesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SeenMoviesPage());
        }



    }

}
