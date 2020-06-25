using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResGen
{
    public partial class frmMain : Form
    {
        private static string vsDevCommandPromptPath = string.Empty;
        public frmMain()
        {
            InitializeComponent();

            lblMessage.Text = string.Empty;
            vsDevCommandPromptPath = GetVSDeveloperCommandPromptPath();
            if (string.IsNullOrEmpty(vsDevCommandPromptPath))
                WriteLog("Visual Studio is required to install!", MessageType.ERROR);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            lblMessage.Text = string.Empty;
            folderBrowserDialogFileLoc.ShowDialog();

            txtFileLoc.Text = folderBrowserDialogFileLoc.SelectedPath;

            if (IsValidLocation(txtFileLoc.Text))
            {
                btnBrowse.Enabled = false;
                btnStart.Enabled = true;
            }
            else
                WriteLog(string.Format("'{0}' does not contain any *.resx file.", txtFileLoc.Text), MessageType.ERROR);
        }

        private bool IsValidLocation(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return false;

            if (Directory.GetFiles(folderPath, "*.resx").Length == 0)
                return false;

            return true;
        }

        private void WriteLog(string msg, MessageType type)
        {
            lblMessage.Text = msg;
            lblMessage.Enabled = true;

            switch (type)
            {
                case MessageType.INFO:
                    lblMessage.ForeColor = Color.Black;
                    break;
                case MessageType.ERROR:
                    lblMessage.ForeColor = Color.Red;
                    break;
                case MessageType.SUCCEED:
                    lblMessage.ForeColor = Color.Green;
                    break;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            List<string> files = GetAllResx(txtFileLoc.Text);
            var proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = @"C:\Windows\System32";
            proc.FileName = @"C:\Windows\System32\cmd.exe";
            proc.Arguments = string.Format(@"%comspec% /k ""{0}""", vsDevCommandPromptPath);
            proc.Arguments += @" && cd /d " + txtFileLoc.Text;

            foreach (var file in files)
            {
                proc.Arguments += " && ResGen " + file;
            }

            proc.WindowStyle = ProcessWindowStyle.Normal;
            Process.Start(proc);
            WriteLog("Done!", MessageType.SUCCEED);
        }

        private List<string> GetAllResx(string path)
        {
            List<string> files = new List<string>();

            foreach (var item in Directory.GetFiles(path, "*.resx"))
            {
                files.Add(Path.GetFileName(item));
            }

            return files;
        }

        private enum MessageType
        {
            INFO = 1,
            ERROR = 2,
            SUCCEED = 3
        }

        private string GetVSDeveloperCommandPromptPath()
        {
            string vsPath = @"C:\Program Files (x86)\Microsoft Visual Studio";

            if (!Directory.Exists(vsPath))
            {
                return string.Empty;
            }

            List<string> files = Directory.EnumerateFiles(vsPath, "VsDevCmd.bat", SearchOption.AllDirectories).ToList();

            if (files.Count == 0)
                return string.Empty;

            return files.Last<string>();
        }
    }
}

