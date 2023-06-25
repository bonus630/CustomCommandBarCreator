using CustomCommandBarCreator.ModelViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vestris.ResourceLib;

namespace CustomCommandBarCreator.Models
{
    public class StructureGenerator
    {
        string folder;
        readonly int StartIconId = 100;
        public void CreateBar(CommandBar bar)
        {
            SelectFolder();
            CreateCorelAddon();
            CreateXSLT(bar);
            CopyGMS(bar);
            InsertIcons(bar);
            CreateConfigXml(bar);
            System.Diagnostics.Process.Start(folder);
        }

        private void CreateXSLT(CommandBar commandBar)
        {
            XSLTGenerator generator = new XSLTGenerator(commandBar, this.folder);
            generator.GenerateAppUI();
            generator.GenerateUserUI();
        }
        public void CopyGMS(CommandBar bar)
        {
            for (int i = 0; i < bar.GmsPaths.Count; i++)
            {
                if (File.Exists(bar.GmsPaths[i]))
                {
                    string fileName = bar.GmsPaths[i].Substring(bar.GmsPaths[i].LastIndexOf('\\'));
                    File.Copy(bar.GmsPaths[i],this.folder+fileName);
                }
            }
        }
        private void InsertIcons(CommandBar bar)
        {
            string assembly = string.Format("{0}\\Resources.dll",this.folder);
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
        private void SelectFolder()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select a empty Folder";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                string path = fbd.SelectedPath;
                if (Directory.GetFiles(path).Length > 0)
                {
                    SelectFolder();
                    return;
                }
                folder = fbd.SelectedPath;
            }
        }

        private void CreateConfigXml(CommandBar commandBar)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<config>");
            sb.AppendLine("<resources>");
            sb.AppendLine("<resource name=\"MyResource\" path=\"Resources.dll\">");
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

            File.WriteAllText(string.Format("{0}\\config.xml", this.folder), sb.ToString());
        }
        private void CreateCorelAddon()
        {
            File.Create(string.Format("{0}\\Coreldrw.addon", this.folder));
        }
    }
}
