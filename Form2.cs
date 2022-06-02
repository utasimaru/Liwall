using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Liwall
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(178, 178, 178);
            IntPtr this_handle = this.Handle;
            IntPtr parent = IntPtr.Zero;
            User32.EnumWindows((hwnd, lParam) =>
            {
                IntPtr shell = User32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shell != IntPtr.Zero) parent = User32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);

            //dame this_handle = User32.FindWindowEx(this_handle, IntPtr.Zero, "Form2", null);
            
            Location=new Point(0,0);
            User32.SetParent(this_handle, parent);

            Program.drawing = true;
            DrawLoop3();
        }
        /*
        //cpu節約
        private void DrawLoop()
        {
            Task.Run(() =>
            {
                Bitmap[] _animes = ImageToBytes.LoadRainyBootsAnime(Directory.GetCurrentDirectory() + "/image");

                int framerate = 1000 / 10;
                int tick, timeToNext, animeIndex = 0;
                while (Form1.drawing)
                {
                    tick = Environment.TickCount;
                    pictureBox1.Image = _animes[animeIndex];
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;

                    timeToNext = Environment.TickCount + framerate - tick; ;
                    if (timeToNext > 0) Thread.Sleep(timeToNext);
                }
            });
        }
        */
        /*private void DrawLoop2()
        {
            Task.Run(() =>
            {
                PixelFormat format = PixelFormat.Format32bppRgb;
                Bitmap[] bmp = new Bitmap[2];
                bmp[0] = new Bitmap(800, 800, format);
                bmp[1] = new Bitmap(800, 800, format);
                int wh = 178 << 16 | 178 << 8 | 178 ; 
                int bl =  9  << 16 |  9  << 8 |  9 ; 
                int framerate = 1000 / 10;
                int tick, timeToNext = 0, animeIndex = 0, bmpIndex = 0;

                BinaryReader br = new BinaryReader(new FileStream(Directory.GetCurrentDirectory() + "/image/ImageBin.bin", FileMode.Open));
                
                while (Form1.drawing)
                {
                    tick = Environment.TickCount;
                    BitmapData bitmapdata = bmp[bmpIndex].LockBits(new Rectangle(0, 0, 800, 800), ImageLockMode.WriteOnly, format);
                    IntPtr ptr = bitmapdata.Scan0;
                    
                    br.BaseStream.Position = animeIndex * 640000;
                    unsafe
                    {
                        int* dst=(int*)ptr.ToPointer();
                        for (int i = 0; i < 640000; i++)
                        {
                            if (br.Read() > 0) *dst++ = wh;
                            else *dst++ = bl;
                        }
                        pictureBox1.Image = bmp[animeIndex % 2];
                    }
                    bmp[bmpIndex].UnlockBits(bitmapdata);
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;
                    bmpIndex = bmpIndex >= 1 ? 0 : bmpIndex + 1;

                    timeToNext = Environment.TickCount + framerate - tick;
                    if (timeToNext > 0) Thread.Sleep(timeToNext);
                }
                br.Close();
            });
        }*/
        //4までで最高のメモリ節約
        private void DrawLoop3()
        {
            Task.Run(() =>
            {
                PixelFormat format = PixelFormat.Format32bppRgb;
                Bitmap bmp = new Bitmap(800,800,format);
                int wh = 178 << 16 | 178 << 8 | 178;
                int bl = 9 << 16 | 9 << 8 | 9;
                int framerate = 1000 / 10;
                int tick, timeToNext = 0, animeIndex = 0;

                BinaryReader br = new BinaryReader(new FileStream(Directory.GetCurrentDirectory() + "/image/ImageBin.bin", FileMode.Open));

                while (Program.drawing)
                {
                    tick = Environment.TickCount;
                    BitmapData bitmapdata = bmp.LockBits(new Rectangle(0, 0, 800, 800), ImageLockMode.WriteOnly, format);
                    IntPtr ptr = bitmapdata.Scan0;

                    br.BaseStream.Position = animeIndex * 640000;
                    unsafe
                    {
                        int* dst = (int*)ptr.ToPointer();
                        for (int i = 0; i < 640000; i++)
                        {
                            if (br.Read() > 0) *dst++ = wh;
                            else *dst++ = bl;
                        }
                    }
                    bmp.UnlockBits(bitmapdata);
                    pictureBox1.Image = bmp;
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;

                    timeToNext = Environment.TickCount + framerate - tick;
                    if (timeToNext > 0) Thread.Sleep(timeToNext);
                }
                br.Close();
            });
        }
        /*
        //3よりcpu使用率高い
        private void DrawLoop4()
        {
            Task.Run(() =>
            {
                PixelFormat format = PixelFormat.Format32bppRgb;
                Bitmap bmp = new Bitmap(800, 800, format);
                int wh = 178 << 16 | 178 << 8 | 178;
                int bl = 9 << 16 | 9 << 8 | 9;
                int framerate = 1000 / 10;
                int animeIndex = 0;

                BinaryReader br = new BinaryReader(new FileStream(Directory.GetCurrentDirectory() + "/image/ImageBin.bin", FileMode.Open));

                while (Form1.drawing)
                {
                    BitmapData bitmapdata = bmp.LockBits(new Rectangle(0, 0, 800, 800), ImageLockMode.WriteOnly, format);
                    IntPtr ptr = bitmapdata.Scan0;

                    br.BaseStream.Position = animeIndex * 640000;
                    unsafe
                    {
                        int* dst = (int*)ptr.ToPointer();
                        for (int i = 0; i < 640000; i++)
                        {
                            if (br.Read() > 0) *dst++ = wh;
                            else *dst++ = bl;
                        }
                    }
                    bmp.UnlockBits(bitmapdata);
                    pictureBox1.Image = bmp;
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;

                    Thread.Sleep(framerate);
                }
                br.Close();
            });
        }*/
    }
}
