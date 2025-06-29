using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace WavPlayerApp
{
    public partial class Form1 : Form
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        public Form1()
        {
            InitializeComponent();

            listBox1.DisplayMember = "Name";

            trackBarVolume.Minimum = 0;
            trackBarVolume.Maximum = 100;
            trackBarVolume.Value = 50;
            labelVolume.Text = $"Volume: {trackBarVolume.Value}%";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            LoadWavFilesFromDirectory(Path.Combine(Application.StartupPath, "Audio"));
        }

        private void LoadWavFilesFromDirectory(string audioDirectory)
        {
            listBox1.Items.Clear();

            if (Directory.Exists(audioDirectory))
            {
                var wavFiles = Directory.GetFiles(audioDirectory, "*.wav");
                var mp3Files = Directory.GetFiles(audioDirectory, "*.mp3");
                foreach (var file in wavFiles)
                {
                    listBox1.Items.Add(new FileInfo(file));
                }
                foreach (var file in mp3Files)
                {
                    listBox1.Items.Add(new FileInfo(file));
                }
            }
            else
            {
                MessageBox.Show("La cartella audio non esiste.");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is FileInfo selectedFile)
            {
                PlayWavFile(selectedFile.FullName);
            }
        }

        private void PlayWavFile(string filePath)
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();

            audioFile = new AudioFileReader(filePath);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);

            audioFile.Volume = trackBarVolume.Value / 100f;

            outputDevice.Play();
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            int volume = trackBarVolume.Value;
            labelVolume.Text = $"Volume: {volume}%";

            if (audioFile != null)
            {
                audioFile.Volume = volume / 100f;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Seleziona la cartella contenente i file WAV";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    LoadWavFilesFromDirectory(selectedPath);
                }
            }
        }
    }
}
