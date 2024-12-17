using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace Paragoniarz
{
    public partial class FindFileControl : UserControl
    {
        public FindFileControl()
        {
            InitializeComponent();

            WindowHelper.SetWindowRoundCorners(panel1,10);
            WindowHelper.SetWindowRoundCorners(panel2,10);
            WindowHelper.SetWindowRoundCorners(panel3,10);
            WindowHelper.SetWindowRoundCorners(panel4,10);

        }


        private void gradientPanel(object sender,PaintEventArgs e)
        {
            // Definicja kolorów dla gradientu
            Color startColor = Color.FromArgb(75,0,130); // Kolor początkowy (np. czerwony)
            Color endColor = Color.FromArgb(0,128,128); // Kolor końcowy (np. niebieski)
            // Tworzymy pędzel z gradientem
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel1.ClientRectangle,  // Określamy obszar panelu
                startColor,              // Kolor początkowy
                endColor,                // Kolor końcowy
                45F))                    // Kąt gradientu (w tym przypadku 45 stopni)
            {
                // Rysujemy tło panelu z użyciem gradientu
                e.Graphics.FillRectangle(brush,panel1.ClientRectangle);
            }
        }

        private void panel1_Paint(object sender,PaintEventArgs e)
        {
            gradientPanel(sender,e);
        }

        private void panel2_Paint(object sender,PaintEventArgs e)
        {
            gradientPanel(sender,e);

        }

        private void panel3_Paint(object sender,PaintEventArgs e)
        {
            gradientPanel(sender,e);
            
        }

        private void panel4_Paint(object sender,PaintEventArgs e)
        {
            gradientPanel(sender,e);
            
        }
    }
}
