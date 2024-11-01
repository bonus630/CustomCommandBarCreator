using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Drawing;
using System.IO;

namespace CustomCommandBarCreator.Models
{
    public static class Extensions
    {

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        //public static ImageSource ToImageSource(this Icon icon)
        //{
        //    Bitmap bitmap = icon.ToBitmap();
        //    IntPtr hBitmap = bitmap.GetHbitmap();

        //    ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
        //        hBitmap,
        //        IntPtr.Zero,
        //        Int32Rect.Empty,
        //        BitmapSizeOptions.FromEmptyOptions());

        //    if (!DeleteObject(hBitmap))
        //    {
        //        throw new Win32Exception();
        //    }

        //    return wpfBitmap;
        //}
        public static ImageSource ToImageSource(this Icon icon)
        {
            MemoryStream iconStream = new MemoryStream();
            icon.Save(iconStream);
            IconBitmapDecoder decoder = new IconBitmapDecoder(
                    iconStream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.None);
            BitmapEncoder encoder = new PngBitmapEncoder();
            for (int i = decoder.Frames.Count -1; i >=0; i--)
            {
                try
                {
                    var imageSource = new BitmapImage();
                    Stream saveStream = new MemoryStream();
                    
                        if (encoder.Frames.Count == 0)
                            encoder.Frames.Add(decoder.Frames[i]);
                        else
                            encoder.Frames[0] = decoder.Frames[i];
                        encoder.Save(saveStream);

                        imageSource.BeginInit();
                        imageSource.StreamSource = saveStream;
                        imageSource.EndInit();


                    
                    return imageSource;
                }
                catch { }
            }
            return null;

        }

    }

}
