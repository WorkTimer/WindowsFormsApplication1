using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Collections;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        public List<WebClient> tab = new List<WebClient>();
        private DictCnEntities m_context = new DictCnEntities();

        private void button1_Click(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.label1.Text = this.tab.Count.ToString();
            string my读音地址 = e.UserState.ToString();
            System.Linq.IQueryable<WindowsFormsApplication1.单词> my单词集 = null;
            if (e.Error != null)
            {
                //throw e.Error;
                System.Linq.IQueryable<WindowsFormsApplication1.单词> my单词读音集 = (from o in m_context.单词集 where o.读音 == my读音地址 select o);
                foreach (单词 item in my单词读音集)
                {
                    if (e.Error.Message == "远程服务器返回错误: (404) 未找到。")
                    {
                        item.已扫描 = true;
                        item.读音 += "|404错误";
                    }
                    else
                    {
                        item.已扫描 = false;
                    }
                }
                m_context.SaveChanges();
            }
            else
            {

                my单词集 = (from o in m_context.单词集 where o.读音 == my读音地址 select o);
                foreach (单词 item in my单词集)
                {
                    item.已扫描 = true;
                }
                m_context.SaveChanges();
            }
            my读音地址 = null;
            my单词集 = (from o in m_context.单词集 where o.已扫描 == false && o.读音 != @"http://www13.dict.cn/mp3.php?q=" select o);
            单词 my单词 = my单词集.FirstOrDefault();
            if (my单词 != null)
            {
                my读音地址 = my单词.读音;
                System.Linq.IQueryable<WindowsFormsApplication1.单词> my单词读音集 = (from o in m_context.单词集 where o.已扫描 == false && o.读音 == my读音地址 select o);
                foreach (单词 item in my单词读音集)
                {
                    item.已扫描 = null;
                }
                m_context.SaveChanges();
            }
            else
            {
                this.timer1.Enabled = false;
                MessageBox.Show("读音下载完成！");
            }
            
            ((WebClient)sender).DownloadFileAsync(new Uri(my读音地址), @"./" + my读音地址.Replace(@"http://www13.dict.cn/mp3.php?q=", "") + ".mp3", my读音地址);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.tab.Count < this.numericUpDown2.Value)
            {
                string my读音地址 = null;
                System.Linq.IQueryable<WindowsFormsApplication1.单词> my单词集 = (from o in m_context.单词集 where o.已扫描 == false && o.读音 != @"http://www13.dict.cn/mp3.php?q=" select o);
                单词 my单词 = my单词集.FirstOrDefault();
                if (my单词 != null)
                {
                    my读音地址 = my单词.读音;
                    System.Linq.IQueryable<WindowsFormsApplication1.单词> my单词读音集 = (from o in m_context.单词集 where o.已扫描 == false && o.读音 == my读音地址 select o);
                    foreach (单词 item in my单词读音集)
                    {
                        item.已扫描 = null;
                    }
                    m_context.SaveChanges();
                }
                else
                {
                    ((Timer)sender).Enabled = false;
                    MessageBox.Show("读音下载完成！");
                }

                WebClient client = new WebClient();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                this.tab.Add(client);
                client.DownloadFileAsync(new Uri(my读音地址), @"./" + my读音地址.Replace(@"http://www13.dict.cn/mp3.php?q=", "") + ".mp3", my读音地址);
            }
            else
            {
                this.timer1.Enabled = false;
            }
        }
          
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.timer1.Interval = Convert.ToInt32(((NumericUpDown)sender).Value);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            System.Linq.IQueryable<WindowsFormsApplication1.单词> my单词读音集 = (from o in m_context.单词集 where o.已扫描 == null select o);
            foreach (单词 item in my单词读音集)
            {
                item.已扫描 = false;
            }
            m_context.SaveChanges();
            this.timer1.Interval = Convert.ToInt32(this.numericUpDown1.Value);
            this.button1.Enabled = true;
        }

    
    }
}
