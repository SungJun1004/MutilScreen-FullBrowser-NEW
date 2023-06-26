using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MutilScreen_FullBrowser.MyForm;

namespace MutilScreen_FullBrowser
{
    public partial class Form2 : Form
    {
        public static string URLMessage = "";
        public static int nAutoRefreshTime = 0;
        private  MyForm  form1Instance;

        public Form2(ChromiumWebBrowser browser,MyForm myform)
        {
            InitializeComponent();
            form1Instance = myform;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            URLMessage = textBox1.Text;

            if  (textBox1.Text == "")
            {
                textBox1.Text = "http://sap.com";
            }
            Properties.Settings.Default.strURL = textBox1.Text;
            
            if (textBox2.Text == "")
            {
                textBox2.Text = "0";
            }
            Properties.Settings.Default.nAutoRefreshTime = int.Parse(textBox2.Text);
            Properties.Settings.Default.nStartWidth = int.Parse(textBox3.Text);
            Properties.Settings.Default.Save();
            
            ChromiumWebBrowser browser = CefSharpBrowserManager.GetBrowser();
            if (browser != null)
            
            {
                browser.LoadUrl(Properties.Settings.Default.strURL);

            }
           
            form1Instance.ReloadTimer_Start();
            

            this.Hide();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            URLMessage = Properties.Settings.Default.strURL;
            form1Instance.ReloadTimer_Start();

            this.Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(450, 400);
            textBox1.Text = Properties.Settings.Default.strURL;
            textBox2.Text = Properties.Settings.Default.nAutoRefreshTime.ToString();
            textBox3.Text = Properties.Settings.Default.nStartWidth.ToString();

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
