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
        private string m_下一个地址 = null;
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
                    string my名称 = htmlNode.InnerText.Trim();
                    string my地址 = m_StartAddress + htmlNode.Attributes["href"].Value.Trim();
                    if (my名称 == "最近更新")
                        continue;
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
                this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
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
            if (e.Url.AbsoluteUri != m_下一个地址) return;
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
            WebBrowser web = (WebBrowser)sender;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(web.DocumentStream);
            HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"bdc_catalog\"]/div[ul]/ul/li/div");
            string my分类名 = doc.DocumentNode.SelectSingleNode("//*[@id=\"bdc_catalog\"]/h4").InnerText;
            string my分类地址 = e.Url.AbsoluteUri;

            if (m_context == null) m_context = new DictCnEntities();
            try
            {
                int i = 0;
                foreach (HtmlNode htmlNode in htmlNodes)
                {
                    HtmlNodeCollection myHtmlNodes = htmlNode.SelectNodes("div");

                    string my名称 = myHtmlNodes[0].ChildNodes[2].InnerText.Trim();
                    string my地址 = m_StartAddress + myHtmlNodes[1].ChildNodes[0].Attributes["href"].Value.Trim();
                    int my单词数量 = -1;
                    string myStr单词数量 = myHtmlNodes[2].InnerText.Trim(new char[] { '共', '词' });
                    if (myStr单词数量 != "")
                    my单词数量 = Convert.ToInt32(myStr单词数量);
                    string my系列 = myHtmlNodes[0].ChildNodes[1].InnerText.Trim();
                    int my序号 = Convert.ToInt32(myHtmlNodes[0].ChildNodes[0].InnerText.Trim(new char[]{'.'}));
                    分类 my分类 = (from o in m_context.分类集 where o.地址 == my分类地址 && o.名称 == my分类名 select o).FirstOrDefault();
                    课本 my课本 = (from o in m_context.课本集 where o.名称 == my名称 && o.地址 == my地址 select o).FirstOrDefault();

                    if (my课本 == null)
                    {
                        my课本 = new 课本
                        {
                            名称 = my名称,
                            地址 = my地址,
                            分类 = my分类,
                            单词数量 = my单词数量,
                            系列 = my系列,
                            序号 = my序号
                        };
                        m_context.课本集.AddObject(my课本);
                    }
                    else
                    {
                        my课本.名称 = my名称;
                        my课本.地址 = my地址;
                        my课本.分类 = my分类;
                        my课本.单词数量 = my单词数量;
                        my课本.系列 = my系列;
                        my课本.序号 = my序号;
                    }
                }

                //扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
                //if (my扫描指针 == null)
                //    m_context.扫描指针集.AddObject(new 扫描指针 { ID = 1, 扫描日期 = DateTime.Now, 扫描地址 = "", 扫描类型 = "分类", 当前ID = 1 });
                //else
                //{
                //    my扫描指针.ID = 1;
                //    my扫描指针.扫描日期 = DateTime.Now;
                //    my扫描指针.扫描地址 = "";
                //    my扫描指针.扫描类型 = "分类";
                //    my扫描指针.当前ID = 1;
                //}
                i = m_context.SaveChanges();
                this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分类DocumentCompleted);
                if (this.dataGridView1.DataSource == m_context.课本集)
                    this.dataGridView1.DataSource = null;
                this.dataGridView1.DataSource = m_context.课本集;
            }
            catch (UpdateException ex)
            {
                this.toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void webBrowser1_课本DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != this.webBrowser1.Url) return;
            if (e.Url.AbsoluteUri != m_下一个地址) return;
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
            WebBrowser web = (WebBrowser)sender;

            //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //doc.Load(web.DocumentStream);
            //HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"bdc_catalog\"]/div[ul]/ul/li/div");
            //string my分类名 = doc.DocumentNode.SelectSingleNode("//*[@id=\"bdc_catalog\"]/h4").InnerText;
            //string my分类地址 = e.Url.AbsoluteUri;

            //if (m_context == null) m_context = new DictCnEntities();
            //try
            //{
            //    int i = 0;
            //    foreach (HtmlNode htmlNode in htmlNodes)
            //    {
            //        HtmlNodeCollection myHtmlNodes = htmlNode.SelectNodes("div");

            //        string my名称 = myHtmlNodes[0].ChildNodes[2].InnerText.Trim();
            //        string my地址 = m_StartAddress + myHtmlNodes[1].ChildNodes[0].Attributes["href"].Value.Trim();
            //        int my单词数量 = -1;
            //        string myStr单词数量 = myHtmlNodes[2].InnerText.Trim(new char[] { '共', '词' });
            //        if (myStr单词数量 != "")
            //            my单词数量 = Convert.ToInt32(myStr单词数量);
            //        string my系列 = myHtmlNodes[0].ChildNodes[1].InnerText.Trim();
            //        int my序号 = Convert.ToInt32(myHtmlNodes[0].ChildNodes[0].InnerText.Trim(new char[] { '.' }));
            //        分类 my分类 = (from o in m_context.分类集 where o.地址 == my分类地址 && o.名称 == my分类名 select o).FirstOrDefault();
            //        课本 my课本 = (from o in m_context.课本集 where o.名称 == my名称 && o.地址 == my地址 select o).FirstOrDefault();

            //        if (my课本 == null)
            //        {
            //            my课本 = new 课本
            //            {
            //                名称 = my名称,
            //                地址 = my地址,
            //                分类 = my分类,
            //                单词数量 = my单词数量,
            //                系列 = my系列,
            //                序号 = my序号
            //            };
            //            m_context.课本集.AddObject(my课本);
            //        }
            //        else
            //        {
            //            my课本.名称 = my名称;
            //            my课本.地址 = my地址;
            //            my课本.分类 = my分类;
            //            my课本.单词数量 = my单词数量;
            //            my课本.系列 = my系列;
            //            my课本.序号 = my序号;
            //        }
            //    }

            //    //扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
            //    //if (my扫描指针 == null)
            //    //    m_context.扫描指针集.AddObject(new 扫描指针 { ID = 1, 扫描日期 = DateTime.Now, 扫描地址 = "", 扫描类型 = "分类", 当前ID = 1 });
            //    //else
            //    //{
            //    //    my扫描指针.ID = 1;
            //    //    my扫描指针.扫描日期 = DateTime.Now;
            //    //    my扫描指针.扫描地址 = "";
            //    //    my扫描指针.扫描类型 = "分类";
            //    //    my扫描指针.当前ID = 1;
            //    //}
            //    i = m_context.SaveChanges();
            this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_课本DocumentCompleted);
            //    if (this.dataGridView1.DataSource == m_context.课本集)
            //        this.dataGridView1.DataSource = null;
            //    this.dataGridView1.DataSource = m_context.课本集;
            //}
            //catch (UpdateException ex)
            //{
            //    this.toolStripStatusLabel1.Text = ex.Message;
            //}
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
            if (((DataGridView)sender).DataSource == null)
                return;
            扫描指针 my扫描指针 = m_context.扫描指针集.Single();

            switch (my扫描指针.扫描类型)
            {
                case "分类":
                    
                    var my下一个分类 = (from o in m_context.分类集 where o.已扫描 == false select o).FirstOrDefault();
                    if (my下一个分类 != null)
                    {
                        this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分类DocumentCompleted);
                        m_下一个地址 = my下一个分类.地址;
                        this.webBrowser1.Navigate(m_下一个地址);
                        my下一个分类.已扫描 = true;
                    }
                    else
                    {
                        my扫描指针.扫描类型 = "课本";
                    }
                    m_context.SaveChanges();
                    break;
                case "课本":
                    this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分类DocumentCompleted);
                    var my下一个课本 = (from o in m_context.课本集 where o.已扫描 == false select o).FirstOrDefault();
                    if (my下一个课本 != null)
                    {
                        this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_课本DocumentCompleted);
                        m_下一个地址 = my下一个课本.地址;
                        this.webBrowser1.Navigate(m_下一个地址);
                        my下一个课本.已扫描 = true;
                    }
                    else
                    {
                        my扫描指针.扫描类型 = "分组";
                    }
                    m_context.SaveChanges();
                    break;
                case "分组":
                    break;
                case "分页":
                    break;
                case "单词":
                    break;
                default:
                    break;
            }
        }
    }
}
