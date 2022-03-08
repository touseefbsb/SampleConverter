using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Shapr3D.Converter.ViewModels
{
    public partial class FileConvertingState : ObservableObject
    {
        [ObservableProperty]
        private bool converting;
        [ObservableProperty]
        private bool converted;
        [ObservableProperty]
        private int progress;
        public FileConvertingState(bool isConverted)
        {
            Converted = isConverted;
            if (isConverted)
            {
                Progress = 100;
            }
        }
    }
}
