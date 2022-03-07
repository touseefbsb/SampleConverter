using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Shapr3D.Converter.Controls
{
    public sealed partial class Flag : UserControl
    {
        public Flag() => InitializeComponent();
        public object FlagContent
        {
            get => GetValue(FlagContentProperty);
            set => SetValue(FlagContentProperty, value);
        }
        public static readonly DependencyProperty FlagContentProperty =
            DependencyProperty.Register("FlagContent", typeof(object), typeof(Flag), new PropertyMetadata(null));
        public Color FlagColor
        {
            get => (Color)GetValue(FlagColorProperty);
            set
            {
                SetValue(FlagColorProperty, value);
                BorderBrush = new SolidColorBrush(ChangeColorBrightness(value, (float)0.3));
            }
        }
        public static readonly DependencyProperty FlagColorProperty =
            DependencyProperty.Register("FlagColor", typeof(Color), typeof(Flag), new PropertyMetadata(null));

        private static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(color.A, Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
        }
    }
}
