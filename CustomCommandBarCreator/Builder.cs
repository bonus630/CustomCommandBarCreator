
using System;
using System.Diagnostics;
using System.IO;

namespace CustomCommandBarCreator
{
    public class Builder
    {

        protected string msbuildPath;
        //private string loggerDll;
        //protected string loggerVariable;
        protected string toolsVersionVariable;
        public string ProjectPath { get; set; }
        private bool sucess = true;

        public event Action<string> DataReceived;
        public event Action<string> ErrorReceived;
        public event Action<bool> Finish;
        protected void SetMsBuildPath()
        {
            var ver = System.Environment.Version;
            string win = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Windows);
            string path = string.Format("{0}\\microsoft.net", win);
            string frame = "Framework64";
            if (!Directory.Exists(string.Format("{0}\\{1}", path, frame)))
                frame = "Framework";
            else if (!Directory.Exists(string.Format("{0}\\{1}", path, frame)))
                throw new Exception(".Net Framework not found");

            path = string.Format("{0}\\{1}\\v{2}.{3}.{4}", path, frame, ver.Major, ver.Minor, ver.Build);

            if (!File.Exists(string.Format("{0}\\MSBuild.exe", path)))
                throw new Exception("MSBuild not found");
            else
                msbuildPath = string.Format("{0}\\MSBuild.exe", path);

            //loggerDll = Path.Combine(Application.StartupPath, "MSBuildLogger.dll");
            //if (File.Exists(loggerDll))
            //    loggerVariable = string.Format(" /logger:\"{0}\"", loggerDll);
            FileInfo sol = new FileInfo(ProjectPath);
            //loggerDll = Path.Combine(sol.Directory.Parent.FullName, "MSBuildLogger.dll");
            //if (File.Exists(loggerDll))
            //    loggerVariable = string.Format(" /logger:\"{0}\"", loggerDll);
            toolsVersionVariable = "";// string.Format(" /tv:{0}.{1}", ver.Major, ver.Minor);


        }
        public void StartMSBuild(string projectFileName, string configurationName)
        {
            if (string.IsNullOrEmpty(msbuildPath))
                SetMsBuildPath();

            Process psi = new Process();
            //ProcessStartInfo psi = new ProcessStartInfo();
            psi.StartInfo.CreateNoWindow = false;
            psi.StartInfo.UseShellExecute = false;
            psi.EnableRaisingEvents = true;
            psi.StartInfo.FileName = msbuildPath;
           // psi.StartInfo.Arguments = string.Format("\"{0}\" /p:Configuration=\"{1}\" /v:d /nologo /noconsolelogger{2}"
           //     , projectFileName, configurationName, toolsVersionVariable);
            psi.StartInfo.Arguments = string.Format("\"{0}\" /p:Configuration=\"{1}\""
                , projectFileName, configurationName, toolsVersionVariable);
          //  psi.StartInfo.RedirectStandardOutput = true;
          //  psi.StartInfo.RedirectStandardError = true;
       
            psi.ErrorDataReceived += Psi_ErrorDataReceived;
            psi.Exited += Psi_Exited;
            psi.Start();
            //psi.BeginOutputReadLine();
           // psi.BeginErrorReadLine();
            //psi.WaitForExit();
            //psi.CancelOutputRead();
            //psi.CancelErrorRead();

        }

        private void Psi_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (ErrorReceived != null)
                ErrorReceived(e.Data);
        }

        protected virtual void OnFinish()
        {
            if (Finish != null)
                Finish(sucess);
        }
        protected virtual void Psi_Exited(object sender, EventArgs e)
        {
            OnFinish();
        }

        //private void R_OutputDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    if (e.Data == null)
        //        return;
        //    if (e.Data.StartsWith("MSG"))
        //    {
        //        if (DataReceived != null)
        //            DataReceived(e.Data.Replace("MSG:", ""));
        //    }
        //    if (e.Data.StartsWith("SUCESS"))
        //    {
        //        bool finish = Boolean.TryParse(e.Data.Replace("SUCESS:", ""), out sucess);
        //    }
        //    if (e.Data.StartsWith("ERROR"))
        //    {
        //        if (ErrorReceived != null)
        //            ErrorReceived(e.Data);
        //    }



        //}




        private string[] configuration = new string[] { "X7 Release", "X8 Release", "2017 Release", "2018 Release", "2019 Release", "2020 Release", "2021 Release", "2022 Release" };
        public int CorelVersion { protected get; set; }
        public void Run()
        {
            //StartMSBuild(string.Format("/nologo /noconsolelogger /logger:{2}, \"{3}\", Version = {4}, Culture = neutral \"{0}\" /p:Configuration=\"{1}\" /m" ,
            //    SolutionPath, configuration[CorelVersion - 17], "PackInstaller.MSBuildLogger", Application.ExecutablePath, Application.ProductVersion));
            StartMSBuild(ProjectPath, CorelVersionInfo.GetCorelAbreviation(CorelVersion) + " Release");

            if (string.IsNullOrEmpty(msbuildPath))
                SetMsBuildPath();

        }

    }
}