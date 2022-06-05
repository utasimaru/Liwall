using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Liwall
{
    public partial class Form5 : Form
    {
        IntPtr handle_workerw;
        IntPtr dc_workerw;
        IntPtr handle_form;
        IntPtr dc_form;
        Type DrawClass;
        public Form5()
        {
            //フォームの基本設定
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.pictureBox1.Location = new Point(0, 0);
            this.pictureBox1.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }
        public void DLLLoadPlay(string dllFileName)
        {
            try
            {
                var asm = Assembly.LoadFrom(dllFileName);
                var module = asm.GetModule(Path.GetFileName(dllFileName));
                DrawClass = module.GetType(Path.GetFileNameWithoutExtension(dllFileName) + ".DrawClass");
                Program.drawing = true;
                DrawLoop();
            }
            catch
            {
                MessageBox.Show("DLLの読み込みでエラーが起きました","エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void DrawLoop()
        {
            Task.Run(() =>
            {   
                //ポインタの設定
                handle_workerw = IntPtr.Zero;
                User32.EnumWindows((hwnd, lParam) =>
                {
                    IntPtr shell = User32.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                    if (shell != IntPtr.Zero) handle_workerw = User32.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);
                    return true;
                }, IntPtr.Zero);
                dc_workerw = User32.GetDCEx(handle_workerw, IntPtr.Zero, 0x403);

                try
                {
                    if (DrawClass != null)
                    {
                        dynamic d1 = Activator.CreateInstance(DrawClass);
                        d1.DrawStart();

                        int tick, timeToNext = 0;
                        int w = Screen.PrimaryScreen.Bounds.Width;
                        int h = Screen.PrimaryScreen.Bounds.Height;
                        handle_form = User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, null, "Form5");
                        dc_form = User32.GetDC(handle_form);
                        while (Program.drawing)
                        {
                            tick = Environment.TickCount;

                            pictureBox1.Image = d1.GetDrawBitmap();
                            GDI32.BitBlt(dc_workerw, 0, 0, w, h, dc_form, 0, 0, 0x00CC0020);
                            timeToNext = Environment.TickCount + Program.framerate - tick;
                            if (timeToNext > 0) Thread.Sleep(timeToNext);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("描写中にエラーが起きました", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                User32.ReleaseDC(handle_form, dc_form);
                User32.ReleaseDC(handle_workerw, dc_workerw);
            });
        }
        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.drawing = false;
        }
    }
}
