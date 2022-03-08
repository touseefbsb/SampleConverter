using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Converter;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Shapr3D.Converter.Datasource;
using Shapr3D.Converter.EventMessages;
using Shapr3D.Converter.Extensions;
using Shapr3D.Converter.Helpers;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Shapr3D.Converter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields
        private IPersistedStore ps;
        #endregion Fields

        #region ObservableFields
        [ObservableProperty]
        private FileViewModel selectedFile;
        #endregion ObservableFields

        #region Ctor
        public MainViewModel() => EventBus.GetInstance().Subscribe((message) =>
                                {
                                    if (message is AppOnSuspendingMessage)
                                    {
                                        foreach (var files in Files)
                                        {
                                            files.CancelConversions();
                                        }
                                    }
                                });
        #endregion Ctor

        #region Props
        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();
        #endregion

        #region Methods

        public async Task InitAsync()
        {
            ps = new DummyStore();
            await ps.InitAsync();

            foreach (var model in await ps.GetAllAsync())
            {
                var fvm = model.ToFileViewModel();
                fvm.Thumbnail = await FileHelper.LoadImageAsync(fvm.ThumbnailBytes);
                Files.Add(fvm);
            }
        }

        [ICommand]
        private void CloseDetails() => SelectedFile = null;

        [ICommand]
        private async Task ConvertAction(ConverterOutputType type)
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

        [ICommand]
        public async Task Add()
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
                var thumbnailBytes = await FileHelper.GetBytesForImageAsync(file);
                var model = new FileViewModel(id, file.Path, false, false, false, props.Size, fileBytes, null, null, null, thumbnailBytes);
                await ps.AddOrUpdateAsync(model.ToModelEntity());
                model.Thumbnail = await FileHelper.LoadImageAsync(model.ThumbnailBytes);
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

        [ICommand]
        private async Task DeleteAll()
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
        #endregion Methods
    }
}
