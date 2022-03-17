using System;
using System.Windows.Input;
using MahApps.Metro.IconPacks;
using Microsoft.Toolkit.Mvvm.Input;
using Shapr3D.Converter.Enums;
using Shapr3D.Converter.Extensions;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shapr3D.Converter.Controls
{
    public sealed partial class FluentGridView : UserControl
    {
        private readonly PackIconMaterialDesign _listIcon = new PackIconMaterialDesign { Kind = PackIconMaterialDesignKind.ViewList };
        private readonly PackIconMaterialDesign _gridIcon = new PackIconMaterialDesign { Kind = PackIconMaterialDesignKind.ViewComfy };

        public event EventHandler<AutoSuggestBoxTextChangedEventArgs> SearchTextChanged;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public FluentGridView() => InitializeComponent();

        #region DPs
        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(FluentGridView), new PropertyMetadata(null));
        public ICommand DeleteAllCommand
        {
            get => (ICommand)GetValue(DeleteAllCommandProperty);
            set => SetValue(DeleteAllCommandProperty, value);
        }
        public static readonly DependencyProperty DeleteAllCommandProperty =
            DependencyProperty.Register("DeleteAllCommand", typeof(ICommand), typeof(FluentGridView), new PropertyMetadata(null));
        public object MySelectedItem
        {
            get => GetValue(MySelectedItemProperty);
            set => SetValue(MySelectedItemProperty, value);
        }
        public static readonly DependencyProperty MySelectedItemProperty =
            DependencyProperty.Register("MySelectedItem", typeof(object), typeof(FluentGridView), new PropertyMetadata(null));
        public object MyItemsSource
        {
            get => GetValue(MyItemsSourceProperty);
            set => SetValue(MyItemsSourceProperty, value);
        }
        public static readonly DependencyProperty MyItemsSourceProperty =
            DependencyProperty.Register("MyItemsSource", typeof(object), typeof(FluentGridView), new PropertyMetadata(null));
        public string MyTitle
        {
            get => (string)GetValue(MyTitleProperty);
            set => SetValue(MyTitleProperty, value);
        }
        public static readonly DependencyProperty MyTitleProperty =
            DependencyProperty.Register("MyTitle", typeof(string), typeof(FluentGridView), new PropertyMetadata(null));
        public double MyDesiredWidth
        {
            get => (double)GetValue(MyDesiredWidthProperty);
            set => SetValue(MyDesiredWidthProperty, value);
        }
        public static readonly DependencyProperty MyDesiredWidthProperty =
            DependencyProperty.Register("MyDesiredWidth", typeof(double), typeof(FluentGridView), new PropertyMetadata(null));
        public double MyItemHeight
        {
            get => (double)GetValue(MyItemHeightProperty);
            set => SetValue(MyItemHeightProperty, value);
        }
        public static readonly DependencyProperty MyItemHeightProperty =
            DependencyProperty.Register("MyItemHeight", typeof(double), typeof(FluentGridView), new PropertyMetadata(null));
        public DataTemplate MyItemTemplate
        {
            get => (DataTemplate)GetValue(MyItemTemplateProperty);
            set => SetValue(MyItemTemplateProperty, value);
        }
        public static readonly DependencyProperty MyItemTemplateProperty =
            DependencyProperty.Register("MyItemTemplate", typeof(DataTemplate), typeof(FluentGridView), new PropertyMetadata(null));
        public DataTemplateSelector MyItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(MyItemTemplateSelectorProperty);
            set => SetValue(MyItemTemplateSelectorProperty, value);
        }
        public static readonly DependencyProperty MyItemTemplateSelectorProperty =
            DependencyProperty.Register("MyItemTemplateSelector", typeof(DataTemplateSelector), typeof(FluentGridView), new PropertyMetadata(null));
        public int ItemsCount
        {
            get => (int)GetValue(ItemsCountProperty);
            set => SetValue(ItemsCountProperty, value);
        }
        public static readonly DependencyProperty ItemsCountProperty =
            DependencyProperty.Register("ItemsCount", typeof(int), typeof(FluentGridView), new PropertyMetadata(null));
        #endregion

        #region Events
        public ICommand ToggleViewCommand => new RelayCommand(OnToggleView);
        private void OnToggleView() => ReverseView();

        private void AppBarButton_Loaded(object sender, RoutedEventArgs e) => CheckView();

        private void CheckView()
        {
            switch (ApplicationData.Current.LocalSettings.ReadCurrentCollectionView())
            {
                case CurrentCollectionView.GridView:
                    TurnToGridView();
                    break;
                case CurrentCollectionView.ListView:
                    TurnToListView();
                    break;
            }
        }
        private void ReverseView()
        {
            switch (ApplicationData.Current.LocalSettings.ReadCurrentCollectionView())
            {
                case CurrentCollectionView.GridView:
                    TurnToListView();
                    break;
                case CurrentCollectionView.ListView:
                    TurnToGridView();
                    break;
            }
        }
        private void TurnToGridView()
        {
            viewChangeButton.Content = _listIcon;
            MyDesiredWidth = (double)Application.Current.Resources["GridDesiredWidth"];
            MyItemHeight = (double)Application.Current.Resources["GridItemHeight"];
            ApplicationData.Current.LocalSettings.SaveString(SettingsStorageExtensions.CollectionViewKey, nameof(CurrentCollectionView.GridView));
        }
        private void TurnToListView()
        {
            viewChangeButton.Content = _gridIcon;
            MyDesiredWidth = (double)Application.Current.Resources["ListDesiredWidth"];
            MyItemHeight = (double)Application.Current.Resources["ListItemHeight"];
            ApplicationData.Current.LocalSettings.SaveString(SettingsStorageExtensions.CollectionViewKey, nameof(CurrentCollectionView.ListView));
        }
        #endregion

        private void MyAdaptiveView_Loaded(object sender, RoutedEventArgs e) => MyAdaptiveView.ItemsPanelRoot.Margin = new Thickness(12, 12, 12, 0);

        private void MySearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) => SearchTextChanged?.Invoke(sender, args);

        private void MyAdaptiveView_SelectionChanged(object sender, SelectionChangedEventArgs e) => SelectionChanged?.Invoke(sender, e);
    }
}
