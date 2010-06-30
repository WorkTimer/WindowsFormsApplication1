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
        private DictCnEntities m_context = null;
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
            if (m_context == null) m_context = new DictCnEntities();

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
                    m_context.分类集.AddObject(my);
                }
                
                //var query = from aaa in context.扫描指针集 where aaa.ID == 1 select aaa;//context.扫描指针集
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
                if (my扫描指针 == null)
                    m_context.扫描指针集.AddObject(new 扫描指针 { ID = 1, 扫描日期 = DateTime.Now, 扫描地址 = "http://dict.cn/bdc/" });
                else
                {
                    my扫描指针.ID = 1;
                    my扫描指针.扫描日期 = DateTime.Now;
                    my扫描指针.扫描地址 = "http://dict.cn/bdc/";
                }
                i = m_context.SaveChanges();
                this.dataGridView1.DataSource = m_context.分类集;
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

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
        }

        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            //this.dataGridView1.DataSourceChanged += new System.EventHandler(this.dataGridView1_DataSourceChanged);
            this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            //if (m_context == null) m_context = new DictCnEntities();
            扫描指针 my扫描指针 = m_context.扫描指针集.Single();
            
        }
    }
}
