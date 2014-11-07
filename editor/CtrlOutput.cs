using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DataTreeEditor
{
    public class CtrlOutput : RichTextBox
    {
        private Dictionary<KeyValuePair<int, int>, Color> colorInfo = new Dictionary<KeyValuePair<int, int>, Color>();

        public void Append(string txt, Color color)
        {
            txt = txt.Replace("\r", string.Empty) + "\n";
            AppendText(txt);
            colorInfo[new KeyValuePair<int, int>(Text.Length - txt.Length, txt.Length)] = color;

            ColorText();
        }

        public new void Clear()
        {
            Text = string.Empty;
            colorInfo.Clear();
        }

        private void ColorText()
        {
            foreach (var info in colorInfo)
            {
                Select(info.Key.Key, info.Key.Value);
                SelectionColor = info.Value;
            }

            Select(Text.Length, 0);
            try
            {
                ScrollToCaret();
            }
            catch { }
        }
    }
}
