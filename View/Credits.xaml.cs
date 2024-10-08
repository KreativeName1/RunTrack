﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für Credits.xaml
    /// </summary>
    public partial class Credits : Page
    {
        private MainModel? _pmodel;
        public Credits()
        {
            InitializeComponent();
            _pmodel = FindResource("pmodel") as MainModel ?? new();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _pmodel?.Navigate(_pmodel.History[^1], false);
        }
    }


}
