using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using dt;

namespace DataTreeEditor
{
    public partial class FormMain : Form
    {
        #region Constants

        private const string FORM_TITLE = "Data Tree Editor";

        private const string FILE_FILTER = "Datatree files(*.dt)|*.dt|All files(*.*)|*.*";

        #endregion

        #region Fields

        private static FormMain self = null;

        private EditStates editState = EditStates.Closed;

        private IntPtr datatree = IntPtr.Zero;

        private IntPtr command = IntPtr.Zero;

        private string dbPath = null;

        private bool openOnStart = false;

        #endregion

        #region Properties

        public EditStates EditState
        {
            get { return editState; }
            set
            {
                editState = value;
                UpdateEditState();
            }
        }

        private string LastFileInfoFile
        {
            get
            {
                return Path.GetDirectoryName(Application.ExecutablePath) + "\\lastfile";
            }
        }

        #endregion

        #region Constructor / destructor

        public FormMain()
        {
            Constructor();
        }

        public FormMain(string file)
        {
            openOnStart = true;

            Constructor();

            Open(file);
        }

        private void Constructor()
        {
            self = this;

            InitializeComponent();

            ctrlValue.ValueChanged += ctrlValue_ValueChanged;

            treeView.NodeMouseClick += treeView_NodeMouseClick;

            txtCmd.KeyDown += txtCmd_KeyDown;

            DatatreeDotNet.dt_create_datatree(ref datatree, OnError);
            DatatreeDotNet.dt_create_command(ref command);

            Text = FORM_TITLE;

            New();

            EditState = EditStates.Closed;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                self = null;

                DatatreeDotNet.dt_unload_datatree(datatree);
                DatatreeDotNet.dt_destroy_datatree(datatree);

                DatatreeDotNet.dt_destroy_command(command);

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Singleton

        public static IntPtr GetDatatree()
        {
            return self.datatree;
        }

        #endregion

        #region Common

        #region File

        private void UpdateEditState()
        {
            menuNew.Enabled = true;
            menuOpen.Enabled = true;
            menuSave.Enabled = false;
            menuSaveAs.Enabled = false;
            switch (editState)
            {
                case EditStates.Closed:
                    break;
                case EditStates.New:
                    menuSave.Enabled = true;
                    menuSaveAs.Enabled = true;
                    break;
                case EditStates.OpenedNotSaved:
                    menuSave.Enabled = true;
                    menuSaveAs.Enabled = true;
                    break;
                case EditStates.Saved:
                    menuSaveAs.Enabled = true;
                    break;
            }

            if (string.IsNullOrEmpty(dbPath))
            {
                Text = FORM_TITLE + " *";
            }
            else
            {
                Text = FORM_TITLE + " - ";
                if (editState == EditStates.New || editState == EditStates.OpenedNotSaved)
                    Text += "*";
                Text += dbPath;
            }
        }

        private bool ConfirmSave()
        {
            if (editState == EditStates.Closed || editState == EditStates.Saved)
                return true;

            DialogResult dr = Util.AskYesNoCancel(this, "Save current working datatree before further operations?");
            if (dr == DialogResult.No)
                return true;
            else if (dr == DialogResult.Cancel)
                return false;

            if (string.IsNullOrEmpty(dbPath))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = FILE_FILTER;
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    dbPath = sfd.FileName;
                else
                    return false;
            }

            return Save();
        }

        private void New()
        {
            dbPath = null;

            if (datatree != IntPtr.Zero)
                DatatreeDotNet.dt_unload_datatree(datatree);
            DatatreeDotNet.dt_load_datatree_string(datatree, "null");

            UpdateNodes();

            EditState = EditStates.New;
        }

        private void Open(string f)
        {
            dbPath = f;

            DatatreeDotNet.dt_load_datatree_file(datatree, dbPath);

            UpdateNodes();

            EditState = EditStates.Saved;
        }

        private void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FILE_FILTER;
            if (ofd.ShowDialog(this) != DialogResult.OK)
                return;

            Open(ofd.FileName);
        }

        private bool Save()
        {
            Debug.Assert(!string.IsNullOrEmpty(dbPath));

            DatatreeDotNet.dt_save_datatree_file(datatree, dbPath, DatatreeDotNet.dt_bool_t.DT_FALSE);

            EditState = EditStates.Saved;

            return true;
        }

        #endregion

        #region Node

