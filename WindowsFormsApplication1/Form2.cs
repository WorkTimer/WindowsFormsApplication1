using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.Web;
using System.IO;
using System.Collections;
using System.Diagnostics;

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

        private const string m_StartAddress = "http://dict.cn/bdc/";
        private const String m_StrMp3Address = "http://www13.dict.cn/mp3.php?q=";

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int i = 0;
            if (e.Url != this.webBrowser1.Url) return;
            if (e.Url.AbsoluteUri != this.textBox1.Text) return;
            WebBrowser web = (WebBrowser)sender;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(web.DocumentStream);

            Debug.Assert("&nbsp;记住：0个&nbsp;".Equals(doc.DocumentNode.SelectSingleNode("//*[@id=\"rememberalready\"]").InnerText));
            HtmlNodeCollection htmlNodes = doc.DocumentNode.SelectNodes("//*[@id=\"wordList\"]/tr");

            string my分组地址 = e.Url.AbsoluteUri;

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
                string my读音 = m_StrMp3Address + array音标和读音[1].Trim();
                string my解释 = htmlNode.ChildNodes[4].InnerText.Trim();

                my音标 = HtmlDecode(my音标);
            }
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
                            int num3 = s.IndexOfAny(new char[] {';'}, i + 1);
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



 


}
