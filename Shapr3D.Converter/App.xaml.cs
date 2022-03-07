using System;
using Microsoft.EntityFrameworkCore;
using Shapr3D.Converter.EventMessages;
using Shapr3D.Converter.Helpers;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Shapr3D.Converter
{
    public sealed partial class App : Application
    {
        private EventBus eventBus;
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;

            eventBus = EventBus.GetInstance();

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                Window.Current.Activate();
            }
            using (var db = new Shapr3D_Converter.Models.Shapr3dDbContext())
            {
                db.Database.Migrate();
            }
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) => throw new Exception("Failed to load Page " + e.SourcePageType.FullName);

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            eventBus.Publish(new AppOnSuspendingMessage());
            deferral.Complete();
        }
    }
}
