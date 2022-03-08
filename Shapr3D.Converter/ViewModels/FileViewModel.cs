using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
            byte[] thumbnailBytes)
        {
            Id = id;
            OriginalPath = originalPath;
            FileBytes = fileBytes;
            StlFileBytes = stlFileBytes;
            ObjFileBytes = objFileBytes;
            StepFileBytes = stepFileBytes;
            ThumbnailBytes = thumbnailBytes;
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
        }

        public void CancelConversions()
        {
            foreach (var convertingType in ConvertingState.Keys)
            {
                CancelConversion(convertingType);
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
            ThumbnailBytes = ThumbnailBytes
        };
        #endregion Methods
    }
}
