using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;
namespace GMSLoader
{
    public partial class ControlUI : UserControl
    {
        public static corel.Application corelApp;
        private readonly string DataSource = "$DataSourceName$";
        public ControlUI(object app)
        {
            try
            {
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
