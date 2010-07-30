using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DictCnEntities m_context = new DictCnEntities();
        private const String m_StartAddress = "http://www13.dict.cn/bdc/";
        string _str下一个地址 = null;
        private string m_下一个地址
        {
            get
            {
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
                if (my扫描指针 == null)
                    return "";
                if (string.IsNullOrEmpty(my扫描指针.扫描地址))
                {
                    _str下一个地址 = m_StartAddress;
                    my扫描指针.扫描地址 = _str下一个地址;
                    m_context.SaveChanges();
                }
                else
                {
                    
                    _str下一个地址 = my扫描指针.扫描地址;
                }
                return _str下一个地址;
            }
            set
            {
                _str下一个地址 = value;
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
                my扫描指针.扫描地址 = _str下一个地址;
                m_context.SaveChanges();
            }
        }

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
                i = m_context.SaveChanges();

                var my下一个分类 = (from o in m_context.分类集 where o.已扫描 == false select o).FirstOrDefault();
                m_下一个地址 = my下一个分类.地址;
                i = m_context.SaveChanges();
                
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
                if (my扫描指针 == null)
                    m_context.扫描指针集.AddObject(new 扫描指针 { ID = 1, 扫描日期 = DateTime.Now, 扫描地址 = "", 扫描类型 = "分类"});
                else
                {
                    my扫描指针.ID = 1;
                    my扫描指针.扫描日期 = DateTime.Now;
                    my扫描指针.扫描地址 = m_下一个地址;
                    my扫描指针.扫描类型 = "分类";
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
            扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
            if (my扫描指针 == null)
            {
                my扫描指针 = new 扫描指针 { ID = 1, 扫描日期 = DateTime.Now, 扫描地址 = m_下一个地址, 扫描类型 = ""};
                m_context.扫描指针集.AddObject(my扫描指针);
                m_context.SaveChanges();
            }
            if (this.dataGridView1.DataSource == m_context.扫描指针集)
                this.dataGridView1.DataSource = null;
            this.dataGridView1.DataSource = m_context.扫描指针集;
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.toolStripTextBox1.Size = new Size(this.toolStrip2.DisplayRectangle.Width - 2, this.toolStripTextBox1.Height);
        }
        
        private void webBrowser1_分类DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != this.webBrowser1.Url) return;
            if (e.Url.AbsoluteUri != m_下一个地址) return;
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
            WebBrowser web = (WebBrowser)sender;

            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(web.DocumentStream);
                HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"bdc_catalog\"]/div[ul]/ul/li/div");
                string my分类名 = doc.DocumentNode.SelectSingleNode("//*[@id=\"bdc_catalog\"]/h4").InnerText;
                string my分类地址 = e.Url.AbsoluteUri;
                if (m_context == null) m_context = new DictCnEntities();
                分类 my分类 = (from o in m_context.分类集 where o.地址 == my分类地址 && o.名称 == my分类名 select o).FirstOrDefault();

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
                my分类.已扫描 = true;
                i = m_context.SaveChanges();
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();


                var my下一个分类 = (from o in m_context.分类集 where o.已扫描 == false select o).FirstOrDefault();
                if (my下一个分类 != null)
                {
                    m_下一个地址 = my下一个分类.地址;
                }
                else
                {
                    var my下一个课本 = (from o in m_context.课本集 where o.已扫描 == false select o).FirstOrDefault();
                    m_下一个地址 = my下一个课本.地址;
                    my扫描指针.扫描类型 = "课本";
                }
                my扫描指针.扫描地址 = m_下一个地址;
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
            try
            {
                int i = 0;

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(web.DocumentStream);
                HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"main_word_top\"]/p");
                HtmlNode my当前课本Node = htmlNodes[0].SelectSingleNode("//*[text()=\"当前课本:\"]");
                HtmlNode my首字母Node = htmlNodes[0].SelectSingleNode("//*[text()=\"首字母:\"]");
                HtmlNode my课数Node = htmlNodes[0].SelectSingleNode("//*[text()=\"课数:\"]");
                HtmlNode my分页Node = htmlNodes[0].SelectSingleNode("//*[text()=\"分页:\"]");
                HtmlNode my分组Node = null;

                string my课本地址 = e.Url.AbsoluteUri;
                //Debug.Assert();
                if (m_context == null) m_context = new DictCnEntities();
                课本 my当前课本 = (from o in m_context.课本集 where (o.地址 == my课本地址) && (o.已扫描 == false) select o).FirstOrDefault();
                if (my首字母Node == null && my课数Node != null)
                {
                    my当前课本.分组方式 = 1;
                    my分组Node = my课数Node.ParentNode;
                }
                else
                    if (my首字母Node != null && my课数Node == null)
                    {
                        my当前课本.分组方式 = 2;
                        my分组Node = my首字母Node.ParentNode;
                    }
                    else
                    {
                        throw new Exception();
                    }

                HtmlNodeCollection my分组Nodes = my分组Node.SelectNodes("./span|./a");
                foreach (HtmlNode htmlNode in my分组Nodes)
                {
                    if (htmlNode.ChildNodes.Count != 2)
                        continue;
                    string my名称 = htmlNode.ChildNodes[0].InnerText.Trim();
                    string my地址 = htmlNode.Attributes["href"] == null ? (my课本地址 + "-" + my名称) : (m_StartAddress + htmlNode.Attributes["href"].Value.Trim());
                    string str单词个数 = htmlNode.ChildNodes[1].InnerText.Trim();
                    str单词个数 = str单词个数.Substring(str单词个数.IndexOf('/') + 1).TrimEnd(new char[] { ')' });
                    int i单词个数 = Convert.ToInt32(str单词个数);

                    分组 my分组 = (from o in m_context.分组集 where o.名称 == my名称 && o.地址 == my地址 select o).FirstOrDefault();
                    if (my分组 == null)
                    {
                        my分组 = new 分组
                        {
                            名称 = my名称,
                            课本 = my当前课本,
                            地址 = my地址,
                            单词数量 = i单词个数,
                            页数 = i单词个数 / 50 + 1
                        };
                    }
                    else
                    {
                        my分组.名称 = my名称;
                        my分组.课本 = my当前课本;
                        my分组.地址 = my地址;
                        my分组.单词数量 = i单词个数;
                        my分组.页数 = (i单词个数-1) / 50 + 1;
                    }
                }
                my当前课本.已扫描 = true;
                i = m_context.SaveChanges();
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();

                var my下一个课本 = (from o in m_context.课本集 where o.已扫描 == false select o).FirstOrDefault();
                if (my下一个课本 != null)
                {
                    m_下一个地址 = my下一个课本.地址;
                }
                else
                {
                    var my下一个分组 = (from o in m_context.分组集 where o.已扫描 == false select o).FirstOrDefault();
                    m_下一个地址 = my下一个分组.地址;
                    my扫描指针.扫描类型 = "分组";
                }
                my扫描指针.扫描地址 = m_下一个地址;
                i = m_context.SaveChanges();
                this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_课本DocumentCompleted);
                if (this.dataGridView1.DataSource == m_context.分组集)
                    this.dataGridView1.DataSource = null;
                this.dataGridView1.DataSource = m_context.分组集;
            }
            catch (UpdateException ex)
            {
                this.toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void webBrowser1_分组DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url != this.webBrowser1.Url) return;
            if (e.Url.AbsoluteUri != m_下一个地址) return;
            this.toolStripTextBox1.Text = e.Url.AbsoluteUri;
            WebBrowser web = (WebBrowser)sender;
            try
            {
                int i = 0;

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(web.DocumentStream);
                HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"main_word_top\"]/p[font=\"分页:\"]");
                HtmlNode my分页Node = null;
                               
                string my分页地址 = e.Url.AbsoluteUri;

                if (m_context == null) m_context = new DictCnEntities();
                分组 my当前分组 = (from o in m_context.分组集 where (o.地址 == my分页地址) && (o.已扫描 == false) select o).FirstOrDefault();
                if (htmlNodes != null)
                {
                    my分页Node = htmlNodes[0];

                    HtmlNodeCollection my分页Nodes = my分页Node.SelectNodes("./span|./a[@class=\"green bold\"]");
                    foreach (HtmlNode htmlNode in my分页Nodes)
                    {
                        Debug.Assert(htmlNode.ChildNodes.Count == 2);
                        if (htmlNode.ChildNodes.Count != 2)
                            continue;

                        string my名称 = htmlNode.ChildNodes[0].InnerText.Trim();
                        string my地址 = htmlNode.Attributes["href"] == null ? (my分页地址 + "-" + i.ToString()) : (m_StartAddress + htmlNode.Attributes["href"].Value.Trim());
                        i++;
                        string str单词个数 = htmlNode.ChildNodes[1].InnerText.Trim();
                        str单词个数 = str单词个数.Substring(str单词个数.IndexOf('/') + 1).TrimEnd(new char[] { ')' });
                        int i单词个数 = Convert.ToInt32(str单词个数);

                        分页 my分页 = (from o in m_context.分页集 where o.名称 == my名称 && o.地址 == my地址 select o).FirstOrDefault();
                        if (my分页 == null)
                        {
                            my分页 = new 分页
                            {
                                名称 = my名称,
                                分组 = my当前分组,
                                地址 = my地址,
                                单词数量 = i单词个数,
                            };
                        }
                        else
                        {
                            my分页.名称 = my名称;
                            my分页.分组 = my当前分组;
                            my分页.地址 = my地址;
                            my分页.单词数量 = i单词个数;
                        }
                    }
                    Debug.Assert((i == my当前分组.页数) && (i == my分页Nodes.Count));
                }
                else
                {
                    string my名称 = "1";
                    string my地址 = my分页地址 + "-" + "0";
                    int i单词个数 = my当前分组.单词数量;

                    分页 my分页 = (from o in m_context.分页集 where o.名称 == my名称 && o.地址 == my地址 select o).FirstOrDefault();
                    if (my分页 == null)
                    {
                        my分页 = new 分页
                        {
                            名称 = my名称,
                            分组 = my当前分组,
                            地址 = my地址,
                            单词数量 = i单词个数,
                        };
                    }
                    else
                    {
                        my分页.名称 = my名称;
                        my分页.分组 = my当前分组;
                        my分页.地址 = my地址;
                        my分页.单词数量 = i单词个数;
                    }
                }
                i = m_context.SaveChanges();
                
                my当前分组.已扫描 = true;
                i = m_context.SaveChanges();
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();

                var my下一个分组 = (from o in m_context.分组集 where o.已扫描 == false select o).FirstOrDefault();
                if (my下一个分组 != null)
                {
                    m_下一个地址 = my下一个分组.地址;
                }
                else
                {
                    var my下一个分页 = (from o in m_context.分页集 where o.已扫描 == false select o).FirstOrDefault();
                    m_下一个地址 = my下一个分页.地址;
                    my扫描指针.扫描类型 = "分页";
                }
                my扫描指针.扫描地址 = m_下一个地址;
                i = m_context.SaveChanges();
                this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分组DocumentCompleted);
                if (this.dataGridView1.DataSource == m_context.分页集)
                    this.dataGridView1.DataSource = null;
                this.dataGridView1.DataSource = m_context.分页集;
            }
            catch (UpdateException ex)
            {
                this.toolStripStatusLabel1.Text = ex.Message;
            }
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
            扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();
            switch (my扫描指针.扫描类型)
            {
                case "分类":
                    this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分类DocumentCompleted);
                    break;
                case "课本":
                    this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_课本DocumentCompleted);
                    break;
                case "分组":
                    this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分组DocumentCompleted);
                    break;
                case "分页":
                    break;
                case "单词":
                    break;
                default:
                    this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
                    break;
            }
            this.webBrowser1.Navigate(m_下一个地址);
        }
    }
}
