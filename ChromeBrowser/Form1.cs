using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.Handler;
using CefSharp.SchemeHandler;
using CefSharp.Event;
using System.Threading;

namespace ChromeBrowser
{
    public partial class Form1 : Form
    {
        
        public ChromiumWebBrowser chromeBrowser;
        public Form1()
        {
            InitializeComponent();
            InitializeChromium();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        int counter = 0;
        private void InitializeChromium()
        {
            CefSettings settings = new CefSettings();
            settings.BrowserSubprocessPath = @"x86\CefSharp.BrowserSubprocess.exe";

            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

            chromeBrowser = new ChromiumWebBrowser("https://www.bing.com");
            //chromeBrowser.RenderProcessMessageHandler = new RenderProcessMessageHandler();


            this.Controls.Add(chromeBrowser);

            chromeBrowser.Dock = DockStyle.Bottom;
            chromeBrowser.Size = new Size(0, 510);
            chromeBrowser.FrameLoadEnd += ChromeBrowser_FrameLoadEnd;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadBing("");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadWiki();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadBing(label1.Text);
        }

        private void getTitle()
        {
            string script = "document.title.replace(\" – Wikipedia\", \"\")";
            var task = chromeBrowser.EvaluateScriptAsync(script);
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    if (response.Success && response.Result != null)
                    {
                        Invoke((Action)(() => label1.Text = response.Result.ToString()));
                        if(0 != counter)
                            LoadBing(response.Result.ToString());
                    }
                }
            });
        }

        Pages PageLoading = Pages.NONE;
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                var result = Convert.ToInt32(textBox1.Text);
                counter = result;
            }
            catch (Exception)
            {
                textBox1.Text = "1";
                return;
            }
            LoadWiki();
        }

        private void LoadWiki()
        {
            LoadPage("https://de.wikipedia.org/wiki/Spezial:Zuf%C3%A4llige_Seite", Pages.WIKI);
        }

        private void LoadBing(string search)
        {
            LoadPage("https://www.bing.com/search?q=" + search, Pages.BING);
        }

        private void LoadPage(string url, Pages page)
        {
            PageLoading = page;
            chromeBrowser.Load(url);
        }

        private void ChromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (!e.Frame.IsMain)
                return;
            switch (PageLoading)
            {
                case Pages.NONE:
                    break;
                case Pages.BING:
                    if (0 == counter)
                        break;
                    counter--;
                    Invoke((Action)(()=>textBox1.Text = counter.ToString()));
                    Thread.Sleep(RandomNumber(5000, 15000));
                    LoadWiki();
                    break;
                case Pages.WIKI:
                    getTitle();
                    break;
                default:
                    break;
            }
        }

        private int RandomNumber(int min, int max)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int rnd = random.Next(min, max);
            Invoke((Action)(() => label2.Text = rnd.ToString()));
            return rnd;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }


    public enum Pages
    {
        NONE,
        BING,
        WIKI,
    }
}
