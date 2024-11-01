using CustomCommandBarCreator.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.IconLib;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
//using System.Windows.Shapes;
using System.Windows.Threading;

namespace CustomCommandBarCreator.ModelViews
{
    [Serializable]
    public class CommandBar : ControlItem
    {
        // private bool canGenerate = false;
        public event Action<string> NewMessageComming;
        private ObservableCollection<string> gmsPaths;
        private ObservableCollection<ICommand> avaliablesBuildCommands;
        public ObservableCollection<string> GmsPaths
        {
            get { return gmsPaths; }
            set
            {
                gmsPaths = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ICommand> AvaliablesBuildCommands
        {
            get { return avaliablesBuildCommands; }
            set
            {
                avaliablesBuildCommands = value;
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
        public Dispatcher Dispatcher { get; set; }
        private string name;
        public new string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
                GenerateCommand.RaiseCanExecuteChanged();
                GenerateCommandOld.RaiseCanExecuteChanged();
                CreateSetupCommand.RaiseCanExecuteChanged();
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
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

                OnPropertyChanged();
            }
        }
        private bool? attached = false;

        public bool? Attached
        {
            get { return attached; }
            set
            {
                attached = value;

                if (value == null)
                    AttachButtonText = "Please Wait!";
                else if (value == true)
                    AttachButtonText = "Deattach";
                else
                    AttachButtonText = "Attach in a CorelDRW";

                OnPropertyChanged();
            }
        }
        private bool isAdmin;

        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; OnPropertyChanged(); }
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
            set
            {
                attachButtonText = value;
                OnPropertyChanged();
            }
        }
        private int commandLeft = 100;

