using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Windows.UI.Popups;

namespace Shapr3D.Converter.Helpers
{
    public static class AppCenterHelper
    {
        public static void TrackException(string exceptionName, Exception ex)
        {
            var exceptionProps = new Dictionary<string, string> { { "ExceptionName", exceptionName }, { "ExceptionMessage", ex.Message }, { "StackTrace", ex.StackTrace } };
            Crashes.TrackError(ex, exceptionProps);
        }

        public static async Task TrackExceptionAndShowErrorDialogAsync(string exceptionName, Exception ex, string messageDialogContent)
        {
            TrackException(exceptionName, ex);
            await new MessageDialog(messageDialogContent) { Title = exceptionName }.ShowAsync();
        }

        public static void TrackEvent(string eventName) => Analytics.TrackEvent(eventName);
    }
}
