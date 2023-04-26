// Decompiled with JetBrains decompiler
// Type: TadahLauncher.Form1
// Assembly: TadahLauncher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3111CEF3-12D2-4E80-9B4F-93665981ABEB
// Assembly location: C:\Users\Case\AppData\Local\Tadah\2014\TadahLauncher.exe

using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TadahLauncher
{
  public class Form1 : Form
  {
    private static string baseUrl = "http://www.tadah.rocks";
    private static string version = "1.0.2";
    private static string client = "2014";
    private static string installPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Tadah\\" + Form1.client;
    private static bool doSha256Check = false;
    private IContainer components;
    private Label label1;
    private ProgressBar progressBar1;
    private PictureBox pictureBox1;

    public Form1()
    {
      this.InitializeComponent();
      this.Text = "Tadah " + Form1.client + " " + Form1.version;
    }

    private void UpdateClient(bool doInstall)
    {
      this.label1.Text = "Getting the latest Tadah...";
      string str1;
      try
      {
        str1 = new WebClient().DownloadString(Form1.baseUrl + "/client/" + Form1.client);
      }
      catch (Exception ex)
      {
        this.label1.Text = "Error: Can't connect.";
        int num = (int) MessageBox.Show("Could not connect to Tadah.\n\nCurrent baseUrl: " + Form1.baseUrl + "\nLauncher version: " + Form1.version + "\n\nIf this persists, please create a ticket in the Discord server if you do not know what to do.\n\nError message: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        this.Close();
        return;
      }
      string sha256 = "none";
      try
      {
        JObject jobject = JObject.Parse(str1);
        JToken.op_Explicit(jobject["version"]);
        JToken.op_Explicit(jobject["url"]);
        sha256 = JToken.op_Explicit(jobject["sha256"]);
      }
      catch
      {
        int num = (int) MessageBox.Show("Could not parse client info JSON data. This usually occurs if the webserver gives an invalid response. Contact the developers if this issue persists.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        this.Close();
        return;
      }
      if (sha256 == "none")
      {
        int num = (int) MessageBox.Show("The client exists on the webserver, but it is not downloadable anymore. If you believe this is an error, contact the developers. (sha256 is \"none\").", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        this.Close();
      }
      else if (doInstall)
      {
        this.label1.Text = "Downloading latest client...";
        WebClient webClient = new WebClient();
        string tempZipArchivePath = Path.GetTempPath() + "Tadah" + Form1.client + ".zip";
        webClient.DownloadProgressChanged += (DownloadProgressChangedEventHandler) ((s, e) => this.progressBar1.Value = e.ProgressPercentage);
        webClient.DownloadFileCompleted += (AsyncCompletedEventHandler) ((s, e) =>
        {
          if (Form1.doSha256Check)
          {
            this.label1.Text = "Verifying downloaded files...";
            byte[] hash;
            using (FileStream inputStream = System.IO.File.OpenRead(tempZipArchivePath))
              hash = SHA256.Create().ComputeHash((Stream) inputStream);
            string str2 = "";
            foreach (byte num in hash)
              str2 += num.ToString("x2");
            if (str2 != sha256)
            {
              int num = (int) MessageBox.Show("SHA256 mismatch.\nWebsite reported: " + sha256 + "\nLocal file: " + str2, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
              this.Close();
              return;
            }
          }
          this.label1.Text = "Extracting files...";
          this.progressBar1.Value = 50;
          try
          {
            if (Directory.Exists(Form1.installPath))
              Directory.Delete(Form1.installPath, true);
            ZipFile.ExtractToDirectory(tempZipArchivePath, Form1.installPath);
          }
          catch (Exception ex)
          {
            int num = (int) MessageBox.Show("Error occurred while attempting to extract the client to its proper directory. (" + ex.Message + ") \n\n" + Form1.installPath, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            this.Close();
            return;
          }
          this.label1.Text = "Setting up URI...";
          try
          {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Classes", true);
            RegistryKey subKey1 = registryKey.CreateSubKey("tadahfourteen");
            subKey1.CreateSubKey("DefaultIcon").SetValue("", (object) (Form1.installPath + "\\TadahLauncher.exe,1"));
            subKey1.SetValue("", (object) "tadahfourteen:Protocol");
            subKey1.SetValue("URL Protocol", (object) "");
            subKey1.CreateSubKey("shell\\open\\command").SetValue("", (object) (Form1.installPath + "\\TadahLauncher.exe -token %1"));
            subKey1.Close();
            RegistryKey subKey2 = registryKey.CreateSubKey("hosttadahfourteen");
            subKey2.CreateSubKey("DefaultIcon").SetValue("", (object) (Form1.installPath + "\\TadahLauncher.exe,1"));
            subKey2.SetValue("", (object) "hosttadahfourteen:Protocol");
            subKey2.SetValue("URL Protocol", (object) "");
            subKey2.CreateSubKey("shell\\open\\command").SetValue("", (object) (Form1.installPath + "\\TadahLauncher.exe -host %1"));
            subKey2.Close();
          }
          catch (Exception ex)
          {
            int num = (int) MessageBox.Show("Could not set up Tadah's URI. This usually happens on machines running Windows 7 or older. Tadah may not launch at all. (" + ex.Message + ")", "URI Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          }
          this.label1.Text = "Clean up...";
          if (Directory.Exists("C:\\Tadah"))
          {
            if (MessageBox.Show("Old Tadah installation folder detected. Would you like to delete the old Tadah installation folder (found at C:\\Tadah)?\n\nPlease make sure you aren't leaving anything important behind, because that data cannot be restored when you confirm you press \"Yes.\"", "Clean Up", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
              try
              {
                Directory.Delete("C:\\Tadah", true);
                int num = (int) MessageBox.Show("Old Tadah folder deleted.", "Clean Up", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
              }
              catch
              {
                int num = (int) MessageBox.Show("Could not delete the entirety of the old installation folder. There may be files remaining, so please go clean those up yourself.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
              }
            }
          }
          if (System.IO.File.Exists(tempZipArchivePath))
            System.IO.File.Delete(tempZipArchivePath);
          int num1 = (int) MessageBox.Show("Tadah successfully updated. Go ahead and play.", "Update Complete", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
          this.Close();
        });
        try
        {
          webClient.DownloadFileAsync(new Uri(Form1.baseUrl + "/client/download/" + Form1.client), tempZipArchivePath);
        }
        catch
        {
          int num = (int) MessageBox.Show("Could not get latest client files from the website.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          this.Close();
        }
      }
      else
      {
        string location = Assembly.GetExecutingAssembly().Location;
        string str3 = Path.GetTempPath() + "TadahLauncher" + Form1.client + ".exe";
        string destFileName = str3;
        System.IO.File.Copy(location, destFileName, true);
        new Process()
        {
          StartInfo = {
            FileName = str3,
            Arguments = "-update"
          }
        }.Start();
        this.Close();
      }
    }

    private async void Form1_Shown(object sender, EventArgs e)
    {
      Form1 form1 = this;
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      if (commandLineArgs.Length < 2)
        form1.UpdateClient(false);
      else if (commandLineArgs[1] == "-host")
      {
        form1.label1.Text = "Launching server Tadah " + Form1.version + "...";
        new Process()
        {
          StartInfo = {
            FileName = (Form1.installPath + "\\TadahPlayerBeta.exe"),
            Arguments = ("--play -a none -t 0 -j \"" + Form1.baseUrl + "/game/newhost/" + commandLineArgs[2].Split(':')[1] + "\"")
          }
        }.Start();
        form1.progressBar1.Value = 100;
        await Task.Delay(5000);
        form1.Close();
      }
      else if (commandLineArgs[1] == "-update")
        form1.UpdateClient(true);
      else if (commandLineArgs[1] != "-token")
        form1.UpdateClient(false);
      else if (!System.IO.File.Exists(Form1.installPath + "\\TadahPlayerBeta.exe"))
      {
        int num = (int) MessageBox.Show("The client couldn't be found, so we are going to install it.\nInstall Path: " + Form1.installPath, "Client Missing", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        form1.UpdateClient(false);
      }
      else
      {
        form1.label1.Text = "Checking version...";
        form1.progressBar1.Value = 50;
        string str1;
        try
        {
          str1 = new WebClient().DownloadString(Form1.baseUrl + "/client/" + Form1.client);
        }
        catch (Exception ex)
        {
          form1.label1.Text = "Error: Can't connect.";
          int num = (int) MessageBox.Show("Could not connect to Tadah.\n\nCurrent baseUrl: " + Form1.baseUrl + "\nLauncher version: " + Form1.version + "\n\nIf this persists, please create a ticket in the Discord server if you do not know what to do.\n\nError message: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          form1.Close();
          return;
        }
        string str2;
        try
        {
          JObject jobject = JObject.Parse(str1);
          str2 = JToken.op_Explicit(jobject["version"]);
          JToken.op_Explicit(jobject["url"]);
          JToken.op_Explicit(jobject["sha256"]);
        }
        catch
        {
          int num = (int) MessageBox.Show("Could not parse client info JSON data. This usually occurs if the webserver gives an invalid response. Contact the developers if this issue persists.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          form1.Close();
          return;
        }
        if (str2 == Form1.version)
        {
          form1.label1.Text = "Launching Tadah " + Form1.version + "...";
          new Process()
          {
            StartInfo = {
              FileName = (Form1.installPath + "\\TadahPlayerBeta.exe"),
              Arguments = ("--play -a none -t 0 -j \"" + Form1.baseUrl + "/game/newjoin/" + commandLineArgs[2].Split(':')[1] + "\"")
            }
          }.Start();
          form1.progressBar1.Value = 100;
          await Task.Delay(5000);
          form1.Close();
        }
        else
        {
          form1.label1.Text = "New version found, updating: " + Form1.version + " -> " + str2;
          await Task.Delay(5000);
          form1.UpdateClient(false);
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (Form1));
      this.label1 = new Label();
      this.progressBar1 = new ProgressBar();
      this.pictureBox1 = new PictureBox();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Microsoft Sans Serif", 9f);
      this.label1.Location = new Point(57, 21);
      this.label1.Name = "label1";
      this.label1.Size = new Size(79, 15);
      this.label1.TabIndex = 0;
      this.label1.Text = "Please wait...";
      this.progressBar1.Location = new Point(60, 48);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new Size(267, 29);
      this.progressBar1.TabIndex = 1;
      this.pictureBox1.Image = (Image) componentResourceManager.GetObject("pictureBox1.Image");
      this.pictureBox1.Location = new Point(16, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(35, 35);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(344, 111);
      this.ControlBox = false;
      this.Controls.Add((Control) this.pictureBox1);
      this.Controls.Add((Control) this.progressBar1);
      this.Controls.Add((Control) this.label1);
      this.Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
      this.MaximizeBox = false;
      this.MaximumSize = new Size(360, 150);
      this.MinimumSize = new Size(360, 150);
      this.Name = nameof (Form1);
      this.Text = "Tadah";
      this.Shown += new EventHandler(this.Form1_Shown);
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
