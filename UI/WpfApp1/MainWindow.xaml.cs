using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    // myFileObject to store file and directory infos
    public class MyFileObject
    {
        private string _Name;
        private string _Path;
        private bool _IsFile;
        private string _ImgSrc;

        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }

        public string Path
        {
            get { return this._Path; }
            set { this._Path = value; }
        }

        public bool IsFile
        {
            get { return this._IsFile; }
            set { this._IsFile = value; }
        }

        public string ImgSrc
        {
            get { return this._ImgSrc; }
            set { this._ImgSrc = value; }
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            Loaded += OnMainWindowLoaded;            
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            ChangeView(new First_page());
        }

        public void ChangeView(Page view)
        {
            MainFrame.NavigationService.Navigate(view);
        }

        public void deletePreviousBMPFiles()
        {
            
            DirectoryInfo fileList = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] files = fileList.GetFiles();

            foreach(FileInfo file in files)
            {
                try
                {

                    if(file.Extension.ToUpper() == ".BMP")
                    {
                        file.Attributes = FileAttributes.Normal;
                        File.Delete(file.FullName);
                    }
                }
                catch { }
            }
        }
    }
}
