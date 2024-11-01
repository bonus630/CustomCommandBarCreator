using CustomCommandBarCreator.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CustomCommandBarCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            if (e.Args.Length > 0)
            {
                Type pia_type = Type.GetTypeFromProgID(string.Format("CorelDRAW.Application.{0}", e.Args[0]));
                object corelApp = Activator.CreateInstance(pia_type);
                if (corelApp != null)
                {
                    var w = new MainWindow(corelApp);
                    w.Show();

                }
            }
            else
            {
                var w = new MainWindow();
                w.Show();
            }
        }
    }
}
