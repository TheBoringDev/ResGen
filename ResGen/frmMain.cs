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

namespace ResGen
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialogFileLoc.ShowDialog() == DialogResult.OK)
            {
                txtFileLoc.Text = folderBrowserDialogFileLoc.SelectedPath;

                if (IsValidLocation(txtFileLoc.Text))
                {
                    btnBrowse.Enabled = false;
                    btnStart.Enabled = true;
                }
                else
                    WriteLog(string.Format("'{0}' does not contain any *.resx file.", txtFileLoc.Text));
            }
        }

        private bool IsValidLocation(string folderPath)
        {
            if (Directory.GetFiles(folderPath, "*.resx").Length == 0)
                return false;

            return true;
        }

        private void WriteLog(string msg)
        {
            if (string.IsNullOrEmpty(rtbLogs.Text))
                rtbLogs.Text = msg;
            else
                rtbLogs.Text += "\n" + msg;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            List<string> files = GetAllResx(txtFileLoc.Text);
            var proc = new ProcessStartInfo();
            string anyCommand = "cd /d " + txtFileLoc.Text;
            proc.UseShellExecute = true;
            proc.WorkingDirectory = @"C:\Windows\System32";
            proc.FileName = @"C:\Windows\System32\cmd.exe";
            proc.Arguments = "/k " + anyCommand;

            foreach (var file in files)
            {
                proc.Arguments += "&&ResGen.exe " + file;
            }

            proc.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc);
            WriteLog("Done!");
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
    }
}

