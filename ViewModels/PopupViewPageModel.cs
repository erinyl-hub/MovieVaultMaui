using System.ComponentModel;

namespace MovieVaultMaui.ViewModels
{
    class PopupViewPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Models.Movie _movie;
        public Models.Movie Movie
        {
            get => _movie;
            set
            {
                _movie = value;
                OnPropertyChanged(nameof(Movie));
            }
        }

        public PopupViewPageModel(Models.Movie movie)
        {
            Movie = movie;
        }


        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
}
