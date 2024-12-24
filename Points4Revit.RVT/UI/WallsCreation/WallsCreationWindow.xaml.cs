using Autodesk.Revit.DB;
using Newtonsoft.Json;
using Points4Revit.Core;
using Points4Revit.RVT.RevitService;
using System.IO;
using System.Text;
using System.Windows;

namespace Points4Revit.RVT.UI.WallsCreation
{
    public partial class WallsCreationWindow : Window
    {
        private readonly Document document;
        private readonly FileSystemWatcher pointFileSystemWatcher;
        private readonly string pathTempFile;

        public WallsCreationWindow(Document document)
        {
            InitializeComponent();
            this.document = document;

            DirectoryInfo parentDir = Directory.GetParent(Common.pathToTmpFile);
            pathTempFile = Path.Combine(parentDir.Parent.FullName, "p4r");

            pointFileSystemWatcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(pathTempFile),
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                Filter = Path.GetFileName(pathTempFile),
                EnableRaisingEvents = true
            };
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            pointFileSystemWatcher.Changed += new FileSystemEventHandler(OnPointFileChanged);
            DataContext = new WallsCreationDataContext();
        }

        private void WindowClosed(object sender, System.EventArgs e)
        {
            pointFileSystemWatcher.Changed -= new FileSystemEventHandler(OnPointFileChanged);
            Utils.WindowsHandler.DisposeWCW();
        }

        private int fileSystemWatcherCounter = 0;
        private void OnPointFileChanged(object sender, FileSystemEventArgs e)
        {
            fileSystemWatcherCounter++;
            if (fileSystemWatcherCounter == 2)
            {
                var objectData = string.Empty;
                try
                {
                    using (StreamReader sr = new StreamReader(pathTempFile, Encoding.Default))
                    {
                        objectData = sr.ReadToEnd();
                        sr.Close();
                    }

                    base.Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrEmpty(objectData))
                            if (JsonConvert.DeserializeObject<ObjectData>(objectData) is ObjectData od)
                                CreateWallByPoints.Commit(document, (WallsCreationDataContext)DataContext, od);
                    });
                }
                catch { }

                fileSystemWatcherCounter = 0;
            }
        }
    }
}
