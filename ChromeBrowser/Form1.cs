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
                {
                    getTitle(null, null);

                }
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chromeBrowser.Load("https://de.wikipedia.org/wiki/Spezial:Zuf%C3%A4llige_Seite");
            chromeBrowser.FrameLoadEnd += (sender2, args) =>
            {
                //Wait for the MainFrame to finish loading
                if (args.Frame.IsMain)
                {
                    getTitle(null, null);

                }
            };

        }

        private void button3_Click(object sender, EventArgs e)
        {
            getTitle(null, null);
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
                    getTitle(null, null);
                    System.Threading.Thread.Sleep(1000);
                    chromeBrowser.Load("https://www.bing.com/search?q=" + searchString);


                }
            };
        }

        private async void getTitle(object sender, EventArgs e)
        {
            string script = "document.title.replace(\" – Wikipedia\", \"\")";
            string returnValue = "";

            var task = chromeBrowser.EvaluateScriptAsync(script);
            await task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;

                    if (response.Success && response.Result != null)
                    {
                        returnValue = response.Result.ToString();
                    }
                }
            });

            this.Invoke(((Action)(() => { label1.Text = returnValue; })));
            searchString = returnValue;
        }
    }
    

}
