using CustomCommandBarCreator.ModelViews;
using System;
using System.IO;
using System.Text;

namespace CustomCommandBarCreator.Models
{
    public class XSLTGenerator
    {
        private readonly string[] Flags = new string[] { "$itemsUser$", "$itemsApp$", "$GuidA$", "$Caption$", "$itemsRef$", "$Shortcuts$","$GuidB$","$Folder$" };

        string itemsUser = "", itemsApp = "", GuidA = "", Caption = "", itemsRef = "", Shortcuts = "",GuidB ="",Folder="";

        private CommandBar commandBar;
        private string folder;

        public XSLTGenerator(CommandBar commandBar, string folder)
        {
            this.commandBar = commandBar;
            this.folder = folder;
            this.GuidA = commandBar.Guid;
            this.GuidB = Guid.NewGuid().ToString();
            this.Folder = (new DirectoryInfo(folder)).Name;
            this.Caption = commandBar.Name;
            this.Shortcuts = generateShortcut();
        }

        public void GenerateUserUI()
        {
            string path = string.Format("{0}\\UserUI.xslt", folder);
            string userui = Properties.Resources.UserUI;
            itemsUser = generateItemUSER();


            userui = userui.Replace(Flags[2], this.GuidA);
            userui = userui.Replace(Flags[6], this.GuidB);
            userui = userui.Replace(Flags[0], itemsUser);
            userui = userui.Replace(Flags[5], Shortcuts);

            File.WriteAllText(path, userui);







        }
        public void GenerateAppUI()
        {

            string path = string.Format("{0}\\AppUI.xslt", folder);
            string appui = Properties.Resources.AppUI;
            itemsApp = generateItemAPP2();
            itemsRef = generateItemREF();

            appui = appui.Replace(Flags[1], itemsApp);
            appui = appui.Replace(Flags[2], this.GuidA);
            appui = appui.Replace(Flags[6], this.GuidB);
            appui = appui.Replace(Flags[7], this.Folder);
            appui = appui.Replace(Flags[3], Caption);
            appui = appui.Replace(Flags[4], itemsRef);
            appui = appui.Replace(Flags[5], Shortcuts);

            File.WriteAllText(path, appui);
        }

        private string generateItemAPP()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("<itemData guid=\"");
                sb.Append(commandBar[i].Guid);
                sb.Append("\" dynamicCommand = \"");
                sb.Append(commandBar[i].Command);
                sb.Append("\" dynamicCategory = \"2cc24a3e-fe24-4708-9a74-9c75406eebcd\" userCaption=\"");
                sb.Append(commandBar[i].Caption);
                sb.Append("\" icon=\"guid://");
                sb.Append(commandBar[i].Guid);
                sb.Append("\" enable=\'");
                sb.Append(commandBar[i].EnableCondition);
                sb.AppendLine("\' />");

            }
            return sb.ToString();
        }
        private string generateItemAPP2()
        {

    //            < itemData type = "button"  onInvoke = "*Bind(DataSource=GMSLoaderDS;Path=LoadGMG)"

    //guid = "a51aba1c-836c-411d-bea3-e75d1057fb0e"  userCaption = "Teste"

    //icon = "guid://a51aba1c-836c-411d-bea3-e75d1057fb0e" enable = 'true' />
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("<itemData guid=\"");
                sb.Append(commandBar[i].Guid);
                sb.Append("\" onInvoke = \"*Bind(DataSource=GMSLoaderDS;Path=LoadGMS");
                sb.Append(i.ToString("000"));
                sb.Append(")\" type = \"button\" userCaption=\"");
                sb.Append(commandBar[i].Caption);
                sb.Append("\" icon=\"guid://");
                sb.Append(commandBar[i].Guid);
                sb.Append("\" enable=\'");
                sb.Append(commandBar[i].EnableCondition);
                sb.AppendLine("\' />");

            }
            return sb.ToString();
        }

        private string generateItemUSER()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("<xsl:if test=\"not(./item[@guidRef=\'");
                sb.Append(commandBar[i].Guid);
                sb.AppendLine("'])\">");
                sb.Append("<item guidRef=\"");
                sb.Append(commandBar[i].Guid);
                sb.AppendLine("\" />");
                sb.AppendLine("</xsl:if>");
            }

            return sb.ToString();
        }
        private string generateItemREF()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("<item  guidRef=\"");
                sb.Append(commandBar[i].Guid);
                sb.AppendLine("\" dock=\"top\"/>");
            }

            return sb.ToString();
        }

        private string generateShortcut()
        {
            if (!commandBar.HaveShortcut)
                return "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<xsl:template match=\"uiConfig/shortcutKeyTables/table[@tableID='bc175625-191c-4b95-9053-756e5eee26fe']\">");
            sb.AppendLine("\t\t<xsl:copy>");
            sb.AppendLine("\t\t\t<xsl:copy-of select=\"@*\"/>");


            for (int i = 0; i < commandBar.Count; i++)
            {
                if (commandBar[i].Shortcuts.Length == 0)
                    continue;
                sb.Append("\t\t<keySequence itemRef=\"");
                sb.Append(commandBar[i].Guid);
                sb.AppendLine("\">");


                for (int s = 0; s < commandBar[i].Shortcuts.Length; s++)
                {


                    sb.Append("\t\t<key");
                    if (commandBar[i].Shortcuts[s].Control)
                        sb.Append(" ctrl=\"true\"");
                    if (commandBar[i].Shortcuts[s].Shift)
                        sb.Append(" shift=\"true\"");
                    if (commandBar[i].Shortcuts[s].Alt)
                        sb.Append(" alt=\"true\"");
                    sb.Append(">");
                    if (commandBar[i].Shortcuts[s].Key.Length > 1)
                        sb.Append("VK_");
                    sb.Append(commandBar[i].Shortcuts[s].Key);
                    sb.AppendLine("</key>");
                }



                sb.AppendLine("\t\t</keySequence>");
            }

            sb.AppendLine("\t\t</xsl:copy>");
            sb.AppendLine("</xsl:template>");


            return sb.ToString();
        }
    }
}
