using System;
using System.Windows.Forms;

namespace Liwall
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        public static bool drawing = false;
        [STAThread]
        static void Main()
        {
            var progman = User32.FindWindow("Progman", null);
            User32.SendMessageTimeout(progman, 0x52C, new IntPtr(0), IntPtr.Zero, 0x0, 1000, out var result);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form3());
        }

    }
}