        public int CommandLeft
        {
            get { return commandLeft; }
            set { commandLeft = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CorelVersionInfo> CorelVersions { get; set; }
        public RelayCommand<CommandBar> GenerateCommand { get; set; }
        public RelayCommand<CommandBar> GenerateCommandOld { get; set; }
        public RelayCommand<CommandBar> CreateSetupCommand { get; set; }
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
            InitializeCommands();
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            IsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            FillCorelVersion();
            SetMessage(string.Format("Welcome | Version.: {0}", Assembly.GetExecutingAssembly().GetName().Version));
        }
        private void InitializeCommands()
        {
            GenerateCommand = new RelayCommand<CommandBar>(GenereteBar, CanGenereteBar, "With DataSource");
            GenerateCommandOld = new RelayCommand<CommandBar>(GenereteBarOld, CanGenereteBar, "Loaded GMS");
            CreateSetupCommand = new RelayCommand<CommandBar>(CreateSetup, CanGenereteBar, "Create a Setup (only DataSource)");
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
            InstallCommand = new RelayCommand<CorelVersionInfo>(Install, CanInstall);
            this.AvaliablesBuildCommands = new ObservableCollection<ICommand>();
            this.AvaliablesBuildCommands.Add(GenerateCommand);
            this.AvaliablesBuildCommands.Add(GenerateCommandOld);
            this.AvaliablesBuildCommands.Add(CreateSetupCommand);
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


            if (attached == false)
                return;
            try
            {
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
                GenerateCommandOld.RaiseCanExecuteChanged();
                CreateSetupCommand.RaiseCanExecuteChanged();
            }
            catch
            {
                this.Attached = false;
            }
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
            GenerateCommandOld.RaiseCanExecuteChanged();
            CreateSetupCommand.RaiseCanExecuteChanged();
        }
        public void InCorel(object corelApp)
        {
            app = corelApp;
            cdrVersion = (app as dynamic).VersionMajor;
            Version = string.Format("{0} {1}", (app as dynamic).Name, (app as dynamic).Version);
            this.Attached = true;
        }
        object app = null;
        private int cdrVersion;

        private void AttachCorelDRW(object obj)
        {
            if (Attached == null)
                return;
            if (attached == true)
            {
                try
                {
                    //(app as dynamic).Quit();
                    Marshal.ReleaseComObject(app);
                    app = null;
                    this.Attached = false;
                }
                catch { }
            }
            else
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        this.Attached = null;

                        Type pia_type = Type.GetTypeFromProgID("CorelDRAW.Application");
                        app = Activator.CreateInstance(pia_type);
                    }
                    catch
                    {
                        SetMessage("Failed to start CorelDraw");
                        this.Attached = false;
                    }
                    if (app != null)
                    {
                        try
                        {
                            this.Attached = true;

                            cdrVersion = (app as dynamic).VersionMajor;
                            Version = string.Format("{0} {1}", (app as dynamic).Name, (app as dynamic).Version);
                        }
                        catch
                        {
                            SetMessage("Failed to retrieve CorelDraw Version");
                        }
                        this.Attached = true;
                    }
                });
                thread.IsBackground = true;
                thread.Start();
            }
        }
        public void SendTo(object obj)
        {
            if (attached == true)
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
                    catch
                    {
                        SetMessage("Failed to send files to Addon Folder");
                    }
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
                SetMessage("Failed to Save a GMS Folder!");
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
            GenerateCommandOld.RaiseCanExecuteChanged();
            CreateSetupCommand.RaiseCanExecuteChanged();
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
            GenerateCommandOld.RaiseCanExecuteChanged();
            CreateSetupCommand.RaiseCanExecuteChanged();
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
            SetMessage("Starting process. Shortcuts:" + this.HaveShortcut);
            StructureGenerator generator = new StructureGenerator();
            generator.GeneratorMessage += (msg) =>
            {
                SetMessage(msg);
            };
            if (!generator.SelectFolderEmpty())
            {
                SetMessage("Invalid Folder!");
                return;
            }
            initializeBarGeneration(() =>
            {

                if (generator.CreateBar(bar))
                {

                    resultFolder = generator.Folder;
                    if (cdrVersion >= CorelVersionInfo.MinVersion)
                    {
                        generator.CreateDataSource(resultFolder, cdrVersion);
                        SetMessage(string.Format("bar created successfully on folder \"{0}\"", resultFolder.Substring(resultFolder.LastIndexOf('\\') + 1)));
                    }
                    else
                    {
                        SetMessage("DataSource is not created, please use install button in administrator level and select your bar folder to makes a correct installation");
                    }
                }
                else
                    resultFolder = string.Empty;
            });
        }
        private void GenereteBarOld(CommandBar bar)
        {
            SetMessage("Starting process. Shortcuts:" + this.HaveShortcut);
            StructureGenerator generator = new StructureGenerator();
            generator.GeneratorMessage += (msg) =>
            {
                SetMessage(msg);
            };
            if (!generator.SelectFolderEmpty())
            {
                SetMessage("Invalid Folder!");
                return;
            }
            initializeBarGeneration(() =>
            {

                if (generator.CreateBarOld(bar))
                {

                    resultFolder = generator.Folder;
                    SetMessage(string.Format("bar created successfully on folder \"{0}\"", resultFolder.Substring(resultFolder.LastIndexOf('\\') + 1)));
                }
                else
                    resultFolder = string.Empty;
            });
        }
        private void CreateSetup(CommandBar bar)
        {
            SetMessage("Starting process. Shortcuts:" + this.HaveShortcut);
            StructureGenerator generator = new StructureGenerator();
            generator.GeneratorMessage += (msg) =>
            {
                SetMessage(msg);
            };
            if (!generator.SelectFolderEmpty())
            {
                SetMessage("Invalid Folder!");
                return;
            }
            generator.AllTasksFinish += () =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (generator.PrepareSetup(bar.Name))
                    {
                        SetMessage("Created Setup!");
                    }
                });
            };
            initializeBarGeneration(() =>
            {
                if (generator.CreateBar(bar))
                {
                    resultFolder = generator.Folder;
                    SetMessage(string.Format("bar created successfully on folder \"{0}\"", resultFolder.Substring(resultFolder.LastIndexOf('\\') + 1)));
                }
                else
                    resultFolder = string.Empty;
            });
        }
        private async void initializeBarGeneration(Action action)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (!string.IsNullOrEmpty(this[i].ShortcutText))
                {
                    this.HaveShortcut = true;
                    break;
                }
            }


            await Task.Run(action);
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
                        SetMessage(string.Format("{0} Salved!", filePath));
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
                    catch
                    {
                        SetMessage("Failed to load GMS Folder!");
                    }
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
        private void SetMessage(string msg)
        {
            NewMessageComming?.Invoke(msg);
        }
        private void Install(CorelVersionInfo version)
        {
            bool sucess = true;
            if (version == null)
                return;
            try
            {

                StructureGenerator generator = new StructureGenerator();
                generator.GeneratorMessage += (msg) =>
                    {
                        SetMessage(msg);
                    };


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
                                try
                                {
                                    SetMessage(string.Format("Copying the file {0}", files[i].Name));
                                    files[i].CopyTo(Path.Combine(addonDir.FullName, files[i].Name), true);
                                }
                                catch
                                {
                                    sucess = false;
                                    SetMessage(string.Format("Copy file:{0} failed!", files[i].Name));
                                }
                            }
                            if (sucess)
                                SetMessage("Installation is completed!");


                        };

                        generator.CreateDataSource(resultFolder, version.CorelVersion);


                    }
                    catch
                    {
                        sucess = false;
                    }
                }


            }
            catch
            {
                sucess = false;
            }


        }
        private bool canInstall = true;
        private bool CanInstall(CorelVersionInfo version)
        {
            return canInstall;
        }

        public string ContertToIcon(string imagePath)
        {
            string iconPath = Path.GetTempFileName();
            iconPath = iconPath.Replace(".tmp", ".ico");
            MultiIcon mIcon = new MultiIcon();
            SingleIcon sIcon = mIcon.Add(Path.GetFileName(imagePath));
            Image original = Bitmap.FromFile(imagePath);
            int size = 16;
            if (original.Width > original.Height)
                size = RoundDownToNearest(original.Width);
            else
                size = RoundDownToNearest(original.Height);
            System.Drawing.Bitmap bitmap16 = new Bitmap(original, size, size);
               
            sIcon.Add(bitmap16);
           if(size == 256)
                sIcon[0].IconImageFormat = IconImageFormat.PNG;
            mIcon.SelectedIndex = 0;
            mIcon.Save(iconPath, MultiIconFormat.ICO);
            //using (Bitmap bitmap = new Bitmap(imagePath))
            //{
            //    Icon icon = Icon.FromHandle(bitmap.GetHicon());
            //    using (System.IO.FileStream stream = new System.IO.FileStream(iconPath, System.IO.FileMode.Create,FileAccess.Write,FileShare.Write))
            //    {
            //        icon.Save(stream);
            //    }
            //}
            return iconPath;
        }
        int RoundDownToNearest(int number)
        {
            int[] values = { 16, 32, 48, 64, 128, 256 };

            int closest = values[0];
            foreach (int val in values)
            {
                if (val <= number)
                {
                    closest = val;
                }
                else
                {
                    break;
                }
            }
            return closest;
        }

    }
    enum FileType
    {
        ICON,
        GMS,
        BAR
    }

}
