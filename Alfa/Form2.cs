using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alfa
{
    public partial class Form2 : Form
    {
        public Form2(int Size)
        {
            InitializeComponent();            
            SquareSize = Size;
            trackBar1.Minimum = 0;
            trackBar1.Maximum = (SquareSize / 2) - 3;
            //trackBar1.Value = 0;
            trackBar2.Minimum = (SquareSize / 2) + 3;
            trackBar2.Maximum = SquareSize;
            trackBar2.Value = SquareSize;
            label1.Text = trackBar1.Value.ToString();
            label2.Text = trackBar2.Value.ToString();
            _a = Convert.ToInt32(trackBar1.Value.ToString());
            _b = Convert.ToInt32(trackBar2.Value.ToString());
        }
       
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            _a = Convert.ToInt32(trackBar1.Value.ToString());
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label2.Text = trackBar2.Value.ToString();
            _b = Convert.ToInt32(trackBar2.Value.ToString());
        }
        private int SquareSize = 0;
        private int _lawNumber = 0;
        private int _a = 0;
        private int _b = 0;
        public int LawNumber()
        {
            if (radioButton1.Checked) _lawNumber = 1;
            else
            if (radioButton2.Checked) _lawNumber = 2;
            else
            if (radioButton3.Checked) _lawNumber = 3;
            else
            if (radioButton4.Checked) _lawNumber = 4;
            return _lawNumber;
        }
        
        public int A
        {
            get { return _a; }
            set { this._a = value; }
        }
        public int B
        {
            get { return _b; }
            set { this._b = value; }
        }
    }
}