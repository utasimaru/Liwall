using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Liwall
{
    public partial class Form1 : Form
    {
        IntPtr handle_wall;
        IntPtr dc_wall;
        IntPtr dc_canvas;
        public Form1()
        {
            InitializeComponent();
            handle_wall = IntPtr.Zero;
            User32.EnumWindows((hwnd, lParam) =>
            {
                IntPtr shell = User32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shell != IntPtr.Zero) handle_wall = User32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);

            dc_wall = User32.GetDCEx(handle_wall, IntPtr.Zero, 0x403);
            dc_canvas = GDI32.CreateCompatibleDC(dc_wall);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (!Program.drawing)
            {
                Program.drawing = true;
                DrawLoop();
            }
        }
        //安定
        private void DrawLoop()
        {
            Task.Run(() =>
            {
                Bitmap[] _animes = ImageToBytes.LoadRainyBootsAnime();
                IntPtr[] hb_animes = new IntPtr[6];
                Color wh = Color.FromArgb(178, 178, 178);
                Bitmap _back = new Bitmap(1920, 1080);
                for (int ix = 0; ix < _back.Width; ix++)
                {
                    for (int iy = 0; iy < _back.Height; iy++)
                    {
                        _back.SetPixel(ix, iy, wh);
                    }
                }
                IntPtr hb_back = _back.GetHbitmap();
                IntPtr dc_back= GDI32.CreateCompatibleDC(dc_wall);
                GDI32.SelectObject(dc_back, hb_back);

                for (int i = 0; i < 6; i++) hb_animes[i] = _animes[i].GetHbitmap();
                int framerate = 1000 / 10;
                int tick, timeToNext,animeIndex=0;
                while (Program.drawing)
                {
                    tick=Environment.TickCount;
                    GDI32.SelectObject(dc_canvas, hb_animes[animeIndex]);
                    animeIndex=animeIndex>=5?0:animeIndex+1;
                    
                    GDI32.BitBlt(dc_wall, 0, 0, 1920, 1080, dc_back, 0, 0, 0x00CC0020);
                    GDI32.BitBlt(dc_wall, 0, 0, 800, 800, dc_canvas, 0, 0, 0x00CC0020);

                    timeToNext = Environment.TickCount + framerate - tick; ;
                    if (timeToNext>0)Thread.Sleep(timeToNext);
                }
                GDI32.DeleteDC(dc_canvas);
                User32.ReleaseDC(handle_wall, dc_wall);
            });
        }
        //メモリ削減案1。遅いしメモリも食う。
        /*
        private void DrawLoop2()
        {
            Task.Run(() =>
            {
                IntPtr hbitmap = new IntPtr();
                Bitmap bitmap = new Bitmap(800, 800);
                Color wh = Color.FromArgb(255, 178, 178, 178);
                Color bl = Color.FromArgb(255, 9, 9, 9);

                int framerate = 1000 / 10;
                int tick, timeToNext, animeIndex = 0;

                BinaryReader br = new BinaryReader(new FileStream(Directory.GetCurrentDirectory() + "/image/ImageBin.bin", FileMode.Open));
                while (drawing)
                {
                    tick = Environment.TickCount;
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;

                    br.BaseStream.Position = animeIndex * 640000;
                    for (int i = 0; i < 640000; i++)
                    {
                        if (br.Read() > 0) bitmap.SetPixel(i % 800, i / 800, wh);
                        else bitmap.SetPixel(i % 800, i / 800, bl);
                    }

                    hbitmap = bitmap.GetHbitmap();
                    GDI32.SelectObject(dc_canvas, hbitmap);
                    GDI32.BitBlt(dc_wall, 0, 0, 800, 800, dc_canvas, 0, 0, 0x00CC0020);

                    timeToNext = Environment.TickCount + framerate - tick; ;
                    if (timeToNext > 0) Thread.Sleep(timeToNext);
                }
                br.Close();
                GDI32.DeleteDC(dc_canvas);
                User32.ReleaseDC(handle_wall, dc_wall);
            });
        }*/
        private void DrawLoop3()
        {
            Task.Run(() =>
            {
                IntPtr hbitmap = new IntPtr();
                PixelFormat format = PixelFormat.Format32bppRgb;
                Bitmap[] bitmap = new Bitmap[2];
                bitmap[0] = new Bitmap(800, 800,format);
                bitmap[1] = new Bitmap(800, 800, format);
                int wh = 178 << 16 | 178 << 8 | 178;
                int bl = 9 << 16 | 9 << 8 | 9;

                int framerate = 1000 / 10;
                int tick, timeToNext, animeIndex = 0;

                hbitmap = bitmap[1].GetHbitmap();
                GDI32.SelectObject(dc_canvas, hbitmap);
                GDI32.BitBlt(dc_wall, 0, 0, 800, 800, dc_canvas, 0, 0, 0x00CC0020);

                BinaryReader br = new BinaryReader(new FileStream(Directory.GetCurrentDirectory() + "/image/ImageBin.bin", FileMode.Open));
                while (Program.drawing)
                {
                    int ib = animeIndex % 2;
                    tick = Environment.TickCount;
                    //////////////
                    BitmapData bitmapdata = bitmap[ib].LockBits(new Rectangle(0, 0, 800, 800), ImageLockMode.WriteOnly, format);
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
                    bitmap[ib].UnlockBits(bitmapdata);
                    //hbitmap = bitmap[ib].GetHbitmap();
                    GDI32.SelectObject(dc_canvas, hbitmap);
                    GDI32.BitBlt(dc_wall, 0, 0, 800, 800, dc_canvas, 0, 0, 0x00CC0020);
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;

                    timeToNext = Environment.TickCount + framerate - tick;
                    if (timeToNext > 0) Thread.Sleep(timeToNext);
                }
                br.Close();
                GDI32.DeleteDC(dc_canvas);
                User32.ReleaseDC(handle_wall, dc_wall);
            });
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.drawing = false;
        }
    }
}
