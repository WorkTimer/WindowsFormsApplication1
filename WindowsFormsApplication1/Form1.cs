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

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != this.webBrowser1.Url) return;
            
            WebBrowser web = (WebBrowser)sender;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(web.DocumentStream);
            //HtmlElementCollection htmlelecol = web.Document.GetElementsByTagName("ul");
            //htmlelecol[4].
            HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes(@"/html[1]/body[1]/div[3]/div[2]/div[1]/ul[1]//@href");
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
            using (DictCnEntities context  = new DictCnEntities())
            {
                try
                {
                    int i = 0;
                    foreach (HtmlNode htmlNode in htmlNodes)
                    {
                        分类 my = new 分类
                        {
                            名称 = htmlNode.InnerText,
                            地址 = htmlNode.Attributes["href"].Value
                        };
                        //分类 my = 分类.Create分类(i++, htmlNode.InnerText, htmlNode.Attributes["href"].Value);
                        context.分类集.AddObject(my);
                    }
                    i = context.SaveChanges();
                    this.dataGridView1.DataSource = context.分类集;
                }
                catch (UpdateException ex)
                {
                    //Console.WriteLine(ex.ToString());
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.toolStripTextBox1.Text = this.webBrowser1.Url.AbsolutePath;
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }
    }
}
