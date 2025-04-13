using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Episode_Renamer
{
    public partial class Form1 : Form
    {
        private readonly RegistryKey _rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\CLASSES\Folder\shell\Episode Renamer");
        private const string REGISTRY_PATH = @"SOFTWARE\CLASSES\Folder\shell\Episode Renamer";
        private const string REGISTRY_COMMAND_PATH = @"SOFTWARE\CLASSES\Folder\shell\Episode Renamer\command";

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
            InitializeShellIntegration();
        }

        private void InitializeShellIntegration()
        {
            bool shell = GetKey<bool>(nameof(shell));
            string folderName = null;
            if (Environment.GetCommandLineArgs().Length > 1)
                folderName = Environment.GetCommandLineArgs()[1];
            textBox1.Text = folderName;

            if (shell && _rk is null)
            {
                if (MessageBox.Show(@"This app create shell command just right click on a folder", 
                    @"Information", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CreateShellIntegration();
                }
                else
                {
                    ShellCheck(nameof(shell), false);
                }
            }
        }

        private void CreateShellIntegration()
        {
            try
            {
                Registry.CurrentUser.CreateSubKey(REGISTRY_PATH);
                var key = Registry.CurrentUser.CreateSubKey(REGISTRY_COMMAND_PATH);
                key?.SetValue("", Application.StartupPath + @"\Episode Renamer.exe " + "\"" + "%1" + "\"");
                key?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création de l'intégration shell : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation des entrées
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Veuillez remplir tous les champs", "Erreur", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var directory = new DirectoryInfo(textBox1.Text);
                if (!directory.Exists)
                {
                    MessageBox.Show("Le dossier spécifié n'existe pas", "Erreur", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Prévisualisation des changements
                var files = directory.GetFiles();
                var changes = files.Select(f => new
                {
                    OldName = f.Name,
                    NewName = f.Name.Replace(textBox3.Text, textBox2.Text)
                }).ToList();

                // Afficher la prévisualisation et demander confirmation
                var message = string.Join("\n", changes.Select(c => $"{c.OldName} -> {c.NewName}"));
                if (MessageBox.Show($"Voulez-vous effectuer les changements suivants ?\n\n{message}", 
                    "Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }

                // Effectuer les renommages
                int successCount = 0;
                foreach (var file in files)
                {
                    var newPath = Path.Combine(directory.FullName, file.Name.Replace(textBox3.Text, textBox2.Text));
                    File.Move(file.FullName, newPath);
                    successCount++;
                }

                MessageBox.Show($"{successCount} fichiers ont été renommés avec succès.", "Succès", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}", "Erreur", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TextBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Length > 0 && Directory.Exists(files[0]))
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
            try
            {
                if (_rk is null)
                {
                    if (MessageBox.Show(@"Do you want to create shell command", 
                        @"Episode Renamer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CreateShellIntegration();
                    }
                }
                else
                {
                    if (MessageBox.Show(@"Do you want to delete shell command", 
                        @"Episode Renamer", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_PATH);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la manipulation du registre : {ex.Message}", 
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}