using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notesieve
{
    public partial class HelperForm : Form
    {
        public HelperForm()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }

        private void HelperForm_Load(object sender, EventArgs e)
        {
            
        }

        Point oldPos;
        bool isDragging = false;
        Point oldMouse;
        private void MyForm_MouseDown(object sender, MouseEventArgs e)
        {
            this.isDragging = true;
            this.oldPos = this.Location;
            this.oldMouse = e.Location;
        }

        private void MyForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                this.Location = new Point(oldPos.X + (e.X - oldMouse.X), oldPos.Y + (e.Y - oldMouse.Y));
            }
        }

        private void MyForm_MouseUp(object sender, MouseEventArgs e)
        {
            this.isDragging = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
