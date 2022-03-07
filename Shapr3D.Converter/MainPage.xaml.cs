using Shapr3D.Converter;
using Shapr3D.Converter.Datasource;
using Shapr3D.Converter.Models;
using Shapr3D.Converter.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


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
