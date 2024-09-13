namespace RunTrack
{
    public class PageModel : BaseModel
    {
        private object? _currentPage;
        private double _windowHeight = 300;
        private double _windowWidth = 400;

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

        public double WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                _windowHeight = value;
                OnPropertyChanged("WindowHeight");
            }
        }

        public double WindowWidth
        {
            get { return _windowWidth; }
            set
            {
                _windowWidth = value;
                OnPropertyChanged("WindowWidth");
            }
        }


        public void Navigate(object page)
        {
            if (page == null) return;
            if (CurrentPage != null)
            {
                History.Add(CurrentPage);
            }
            CurrentPage = page;
            // center page on screen



        }

    }
}
