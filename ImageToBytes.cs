
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Liwall
{
    internal class ImageToBytes
    {
        public static void ConvertRainyBootsAnime(string folderPath)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(folderPath+"/ImageBin.bin", FileMode.OpenOrCreate));
            int sikii = 100;
            for (int num = 0; num < 6; num++)
            {
                Bitmap bitmap = new Bitmap(folderPath + "/" + num + ".png");
                for (int i = 0; i < 640000; i++)
                {
                    bw.Write(bitmap.GetPixel(i % 800, i / 800).R > sikii);
                }
            }
            bw.Close();
        }
        public static Bitmap[] LoadRainyBootsAnime()
        {
            int wh = 178 << 16 | 178 << 8 | 178;
            int bl = 9 << 16 | 9 << 8 | 9;
            PixelFormat format = PixelFormat.Format32bppRgb;
            Bitmap[] res=new Bitmap[6];
            for(int i = 0; i < 6; i++)
            {
                res[i] = new Bitmap(800, 800, format);
            }
            BinaryReader br = new BinaryReader(new FileStream(Directory.GetCurrentDirectory() + "/image/ImageBin.bin", FileMode.Open));
            for (int i = 0; i < 6; i++)
            {
                BitmapData bitmapdata = res[i].LockBits(new Rectangle(0, 0, 800, 800), ImageLockMode.WriteOnly, format);
                IntPtr ptr = bitmapdata.Scan0;
                br.BaseStream.Position = i * 640000;
                unsafe
                {
                    int* dst = (int*)ptr.ToPointer();
                    for (int ii = 0; ii < 640000; ii++)
                    {
                        if (br.Read() > 0) *dst++ = wh;
                        else *dst++ = bl;
                    }
                }
                res[i].UnlockBits(bitmapdata);
            }
            br.Close();
            return res;
        }
    }
}
