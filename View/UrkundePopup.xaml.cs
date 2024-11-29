using FullControls.Controls;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

    public partial class UrkundePopup : MetroWindow
    {
        private PopupType? _type;
        private PopupButtons? _buttons;
        public bool? Result = null;

        public bool? Display(string title, string message, PopupType type, PopupButtons buttons)
        {
            _type = type;
            _buttons = buttons;

            InitializeComponent();
            Title = "";
            tbTitel.Content = title;
            tbMessage.Text = message;

            SetupButtons();


            this.Topmost = true;
            this.Activate();

            this.ShowDialog();

            return Result;
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
                Width = 110,
                Height = 30,
                Foreground = Brushes.White,
                ForegroundOnMouseOver = Brushes.White,
                BackgroundOnMouseOver = new SolidColorBrush(Color.FromArgb(255, 135, 155, 190)),
                Background = new SolidColorBrush(Color.FromArgb(255, 108, 124, 152)),
                Margin = new Thickness(5, 0, 5, 0)
            };
        }

        private void BTN_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BTN_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B42041"));
        }
        private void BTN_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            BTN_Close.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#009664"));
        }
    }
}
