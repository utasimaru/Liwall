using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Liwall
{
    public partial class Form3 : Form
    {
        IntPtr handle_workerw;
        IntPtr dc_workerw;
        IntPtr handle_form;
        IntPtr dc_form;
        int animeIndex;
        Bitmap[] bitmap;
        public Form3()
        {
            InitializeComponent();
            handle_workerw = IntPtr.Zero;
            User32.EnumWindows((hwnd, lParam) =>
            {
                IntPtr shell = User32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shell != IntPtr.Zero) handle_workerw = User32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);

            dc_workerw = User32.GetDCEx(handle_workerw, IntPtr.Zero, 0x403);
            BackColor = Color.FromArgb(178, 178, 178);
            Program.drawing = true;
            DrawLoop2();
        }

        private void DrawLoop()
        {
            Task.Run(() =>
            {
                PixelFormat format = PixelFormat.Format32bppRgb;
                bitmap = new Bitmap[2];
                bitmap[0] = new Bitmap(800, 800, format);
                bitmap[1] = new Bitmap(800, 800, format);
                pictureBox1.Image = bitmap[1];
                int wh = 178 << 16 | 178 << 8 | 178;
                int bl = 9 << 16 | 9 << 8 | 9;

                int framerate = 1000 / 60;
                int tick, timeToNext = 0;

                handle_form = User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Form3");
                dc_form = User32.GetDC(handle_form);
                int w = Screen.PrimaryScreen.Bounds.Width;
                int h = Screen.PrimaryScreen.Bounds.Height;

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
                    pictureBox1.Image = bitmap[ib];
                    GDI32.BitBlt(dc_workerw, 0, 0, w, h, dc_form, 0, 0, 0x00CC0020);
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;

                    timeToNext = Environment.TickCount + framerate - tick;
                    if (timeToNext > 0) Thread.Sleep(timeToNext);
                }
                br.Close();
                User32.ReleaseDC(handle_form, dc_form);
                User32.ReleaseDC(handle_workerw, dc_workerw);
            });
        }
        private void DrawLoop2()
        {
            Task.Run(() =>
            {
                bitmap = ImageToBytes.LoadRainyBootsAnime2();
                pictureBox1.Image = bitmap[1];

                int framerate = 1000 / 20;
                //int tick, timeToNext = 0;
                int w = Screen.PrimaryScreen.Bounds.Width;
                int h = Screen.PrimaryScreen.Bounds.Height;
                handle_form = User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Form3");
                dc_form = User32.GetDC(handle_form);

                while (Program.drawing)
                {
                    //tick = Environment.TickCount;
                    
                    pictureBox1.Image = bitmap[animeIndex];
                    GDI32.BitBlt(dc_workerw, 0, 0, w, h, dc_form, 0, 0, 0x00CC0020);
                    animeIndex = animeIndex >= 5 ? 0 : animeIndex + 1;

                    //timeToNext = Environment.TickCount + framerate - tick;
                    //if (timeToNext > 0) Thread.Sleep(timeToNext);
                    Thread.Sleep(framerate);
                }
                User32.ReleaseDC(handle_form, dc_form);
                User32.ReleaseDC(handle_workerw, dc_workerw);
            });
        }
        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.drawing = false;
        }
    }
}
