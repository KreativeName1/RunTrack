using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für TimeWarningPopup.xaml
    /// </summary>
    public partial class TimeWarningPopup : MetroWindow
    {
        private DispatcherTimer timer;

        // Konstruktor, der das Popup mit Titel und Nachricht initialisiert
        public TimeWarningPopup()
        {
            InitializeComponent();
            Title = "";

            // Initialisiere den Timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Aktualisiere jede Sekunde
            timer.Tick += Timer_Tick;
            timer.Start();

            // Event-Handler für den OK-Button
            btnOK.Click += (s, e) =>
            {
                timer.Stop();
                Close();
            };
        }

        // Event-Handler für den Timer-Tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            tbTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

       
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Fenster-Nachrichten abfangen
            var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_NCLBUTTONDOWN = 0xA1; // Nachricht für Mausklick auf Nicht-Client-Bereich
            const int HTCAPTION = 0x2;         // Titelbereich

            // Bewegung unterbinden
            if (msg == WM_NCLBUTTONDOWN && wParam.ToInt32() == HTCAPTION)
            {
                handled = true; // Blockiere die Nachricht
            }

            return IntPtr.Zero;
        }
    }
}
