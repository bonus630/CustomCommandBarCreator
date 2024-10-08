﻿using CustomCommandBarCreator.ModelViews;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CustomCommandBarCreator.Models
{
    public class XSLTGenerator
    {
        private readonly string[] Flags = new string[] { "$itemsUser$", "$itemsApp$", "$GuidA$", "$Caption$", "$itemsRef$", "$Shortcuts$","$GuidB$","$Folder$", "$DataSourceName$","$Customizations$","$itemDS$","$itemRefDS$" };

        string itemsUser = "", itemsApp = "", GuidA = "", Caption = "", itemsRef = "", Shortcuts = "",GuidB ="",Folder="",Customizations ="";

        private CommandBar commandBar;
        private string folder;
        public string DsName { get; protected set; }
        private string dsItemApp= "<itemData guid=\"$GuidB$\"\ntype=\"wpfhost\"\nhostedType=\"Addons\\$Folder$\\$DataSourceName$.CorelAddon,$DataSourceName$.ControlUI\"\nenable=\"true\"/>";
        private string dsItemRefApp = "<item  guidRef=\"$GuidB$\"/>";
        
        public XSLTGenerator(CommandBar commandBar, string folder)
        {
            this.commandBar = commandBar;
            this.folder = folder;
            this.GuidA = commandBar.Guid;
            this.GuidB = Guid.NewGuid().ToString();
            this.Folder = (new DirectoryInfo(folder)).Name;
            this.Caption = commandBar.Name;
            this.Shortcuts = generateShortcut();
            DsName = RandomNameGenarator();
            Customizations = generateCustomization();
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
           // appui = appui.Replace(Flags[6], this.GuidB);
            //appui = appui.Replace(Flags[7], this.Folder);
            appui = appui.Replace(Flags[3], Caption);
            appui = appui.Replace(Flags[4], itemsRef);
            appui = appui.Replace(Flags[5], Shortcuts);
            appui = appui.Replace(Flags[8], DsName);
            appui = appui.Replace(Flags[9], Customizations);

            dsItemApp = dsItemApp.Replace(Flags[6], this.GuidB);
            dsItemApp = dsItemApp.Replace(Flags[7], this.Folder);
            dsItemApp = dsItemApp.Replace(Flags[8], DsName);

            dsItemRefApp = dsItemRefApp.Replace(Flags[6], this.GuidB);

            appui = appui.Replace(Flags[10], dsItemApp);
            appui = appui.Replace(Flags[11], dsItemRefApp);

            File.WriteAllText(path, appui);
        }
        public void GenerateAppUIOld()
        {

            string path = string.Format("{0}\\AppUI.xslt", folder);
            string appui = Properties.Resources.AppUI;
            itemsApp = generateItemAPP();
            itemsRef = generateItemREF();

            appui = appui.Replace(Flags[1], itemsApp);
            appui = appui.Replace(Flags[2], this.GuidA);
            appui = appui.Replace(Flags[3], Caption);
            appui = appui.Replace(Flags[4], itemsRef);
            appui = appui.Replace(Flags[5], Shortcuts);
            appui = appui.Replace(Flags[9], Customizations);
            appui = appui.Replace(Flags[10], "");
            appui = appui.Replace(Flags[11], "");

            File.WriteAllText(path, appui);
        }

        private string generateItemAPP()
        {
            StringBuilder sb = new StringBuilder();
           
            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("\t\t\t\t\t<itemData guid=\"");
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

    //< itemData type = "button"  onInvoke = "*Bind(DataSource=GMSLoaderDS;Path=LoadGMG)"

    //guid = "a51aba1c-836c-411d-bea3-e75d1057fb0e"  userCaption = "Teste"

    //icon = "guid://a51aba1c-836c-411d-bea3-e75d1057fb0e" enable = 'true' />
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < commandBar.Count; i++)
            {
          
                sb.Append("\t\t\t\t\t<itemData guid=\"");
                sb.Append(commandBar[i].Guid);
                sb.Append("\" onInvoke = \"*Bind(DataSource=");
                sb.Append(DsName);
                sb.Append(";Path=LoadGMS");
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
        private string generateCustomization(bool old = false)
        {
            //Podemos colocar um icone na barra de commandos????
            string firstGuid = commandBar.Guid;
            //if (old)
            //    firstGuid = commandBar[1].Guid;
            //else
            //    firstGuid = this.GuidB;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\t<xsl:template match=\"/uiConfig/customizationList/container\">");
            sb.AppendLine("\t\t<xsl:copy>");
            sb.AppendLine("\t\t\t<xsl:apply-templates select=\"node()|@*\"/>");
            sb.Append("\t\t\t<modeData guid=\"");
            sb.Append(firstGuid);
            sb.AppendLine("\" >");
            for (int i = 0; i < commandBar.Count; i++)
            {
                sb.Append("\t\t\t\t<!-- ");
                sb.Append(commandBar[i].Command);
                sb.AppendLine(" -->");
                sb.Append("\t\t\t\t<item guidRef=\"");
                sb.Append(commandBar[i].Guid);
                sb.AppendLine("\" />");

            }
            sb.AppendLine("\t\t\t</modeData>");
            sb.AppendLine("\t\t</xsl:copy>");
            sb.AppendLine("\t</xsl:template>");
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
            sb.AppendLine("\t<xsl:template match=\"uiConfig/shortcutKeyTables/table[@tableID='bc175625-191c-4b95-9053-756e5eee26fe']\">");
            sb.AppendLine("\t\t<xsl:copy>");
            sb.AppendLine("\t\t\t<xsl:apply-templates select=\"node()|@*\"/>");
       

            for (int i = 0; i < commandBar.Count; i++)
            {
                if (commandBar[i].Shortcuts.Length == 0)
                    continue;
                sb.Append("\t\t\t<xsl:if test=\"not(./keySequence[@itemRef='");
                sb.Append(commandBar[i].Guid);
                sb.AppendLine("'])\">");
                sb.Append("\t\t\t\t<keySequence itemRef=\"");
                sb.Append(commandBar[i].Guid);
                sb.AppendLine("\">");


                for (int s = 0; s < commandBar[i].Shortcuts.Length; s++)
                {


                    sb.Append("\t\t\t\t\t<key");
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

                sb.AppendLine("\t\t\t\t</keySequence>");
                sb.AppendLine("\t\t\t</xsl:if>");


            }

            sb.AppendLine("\t\t</xsl:copy>");
            sb.AppendLine("\t</xsl:template>");


            return sb.ToString();
        }
        public static string GetDataSourceName(string appUiPath)
        {
            string text = File.ReadAllText(appUiPath);
            Regex rg = new Regex(@"Bind\(DataSource=(?<dataSourceName>GMS([A-H0-9]{6})DS);", RegexOptions.Compiled);
            Match match = rg.Match(text);

            string dsName = match.Result("${dataSourceName}");
            return dsName;


        }
        private string RandomNameGenarator()
        {
           // $DataSourceName$
            StringBuilder sb = new StringBuilder();

            sb.Append("GMS");

            char[] chars = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            Random Rd = new Random();

            for (int i = 0; i < 6; i++) {
               sb.Append(chars[Rd.Next(0, chars.Length)]);
            }

            sb.Append("DS");
            return sb.ToString();

        }
    }
}
