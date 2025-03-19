using MovieVaultMaui.Enums;
using MovieVaultMaui.Managers;

namespace MovieVaultMaui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            UpdateConnectionStatus();

            var databaseFacade = new DatabaseFacade();
            databaseFacade.Execute(null, DatabaseAction.LoadData, MovieLibraryType.None);
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
