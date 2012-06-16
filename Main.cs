using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Ionic.Zip;

namespace thememan
{
    public partial class XTM : Form
    {
        public string appdata = (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\X-Chat 2\\");
        public string home = (Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/.xchat2/");

        public string xchatdir;
        public string themedir = "themes\\";

        public XTM ()
		{
			InitializeComponent ();
            
			if (File.Exists ("portable-mode"))
				xchatdir = ("config\\");
			else if (Directory.Exists (appdata))
				xchatdir = (appdata);
			else if (Directory.Exists (home)) {
				xchatdir = (home); themedir = "themes/";
			} else
				Console.WriteLine("Install not found");

            ShowFiles();
        }

        private void ShowFiles()
        {
            if (System.IO.Directory.Exists(xchatdir + themedir))
                foreach (string theme in Directory.GetDirectories(xchatdir + themedir))
                {
                    themelist.Items.Add(theme.Remove(0, xchatdir.Length + themedir.Length));
                }
            else
                Directory.CreateDirectory(xchatdir + themedir);
        }

        private void ShowColors(List<List<string>> themecolors)
        {
            List<Control> labels = this.Controls.OfType<Label>().Cast<Control>().OrderBy(label => label.Name).ToList();
            for (byte themecolor = 0; themecolor < themecolors.Count; themecolor++)
            {
                byte rval = Convert.ToByte(int.Parse(themecolors[themecolor][0].ToString(), System.Globalization.NumberStyles.HexNumber) / 257);
                byte gval = Convert.ToByte(int.Parse(themecolors[themecolor][1].ToString(), System.Globalization.NumberStyles.HexNumber) / 257);
                byte bval = Convert.ToByte(int.Parse(themecolors[themecolor][2].ToString(), System.Globalization.NumberStyles.HexNumber) / 257);
                
                if (themecolor <= 15)
                    labels[themecolor].BackColor = Color.FromArgb(rval, gval, bval);
                else if (themecolor == 16)
                    themecolorfgmarked.ForeColor = Color.FromArgb(rval, gval, bval);
                else if (themecolor == 17)
                    themecolorfgmarked.BackColor = Color.FromArgb(rval, gval, bval);
                else if (themecolor == 18)
                    themecolorfg.ForeColor = Color.FromArgb(rval, gval, bval);
                else if (themecolor == 19)
                {
                    themecolortextbg.BackColor = Color.FromArgb(rval, gval, bval);
                    themecolorfg.BackColor = themecolortextbg.BackColor;
                }
            }
        }

        private List<List<string>> ReadTheme(string theme)
        {
            List<List<string>> themecolors = new List<List<string>>();
            foreach (string line in File.ReadLines(xchatdir + themedir + theme + "/colors.conf"))
            {
                List<string> colors = new List<string>();
                List<string> colorlist = new List<string>();
                string[] possiblecolors = { "color_256", "color_257", "color_258", "color_259" };

                for (byte num = 16; num <=31; num++)
                    colorlist.Add("color_" + num);
                colorlist.AddRange(possiblecolors);

                string[] config = line.Split(new char[] { ' ' });
                if(colorlist.Contains(config[0]) == true)
                {
                    colors.Add(config[2]);
                    colors.Add(config[3]);
                    colors.Add(config[4]);
                    themecolors.Add(colors);
                }
            }
            return themecolors;
        }

        private void applybutton_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("XChat must be closed and this will overwrite your current theme, Continue?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            File.Copy(xchatdir + themedir + themelist.SelectedItem.ToString() + "\\colors.conf", xchatdir + "colors.conf", true);
            if (File.Exists(xchatdir + themedir + themelist.SelectedItem.ToString() + "\\pevents.conf"))
            {
                File.Copy(xchatdir + themedir + themelist.SelectedItem.ToString() + "\\pevents.conf", xchatdir + "pevents.conf", true);
            }
        }

        private void theme_selected(object sender, EventArgs e)
        {
            ShowColors(ReadTheme(themelist.SelectedItem.ToString()));
        }

        private void importbutton_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog importdialog = new OpenFileDialog();
            importdialog.Filter = "Zip Files (.zip)|*.zip";
            importdialog.FilterIndex = 1;
            importdialog.ShowDialog();
            try
            {
                ZipFile zip = ZipFile.Read(importdialog.FileName);
                zip.ExtractAll(xchatdir + themedir);
                ShowFiles();
            }
            catch{ } //not the best fix
        }
    }
}
