using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace FileSizeDetector
{
    enum Status
    {
        Ready,
        Started,
        Completed,
        Error
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int MAX_LEVEL = 100;
        List<FolderOrFile>? folderOrFiles;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }


        private List<string> GetDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            return drives.Select(x => x.Name).ToList();
        }
        private void Init()
        {
            SetStatus(Status.Ready);
            drive_cbox.ItemsSource = GetDrives();
            if (drive_cbox.Items.Count > 0)
                drive_cbox.SelectedIndex = 0;
        }

        private void start_btn_Click(object sender, RoutedEventArgs e)
        {
            string? startPath = drive_cbox.SelectedItem.ToString();
            if (startPath != null)
            {
                Task.Factory.StartNew(() =>
                {
                    folderOrFiles = Search(startPath, 0, out long rootSize);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        treeView.ItemsSource = folderOrFiles;
                        SetStatus(Status.Completed);
                    });
                });
            }
            else
                MessageBox.Show("Drive must be selected!");
            //MessageBox.Show("The searching task is started!");
            SetStatus(Status.Started);
        }

        private List<FolderOrFile>? Search(string startPath, int level, out long _childrenSize)
        {
            if (level == MAX_LEVEL)
            {
                _childrenSize = 0;
                return null;
            }

            List<FolderOrFile> result = new List<FolderOrFile>();
            long filesSize = 0;
            long directoriesSize = 0;

            try
            {
                foreach (var filePath in Directory.GetFiles(startPath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    result.Add(new FolderOrFile(false, fileInfo.Name, fileInfo.FullName, fileInfo.Length, null));
                    filesSize += fileInfo.Length;
                }

                foreach (var directoryPath in Directory.GetDirectories(startPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    var children = Search(directoryPath, level + 1, out long childrenSize);
                    result.Add(new FolderOrFile(true, directoryInfo.Name, directoryInfo.FullName, childrenSize, children));
                    directoriesSize += childrenSize;
                }

                _childrenSize = directoriesSize + filesSize;
            }
            catch (System.UnauthorizedAccessException)
            {
                _childrenSize = 0;
                return null;
            }


            return result.Count > 0 ? result : null;
        }

        private void SetStatus(Status status)
        {
            status_label.Content = $"Status: {status}";
            switch (status)
            {
                case Status.Ready:
                    start_btn.IsEnabled = true;
                    drive_cbox.IsEnabled = true;
                    orderBy_btn.IsEnabled = true;
                    break;
                case Status.Started:
                    start_btn.IsEnabled = false;
                    drive_cbox.IsEnabled = false;
                    orderBy_btn.IsEnabled = false;
                    break;
                case Status.Completed:
                    start_btn.IsEnabled = true;
                    drive_cbox.IsEnabled = true;
                    orderBy_btn.IsEnabled = true;
                    break;
                case Status.Error:
                    start_btn.IsEnabled = true;
                    drive_cbox.IsEnabled = true;
                    orderBy_btn.IsEnabled = true;
                    break;
                default:
                    break;
            }
        }

        private void orderBy_btn_Click(object sender, RoutedEventArgs e)
        {
            treeView.ItemsSource = OrderBySize(folderOrFiles);
        }

        private List<FolderOrFile>? OrderBySize(List<FolderOrFile>? folderOrFiles)
        {
            List<FolderOrFile>? result = new List<FolderOrFile>();

            if (folderOrFiles == null)
                return result;
            else
            {
                result = folderOrFiles.OrderBy(x => x.Size).ToList();

                //foreach (FolderOrFile? folderOrFile in folderOrFiles)
                //{
                //    if (folderOrFile != null && folderOrFile.IsFolder)
                //    {
                //        var orderedFolderOrFiles = OrderBySize(folderOrFiles);
                //        if (orderedFolderOrFiles != null)
                //            result.AddRange(orderedFolderOrFiles);
                //    }
                //    else if (folderOrFile != null)
                //        result.Add(folderOrFile);
                //}
            }

            return result.Count() > 0 ? result : null;
        }
    }
}
