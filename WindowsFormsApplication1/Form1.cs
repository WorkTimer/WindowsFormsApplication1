using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private DictCnEntities context = null;
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != this.webBrowser1.Url) return;
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
            WebBrowser web = (WebBrowser)sender;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(web.DocumentStream);
            HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"main_left\"]/div[h3=\"在线背单词\"]/ul[1]/li/a");
            //var numQuery = from htmlNode in htmlNodes
            //               where htmlNode.InnerText != null
            //               select htmlNode;
            // new DictCnEntities
            //foreach (HtmlNode htmlNode in htmlNodes)
            //{
            //    //htmlNode.InnerText
            //    int i = 0;
            //}
            //this.dictCnEntitiesBindingSource
            if (context == null) context = new DictCnEntities();

            try
            {
                int i = 0;
                foreach (HtmlNode htmlNode in htmlNodes)
                {
                    分类 my = new 分类
                    {
                        名称 = htmlNode.InnerText,
                        地址 = htmlNode.Attributes["href"].Value,
                    };
                    //分类 my = 分类.Create分类(i++, htmlNode.InnerText, htmlNode.Attributes["href"].Value);
                    context.分类集.AddObject(my);
                }
                i = context.SaveChanges();
                this.dataGridView1.DataSource = context.分类集;
            }
            catch (UpdateException ex)
            {
                this.toolStripStatusLabel1.Text = ex.Message;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //this.toolStripTextBox1.Text = this.webBrowser1.Url.AbsolutePath;
            //this.webBrowser1.Url = new System.Uri(this.toolStripTextBox1.Text, System.UriKind.Absolute);
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }

        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            //this.dataGridView1.DataSourceChanged += new System.EventHandler(this.dataGridView1_DataSourceChanged);
            this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            //context.分类集
            //this.webBrowser1.Navigate("");
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
        }
    }
}
