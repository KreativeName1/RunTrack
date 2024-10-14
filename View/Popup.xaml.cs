using FullControls.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace RunTrack
{
  /// <summary>
  /// Interaktionslogik für PopupWindow.xaml
  /// </summary>
  ///
  public enum PopupType
  {
    Error,
    Info,
    Warning,
    Question,
    Success,
  }
  public enum PopupButtons
  {
    Ok,
    OkCancel,
    YesNo,
    YesNoCancel,
  }

  public class PopupImage
  {
    private PopupImage(string value) { Value = value; }

    public string Value { get; private set; }

    public static PopupImage Info { get { return new PopupImage("pack://application:,,,/Images/popupIcons/info.png"); } }
    public static PopupImage Error { get { return new PopupImage("pack://application:,,,/Images/popupIcons/error.png"); } }
    public static PopupImage Warning { get { return new PopupImage("pack://application:,,,/Images/popupIcons/warning.png"); } }
    public static PopupImage Question { get { return new PopupImage("pack://application:,,,/Images/popupIcons/question.png"); } }
    public static PopupImage Success { get { return new PopupImage("pack://application:,,,/Images/popupIcons/success.png"); } }

    public override string ToString()
    {
      return Value;
    }
  }

  public partial class Popup : Window
  {
    private PopupType? _type;
    private PopupButtons? _buttons;
    public bool? Result = null;

    public bool? Display(string title, string message, PopupType type, PopupButtons buttons)
    {
      _type = type;
      _buttons = buttons;

      InitializeComponent();
      Title = title;
      tbTitel.Content = title;
      tbMessage.Text = message;

      SetupIcon();
      SetupButtons();


      this.Topmost = true;
      this.Activate();
      StartAnimations();

      this.ShowDialog();

      return Result;
    }

    private void StartAnimations()
    {
      var fadeInStoryboard = (Storyboard)this.Resources["FadeInStoryboard"];
      fadeInStoryboard.Begin(this);

      var zoomInStoryboard = (Storyboard)this.Resources["ZoomInStoryboard"];
      zoomInStoryboard.Begin(this);

      var slideInStoryboard = (Storyboard)this.Resources["SlideInStoryboard"];
      slideInStoryboard.Begin(this);

      var backgroundColorStoryboard = (Storyboard)this.Resources["BackgroundColorStoryboard"];
      backgroundColorStoryboard.Begin(this);
    }

    private void SetupIcon()
    {
      switch (_type)
      {
        case PopupType.Success:

          imgIcon.Source = new BitmapImage(new Uri(PopupImage.Success.ToString()));
          break;
        case PopupType.Info:
          imgIcon.Source = new BitmapImage(new Uri(PopupImage.Info.ToString()));
          break;

        case PopupType.Warning:
          imgIcon.Source = new BitmapImage(new Uri(PopupImage.Warning.ToString()));
          break;

        case PopupType.Error:
          imgIcon.Source = new BitmapImage(new Uri(PopupImage.Error.ToString()));
          break;

        case PopupType.Question:
          imgIcon.Source = new BitmapImage(new Uri(PopupImage.Question.ToString()));
          break;
      }
    }
    private void SetupButtons()
    {
      switch (_buttons)
      {
        case PopupButtons.Ok:
          ButtonPlus button1 = CreateButton("btnOK", "OK");
          button1.Click += (s, e) =>
          {
            Result = true;
            this.Close();
          };
          spButtons.Children.Add(button1);
          break;
        case PopupButtons.OkCancel:
          ButtonPlus button2 = CreateButton("btnOK", "OK");
          button2.Click += (s, e) =>
          {
            Result = true;
            this.Close();
          };
          ButtonPlus button3 = CreateButton("btnCancel", "Abbrechen");
          button3.Click += (s, e) =>
          {
            Result = false;
            this.Close();
          };
          spButtons.Children.Add(button2);
          spButtons.Children.Add(button3);
          break;
        case PopupButtons.YesNo:
          ButtonPlus button4 = CreateButton("btnYes", "Ja");
          button4.Click += (s, e) =>
          {
            Result = true;
            this.Close();
          };
          ButtonPlus button5 = CreateButton("btnNo", "Nein");
          button5.Click += (s, e) =>
          {
            Result = false;
            this.Close();
          };
          spButtons.Children.Add(button4);
          spButtons.Children.Add(button5);
          break;
        case PopupButtons.YesNoCancel:
          ButtonPlus button6 = CreateButton("btnYes", "Ja");
          button6.Click += (s, e) =>
          {
            Result = true;
            this.Close();
          };
          ButtonPlus button7 = CreateButton("btnNo", "Nein");
          button7.Click += (s, e) =>
          {
            Result = false;
            this.Close();
          };
          ButtonPlus button8 = CreateButton("btnCancel", "Abbrechen");
          button8.Click += (s, e) =>
          {
            Result = null;
            this.Close();
          };
          spButtons.Children.Add(button6);
          spButtons.Children.Add(button7);
          spButtons.Children.Add(button8);
          break;
      }
    }

    private ButtonPlus CreateButton(string name, string content)
    {
      return new ButtonPlus
      {
        Name = name,
        Content = content,
        Width = 80,
        Height = 30,
        Foreground = Brushes.White,
        ForegroundOnMouseOver = Brushes.White,
        BackgroundOnMouseOver = new SolidColorBrush(Color.FromArgb(255, 135, 155, 190)),
        Background = new SolidColorBrush(Color.FromArgb(255, 108, 124, 152))
      };
    }
  }
}
