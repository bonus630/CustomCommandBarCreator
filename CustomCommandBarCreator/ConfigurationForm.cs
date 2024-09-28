using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProjectHelper;

namespace ConfigurationForm
{
    public partial class ConfigurationForm : Form
    {
        //string solutionFile = "";
        //public ConfigurationForm(string solutionFile)
        //{
        //    InitializeComponent();
        //    InitializeCheckBox();


        //    string text = File.ReadAllText(solutionFile);

        //    string[] p = text.Split(new string[] { Environment.NewLine.ToString() }, StringSplitOptions.RemoveEmptyEntries) ;

        //    Dictionary<string, string> projects = new Dictionary<string, string>();

        //    for (int i  = 0; i < p.Length; i++)
        //    {
        //        if(p[i].StartsWith("Project"))
        //        {
        //            string[] s = p[i].Split(new string[] { ", " } , StringSplitOptions.RemoveEmptyEntries);
        //            //Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SampleButtonCS1fddsa-apagar", "SampleButtonCS1fddsa-apagar\SampleButtonCS1fddsa-apagar.csproj", "{3934EEB9-639E-4A98-BEBE-CDD607297F66}"
        //            projects.Add(s[2], s[1]);
        //        }
        //    }



        //}
        //public ConfigurationForm(string[] config)
        //{
        //    InitializeCheckBox();
        //}
        public List<string> ConfigurationAbs = new List<string>();
        public ConfigurationForm(SolutionConfigurations solutionConfigurations)
        {
            for (int i = CorelVersionInfo.MinVersion; i < CorelVersionInfo.MaxVersion; i++)
            {
                CheckBox temp = new CheckBox();
                temp.Tag = i;
                temp.Text = i + " Configuration";
                for (int j = 0; j < solutionConfigurations.Count; j++)
                {
                    if(solutionConfigurations.Item(j).Name.Contains(i.ToString()))
                    {
                        temp.Checked = true;
                    }
                }
                flowLayoutPanel_Versions.Controls.Add(temp);
            }
        }
        private void InitializeCheckBox()
        {
          
        }
        private void AddCheckBox(string text, int id, bool enabled)
        {
            CheckBox ck = new CheckBox();
            ck.Text = text;
            ck.Tag = id;
            ck.Enabled = true;
            if (enabled)
                ck.ForeColor = Color.Black;
            else
                ck.ForeColor = Color.Red;
            ck.AutoSize = true;
            ck.Click += Ck_Click;
            this.flowLayoutPanel_Versions.Controls.Add(ck);
        }
        private void Ck_Click(object sender, EventArgs e)
        {
            CheckBox ck = sender as CheckBox;
            if (ck.Checked)
                ConfigurationAbs.Add(ck.Tag.ToString());
            else
                ConfigurationAbs.Remove(ck.Tag.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
