using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Shapr3D.Converter.Helpers
{
    public static class FileHelper
    {
        private static uint _thumbnailReqestedSize => 190;
        private static Uri _storeLogoUriSource = new Uri("ms-appx:///Assets/StoreLogo.png");
        public static async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            if (file is null)
            {
                return null;
            }

            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }
        public static async Task<byte[]> GetBytesForImageAsync(StorageFile mediafile)
        {
            byte[] bts;
            using (var imgSource = await mediafile.GetScaledImageAsThumbnailAsync(ThumbnailMode.VideosView, _thumbnailReqestedSize, ThumbnailOptions.UseCurrentScale))
            {
                if (!(imgSource is null))
                {
                    using (var stream = new MemoryStream())
                    {
                        await imgSource.AsStream().CopyToAsync(stream);
                        bts = stream.ToArray();
                        return bts;
                    }
                }
                else
                {
                    return new byte[] { };
                }
            }
        }
        public static async Task<BitmapImage> LoadImageAsync(byte[] thumbPrint)
        {
            try
            {
                if (thumbPrint?.Length > 0 && await GetImageFromBytesAsync(thumbPrint) is BitmapImage tempThumb)
                {
                    return tempThumb;
                }
            }
            catch
            {
                //we dont need to handle it because we are using a default image below.
            }
            return new BitmapImage(_storeLogoUriSource);
        }
        public static async Task<BitmapImage> GetImageFromBytesAsync(byte[] bytes)
        {
            var image = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(bytes.AsBuffer());
                stream.Seek(0);
                await image.SetSourceAsync(stream);
            }
            return image;
        }
    }
}