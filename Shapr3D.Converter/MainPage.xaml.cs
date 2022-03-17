using Shapr3D.Converter.ViewModels;
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

        private async void MainPage_Loaded(object sender, RoutedEventArgs e) => await ViewModel.InitAsync();

        private void FluentGridView_SearchTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if(e.Reason == AutoSuggestionBoxTextChangeReason.UserInput && sender is AutoSuggestBox box)
            {
                ViewModel.FilesCollectionView.Filter = string.IsNullOrWhiteSpace(box.Text)
                    ? (_ => true)
                    : (x => ((FileViewModel)x).Name.Contains(box.Text, System.StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}
