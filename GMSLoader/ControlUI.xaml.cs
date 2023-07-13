using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using corel = Corel.Interop.VGCore;

namespace GMSLoader
{
    public partial class ControlUI : UserControl
    {
        public static corel.Application corelApp;
        private readonly string DataSource = "GMSLoaderDS";


        public ControlUI(object app)
        {

            try
            {
                corelApp = app as corel.Application;
                this.Loaded += ControlUI_Loaded;
                this.Unloaded += ControlUI_Unloaded;
                var dsf = new DataSource.DataSourceFactory();
                dsf.AddDataSource(DataSource, typeof(DataSource.GMSLoaderDataSource));
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
            catch
            {

            }
        }

        private void ControlUI_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
