using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace Episode_Renamer
{
    public partial class Form1 : Form
    {
        private readonly RegistryKey _rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");

        private static T GetKey<T>(string key)
        {
            return (T)Properties.Settings.Default[key];
        }

        private static void ShellCheck(string key, bool o)
        {
            Properties.Settings.Default[key] = o;
            Properties.Settings.Default.Save();
        }

        public Form1()
        {
            InitializeComponent();
            bool shell = GetKey<bool>(nameof(shell));
            string folderName = null;
            if (Environment.GetCommandLineArgs().Length > 1)
                folderName = Environment.GetCommandLineArgs()[1];
            textBox1.Text = folderName;
            if (shell is true)
            {
                if (_rk is null)
                {
                    if (MessageBox.Show(@"This app create shell command just right click on a folder", @"information", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Registry.LocalMachine.CreateSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");
                        var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer\command");
                        key?.SetValue("", Application.StartupPath + @"\Episode Renamer.exe " + "\"" + "%1" + "\"");
                        key?.Close();
                    }
                    else
                    {
                        ShellCheck(nameof(shell), false);
                    }
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var d = new DirectoryInfo(textBox1.Text);
            var infos = d.GetFiles();
            foreach (var f in infos)
            {
                File.Move(f.FullName, f.FullName.Replace(textBox3.Text, textBox2.Text));
            }
        }

        private void TextBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (Directory.Exists(files?[0]))
            {
                textBox1.Text = files[0];
            }
        }

        private void TextBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Do you want to delete shell command", @"Episode Renamer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Registry.LocalMachine.DeleteSubKeyTree(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");
            }
        }
    }
}