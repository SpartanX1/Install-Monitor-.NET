using System;
using System.IO;
using System.Windows.Threading;

namespace InstallMonitor.Helper
{
    class FileMonitor
    {
        FileMonitorView fileMonitorView;

        public FileMonitor()
        {
            fileMonitorView = new FileMonitorView();
        }
        public void Watch(string path)
        { 
            fileMonitorView.Show();
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                                            | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("New File created at " + e.FullPath);
            fileMonitorView.Dispatcher.Invoke(() =>
            {
                fileMonitorView.FileLog.Text += "New File created at " + e.FullPath+"\n";
            });
            
        }
    }
}
