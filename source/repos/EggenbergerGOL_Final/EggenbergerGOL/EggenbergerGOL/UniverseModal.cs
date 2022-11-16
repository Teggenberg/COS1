using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EggenbergerGOL
{
    public partial class UniverseModal : Form
    {
        public UniverseModal()
        {
            InitializeComponent();
        }

        private void UniverseModal_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public int uniWidth
        {

            get
            {
                 return (int)numericUpDownWidth.Value;
            }

            set
            {
                numericUpDownWidth.Value = value;
            }
        }

        public int uniHeight
        {
            get
            {
                return (int)numericUpDownHeight.Value;
            }

            set
            {
                numericUpDownHeight.Value = value;
            }

        }

        public int Time
        {
            get
            {
                return (int)numericUpDownTime.Value;
            }

            set
            {
                numericUpDownTime.Value = value;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
