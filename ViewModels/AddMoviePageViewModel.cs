using System.ComponentModel;

namespace MovieVaultMaui.ViewModels
{
    class AddMoviePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Models.Movie _movie;
        public Models.Movie Movie
        {
            get => _movie;
            set
            {
                if (_movie != value)
                {
                    _movie = value;
                    OnPropertyChanged(nameof(Movie));
                }
            }
        }

        public AddMoviePageViewModel(Models.Movie movie)
        {
            Movie = movie;
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
