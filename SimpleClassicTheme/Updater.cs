﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClassicTheme
{
    public partial class Updater : Form
    {
        public Updater()
        {
            InitializeComponent();
            Text += " v" + Assembly.GetExecutingAssembly().GetName().Version;
        }

        Version ver;

        private void Updater_Load(object sender, EventArgs e)
        {
            //Get latest release info
            string f;
            using (WebClient c = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                c.Headers.Set(HttpRequestHeader.UserAgent, "SimpleClasicTheme");
                f = c.DownloadString("https://api.github.com/repos/AEAEAEAE4343/SimpleClassicTheme/releases/latest");
            }

            //Resond to messages
            Application.DoEvents(); Application.DoEvents(); Application.DoEvents();

            //Get version string
            string s = f.Substring(f.IndexOf("\"tag_name\""));
            s = s.Remove(s.IndexOf("\"target_commitish\""));
            string tagName = s.Substring(s.Remove(s.LastIndexOf('"')).LastIndexOf('"') + 1,  5);

            //Resond to messages
            Application.DoEvents(); Application.DoEvents(); Application.DoEvents();

            //Make sure we got version string
            if (tagName != "")
            {
                Version newestVersion = Version.Parse(tagName);
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                //Check if newestVersion is bigger then currentVersion
                if (currentVersion.CompareTo(newestVersion) < 0)
                {
                    label1.Text = "Downloading update " + newestVersion.ToString(3) + "...";
                    ver = newestVersion;
                    DownloadNewestVersion();
                }
                else
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }

        private void DownloadNewestVersion()
        {
            //Respond to messages
            Application.DoEvents(); Application.DoEvents(); Application.DoEvents();

            WebClient c = new WebClient();

            c.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
            {
                progressBar1.Maximum = (int)(e.TotalBytesToReceive / 1000);
                progressBar1.Value = (int)(e.BytesReceived / 1000);
            };
            c.DownloadFileCompleted += delegate
            {
                File.WriteAllText("___UPDATESCT.bat", Properties.Resources.updateString);
                Process.Start("___UPDATESCT.bat", $"{ver.ToString(3)} {Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName)} ___SCT.exe");
                Environment.Exit(0);
            };

            c.Headers.Set(HttpRequestHeader.UserAgent, "SimpleClasicTheme");
            c.DownloadFileAsync(new Uri("https://github.com/AEAEAEAE4343/SimpleClassicTheme/releases/latest/download/SimpleClassicTheme.exe"), "___SCT.exe");
        }
    }
}
