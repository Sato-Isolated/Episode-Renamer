using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace Episode_Renamer
{
    public partial class Form1 : Form
    {
        private readonly RegistryKey _rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");
        private readonly RegistryKey _rk1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer\command");
        private readonly RegistryKey _key;

        public Form1()
        {
            InitializeComponent();
            string folderName = null;
            if (Environment.GetCommandLineArgs().Length > 1)
                folderName = Environment.GetCommandLineArgs()[1];
            if (_rk != null) { }
            else
            {
                _key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");
            }
            if (_rk1 != null) { }
            else
            {
                _key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer\command");
                _key.SetValue("", Application.StartupPath + @"\Episode Renamer.exe " + "\"" + "%1" + "\"");
                _key.Close();
                MessageBox.Show(@"This app create shell command just right click on a folder", @"information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            textBox1.Text = folderName;
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
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (Directory.Exists(files[0]))
            {
                textBox1.Text = files[0];
            }
        }

        private void TextBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }
    }
}