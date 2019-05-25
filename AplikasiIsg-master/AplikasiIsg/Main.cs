using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AplikasiIsg
{
    public partial class Main : MetroFramework.Forms.MetroForm
    {
        Timer t = new Timer();
        static int schedInt = 10;
        DateTime next = DateTime.Now;
        fileMan fman = new fileMan();
        wsClass wsf = new wsClass();
        string inbox, outbox, succcess, error;

        public Main()
        {
            InitializeComponent();

        }

        private void Main_Load(object sender, EventArgs e)
        {
            reloadini();
            //timer interval
            t.Interval = 1000;  //in milliseconds

            t.Tick += new EventHandler(this.t_Tick);

            //start timer when form loads
            // t.Start();  //this will use t_Tick() method
            button1.Hide();
            button4.Hide();
            button2.Hide();
            button3.Hide();
        }

        //timer eventhandler
        private void t_Tick(object sender, EventArgs e)
        {
            string log = "";
            int jum = 0;

            //get current time
            int hh = DateTime.Now.Hour;
            int mm = DateTime.Now.Minute;
            int ss = DateTime.Now.Second;

            //time
            string time = jamText(hh.ToString() + ":" + mm.ToString() + ":" + ss.ToString());

            //update label
            txtJam.Text = time;

            if (next < DateTime.Now)
            {
                
                jum = fman.cekDir(inbox);
                log = "[" + DateTime.Now.ToString("HH:MM:ss") + "]  " + jum.ToString() + " files on Inbox";
                updateLog(log);

                if (jum > 0)
                {
                    //konversi
                    string[] files = fman.getFile(inbox);
                    if (files != null && files.Length > 0)
                    {
                        foreach (string i in files)
                        {
                            int conFile = fman.xlsToTxt(i, "xlsx");

                            if (conFile > 0)
                            {
                                fman.moveFile(i, error);
                                log = "[" + DateTime.Now.ToString("HH:MM:ss") + "]  " + conFile.ToString() + " new files Converted";
                                updateLog(log);
                            }
                            else
                            {
                                fman.moveFile(i, error);
                            }
                        }
                    }
                }

                int jumOut = fman.cekDir(outbox);
                log = "[" + DateTime.Now.ToString("HH:MM:ss") + "]  " + jumOut.ToString() + " files on Outbox";
                updateLog(log);

                if (jumOut > 0)
                {
                    string[] files = fman.getFile(outbox);
                    string pes = "";
                    string[] fName = null;
                    if (files != null && files.Length > 0)
                    {
                        foreach (string i in files)
                        {
                            pes = wsf.invokeProd("", "", i);
                            //pes = wsf.invokeProdUoi("", "", i);
                            string[] has = pes.Split('|');
                            fName = i.Split('\\');
                            string namaFile = fName[fName.Length - 1];
                            System.Console.WriteLine(namaFile + " " + pes);
                            log = "[" + DateTime.Now.ToString("HH:MM:ss") + "]  " + namaFile + " " + pes;
                            updateLog(log);

                            if (has[0] == "failed")
                            {
                                fman.moveFile(i, error);
                            }
                            else
                            {
                                fman.moveFile(i, succcess);
                            }
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }

                next = DateTime.Now.AddSeconds(schedInt);
                txtNext.Text = jamText(next.Hour.ToString() + ":" + next.Minute.ToString() + ":" + next.Second.ToString());
            }
        }

        private void updateLog(string log)
        {
            string lognew = "";
            lognew = log + Environment.NewLine;
            txtLog.Text = lognew + txtLog.Text;
        }

        private string jamText(string jam)
        {
            string jamNew = "";
            string bgJam = "";
            string[] arjam = jam.Split(':');
            foreach(string spJam in arjam)
            {
                if (spJam.Length <= 1)
                {
                    bgJam = "0" + spJam;
                }
                else
                {
                    bgJam = spJam;
                }
                jamNew = jamNew + bgJam + ":";
            }
            jamNew = jamNew.Remove(jamNew.Length - 1);
            return jamNew;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            var ws = new wsClass();
            string pesan = ws.cekLogin("","");
            MessageBox.Show(pesan);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fm = new fileMan();
            

            var ws = new wsClass();
            string[] pesan = fm.getFile(outbox);
            string pes = ws.invokeProd("", "", pesan[0]);
            MessageBox.Show(pes);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var fm = new fileMan();
            //int jum = fm.jumlah("c:\\tes\\MB51Jan-Apr.xlsx", "xlsx");
            //MessageBox.Show(jum.ToString()); 
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnDirInput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog brFolder = new FolderBrowserDialog();
            DialogResult result = brFolder.ShowDialog();
            if (!string.IsNullOrWhiteSpace(brFolder.SelectedPath))
            {
               // txtDirInput.Text = brFolder.SelectedPath;
            }
        }


        private void metroPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            button1.Hide();
            button4.Hide();
            string gel = btnStart.Text;
            
            if (gel == "Start")
            {
                t.Start();
                next=DateTime.Now.AddSeconds(schedInt);
                txtNext.Text = jamText(next.Hour.ToString()+":" + next.Minute.ToString()+":" + next.Second.ToString());
                btnStart.Text = "Stop";
            }
            else
            {
                t.Stop();
                btnStart.Text = "Start";
            }
            
        }

        private void schedullerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pnResend.BringToFront();
        }

        private void showPanel(Panel panelName)
        {
            hidePanel();
            panelName.Show();
        }

        private void hidePanel()
        {
            pnResend.Hide();
            pnHome.Hide();
            pnSetting.Hide();
        }

        private void homeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            pnHome.BringToFront();
        }

        private void settingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            pnSetting.BringToFront();
            reloadini();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            konversi kon = new konversi();
            wsClass ws = new wsClass();
            //string str = ws.cekLogin("adminkomdn", "ediimdn");
            string str = ws.cekLoginUoi("adminuoi", "adminuoi1");

            //string str = kon.strToTxt("5659|9033|2019|9533156626|4|01.01.2019||01.01.2019||996001665|21105542|SODIUM ISETHIONATE AQUEOUS SOLUTION 57%|-14.076.900|261|KG|01.01.2019|AMRULLA_Y500|T73A||03:11:33|||||");
            MessageBox.Show(str);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            var fm = new fileMan();
            List<string> jum = fm.jumlah("c:\\tes\\xls\\tes.xlsx", "xlsx");
            jum.ForEach(el => Console.WriteLine(el));
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var fm = new fileMan();
            fm.xlsToTxt("c:\\tes\\xls\\satu.xlsx", "xlsx");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string e = "";
            var fm = new fileMan();
            var ws = new wsClass();
            string[] files = fm.getFile(outbox);
            string pes = "";
            string[] fName = null;
            if (files != null && files.Length>0)
            {
                foreach (string i in files)
                {
                    try
                    {
                        pes = ws.invokeProd("", "", i);
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine(ex);
                    }
                    //string[] has = pes.Split('|');
                    //fName = i.Split('\\');
                    string namaFile = "";// fName[fName.Length - 1];
                    System.Console.WriteLine(namaFile + " " + pes);
                    //if (has[0] == "failed")
                    //{
                    //    fm.moveFile(i, "c:\\tes\\error\\");
                    //}
                    //else
                    //{
                    //    fm.moveFile(i, "c:\\tes\\success\\");
                    //}
                }
            }
        }

        private void pnSetting_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = fbd.SelectedPath;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = fbd.SelectedPath;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox4.Text = fbd.SelectedPath;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            reloadini();
            pnHome.BringToFront();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            textBox6.Text = "3306";
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back;
        }

        private void btnStrt_Click(object sender, EventArgs e)
        {
            string gel = btnStrt.Text;
            if(gel == "Start")
            {
                btnStrt.Text = "Stop";
                lblPause.Text = "Running";
            }
            else
            {
                btnStrt.Text = "Start";
                lblPause.Text = "Pause";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            writeinidir("Inbox", textBox1.Text);
            writeinidir("Outbox", textBox2.Text);
            writeinidir("Success", textBox3.Text);
            writeinidir("Error", textBox4.Text);

            writeinimysql("IP", textBox5.Text);
            writeinimysql("Port", textBox6.Text);
            writeinimysql("Username", textBox7.Text);
            writeinimysql("Password", textBox8.Text);
            writeinimysql("Database", textBox9.Text);

            writeinitimer("SThh", listBox1.Text);
            writeinitimer("STmm", listBox2.Text);
            writeinitimer("STss", listBox3.Text);
            writeinitimer("EThh", listBox6.Text);
            writeinitimer("ETmm", listBox5.Text);
            writeinitimer("ETss", listBox4.Text);
            writeinitimer("Interval", textBox10.Text);
            if(rdoH.Checked)
            {
                writeinitimer("RButtonH", "true");
                writeinitimer("RButtonM", "false");
                writeinitimer("RButtonS", "false");
            }
            else if(rdoM.Checked)
            {
                writeinitimer("RButtonH", "false");
                writeinitimer("RButtonM", "true");
                writeinitimer("RButtonS", "false");
            }
            else if(rdoS.Checked)
            {
                writeinitimer("RButtonH", "false");
                writeinitimer("RButtonM", "false");
                writeinitimer("RButtonS", "true");
            }
            writeinitimer("Date", dateTimePicker1.Text);

            if(checkBox1.Checked)
            {
                writeinicustome("BC23", "1");
            }
            else
            {
                writeinicustome("BC23", "0");
            }
            if (checkBox2.Checked)
            {
                writeinicustome("BC25", "1");
            }
            else
            {
                writeinicustome("BC25", "0");
            }
            if (checkBox3.Checked)
            {
                writeinicustome("BC27", "1");
            }
            else
            {
                writeinicustome("BC27", "0");
            }
            if (checkBox4.Checked)
            {
                writeinicustome("BC40", "1");
            }
            else
            {
                writeinicustome("BC40", "0");
            }
            if (checkBox5.Checked)
            {
                writeinicustome("BC41", "1");
            }
            else
            {
                writeinicustome("BC41", "0");
            }
            if (checkBox6.Checked)
            {
                writeinicustome("BC261", "1");
            }
            else
            {
                writeinicustome("BC261", "0");
            }
            if (checkBox7.Checked)
            {
                writeinicustome("BC262", "1");
            }
            else
            {
                writeinicustome("BC262", "0");
            }
            if (chcUse.Checked)
            {
                writeinicustome("Use Share Folder", "1");
            }
            else
            {
                writeinicustome("Use Share Folder", "0");
            }
            if (chcSave.Checked)
            {
                writeinicustome("Save And Show Notification", "1");
            }
            else
            {
                writeinicustome("Save And Show Notification", "0");
            }
            if (chcAlways.Checked)
            {
                writeinicustome("Always Allow Re - Upload Duplicate Record", "1");
            }
            else
            {
                writeinicustome("Always Allow Re - Upload Duplicate Record", "0");
            }
            if (chcDelete.Checked)
            {
                writeinicustome("Delete Previous Logs Duplicate Record", "1");
            }
            else
            {
                writeinicustome("Delete Previous Logs Duplicate Record", "0");
            }
            if (chcRecord.Checked)
            {
                writeinicustome("Record Logs", "1");
            }
            else
            {
                writeinicustome("Record Logs", "0");
            }

            pnSetting.SendToBack();
            MessageBox.Show("Saved", "Information");
            reloadini();
        }
        private void writeinidir(string title, string set)
        {
            INIFile ini = new INIFile(@"C:\tes\setting\config.ini");
            ini.Write("Direktori", title, set);
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            About a = new About();
            a.ShowDialog();
        }

        private void writeinimysql(string title, string set)
        {
            INIFile ini = new INIFile(@"C:\tes\setting\config.ini");
            ini.Write("Mysql", title, set);
        }

        private void logToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            panel1.BringToFront();
        }

        private void writeinitimer(string title, string set)
        {
            INIFile ini = new INIFile(@"C:\tes\setting\config.ini");
            ini.Write("Timer", title, set);
        }
        private void writeinicustome(string title, string set)
        {
            INIFile ini = new INIFile(@"C:\tes\setting\config.ini");
            ini.Write("Custome", title, set);
        }
        private void reloadini()
        {
            INIFile ini = new INIFile(@"C:\tes\setting\config.ini");
            inbox = ini.Read("Direktori", "Inbox");
            outbox = ini.Read("Direktori", "Outbox");
            succcess = ini.Read("Direktori", "Success");
            error = ini.Read("Direktori", "Error");
            textBox1.Text = inbox;
            textBox2.Text = outbox;
            textBox3.Text = succcess;
            textBox4.Text = error;

            textBox5.Text = ini.Read("Mysql", "IP");
            textBox6.Text = ini.Read("Mysql", "Port");
            textBox7.Text = ini.Read("Mysql", "Username");
            textBox8.Text = ini.Read("Mysql", "Password");
            textBox9.Text = ini.Read("Mysql", "Database");

            listBox1.Text = ini.Read("Timer", "SThh");
            listBox2.Text = ini.Read("Timer", "STmm");
            listBox3.Text = ini.Read("Timer", "STss");
            listBox6.Text = ini.Read("Timer", "EThh");
            listBox5.Text = ini.Read("Timer", "ETmm");
            listBox4.Text = ini.Read("Timer", "ETss");
            textBox10.Text = ini.Read("Timer", "Interval");
            string rbh, rbm, rbs;
            rbh = ini.Read("Timer", "RButtonH");
            rbm = ini.Read("Timer", "RButtonM");
            rbs = ini.Read("Timer", "RButtonS");
            if (rbh == "true")
            {
                rdoH.Checked = true;
            }
            if (rbm == "true")
            {
                rdoM.Checked = true;
            }
            if (rbs == "true")
            {
                rdoS.Checked = true;
            }
            dateTimePicker1.Text = ini.Read("Timer", "Date");

            string a, b, c, d, e, f, g, h, i, j, k, l;
            a = ini.Read("Custome", "BC23");
            b = ini.Read("Custome", "BC25");
            c = ini.Read("Custome", "BC27");
            d = ini.Read("Custome", "BC40");
            e = ini.Read("Custome", "BC41");
            f = ini.Read("Custome", "BC261");
            g = ini.Read("Custome", "BC262");
            h = ini.Read("Custome", "Use Share Folder");
            i = ini.Read("Custome", "Save And Show Notification");
            j = ini.Read("Custome", "Always Allow Re - Upload Duplicate Record");
            k = ini.Read("Custome", "Delete Previous Logs Duplicate Record");
            l = ini.Read("Custome", "Record Logs");
            if (a == "1")
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
            if (b == "1")
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
            if (c == "1")
            {
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
            }
            if (d == "1")
            {
                checkBox4.Checked = true;
            }
            else
            {
                checkBox4.Checked = false;
            }
            if (e == "1")
            {
                checkBox5.Checked = true;
            }
            else
            {
                checkBox5.Checked = false;
            }
            if (f == "1")
            {
                checkBox6.Checked = true;
            }
            else
            {
                checkBox6.Checked = false;
            }
            if (g == "1")
            {
                checkBox7.Checked = true;
            }
            else
            {
                checkBox7.Checked = false;
            }
            if (h == "1")
            {
                chcUse.Checked = true;
            }
            else
            {
                chcUse.Checked = false;
            }
            if (i == "1")
            {
                chcSave.Checked = true;
            }
            else
            {
                chcSave.Checked = false;
            }
            if (j == "1")
            {
                chcAlways.Checked = true;
            }
            else
            {
                chcAlways.Checked = false;
            }
            if (k == "1")
            {
                chcDelete.Checked = true;
            }
            else
            {
                chcDelete.Checked = false;
            }
            if (l == "1")
            {
                chcRecord.Checked = true;
            }
            else
            {
                chcRecord.Checked = false;
            }
        }
    }
}
