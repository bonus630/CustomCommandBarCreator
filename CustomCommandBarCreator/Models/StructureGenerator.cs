using CustomCommandBarCreator.ModelViews;
using SetupCreator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.IconLib;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
//using System.Windows.Shapes;
using System.Xml.Linq;
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


                //if (!SelectFolderEmpty())
                //{
                //    GeneratorMessage?.Invoke("Invalid Folder!");
                //    return false;
                //}
                GeneratorMessage?.Invoke("Creating the \"CorelAddon\" file");
                CreateCorelAddon();
                GeneratorMessage?.Invoke("Creating the \"XSLT\" files");
                CreateXSLT(bar);
                GeneratorMessage?.Invoke("Coping GMS files");
                CopyGMS(bar, true);
                GeneratorMessage?.Invoke("Merging icons");
                if (!InsertIcons(bar))
                    return false;
                GeneratorMessage?.Invoke("Merging captions");
                InsertCaptionStrings(bar);
                GeneratorMessage?.Invoke("Creating the \"Config\" file");
                CreateConfigXml(bar);
                GeneratorMessage?.Invoke("Writing the \"Table\" file");
                WriteTable(bar);
                AllTasksFinish?.Invoke();
                result = true;
            }
            catch { }

            return result;
        }
        public bool CreateBarOld(CommandBar bar)
        {
            bool result = false;
            try
            {
                //if (!SelectFolderEmpty())
                //{
                //    GeneratorMessage?.Invoke("Invalid Folder!");
                //    return false;
                //}
                GeneratorMessage?.Invoke("Creating the \"CorelAddon\" file");
                CreateCorelAddon();
                GeneratorMessage?.Invoke("Creating the \"XSLT\" files");
                CreateXSLTOld(bar);
                GeneratorMessage?.Invoke("Coping GMS files");
                CopyGMS(bar, false);
                GeneratorMessage?.Invoke("Merging icons");
                if (!InsertIcons(bar))
                    return false;
                GeneratorMessage?.Invoke("Merging captions");
                InsertCaptionStrings(bar);
                GeneratorMessage?.Invoke("Creating the \"Config\" file");
                CreateConfigXml(bar);
                AllTasksFinish?.Invoke();
                result = true;
            }
            catch
            {

            }
            return result;
        }
        public void CreateDataSource(string folder, int version)
        {
            string projectDir = UnzipFiles();

            ModifyProject(projectDir, folder);
            CreateTargetFile(projectDir, folder, DsName);
            BuildDataSource(projectDir, version);
        }
        public bool PrepareSetup(string barName)
        {
            string projectDir = UnzipFiles();
            ModifyProject(projectDir, this.Folder);
            string solutionFullName = ModifyProject2(projectDir, this.Folder, this.Folder);

            Creator creator = new Creator(solutionFullName);
            SetupSettings setup = new SetupSettings(new FileInfo(solutionFullName));
            if (setup.ShowDialog() == DialogResult.OK)
            {
                creator.SetupFolder = setup.Settings.SetupFolder;
                if (string.IsNullOrEmpty(creator.SetupFolder))
                    return false;
                creator.LogoPath = setup.Settings.LogoPath;
                creator.IconPath = setup.Settings.IconPath;
                creator.Readme = setup.Settings.ReadmePath;
                creator.Author = setup.Settings.Author;
                creator.Email = setup.Settings.Email;
#if (Donate || Debug)
                creator.OpenSite = setup.Settings.OpenSite;
                creator.URL = setup.Settings.URL;
#endif
            }
            else
            {

                return false;

            }

            try
            {
                creator.CallPacker();
                creator.Finish += Builder_Finish;
                creator.Builder.DataReceived += (msg) =>
                {
                    GeneratorMessage?.Invoke(msg);
                };
            }
            catch (Exception ex)
            {


            }


            return true;


        }

        private void Builder_Finish(string obj)
        {
            GeneratorMessage?.Invoke(obj);
        }

        private void CreateXSLT(CommandBar commandBar)
        {
            XSLTGenerator generator = new XSLTGenerator(commandBar, this.Folder);
            generator.GenerateAppUI();
            DsName = generator.DsName;
            generator.GenerateUserUI();
        }
        private void CreateXSLTOld(CommandBar commandBar)
        {
            XSLTGenerator generator = new XSLTGenerator(commandBar, this.Folder);
            generator.GenerateAppUIOld();
            generator.GenerateUserUI();
        }
        public void CopyGMS(CommandBar bar, bool newBar = true)
        {
            for (int i = 0; i < bar.GmsPaths.Count; i++)
            {
                if (File.Exists(bar.GmsPaths[i]))
                {
                    //obj.Substring(obj.LastIndexOf("\\") + 1).Split('.')[0];
                    string fileName = bar.GmsPaths[i].Substring(bar.GmsPaths[i].LastIndexOf('\\'));
                    if (newBar)
                        fileName = fileName.Split('.')[0];

                    File.Copy(bar.GmsPaths[i], this.Folder + fileName);
                }
            }
        }
        private void InsertCaptionStrings(CommandBar bar)
        {
            string assembly = string.Format("{0}\\Resources.dll", this.Folder);
            string name = string.Empty;
            int blockId = 10000;
            int stringId = 1;
            //string pre = "20";
            StringResource sr = new StringResource()
            {
                Name = new ResourceId(StringResource.GetBlockId(blockId))
            };

            ///sr.Name = new ResourceId(StringResource.GetBlockId(blockId));
            // name = "20000";
            // name = ("1000");
            sr[10000] = bar.Name;
            for (int i = 1; i <= bar.Count; i++)
            {
                if (stringId >= 16)
                {
                    sr.SaveTo(assembly);

                    sr = new StringResource();
                    blockId += 10000;
                    sr.Name = new ResourceId(StringResource.GetBlockId(blockId));
                    stringId = 1;
                }
                ushort index = ushort.Parse((blockId / 1000).ToString() + stringId.ToString("000"));
                sr[index] = bar[i - 1].Caption;
                bar[i - 1].CaptionID = index;
                stringId++;
            }
            sr.SaveTo(assembly);
            //sr[ushort.Parse("20016")] = "bosta";

        }
        public string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            string normalizedText = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedText)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark &&
                    (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)))
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        private bool InsertIcons(CommandBar bar)
        {
            string currentIcon = "";
            bool result = false;
            try
            {
                string assembly = string.Format("{0}\\Resources.dll", this.Folder);
                File.WriteAllBytes(assembly, Properties.Resources.IconsResources);
                ushort iconMaxId = GetMaxIconId(assembly);

                int groupIconIdCounter = StartIconId;
                groupIconIdCounter++;
                currentIcon = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\CDRCommandBarBuilder.ico";
                IconDirectoryResource newIcon = new IconDirectoryResource(new IconFile(currentIcon));
                newIcon.Name.Id = new IntPtr(groupIconIdCounter);
                foreach (var icon in newIcon.Icons)
                {
                    icon.Id = ++iconMaxId;
                }

                // bar[i].IconID = newIcon.Name.Id;

                int security = 0;
                do
                {
                    Thread.Sleep(100);
                    Debug.WriteLine(security);
                    security++;
                }
                while (!File.Exists(assembly) && security < 10);
                newIcon.SaveTo(assembly);




                for (int i = 0; i < bar.Count; i++)
                {
                    GeneratorMessage?.Invoke(string.Format("Merging \"{0}\"", currentIcon.Substring(currentIcon.LastIndexOf('\\') + 1)));
                    
                    currentIcon = RewriteIcon(bar[i].IconPath);
                    if (string.IsNullOrEmpty(currentIcon))
                    {
                        result = false;
                        MessageBox.Show(string.Format("Unable to rewrite the file {0}", Path.GetFileName(bar[i].IconPath)));
                    }
                    groupIconIdCounter++;
                    newIcon = new IconDirectoryResource(new IconFile(currentIcon));
                    newIcon.Name.Id = new IntPtr(groupIconIdCounter);
                    foreach (var icon in newIcon.Icons)
                    {
                        icon.Id = ++iconMaxId;
                    }
                    bar[i].IconID = newIcon.Name.Id;
                    try
                    {
                        newIcon.SaveTo(assembly);
                    }
                    catch
                    {
                        Thread.Sleep(100);
                        newIcon.SaveTo(assembly);
                    }


                }
                result = true;
            }
            catch (System.ComponentModel.Win32Exception wEx)
            {
                GeneratorMessage?.Invoke("There was a failure in merging the icons, the bar will not function properly.");
                MessageBox.Show(wEx.Message + " " + currentIcon);
            }
            return result;

        }


        private ushort GetMaxIconId(string assembly)
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
        private string RewriteIcon(string imagePath)
        {
            string path = Path.GetTempFileName();
            path = path.Replace(".tmp", ".ico");
            try
            {
                MultiIcon mIcon = new MultiIcon();
                using (Stream iconStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    mIcon.Load(iconStream);
                }
                if (mIcon[0][mIcon[0].Count-1].Icon.Width == 256)
                    mIcon[0][mIcon[0].Count-1].IconImageFormat = IconImageFormat.PNG;
                mIcon.Save(path, MultiIconFormat.ICO);
            }
            catch (Exception ex) { path = string.Empty; }

            return path;

        }

        public bool SelectFolderEmpty()
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
                    else
                        return false;
                }
                if (Directory.GetFiles(path).Length > 0)
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

            sb.AppendLine("<!-- CommandBar Caption -->");
            sb.Append("<resEntry id=\"");
            sb.Append(commandBar.Guid);
            sb.Append("\" string=\"");
            sb.Append("10000");

            sb.AppendLine("\"/>");

            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("<!-- ");
                sb.Append(commandBar[i].Command);
                sb.AppendLine(" -->");
                sb.Append("<resEntry id=\"");
                sb.Append(commandBar[i].Guid);
                sb.Append("\" string=\"");
                sb.Append(commandBar[i].CaptionID.ToString());
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
#if DEBUG
            // System.Diagnostics.Process.Start(projectDir);
