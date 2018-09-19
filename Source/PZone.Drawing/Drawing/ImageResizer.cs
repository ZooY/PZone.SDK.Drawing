using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PZone.Drawing
{
    /// <summary>
    /// Изменение размеров изображения.
    /// </summary>
    public class ImageResizer
    {
        /// <summary>
        /// Изменение размеров изображения.
        /// </summary>
        /// <param name="fileContent">Файл изображения в формате Base64.</param>
        /// <param name="width">Ширина нового изображения.</param>
        /// <param name="height">Высота нового изображения.</param>
        /// <returns>
        /// Строка в формате Base64 с новым файлом изображения.
        /// </returns>
        public string Resize(string fileContent, int width, int height)
        {
            var bitmap = GetSourceImageFromContent(fileContent, out var format);
            var thumbnail = CreateThumbnail(bitmap, width, height);
            return thumbnail.ToBase64(format);
        }

        /// <summary>
        /// Изменение размеров изображения с сохранением результата в файл.
        /// </summary>
        /// <param name="fileContent">Файл изображения в формате Base64.</param>
        /// <param name="resultFileName">Имя результирующего файла.</param>
        /// <param name="width">Ширина нового изображения.</param>
        /// <param name="height">Высота нового изображения.</param>
        public void Resize(string fileContent, string resultFileName, int width, int height)
        {
            var bitmap = GetSourceImageFromContent(fileContent);
            var thumbnail = CreateThumbnail(bitmap, width, height);
            SaveThumbnail(resultFileName, thumbnail);
        }

        /// <summary>
        /// Изменение размеров изображения с сохранением результата в файл.
        /// </summary>
        /// <param name="fileName">Имя файла изображения.</param>
        /// <param name="resultFileName">Имя результирующего файла.</param>
        /// <param name="width">Ширина нового изображения.</param>
        /// <param name="height">Высота нового изображения.</param>
        public void ResizeFile(string fileName, string resultFileName, int width, int height)
        {
            var bitmap = GetSourceImageFromFile(fileName);
            var thumbnail = CreateThumbnail(bitmap, width, height);
            SaveThumbnail(resultFileName, thumbnail);
        }

        /// <summary>
        /// Изменение размеров изображения.
        /// </summary>
        /// <param name="fileName">Имя файла изображения.</param>
        /// <param name="width">Ширина нового изображения.</param>
        /// <param name="height">Высота нового изображения.</param>
        /// <returns>
        /// Строка в формате Base64 с новым файлом изображения.
        /// </returns>
        public string ResizeFile(string fileName, int width, int height)
        {
            var bitmap = GetSourceImageFromFile(fileName, out var format);
            var thumbnail = CreateThumbnail(bitmap, width, height);
            return thumbnail.ToBase64(format);
        }


        private static Bitmap GetSourceImageFromContent(string fileContent)
        {
            var bitmapData = Convert.FromBase64String(fileContent);
            using (var stream = new MemoryStream(bitmapData))
            {
                return GetSourceImage(null, stream);
            }
        }


        private static Bitmap GetSourceImageFromContent(string fileContent, out ImageFormat format)
        {
            var image = GetSourceImageFromContent(fileContent);
            format = image.RawFormat;
            return image;
        }


        private static Bitmap GetSourceImageFromFile(string fileName)
        {
            return GetSourceImage(fileName, null);
        }


        private static Bitmap GetSourceImageFromFile(string fileName, out ImageFormat format)
        {
            var image = GetSourceImage(fileName, null);
            format = image.RawFormat;
            return image;
        }


        private static Bitmap GetSourceImage(string fileName, Stream fileStream)
        {
            Bitmap bitmap;
            try
            {
                bitmap = fileName == null ? new Bitmap(fileStream) : new Bitmap(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read the source file. " + ex.Message, ex);
            }

            return bitmap;
        }


        private static Image CreateThumbnail(Bitmap bitmap, int width, int height)
        {
            // Вычисляем недостающие размеры
                // ReSharper disable PossibleLossOfFraction
            if (width == 0)
                width = (int)Math.Round((double)(bitmap.Width * height / bitmap.Height));
            else if (height == 0)
                height = (int)Math.Round((double)(bitmap.Height * width / bitmap.Width));
            // ReSharper restore PossibleLossOfFraction

            // Создаем миниатюрное изображение
            Image thumbnail;
            try
            {
                var myCallback = new Image.GetThumbnailImageAbort(() => throw new Exception("Unable to resize image. Operation is aborted."));
                thumbnail = bitmap.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to resize image. " + ex.Message, ex);
            }

            return thumbnail;
        }


        private static void SaveThumbnail(string resultFileName, Image thumbnail)
        {
            try
            {
                thumbnail.Save(resultFileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to save image to file. " + ex.Message, ex);
            }
        }
    }
}