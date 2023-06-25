using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using System.Windows.Shapes;


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
            InitializeComponent();

            commandBar = new ModelViews.CommandBar();
            commandBar.GmsPaths = new ObservableCollection<string>();
            commandBar.CommandItems = new System.Collections.ObjectModel.ObservableCollection<ModelViews.CommandItem>();
            this.DataContext = commandBar;

        }
        private void txt_PreviewKeyUP(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;



            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                tb.Clear();
                e.Handled = true;
                return;
            }
            if (e.Key == Key.LeftAlt || e.Key == Key.LeftCtrl || e.Key == Key.LeftShift)
            {
                e.Handled = true;
                return;
            }

            tb.Text = Enum.GetName(typeof(Key), e.Key);

        }
    }
}
