using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PZone.Drawing
{
    /// <summary>
    /// Расширение класса <see cref="Image"/>.
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Преобразование изображения в строку Base64.
        /// </summary>
        /// <param name="image">Изображение.</param>
        /// <param name="format">Формат изображения.</param>
        /// <returns>
        /// Строка в формате Base64 с содержимым изображения.
        /// </returns>
        public static string ToBase64(this Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                var base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }
}