using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DataTreeEditor
{
    public static class Program
    {
        public static void RelateFileType(string fileExt, string fileTypeName, string fileTypeDesc, string icoIndex)
        {
            string keyName = fileTypeName;
            string keyValue = fileTypeDesc;
            RegistryKey isExCommand = null;
            bool isCreateRetistry = true;
            try
            {
                isExCommand = Registry.ClassesRoot.OpenSubKey(keyName);
                if (isExCommand == null)
                {
                    isCreateRetistry = true;
                }
                else
                {
                    if (isExCommand.GetValue("Create").ToString() == Application.ExecutablePath)
                    {
                        isCreateRetistry = false;
                    }
                    else
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree(keyName);
                        isCreateRetistry = true;
                    }
                }
            }
            catch
            {
                isCreateRetistry = true;
            }

            if (isCreateRetistry)
            {
                try
                {
                    RegistryKey key = Registry.ClassesRoot.CreateSubKey(keyName);
                    key.SetValue("Create", Application.ExecutablePath);
                    RegistryKey keyIcon = key.CreateSubKey("DefaultIcon");
                    keyIcon.SetValue("", Application.ExecutablePath + "," + icoIndex);
                    key.SetValue("", keyValue);
                    key = key.CreateSubKey("Shell");
                    key = key.CreateSubKey("Open");
                    key = key.CreateSubKey("Command");
                    key.SetValue("", "\"" + Application.ExecutablePath + "\" \"%1\"");
                    keyName = fileExt;
                    keyValue = fileTypeName;
                    key = Registry.ClassesRoot.CreateSubKey(keyName);
                    key.SetValue("", keyValue);
                }
                catch
                {
                }
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                RelateFileType(".dt", "Datatree file", "Datatree file", "0");

                Application.Run(new FormMain());
            }
            else
            {
                if (args.Length == 1 && args.First().ToLower() == "-e")
                {
                    Application.Run(new FormMain());
                }
                else
                {
                    string file = string.Empty;
                    foreach (string arg in args)
                        file += arg + " ";
                    Application.Run(new FormMain(file));
                }
            }
        }
    }
}
