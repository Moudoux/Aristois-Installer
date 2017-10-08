using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aristois_Installer
{
    public partial class Form1 : Form
    {

        private string installer = System.IO.Directory.GetCurrentDirectory() + "\\Installer.jar";
        private string java = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Moudoux/Aristois-Installer");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                lab_status.Text = "Status: Token cannot be empty.";
                return;
            }
            lab_status.Text = "Status: Verifying account...";
            string token = textBox1.Text;
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.Headers.Add("token", token);
            string downloadString = "";
            try
            {
                downloadString = client.DownloadString("https://mc-oauth.net/api/api?token");
            } catch (WebException ex)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    downloadString = reader.ReadToEnd(); 
                }
            }
            Console.WriteLine(downloadString);
            dynamic json = JsonConvert.DeserializeObject(downloadString);
            if (json.status == "success")
            {
                installClient(token);
            } else
            {
                lab_status.Text = "Status: Failed -> Invalid token";
            }
        }

        private void installClient(string token)
        {
            lab_status.Text = "Status: Downloading client...";
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.DownloadFileAsync(new System.Uri("https://aristois.opentexon.com/download?update_oauth_code_v3=" + token), installer);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler((object sender, AsyncCompletedEventArgs e) =>
            {
                long length = new System.IO.FileInfo(installer).Length;
                if (length > 5000)
                {
                    lab_status.Text = "Status: Installing client...";
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processInfo.FileName = "cmd.exe";
                    processInfo.Arguments = "/c START \"" + java + "\" \"" + installer + "\"";
                    Process.Start(processInfo);
                } else
                {
                    lab_status.Text = "Status: Failed -> Not a donor account.";
                    File.Delete(installer);
                }
            });
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
            foreach (string file in Directory.GetDirectories("C:\\Program Files (x86)\\Minecraft\\runtime\\jre-x64"))
            {
                java = file + "\\bin\\java.exe";
                break;
            }
        }
    }
}
