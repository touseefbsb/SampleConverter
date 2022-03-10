using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Shapr3D.Converter.Controls
{
    // TODO
    public sealed partial class ConverterStateButton : UserControl
    {
        public ConverterStateButton() => InitializeComponent();
        public bool IsConverting
        {
            get => (bool)GetValue(IsConvertingProperty);
            set
            {
                SetValue(IsConvertingProperty, value);
                UpdateHideDefaultIcon();
            }
        }

        public static readonly DependencyProperty IsConvertingProperty =
            DependencyProperty.Register(nameof(IsConverting), typeof(bool), typeof(ConverterStateButton), new PropertyMetadata(false));

        public bool HideDefaultIcon
        {
            get => (bool)GetValue(HideDefaultIconProperty);
            set => SetValue(HideDefaultIconProperty, value);
        }
        public static readonly DependencyProperty HideDefaultIconProperty =
            DependencyProperty.Register(nameof(HideDefaultIcon), typeof(bool), typeof(ConverterStateButton), new PropertyMetadata(false));

        public bool IsDownloadAvailable
        {
            get => (bool)GetValue(IsDownloadAvailableProperty);
            set
            {
                SetValue(IsDownloadAvailableProperty, value);
                UpdateHideDefaultIcon();
            }
        }

        public static readonly DependencyProperty IsDownloadAvailableProperty =
            DependencyProperty.Register(nameof(IsDownloadAvailable), typeof(bool), typeof(ConverterStateButton), new PropertyMetadata(false));

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(nameof(Progress), typeof(double), typeof(ConverterStateButton), new PropertyMetadata(0.0));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ConverterStateButton), new PropertyMetadata(""));

        public string ConversionTimeStr
        {
            get => (string)GetValue(ConversionTimeStrProperty);
            set => SetValue(ConversionTimeStrProperty, value);
        }
        public static readonly DependencyProperty ConversionTimeStrProperty =
            DependencyProperty.Register("ConversionTimeStr", typeof(string), typeof(ConverterStateButton), new PropertyMetadata(null));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ConverterStateButton), new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ConverterStateButton), new PropertyMetadata(0));

        private void UpdateHideDefaultIcon() => HideDefaultIcon = IsConverting || IsDownloadAvailable;
    }
}
