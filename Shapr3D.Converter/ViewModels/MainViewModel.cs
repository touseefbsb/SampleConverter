﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Converter;
using Shapr3D.Converter.Datasource;
using Shapr3D.Converter.EventMessages;
using Shapr3D.Converter.Extensions;
using Shapr3D.Converter.Helpers;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Shapr3D.Converter.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IPersistedStore ps;

        public MainViewModel()
        {
            AddCommand = new RelayCommand(Add);
            DeleteAllCommand = new RelayCommand(DeleteAll);
            ConvertActionCommand = new RelayCommand<ConverterOutputType>(ConvertAction);
            CloseDetailsCommand = new RelayCommand(CloseDetails);

            EventBus.GetInstance().Subscribe((message) =>
            {
                if (message is AppOnSuspendingMessage)
                {
                    foreach (var files in Files)
                    {
                        files.CancelConversions();
                    }
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();
        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteAllCommand { get; }
        public RelayCommand<ConverterOutputType> ConvertActionCommand { get; }
        public RelayCommand CloseDetailsCommand { get; }

        private FileViewModel selectedFile;
        public FileViewModel SelectedFile
        {
            get => selectedFile;
            set
            {
                if (selectedFile != value)
                {
                    selectedFile = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFile)));
                }
            }
        }

        public async Task InitAsync()
        {
            ps = new DummyStore();
            await ps.InitAsync();

            foreach (var model in await ps.GetAllAsync())
            {
                Files.Add(model.ToFileViewModel());
            }
        }

        private void CloseDetails() => SelectedFile = null;

        private async void ConvertAction(ConverterOutputType type)
        {
            var state = selectedFile.ConvertingState[type];
            if (state.Converting)
            {
                selectedFile.CancelConversion(type);
            }
            else if (state.Converted)
            {
                Save(SelectedFile, type);
            }
            else
            {
                await ConvertFile(SelectedFile, type);
            }
        }

        public async void Add()
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".shapr");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var id = Guid.NewGuid();
                var props = await file.GetBasicPropertiesAsync();
                var fileBytes = await FileHelper.GetBytesAsync(file);
                var model = new FileViewModel(id, file.Path, false, false, false, props.Size, fileBytes, null, null, null);
                await ps.AddOrUpdateAsync(model.ToModelEntity());

                Files.Add(model);
            }
        }

        private async Task ConvertFile(FileViewModel model, ConverterOutputType type)
        {
            var state = selectedFile.ConvertingState[type];
            var progress = new Progress<int>((p) => state.Progress = p);

            state.Converting = true;

            try
            {
                await Convert(model, progress, type);
                state.Converted = true;

                await ps.AddOrUpdateAsync(model.ToModelEntity());
            }
            catch (TaskCanceledException)
            {
                state.Progress = 0;
            }
            finally
            {
                state.Converting = false;
            }
        }

        private async void Save(FileViewModel model, ConverterOutputType type)
        {
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add
                                  (string.Format("{0} file", type.ToString().ToLower()), new List<string>() { string.Format(".{0}", type.ToString().ToLower()) });
            savePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(model.OriginalPath);

            var savedFile = await savePicker.PickSaveFileAsync();
            // TODO
            if (savedFile != default)
            {
                switch (type)
                {
                    case ConverterOutputType.Stl:
                        await FileIO.WriteBytesAsync(savedFile, model.StlFileBytes);
                        break;
                    case ConverterOutputType.Obj:
                        await FileIO.WriteBytesAsync(savedFile, model.ObjFileBytes);
                        break;
                    case ConverterOutputType.Step:
                        await FileIO.WriteBytesAsync(savedFile, model.StepFileBytes);
                        break;
                }
            }
        }

        private async void DeleteAll()
        {
            foreach (var model in Files)
            {
                model.CancelConversions();
            }

            await ps.DeleteAllAsync();

            SelectedFile = null;
            Files.Clear();
        }

        private async Task Convert(FileViewModel model, IProgress<int> progress, ConverterOutputType outputType) =>
            // TODO
            await Task.Run(() =>
            {
                var converted = ModelConverter.ConvertChunk(model.FileBytes);
                switch (outputType)
                {
                    case ConverterOutputType.Stl:
                        model.StlFileBytes = converted;
                        break;
                    case ConverterOutputType.Obj:
                        model.ObjFileBytes = converted;
                        break;
                    case ConverterOutputType.Step:
                        model.StepFileBytes = converted;
                        break;
                }
                progress.Report(100);
            });
    }
}
