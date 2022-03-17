using System;
using Shapr3D.Converter.Enums;
using Shapr3D.Converter.Extensions;
using Shapr3D.Converter.ViewModels;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shapr3D.Converter.Controls.DataTemplates
{
    public sealed partial class FileTemplate : UserControl
    {
        public FileViewModel FileViewModel => DataContext as FileViewModel;
        public FileTemplate()
        {
            InitializeComponent();
            DataContextChanged += (s, e) => Bindings.Update();
        }
        public bool IgnoreCheckView
        {
            get => (bool)GetValue(IgnoreCheckViewProperty);
            set => SetValue(IgnoreCheckViewProperty, value);
        }

        public static readonly DependencyProperty IgnoreCheckViewProperty =
            DependencyProperty.Register("IgnoreCheckView", typeof(bool), typeof(FileTemplate), new PropertyMetadata(null));
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CheckView();
            SettingsStorageExtensions.OnCollectionViewSelected += SettingsStorageExtensions_OnCollectionViewSelected;
        }
        private void SettingsStorageExtensions_OnCollectionViewSelected(object sender, System.EventArgs e) => CheckView();
        private void UserControl_Unloaded(object sender, RoutedEventArgs e) => SettingsStorageExtensions.OnCollectionViewSelected -= SettingsStorageExtensions_OnCollectionViewSelected;
        private void CheckView()
        {
            if (!IgnoreCheckView)
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
        }
        private void TurnToGridView()
        {
            GridVieww.Visibility = Visibility.Visible;
            ConnectedAnimationService.GetForCurrentView().DefaultDuration = TimeSpan.FromMilliseconds(500);
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("ListToGridAnimation", ListVieww);
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ListToGridAnimation");
            if (animation != null)
            {
                animation.Configuration = new GravityConnectedAnimationConfiguration();
                animation.TryStart(GridVieww);
            }
            ListVieww.Visibility = Visibility.Collapsed;
        }
        private void TurnToListView()
        {
            ListVieww.Visibility = Visibility.Visible;
            ConnectedAnimationService.GetForCurrentView().DefaultDuration = TimeSpan.FromMilliseconds(500);
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("GridToListAnimation", GridVieww);
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("GridToListAnimation");
            if (animation != null)
            {
                animation.Configuration = new GravityConnectedAnimationConfiguration();
                animation.TryStart(ListVieww);
            }
            GridVieww.Visibility = Visibility.Collapsed;
        }
    }
}