using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        int MAX_LEVEL = 100;
        List<FolderOrFile>? folderOrFiles;

        private long sizeOfDrive;

        public long SizeOfDrive
        {
            get { return sizeOfDrive; }
            set
            {
                sizeOfDrive = value;
                OnPropertyChanged("SizeOfDrive");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    folderOrFiles = Search(startPath, 0, out long rootSize);
                    SizeOfDrive = rootSize;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        treeView.ItemsSource = folderOrFiles;
                        SetStatus(Status.Completed);
                    });

                    stopwatch.Stop();
                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    MessageBox.Show($"Elapsed time: {elapsedTime.Minutes} minutes and {elapsedTime.Seconds} seconds");
                });
            }
            else
                MessageBox.Show("Drive must be selected!");

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
                    string fileName = fileInfo.FullName;
                    long fileLength = fileInfo.Length;
                    result.Add(new FolderOrFile(false, fileName, fileLength, null));
                    filesSize += fileLength;
                }

                foreach (var directoryPath in Directory.GetDirectories(startPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    var children = Search(directoryPath, level + 1, out long childrenSize);
                    string directoryFullName = directoryInfo.FullName;
                    result.Add(new FolderOrFile(true, directoryFullName, childrenSize, children));
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

        private List<FolderOrFile> OrderBySize(List<FolderOrFile> folderOrFiles)
        {
            List<FolderOrFile> result = new List<FolderOrFile>();

            if (folderOrFiles == null)
                return result;
            else
            {
                result = folderOrFiles.OrderByDescending(x => x.Size).ToList();

                foreach (FolderOrFile? folderOrFile in result)
                {
                    if (folderOrFile != null && folderOrFile.IsFolder && folderOrFile.Children != null)
                        folderOrFile.Children = OrderBySize(folderOrFile.Children);
                }
            }

            return result;
        }
    }
}
