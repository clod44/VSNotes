using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Xml.Linq;
using IniParser;
using IniParser.Model;
using System.Drawing;
using System.Security.Policy;

namespace VSNotes
{
    public partial class Form1 : Form
    {
        string currentFilePath = null;
        bool changesExist = false;
        ConfigFile currentConfig = new ConfigFile();
        public Form1()
        {
            InitializeComponent();
            changeCurrentFilePath(null);

            //check if config files exists
            if (!File.Exists(currentConfig.GetConfigFilePath()))
            {
                //create config file
                currentConfig.ExportConfig();
            }
            else
            {
                currentConfig.ImportConfig();
            }

            ApplyConfig();
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveAs();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save();
        }

        private void openFile()
        {
            if (changesExist)
            {
                DialogResult result = MessageBox.Show("Do you want to continue? All unsaved content of current file will be lost.", "There are unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                changeCurrentFilePath(fileName);

                using (StreamReader reader = new StreamReader(fileName))
                {
                    richTextBox_notes.Text = reader.ReadToEnd();
                }
                setChangesExist(false);
            }
        }

        private void saveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.DefaultExt = "txt";

            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.InitialDirectory = documentsFolder;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                changeCurrentFilePath(fileName);

                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    writer.Write(richTextBox_notes.Text);
                }
                setChangesExist(false);
            }
        }

        private void save()
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                if (File.Exists(currentFilePath))
                {
                    // File exists, proceed with saving
                    using (StreamWriter writer = new StreamWriter(currentFilePath))
                    {
                        writer.Write(richTextBox_notes.Text);
                    }
                    setChangesExist(false);
                }
                else
                {
                    // File does not exist, show error message or handle accordingly
                    MessageBox.Show("The file does not exist. Save as instead.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    saveAs();
                }
            }
            else
            {
                saveAs();
            }
        }

        private void newNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (changesExist)
            {
                DialogResult result = MessageBox.Show("Do you want to Create a new note without saving this first?. all unsaved changes will be lost.", "There are unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }
            richTextBox_notes.Text = "";
            changeCurrentFilePath(null);
            setChangesExist(false);
        }

        private void toolStripButton_about_Click(object sender, EventArgs e)
        {
            MessageBox.Show("VSNotes (Very Sophisticated Notes) made by clod44 (github). 09/06/2023", "VSNotes v1.000", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void changeCurrentFilePath(string newFilePath)
        {

            this.Text = "VSNotes - " + (newFilePath == null ? "New Note" : newFilePath);
            currentFilePath = newFilePath;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tryToExit()) Application.Exit();
        }

        private void richTextBox_notes_TextChanged(object sender, EventArgs e)
        {
            setChangesExist(true);
        }

        private void setChangesExist(bool _changesExist)
        {
            changesExist = _changesExist;
            if (changesExist)
            {
                if (!this.Text.EndsWith("*"))
                {
                    this.Text += "*";
                }
            }
            else
            {
                if (this.Text.EndsWith("*"))
                {
                    this.Text = this.Text.TrimEnd('*');
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !tryToExit();
        }

        private bool tryToExit()
        {
            if (changesExist)
            {
                DialogResult result = MessageBox.Show("Do you want to exit without saving?", "There are unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void toolStripButton_settings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.FormClosed += SettingsForm_FormClosed;
            settingsForm.ShowDialog();
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            currentConfig.ImportConfig();
            ApplyConfig();
        }
        private void ApplyConfig()
        {
            richTextBox_notes.Font = new Font(richTextBox_notes.Font.FontFamily, Math.Max(1,Math.Min(300, int.Parse(currentConfig.FontSize))), richTextBox_notes.Font.Style);
            richTextBox_notes.WordWrap = bool.Parse(currentConfig.WordWrap);
            Color textColor = Color.FromArgb(
                int.Parse(currentConfig.TextColorR),
                int.Parse(currentConfig.TextColorG),
                int.Parse(currentConfig.TextColorB)
                );
            richTextBox_notes.ForeColor = textColor;
            Color backgroundColor = Color.FromArgb(
                int.Parse(currentConfig.BackgroundColorR),
                int.Parse(currentConfig.BackgroundColorG),
                int.Parse(currentConfig.BackgroundColorB)
                );
            richTextBox_notes.BackColor = backgroundColor;
        }
    }
}





class ConfigFile
{
    public string FontSize { get; set; } = "10";
    public string TextColorR { get; set; } = "0";
    public string TextColorG { get; set; } = "0";
    public string TextColorB { get; set; } = "0";
    public string BackgroundColorR { get; set; } = "255";
    public string BackgroundColorG { get; set; } = "255";
    public string BackgroundColorB { get; set; } = "255";
    public string WordWrap { get; set; } = "true";

    public void ExportConfig()
    {
        var parser = new FileIniDataParser();
        IniData data = new IniData();

        // Populate the data object with your configuration values
        data["General"]["FontSize"] = FontSize;
        data["General"]["WordWrap"] = WordWrap;
        data["TextColor"]["R"] = TextColorR;
        data["TextColor"]["G"] = TextColorG;
        data["TextColor"]["B"] = TextColorB;
        data["BackgroundColor"]["R"] = BackgroundColorR;
        data["BackgroundColor"]["G"] = BackgroundColorG;
        data["BackgroundColor"]["B"] = BackgroundColorB;

        // Write the data to the INI file
        parser.WriteFile(GetConfigFilePath(), data);
    }

    public void ImportConfig()
    {
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(GetConfigFilePath());

        // Read the values from the INI data and assign them to the properties
        FontSize = data["General"]["FontSize"];
        WordWrap = data["General"]["WordWrap"];
        TextColorR = data["TextColor"]["R"];
        TextColorG = data["TextColor"]["G"];
        TextColorB = data["TextColor"]["B"];
        BackgroundColorR = data["BackgroundColor"]["R"];
        BackgroundColorG = data["BackgroundColor"]["G"];
        BackgroundColorB = data["BackgroundColor"]["B"];
    }

    public string GetConfigFilePath()
    {
        string baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appDirectory = Path.Combine(baseDirectory, "VSNotes");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(appDirectory))
        {
            Directory.CreateDirectory(appDirectory);
        }

        return Path.Combine(appDirectory, "config.ini");
    }
}






