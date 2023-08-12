using CustomCommandBarCreator.ModelViews;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using Vestris.ResourceLib;

namespace CustomCommandBarCreator.Models
{
    public class StructureGenerator
    {
        public string Folder { get; protected set; }
        readonly int StartIconId = 100;
        
        public event Action BuildDataSourceFinish;
        public event Action AllTasksFinish;
        public event Action<string> GeneratorMessage;
        public string DsName { get; protected set; }


        public bool CreateBar(CommandBar bar)
        {
            bool result = false;
            try
            {


                if (!SelectFolderEmpty())
                {
                    GeneratorMessage?.Invoke("Invalid Folder!");
                    return false;
                }
                GeneratorMessage?.Invoke("Creating the \"CorelAddon\" file");
                CreateCorelAddon();
                GeneratorMessage?.Invoke("Creating the \"XSLT\" files");
                CreateXSLT(bar);
                GeneratorMessage?.Invoke("Coping GMS files");
                CopyGMS(bar);
                GeneratorMessage?.Invoke("Merging icons");
                InsertIcons(bar);
                GeneratorMessage?.Invoke("Creating the \"Config\" file");
                CreateConfigXml(bar);
                GeneratorMessage?.Invoke("Writing the \"Table\" file");
                WriteTable(bar);

                result = true;
            }
            catch { }

            return result;
        }
        public void CreateDataSource(string folder, int version)
        {
            string projectDir = UnzipFiles();
           
            ModifyProject(projectDir, folder); 
            CreateTargetFile(projectDir, folder,DsName);
            BuildDataSource(projectDir, version);
        }
        private void CreateXSLT(CommandBar commandBar)
        {
            XSLTGenerator generator = new XSLTGenerator(commandBar, this.Folder);
            generator.GenerateAppUI();
            DsName = generator.DsName;
            generator.GenerateUserUI();
        }
        public void CopyGMS(CommandBar bar)
        {
            for (int i = 0; i < bar.GmsPaths.Count; i++)
            {
                if (File.Exists(bar.GmsPaths[i]))
                {
                    //obj.Substring(obj.LastIndexOf("\\") + 1).Split('.')[0];
                    string fileName = bar.GmsPaths[i].Substring(bar.GmsPaths[i].LastIndexOf('\\')).Split('.')[0];
                    File.Copy(bar.GmsPaths[i], this.Folder + fileName);
                }
            }
        }
        private void InsertIcons(CommandBar bar)
        {
            string assembly = string.Format("{0}\\Resources.dll", this.Folder);
            File.WriteAllBytes(assembly, Properties.Resources.IconsResources);
            ushort iconMaxId = GetMaxIconId(assembly);

            int groupIconIdCounter = StartIconId;
            for (int i = 0; i < bar.Count; i++)

            {
                groupIconIdCounter++;
                IconDirectoryResource newIcon = new IconDirectoryResource(new IconFile(bar[i].IconPath));
                newIcon.Name.Id = new IntPtr(groupIconIdCounter);
                foreach (var icon in newIcon.Icons)
                {
                    icon.Id = ++iconMaxId;
                }
                bar[i].IconID = newIcon.Name.Id;
                newIcon.SaveTo(assembly);
            }

        }
        private static ushort GetMaxIconId(string assembly)
        {
            using (var info = new ResourceInfo())
            {
                info.Load(assembly);

                ResourceId groupIconId = new ResourceId(Kernel32.ResourceTypes.RT_GROUP_ICON);
                if (info.Resources.ContainsKey(groupIconId))
                {
                    return info.Resources[groupIconId].OfType<IconDirectoryResource>().Max(idr => idr.Icons.Max(icon => icon.Id));
                }
            }
            return 0;
        }
        private bool SelectFolderEmpty()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a empty Folder";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string path = fbd.SelectedPath;
                string[] files = Directory.GetFiles(path);
                if (files.Length > 0)
                {
                    if (MessageBox.Show("Clear this folder?", "Attention!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        for (int i = 0; i < files.Length; i++)
                        {
                            try
                            {
                                File.Delete(files[i]); 
                              
                            }
                            catch { }
                        }

                    }
                   
                    return false;
                }
                if(Directory.GetFiles(path).Length > 0)
                    return false;
                else
                {
                    Folder = fbd.SelectedPath;
                    return true;
                }
               
            }
            return false;
        }
        public string SelectBarFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select your bar folder";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string path = fbd.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] fi = di.GetFiles("Coreldrw.addon");
                if (fi.Length == 0)
                {
                    MessageBox.Show("Invalid Directory!");
                    SelectBarFolder();
                }
                return path;


            }
            return string.Empty;
        }
        private void WriteTable(CommandBar bar)

        {
            string name = "table.89";


            using (FileStream fs = File.OpenWrite(string.Format("{0}\\{1}", Folder, name)))
            {
                byte[] size = BitConverter.GetBytes(bar.Count);
                int position = 0;
                fs.Write(size, 0, size.Length);
                position += 4;
                for (int i = 0; i < bar.Count; i++)
                {
                    size = BitConverter.GetBytes(0);
                    fs.Write(size, 0, 4);
                    position += 4;
                }
                for (int i = 0; i < bar.Count; i++)
                {
                    size = Encoding.UTF8.GetBytes(string.Format("{0}${1}", bar[i].GmsPath, bar[i].Command));
                    fs.Write(size, 0, size.Length);
                    position += size.Length;
                    size = BitConverter.GetBytes(size.Length);
                    fs.Position = (i + 1) * 4;
                    fs.Write(size, 0, 4);
                    fs.Position = fs.Length;
                }
            }
        }

        private void CreateConfigXml(CommandBar commandBar)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<config>");
            sb.AppendLine("<resources>");
            sb.Append("<resource name=\"");
            sb.Append(Guid.NewGuid().ToString());
            sb.AppendLine("\" path=\"Resources.dll\">");

            sb.AppendLine("<resourceMap>");

            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("<resEntry id=\"");
                sb.Append(commandBar[i].Guid);
                sb.Append("\" icon=\"");
                sb.Append(commandBar[i].IconID);
                sb.AppendLine("\"/>");
            }

            sb.AppendLine("</resourceMap>");

            sb.AppendLine("</resource>");
            sb.AppendLine("</resources>");
            sb.AppendLine("</config>");

            File.WriteAllText(string.Format("{0}\\config.xml", this.Folder), sb.ToString());
        }
        private void CreateCorelAddon()
        {
            FileStream fs = File.Create(string.Format("{0}\\Coreldrw.addon", this.Folder));
            fs.Close();
            fs.Dispose();

        }
        private string UnzipFiles()
        {


            string extractFolder = Path.GetTempPath();
            string projectDir = Path.Combine(Path.GetTempPath(), "GMSLoader");
            if (Directory.Exists(projectDir))
                Directory.Delete(projectDir, true);
            Directory.CreateDirectory(projectDir);

            string zip = string.Format("{0}GMSLoader.zip", extractFolder);
            File.WriteAllBytes(zip, Properties.Resources.GMSLoader);
            using (ZipArchive archive = new ZipArchive(File.OpenRead(zip)))
            {
                var entries = archive.Entries;
                foreach (ZipArchiveEntry item in entries)
                {
                    using (Stream stream = item.Open())
                    {
                        if (!string.IsNullOrEmpty(item.Name))
                        {
                            string dir = item.FullName.Replace(item.Name, "");
                            if (!Directory.Exists(Path.Combine(Path.GetTempPath(), dir)))
                                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), dir));

                            byte[] buffer = new byte[item.Length];
                            stream.Read(buffer, 0, buffer.Length);
                            string path = string.Format("{0}{1}", extractFolder, item.FullName);
                            using (FileStream fs = File.Create(path))
                            {
                                fs.Write(buffer, 0, buffer.Length);
                            }
                        }

                    }
                }

            }
            return projectDir;

        }
        private void ModifyProject(string projectDir, string folder)
        {
            //$DataSourceName$
            if (string.IsNullOrEmpty(DsName))
                DsName = XSLTGenerator.GetDataSourceName(string.Format("{0}\\AppUI.xslt", folder));

            string[] files = { "GMSLoader.csproj", "ControlUI.xaml.cs", "ControlUI.xaml", "Properties\\AssemblyInfo.cs"
            ,"DataSource\\BaseDataSource.cs","DataSource\\DataSourceFactory.cs","DataSource\\GMSLoaderDataSource.cs"};
            int[][] ids = { new int[] { 15, 16, 205 }, new int[] { 3, 8, 17 }, new int[] { 0, 5 }, new int[] { 7, 11 }
            , new int[] { 10 }, new int[] { 6 }, new int[] { 7, 11,15,49 }};

            for (int i = 0; i < files.Length; i++)
            {
                UpdateFiles(projectDir, files[i], ids[i]);
            }
          
            if(!File.Exists(string.Format("{0}\\DataSource\\{1}DataSource.cs", projectDir, DsName)))
                File.Copy(string.Format("{0}\\DataSource\\GMSLoaderDataSource.cs", projectDir),string.Format("{0}\\DataSource\\{1}DataSource.cs",projectDir,DsName));  


        }
        private void UpdateFiles(string projectDir,string path, int[] lineId)
        {
            path = string.Format("{0}\\{1}", projectDir,path);
            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lineId.Length; i++)
            {
                lines[lineId[i]] = lines[lineId[i]].Replace("$DataSourceName$", DsName);
            }
            File.WriteAllLines(path, lines);
        }
        private void CreateTargetFile(string projectDir, string barFolder, string dsName)
        {
            TargetsCreator tc = new TargetsCreator();
            tc.WriteTargetsFile(projectDir, barFolder,dsName);
        }
        private void BuildDataSource(string projectDir, int corelVersion)
        {
            Thread t = new Thread(() =>
            {
                Builder builder = new Builder();
                builder.ProjectPath = string.Format("{0}\\GMSLoader.csproj", projectDir);
                builder.CorelVersion = corelVersion;
                builder.ErrorReceived += (erro) =>
                {
                    GeneratorMessage?.Invoke(erro);
                };
                builder.DataReceived += (data) =>
                {
                    GeneratorMessage?.Invoke(data);
                };
                builder.Finish += (b) =>
                {
                    if (b)
                    {
                       
                        GeneratorMessage?.Invoke("Build DataSource completed");
                        BuildDataSourceFinish?.Invoke();
                    }
                };
                GeneratorMessage?.Invoke("Starting DataSource build");
                try
                {
                    builder.Run();
                }
                catch(Exception ex) 
                {
                    GeneratorMessage?.Invoke(ex.Message);
                }

            });
            t.IsBackground = true;
            t.Start();
        }


    }
}

