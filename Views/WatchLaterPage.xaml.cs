using MovieVaultMaui.Models;
using System.Collections.ObjectModel;

namespace MovieVaultMaui;

public partial class WatchLaterPage : ContentPage
{
    public List<string> SortOptions { get; set; } = new List<string> { "Last added", "Rating", "Length" };
    public List<string> SearchOptions { get; set; } = new List<string> { "Titel", "Director", "Actor" , "Genre" };


    public WatchLaterPage()
    {
        InitializeComponent();
        UpdateConnectionStatus();
        Connectivity.ConnectivityChanged += (s, e) => UpdateConnectionStatus();
        BindingContext = this;
        SearchOptionsPicker.SelectedIndex = 0;
        SortOptionsPicker.SelectedIndex = 0;
        




        ObservableCollection<Movie> MoviesToSeeObservableList = new ObservableCollection<Movie>(aplicationData.MoviesToSee);

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

    private void OnSortChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        string selectedOption = picker.SelectedItem.ToString();
        // Lägg till sorteringslogik här
    }



}