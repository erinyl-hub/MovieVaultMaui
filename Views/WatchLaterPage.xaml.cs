using MovieVaultMaui.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MovieVaultMaui;

public partial class WatchLaterPage : ContentPage
{
    public List<string> SortOptionsPicker { get; } = new List<string> { "Last added", "Rating", "Alphabetically", "Length", "Year" };
    public List<string> SearchOptionsPicker { get; } = new List<string> { "Titel", "Director", "Actor", "Genre", "ImdbID" };

    public Dictionary<string, Func<string, IEnumerable<Movie>>> movieSearchDictionary;


    public WatchLaterPage()
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


        ObservableCollection<Movie> MoviesToSeeObservableList =
            new ObservableCollection<Movie>(aplicationData.MoviesToSee.AsEnumerable().Reverse().ToList());
        MoviesToSeeCollectionView.ItemsSource = MoviesToSeeObservableList;
    }

    private void OnItemSelected(object sender, SelectionChangedEventArgs e)  // ta bort
    {
        if (e.CurrentSelection.FirstOrDefault() is Movie selectedItem)
        {
            DisplayAlert("Valt objekt", $"Du tryckte på: {selectedItem.Poster}", "OK");
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

        UppdateMovieView();
    }

    private void SearchEntryChange(object sender, TextChangedEventArgs e)
    {
        UppdateMovieView();
    }

    private async void CreateMovieSearchDictionary()
    {
        movieSearchDictionary = await Helpers.CreateSearchEngineDictionary(aplicationData.MoviesToSee.AsEnumerable().Reverse().ToList());

    }

    private async void UppdateMovieView()
    {


        string searchWord = SearchEntry.Text ?? "";


        ObservableCollection<Movie> MoviesToSeeObservableList
             = Helpers.SearchEngine
             (movieSearchDictionary, SearchOptionsPickerOnPage.SelectedItem.ToString(), searchWord, SortOptionsPickerOnPage.SelectedItem.ToString());

        MoviesToSeeCollectionView.ItemsSource = MoviesToSeeObservableList;

    }

   
}