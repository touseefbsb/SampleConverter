using Shapr3D.Converter.Datasource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Shapr3D.Converter.Models
{
    public class FileConvertingState : INotifyPropertyChanged
    {
        private bool converting;
        private bool converted;
        private int progress;

        public FileConvertingState(bool isConverted)
        {
            Converted = isConverted;
            if (isConverted)
            {
                Progress = 100;
            }
        }
        public bool Converting
        {
            get => converting;
            set
            {
                if (converting != value)
                {
                    converting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Converting)));
                }
            }
        }
        public bool Converted
        {
            get => converted;
            set
            {
                if (converted != value)
                {
                    converted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Converted)));
                }
            }
        }
        public int Progress
        {
            get => progress;
            set
            {
                if (progress != value)
                {
                    progress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class FileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FileViewModel(Guid id, string originalPath, bool objConverted, bool stepConverted, bool stlConverted, ulong fileSize, byte[] fileBytes)
        {
            Id = id;
            OriginalPath = originalPath;
            FileBytes = fileBytes;
            ConvertingState.Add(ConverterOutputType.Obj, new FileConvertingState(objConverted));
            ConvertingState.Add(ConverterOutputType.Step, new FileConvertingState(stepConverted));
            ConvertingState.Add(ConverterOutputType.Stl, new FileConvertingState(stlConverted));
            foreach (var (type, state) in ConvertingState)
            {
                state.PropertyChanged += OnConvertingStatePropertyChanged;
            }

            var nameLower = Path.GetFileNameWithoutExtension(originalPath).ToLower().Replace("_", " ");
            Name = nameLower.Substring(0, 1).ToUpper() + nameLower.Substring(1);

            this.fileSize = fileSize;
        }

        public Guid Id { get; }
        public string OriginalPath { get; }
        public byte[] FileBytes { get; }
        public string Name { get; }

        public Dictionary<ConverterOutputType, FileConvertingState> ConvertingState { get; } = new Dictionary<ConverterOutputType, FileConvertingState>();

        public FileConvertingState ObjConvertingState => ConvertingState[ConverterOutputType.Obj];
        public FileConvertingState StepConvertingState => ConvertingState[ConverterOutputType.Step];
        public FileConvertingState StlConvertingState => ConvertingState[ConverterOutputType.Stl];

        public bool IsConverting => ConvertingState.Any(state => state.Value.Converting);

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

        private readonly ulong fileSize;
        public string FileSizeFormatted
        {
            get
            {
                return string.Format("{0} megabytes", ((double)fileSize / 1024 / 1024).ToString("0.00"));
            }
        }

        private void OnConvertingStatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileConvertingState.Converting))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConverting)));
            }
        }

        public ModelEntity ToModelEntity()
        {
            return new ModelEntity()
            {
                Id = Id,
                ObjConverted = ConvertingState[ConverterOutputType.Obj].Converted,
                StepConverted = ConvertingState[ConverterOutputType.Step].Converted,
                StlConverted = ConvertingState[ConverterOutputType.Stl].Converted,
                OriginalPath = OriginalPath,
                FileSize = fileSize,
                FileBytes = FileBytes
            };
        }
    }

    public enum ConverterOutputType
    {
        Stl,
        Obj,
        Step
    }
}
