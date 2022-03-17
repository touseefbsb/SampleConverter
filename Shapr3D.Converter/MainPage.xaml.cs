using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Shapr3D.Converter.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

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

        private void FluentGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is AdaptiveGridView gridview)
            {
                if (gridview.SelectedItem != null)
                {
                    var container = gridview.ContainerFromItem(e.AddedItems[0]);
                    if (container is UIElement gridViewItem)
                    {
                        ConnectedAnimationService.GetForCurrentView().DefaultDuration = TimeSpan.FromMilliseconds(500);
                        ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("GridViewForwardAnimation", gridViewItem);
                        var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("GridViewForwardAnimation");
                        if (animation != null)
                        {
                            animation.Configuration = new GravityConnectedAnimationConfiguration();
                            animation.TryStart(TitleTextBlock);
                        }
                    }
                }
            }
        }
    }
}
