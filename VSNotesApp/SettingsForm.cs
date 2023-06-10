using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSNotes
{
    public partial class SettingsForm : Form
    {
        ConfigFile currentConfig = new ConfigFile();

        public SettingsForm()
        {
            InitializeComponent();

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

            ImportSettingsVisuals();
        }
        private void ImportSettingsVisuals()
        {

            //fill the visuals
            numericUpDown_fontSize.Value = int.Parse(currentConfig.FontSize);
            checkBox_wordWrap.Checked = bool.Parse(currentConfig.WordWrap);
            Color textColor = Color.FromArgb(
                int.Parse(currentConfig.TextColorR),
                int.Parse(currentConfig.TextColorG),
                int.Parse(currentConfig.TextColorB)
                );
            button_textColor.BackColor = textColor;
            Color backgroundColor = Color.FromArgb(
                int.Parse(currentConfig.BackgroundColorR),
                int.Parse(currentConfig.BackgroundColorG),
                int.Parse(currentConfig.BackgroundColorB)
                );
            button_backgroundColor.BackColor = backgroundColor;
        }

        private void ExportSettingsVisuals()
        {
            currentConfig.FontSize = "" + numericUpDown_fontSize.Value;
            currentConfig.WordWrap = "" + checkBox_wordWrap.Checked;
            currentConfig.TextColorR = "" + button_textColor.BackColor.R;
            currentConfig.TextColorG = "" + button_textColor.BackColor.G;
            currentConfig.TextColorB = "" + button_textColor.BackColor.B;
            currentConfig.BackgroundColorR = "" + button_backgroundColor.BackColor.R;
            currentConfig.BackgroundColorG = "" + button_backgroundColor.BackColor.G;
            currentConfig.BackgroundColorB = "" + button_backgroundColor.BackColor.B;
            currentConfig.ExportConfig();
        }

        private void selectColor(object sender)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                Color selectedColor = colorDialog.Color;
                Button button = (Button)sender;
                button.BackColor = selectedColor;
            }
        }
        private void button_textColor_Click(object sender, EventArgs e)
        {
            selectColor(sender);
        }

        private void button_backgroundColor_Click(object sender, EventArgs e)
        {
            selectColor(sender);
        }

        private void button_saveSettings_Click(object sender, EventArgs e)
        {
            ExportSettingsVisuals();
        }

        private void button_resetConfig_Click(object sender, EventArgs e)
        {
            currentConfig = new ConfigFile();
            currentConfig.ExportConfig();
            ImportSettingsVisuals();
        }

        private void button_openConfig_Click(object sender, EventArgs e)
        { 
            try
            {
                string configFilePath = currentConfig.GetConfigFilePath();
                //this info is required to ignore dialog's opening place
                //so we can still use AppDomain.CurrentDomain.BaseDirectory
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = configFilePath,
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
