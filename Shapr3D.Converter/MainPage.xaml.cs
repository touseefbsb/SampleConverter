﻿using Shapr3D.Converter.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Shapr3D.Converter
{

    public sealed partial class MainPage : Page
    {

        public MainViewModel ViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            DataContext = ViewModel;

            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.InitAsync();
        }
    }
}
