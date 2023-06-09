using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSNotes
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void button_textColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Color selectedColor = colorDialog.Color;
                // Use the selected color as needed
                MessageBox.Show(""+selectedColor.R +", "+ selectedColor.G + "," + selectedColor.B);
            }
        }

        private void button_backgroundColor_Click(object sender, EventArgs e)
        {

            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Color selectedColor = colorDialog.Color;
                // Use the selected color as needed
                MessageBox.Show("" + selectedColor.R + ", " + selectedColor.G + "," + selectedColor.B);
            }
        }
    }
}
