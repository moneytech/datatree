using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using dt;

namespace DataTreeEditor
{
    public enum EditStates
    {
        Closed,
        New,
        OpenedNotSaved,
        Saved
    }

    public struct ValueInfo
    {
        public IntPtr Value;
        public bool Readonly;
        public bool WithChild;
        public ValueInfo(IntPtr _val, bool _readonly, bool _withChild)
        {
            Value = _val;
            Readonly = _readonly;
            WithChild = _withChild;
        }
    }

    public static class Util
    {
        public static string Input(string prompt, string def)
        {
            return Interaction.InputBox(prompt, "Datatree", def, -1, -1);
        }

        public static DialogResult AskYesNoCancel(IWin32Window wnd, string q)
        {
            return MessageBox.Show(wnd, q, "Datatree", MessageBoxButtons.YesNoCancel);
        }

        public static string FormatDatatree(IntPtr dt, bool compact)
        {
            return FormatValue(dt, DatatreeDotNet.dt_root_value(dt), compact);
        }

        public static string FormatValue(IntPtr dt, IntPtr v, bool compact)
        {
            IntPtr s = IntPtr.Zero;
            DatatreeDotNet.dt_format_value
            (
                dt,
                v,
                ref s,
                compact ? DatatreeDotNet.dt_bool_t.DT_TRUE : DatatreeDotNet.dt_bool_t.DT_FALSE
            );
            string str = Marshal.PtrToStringAnsi(s);
            DatatreeDotNet.dt_free(ref s);

            return str;
        }
    }
}
