using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieVaultMaui.ViewModels
{
    class AddMoviePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private Models.UserInfoOnMovie _userInfo;
        
        public Models.UserInfoOnMovie UserInfo
        {
            get => _userInfo;
            set
            {
                if (_userInfo != value)
                {
                    _userInfo = value;
                    OnPropertyChanged(nameof(UserInfo));
                }
            }
        }

        public AddMoviePageViewModel()
        {
            UserInfo = new Models.UserInfoOnMovie();
        }


        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
 