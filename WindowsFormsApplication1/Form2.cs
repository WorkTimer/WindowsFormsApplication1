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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.webBrowser1.Navigate(this.textBox1.Text);
        }

        private string m_StartAddress = "http://dict.cn/bdc/";

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int i = 0;
            if (e.Url != this.webBrowser1.Url) return;
            if (e.Url.AbsoluteUri != this.textBox1.Text) return;
            WebBrowser web = (WebBrowser)sender;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(web.DocumentStream);
            HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"main_word_top\"]/p[font=\"分页:\"]");
                            HtmlNode my分页Node = null;
                if (htmlNodes != null)
                    my分页Node = htmlNodes[0];
                else
                    return;

                string my分页地址 = e.Url.AbsoluteUri;


                HtmlNodeCollection my分页Nodes = my分页Node.SelectNodes("./span|./a[@class=\"green bold\"]");
                foreach (HtmlNode htmlNode in my分页Nodes)
                {
                    if (htmlNode.ChildNodes.Count != 2)
                        continue;
                    string my名称 = htmlNode.ChildNodes[0].InnerText.Trim();
                    string my地址 = htmlNode.Attributes["href"] == null ? (my分页地址 + "-" + i.ToString()) : (m_StartAddress + htmlNode.Attributes["href"].Value.Trim());
                    i++;
                    string str单词个数 = htmlNode.ChildNodes[1].InnerText.Trim();
                    str单词个数 = str单词个数.Substring(str单词个数.IndexOf('/') + 1).TrimEnd(new char[] { ')' });
                    int i单词个数 = Convert.ToInt32(str单词个数);
                }
        }
    }
}
