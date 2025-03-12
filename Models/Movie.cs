

namespace MovieVaultMaui.Models
{

    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Poster { get; set; }
        public string imdbRating { get; set; }
        public string imdbID { get; set; }

        public UserInfoOnMovie UserData { get; set; } = new UserInfoOnMovie();
        public DateTime MovieRegisterdTime { get; set; }

    }

    public class UserInfoOnMovie
    {
        public double? UserRating { get; set; }
        public bool SeeAgain { get; set; }
        public string? UserReview { get; set; }
        public int? AmountTimeSeen { get; set; }
        public DateTime? LastTimeSeen { get; set; }

    }

}
