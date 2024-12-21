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

            
            WindowHelper.SetWindowRoundCorners(panel6,10);

        }


        private void gradientPanel(object sender,PaintEventArgs e, Panel panel)
        {
            // Definicja kolorów dla gradientu
            Color startColor = Color.FromArgb(0,64,95); // Kolor początkowy (np. czerwony)
            Color endColor = Color.FromArgb(64,64,64); // Kolor końcowy (np. niebieski)
            // Tworzymy pędzel z gradientem
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,  // Określamy obszar panelu
                startColor,              // Kolor początkowy
                endColor,                // Kolor końcowy
                45F))                    // Kąt gradientu (w tym przypadku 45 stopni)
            {
                // Rysujemy tło panelu z użyciem gradientu
                e.Graphics.FillRectangle(brush,panel.ClientRectangle);
            }
        }

       

        private void panel6_Paint(object sender,PaintEventArgs e)
        {
            gradientPanel(sender,e,panel6);
        }
    }
}
