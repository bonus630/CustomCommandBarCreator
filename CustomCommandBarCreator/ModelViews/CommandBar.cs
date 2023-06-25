using CustomCommandBarCreator.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        public RelayCommand<CommandBar> GenerateCommand { get; set; }
        public RelayCommand<string> AddFileCommand { get; set; }
        public RelayCommand<CommandItem> AddCommandItemCommand { get; set; }
        public RelayCommand<string> RemoveFileCommand { get; set; }
        public RelayCommand<CommandItem> RemoveCommandItemCommand { get; set; }
        public RelayCommand<CommandItem> AddIconCommand { get; set; }

        public CommandBar():base()
        {

            GenerateCommand = new RelayCommand<CommandBar>(GenereteBar,CanGenereteBar);
            AddFileCommand = new RelayCommand<string>(AddFile);
            AddCommandItemCommand = new RelayCommand<CommandItem>(AddCommandItem);
            RemoveFileCommand = new RelayCommand<string>(RemoveFile);
            RemoveCommandItemCommand = new RelayCommand<CommandItem>(RemoveCommandItem);
            AddIconCommand = new RelayCommand<CommandItem>(AddIcon);
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
            if(File.Exists(iconPath))
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

        public CommandItem this[int index] { get { return commandItems[index]; }
            set { commandItems[index] = value; } }

        public int Count { get => commandItems.Count; }
        public bool HaveShortcut { get; protected set; }

        private string[] GetFilePath(FileType type)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            string filter = "";
            string title = "";
            bool multiselect = false;
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
                    multiselect = true;
                    break;
            }
            ofd.Filter = filter;
            ofd.Title = title;
            ofd.Multiselect = multiselect;
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
