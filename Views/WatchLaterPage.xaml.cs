using MovieVaultMaui.Enums;
using MovieVaultMaui.Models;
using MovieVaultMaui.Views;
using System.Collections.ObjectModel;
using MovieVaultMaui.Managers;

namespace MovieVaultMaui;

public partial class WatchLaterPage : ContentPage
{
    public List<string> SortOptionsPicker { get; } = new List<string> { "Last added", "Rating", "Alphabetically", "Length", "Year" };
    public List<string> SearchOptionsPicker { get; } = new List<string> { "Titel", "Director", "Actor", "Genre", "ImdbID" };

    private IEnumerable<Movie> _moviesToSeeList;
    public Dictionary<string, Func<string, IEnumerable<Movie>>> movieSearchDictionary;

    public bool changeMovieOrderBy = false;

    private int lastPage = 0;
    private int currentPage = 1;
    private int itemsPerPage = 28;

    private static TaskCompletionSource<bool> _tcs = new();
    private static WatchLaterPage _instanceWatchLaterPage;

    public WatchLaterPage()
    {
        InitializeComponent();
        InitializePage();
        Connectivity.ConnectivityChanged += (s, e) => UpdateConnectionStatus();
    }

    public async void InitializePage()
    {
        movieSearchDictionary = await
        SearchFilterManager.GetMovieSearchEngineDictionary(MovieLibraryType.MoviesToSee);
        _instanceWatchLaterPage = this;
        PickerSeter();
        UppdateMoviesViewed();
        UpdateConnectionStatus();
    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
        {
            await Navigation.PushModalAsync
                (new PopupViewPage(selectedMovie, PopupViewPageSettingsType.WatchLaterPageSettings, MovieLibraryType.MoviesToSee));
            ((CollectionView)sender).SelectedItem = null;
        }
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

    private void PickerSeter()
    {

        BindingContext = this;

        SearchOptionsPickerOnPage.SelectedIndexChanged -= OnPickerChanged;
        SortOptionsPickerOnPage.SelectedIndexChanged -= OnPickerChanged;

        SearchOptionsPickerOnPage.SelectedIndex = 0;
        SortOptionsPickerOnPage.SelectedIndex = 0;

        SearchOptionsPickerOnPage.SelectedIndexChanged += OnPickerChanged;
        SortOptionsPickerOnPage.SelectedIndexChanged += OnPickerChanged;
    }

    private void OnPickerChanged(object sender, EventArgs e)
    {
        UppdateMoviesViewed();
    }


    private void SearchEntryChange(object sender, TextChangedEventArgs e)
    {
        UppdateMoviesViewed();
    }


    private async void UppdateMoviesViewed()
    {
        string searchWord = SearchEntry.Text ?? "";
        currentPage = 1;

        _moviesToSeeList
             = Helpers.SearchEngine
             (movieSearchDictionary, SearchOptionsPickerOnPage.SelectedItem.ToString(),
             searchWord, SortOptionsPickerOnPage.SelectedItem.ToString(), changeMovieOrderBy);

        UpdatePageView();
        ResetPageCount();
    }

    private void ChangeOrderClicked(object sender, EventArgs e)
    {
        changeMovieOrderBy = changeMovieOrderBy ? false : true;
        UppdateMoviesViewed();
    }

    public void UpdatePageView()
    {
        var pagedMovies = _moviesToSeeList
            .Skip((currentPage - 1) * itemsPerPage)
            .Take(itemsPerPage);

        ObservableCollection<Movie> moviesToSeeObservableListView = new ObservableCollection<Movie>(pagedMovies);
        MoviesToSeeCollectionView.ItemsSource = moviesToSeeObservableListView;
    }

    public void NextPage(object sender, EventArgs e)
    {
        currentPage++;
        if (currentPage == (lastPage)) { GoForward.IsVisible = false; }

        GoBackwards.IsVisible = true;
        UpdatePageView();
    }

    public void PreviousPage(object sender, EventArgs e)
    {
        currentPage--;
        if (currentPage == 1) { GoBackwards.IsVisible = false; }

        GoForward.IsVisible = true;
        UpdatePageView();
    }

    public void ResetPageCount()
    {
        lastPage = (int)Math.Ceiling((_moviesToSeeList.Count() / (double)itemsPerPage));
        currentPage = 1;
        GoBackwards.IsVisible = false;
        if (lastPage < 2) { GoForward.IsVisible = false; }
        else { GoForward.IsVisible = true; }
    }


    private async Task UpdateViewOnChange()
    {
        await _tcs.Task;
        movieSearchDictionary =
        await SearchFilterManager.GetMovieSearchEngineDictionary(MovieLibraryType.MoviesToSee);
        UppdateMoviesViewed();
        _tcs = new TaskCompletionSource<bool>();
    }

    public static void SetDataWatchLaterPage()
    {
        _tcs.TrySetResult(true);
    }

    public async static Task RefreshWatchLaterPage()
    {
        _instanceWatchLaterPage?.UpdateViewOnChange();
    }


}