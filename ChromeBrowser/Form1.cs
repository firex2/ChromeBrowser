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


//public class RenderProcessMessageHandler : IRenderProcessMessageHandler
//{
//    // Wait for the underlying `Javascript Context` to be created, this is only called for the main frame.
//    // If the page has no javascript, no context will be created.
//    void IRenderProcessMessageHandler.OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
//    {
//        const string script = "document.addEventListener('DOMContentLoaded', function(){ alert('DomLoaded'); });";

//        frame.ExecuteJavaScriptAsync(script);
//    }
//}

namespace ChromeBrowser
{
    public partial class Form1 : Form
    {

        String searchString = "";
        public ChromiumWebBrowser chromeBrowser;
        public Form1()
        {
            InitializeComponent();
            InitializeChromium();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

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
            chromeBrowser.Load("https://www.bing.com");
            chromeBrowser.FrameLoadEnd += (sender2, args) =>
            {
                //Wait for the MainFrame to finish loading
                if (args.Frame.IsMain)
                    label1.Text = getTitle();
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chromeBrowser.Load("https://de.wikipedia.org/wiki/Spezial:Zuf%C3%A4llige_Seite");
            chromeBrowser.FrameLoadEnd += (sender2, args) =>
            {
                //Wait for the MainFrame to finish loading
                if (args.Frame.IsMain)
                    label1.Text = getTitle();
            };

        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = getTitle();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            chromeBrowser.Load("https://www.bing.com/search?q=" + searchString);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            chromeBrowser.Load("https://de.wikipedia.org/wiki/Spezial:Zuf%C3%A4llige_Seite");
            chromeBrowser.FrameLoadEnd += (sender2, args) =>
            {
                //Wait for the MainFrame to finish loading
                if (args.Frame.IsMain)
                {
                    label1.Text = getTitle();
                    System.Threading.Thread.Sleep(1000);
                    chromeBrowser.Load("https://www.bing.com/search?q=" + searchString);
                }
            };
        }

        private string getTitle()
        {
            string script = "document.title.replace(\" – Wikipedia\", \"\")";
            var task = chromeBrowser.EvaluateScriptAsync(script);
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    if (response.Success && response.Result != null)
                        LoadBing(response.Result.ToString());
                }
            });
            return "";
        }

        Pages PageLoading = Pages.NONE;
        private void button6_Click(object sender, EventArgs e)
        {
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
                    Thread.Sleep(1000);
                    LoadWiki();
                    break;
                case Pages.WIKI:
                    getTitle();
                    break;
                default:
                    break;
            }
        }
    }
    
    public enum Pages
    {
        NONE,
        BING,
        WIKI,
    }
}
