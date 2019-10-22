﻿using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Jaya.Shared.Contracts;
using Jaya.Shared.Models;
using Jaya.Shared.Services;
using System;
using System.Globalization;
using System.IO;

namespace Jaya.Ui.Converters
{
    public class FileSystemObjectToImageConverter : IValueConverter
    {
        const string IMAGE_PATH_FORMAT = "avares://Jaya.Ui/Assets/Images/{0}{1}.png";
        const string FILE_PATH_FORMAT = "avares://Jaya.Ui/Assets/Images/FileExtensions/{0}";

        readonly IMemoryCacheService _cache;

        public FileSystemObjectToImageConverter()
        {
            _cache = ServiceLocator.Instance.GetService<IMemoryCacheService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fso = value as FileSystemObjectModel;
            if (fso == null)
                return null;

            var iconSize = 48;
            if (parameter != null)
                iconSize = int.Parse(parameter as string);

            Uri uri;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            switch (fso.Type)
            {
                case FileSystemObjectType.Drive:
                    uri = new Uri(string.Format(IMAGE_PATH_FORMAT, "Hdd-", iconSize), UriKind.RelativeOrAbsolute);
                    break;

                case FileSystemObjectType.Directory:
                    uri = new Uri(string.Format(IMAGE_PATH_FORMAT, "Folder-", iconSize), UriKind.RelativeOrAbsolute);
                    break;

                case FileSystemObjectType.File:
                    return GetFileImage(fso as FileModel, iconSize, assets);

                default:
                    return null;
            }


            return new Bitmap(assets.Open(uri));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        Bitmap AddOrGetFromCache(Uri uri, IAssetLoader assets, Uri fallbackUri = null)
        {
            if (_cache.TryGetValue(uri, out Bitmap image))
                return image;

            try
            {
                image = new Bitmap(assets.Open(uri));
                _cache.Set(uri, image);

            }
            catch (FileNotFoundException)
            {
                if (fallbackUri == null)
                    return null;

                if (_cache.TryGetValue(fallbackUri, out image))
                    return image;

                image = new Bitmap(assets.Open(fallbackUri));
                _cache.Set(fallbackUri, image);
            }

            return image;
        }

        Bitmap GetFileImage(FileModel fso, int iconSize, IAssetLoader assets)
        {
            var fallbackUri = new Uri(string.Format(IMAGE_PATH_FORMAT, "File-", iconSize), UriKind.RelativeOrAbsolute);

            if (string.IsNullOrEmpty(fso.Extension))
                return AddOrGetFromCache(fallbackUri, assets);

            var extensionImageFile = string.Format("{0}-{1}.png", fso.Extension, iconSize);
            var uri = new Uri(string.Format(FILE_PATH_FORMAT, extensionImageFile), UriKind.RelativeOrAbsolute);

            return AddOrGetFromCache(uri, assets, fallbackUri);
        }
    }
}
