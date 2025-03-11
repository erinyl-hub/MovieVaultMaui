using MovieVaultMaui.Models;
using MovieVaultMaui.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MovieVaultMaui;

public partial class SeenMoviesPage : ContentPage
{

    public List<string> SortOptionsPicker { get; } = new List<string> { "Last added", "Rating", "Alphabetically", "Length", "Year" };
    public List<string> SearchOptionsPicker { get; } = new List<string> { "Titel", "Director", "Actor", "Genre", "ImdbID" };

    public IEnumerable<Movie> SeenMoviesList;
    public Dictionary<string, Func<string, IEnumerable<Movie>>> movieSearchDictionary;
    public bool changeMovieOrderBy = false;

    private int lastPage = 0;
    private int currentPage = 1;
    private int itemsPerPage = 28;

    public SeenMoviesPage()
	{
        InitializeComponent();
        InitializePage();


        Connectivity.ConnectivityChanged += (s, e) => UpdateConnectionStatus();

    }

    public void InitializePage()
    {
        CreateMovieSearchDictionary();
        PickerSeter();
        UppdateMoviesViewed();
        UpdateConnectionStatus();

    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Movie selectedMovie)
        {
            await Navigation.PushModalAsync(new PopupViewPage(selectedMovie));
            ((CollectionView)sender).SelectedItem = null;
        }
    }

    private async void CreateMovieSearchDictionary()
    {
        var seenMovies = Managers.DataManager.GetMovieList(Enums.MovieListType.SeenMovies);
        movieSearchDictionary = await Helpers.CreateSearchEngineDictionary(seenMovies.AsEnumerable().Reverse().ToList());
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

        SeenMoviesList
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
        var pagedMovies = SeenMoviesList
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
        lastPage = (int)Math.Ceiling((SeenMoviesList.Count() / (double)itemsPerPage));
        currentPage = 1;
        GoBackwards.IsVisible = false;
        if (lastPage < 2) { GoForward.IsVisible = false; }
        else { GoForward.IsVisible = true; }
    }
}