        private void Add(Func<string> source)
        {
            if (treeView.SelectedNode == null)
            {
                Message("Nothing selected." + Environment.NewLine);
                return;
            }

            if (!(treeView.SelectedNode.Tag is ValueInfo))
            {
                Message("Cannot add to this node." + Environment.NewLine);
                return;
            }

            ValueInfo valueInfo = (ValueInfo)treeView.SelectedNode.Tag;
            DatatreeDotNet.dt_type_t t = DatatreeDotNet.dt_value_type(valueInfo.Value);
            if (t == DatatreeDotNet.dt_type_t.DT_ARRAY)
            {
                string ipt = source();

                IntPtr o = IntPtr.Zero;
                DatatreeDotNet.dt_create_value(datatree, ref o, OnError, ipt);
                uint idx = 0;
                DatatreeDotNet.dt_add_array_elem
                (
                    datatree,
                    DatatreeDotNet.dt_value_data(valueInfo.Value),
                    o,
                    ref idx
                );
            }
            else if (t == DatatreeDotNet.dt_type_t.DT_OBJECT)
            {
                string ipt = source();

                IntPtr k = IntPtr.Zero;
                IntPtr v = IntPtr.Zero;
                DatatreeDotNet.dt_create_value(datatree, ref k, OnError, ipt);
                DatatreeDotNet.dt_create_value(datatree, ref v, OnError, "null");
                uint idx = 0;
                DatatreeDotNet.dt_status_t status = DatatreeDotNet.dt_add_object_member
                (
                    datatree,
                    DatatreeDotNet.dt_value_data(valueInfo.Value),
                    k,
                    v,
                    ref idx
                );
                if (status == DatatreeDotNet.dt_status_t.DT_KEY_EXISTS)
                {
                    Message("A key-value pair with the same key already exists." + Environment.NewLine);
                    return;
                }
            }
            else
            {
                Message("Cannot add to this node." + Environment.NewLine);
                return;
            }

            treeView.SelectedNode.Nodes.Add(new TreeNode());

            UpdateNodes();

            EditState = EditStates.OpenedNotSaved;
        }

        private void Delete()
        {
            if (treeView.SelectedNode == null)
            {
                Message("Nothing selected." + Environment.NewLine);
                return;
            }

            TreeNode parent = treeView.SelectedNode.Parent;
            if ((parent == null) || !(parent.Tag is ValueInfo))
            {
                Message("Cannot remove this node." + Environment.NewLine);
                return;
            }

            ValueInfo valueInfo = (ValueInfo)parent.Tag;
            DatatreeDotNet.dt_type_t t = DatatreeDotNet.dt_value_type(valueInfo.Value);
            if (t == DatatreeDotNet.dt_type_t.DT_ARRAY)
            {
                uint w = (uint)parent.Nodes.IndexOf(treeView.SelectedNode);
                IntPtr o = IntPtr.Zero;
                DatatreeDotNet.dt_create_value(datatree, ref o, OnError, null, DatatreeDotNet.dt_type_t.DT_NULL);
                DatatreeDotNet.dt_remove_array_elem_at
                (
                    datatree,
                    DatatreeDotNet.dt_value_data(valueInfo.Value),
                    w,
                    o
                );
                DatatreeDotNet.dt_destroy_value(datatree, o);
            }
            else if (t == DatatreeDotNet.dt_type_t.DT_OBJECT)
            {
                uint w = (uint)parent.Nodes.IndexOf(treeView.SelectedNode);
                IntPtr k = IntPtr.Zero;
                IntPtr v = IntPtr.Zero;
                DatatreeDotNet.dt_create_value(datatree, ref k, OnError, null, DatatreeDotNet.dt_type_t.DT_NULL);
                DatatreeDotNet.dt_create_value(datatree, ref v, OnError, null, DatatreeDotNet.dt_type_t.DT_NULL);
                DatatreeDotNet.dt_remove_object_member_at
                (
                    datatree,
                    DatatreeDotNet.dt_value_data(valueInfo.Value),
                    w,
                    k,
                    v
                );
                DatatreeDotNet.dt_destroy_value(datatree, k);
                DatatreeDotNet.dt_destroy_value(datatree, v);
            }
            else
            {
                Debug.Fail("Impossible");
            }

            parent.Nodes.Remove(treeView.SelectedNode);

            EditState = EditStates.OpenedNotSaved;
        }

        private bool Copy()
        {
            if (treeView.SelectedNode == null)
            {
                Message("No available selected." + Environment.NewLine);
                return false;
            }

            if (!(treeView.SelectedNode.Tag is ValueInfo))
            {
                Message("Cannot retrieve this node." + Environment.NewLine);
                return false;
            }

            ValueInfo valueInfo = (ValueInfo)treeView.SelectedNode.Tag;
            string str = Util.FormatValue(datatree, valueInfo.Value, false);
            Clipboard.SetText(str);

            return true;
        }

