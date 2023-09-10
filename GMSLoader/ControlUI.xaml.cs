using System;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;
namespace $DataSourceName$
{
    public partial class ControlUI : UserControl
    {
        public static corel.Application corelApp;
        private readonly string DataSource = "$DataSourceName$";
        public ControlUI(object app)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                corelApp = app as corel.Application;
                this.Unloaded += ControlUI_Unloaded;
                var dsf = new DataSource.DataSourceFactory();
                dsf.AddDataSource(DataSource, typeof(DataSource.$DataSourceName$DataSource));
                dsf.Register();
            }
            catch
            {
                global::System.Windows.MessageBox.Show("VGCore Erro");
            }
        }
        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = args.Name;
            if (name.Contains("Corel.Interop.VGCore"))
            {
                string codeBase = typeof($DataSourceName$.ControlUI).Assembly.CodeBase;
                string vgCoreDllPath = string.Format("{0}\\Assemblies\\Corel.Interop.VGCore.dll",Directory.GetParent(Path.GetDirectoryName(codeBase.Substring(8))).Parent.FullName);
                Assembly asm = Assembly.LoadFile(vgCoreDllPath);
                return asm;
            }
            return args.RequestingAssembly;
        }
        private void ControlUI_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                corel.DataSourceProxy dsp = corelApp.FrameWork.Application.DataContext.GetDataSource(DataSource);
                dsp.InvokeMethod("UnloadGMS");
            }
            catch{ }
        }
    }
}
