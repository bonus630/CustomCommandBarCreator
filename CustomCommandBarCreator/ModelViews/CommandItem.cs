using CustomCommandBarCreator.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

namespace CustomCommandBarCreator.ModelViews
{
    public class CommandItem : ControlItem
    {
        private string caption;

        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                OnPropertyChanged();
            }
        }
        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; OnPropertyChanged(); }
        }


        private string command = String.Empty;

        public string Command
        {
            get { return command; }
            set
            {
                command = value;
                this.IsOk = true;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> commands = new ObservableCollection<string>() { ""};

        public ObservableCollection<string> Commands
        {
            get { return commands; }
            set
            {
                commands = value;
                
                OnPropertyChanged();
            }
        }
        private string enableCondition = "true";

        public string EnableCondition
        {
            get { return enableCondition; }
            set
            {
                enableCondition = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> enableConditions =
            new ObservableCollection<string>(){
                "true",
                "false",
                "*Bind(DataSource=AppDS;Path=DocumentAvailable)",
                "*Bind(DataSource=WPageDataSource;Path=CanInsertPage)",
                "*GtrI(*Bind(DataSource=DrawingInfoDS;Path=NumObjects), 0)",
                "*Bind(DataSource=WViewDataSource;Path=ToggleViewEnabled)"};

        public ObservableCollection<string> EnableConditions
        {
            get { return enableConditions; }
            set
            {
                enableConditions = value;
                OnPropertyChanged();
            }
        }

        private ImageSource icon;

        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                OnPropertyChanged();
            }
        }

        private string iconPath;
        public string IconPath
        {
            get { return iconPath; }
            set
            {
                iconPath = value;

                try
                {
                    this.Icon = (new System.Drawing.Icon(iconPath).ToImageSource());
                }
                catch 
                {
                    System.Windows.MessageBox.Show("Unable to process this image, please select another one!");
                }
            }
        }
        private Shortcut[] shortcuts;

        public Shortcut[] Shortcuts
        {
            get { GenerateShortcuts(); return shortcuts; }
           protected set
            {
                shortcuts = value;
               
            }
        }

        private string shortcutText;

        public string ShortcutText
        {
            get { 
                return shortcutText; }
            set
            {
                shortcutText = value;
                OnPropertyChanged();
               
            }
        }
        private string gmsPath = String.Empty;

        public string GmsPath
        {
            get { return gmsPath; }
            set
            {
                gmsPath = value;
                this.IsOk = true;
                OnPropertyChanged();
            }
        }

        private bool isOk = false;

        public bool IsOk
        {
            get { return this.isOk; }
            protected set
            {
                this.isOk = (!String.IsNullOrEmpty(this.Command) && !String.IsNullOrEmpty(this.GmsPath));
                OnPropertyChanged();
            }
        }


        public IntPtr IconID { get; set; }

        public CommandItem() : base()
        {
           

            string tempIconPath = Path.GetTempFileName();
            this.Selected = true;
         

            using (FileStream fs = new FileStream(tempIconPath, FileMode.OpenOrCreate))
            {
                Properties.Resources.IconGroup104.Save(fs);
            }
            this.IconPath = tempIconPath;
        }
        public CommandItem(string caption, string command, string iconPath, string enableCondition = "true") : this()
        {
            this.Caption = caption;
            this.Command = command;
            this.IconPath = iconPath;
          
            this.EnableCondition = enableCondition;

        }
        private void GenerateShortcuts()
        {
            //ctrl+shift+alt+
            string[] pes = shortcutText.Split(new char[] {',' }, StringSplitOptions.RemoveEmptyEntries);
            shortcuts = new Shortcut[pes.Length];
            for (int i = 0; i < pes.Length; i++)
            {
                Shortcut s = new Shortcut();
                if (pes[i].Contains("ctrl+"))
                {
                    s.Control = true;
                    pes[i]= pes[i].Replace("ctrl+", "");
                }
                if (pes[i].Contains("shift+"))
                {
                    s.Shift = true;
                    pes[i]=pes[i].Replace("shift+", "");
                }
                if (pes[i].Contains("alt+"))
                {
                    s.Alt = true;
                    pes[i]=pes[i].Replace("alt+", "");
                }
                s.Key = pes[i].Trim();
                shortcuts[i] = s;
            }
        }

    }

}
