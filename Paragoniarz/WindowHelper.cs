using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Paragoniarz
{
    public static class WindowHelper //NIEDZIALA
    {
        // Import biblioteki do zaokrąglania rogów okna
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );

        public static void SetWindowRoundCorners(Control control, int cornerRadius)
        {
            control.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, control.Width, control.Height, cornerRadius, cornerRadius)
            );
        }

        // Import funkcji do przesuwania okna
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public static void SetWindowRoundCorners(Form form, int radius)
        {
            form.Region = Region.FromHrgn(
                CreateRoundRectRgn(0, 0, form.Width, form.Height, radius, radius)
            );
        }

        public static void EnableWindowDragging(Control control, Form form)
        {
            control.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(form.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };
        }
    }
}
