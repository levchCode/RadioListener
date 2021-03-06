﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WMPLib;
using Newtonsoft.Json;

         

namespace RadioListener
{
    public partial class Form1 : Form
    {
        
        public WindowsMediaPlayer player = new WindowsMediaPlayer();
        string path = "Default.json";
        public BindingList<Station> stations = new BindingList<Station>();
        Station selected = new Station();
    
        bool paused = false;
       

        public bool Parse()
        {
            try
            {
                using (StreamReader file = File.OpenText(path))
                {
                    stations = JsonConvert.DeserializeObject<BindingList<Station>>(File.ReadAllText(path));
                }
                listBox1.DataSource = stations;
                return true;
            }
            catch (JsonReaderException)
            {
                MessageBox.Show("It seems that list of stations is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return false;
            }
        }

     

        public Form1()
        {
            InitializeComponent();
            ni.Icon = SystemIcons.Information;
            trackBar1.Minimum = 1;
            trackBar1.Maximum = 100;
            trackBar1.Value = 50;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
            //Parse();
            listBox1.DisplayMember = "Name";
           
            if (Parse())
            {
                player.URL = stations[0].URL;

                player.controls.play();
            }
        }

        

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.controls.stop();
             
            selected = (Station)listBox1.SelectedItem;
            player.URL = selected.URL;
            player.controls.play();
            label1.Text = selected.Name;
            if (selected.Thumbnail != "")
            {
                pictureBox1.ImageLocation = selected.Thumbnail;
            }

            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Value = player.network.bufferingProgress;
             
             
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ni.BalloonTipText = selected.Name;
                ni.ShowBalloonTip(10000);
                Hide();
            }
        }

        private void newStationListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewStation newS = new NewStation();
            newS.Show();
        }

        private void openStationsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            path = openFileDialog1.FileName;
            Parse();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            paused = !paused;
            if (player.playState == WMPPlayState.wmppsPlaying && !paused)
            {
                button1.Text = "Pause";
                player.controls.stop();
            }
            else
            {
                player.controls.play();
                button1.Text = "Play";
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) 
        {
            Show();
            this.WindowState = FormWindowState.Normal;
          
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            player.settings.volume = trackBar1.Value;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.Show();
        }

        private void watchIPTVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TV tv = new TV();
            player.controls.stop();
            tv.Show();
        }
    }
}
