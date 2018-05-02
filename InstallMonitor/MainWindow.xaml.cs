using System;
using System.Collections.Generic;
using System.IO;
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

namespace InstallMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
               if(file.Extension==".xml")
                {
                    file.Delete();
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<About>().Any())
            {
                About obj = new About();
                obj.Show();
            }
        }
    }
}
