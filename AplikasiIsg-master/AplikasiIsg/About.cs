﻿using System;
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
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 0.90)
            {
                this.Opacity += 0.01;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.01;
            }
            else
            {
                timer2.Enabled = false;
                this.Close();
            }
        }

        private void About_MouseClick(object sender, MouseEventArgs e)
        {
            timer2.Start();
            timer1.Enabled = false;
        }
    }
}
