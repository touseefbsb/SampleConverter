using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Shapr3D_Converter.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Shapr3D.Converter.ViewModels
{
    public partial class FileViewModel : ObservableObject
    {
        #region ReadonlyFields
        private readonly ulong fileSize;
        #endregion ReadonlyFields

        #region ObservableFields
        [ObservableProperty]
        private BitmapImage thumbnail;
        [ObservableProperty]
        private bool isConverting;
        [ObservableProperty]
        private TimeSpan? stlConversionTime;
        [ObservableProperty]
        private TimeSpan? objConversionTime;
        [ObservableProperty]
        private TimeSpan? stepConversionTime;
        #endregion ObservableFields

        #region Ctor

        public FileViewModel(
            Guid id,
            string originalPath,
            bool objConverted,
            bool stepConverted,
            bool stlConverted,
            ulong fileSize,
            byte[] fileBytes,
            byte[] stlFileBytes,
            byte[] objFileBytes,
            byte[] stepFileBytes,
            byte[] thumbnailBytes,
            TimeSpan? stlConversionTime,
            TimeSpan? objConversionTime,
            TimeSpan? stepConversionTime)
        {
            Id = id;
            OriginalPath = originalPath;
            FileBytes = fileBytes;
            StlFileBytes = stlFileBytes;
            ObjFileBytes = objFileBytes;
            StepFileBytes = stepFileBytes;
            ThumbnailBytes = thumbnailBytes;
            StlConversionTime = stlConversionTime;
            ObjConversionTime = objConversionTime;
            StepConversionTime = stepConversionTime;
            ConvertingState.Add(ConverterOutputType.Obj, new FileConvertingState(objConverted));
            ConvertingState.Add(ConverterOutputType.Step, new FileConvertingState(stepConverted));
            ConvertingState.Add(ConverterOutputType.Stl, new FileConvertingState(stlConverted));
            IsConverting = GetIsConverting();
            foreach (var (type, state) in ConvertingState)
            {
                state.PropertyChanged += OnConvertingStatePropertyChanged;
            }

            var nameLower = Path.GetFileNameWithoutExtension(originalPath).ToLower().Replace("_", " ");
            Name = nameLower.Substring(0, 1).ToUpper() + nameLower.Substring(1);

            this.fileSize = fileSize;
        }
        #endregion Ctor

        #region Props
        public Guid Id { get; }
        public string OriginalPath { get; }
        public byte[] FileBytes { get; }
        public byte[] StlFileBytes { get; set; }
        public byte[] ObjFileBytes { get; set; }
        public byte[] StepFileBytes { get; set; }
        public byte[] ThumbnailBytes { get; set; }
        public CancellationTokenSource StlCancellationTokenSource { get; set; }
        public CancellationTokenSource ObjCancellationTokenSource { get; set; }
        public CancellationTokenSource StepCancellationTokenSource { get; set; }
        public string Name { get; }
        public Dictionary<ConverterOutputType, FileConvertingState> ConvertingState { get; } = new Dictionary<ConverterOutputType, FileConvertingState>();
        public FileConvertingState ObjConvertingState => ConvertingState[ConverterOutputType.Obj];
        public FileConvertingState StepConvertingState => ConvertingState[ConverterOutputType.Step];
        public FileConvertingState StlConvertingState => ConvertingState[ConverterOutputType.Stl];
        public string FileSizeFormatted => string.Format("{0} megabytes", ((double)fileSize / 1024 / 1024).ToString("0.00"));
        #endregion Props

        #region Methods
        public void CancelConversion(ConverterOutputType type)
        {
            // TODO
            try
            {
                switch (type)
                {
                    case ConverterOutputType.Stl:
                        StlCancellationTokenSource?.Cancel();
                        break;
                    case ConverterOutputType.Obj:
                        ObjCancellationTokenSource?.Cancel();
                        break;
                    case ConverterOutputType.Step:
                        StepCancellationTokenSource?.Cancel();
                        break;
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void CancelConversions()
        {
            foreach (var convertingType in ConvertingState.Keys)
            {
                CancelConversion(convertingType);
            }
        }
        public void DisposeCancellationTokenSource(ConverterOutputType type)
        {
            try
            {
                switch (type)
                {
                    case ConverterOutputType.Stl:
                        StlCancellationTokenSource?.Dispose();
                        break;
                    case ConverterOutputType.Obj:
                        ObjCancellationTokenSource?.Dispose();
                        break;
                    case ConverterOutputType.Step:
                        StepCancellationTokenSource?.Dispose();
                        break;
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void OnConvertingStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileConvertingState.Converting))
            {
                IsConverting = GetIsConverting();
            }
        }
        private bool GetIsConverting() => ConvertingState.Any(state => state.Value.Converting);

        public ModelEntity ToModelEntity() => new ModelEntity()
        {
            Id = Id,
            ObjConverted = ConvertingState[ConverterOutputType.Obj].Converted,
            StepConverted = ConvertingState[ConverterOutputType.Step].Converted,
            StlConverted = ConvertingState[ConverterOutputType.Stl].Converted,
            OriginalPath = OriginalPath,
            FileSize = fileSize,
            FileBytes = FileBytes,
            StlFileBytes = StlFileBytes,
            ObjFileBytes = ObjFileBytes,
            StepFileBytes = StepFileBytes,
            ThumbnailBytes = ThumbnailBytes,
            StlConversionTime = StlConversionTime,
            ObjConversionTime = ObjConversionTime,
            StepConversionTime = StepConversionTime
        };
        #endregion Methods
    }
}
