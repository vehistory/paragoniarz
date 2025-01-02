using System.Windows.Forms;

namespace Paragoniarz
{
    public partial class FindFileControl : UserControl
    {
        public FindFileControl()
        {
            InitializeComponent();

            WindowHelper.SetWindowRoundCorners(panel1, 10);
            WindowHelper.SetWindowRoundCorners(panel2, 10);
            WindowHelper.SetWindowRoundCorners(panel3, 10);
            WindowHelper.SetWindowRoundCorners(panel4, 10);
        }

        private void gradientPanel(object sender, PaintEventArgs e) { }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            gradientPanel(sender, e);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            gradientPanel(sender, e);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            gradientPanel(sender, e);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            gradientPanel(sender, e);
        }
    }
}
