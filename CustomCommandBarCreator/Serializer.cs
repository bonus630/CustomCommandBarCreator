using CustomCommandBarCreator.ModelViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CustomCommandBarCreator
{
    public  class Serializer
    {
        public static void Serialize(CommandBar bar,string path)
        {
         
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateNode(XmlNodeType.XmlDeclaration, "", ""));
            XmlNode mainNode = doc.CreateNode(XmlNodeType.Element, "CommandBar", "");

            XmlAttribute attribute = doc.CreateAttribute("version");
            attribute.Value = "2.0";

            mainNode.Attributes.Append(attribute);

            attribute = doc.CreateAttribute("name");
            attribute.Value = bar.Name;
            mainNode.Attributes.Append(attribute);

            XmlNode node = doc.CreateNode(XmlNodeType.Element, "GmsFiles", "");
            mainNode.AppendChild(node);
            for (int i = 0; i < bar.GmsPaths.Count; i++)
            {
                XmlNode n = doc.CreateNode(XmlNodeType.Element, "Gms", "");
                attribute = doc.CreateAttribute("id");
                attribute.Value = i.ToString();
                n.Attributes.Append(attribute);
                var p = doc.CreateTextNode(bar.GmsPaths[i]);
                n.AppendChild(p);
                node.AppendChild(n);
            }
            node = doc.CreateNode(XmlNodeType.Element, "Commands", "");
            mainNode.AppendChild(node);
            for (int i = 0; i < bar.Count; i++)
            {
                XmlNode n = doc.CreateNode(XmlNodeType.Element, "CommandItem", "");
                attribute = doc.CreateAttribute("id");
                attribute.Value = i.ToString();
                n.Attributes.Append(attribute);

                XmlNode item = doc.CreateNode(XmlNodeType.Element, "GmsFileId", "");
                item.InnerText = bar.GmsPaths.IndexOf(bar.GmsPaths.SingleOrDefault(r => r.Contains(bar[i].GmsPath))).ToString();
                n.AppendChild(item);

                item = doc.CreateNode(XmlNodeType.Element, "Caption", "");
                item.InnerText = bar[i].Caption;
                n.AppendChild(item);

                item = doc.CreateNode(XmlNodeType.Element, "Command", "");
                item.InnerText = bar[i].Command;
                n.AppendChild(item);


                item = doc.CreateNode(XmlNodeType.Element, "Enable", "");
                item.InnerText = bar[i].EnableCondition;
                n.AppendChild(item);

                item = doc.CreateNode(XmlNodeType.Element, "Shortcut", "");
                item.InnerText = bar[i].ShortcutText;
                n.AppendChild(item);

                item = doc.CreateNode(XmlNodeType.Element, "Icon", "");
                FileInfo iconPathInfo = new FileInfo(bar[i].IconPath);
                if (iconPathInfo.Extension != ".tmp" && iconPathInfo.Exists)
                    item.InnerText = bar[i].IconPath;
                n.AppendChild(item);

                node.AppendChild(n);
            }


            doc.AppendChild(mainNode);

            doc.Save(path);


        }
        public static void DeSerialize(CommandBar bar,string path)
        {
        
            if (!File.Exists(path))
                return;
            XmlDocument doc = new XmlDocument();
            doc.Load(path);


            bar.Name = doc.LastChild.Attributes["name"].Value;
            XmlNode gmsNode = doc.ChildNodes[1].ChildNodes[0];
            XmlNode commandsNode = doc.ChildNodes[1].ChildNodes[1];


            for (int i = 0; i < gmsNode.ChildNodes.Count; i++)
            {
                XmlNode node = gmsNode.ChildNodes[i];
                bar.CheckAndAddGmsFile(node.InnerText);
            }
            for (int i = 0; i < commandsNode.ChildNodes.Count; i++)
            {
                XmlNode node = commandsNode.ChildNodes[i];



                CommandItem ci = new CommandItem();

                ci.Caption = node.ChildNodes[1].InnerText;
                ci.Command = node.ChildNodes[2].InnerText;
               if(!string.IsNullOrEmpty(node.ChildNodes[5].InnerText))
                    ci.IconPath = node.ChildNodes[5].InnerText;
                ci.EnableCondition = node.ChildNodes[3].InnerText;
                int gmsid = -1;
                Int32.TryParse(node.ChildNodes[0].InnerText, out gmsid);
                if (gmsid > -1)
                    ci.GmsPath = gmsNode.SelectSingleNode($"//Gms[@id='{gmsid}']").InnerText;
                ci.ShortcutText = node.ChildNodes[4].InnerText;
                ci.Selected = false;

                bar.AddCommandItem(ci);

            }

        }
    }
}