        private void EnsureNodeCount(TreeNode node, int count)
        {
            int d = node.Nodes.Count - count;
            if (d > 0)
            {
                for (int i = 0; i < d; i++)
                    node.Nodes.RemoveAt(count);
            }
            else if (d < 0)
            {
                d = -d;
                for (int i = 0; i < d; i++)
                    node.Nodes.Add(new TreeNode());
            }
        }

        private void WalkValue(TreeNode node, IntPtr v, bool readOnly)
        {
            DatatreeDotNet.dt_type_t t = DatatreeDotNet.dt_value_type(v);
            switch (t)
            {
                case DatatreeDotNet.dt_type_t.DT_NULL:
                case DatatreeDotNet.dt_type_t.DT_BOOL:
                case DatatreeDotNet.dt_type_t.DT_LONG:
                case DatatreeDotNet.dt_type_t.DT_DOUBLE:
                case DatatreeDotNet.dt_type_t.DT_STRING:
                    node.Tag = new ValueInfo(v, readOnly, false);

                    string str = Util.FormatValue(datatree, v, false);
                    node.Text = str;
                    if (node.Nodes.Count != 0)
                        node.Nodes.Clear();
                    break;
                case DatatreeDotNet.dt_type_t.DT_OBJECT:
                    node.Tag = new ValueInfo(v, readOnly, true);

                    node.Text = "{ ... }";
                    WalkObject(node, DatatreeDotNet.dt_value_data(v));
                    break;
                case DatatreeDotNet.dt_type_t.DT_ARRAY:
                    node.Tag = new ValueInfo(v, readOnly, true);

                    node.Text = "[ ... ]";
                    WalkArray(node, DatatreeDotNet.dt_value_data(v));
                    break;
            }
        }

        private void WalkPair(TreeNode parent, IntPtr k, IntPtr v, uint index)
        {
            TreeNode pairNode = parent.Nodes[(int)index];
            EnsureNodeCount(pairNode, 2);

            TreeNode keyNode = pairNode.Nodes[0];
            WalkValue(keyNode, k, true);

            TreeNode valNode = pairNode.Nodes[1];
            WalkValue(valNode, v, false);

            pairNode.Tag = keyNode.Text;

            pairNode.Name = pairNode.Text = keyNode.Text + " : " + valNode.Text;
        }

        private void WalkObject(TreeNode parent, IntPtr o)
        {
            uint c = 0;
            DatatreeDotNet.dt_object_member_count(datatree, o, ref c);

            EnsureNodeCount(parent, (int)c);

            for (uint i = 0; i < c; i++)
            {
                IntPtr k = IntPtr.Zero;
                IntPtr v = IntPtr.Zero;
                DatatreeDotNet.dt_object_member_at(datatree, o, i, ref k, ref v);

                WalkPair(parent, k, v, i);
            }
        }

        private void WalkArray(TreeNode parent, IntPtr a)
        {
            uint c = 0;
            DatatreeDotNet.dt_array_elem_count(datatree, a, ref c);

            EnsureNodeCount(parent, (int)c);

            for (uint i = 0; i < c; i++)
            {
                IntPtr v = IntPtr.Zero;
                DatatreeDotNet.dt_array_elem_at(datatree, a, i, ref v);

                TreeNode node = parent.Nodes[(int)i];
                WalkValue(node, v, false);
            }
        }

        private void UpdateNodes()
        {
            treeView.Visible = treeView.Enabled = false;

            if (treeView.Nodes.Count == 0)
            {
                TreeNode root = new TreeNode();
                WalkValue(root, DatatreeDotNet.dt_root_value(datatree), false);
                treeView.Nodes.Add(root);
            }
            else
            {
                TreeNode root = treeView.Nodes[0];
                treeView.Nodes.Clear();
                WalkValue(root, DatatreeDotNet.dt_root_value(datatree), false);
                treeView.Nodes.Add(root);
            }

            treeView.Visible = treeView.Enabled = true;
        }
      
        #endregion

        #region Callback

        private void OnError(DatatreeDotNet.dt_status_t status, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string msg, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string pos, IntPtr row, IntPtr col)
        {
            string _msg = "Datatree parsing error:" + Environment.NewLine;
            _msg += "  error code: " + status.ToString() + Environment.NewLine;
            _msg += "  error message: " + msg + Environment.NewLine;
            _msg += "  error pos: {row: " + row.ToString() + ", col: " + col.ToString() + "}" + Environment.NewLine;

            Error(_msg);
        }

