using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dt;

namespace DataTreeEditor
{
    public partial class CtrlValue : UserControl
    {
        public delegate void ValueChangedHandler(object sender, string text);

        public event ValueChangedHandler ValueChanged;

        private string oldValue = null;

        public CtrlValue()
        {
            InitializeComponent();

            cmbValue.LostFocus += cmbValue_LostFocus;
            cmbValue.KeyDown += cmbValue_KeyDown;
        }

        public void SetData(object data)
        {
            if (data is string)
            {
                cmbValue.Text = "KEY VALUE PAIR";
                cmbValue.Enabled = false;

                oldValue = cmbValue.Text;
            }
            else if (data is ValueInfo)
            {
                ValueInfo v = (ValueInfo)data;
                string str = Util.FormatValue(FormMain.GetDatatree(), v.Value, true);
                cmbValue.Text = str;

                oldValue = cmbValue.Text;

                cmbValue.Enabled = !v.Readonly;
            }
            else
            {
                cmbValue.Text = string.Empty;
                cmbValue.Enabled = false;

                oldValue = cmbValue.Text;
            }
        }

        private void cmbValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                UpdateData();
        }

        private void cmbValue_LostFocus(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            if (ValueChanged != null)
                ValueChanged(this, cmbValue.Text);
        }

        public void Revert()
        {
            cmbValue.Text = oldValue;
        }
    }
}
