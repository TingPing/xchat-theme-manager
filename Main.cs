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
using Microsoft.VisualBasic;
using System.Net;

namespace thememan
{
    public partial class XTM : Form
    {
        public XTM()
        {
            InitializeComponent();
            ShowFiles();
        }

        //TODO
        //find folder per os
        //clean up folder strings throughout
        public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string xchatdir = "\\X-Chat 2\\";
        public string themedir = "themes\\";

        private void ShowFiles()
        {
            if (System.IO.Directory.Exists(appdata + xchatdir))
                foreach (string theme in Directory.GetDirectories(appdata + xchatdir + themedir))
                {
                    themelist.Items.Add(theme.Remove(0, appdata.Length + xchatdir.Length + themedir.Length));
                }
            else
                FirstRun();
        }

        private void ShowColors(List<List<string>> themecolors)
        {
            List<Control> labels = this.Controls.OfType<Label>().Cast<Control>().OrderBy(label => label.Name).ToList();
            for (byte themecolor = 0; themecolor < themecolors.Count; themecolor++)
            {
                //TODO
                //possibly clean up?
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
            foreach (string line in File.ReadLines(appdata + xchatdir + themedir + theme + "/colors.conf"))
            {
                List<string> colors = new List<string>();
                List<string> colorlist = new List<string>();
                string[] possiblecolors = { "color_256", "color_257", "color_258", "color_259" };

                //TODO
                //button/menu to alternate all colors?
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

        private void savebutton_Click(object sender, EventArgs e)
        {
            string fulldir = appdata + xchatdir + themedir;
            string themename = (Microsoft.VisualBasic.Interaction.InputBox("Save current theme as:", "Save Prompt", ""));
            if (themename != null && themename != "")
                try
                {
                    System.IO.Directory.CreateDirectory(appdata + xchatdir + themedir + themename);
                    System.IO.File.Copy(appdata + xchatdir + "colors.conf", fulldir + themename + "\\colors.conf", true);
                    try
                    {
                        if (System.IO.File.Equals(appdata + xchatdir + "pevents.conf", fulldir + "original\\pevents.conf") == false)
                        {
                            System.IO.File.Copy(appdata + xchatdir + "pevents.conf", fulldir + themename + "\\pevents.conf", true);
                        }
                    }
                    catch (FileNotFoundException err)
                    {
                        System.IO.File.Copy(appdata + xchatdir + "pevents.conf", fulldir + themename + "\\pevents.conf", true);
                    }
                }
                catch (FileNotFoundException err)
                {
                    Console.WriteLine(err);
                }
                catch (DirectoryNotFoundException err2)
                {
                    Console.WriteLine(err2);
                }
        }

        private void loadbutton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This will overwrite current theme, Continue?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            File.Copy(appdata + xchatdir + themedir + themelist.SelectedItem.ToString() + "\\colors.conf", appdata + xchatdir + "colors.conf", true);
            if (File.Exists(appdata + xchatdir + themedir + themelist.SelectedItem.ToString() + "\\pevents.conf"))
            {
                File.Copy(appdata + xchatdir + themedir + themelist.SelectedItem.ToString() + "\\pevents.conf", appdata + xchatdir + "pevents.conf", true);
            }
        }

        private void theme_selected(object sender, EventArgs e)
        {
            ShowColors(ReadTheme(themelist.SelectedItem.ToString()));
        }

        private void FirstRun()
        {
            string themefolder = appdata + xchatdir + themedir;
            Directory.CreateDirectory(themefolder);
            using (WebClient client = new WebClient())
            {
                Directory.CreateDirectory(themefolder + "Monokai");
                client.DownloadFile("https://raw.github.com/gist/1587237/147f7d916360abae6e44bc559d2f5cf594022dbd/colors.conf", themefolder + "Monokai\\colors.conf");
                client.DownloadFile("https://raw.github.com/gist/1587237/f2c52024d54bd9a4110f8b6aa9a2eeb31e7ac8ca/pevents.conf", themefolder + "Monokai\\pevents.conf");
                //TODO
                //Download zip of multiple themes
                //System.IO.Compression.DeflateStream(themefolder + );
            }
        }

    }
}
