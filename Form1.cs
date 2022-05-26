using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Liwall
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PAINTSTRUCT
    {
        public IntPtr hdc;
        public bool fErase;
        public RECT rcPaint;
        public bool fRestore;
        public bool fIncUpdate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] rgbReserved;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public partial class Form1 : Form
    {

        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, uint flags);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);


        public Form1()
        {
            InitializeComponent();
            for (int ix = 0; ix < 1920; ix++)
            {
                for (int iy = 0; iy < 1080; iy++)
                {
                    int c = (int)((ix + iy) * 255 / (1920 + 1080));
                    bitmap.SetPixel(ix, iy, Color.FromArgb(c, c, c));
                }
            }
        }
        Bitmap bitmap = new Bitmap(1920, 1080);
        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr handle_wall = IntPtr.Zero;
            EnumWindows((hwnd, lParam) =>
            {
                IntPtr shell = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shell != IntPtr.Zero) handle_wall = FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                return true;
            }, IntPtr.Zero);

            IntPtr dc_wall = GetDCEx(handle_wall, IntPtr.Zero, 0x403);
            IntPtr dc_canvas = CreateCompatibleDC(dc_wall);
            SelectObject(dc_canvas, bitmap.GetHbitmap());
            BitBlt(dc_wall, 0, 0, bitmap.Width, bitmap.Height, dc_canvas, 0, 0, 0x00CC0020);

            DeleteDC(dc_canvas);
            ReleaseDC(handle_wall, dc_wall);
        }
    }
}
