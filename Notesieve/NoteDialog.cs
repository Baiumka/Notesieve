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
    public partial class NoteDialog : Form
    {

        public enum NoteDialogType
        {
            dConfirm,
            dError,
            dWarning
        }

        public NoteDialog(string labelText, NoteDialogType type)
        {
            InitializeComponent();
            label1.Text = labelText;
            switch(type)
            {
                case NoteDialogType.dError:
                    panel1.BackColor = Color.DarkRed;
                    pictureBox1.Image = new Bitmap(Notesieve.Properties.Resources.x);
                    break;
                case NoteDialogType.dConfirm:
                    panel1.BackColor = Color.Orange;
                    pictureBox1.Image = new Bitmap(Notesieve.Properties.Resources.confirm);
                    break;
                case NoteDialogType.dWarning:
                    panel1.BackColor = Color.DarkGray;
                    pictureBox1.Image = new Bitmap(Notesieve.Properties.Resources.alert);
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
