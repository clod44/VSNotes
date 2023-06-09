using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSNotes
{
    public partial class Form1 : Form
    {
        string currentFilePath = null;
        bool changesExist = false;
        public Form1()
        {
            InitializeComponent();
            changeCurrentFilePath(null);
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
                if (result == DialogResult.Yes) { 
                    richTextBox_notes.Text = "";
                    setChangesExist(false);
                    changeCurrentFilePath(null);
                }
            }
            else
            {
                richTextBox_notes.Text = "";
                changeCurrentFilePath(null);
                setChangesExist(false);
            }

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
            if(tryToExit()) Application.Exit();
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
    }
}