#endif
            return projectDir;

        }
        private void ModifyProject(string projectDir, string folder)
        {
            //$DataSourceName$
            if (string.IsNullOrEmpty(DsName))
                DsName = XSLTGenerator.GetDataSourceName(string.Format("{0}\\AppUI.xslt", folder));

            string[] files = { "GMSLoader.csproj", "ControlUI.xaml.cs", "ControlUI.xaml", "Properties\\AssemblyInfo.cs"
            ,"DataSource\\BaseDataSource.cs","DataSource\\DataSourceFactory.cs","DataSource\\GMSLoaderDataSource.cs"};
            int[][] ids = { new int[] { 13, 14, 151 }, new int[] { 6, 11, 20,33 }, new int[] { 0, 5 }, new int[] { 7, 11 }
            , new int[] { 10 }, new int[] { 6 }, new int[] { 7, 11,15,49 }};

            for (int i = 0; i < files.Length; i++)
            {
                UpdateFiles(projectDir, files[i], ids[i]);
            }

            if (!File.Exists(string.Format("{0}\\DataSource\\{1}DataSource.cs", projectDir, DsName)))
                File.Copy(string.Format("{0}\\DataSource\\GMSLoaderDataSource.cs", projectDir), string.Format("{0}\\DataSource\\{1}DataSource.cs", projectDir, DsName));


        }
        private string ModifyProject2(string projectDir, string path, string barName)
        {
            try
            {
                barName = path.Substring(path.LastIndexOf('\\') + 1);
                string solutionDir = string.Format("{0}{1}", Path.GetTempPath(), barName);
                try
                {
                    if (Directory.Exists(solutionDir))
                        Directory.Delete(solutionDir, true);
                    Directory.CreateDirectory(solutionDir);
                }
                catch { }
                Directory.Move(projectDir, string.Format("{0}\\GMSLoader", solutionDir));
                path = string.Format("{0}\\{1}.sln", solutionDir, barName);
                string solution = Properties.Resources.Bar;
                solution = solution.Replace("$ProjectName$", barName);
#if DEBUG
                //   System.Diagnostics.Process.Start(solutionDir);
#endif


                File.WriteAllText(path, solution);
                string[] files = Directory.GetFiles(this.Folder);
                List<string> contentText = new List<string>();
                contentText.Add("<ItemGroup>");
                for (int i = 0; i < files.Length; i++)
                {
                    string name = files[i].Substring(files[i].LastIndexOf('\\') + 1);
                    File.Move(files[i], string.Format("{0}\\GMSLoader\\{1}", solutionDir, name));
                    contentText.Add("<Content Include=\"" + name + "\">");
                    contentText.Add("<CopyToOutputDirectory>Always</CopyToOutputDirectory>");
                    contentText.Add("</Content>");
                }
                contentText.Add("</ItemGroup>");

                string csproj = string.Format("{0}\\GMSLoader\\GMSLoader.csproj", solutionDir);
                List<string> projectText = new List<string>(File.ReadAllLines(csproj));
                projectText.InsertRange(projectText.Count - 2, contentText);

                string[] lines = new string[] {
                "<Target Name=\"CopyFiles\" AfterTargets=\"Build\">",
                "<MakeDir Directories=\"$(CurrentCorelPath)Addons\\$(SolutionName)\" />",
                "<Exec Condition=\"Exists('$(CurrentCorelPath)Addons\\$(SolutionName)')\" Command='xcopy \"$(ProjectDir)$(OutDir)*.*\" \"$(CurrentCorelPath)Addons\\$(SolutionName)\" /y /d /e /c' />",
                "</Target>"};
                projectText.InsertRange(projectText.Count - 2, lines);

                File.Delete(csproj);
                File.WriteAllLines(csproj, projectText);
            }
            catch { }
            return path;

        }
        private void UpdateFiles(string projectDir, string path, int[] lineId)
        {
            path = string.Format("{0}\\{1}", projectDir, path);
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
            tc.WriteTargetsFile(projectDir, barFolder, dsName);
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
                catch (Exception ex)
                {
                    GeneratorMessage?.Invoke(ex.Message);
                }

            });
            t.IsBackground = true;
            t.Start();
        }
        private void ApplyFolderIcon(string targetFolderPath, string localIcon = "", string description = "")
        {
            if (string.IsNullOrEmpty(localIcon) && string.IsNullOrEmpty(description))
                return;
            if (!Directory.Exists(targetFolderPath))
                return;
            var iniPath = Path.Combine(targetFolderPath, "desktop.ini");
            if (File.Exists(iniPath))
            {
                //remove hidden and system attributes to make ini file writable
                File.SetAttributes(
                   iniPath,
                   File.GetAttributes(iniPath) &
                   ~(FileAttributes.Hidden | FileAttributes.System));
            }
            string iconPath = string.Format("{0}\\icon.ico", targetFolderPath);
            if (!File.Exists(iconPath) && !string.IsNullOrEmpty(localIcon))
            {
                File.Copy(localIcon, iconPath, true);
                File.SetAttributes(iconPath, File.GetAttributes(iconPath) | FileAttributes.Hidden | FileAttributes.System);
            }
            //create new ini file with the required contents
            StringBuilder iniContents = new StringBuilder();
            iniContents.AppendLine("[.ShellClassInfo]");
            iniContents.AppendLine("ConfirmFileOp=0");
            if (!string.IsNullOrEmpty(localIcon))
            {
                iniContents.AppendLine(string.Format("IconResource={0},0", iconPath));
                iniContents.AppendLine(string.Format("IconFile={0}", iconPath));
                iniContents.AppendLine("IconIndex=0");
            }
            if (!string.IsNullOrEmpty(localIcon))
                iniContents.AppendLine(string.Format("InfoTip={0}", description));

            File.WriteAllText(iniPath, iniContents.ToString());

            //hide the ini file and set it as system
            File.SetAttributes(
               iniPath,
               File.GetAttributes(iniPath) | FileAttributes.Hidden | FileAttributes.System);
            //set the folder as system
            File.SetAttributes(
                targetFolderPath,
                File.GetAttributes(targetFolderPath) | FileAttributes.System);
        }


    }
}

