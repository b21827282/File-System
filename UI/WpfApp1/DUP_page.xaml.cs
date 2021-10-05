using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using System.Data;
using System.Data.SqlClient;
using WpfApp1.classes;
using System.ComponentModel;
using System.Collections;
using ceTe.DynamicPDF.Rasterizer;
using Path = System.IO.Path;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for DUP_page.xaml
    /// User can only select pdf files
    /// Files and folders are shown on filesTable, users can browse them on filePathTextBox and select files from filesTable via double click
    /// Selected files are added to the selectedFilesTable, selected files can be removed via double click and their order can be changed via drag drop.
    /// User can save selected file list with save button, saved list will be shown on selectedFilesTable after reopening the program
    /// </summary>
    public partial class DUP_page : Page
    {
        private string filePath = @"D:\";// file path string (D drive by default)

        MyFileObject currentlySelectedFileOrFolder; // selected folder or file

        object currentlySelectedQuestion; // selected question

        ObservableCollection<MyFileObject> selectedFileList = new ObservableCollection<MyFileObject>(); // stores selected files

        [JsonIgnore]
        ObservableCollection<object> selectedQuestionList = new ObservableCollection<object>(); // stores selected questions

        ObservableCollection<MyFileObject> currentPathItems = new ObservableCollection<MyFileObject>(); // currently selected path's items

        int currentPageNumber = 0; // number of the file that is currently viewing

        ArrayList bitmapImages = new ArrayList(); // bitmap images of the selected folders

        InputBox inputBox = new InputBox(); // input box for database connection 

        ArrayList databaseTableColumnsNames = new ArrayList();  // selected files' content as bitmap images

        // Icons' file paths
        string folderIcon = "pack://application:,,,/assets/folder.png";
        string jpgFileIcon = "pack://application:,,,/assets/jpg.png";
        string pngFileIcon = "pack://application:,,,/assets/png.jpg";
        string pdfFileIcon = "pack://application:,,,/assets/pdf.png";
        string pptFileIcon = "pack://application:,,,/assets/ppt.png";
        string mp2FileIcon = "pack://application:,,,/assets/mp2.png";
        string mp3FileIcon = "pack://application:,,,/assets/mp3.png";
        string mp4FileIcon = "pack://application:,,,/assets/mp4.png";
        string bmpFileIcon = "pack://application:,,,/assets/bmp.jpg";
        string gifFileIcon = "pack://application:,,,/assets/gif.png";
        string mpgFileIcon = "pack://application:,,,/assets/mpg.png";
        string aviFileIcon = "pack://application:,,,/assets/avi.png";
        string mkvFileIcon = "pack://application:,,,/assets/mkv.jpg";

        public DUP_page()
        {
            InitializeComponent();
            filePathSearchBox.Text = filePath;
            goToPathFromSearchBox();
            showSavedData();
        }

        private void DUP_Loaded(object sender, RoutedEventArgs e)
        {   
        }

        // go to the path that is written in search box
        private void goToPathFromSearchBox()
        {
            try
            {
                DirectoryInfo fileList = new DirectoryInfo(filePathSearchBox.Text);
                FileInfo[] files = fileList.GetFiles(); // all files from the path
                DirectoryInfo[] dirs = fileList.GetDirectories(); // all directoires from the path
                string fileExtension = ""; // file extension string

                // clear items from the previous path      
                filesTable.ItemsSource = null;
                currentPathItems.Clear();
                filesTable.Items.Clear();

                // add items from the new path
                for (int i = 0; i < files.Length; i++)
                {

                    fileExtension = files[i].Extension.ToUpper(); // convert file extensions letters to upper case
                    switch (fileExtension)
                    {
                        case ".PDF":
                            currentPathItems.Add(new MyFileObject { Name = files[i].Name, IsFile = true, Path = files[i].FullName, ImgSrc = pdfFileIcon });
                            break;

                        default:                         
                            break;
                    }
                }

                // add directories to filesTable
                for (int i = 0; i < dirs.Length; i++)
                {
                    currentPathItems.Add(new MyFileObject { Name = dirs[i].Name, IsFile = false, Path = dirs[i].FullName, ImgSrc = folderIcon });
                }

                // show items on filesTable
                filesTable.ItemsSource = currentPathItems;

                // collection view to filter
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(filesTable.ItemsSource);
                view.Filter = fileFilter;

            }

            // invalid input
            catch
            {
                MessageBox.Show("Cannot access this path");
            }
        }

       // filter listview
        private bool fileFilter(object item)
        {
            if (String.IsNullOrEmpty(filePathSearchBox.Text))
                return true;
            else
                return ((item as MyFileObject).Path.IndexOf(filePathSearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        // search box filter 
        private void filePathSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(filesTable != null && filesTable.ItemsSource != null)
                CollectionViewSource.GetDefaultView(filesTable.ItemsSource).Refresh();
        }

        // reads stored objects from DUP_data.json file and shows them on selectedFilesTable
        private void showSavedData()
        {
            try
            {
                // show saved files
                string json_files = File.ReadAllText(@"DUP_data.json");
                var storedData = JsonConvert.DeserializeObject<List<MyFileObject>>(json_files);
                foreach (MyFileObject obj in storedData)
                {
                    selectedFileList.Add(obj);
                }
                selectedFilesTable.DisplayMemberPath = "Name";
                selectedFilesTable.ItemsSource = selectedFileList;
            }
            catch
            {

            }       
        }

        // go back to first page
        private void Main_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.ChangeView(new First_page());
        }

        // enter key event on file path search box
        private void File_Search_Box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                goToPathFromSearchBox();      // shows directories and folders from written path in 
            }
        }

        // file path search button click event
        private void File_Search_Button_Click(object sender, RoutedEventArgs e)
        {
            goToPathFromSearchBox();

            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);
                    filePathSearchBox.Text = fbd.SelectedPath;
                    goToPathFromSearchBox();
                }
            }
        }

        // checks if the file is already selected
        public bool isFileAlreadySelected()
        {
            bool isSelected = false;
            if (selectedFilesTable.Items.Contains(currentlySelectedFileOrFolder))
            {
                isSelected = true;
            }
            return isSelected;
        }


        ////////////////////////  filesTable (Files and Folders Table)  ////////////////////////////////

        // filesTable item double click event (if it is a file add to list, if it is a folder show content)
        private void filesTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != filesTable)
            {
                if (obj.GetType() == typeof(ListViewItem))
                {
                    // if it is not in selectedFilesTable and it is a file add it
                    if (!isFileAlreadySelected() && currentlySelectedFileOrFolder.IsFile)
                    {
                        selectedFileList.Add(currentlySelectedFileOrFolder); // add to selected file list
                        selectedFilesTable.DisplayMemberPath = "Name";
                        selectedFilesTable.ItemsSource = selectedFileList; // assign selected file list to selectedFilesTable's items source 
                    }

                    // if selected item is a directory go to that path
                    if (!currentlySelectedFileOrFolder.IsFile)
                    {
                        filePathSearchBox.Text = currentlySelectedFileOrFolder.Path; // set search box text to currently selected path

                        goToPathFromSearchBox(); // go to the path written in search box

                        //if directory paths last char is not "\" in search box then add it
                        if(filePathSearchBox.Text.Substring(filePathSearchBox.Text.Length-1) != "\\")
                        {      
                            filePathSearchBox.Text += "\\";
                        }
                    }
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }

            // reset displayed page
            ImageViewer1.Source = null;
            currentPageNumber = 0;
            bitmapImages.Clear();
        }

        // selection changed handler on filesTable
        private void filesTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedFileOrFolder = (MyFileObject)filesTable.SelectedItem;
        }

        // go to previous path
        private void Directory_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            char slashChar = '\u005c'; // back slash character

            string path = filePathSearchBox.Text;

            int i = path.LastIndexOf(@"\"); // index of last back slash

            int count = path.Where(x => (x == slashChar)).Count(); // frequency of back slash in path

            if (i < 0) // if there is no slash character in string do nothing
            {
                return;
            }

            if (count > 1) // if there are more than 1 slash characters remove it
            {
                string newPath = path.Substring(0, i);
                filePathSearchBox.Text = newPath;
                goToPathFromSearchBox();
            }

            else // if there is only one slash character do not remove it
            {
                string newPath = path.Substring(0, i + 1);
                filePathSearchBox.Text = newPath;
                goToPathFromSearchBox();
            }
        }

        // selectedFilesTable drop event (while selecting folders)
        private void selectedFilesTable_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListViewItem)
            {
                MyFileObject droppedData = e.Data.GetData(typeof(MyFileObject)) as MyFileObject;
                MyFileObject target = ((ListViewItem)(sender)).DataContext as MyFileObject;

                int removedIdx = selectedFilesTable.Items.IndexOf(droppedData);
                int targetIdx = selectedFilesTable.Items.IndexOf(target);

                if (removedIdx < targetIdx)
                {
                    selectedFileList.Insert(targetIdx + 1, droppedData);
                    selectedFileList.RemoveAt(removedIdx);
                }
                else
                {
                    int remIdx = removedIdx + 1;
                    if (selectedFileList.Count + 1 > remIdx)
                    {
                        selectedFileList.Insert(targetIdx, droppedData);
                        selectedFileList.RemoveAt(remIdx);
                    }
                }
            }
            // reset displayed page
            ImageViewer1.Source = null;
            currentPageNumber = 0;
            bitmapImages.Clear();
        }

        // selectedFilesTable double click event (removes file from selected list)
        private void selectedFilesTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MyFileObject fileToBeRemoved = (MyFileObject)selectedFilesTable.SelectedItem;
            selectedFileList.Remove(fileToBeRemoved);
            selectedFilesTable.ItemsSource = selectedFileList;

            // reset displayed page
            ImageViewer1.Source = null;
            currentPageNumber = 0;
            bitmapImages.Clear();
        }

        // selectedFilesTable mouse move event for drag and drop functionality
        private void selectedFilesTable_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ListViewItem & e.LeftButton == MouseButtonState.Pressed)
            {
                ListViewItem draggedItem = sender as ListViewItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;

            }
        }

        // save button click (saves selected file objects to json file)
        private void Selected_Files_Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string strResultJson = JsonConvert.SerializeObject(selectedFileList);
            File.WriteAllText(@"DUP_data.json", strResultJson);
            MessageBox.Show("Saved"); 
        }

        // delete turkish letters
        public string StringReplace(string text)
        {
            text = text.Replace("İ", "I");
            text = text.Replace("ı", "i");
            text = text.Replace("Ğ", "G");
            text = text.Replace("ğ", "g");
            text = text.Replace("Ö", "O");
            text = text.Replace("ö", "o");
            text = text.Replace("Ü", "U");
            text = text.Replace("ü", "u");
            text = text.Replace("Ş", "S");
            text = text.Replace("ş", "s");
            text = text.Replace("Ç", "C");
            text = text.Replace("ç", "c");
            text = text.Replace(" ", "_");
            return text;
        }

        // view button click event
        private void View_Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow?.deletePreviousBMPFiles();

            for (int i = 0; i < selectedFilesTable.Items.Count; i++)
            {
                MyFileObject currentFile = (MyFileObject)selectedFilesTable.Items[i];

                if (currentFile.Path.Split('.')[1] == "pdf")
                {
                    int count = 2;
                    PdfRasterizer rasterizer = new PdfRasterizer(currentFile.Path);
                    string newFileName = StringReplace(currentFile.Name.Split('.')[0] + ".bmp");
                    rasterizer.Draw(newFileName, ImageFormat.Bmp, ImageSize.Dpi96);
                    string url = Path.GetFullPath(newFileName);
                    BitmapImage bitmap = new BitmapImage(new Uri(url));
                    bitmapImages.Add(bitmap);
                    string otherPagesUrl = url.Split('.')[0] + count + ".bmp";
                    while (File.Exists(otherPagesUrl))
                    {
                        bitmap = new BitmapImage(new Uri(otherPagesUrl));
                        bitmapImages.Add(bitmap);
                        count++;
                        otherPagesUrl = url.Split('.')[0] + count + ".bmp";

                    }
                }
               
                if(currentFile.Path.Split('.')[1] == "jpg" || currentFile.Path.Split('.')[1] == "png" || currentFile.Path.Split('.')[1] == "bmp" ||
                    currentFile.Path.Split('.')[1] == "gif" || currentFile.Path.Split('.')[1] == "mpg")
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(currentFile.Path);
                    bitmap.EndInit();
                    bitmapImages.Add(bitmap);
                }

                if (currentFile.Path.Split('.')[1] == "pptx")
                {
                    // could not convert pptx files to bitmap images
                }

                if (bitmapImages.Count > 0)
                    ImageViewer1.Source = (BitmapImage)bitmapImages[0];
            }


        }

        // display previous file content
        private void left_arrow_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageNumber - 1 >= 0)
            {
                currentPageNumber--;
                ImageViewer1.Source = (BitmapImage)bitmapImages[currentPageNumber];
            }


        }

        //display next file content
        private void right_arrow_Click(object sender, RoutedEventArgs e)
        {
            if (currentPageNumber + 1 < bitmapImages.Count)
            {
                currentPageNumber++;
                ImageViewer1.Source = (BitmapImage)bitmapImages[currentPageNumber];
            }
        }

        
        
        /////////////// DATABASE'S QUESTIONS TABLE ////////////////////

        // selection changed handler on questionsTable
        private void questions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentlySelectedQuestion = (object) questions.SelectedItem;
        }


        // questions table item double click event
        private void questions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != questions)
            {
                if (obj.GetType() == typeof(ListViewItem))
                {
                    selectedQuestionList.Add(currentlySelectedQuestion); // add to selected questions list
                    foreach(string columnNames in databaseTableColumnsNames)
                    {
                        selectedQuestions.DisplayMemberPath = columnNames;
                    }
                    selectedQuestions.ItemsSource = selectedQuestionList; // assign selected questions list to selectedQuestions's items source
                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }

        }


        ///////////////SELECTED QUESTIONS TABLE////////////////////

        // double click event (removes question from selected questions)
        private void selectedQuestions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            object questionToBeRemoved = (object)selectedQuestions.SelectedItem;
            selectedQuestionList.Remove(questionToBeRemoved);
            selectedQuestions.ItemsSource = selectedQuestionList;
        }

        // connect to database
        private void Connect_Click(object sender, RoutedEventArgs e)
        {          
            inputBox.ShowDialog();

            if (inputBox.OK_ButtonPressed)
            {
                try
                {
                    string tableName = inputBox.TableNameTextBox.Text;
                    DbClass.openConnection(inputBox.connectionStringTextBox.Text);

                    DbClass.sql = "SELECT * FROM " + tableName + ";"; // database query
                    DbClass.cmd.CommandType = CommandType.Text;
                    DbClass.cmd.CommandText = DbClass.sql;

                    DbClass.da = new SqlDataAdapter(DbClass.cmd);
                    DbClass.dt = new DataTable();
                    DbClass.da.Fill(DbClass.dt);

                    // get database table's column names
                    foreach (DataColumn column in DbClass.dt.Columns)
                    {
                        databaseTableColumnsNames.Add(column.ColumnName);
                    }
                    

                    questions.ItemsSource = DbClass.dt.DefaultView;

                    DbClass.closeConnection();
                }

                catch
                {
                    MessageBox.Show("Cannot connect to database");
                }


               
            }
            
        }

       
    }
}
