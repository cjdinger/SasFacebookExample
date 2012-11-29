﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAS.FbFriends
{
    /// <summary>
    /// A simple dialog to show some disclaimer
    /// information and acknowledge that we used
    /// the Facebook Developer Toolkit
    /// </summary>
    public partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void linkCodeplex_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkCodeplex.Text);
        }
    }
}