        #endregion

        #region Output

        private void Message(string txt)
        {
            txtOutput.Append(txt, Color.Black);
        }

        private void Warning(string txt)
        {
            txtOutput.Append(txt, Color.Yellow);
        }

        private void Error(string txt)
        {
            txtOutput.Append(txt, Color.Red);
        }

        #endregion

        #endregion

        #region Form events

        #region Form

        private void FormMain_Load(object sender, EventArgs e)
        {
            Message("Ready" + Environment.NewLine);

            if (!openOnStart && File.Exists(LastFileInfoFile))
            {
                using (FileStream fs = new FileStream(LastFileInfoFile, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string f = sr.ReadToEnd();
                        if (File.Exists(f))
                            Open(f);
                    }
                }
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmSave())
            {
                e.Cancel = true;
            }
            else
            {
                using (FileStream fs = new FileStream(LastFileInfoFile, FileMode.OpenOrCreate, FileAccess.Write))
                {
                }
                using (FileStream fs = new FileStream(LastFileInfoFile, FileMode.Truncate, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        if (!string.IsNullOrEmpty(dbPath))
                            sw.Write(dbPath);
                    }
                }
            }
        }

        #endregion

        #region Main menu

        private void menuNew_Click(object sender, EventArgs e)
        {
            if (ConfirmSave())
                New();
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            if (ConfirmSave())
                Open();
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = FILE_FILTER;
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    dbPath = sfd.FileName;
                    Save();
                }
            }
            else
            {
                Save();
            }
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = FILE_FILTER;
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                dbPath = sfd.FileName;

                Save();
            }
        }

        #endregion

        #region Context menu

        private void menuAdd_Click(object sender, EventArgs e)
        {
            Add(() => { return "\"" + Util.Input("Input a value:", "null") + "\""; });
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            if (!Copy())
                return;

            Delete();
        }

        private void menuPaste_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
                return;

            Add(() => { return Clipboard.GetText(); });
        }

        #endregion

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeView.SelectedNode != e.Node)
                treeView.SelectedNode = e.Node;

            ctrlValue.SetData(e.Node.Tag);

            if (e.Button == MouseButtons.Right)
                contextMenuStripTreeView.Show(treeView, e.Location);
        }

        private void ctrlValue_ValueChanged(object sender, string text)
        {
            if (treeView.SelectedNode == null)
                return;
            if (!(treeView.SelectedNode.Tag is ValueInfo))
                return;

            ValueInfo valueInfo = (ValueInfo)treeView.SelectedNode.Tag;
            if(valueInfo.Readonly)
                return;

            IntPtr v = IntPtr.Zero;
            int r = DatatreeDotNet.dt_create_value(datatree, ref v, OnError, text);
            if (r == 0)
            {
                ctrlValue.Revert();
                return;
            }

            DatatreeDotNet.dt_value_mem_swap(valueInfo.Value, v);
            DatatreeDotNet.dt_destroy_value(datatree, v);

            EditState = EditStates.OpenedNotSaved;

            UpdateNodes();
        }

        private void txtCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(txtCmd.Text))
                    return;

                if (txtCmd.Text == "clear")
                {
                    txtOutput.Clear();
                    if (!txtCmd.Items.Contains(txtCmd.Text))
                        txtCmd.Items.Add(txtCmd.Text);
                    txtCmd.Text = string.Empty;
                    return;
                }

                DatatreeDotNet.dt_clear_command(command);
                DatatreeDotNet.dt_parse_command(command, OnError, txtCmd.Text);
                IntPtr ret = IntPtr.Zero;
                DatatreeDotNet.dt_query_status_t status = DatatreeDotNet.dt_query(DatatreeDotNet.dt_root_value(datatree), command, ref ret);
                if (status == DatatreeDotNet.dt_query_status_t.DTQ_GOT_NOTHING)
                {
                }
                else if (status == DatatreeDotNet.dt_query_status_t.DTQ_GOT_REF || status == DatatreeDotNet.dt_query_status_t.DTQ_GOT_NOREF)
                {
                    string str = Util.FormatValue(datatree, ret, false);
                    Message("Queried:" + Environment.NewLine + str + Environment.NewLine);
                    if (status == DatatreeDotNet.dt_query_status_t.DTQ_GOT_NOREF)
                    {
                        DatatreeDotNet.dt_destroy_value(datatree, ret);
                    }

                    if (!txtCmd.Items.Contains(txtCmd.Text))
                        txtCmd.Items.Add(txtCmd.Text);
                }

                txtCmd.Text = string.Empty;
            }
        }

        #endregion
    }
}
