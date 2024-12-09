using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paragoniarz
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender,EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.StartPosition = FormStartPosition.Manual;
            int x = this.Location.X + (this.Width - form1.Width) / 2;
            int y = this.Location.Y + (this.Height - form1.Height) / 2;
            form1.Location = new Point(x,y);
            form1.Show();
            this.Hide();
        }
    }
}
