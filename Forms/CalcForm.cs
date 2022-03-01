using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinCalculator.Core;

namespace WinCalculator.Forms
{
    public partial class CalcForm : Form
    {
        private string _mathOperation;
        public string MathOperation
        {
            get => _mathOperation ?? "";
            set
            {
                _mathOperation = value;
                txtbx_CalcNum.Text = value;
                txtbx_CalcNum.ScrollToCaret();
            }
        }

        public CalcForm()
        {
            InitializeComponent();
        }

        private void Click_CalculatorOrOperation(object sender, EventArgs e) // any number/operation clicked
        {
            MathOperation += (sender as Button)?.Text ?? "";
        }

        private void btn_Equals_Click(object sender, EventArgs e)
        {
            try
            {
                MathOperation = Calculator.Evaluate(MathOperation).ToString();
            }
            catch
            {
                MathOperation = "";
                txtbx_CalcNum.Text = "Op error";
            }
        }

        private void btn_Remove_Click(object sender, EventArgs e)
        {
            if (MathOperation.Length > 0)
            {
                MathOperation = MathOperation.Substring(0, MathOperation.Length - 1);
            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            MathOperation = "";
        }
    }
}
