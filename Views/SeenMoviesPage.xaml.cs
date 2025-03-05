using MovieVaultMaui.Models;
using System.Collections.ObjectModel;

namespace MovieVaultMaui;


public partial class SeenMoviesPage : ContentPage
{

    public List<string> SortOptionsPicker { get; } = new List<string> { "Last added", "Rating", "Alphabetically", "Length", "Year" };
    public List<string> SearchOptionsPicker { get; } = new List<string> { "Titel", "Director", "Actor", "Genre", "ImdbID" };

    public IEnumerable<Movie> moviesToSeeList;
    public Dictionary<string, Func<string, IEnumerable<Movie>>> movieSearchDictionary;
    public bool changeMovieOrderBy = false;

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
        UpdateConnectionStatus();

    }

    private void OnItemSelected(object sender, SelectionChangedEventArgs e)  // ta bort
    {
        if (e.CurrentSelection.FirstOrDefault() is Movie selectedItem)
        {
            DisplayAlert("Valt objekt", $"Du tryckte på: {selectedItem.Title}  {selectedItem.Actors} {selectedItem.Runtime}", "OK");
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
        SearchOptionsPickerOnPage.SelectedIndex = 0;
        SortOptionsPickerOnPage.SelectedIndex = 0;
    }

    private void OnPickerChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        string selectedOption = picker.SelectedItem.ToString();
    }

    private void OnSortChanged(object sender, EventArgs e)
    {
        OnPickerChanged(sender, e);
        changeMovieOrderBy = false;
        UppdateMoviesViewed();
    }

    private void SearchEntryChange(object sender, TextChangedEventArgs e)
    {
        UppdateMoviesViewed();
    }

    private async void CreateMovieSearchDictionary()
    {
        movieSearchDictionary = await Helpers.CreateSearchEngineDictionary(aplicationData.MoviesToSee.AsEnumerable().Reverse().ToList());
    }

    private async void UppdateMoviesViewed()
    {
        string searchWord = SearchEntry.Text ?? "";
        currentPage = 1;

        moviesToSeeList
             = Helpers.SearchEngine
             (movieSearchDictionary, SearchOptionsPickerOnPage.SelectedItem.ToString(),
             searchWord, SortOptionsPickerOnPage.SelectedItem.ToString(), changeMovieOrderBy);

        UpdateCollectionView();
    }

    private void ChangeOrderClicked(object sender, EventArgs e)
    {
        changeMovieOrderBy = changeMovieOrderBy ? false : true;
        UppdateMoviesViewed();
    }

    public void UpdateCollectionView()
    {
        var pagedMovies = moviesToSeeList
            .Skip((currentPage - 1) * itemsPerPage)
            .Take(itemsPerPage);

        ObservableCollection<Movie> moviesToSeeObservableListView = new ObservableCollection<Movie>(pagedMovies);
        MoviesToSeeCollectionView.ItemsSource = moviesToSeeObservableListView;
    }

    public void NextPage(object sender, EventArgs e)
    {
        currentPage++;
        UpdateCollectionView();
        GoBackwards.IsVisible = true;

    }

    public void PreviousPage(object sender, EventArgs e)
    {
        if (currentPage == 1) { GoBackwards.IsVisible = false; }
        currentPage--;
        UpdateCollectionView();
    }


}
