﻿using System.IO;
using System.Text;


namespace CustomCommandBarCreator
{
    public class TargetsCreator
    {
        CorelVersionInfo[] versions;
        readonly string targetsName = "bonus630.CDRCommon.targets";
        public TargetsCreator()
        {
            versions = new CorelVersionInfo[CorelVersionInfo.MaxVersion - CorelVersionInfo.MinVersion];
            for (int i = 0; i < versions.Length; i++)
            {
                versions[i] = new CorelVersionInfo(i + CorelVersionInfo.MinVersion);
            }
        }
        public bool WriteTargetsFile(string projectDir,string barFolder)
        {
            try
            {
                string path = Path.Combine(projectDir, targetsName);
                if (File.Exists(path))
                    File.Delete(path);
                File.AppendAllText(path, buildProjectTargetString(barFolder));
                return true;
            }
            catch
            {
                return false;
            }
        }
        //Version 2.605
        private string buildProjectTargetString(string barFolder)
        {
            StringBuilder sr = new StringBuilder();

            sr.AppendLine("<Project DefaultTargets = \"Compile\" xmlns = \"http://schemas.microsoft.com/developer/msbuild/2003\">");
            sr.AppendLine("\t<Choose>\n");

            for (int i = 0; i < versions.Length; i++)
            {
                sr.AppendFormat("\t\t<When Condition=\"'$(Configuration)' == '{0} Release' or '$(Configuration)' == '{0} Debug'\">\n", versions[i].CorelAbreviation);
                sr.AppendLine("\t\t\t<PropertyGroup>\n");
                string corelPath = "";

                if (!versions[i].CorelInstallationNotFound)
                {
                    if (versions[i].Corel64Bit == CorelVersionInfo.CorelIs64Bit.Corel32)
                    {
                        versions[i].CorelAddonsPath(out corelPath);
                    }
                    else
                        versions[i].CorelAddonsPath64(out corelPath);
                    DirectoryInfo dir = new DirectoryInfo(corelPath);
                    if (dir.Exists)
                    {
                        corelPath = dir.Parent.FullName + "\\";
                    }
                    else
                    {
                        corelPath = "";
                    }
                }

                sr.AppendFormat("\t\t\t\t<CurrentCorelPath>{0}</CurrentCorelPath>\n", corelPath);
                sr.AppendFormat("\t\t\t\t<CurrentCorelAbr>{0}</CurrentCorelAbr>\n", versions[i].CorelAbreviation);
                sr.AppendFormat("\t\t\t\t<CurrentCorelDebugConst>{0}</CurrentCorelDebugConst>\n", versions[i].CorelDebugConst);
                sr.AppendLine("\t\t\t</PropertyGroup>");
                sr.AppendLine("\t\t</When>");
            }
            sr.AppendLine("\t</Choose>");
            sr.AppendLine("\t<PropertyGroup>");
            sr.AppendLine("\t\t<VGCoreDLL>Assemblies\\Corel.Interop.VGCore.dll</VGCoreDLL>");
            sr.AppendLine("\t</PropertyGroup>");
            sr.AppendLine("\t<Target Name=\"RenameFile\" AfterTargets=\"Build\"" +
                " Condition=\"'$(TemplateGuid)' == '{2005f455-f415-4dfb-a4de-d6d9562b3813}' " +
                "OR '$(TemplateGuid)' == '{3c372ba8-3752-489f-87f2-df27aa4850c8}' " +
                "OR '$(TemplateGuid)' == '{0AC96025-9E94-4F81-B6FD-C25731EED4A7}' " +
                "OR '$(TemplateGuid)' == '{6ff803ae-8506-4b82-bf92-7c531de81189}'\">");
            sr.AppendLine("\t\t<Message Text=\"Rename \'.dll\' file extension to \'.CorelAddon\' extension\" />");
            sr.AppendLine("\t\t<Message Text=\"$(CurrentCorelPath)\" />");
            sr.AppendLine("\t\t<Exec Condition=\"Exists('$(TargetDir)$(TargetName).CorelAddon')\" Command='del \"$(TargetDir)$(TargetName).CorelAddon\"' />");
            sr.AppendLine("\t\t<Exec Command='rename \"$(TargetPath)\" \"$(TargetName).CorelAddon\"'/>");
            sr.AppendLine("\t</Target>");
            sr.AppendLine("\t<Target Name=\"CopyFiles\" AfterTargets=\"Build\">");
            sr.AppendLine("\t\t<Message Text=\"CopyFiles\" />");
            sr.AppendLine("\t\t<Exec Condition=\"Exists('"+ barFolder + "')\" Command='xcopy \"$(ProjectDir)$(OutDir)GMSLoader.CorelAddon\" \"" + barFolder + "\" /y /d /e /c' />");
            sr.AppendLine("\t</Target>");
            sr.Append("</Project>");
            return sr.ToString();
        }
    }
}