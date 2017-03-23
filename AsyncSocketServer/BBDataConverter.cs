using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AsyncSocketServer
{
    public class BBDataConverter
    {
        public static string ByteToHexString(byte[] b)
        {
            string rtn = "";
            for (int i = 0; i < b.Length; i++)
            {
                rtn += b[i].ToString("X2") + " ";
            }
            return rtn;
        }

        static public byte[] Int32ToByte(int value)
        {
            return BitConverter.GetBytes(value);
        }

        // 바이트 배열을 String으로 변환 
        static public string ByteToString(byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }

        public static string ByteToString(byte[] b, Encoding encode)
        {
            return encode.GetString(b);
        }

        // String을 바이트 배열로 변환 
        static public byte[] StringToByte(string str)
        {
            return (str != null) ? Encoding.UTF8.GetBytes(str) : Encoding.UTF8.GetBytes("");
        }

        public static int StringToInt32(string str)
        {
            return Int32.Parse(str);
        }

        public static Bitmap GrayRawToBitmap(byte[] frame, int width, int height)
        {
            // Format24bppRgb
            //Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            //BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            // Format8bppIndexed
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            Marshal.Copy(frame, 0, data.Scan0, frame.Length);
            bmp.UnlockBits(data);

            ColorPalette cp = bmp.Palette;
            for (int i = 0; i < 256; i++)
                cp.Entries[i] = Color.FromArgb(i, i, i);
            bmp.Palette = cp;
            return bmp;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static Image BytesToImage(byte[] bytes)
        {
            try
            {
                return Image.FromStream(new MemoryStream(bytes));
            }
            catch
            {
                return Image.FromStream(new MemoryStream(ImageToByte(GrayRawToBitmap(bytes, FingerSensorPacket.SIZE_FP_WIDTH, FingerSensorPacket.SIZE_FP_HEIGHT))));
            }
        }

        public static BitmapImage ByteToBitmapImage(Byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        public static byte[] BitmapToByte(Bitmap bitmap)
        {
            byte[] result = null;
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, bitmap.RawFormat);
                result = stream.ToArray();
            }
            return result;
        }

        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        public static DateTime ByteToDateTime(byte[] value)
        {
            return DateTime.FromBinary(BitConverter.ToInt64(value, 0));
        }

        public static byte[] DateTimeToByte(DateTime dt)
        {
            return BitConverter.GetBytes(dt.Ticks);
        }
    }
}
