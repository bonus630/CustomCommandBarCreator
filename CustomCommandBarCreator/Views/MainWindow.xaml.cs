using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;



namespace CustomCommandBarCreator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ModelViews.CommandBar commandBar;

        public MainWindow()
        {
            string vestris = "Vestris.ResourceLib.dll";
            string appPath  = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+"\\"+vestris;
            if (!File.Exists(appPath))
                File.WriteAllBytes(appPath,Properties.Resources.Vestris_ResourceLib);
            InitializeComponent();

            commandBar = new ModelViews.CommandBar();
            commandBar.GmsPaths = new ObservableCollection<string>();
            commandBar.CommandItems = new System.Collections.ObjectModel.ObservableCollection<ModelViews.CommandItem>();
            this.DataContext = commandBar;
         
            //object obj = System.Runtime.InteropServices.Marshal.GetActiveObject("CorelDRAW.Application.18");
        }
        private void txt_PreviewKeyUP(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            //if(tb.Text.Length==1)
            //{
            //    tb.Text = Enum.GetName(typeof(Key), e.Key);
            //    e.Handled = true;
            //    return;
            //}
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                tb.Clear();
                e.Handled = true;
                return;
            }
            if (e.Key == Key.LeftAlt || e.Key == Key.LeftCtrl || e.Key == Key.LeftShift)
            {
                tb.Text = "CTRL+";
                e.Handled = true;
                return;
            }

            //tb.CaretIndex = 0;
            //if (tb.Text.Length > 0)
            //{
            //    tb.Text = tb.Text.Substring(0, 1);

            //}

            tb.Text = Enum.GetName(typeof(Key), e.Key).ToUpper();



        }
    }
}
