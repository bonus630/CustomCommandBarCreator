using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.IconLib;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomCommandBarCreator.Models
{
    public class IconCreator
    {
        public string RewriteIcon(string imagePath)
        {
            string path = Path.GetTempFileName();
            path = path.Replace(".tmp", ".ico");
            try
            {
                MultiIcon mIcon = new MultiIcon();
                using (Stream iconStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    mIcon.Load(iconStream);
                }
                if (mIcon[0][mIcon[0].Count - 1].Icon.Width == 256)
                    mIcon[0][mIcon[0].Count - 1].IconImageFormat = IconImageFormat.PNG;
                mIcon.Save(path, MultiIconFormat.ICO);
            }
            catch (Exception ex) { path = string.Empty; }

            return path;

        }
        public string ContertToIcon(string imagePath)
        {
            string iconPath = Path.GetTempFileName();
            iconPath = iconPath.Replace(".tmp", ".ico");
            MultiIcon mIcon = new MultiIcon();
            SingleIcon sIcon = mIcon.Add(Path.GetFileName(imagePath));
            Image original = Bitmap.FromFile(imagePath);
            int size = 16;
            if (original.Width > original.Height)
                size = RoundDownToNearest(original.Width);
            else
                size = RoundDownToNearest(original.Height);
            System.Drawing.Bitmap bitmap16 = new Bitmap(original, size, size);

            sIcon.Add(bitmap16);
            if (size == 256)
                sIcon[0].IconImageFormat = IconImageFormat.PNG;
            mIcon.SelectedIndex = 0;
            mIcon.Save(iconPath, MultiIconFormat.ICO);
            //using (Bitmap bitmap = new Bitmap(imagePath))
            //{
            //    Icon icon = Icon.FromHandle(bitmap.GetHicon());
            //    using (System.IO.FileStream stream = new System.IO.FileStream(iconPath, System.IO.FileMode.Create,FileAccess.Write,FileShare.Write))
            //    {
            //        icon.Save(stream);
            //    }
            //}
            return iconPath;
        }
        private int RoundDownToNearest(int number)
        {
            int[] values = { 16, 32, 48, 64, 128, 256 };

            int closest = values[0];
            foreach (int val in values)
            {
                if (val <= number)
                {
                    closest = val;
                }
                else
                {
                    break;
                }
            }
            return closest;
        }
        public string ContertToJpg128(string imagePath)
        {
            string iconPath = Path.GetTempFileName();
            iconPath = iconPath.Replace(".tmp", ".png");

            Image original = Bitmap.FromFile(imagePath);
            int size = 16;
            System.Drawing.Bitmap bitmap16 = new Bitmap(size, size);

            Graphics g = Graphics.FromImage(bitmap16);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;

            g.FillRectangle(new SolidBrush(Color.FromArgb(0, 255, 255, 255)), new Rectangle(0, 0, size, size));
            g.DrawImage(original, new Rectangle(0, 0, size, size), new Rectangle(0, 0, original.Width, original.Height), GraphicsUnit.Pixel);


            bitmap16.Save(iconPath, System.Drawing.Imaging.ImageFormat.Png);

            return iconPath;
        }
        public string GetImageFromIcon(string imagePath, int size = 16)
        {
            string iconPath = Path.GetTempFileName();
            try
            {
                MultiIcon mIcon = new MultiIcon();
                using (Stream iconStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    mIcon.Load(iconStream);
                }
                if (mIcon[0].Count > 0)
                {
                    for (int i = 0; i < mIcon[0].Count; i++)
                    {
                        System.Drawing.Bitmap bitmap = mIcon[0][i].Icon.ToBitmap();
                        bitmap.Save(iconPath);
                        return iconPath;
                    }
                }
            }
            catch
            {

            }
            return iconPath;
        }

    }
}
