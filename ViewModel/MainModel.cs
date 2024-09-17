using System.Windows;
using System.Windows.Controls;

namespace RunTrack
{
    public class MainModel : BaseModel
    {
        private object? _currentPage;
        private string? _pageTitle;

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

        private Benutzer? _benutzer;

        public Benutzer Benutzer
        {
            get
            {
                return this._benutzer ?? new Benutzer();
            }
            set
            {
                this._benutzer = value;
                OnPropertyChanged("Benutzer");
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

        public string? PageTitle
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
            PageTitle = (CurrentPage as Page)?.Title;

            Window window = Application.Current.MainWindow;
            if (window != null)
            {
                window.SizeToContent = SizeToContent.WidthAndHeight;

                if (CurrentPage.GetType().GetProperty("ResizeMode") != null)
                {
                    if (CurrentPage.GetType()?.GetProperty("ResizeMode")?.GetValue(CurrentPage)?.ToString() == "CanResize")
                    {
                        window.ResizeMode = ResizeMode.CanResize;
                    }
                    else
                    {
                        window.ResizeMode = ResizeMode.NoResize;
                    }

                }
                else
                {
                    window.ResizeMode = ResizeMode.CanResize;
                }
            }



        }

    }
}
