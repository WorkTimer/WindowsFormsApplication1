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
using System.Collections;
using System.IO;

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
        private const String m_StrMp3Address = "http://www13.dict.cn/mp3.php?q=";
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                    Debug.Assert(i单词个数 <= 50);

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
            catch (Exception ex)
            {
                this.toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void webBrowser1_分页DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int i = 0;
            if (e.Url != this.webBrowser1.Url) return;
            if (e.Url.AbsoluteUri != m_下一个地址) return;
            WebBrowser web = (WebBrowser)sender;

            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.Load(web.DocumentStream);

                Debug.Assert("&nbsp;记住：0个&nbsp;".Equals(doc.DocumentNode.SelectSingleNode("//*[@id=\"rememberalready\"]").InnerText));
                HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"wordList\"]/tr");

                string my分页地址 = e.Url.AbsoluteUri;
                分页 my当前分页 = (from o in m_context.分页集 where (o.地址 == my分页地址) && (o.已扫描 == false) select o).FirstOrDefault();

                foreach (HtmlNode htmlNode in htmlNodes)
                {
                    if (htmlNode.ChildNodes.Count != 5)
                        continue;

                    string str编号 = htmlNode.ChildNodes[0].InnerText.Trim();
                    int i编号 = Convert.ToInt32(str编号);
                    string my拼写 = htmlNode.ChildNodes[1].InnerText.Trim();
                    string str音标和读音 = htmlNode.ChildNodes[2].InnerText.Trim();
                    string[] array音标和读音 = str音标和读音.Split(new char[] { '|' });
                    string my音标 = array音标和读音[0].Trim();
                    my音标 = HtmlDecode(my音标);
                    string my读音 = m_StrMp3Address + array音标和读音[1].Trim();
                    string my解释 = htmlNode.ChildNodes[4].InnerText.Trim();


                    单词 my单词 = (from o in m_context.单词集 
                               where o.拼写 == my拼写 && o.分页.地址 == my分页地址 && o.编号 == i编号 && o.读音 == my读音 && o.音标 == my音标 && o.拼写 == my拼写 && o.解释 == my解释 select o).FirstOrDefault();
                    if (my单词 == null)
                    {
                        my单词 = new 单词
                        {
                            编号 = i编号,
                            分页 = my当前分页,
                            拼写 = my拼写,
                            音标 = my音标,
                            读音 = my读音,
                            解释 = my解释
                        };
                    }
                    else
                    {
                        my单词.编号 = i编号;
                        my单词.分页 = my当前分页;
                        my单词.拼写 = my拼写;
                        my单词.音标 = my音标;
                        my单词.读音 = my读音;
                        my单词.解释 = my解释;
                    }
                }

                i = m_context.SaveChanges();

                my当前分页.已扫描 = true;
                i = m_context.SaveChanges();
                扫描指针 my扫描指针 = m_context.扫描指针集.SingleOrDefault();

                var my下一个分页 = (from o in m_context.分页集 where o.已扫描 == false select o).FirstOrDefault();
                if (my下一个分页 != null)
                {
                    m_下一个地址 = my下一个分页.地址;
                }
                else
                {
                    MessageBox.Show("完成");
                    return;
                }
            

                my扫描指针.扫描地址 = m_下一个地址;
                i = m_context.SaveChanges();
                this.webBrowser1.DocumentCompleted -= new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分页DocumentCompleted);
                if (this.dataGridView1.DataSource == m_context.分页集)
                    this.dataGridView1.DataSource = null;
                this.dataGridView1.DataSource = m_context.分页集;

            }
            catch(Exception ex)
            {
                this.toolStripStatusLabel1.Text = ex.Message;
                MessageBox.Show(ex.Message);
            }
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
                    this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_分页DocumentCompleted);
                    break;
                case "单词":
                    break;
                default:
                    this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
                    break;
            }
            this.webBrowser1.Navigate(m_下一个地址);
        }

        public static string HtmlDecode(string s)
        {
            if (s == null)
            {
                return null;
            }
            if (s.IndexOf('&') < 0)
            {
                return s;
            }
            StringBuilder sb = new StringBuilder();
            StringWriter output = new StringWriter(sb);
            HtmlDecode(s, output);
            return sb.ToString();
        }


        public static void HtmlDecode(string s, TextWriter output)
        {
            if (s != null)
            {
                if (s.IndexOf('&') < 0)
                {
                    output.Write(s);
                }
                else
                {
                    int length = s.Length;
                    for (int i = 0; i < length; i++)
                    {
                        char ch = s[i];
                        if (ch == '&')
                        {
                            int num3 = s.IndexOfAny(new char[] { ';' }, i + 1);
                            if ((num3 > 0) && (s[num3] == ';'))
                            {
                                string entity = s.Substring(i + 1, (num3 - i) - 1);
                                if ((entity.Length > 1) && (entity[0] == '#'))
                                {
                                    try
                                    {
                                        if ((entity[1] == 'x') || (entity[1] == 'X'))
                                        {
                                            ch = (char)int.Parse(entity.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                        }
                                        else
                                        {
                                            ch = (char)int.Parse(entity.Substring(1));
                                        }
                                        i = num3;
                                    }
                                    catch (FormatException)
                                    {
                                        i++;
                                    }
                                    catch (ArgumentException)
                                    {
                                        i++;
                                    }
                                }
                                else
                                {
                                    i = num3;
                                    char ch2 = HtmlEntities.Lookup(entity);
                                    if (ch2 != '\0')
                                    {
                                        ch = ch2;
                                    }
                                    else
                                    {
                                        output.Write('&');
                                        output.Write(entity);
                                        output.Write(';');
                                        goto Label_0103;
                                    }
                                }
                            }
                        }
                        output.Write(ch);
                    Label_0103: ;
                    }
                }
            }
        }

    }

    internal class HtmlEntities
    {
        // Fields
        private static string[] _entitiesList = new string[] { 
        "\"-quot", "&-amp", "<-lt", ">-gt", "\x00a0-nbsp", "\x00a1-iexcl", "\x00a2-cent", "\x00a3-pound", "\x00a4-curren", "\x00a5-yen", "\x00a6-brvbar", "\x00a7-sect", "\x00a8-uml", "\x00a9-copy", "\x00aa-ordf", "\x00ab-laquo", 
        "\x00ac-not", "\x00ad-shy", "\x00ae-reg", "\x00af-macr", "\x00b0-deg", "\x00b1-plusmn", "\x00b2-sup2", "\x00b3-sup3", "\x00b4-acute", "\x00b5-micro", "\x00b6-para", "\x00b7-middot", "\x00b8-cedil", "\x00b9-sup1", "\x00ba-ordm", "\x00bb-raquo", 
        "\x00bc-frac14", "\x00bd-frac12", "\x00be-frac34", "\x00bf-iquest", "\x00c0-Agrave", "\x00c1-Aacute", "\x00c2-Acirc", "\x00c3-Atilde", "\x00c4-Auml", "\x00c5-Aring", "\x00c6-AElig", "\x00c7-Ccedil", "\x00c8-Egrave", "\x00c9-Eacute", "\x00ca-Ecirc", "\x00cb-Euml", 
        "\x00cc-Igrave", "\x00cd-Iacute", "\x00ce-Icirc", "\x00cf-Iuml", "\x00d0-ETH", "\x00d1-Ntilde", "\x00d2-Ograve", "\x00d3-Oacute", "\x00d4-Ocirc", "\x00d5-Otilde", "\x00d6-Ouml", "\x00d7-times", "\x00d8-Oslash", "\x00d9-Ugrave", "\x00da-Uacute", "\x00db-Ucirc", 
        "\x00dc-Uuml", "\x00dd-Yacute", "\x00de-THORN", "\x00df-szlig", "\x00e0-agrave", "\x00e1-aacute", "\x00e2-acirc", "\x00e3-atilde", "\x00e4-auml", "\x00e5-aring", "\x00e6-aelig", "\x00e7-ccedil", "\x00e8-egrave", "\x00e9-eacute", "\x00ea-ecirc", "\x00eb-euml", 
        "\x00ec-igrave", "\x00ed-iacute", "\x00ee-icirc", "\x00ef-iuml", "\x00f0-eth", "\x00f1-ntilde", "\x00f2-ograve", "\x00f3-oacute", "\x00f4-ocirc", "\x00f5-otilde", "\x00f6-ouml", "\x00f7-divide", "\x00f8-oslash", "\x00f9-ugrave", "\x00fa-uacute", "\x00fb-ucirc", 
        "\x00fc-uuml", "\x00fd-yacute", "\x00fe-thorn", "\x00ff-yuml", "Œ-OElig", "œ-oelig", "Š-Scaron", "š-scaron", "Ÿ-Yuml", "ƒ-fnof", "ˆ-circ", "˜-tilde", "Α-Alpha", "Β-Beta", "Γ-Gamma", "Δ-Delta", 
        "Ε-Epsilon", "Ζ-Zeta", "Η-Eta", "Θ-Theta", "Ι-Iota", "Κ-Kappa", "Λ-Lambda", "Μ-Mu", "Ν-Nu", "Ξ-Xi", "Ο-Omicron", "Π-Pi", "Ρ-Rho", "Σ-Sigma", "Τ-Tau", "Υ-Upsilon", 
        "Φ-Phi", "Χ-Chi", "Ψ-Psi", "Ω-Omega", "α-alpha", "β-beta", "γ-gamma", "δ-delta", "ε-epsilon", "ζ-zeta", "η-eta", "θ-theta", "ι-iota", "κ-kappa", "λ-lambda", "μ-mu", 
        "ν-nu", "ξ-xi", "ο-omicron", "π-pi", "ρ-rho", "ς-sigmaf", "σ-sigma", "τ-tau", "υ-upsilon", "φ-phi", "χ-chi", "ψ-psi", "ω-omega", "ϑ-thetasym", "ϒ-upsih", "ϖ-piv", 
        " -ensp", " -emsp", " -thinsp", "‌-zwnj", "‍-zwj", "‎-lrm", "‏-rlm", "–-ndash", "—-mdash", "‘-lsquo", "’-rsquo", "‚-sbquo", "“-ldquo", "”-rdquo", "„-bdquo", "†-dagger", 
        "‡-Dagger", "•-bull", "…-hellip", "‰-permil", "′-prime", "″-Prime", "‹-lsaquo", "›-rsaquo", "‾-oline", "⁄-frasl", "€-euro", "ℑ-image", "℘-weierp", "ℜ-real", "™-trade", "ℵ-alefsym", 
        "←-larr", "↑-uarr", "→-rarr", "↓-darr", "↔-harr", "↵-crarr", "⇐-lArr", "⇑-uArr", "⇒-rArr", "⇓-dArr", "⇔-hArr", "∀-forall", "∂-part", "∃-exist", "∅-empty", "∇-nabla", 
        "∈-isin", "∉-notin", "∋-ni", "∏-prod", "∑-sum", "−-minus", "∗-lowast", "√-radic", "∝-prop", "∞-infin", "∠-ang", "∧-and", "∨-or", "∩-cap", "∪-cup", "∫-int", 
        "∴-there4", "∼-sim", "≅-cong", "≈-asymp", "≠-ne", "≡-equiv", "≤-le", "≥-ge", "⊂-sub", "⊃-sup", "⊄-nsub", "⊆-sube", "⊇-supe", "⊕-oplus", "⊗-otimes", "⊥-perp", 
        "⋅-sdot", "⌈-lceil", "⌉-rceil", "⌊-lfloor", "⌋-rfloor", "〈-lang", "〉-rang", "◊-loz", "♠-spades", "♣-clubs", "♥-hearts", "♦-diams"
     };
        private static Hashtable _entitiesLookupTable;
        private static object _lookupLockObject = new object();

        // Methods
        private HtmlEntities()
        {
        }

        internal static char Lookup(string entity)
        {
            if (_entitiesLookupTable == null)
            {
                lock (_lookupLockObject)
                {
                    if (_entitiesLookupTable == null)
                    {
                        Hashtable hashtable = new Hashtable();
                        foreach (string str in _entitiesList)
                        {
                            hashtable[str.Substring(2)] = str[0];
                        }
                        _entitiesLookupTable = hashtable;
                    }
                }
            }
            object obj2 = _entitiesLookupTable[entity];
            if (obj2 != null)
            {
                return (char)obj2;
            }
            return '\0';
        }
    }
}
