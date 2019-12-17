using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace Episode_Renamer
{
    public partial class Form1 : Form
    {
        private RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");
        private RegistryKey rk1 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer\command");
        private RegistryKey key;

        public Form1()
        {
            InitializeComponent();
            string folderName = null;
            if (Environment.GetCommandLineArgs().Length > 1)
                folderName = Environment.GetCommandLineArgs()[1];
            if (rk != null) { }
            else
            {
                key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");
            }
            if (rk1 != null) { }
            else
            {
                key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer\command");
                key.SetValue("", Application.StartupPath + @"\Episode Renamer.exe " + "\"" + "%1" + "\"");
                key.Close();
                MessageBox.Show("This app create shell command just right click on a folder", "information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            textBox1.Text = folderName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(textBox1.Text);
            FileInfo[] infos = d.GetFiles();
            foreach (FileInfo f in infos)
            {
                File.Move(f.FullName, f.FullName.Replace(textBox3.Text, textBox2.Text));
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (Directory.Exists(files[0]))
                {
                    textBox1.Text = files[0];
                }
            }
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            { e.Effect = DragDropEffects.Copy; }
            else
            { e.Effect = DragDropEffects.None; }
        }
    }
}