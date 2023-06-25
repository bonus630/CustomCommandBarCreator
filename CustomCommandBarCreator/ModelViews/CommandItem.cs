using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using CustomCommandBarCreator.Models;
using System.IO;

namespace CustomCommandBarCreator.ModelViews
{
    public class CommandItem : ControlItem
    {
		private string caption;

		public string Caption
		{
			get { return caption; }
			set { caption = value;
				OnPropertyChanged();
			}
		}


		private string command;

		public string Command
		{
			get { return command; }
			set { command = value;
				OnPropertyChanged();
			}
		}

		private string enableCondition = "true";

		public string EnableCondition
		{
			get { return enableCondition; }
			set { enableCondition = value;
				OnPropertyChanged();
			}
		}

		private ImageSource icon;

		public ImageSource Icon
		{
			get { return icon; }
			set { icon = value;
				OnPropertyChanged();
			}
		}

		private string iconPath;
        public string IconPath { get { return iconPath; } 
			set { 
				iconPath = value; 
				
				this.Icon = (new System.Drawing.Icon(iconPath).ToImageSource());	
			} }
		private Shortcut shortcut;

		public Shortcut Shortcut
		{
			get { return shortcut; }
			set { shortcut = value;
				OnPropertyChanged();
			}
		}


        public IntPtr IconID { get; set; }

        public CommandItem():base()
        {
			this.Shortcut = new Shortcut();

			string tempIconPath = Path.GetTempFileName();

            using (FileStream fs = new FileStream(tempIconPath, FileMode.OpenOrCreate))
			{
				Properties.Resources.IconGroup104.Save(fs);
			}
			this.IconPath = tempIconPath;
        }
        public CommandItem(string caption, string command,string iconPath, string enableCondition = "true"):this()
        {
			this.Caption = caption;
			this.Command = command;
			this.IconPath = iconPath;
			this.Shortcut = shortcut;
			this.EnableCondition = enableCondition;
           
        }

    }
    
}
