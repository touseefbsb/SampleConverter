using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Converter;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Shapr3D.Converter.Datasource;
using Shapr3D.Converter.EventMessages;
using Shapr3D.Converter.Extensions;
using Shapr3D.Converter.Helpers;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;

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
        public MainViewModel()
        {
            EventBus.GetInstance().Subscribe((message) =>
            {
                if (message is AppOnSuspendingMessage)
                {
                    foreach (var file in Files)
                    {
                        file.CancelConversions();
                    }
                }
            });
            FilesCollectionView = new AdvancedCollectionView(Files, true);
            FilesCollectionView.ObserveFilterProperty(nameof(FileViewModel.Name));
        }
        #endregion Ctor

        #region Props
        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();
        public AdvancedCollectionView FilesCollectionView { get; }
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
                var model = new FileViewModel(id, file.Path, false, false, false, props.Size, fileBytes, null, null, null, thumbnailBytes, null, null, null);
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
            catch (OperationCanceledException)
            {
                state.Progress = 0;
            }
            catch (Exception ex)
            {
                state.Progress = 0;
                await AppCenterHelper.TrackExceptionAndShowErrorDialogAsync($"{nameof(MainViewModel.ConvertFile)} failed for type : {type}", ex, "Conversion failed");
            }
            finally
            {
                state.Converting = false;
                model.DisposeCancellationTokenSource(type);
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
            var messageDialog = new MessageDialog($"Are you sure you want to delete these {Files.Count} items?") { Title = "Delete confirmation" };
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(DeleteCommandInvokedHandler), 0));
            messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(DeleteCommandInvokedHandler), 1));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;
            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            await messageDialog.ShowAsync();
        }
        private async void DeleteCommandInvokedHandler(IUICommand command)
        {
            if ((int)command.Id == 0)
            {
                foreach (var model in Files)
                {
                    model.CancelConversions();
                }
                await ps.DeleteAllAsync();
                SelectedFile = null;
                Files.Clear();
            }
        }
        private async Task Convert(FileViewModel model, IProgress<int> progress, ConverterOutputType outputType)
        {
            // TODO
            CancellationToken cancellationToken;
            switch (outputType)
            {
                case ConverterOutputType.Stl:
                    model.StlCancellationTokenSource = new();
                    cancellationToken = model.StlCancellationTokenSource.Token;
                    break;
                case ConverterOutputType.Obj:
                    model.ObjCancellationTokenSource = new();
                    cancellationToken = model.ObjCancellationTokenSource.Token;
                    break;
                case ConverterOutputType.Step:
                    model.StepCancellationTokenSource = new();
                    cancellationToken = model.StepCancellationTokenSource.Token;
                    break;
            }
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            await Task.Run(async () =>
            {
                var timer = new Stopwatch();
                timer.Start();
                cancellationToken.ThrowIfCancellationRequested();

                var converted = new byte[model.FileBytes.Length];
                var totalChunks = 10;
                var chunksSize = System.Convert.ToInt32(model.FileBytes.Length / totalChunks);
                var index = 0;
                var i = 0;
                while (index < model.FileBytes.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (model.FileBytes.Length - index < chunksSize)
                    {
                        var remainingLength = model.FileBytes.Length - index;
                        var tempByteArray = new byte[remainingLength];
                        Array.Copy(model.FileBytes, index, tempByteArray, 0, remainingLength);
                        tempByteArray = ModelConverter.ConvertChunk(tempByteArray);
                        Array.Copy(tempByteArray, 0, converted, index, remainingLength);
                        index += remainingLength;
                        progress.Report(totalChunks);
                    }
                    else
                    {
                        var tempByteArray = new byte[chunksSize];
                        Array.Copy(model.FileBytes, index, tempByteArray, 0, chunksSize);
                        tempByteArray = ModelConverter.ConvertChunk(tempByteArray);
                        Array.Copy(tempByteArray, 0, converted, index, chunksSize);
                        index += chunksSize;
                        i++;
                        progress.Report(i);
                    }
                }
                progress.Report(totalChunks);
                timer.Stop();
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
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    switch (outputType)
                    {
                        case ConverterOutputType.Stl:
                            model.StlConversionTime = timer.Elapsed;
                            break;
                        case ConverterOutputType.Obj:
                            model.ObjConversionTime = timer.Elapsed;
                            break;
                        case ConverterOutputType.Step:
                            model.StepConversionTime = timer.Elapsed;
                            break;
                    }
                });
            }, cancellationToken);
        }
        #endregion Methods
    }
}
