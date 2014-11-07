namespace DataTreeEditor
{
    partial class CtrlValue
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlSimpleValue = new System.Windows.Forms.Panel();
            this.cmbValue = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlSimpleValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSimpleValue
            // 
            this.pnlSimpleValue.Controls.Add(this.cmbValue);
            this.pnlSimpleValue.Controls.Add(this.label1);
            this.pnlSimpleValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSimpleValue.Location = new System.Drawing.Point(0, 0);
            this.pnlSimpleValue.Name = "pnlSimpleValue";
            this.pnlSimpleValue.Size = new System.Drawing.Size(704, 571);
            this.pnlSimpleValue.TabIndex = 0;
            // 
            // cmbValue
            // 
            this.cmbValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbValue.FormattingEnabled = true;
            this.cmbValue.Items.AddRange(new object[] {
            "null",
            "false",
            "true",
            "0",
            "{ }",
            "[ ]"});
            this.cmbValue.Location = new System.Drawing.Point(35, 0);
            this.cmbValue.Name = "cmbValue";
            this.cmbValue.Size = new System.Drawing.Size(669, 20);
            this.cmbValue.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Value";
            // 
            // CtrlValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlSimpleValue);
            this.Name = "CtrlValue";
            this.Size = new System.Drawing.Size(704, 571);
            this.pnlSimpleValue.ResumeLayout(false);
            this.pnlSimpleValue.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSimpleValue;
        private System.Windows.Forms.ComboBox cmbValue;
        private System.Windows.Forms.Label label1;
    }
}
