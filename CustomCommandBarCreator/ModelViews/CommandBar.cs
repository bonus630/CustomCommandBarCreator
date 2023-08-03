using CustomCommandBarCreator.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;



namespace CustomCommandBarCreator.ModelViews
{
    [Serializable]
    public class CommandBar : ControlItem
    {
        // private bool canGenerate = false;
        
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

        private string name;
        public new string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
                GenerateCommand.RaiseCanExecuteChanged();
            }
        }
        [NonSerialized]
        private string currentGMS;

        public string CurrentGMS
        {
            get { return currentGMS; }
            set
            {
                currentGMS = value;

                OnPropertyChanged();
            }
        }
        [NonSerialized]
        private bool attached;

        public bool Attached
        {
            get { return attached; }
            set { attached = value; OnPropertyChanged(); }
        }
        [NonSerialized]
        private bool isAdmin;

        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; OnPropertyChanged(); }
        }
        [NonSerialized]
        private string version;

        public string Version
        {
            get { return version; }
            set { version = value; OnPropertyChanged(); }
        }
        [NonSerialized]
        private string attachButtonText = "Attach in a CorelDRW";

        public string AttachButtonText
        {
            get { return attachButtonText; }
            set
            {
                attachButtonText = value;
                OnPropertyChanged();
            }
        }
        [NonSerialized]
        private int commandLeft = 100;

        public int CommandLeft
        {
            get { return commandLeft; }
            set { commandLeft = value; OnPropertyChanged(); }
        }
       
        public ObservableCollection<CorelVersionInfo> CorelVersions { get; set; }
        public RelayCommand<CommandBar> GenerateCommand { get; set; }
        public RelayCommand<CommandBar> SaveBarCommand { get; set; }
        public RelayCommand<CommandBar> SaveAsBarCommand { get; set; }
        public RelayCommand<CommandBar> LoadBarCommand { get; set; }
        public RelayCommand<CommandBar> NewBarCommand { get; set; }
        public RelayCommand<string> AddFileCommand { get; set; }
        public RelayCommand<CommandItem> AddCommandItemCommand { get; set; }
        public RelayCommand<string> RemoveFileCommand { get; set; }
        public RelayCommand<CommandItem> RemoveCommandItemCommand { get; set; }
        public RelayCommand<CommandItem> AddIconCommand { get; set; }
        public RelayCommand<object> AttachCorelDRWCommand { get; set; }
        public RelayCommand<object> SendToCommand { get; set; }
        public RelayCommand<string> SetCommand { get; set; }
        public RelayCommand<string> LinkCommand { get; set; }
        public RelayCommand<CorelVersionInfo> InstallCommand { get; set; }



        public CommandBar() : base()
        {

            GenerateCommand = new RelayCommand<CommandBar>(GenereteBar, CanGenereteBar);
            SaveBarCommand = new RelayCommand<CommandBar>(SaveBar);
            SaveAsBarCommand = new RelayCommand<CommandBar>(SaveAsBar);
            LoadBarCommand = new RelayCommand<CommandBar>(LoadBar);
            NewBarCommand = new RelayCommand<CommandBar>(NewBar);

            AddFileCommand = new RelayCommand<string>(AddFile);
            AddCommandItemCommand = new RelayCommand<CommandItem>(AddCommandItem, CanAddCommandItem);
            RemoveFileCommand = new RelayCommand<string>(RemoveFile);
            RemoveCommandItemCommand = new RelayCommand<CommandItem>(RemoveCommandItem);
            AddIconCommand = new RelayCommand<CommandItem>(AddIcon);
            AttachCorelDRWCommand = new RelayCommand<object>(AttachCorelDRW);
            SendToCommand = new RelayCommand<object>(SendTo);
            SetCommand = new RelayCommand<string>(CurrentGMSSelect);
            LinkCommand = new RelayCommand<string>(LinkGMSSelect);
            InstallCommand = new RelayCommand<CorelVersionInfo>(Install,CanInstall);


            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            IsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            FillCorelVersion();
        }
        private void FillCorelVersion()
        {
            this.CorelVersions = new ObservableCollection<CorelVersionInfo>();
            for (int i = CorelVersionInfo.MinVersion; i < CorelVersionInfo.MaxVersion; i++)
            {
                CorelVersionInfo cvi = new CorelVersionInfo(i);
                if (!cvi.CorelInstallationNotFound)
                    this.CorelVersions.Add(cvi);
            }
            OnPropertyChanged("CorelVersions");

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
                {
                    this[i].Commands = result;
                    this[i].Command = result[0];
                    this[i].GmsPath = obj.Substring(obj.LastIndexOf("\\") + 1).Split('.')[0];
                }
            }
            GenerateCommand.RaiseCanExecuteChanged();
        }
        private void LinkGMSSelect(string obj)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Selected)
                {
                    this[i].GmsPath = obj.Substring(obj.LastIndexOf("\\") + 1).Split('.')[0];
                }
            }
            GenerateCommand.RaiseCanExecuteChanged();
        }
        [NonSerialized]
        object app = null;
        [NonSerialized]
        private int cdrVersion;
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
                Thread thread = new Thread(() =>
                {
                    AttachButtonText = "Please Wait!";
                    Type pia_type = Type.GetTypeFromProgID("CorelDRAW.Application");
                    app = Activator.CreateInstance(pia_type);
                    if (app != null)
                    {

                        this.Attached = true;
                        cdrVersion = (app as dynamic).VersionMajor;
                        Version = string.Format("{0} {1}", (app as dynamic).Name, (app as dynamic).Version);
                        AttachButtonText = "Deattach";
                    }
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }
        public void SendTo(object obj)
        {
            if (attached)
            {
                string addonPath = (app as dynamic).AddonPath;
                if (!this.resultFolder.Equals(string.Empty))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(this.resultFolder);
                        DirectoryInfo addonDir = Directory.CreateDirectory(Path.Combine(addonPath, di.Name));

                        FileInfo[] files = di.GetFiles();

                        for (int i = 0; i < files.Length; i++)
                        {
                            files[i].CopyTo(Path.Combine(addonDir.FullName, files[i].Name), true);
                        }

                    }
                    catch { }
                }
            }
        }

        private bool CanGenereteBar(CommandBar bar)
        {
            if (string.IsNullOrEmpty(Name) || this.Count == 0)
            {
                return false;

            }
            return !this.commandItems.Any(r => r.IsOk == false);
        }

        private void AddFile(string obj)
        {
            string[] files = GetFilePath(FileType.GMS);
            if (files == null)
                return;
            try
            {
                Properties.Settings.Default.LastGMSFolder = files[0].Substring(0, files[0].LastIndexOf("\\"));
                Properties.Settings.Default.Save();
            }
            catch
            {

            }
            for (int i = 0; i < files.Length; i++)
            {
                CheckAndAddGmsFile(files[i]);
            }
        }
        public void CheckAndAddGmsFile(string path)
        {
            if (File.Exists(path) && !this.gmsPaths.Contains(path))
                this.GmsPaths.Add(path);
        }
        public void AddCommandItem(CommandItem item)
        {
            if (item == null)
                item = new CommandItem();
            this.CommandItems.Add(item);
            this.CommandLeft--;
            AddCommandItemCommand.RaiseCanExecuteChanged();
            GenerateCommand.RaiseCanExecuteChanged();
        }
        private bool CanAddCommandItem(CommandItem item)
        {
            return this.CommandLeft > 0;
        }
        private void RemoveFile(string file)
        {
            this.GmsPaths.Remove(file);
            for (int i = 0; i < this.Count; i++)
            {

                if (this[i].GmsPath.Equals(file.Substring(file.LastIndexOf("\\") + 1).Split('.')[0]))
                    this[i].GmsPath = String.Empty;

            }
            OnPropertyChanged(nameof(GmsPaths));
            GenerateCommand.RaiseCanExecuteChanged();
        }
        private void RemoveCommandItem(CommandItem item)
        {
            this.CommandItems.Remove(item);
            this.CommandLeft++;
            AddCommandItemCommand.RaiseCanExecuteChanged();
        }
        private void AddIcon(CommandItem item)
        {
            object o = GetFilePath(FileType.ICON);
            if (o == null)
                return;
            string iconPath = (o as string[])[0];
            FileInfo fi = new FileInfo(iconPath);
            if (!fi.Extension.Equals(".ico"))
            {
                iconPath = this.ContertToIcon(iconPath);
            }
            if (File.Exists(iconPath))
                item.IconPath = iconPath;
        }
        private string resultFolder = string.Empty;
        private void GenereteBar(CommandBar bar)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (!string.IsNullOrEmpty(this[i].ShortcutText))
                {
                    this.HaveShortcut = true;
                    break;
                }
            }
            StructureGenerator generator = new StructureGenerator();
            if (generator.CreateBar(bar))
            {

                resultFolder = generator.Folder;
                if(cdrVersion >= CorelVersionInfo.MinVersion)
                    generator.CreateDataSource(resultFolder, cdrVersion);
                else
                {
                    System.Windows.MessageBox.Show("DataSource is not created, please use install button in administrator level and select your bar folder to makes a correct installation","Attention!"
                        ,System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Warning);
                }
            }
            else
                resultFolder = string.Empty;
        }
        private string filePath = string.Empty;
        private void SaveBar(CommandBar bar)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Bar File (*.bar)|*.bar";
                saveFileDialog.DefaultExt = ".bar";
                saveFileDialog.Title = "Save your Bar Project";
                if ((bool)saveFileDialog.ShowDialog())
                {
                    string path = saveFileDialog.FileName;
                    if (Serializer.Serialize(bar, path))
                    {
                        filePath = path;
                        Dirty = false;
                    }
                }
            }
            else
            {
                if (Serializer.Serialize(bar, filePath))
                    Dirty = false;
            }
        }
        private void SaveAsBar(CommandBar bar)
        {
            filePath = string.Empty;
            SaveBar(bar);
        }
        private void LoadBar(CommandBar bar)
        {
            string[] path = GetFilePath(FileType.BAR);
            if (path == null)
                return;
            NewBar(bar);
            Serializer.DeSerialize(this, path[0]);
            Dirty = false;

        }
        private void NewBar(CommandBar bar)
        {
            this.Name = String.Empty;
            this.GmsPaths.Clear();
            this.commandItems.Clear();
            this.HaveShortcut = false;
            this.CommandLeft = 100;
            filePath = string.Empty;
            Dirty = false;
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
                    filter = "Images or Icon|*.ico; *.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff";
                    title = "Select your icon!";
                    multiselect = false;
                    break;
                case FileType.GMS:
                    filter = "GMS Files (*.gms)|*.gms";
                    title = "Select your GMS files!";
                    try
                    {
                        startFolder = Properties.Settings.Default.LastGMSFolder;
                    }
                    catch { }
                    multiselect = true;
                    break;
                case FileType.BAR:
                    filter = "Bar File (*.bar)|*.bar";
                    title = "Select a .bar file!";
                    multiselect = false;
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
        private void Install(CorelVersionInfo version)
        {
            if (version == null)
                return;
            try
            {
            
                StructureGenerator generator = new StructureGenerator();
                resultFolder = generator.SelectBarFolder();

                if (!this.resultFolder.Equals(string.Empty))
                {
                    try
                    {
                        generator.BuildDataSourceFinish += () =>
                        {
                            string addonPath = "";
                            if (version.Corel64Bit == CorelVersionInfo.CorelIs64Bit.Corel32)
                                version.CorelAddonsPath(out addonPath);
                            else
                                version.CorelAddonsPath64(out addonPath);
                            DirectoryInfo di = new DirectoryInfo(this.resultFolder);
                            DirectoryInfo addonDir = Directory.CreateDirectory(Path.Combine(addonPath, di.Name));

                            FileInfo[] files = di.GetFiles();

                            for (int i = 0; i < files.Length; i++)
                            {
                                files[i].CopyTo(Path.Combine(addonDir.FullName, files[i].Name), true);
                            }
                           
                        };
                        generator.CreateDataSource(resultFolder, version.CorelVersion);


                    }
                    catch {
                     
                    }
                }


            }
            catch
            {

            }
        }
        [NonSerialized]
        private bool canInstall = true;
        private bool CanInstall(CorelVersionInfo version)
        {
            return canInstall;
        }

        public string ContertToIcon(string imagePath)
        {
            string iconPath = Path.GetTempFileName();
            iconPath = iconPath.Replace(".tmp", ".ico");
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                Icon icon = Icon.FromHandle(bitmap.GetHicon());
                using (System.IO.FileStream stream = new System.IO.FileStream(iconPath, System.IO.FileMode.Create))
                {
                    icon.Save(stream);
                }
            }
            return iconPath;
        }
    }
    enum FileType
    {
        ICON,
        GMS,
        BAR
    }

}
