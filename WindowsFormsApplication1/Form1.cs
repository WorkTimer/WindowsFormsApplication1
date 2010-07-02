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
        private const String m_StartAddress = "http://dict.cn/bdc/";
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != this.webBrowser1.Url) return;
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
            WebBrowser web = (WebBrowser)sender;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(web.DocumentStream);
            HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"main_left\"]/div[h3=\"在线背单词\"]/ul[1]/li/a");

            if (m_context == null) m_context = new DictCnEntities();
            try
            {
                int i = 0;
                foreach (HtmlNode htmlNode in htmlNodes)
                {
                    string my名称 = htmlNode.InnerText;
                    string my地址 = htmlNode.Attributes["href"].Value;
                    分类 my分类 = (from o in m_context.分类集 where o.地址 == my地址 && o.名称 == my名称 select o).FirstOrDefault();
                    if (my分类 == null)
                    {
                        my分类 = new 分类
                        {
                            名称 = my名称,
                            地址 = my地址
                        };
                        m_context.分类集.AddObject(my分类);
                    }
                }
                
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
                if (my扫描指针 == null)
                    m_context.扫描指针集.AddObject(new 扫描指针 { ID = 1, 扫描日期 = DateTime.Now, 扫描地址 = "", 扫描类型 = "分类", 当前ID = 1 });
                else
                {
                    my扫描指针.ID = 1;
                    my扫描指针.扫描日期 = DateTime.Now;
                    my扫描指针.扫描地址 = "";
                    my扫描指针.扫描类型 = "分类";
                    my扫描指针.当前ID = 1;
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
            //this.webBrowser1.Url = new System.Uri("http://dict.cn/bdc/", System.UriKind.Absolute);
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this.webBrowser1.Navigate(m_StartAddress);
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
        }
        private void webBrowser1_分类DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != this.webBrowser1.Url) return;
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
            WebBrowser web = (WebBrowser)sender;
        }

        private void webBrowser1_课本DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void webBrowser1_分组DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void webBrowser1_分页DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void webBrowser1_单词DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            扫描指针 my扫描指针 = m_context.扫描指针集.Single();

            switch (my扫描指针.扫描类型)
            {
                
                case "分类":
                    this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分类DocumentCompleted);
                    this.webBrowser1.Navigate(m_StartAddress + (from o in m_context.分类集 where o.ID == my扫描指针.当前ID select o).FirstOrDefault().地址);
                    break;
                case "课本":
                    break;
                case "分组":
                    break;
                case "分页":
                    break;
                case "单词":
                    break;
                default :
                     break;
            }
        }
    }
}
