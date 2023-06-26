﻿using CustomCommandBarCreator.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CustomCommandBarCreator.ModelViews
{
    public class CommandBar : ControlItem
    {
        private ObservableCollection<string> gmsPaths;

        public ObservableCollection<string> GmsPaths
        {
            get { return gmsPaths; }
            set
            {
                gmsPaths = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CommandItem> commandItems;

        public ObservableCollection<CommandItem> CommandItems
        {
            get { return commandItems; }
            set
            {
                commandItems = value;
                OnPropertyChanged();
            }
        }

        private string currentGMS;

        public string CurrentGMS
        {
            get { return currentGMS; }
            set
            {
                currentGMS = value;
                CurrentGMSSelect(value);
                OnPropertyChanged();
            }
        }

        private bool attached;

        public bool Attached
        {
            get { return attached; }
            set { attached = value; OnPropertyChanged(); }
        }

        private string version;

        public string Version
        {
            get { return version; }
            set { version = value; OnPropertyChanged(); }
        }

        private string attachButtonText = "Attach in a CorelDRW";

        public string AttachButtonText
        {
            get { return attachButtonText; }
            set { attachButtonText = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand<CommandBar> GenerateCommand { get; set; }
        public RelayCommand<string> AddFileCommand { get; set; }
        public RelayCommand<CommandItem> AddCommandItemCommand { get; set; }
        public RelayCommand<string> RemoveFileCommand { get; set; }
        public RelayCommand<CommandItem> RemoveCommandItemCommand { get; set; }
        public RelayCommand<CommandItem> AddIconCommand { get; set; }
        public RelayCommand<object> AttachCorelDRWCommand { get; set; }



        public CommandBar() : base()
        {

            GenerateCommand = new RelayCommand<CommandBar>(GenereteBar, CanGenereteBar);
            AddFileCommand = new RelayCommand<string>(AddFile);
            AddCommandItemCommand = new RelayCommand<CommandItem>(AddCommandItem);
            RemoveFileCommand = new RelayCommand<string>(RemoveFile);
            RemoveCommandItemCommand = new RelayCommand<CommandItem>(RemoveCommandItem);
            AddIconCommand = new RelayCommand<CommandItem>(AddIcon);
            AttachCorelDRWCommand = new RelayCommand<object>(AttachCorelDRW);

        }

        private void CurrentGMSSelect(string obj)
        {


            if (!attached)
                return;
            dynamic corelApp = app as dynamic;
            if (!corelApp.InitializeVBA())
                return;
            ObservableCollection<string> result = new ObservableCollection<string>();
            dynamic gmp = corelApp.GMSManager.Projects.Load(obj);
            dynamic macros = gmp.Macros;

            for (int r = 1; r <= macros.Count; r++)
            {
                string name = string.Format("{0}.{1}", gmp.Name, macros[r].Name);

                result.Add(name);
            }
            gmp.Unload();



            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Selected)
                    this[i].Commands = result;
            }
        }
        object app = null;
        private void AttachCorelDRW(object obj)
        {
            if (attached)
            {
                try
                {
                    (app as dynamic).Quit();
                    Marshal.ReleaseComObject(app);
                    app = null;
                    this.Attached = false;
                    AttachButtonText = "Attach in a CorelDRW";
                }
                catch { }
            }
            else
            {
                Type pia_type = Type.GetTypeFromProgID("CorelDRAW.Application");
                app = Activator.CreateInstance(pia_type);
                if (app != null)
                {
                    this.Attached = true;
                    Version = string.Format("{0} {1}", (app as dynamic).Name, (app as dynamic).Version);
                    AttachButtonText = "Deattach";
                }
            }
        }

        private bool CanGenereteBar(CommandBar bar)
        {
            if (!string.IsNullOrEmpty(bar.Name))
                return true;
            return true;
        }

        private void AddFile(string obj)
        {
            string[] files = GetFilePath(FileType.GMS);
            if (files == null)
                return;
            Properties.Settings.Default.LastGMSFolder = files[0].Substring(0, files[0].LastIndexOf("\\"));
            Properties.Settings.Default.Save();
            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]) && !this.gmsPaths.Contains(files[i]))
                    this.GmsPaths.Add(files[i]);
            }
        }
        private void AddCommandItem(CommandItem item)
        {
            this.CommandItems.Add(new CommandItem());
        }

        private void RemoveFile(string file)
        {
            this.GmsPaths.Remove(file);
            OnPropertyChanged(nameof(GmsPaths));
        }
        private void RemoveCommandItem(CommandItem item)
        {
            this.CommandItems.Remove(item);
        }
        private void AddIcon(CommandItem item)
        {
            string iconPath = GetFilePath(FileType.ICON)[0];
            if (File.Exists(iconPath))
                item.IconPath = iconPath;
        }
        private void GenereteBar(CommandBar bar)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (!string.IsNullOrEmpty(this[i].Shortcut.Key))
                {
                    this.HaveShortcut = true;
                    break;
                }
            }
            StructureGenerator generator = new StructureGenerator();
            generator.CreateBar(bar);
        }
        public void AddCommandItem(CommandItem command, string gmsPath)
        {
            if (!GmsPaths.Contains(gmsPath))
                GmsPaths.Add(gmsPath);
            CommandItems.Add(command);
        }

        public CommandItem this[int index]
        {
            get { return commandItems[index]; }
            set { commandItems[index] = value; }
        }

        public int Count { get => commandItems.Count; }
        public bool HaveShortcut { get; protected set; }

        private string[] GetFilePath(FileType type)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            string filter = "";
            string title = "";
            bool multiselect = false;
            string startFolder = "";
            switch (type)
            {
                case FileType.ICON:
                    filter = "Icon (*.ico)|*.ico";
                    title = "Select your icon!";
                    multiselect = false;
                    break;
                case FileType.GMS:
                    filter = "GMS Files (*.gms)|*.gms";
                    title = "Select your GMS files!";
                    startFolder = Properties.Settings.Default.LastGMSFolder;
                    multiselect = true;
                    break;
            }
            ofd.Filter = filter;
            ofd.Title = title;
            ofd.Multiselect = multiselect;
            ofd.InitialDirectory = startFolder;
            if ((bool)ofd.ShowDialog())
            {
                return ofd.FileNames;
            }
            return null;

        }









    }
    enum FileType
    {
        ICON,
        GMS
    }

}
