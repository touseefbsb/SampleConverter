using System;
using System.IO;
using System.Threading.Tasks;
using Shapr3D.Converter.Enums;
using Shapr3D.Converter.Helpers;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Shapr3D.Converter.Extensions
{
    public static class SettingsStorageExtensions
    {
        #region Fields
        private const string FileExtension = ".json";
        public static readonly string CollectionViewKey = "CollectionView";
        private static CurrentCollectionView currentCollectionView;
        #endregion Fields

        #region Events
        public static event EventHandler<EventArgs> OnCollectionViewSelected;
        #endregion Events

        #region Methods

        public static async Task SaveAsync<T>(this StorageFolder folder, string name, T content)
        {
            var file = await folder.CreateFileAsync(GetFileName(name), CreationCollisionOption.ReplaceExisting);
            var fileContent = await Json.StringifyAsync(content);

            await FileIO.WriteTextAsync(file, fileContent);
        }

        public static async Task<T> ReadAsync<T>(this StorageFolder folder, string name)
        {
            if (!File.Exists(Path.Combine(folder.Path, GetFileName(name))))
            {
                return default;
            }

            var file = await folder.GetFileAsync($"{name}.json");
            var fileContent = await FileIO.ReadTextAsync(file);

            return await Json.ToObjectAsync<T>(fileContent);
        }

        public static async Task SaveAsync<T>(this ApplicationDataContainer settings, string key, T value) => settings.SaveString(key, await Json.StringifyAsync(value));
        public static void SaveString(this ApplicationDataContainer settings, string key, string value)
        {
            if (value == nameof(CurrentCollectionView.GridView) || value == nameof(CurrentCollectionView.ListView))
            {
                if (settings.Values.ContainsKey(key))
                {
                    if (settings.Values[key].ToString() != value)
                    {
                        settings.Values[key] = value;
                        OnCollectionViewSelected?.Invoke(null, EventArgs.Empty);
                    }
                }
                else
                {
                    settings.Values[key] = value;
                    OnCollectionViewSelected?.Invoke(null, EventArgs.Empty);
                }
            }
            else
            {
                settings.Values[key] = value;
            }
        }
        public static CurrentCollectionView ReadCurrentCollectionView(this ApplicationDataContainer settings)
        {
            if (settings.Values.ContainsKey(CollectionViewKey))
            {
                var view = settings.Values[CollectionViewKey].ToString();
                currentCollectionView = view == nameof(CurrentCollectionView.GridView) ? CurrentCollectionView.GridView : CurrentCollectionView.ListView;
            }
            else
            {
                settings.SaveString(CollectionViewKey, nameof(CurrentCollectionView.GridView));
                currentCollectionView = CurrentCollectionView.GridView;
            }
            return currentCollectionView;
        }
        public static string ReadString(this ApplicationDataContainer settings, string key) => settings.Values[key].ToString();
        public static async Task<T> ReadAsync<T>(this ApplicationDataContainer settings, string key)
        {
            if (settings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
            return default;
        }

        public static async Task<StorageFile> SaveFileAsync(this StorageFolder folder, byte[] content, string fileName, CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("File name is null or empty. Specify a valid file name", nameof(fileName));
            }

            var storageFile = await folder.CreateFileAsync(fileName, options);
            await FileIO.WriteBytesAsync(storageFile, content);
            return storageFile;
        }

        public static async Task<byte[]> ReadFileAsync(this StorageFolder folder, string fileName)
        {
            var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);

            if (item?.IsOfType(StorageItemTypes.File) == true)
            {
                var storageFile = await folder.GetFileAsync(fileName);
                return await storageFile.ReadBytesAsync();
            }
            return null;
        }

        public static async Task<byte[]> ReadBytesAsync(this StorageFile file)
        {
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenReadAsync())
                {
                    using (var reader = new DataReader(stream.GetInputStreamAt(0)))
                    {
                        await reader.LoadAsync((uint)stream.Size);
                        var bytes = new byte[stream.Size];
                        reader.ReadBytes(bytes);
                        return bytes;
                    }
                }
            }

            return null;
        }
        private static string GetFileName(string name) => string.Concat(name, FileExtension);
        #endregion Methods
    }
}