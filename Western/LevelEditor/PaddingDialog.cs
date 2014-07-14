using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor
{
    public partial class PaddingDialog : Form
    {
        public new int Left
        {
            get { return GetValue(_leftTextBox); }
        }

        public new int Top
        {
            get { return GetValue(_topTextBox); }
        }
        public new int Right
        {
            get { return GetValue(_rightTextBox); }
        }
        public new int Bottom
        {
            get { return GetValue(_bottomTextBox); }
        }

        public Rectangle Rectangle
        {
            get { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }
        }

        private int GetValue(TextBox tb)
        {
            int value;
            if (Int32.TryParse(tb.Text, out value))
            {
                if (value < 0)
                    value = 0;
                return value;
            }
            else
                return 0;
        }

        public PaddingDialog()
        {
            InitializeComponent();
        }


        private void PaddingDialog_Load(object sender, EventArgs e)
        {
            _okButton.Focus();
        }
    }
}
