using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;


using System.IO;
using System.Xml;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
    
namespace Notesieve
{
    public partial class MainForm : Form
    {
        private const int START_LOGO_TIME = 3000;
        public const string currentVersion = "1.1.0.00";
      
        //private const string xmlUrl = "http://notesieve.com/";
        private const string xmlUrl = "https://notesieve.000webhostapp.com/NotesieveUpdate/";
        private int currentFilesDownloaded = 0;
        private int maxFiles = 0;
        private bool isDownloadingNow = false;

        private LogoForm welocomeLogoForm;
        private Drawer viewDrawer;

        public NotifyIcon notifyIcon { get => notifyIcon1; }

        public MainForm()
        {
            InitializeComponent();
        }        

        public void HideIcon()
        {
            notifyIcon1.Visible = false;
            notifyIcon1.Icon = null;
            notifyIcon1.Dispose();
            notifyIcon1 = null;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Text = "Notesieve";
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;

            notifyIcon1.ContextMenuStrip = ContextMenu;
            notifyIcon1.ContextMenuStrip.Items[0].Click += UpdateClick;

            //Отображаем стартовое окно
            this.Hide();
            
            welocomeLogoForm = new LogoForm();
            welocomeLogoForm.ShowInTaskbar = false;
            welocomeLogoForm.Show();
            welocomeLogoForm.Refresh();
            Thread.Sleep(START_LOGO_TIME);
            welocomeLogoForm.Close();
 
            //Рисуем боковую панель
            viewDrawer = new Drawer(this);
            viewDrawer.DrawStart();
            this.Show();
            CheckUpdate(false);
        }
        private void UpdateClick(object sender, EventArgs e)
        {
            CheckUpdate(true);
        }
        private void CheckUpdate(bool isClicked)
        {
            if (isDownloadingNow)
            {
                MessageBox.Show("Обновление уже скачивается", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; 
            }
            try
            {
                XmlDocument doc = new XmlDocument();
                 doc.Load(xmlUrl + "info.xml");

                string versionText = doc.GetElementsByTagName("version")[0].InnerText;
                double versionRemote = Convert.ToDouble(versionText.Replace(".", ""));
                double thisVersion = Convert.ToDouble(currentVersion.Replace(".", ""));

                if (thisVersion < versionRemote)
                {
                    DialogResult result = MessageBox.Show("Доступна новая версия: " + versionText + "\nСкачать обновление сейчас?", "Подтвердите обновления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else 
                    {

                    }
                }
                else
                {
                    if (isClicked) MessageBox.Show("Нет доступных обновлений, увас последняя версия");
                }
            }
            catch (WebException e)
            {
                if (isClicked) MessageBox.Show("Проблема с соединением.\n" + e.Message);
            }
            catch (Exception e)
            {
                if (isClicked) MessageBox.Show(e.Message);
            }


        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                isDownloadingNow = true;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(xmlUrl + "info.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                List<string> filesToDownload = new List<string>();

                foreach (XmlNode xnode in xRoot)
                {
                    if (xnode.Name == "fileToDownload") filesToDownload.Add(xnode.InnerText);
                }


                if (filesToDownload.Count > 0)
                {
                    currentFilesDownloaded = 0;
                    maxFiles = filesToDownload.Count;
                    
                    DownloadManyFiles(filesToDownload);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                isDownloadingNow = false;
            }
           
           
        }
        public async Task DownloadManyFiles(List<string> files)
        {
            try
            {
                string folder = Application.StartupPath + @"\" + "Updates";
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                WebClient wc = new WebClient();
                wc.DownloadFileCompleted += download_Completed;
                foreach (string file in files)
                {
                    switch(file)
                    {
                        case "Notesieve_exe":
                            await wc.DownloadFileTaskAsync(xmlUrl + file, folder + @"\" + "Notesieve.exe");
                            break;
                        default:
                            await wc.DownloadFileTaskAsync(xmlUrl + file, folder + @"\" + file);
                            break;
                    }
                }

                wc.Dispose();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void download_Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (isDownloadingNow)
            {
                currentFilesDownloaded++;
                if (currentFilesDownloaded == maxFiles)
                {
                    isDownloadingNow = false;
                    Restart();
                }
            }
        }

        private void Restart()
        {
            try
            {
                Process.Start(Application.StartupPath + @"\updaters" + @"\" + "NotesieveUpdater.exe");
                Process.GetCurrentProcess().Kill();
            }
            catch
            {

            }
        }
    }
}
