using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            readconfig();
            viewlist(launcherdbs);
        }
        List<Launcherdb> launcherdbs = new List<Launcherdb>();
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //gbzczx.Visible = false;
            //if (comboBox1.Items[comboBox1.SelectedIndex].ToString() == "注册中心")
            //{
            //    gbzczx.Visible = true;
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Launcherdb launcherdb = new Launcherdb();
            launcherdb.name = txtname.Text;
            launcherdb.port =Convert.ToInt32( txtport.Text);
            launcherdb.type = comboBox1.Items[comboBox1.SelectedIndex].ToString();
            launcherdb.url = txturl.Text;
            launcherdb.CD = txtcd.Text;
            launcherdbs.Add(launcherdb);

            save(launcherdbs);
            viewlist(launcherdbs);
        }
        public void readconfig()
        {
            try
            {
                System.IO.StreamReader streamWriter = new System.IO.StreamReader("config.bin");
                String str = streamWriter.ReadToEnd();
                streamWriter.Close();
                launcherdbs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Launcherdb>>(str);
            }
            catch { }
        }
        public void viewlist(List<Launcherdb> launcherdbs)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < launcherdbs.Count; i++)
            {
                listBox1.Items.Add(launcherdbs[i].name);
            }
        }
        public void save(List<Launcherdb> launcherdbs)
        {
            String str = Newtonsoft.Json.JsonConvert.SerializeObject(launcherdbs);
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("config.bin", false);
            streamWriter.Write(str);
            streamWriter.Close();
        }

        private void gbzczx_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            txtcd.Text= openFileDialog1.FileName;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Launcherdb launcherdb = new Launcherdb();
            launcherdb.name = txtname.Text;
            launcherdb.port = Convert.ToInt32(0);
            launcherdb.type = comboBox1.Items[comboBox1.SelectedIndex].ToString();
            launcherdb.url ="";
            launcherdb.CD = txtcd.Text;
            launcherdbs.Add(launcherdb);

            save(launcherdbs);
            viewlist(launcherdbs);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DialogResult dr1 = MessageBox.Show("是否删除","",MessageBoxButtons.YesNo);
            try
            {
                if (dr1 == DialogResult.Yes)
                {
                    launcherdbs.RemoveAt(listBox1.SelectedIndex);
                    save(launcherdbs);
                    viewlist(launcherdbs);
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < launcherdbs.Count; i++)
            {

                Process process= Process.Start(launcherdbs[i].CD);
                //process.WaitForExit();
            }
            
        }
    }
    public class Launcherdb
    {
        public string name = "";
        public string type = "";
        public int port = 9001;
        public string url = "";
        public String CD = "";
    }
}
