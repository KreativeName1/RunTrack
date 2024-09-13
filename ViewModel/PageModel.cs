namespace RunTrack
{
    public class PageModel : BaseModel
    {
        private object? _currentPage;
        private string _pageTitle;

        private List<object> _history = new List<object>();

        public object? CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        public List<object> History
        {
            get { return _history; }
            set
            {
                _history = value;
                OnPropertyChanged("History");
            }
        }

        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                _pageTitle = value;
                OnPropertyChanged("PageTitle");
            }
        }



        public void Navigate(object page, bool? addToHistory = true)
        {
            if (page == null) return;
            if (CurrentPage != null && addToHistory == true)
            {
                History.Add(CurrentPage);
            }
            CurrentPage = page;



        }

    }
}
