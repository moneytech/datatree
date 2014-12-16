namespace DataTreeEditor
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMain = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.treeView = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.panelNode = new System.Windows.Forms.Panel();
            this.ctrlValue = new DataTreeEditor.CtrlValue();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOutput = new DataTreeEditor.CtrlOutput();
            this.txtCmd = new System.Windows.Forms.ComboBox();
            this.contextMenuStripTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStripMain.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelNode.SuspendLayout();
            this.contextMenuStripTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(713, 25);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "Menu";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuNew,
            this.menuOpen,
            this.menuSave,
            this.menuSaveAs});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(39, 21);
            this.menuFile.Text = "&File";
            // 
            // menuNew
            // 
            this.menuNew.Name = "menuNew";
            this.menuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuNew.Size = new System.Drawing.Size(155, 22);
            this.menuNew.Text = "&New";
            this.menuNew.Click += new System.EventHandler(this.menuNew_Click);
            // 
            // menuOpen
            // 
            this.menuOpen.Name = "menuOpen";
            this.menuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuOpen.Size = new System.Drawing.Size(155, 22);
            this.menuOpen.Text = "&Open";
            this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
            // 
            // menuSave
            // 
            this.menuSave.Name = "menuSave";
            this.menuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSave.Size = new System.Drawing.Size(155, 22);
            this.menuSave.Text = "&Save";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuSaveAs
            // 
            this.menuSaveAs.Name = "menuSaveAs";
            this.menuSaveAs.Size = new System.Drawing.Size(155, 22);
            this.menuSaveAs.Text = "Save &As";
            this.menuSaveAs.Click += new System.EventHandler(this.menuSaveAs_Click);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.splitContainer1);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 25);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(713, 599);
            this.panelMain.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtOutput);
            this.splitContainer1.Panel2.Controls.Add(this.txtCmd);
            this.splitContainer1.Size = new System.Drawing.Size(713, 599);
            this.splitContainer1.SplitterDistance = 472;
            this.splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.panelNode);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(713, 472);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.treeView);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(713, 436);
            this.panel2.TabIndex = 3;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 12);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(713, 424);
            this.treeView.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Tree";
            // 
            // panelNode
            // 
            this.panelNode.Controls.Add(this.ctrlValue);
            this.panelNode.Controls.Add(this.label2);
            this.panelNode.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelNode.Location = new System.Drawing.Point(0, 436);
            this.panelNode.Name = "panelNode";
            this.panelNode.Size = new System.Drawing.Size(713, 36);
            this.panelNode.TabIndex = 4;
            // 
            // ctrlValue
            // 
            this.ctrlValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlValue.Location = new System.Drawing.Point(0, 12);
            this.ctrlValue.Name = "ctrlValue";
            this.ctrlValue.Size = new System.Drawing.Size(713, 24);
            this.ctrlValue.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Node";
            // 
            // txtOutput
            // 
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(0, 0);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(713, 103);
            this.txtOutput.TabIndex = 3;
            this.txtOutput.Text = "";
            // 
            // txtCmd
            // 
            this.txtCmd.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtCmd.Location = new System.Drawing.Point(0, 103);
            this.txtCmd.Name = "txtCmd";
            this.txtCmd.Size = new System.Drawing.Size(713, 20);
            this.txtCmd.TabIndex = 2;
            // 
            // contextMenuStripTreeView
            // 
            this.contextMenuStripTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAdd,
            this.menuDelete,
            this.toolStripMenuItem2,
            this.menuCopy,
            this.menuCut,
            this.menuPaste});
            this.contextMenuStripTreeView.Name = "contextMenuStripTreeView";
            this.contextMenuStripTreeView.Size = new System.Drawing.Size(159, 120);
            // 
            // menuAdd
            // 
            this.menuAdd.Name = "menuAdd";
            this.menuAdd.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.menuAdd.Size = new System.Drawing.Size(158, 22);
            this.menuAdd.Text = "&Add";
            this.menuAdd.Click += new System.EventHandler(this.menuAdd_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.menuDelete.Size = new System.Drawing.Size(158, 22);
            this.menuDelete.Text = "&Delete";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(155, 6);
            // 
            // menuCopy
            // 
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCopy.Size = new System.Drawing.Size(158, 22);
            this.menuCopy.Text = "&Copy";
            this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // menuCut
            // 
            this.menuCut.Name = "menuCut";
            this.menuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuCut.Size = new System.Drawing.Size(158, 22);
            this.menuCut.Text = "Cu&t";
            this.menuCut.Click += new System.EventHandler(this.menuCut_Click);
            // 
            // menuPaste
            // 
            this.menuPaste.Name = "menuPaste";
            this.menuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuPaste.Size = new System.Drawing.Size(158, 22);
            this.menuPaste.Text = "&Paste";
            this.menuPaste.Click += new System.EventHandler(this.menuPaste_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 624);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Tree Editor";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelNode.ResumeLayout(false);
            this.panelNode.PerformLayout();
            this.contextMenuStripTreeView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuNew;
        private System.Windows.Forms.ToolStripMenuItem menuOpen;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox txtCmd;
        private DataTreeEditor.CtrlOutput txtOutput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Panel panelNode;
        private CtrlValue ctrlValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTreeView;
        private System.Windows.Forms.ToolStripMenuItem menuAdd;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStripMenuItem menuCut;
        private System.Windows.Forms.ToolStripMenuItem menuPaste;
    }
}

